using Quartz;
using QuartzComponent;
using QuartzHelper;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuartzHandle
{
    public class QuartzBuilder : IQuartzBuilder
    {
        IOConfigurationReader configurationReader;
        private IQuartzSchedule QuartzSchedule
        {
            get;
            set;
        }

        internal QuartzBuilder(IQuartzSchedule quartzSchedule)
        {
            this.QuartzSchedule = quartzSchedule;
            configurationReader = IOConfigurationReader.GetInstance();
            ScanQuartzExterior();
        }

        internal QuartzBuilder(IQuartzSchedule quartzSchedule, IList<PlugModel> plugs)
        {
            this.QuartzSchedule = quartzSchedule;
            ScanQuartzExterior(plugs);
        }

        /// <summary>
        /// 两个问题有QuartzModel 有了对象还要这个做什么？二考虑把 QuartzModelList 这个分出去
        /// </summary>
        public bool Update(string jobName)
        {
            if (this.QuartzSchedule == null || string.IsNullOrEmpty(jobName))
            {
                return false;
            }

            string dllName = jobName + "QuartzTask.dll";
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, jobName, dllName);
            byte[] assemblyBuf = File.ReadAllBytes(path);
            Assembly assembly = Assembly.Load(assemblyBuf);

            Type type = assembly.GetType(dllName + "." + jobName);
            if (type == null) return false;
            object[] objs = type.GetCustomAttributes(typeof(ScanSignal), true);
            if (objs != null && objs.Length > 0)
            {
                JobBase job1 = Activator.CreateInstance(type) as JobBase;
                QuartzModel model = null;
                ScanSignal customScanSignal = objs[0] as ScanSignal;
                if (customScanSignal != null)
                {
                    IJobDetail job = JobBuilder.Create(type)
                                   .WithIdentity(customScanSignal.JobName, QuartzGroupLevel.GetGroupLevelName(customScanSignal.GroupLevel))
                                   .UsingJobData(new JobDataMap(job1.map))
                                   .Build();
                    TriggerBuilder triggerBuilder = TriggerBuilder.Create()
                            .WithIdentity(customScanSignal.JobName + "trigger", QuartzGroupLevel.GetGroupLevelName(customScanSignal.GroupLevel))
                            .StartNow();
                    if (String.IsNullOrEmpty(customScanSignal.CronExpression))
                    {
                        triggerBuilder.WithSimpleSchedule(x => x.WithInterval(customScanSignal.TimeSpan).RepeatForever());
                    }
                    else
                        triggerBuilder.WithCronSchedule(customScanSignal.CronExpression);
                    ITrigger trigger = triggerBuilder.Build();
                    model = new QuartzModel(customScanSignal.JobName, job, trigger);
                    return this.QuartzSchedule.UpdateJob(model, model) != null;
                }
            }
            return false;
        }
        /// <summary>
        /// 成功返回true ,false 失败有可能是已经存在或找不到jobname
        /// </summary>
        /// <param name="jobName"></param>
        /// <returns></returns>
        public IQuartzTask Add(string jobName)
        {
            if (this.QuartzSchedule == null || String.IsNullOrWhiteSpace(jobName))
            {
                return null;
            }
            string dllName = jobName + "QuartzTask.dll";
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, jobName, dllName);

            string plugName = string.Concat(jobName, "Plug.txt");
            string plugNamepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, jobName, plugName);
            var plugdic = configurationReader.Read(plugNamepath);
            byte[] assemblyBuf = File.ReadAllBytes(path);
            Assembly assembly = Assembly.Load(assemblyBuf);

            Type type = assembly.GetType(Path.GetFileNameWithoutExtension(dllName) + "." + jobName);
            if (type.GetInterface("IQuartzService") != null)
            {
                QuartzModel model = null;
                AssemblyDynamicLoader dyNamicLoader = new AssemblyDynamicLoader(jobName);
                dyNamicLoader.LoadAssembly(path);

                TaskBase task = new TaskBase();
                IDictionary<string, object> dic = task.map;
                dic.Add("DyNamicLoader", dyNamicLoader);
                dic.Add("DyNamicMethod", Path.GetFileNameWithoutExtension(dllName) + "." + jobName);
                string timePression = "";
                long timelongPression = 1000;
                bool bl = false;
                bl = plugdic.TryGetValue("CRONEXPRESSION", out timePression);
                if (!bl)
                {
                    if (plugdic.TryGetValue("TIMESPAN", out timePression))
                    {
                        if (!long.TryParse(timePression, out timelongPression))
                            timePression = "1000";
                    }
                }

                IJobDetail job = JobBuilder.Create(task.GetType())
                                  .WithIdentity(plugdic["JOBNAME"], plugdic["GROUPNAME"])
                                  .UsingJobData(new JobDataMap(dic))
                                  .Build();

                TriggerBuilder triggerBuilder = TriggerBuilder.Create()
                     .WithIdentity(plugdic["JOBNAME"] + "TRIGGER", plugdic["GROUPNAME"])
                     .StartNow();
                if (bl)
                {
                    triggerBuilder.WithCronSchedule(timePression);
                }
                else
                {
                    triggerBuilder.WithSimpleSchedule
                      (x => x.WithInterval(new TimeSpan(timelongPression))
                     .RepeatForever());
                }
                ITrigger trigger = triggerBuilder.Build();
                model = new QuartzModel(plugdic["JOBNAME"], job, trigger);
                if (!this.QuartzSchedule.CheckExists(model))
                {
                    this.QuartzSchedule.AddJob(model);
                }
            }
            return null;
        }

        public IQuartzTask Add(PlugModel plug)
        {
            if (this.QuartzSchedule == null || plug == null)
            {
                return null;
            }

            string runroot = AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.Combine(
                                            runroot,
                                            plug.DllRelativeDir,
                                            plug.DllIQuartzServiceImplementFile
                                          );
            if (!File.Exists(path))
                throw new ArgumentException("找不到需要载入的目录或文件", path);
            byte[] assemblyBuf = File.ReadAllBytes(path);
            Assembly assembly = null;
            if (assemblyBuf.Length > 0)
                assembly = Assembly.Load(assemblyBuf);
            else
                return null;
            Type[] types = assembly.GetTypes();
            try
            {
                foreach (var type in types)
                {
                    if (type.GetInterface("IQuartzService") != null)
                    {
                        QuartzModel model = null;
                        AssemblyDynamicLoader dyNamicLoader = new AssemblyDynamicLoader(plug.DllRelativeDir);
                        dyNamicLoader.LoadAssembly(path);

                        TaskBase task = new TaskBase();
                        IDictionary<string, object> dic = task.map;
                        dic.Add("DyNamicLoader", dyNamicLoader);
                        dic.Add("DyNamicMethod", type.FullName);
                        IJobDetail job = JobBuilder.Create(task.GetType())
                                             .WithIdentity(plug.JobName, plug.GroupName)
                                             .UsingJobData(new JobDataMap(dic))
                                             .Build();

                        TriggerBuilder triggerBuilder = TriggerBuilder.Create()
                             .WithIdentity(plug.JobName + "Trigger", plug.GroupName)
                             .StartNow();

                        if (!string.IsNullOrEmpty(plug.CronexPression))
                        {
                            triggerBuilder.WithCronSchedule(plug.CronexPression);
                        }
                        else
                        {
                            triggerBuilder.WithSimpleSchedule
                                  (x => x.WithInterval(new TimeSpan(0, 0, 0, 0, plug.TimeSpan))
                                 .RepeatForever());
                        }

                        ITrigger trigger = triggerBuilder.Build();
                        model = new QuartzModel(plug.JobName, job, trigger);
                        return Add(model);
                    }
                }
            }
            catch (Exception oe)
            {
                Debug.WriteLine(oe.Message);
            }
            return null;
        }

        private IQuartzTask Add(QuartzModel quartzModel)
        {
            if (!this.QuartzSchedule.CheckExists(quartzModel))
            {
                return this.QuartzSchedule.AddJob(quartzModel);
            }
            return null;
        }

        internal void ScanQuartzExterior()
        {
            string runroot = AppDomain.CurrentDomain.BaseDirectory;
            var subDir = Directory.GetDirectories(runroot);

            foreach (var dir in subDir)
            {
                var dinfo = new DirectoryInfo(dir);
                string dllName = string.Concat(dinfo.Name, "QuartzTask.dll");
                string path = Path.Combine(dir, dllName);
                string plugName = string.Concat(dinfo.Name, "Plug.txt");
                string plugNamepath = Path.Combine(dir, plugName);
                if (!File.Exists(plugNamepath)) continue;
                var plugdic = configurationReader.Read(plugNamepath);
                try
                {
                    byte[] assemblyBuf = File.ReadAllBytes(path);
                    Assembly assembly = Assembly.Load(assemblyBuf);
                    Type[] types = assembly.GetTypes();
                    foreach (var item in types)
                    {
                        if (item.GetInterface("IQuartzService") != null)
                        {
                            QuartzModel model = null;
                            AssemblyDynamicLoader dyNamicLoader = new AssemblyDynamicLoader(dinfo.Name);
                            dyNamicLoader.LoadAssembly(path);

                            TaskBase task = new TaskBase();
                            IDictionary<string, object> dic = task.map;
                            dic.Add("DyNamicLoader", dyNamicLoader);
                            dic.Add("DyNamicMethod", Path.GetFileNameWithoutExtension(dllName) + "." + dinfo.Name);
                            string timePression = "";
                            int timelongPression = 1000;
                            bool bl = false;
                            bl = plugdic.TryGetValue("CRONEXPRESSION", out timePression);
                            if (!bl)
                            {
                                if (plugdic.TryGetValue("TIMESPAN", out timePression))
                                {
                                    if (!int.TryParse(timePression, out timelongPression))
                                        timePression = "1000";
                                }
                            }

                            IJobDetail job = JobBuilder.Create(task.GetType())
                                              .WithIdentity(plugdic["JOBNAME"], plugdic["GROUPNAME"])
                                              .UsingJobData(new JobDataMap(dic))
                                              .Build();

                            TriggerBuilder triggerBuilder = TriggerBuilder.Create()
                                 .WithIdentity(plugdic["JOBNAME"] + "TRIGGER", plugdic["GROUPNAME"])
                                 .StartNow();
                            if (bl)
                            {
                                triggerBuilder.WithCronSchedule(timePression);
                            }
                            else
                            {
                                triggerBuilder.WithSimpleSchedule
                                  (x => x.WithInterval(new TimeSpan(0, 0, 0, 0, timelongPression))
                                 .RepeatForever());
                            }
                            ITrigger trigger = triggerBuilder.Build();
                            model = new QuartzModel(plugdic["JOBNAME"], job, trigger);
                            if (!this.QuartzSchedule.CheckExists(model))
                            {
                                this.QuartzSchedule.AddJob(model);
                            }
                        }
                    }
                }
                catch (Exception oe)
                {
                    Debug.WriteLine(oe.Message);
                }
            }
        }

        public void ScanQuartzExterior(IList<PlugModel> plugs)
        {
            if (plugs == null || plugs.Count < 0) throw new ArgumentNullException("载入的插件列表为空");
            foreach (var item in plugs)
            {
                if (Add(item) == null)
                {
                    throw new ArgumentNullException("在插入一个插件时出错" + item.JobName);
                }
            }
        }
    }
}
