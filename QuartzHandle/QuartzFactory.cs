using Quartz;
using Quartz.Impl;
using QuartzComponent;
using QuartzHelper;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartzHandle
{
    public class QuartzFactory
    {
        private QuartzBuilder builder = null;
        private IOConfigurationReader configurationReader;

        public Func<IList<PlugModel>> GetPlugs
        {
            get;
            private set;
        }

        public QuartzFactory(NameValueCollection quartzProperty, Func<IList<PlugModel>> getPlugs)
        {
            StdSchedulerFactory factory = null;
            if (quartzProperty != null)
                factory = new StdSchedulerFactory(quartzProperty);
            else
                factory = new StdSchedulerFactory();
            var scheduler = factory.GetScheduler();
            this.GetCurrentScheduer = new QuartzSchedule(scheduler);
            if (getPlugs != null)
                this.GetPlugs = getPlugs;
            else
                this.GetPlugs = GetFileModel;
        }

        public QuartzFactory(Func<IList<PlugModel>> getPlugs)
            : this(null, getPlugs)
        {
        }

        public QuartzSchedule GetCurrentScheduer
        {
            get;
            private set;
        }

        public void Start()
        {
            this.GetCurrentScheduer.Start();
            builder = new QuartzBuilder(this.GetCurrentScheduer, GetPlugs.Invoke());
        }

        public void Update(string jobName)
        {
            builder.Update(jobName);
        }


        public void AddJob(string jobName)
        {
            var list = GetPlugs.Invoke();
            var model = list.FirstOrDefault(x => x.JobName == jobName);
            builder.Add(model);
        }

        public ScheduleStatus GetServiceStatus()
        {
            return this.GetCurrentScheduer.GetScheduleStatus;
        }

        private IList<PlugModel> GetFileModel()
        {
            string runroot = AppDomain.CurrentDomain.BaseDirectory;
            var subDir = Directory.GetDirectories(runroot);
            IOConfigurationReader configurationReader = IOConfigurationReader.GetInstance();
            IList<PlugModel> list = new List<PlugModel>();
            foreach (var item in subDir)
            {
                string filename = new DirectoryInfo(item).Name;
                PlugModel model = new PlugModel();
                string plugNamepath = Path.Combine(item, string.Concat(filename, "Plug.txt"));
                if (!File.Exists(plugNamepath)) continue;
                var plugdic = configurationReader.Read(plugNamepath);
                model.JobName = plugdic["JOBNAME"];
                model.GroupName = plugdic["GROUPNAME"];
                string CronexPression = string.Empty;
                if (plugdic.TryGetValue("CRONEXPRESSION", out CronexPression))
                    model.CronexPression = CronexPression;
                else
                    model.TimeSpan = int.Parse(plugdic["TIMESPAN"]);
                model.DllRelativeDir = plugdic["JOBNAME"];
                model.DllIQuartzServiceImplementFile = string.Concat(plugdic["JOBNAME"], "QuartzTask.dll");
                list.Add(model);
            }
            return list;
        }
    }
}
