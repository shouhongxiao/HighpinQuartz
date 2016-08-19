using log4net;
using QuartzComponent;
using QuartzHandle;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace QuartzService
{
    public class QuartzBootStrap
    {
        public static string Monitoring
        {
            get
            {
                return ConfigurationManager.AppSettings["monitoring"] ?? "127.0.0.1:39152";
            }
        }

        public static void BootStrap()
        {
            ILog log = log4net.LogManager.GetLogger("loginfo");
            ILog logerr = log4net.LogManager.GetLogger("logerror");

            var properties = new NameValueCollection();
            properties["quartz.jobStore.clustered"] = "true";
            properties["quartz.scheduler.instanceId"] = "AUTO";
            //===持久化====
            // 存储类型
            properties["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz";
            //表明前缀
            properties["quartz.jobStore.tablePrefix"] = "QRTZ_";
            //驱动类型
            properties["quartz.jobStore.driverDelegateType"] = "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz";
            //数据源名称
            properties["quartz.jobStore.dataSource"] = "myDS";
            //连接字符串
            properties["quartz.dataSource.myDS.connectionString"] = @"Data Source = SHOU-PC\SQLEXPRESS;Initial Catalog = JobScheduler;Integrated Security = SSPI;";
            //sqlserver版本
            properties["quartz.dataSource.myDS.provider"] = "SqlServer-20";

            QuartzFactory quartzFactory = new QuartzFactory(null);
            quartzFactory.Start();

            if (!HttpListener.IsSupported)
            {
                logerr.Error("必须2003以上系统才可以");
            }
            // 注意前缀必须以 / 正斜杠结尾
            string[] prefixes = new string[] { Monitoring};
            IList<string> removed = new List<string>();
            // 创建监听器.
            HttpListener listener = new HttpListener();
            listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;//指定身份验证 Anonymous匿名访问
            // 增加监听的前缀.
            foreach (string s in prefixes)
            {
                listener.Prefixes.Add(s);
            }

            try
            {
                listener.Start();
                log.Info("监听中..." + Monitoring);
                //可以改成iocp模型 以下监控可以放到factory moniter中 监控代码改成cmd模式，页面用velcity做，
                Thread thread31 = new Thread(() =>
                {
                    while (true)
                    {
                        // 注意: GetContext 方法将阻塞线程，直到请求到达
                        HttpListenerContext context = listener.GetContext();
                        // 取得请求对象
                        HttpListenerRequest request = context.Request;
                        HttpListenerResponse response = context.Response;
                        //Console.WriteLine("{0} {1} HTTP/1.1", request.HttpMethod, request.RawUrl);
                        log.Info(string.Format("{0} {1} HTTP/1.1", request.HttpMethod, request.RawUrl));
                        if (request.RawUrl.Contains("stopjob"))
                        {
                            string name = request.QueryString.Get("jobname");
                            var list = quartzFactory.GetCurrentScheduer.QuartzTaskList;
                            IQuartzTask quartzTask = null;
                            if (list.TryGetValue(name, out quartzTask))
                            {
                                removed.Add(name);
                                quartzTask.StopJob();
                            }
                        }

                        else if (request.RawUrl.Contains("pausejob"))
                        {
                            string name = request.QueryString.Get("jobname");
                            var list = quartzFactory.GetCurrentScheduer.QuartzTaskList;
                            IQuartzTask quartzTask = null;
                            if (list.TryGetValue(name, out quartzTask))
                            {
                                quartzTask.Pause();
                            }
                        }
                        else if (request.RawUrl.Contains("resumejob"))
                        {
                            string name = request.QueryString.Get("jobname");
                            var list = quartzFactory.GetCurrentScheduer.QuartzTaskList;
                            IQuartzTask quartzTask = null;
                            if (list.TryGetValue(name, out quartzTask))
                            {
                                quartzTask.Resume();
                            }
                        }

                        else if (request.RawUrl.Contains("startjob"))
                        {
                            string name = request.QueryString.Get("jobname");
                            var list = quartzFactory.GetCurrentScheduer.QuartzTaskList;
                            IQuartzTask quartzTask = null;
                            if (!list.TryGetValue(name, out quartzTask))
                            {
                                removed.Remove(name);
                                quartzFactory.AddJob(name);
                            }
                        }
                        else if (request.RawUrl.Contains("stopservice"))
                        {
                            quartzFactory.GetCurrentScheduer.Stop();
                        }

                        else if (request.RawUrl.Contains("startservice"))
                        {
                            removed.Clear();
                            quartzFactory.Start();
                        }

                        var list1 = quartzFactory.GetCurrentScheduer.QuartzTaskList;
                        StringBuilder sb = new StringBuilder();
                        sb.AppendFormat("当前服务状态【{0}】", quartzFactory.GetCurrentScheduer.GetScheduleStatus.ToString());
                        sb.Append("<a href='" + Monitoring +"'>【刷新】</a>");
                        sb.Append("<a href='" + Monitoring + "stopservice'>【停止全部任务】</a>");
                        sb.Append("<a href='" + Monitoring + "startservice'>【启动全部任务】</a>");
                        sb.Append("</br>");
                        sb.Append("-------以下是正在执行的任务-------");
                        sb.Append("</br>");
                        foreach (var item in list1)
                        {
                            sb.AppendFormat("组名{0}:服务名{1}:当前状态【{2}】:下次执行时间{3}", item.Value.GroupName, item.Key, item.Value.JobStatus.ToString(), item.Value.GetNexeTime());
                            sb.AppendFormat("<a href='" + Monitoring + "pausejob?jobname={0}'>【暂停】</a>", item.Key);
                            sb.AppendFormat("<a href='" + Monitoring + "resumejob?jobname={0}'>【继续】</a>", item.Key);
                            sb.AppendFormat("<a href='" + Monitoring + "stopjob?jobname={0}'>【停止】</a>", item.Key);
                            sb.Append("</br>");
                        }
                        sb.Append("-------以下是已经停止的任务------");
                        sb.Append("</br>");
                        foreach (var it in removed)
                        {
                            sb.Append(it);
                            sb.AppendFormat("<a href='" + Monitoring + "startjob?jobname={0}'>【开始】</a>", it);
                            sb.Append("</br>");
                        }

                        sb.Append("-------以下是没有增加的任务------");
                        sb.Append("</br>");
                        var listsurplus = SurplusPlug(quartzFactory.GetPlugs(), list1.Keys.ToList(), removed);
                        foreach (var it in listsurplus)
                        {
                            sb.Append(it);
                            sb.AppendFormat("<a href='" + Monitoring + "startjob?jobname={0}'>【加载启动】</a>", it);
                            sb.Append("</br>");
                        }

                        string responseString = @"<html> <head><title>From HttpListener Server</title></head> <body>" + sb.ToString() + "</body> </html>";
                        response.ContentLength64 = System.Text.Encoding.UTF8.GetByteCount(responseString);
                        response.ContentType = "text/html; charset=UTF-8";
                        // 输出回应内容
                        System.IO.Stream output = response.OutputStream;
                        System.IO.StreamWriter writer = new System.IO.StreamWriter(output);
                        writer.Write(responseString);
                        // 必须关闭输出流
                        writer.Close();
                    }
                });
                thread31.Start();
            }
            catch (Exception oe)
            {
                logerr.Error(oe.Message);
            }
        }

        static IList<string> SurplusPlug(IList<PlugModel> listall, IList<string> running, IList<string> stoped)
        {
            var listallstring = listall.Select(x => x.JobName).ToList();
            string[] list = new string[running.Count + stoped.Count];
            running.CopyTo(list, 0);
            stoped.CopyTo(list, running.Count);
            var re = listallstring.Except(list);
            return re.ToList();
        }
    }
}
