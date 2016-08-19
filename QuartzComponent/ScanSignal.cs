using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartzComponent
{
    //扫描标签
    [AttributeUsage(AttributeTargets.Class,Inherited=true)]
    public class ScanSignal : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobName">第一个任务名</param>
        /// <param name="groupLevel">任务组等级</param>
        /// <param name="cronExpression"></param>
        public ScanSignal(String jobName, GroupLevel groupLevel, String cronExpression)
        {
            this.JobName = jobName;
            this.GroupLevel = groupLevel;
            this.CronExpression = cronExpression;
        }

        public ScanSignal(String jobName, GroupLevel groupLevel, int milliseconds)
        {
            this.JobName = jobName;
            this.GroupLevel = groupLevel;
            this.TimeSpan = new TimeSpan(0,0,0,0,milliseconds);
        }


        public TimeSpan TimeSpan
        {
            get;
            set;
        }

        /// <summary>
        /// 任务名
        /// </summary>
        public String JobName
        {
            get;
            set;
        }

        /// <summary>
        /// 任务等级
        /// </summary>
        public GroupLevel GroupLevel
        {
            get;
            set;
        }

        /// <summary>
        /// Cron表达式 http://www.bejson.com/cronCreator/
        /// cron配置 说明 
        //0 0 12 * * ? 每天12点触发 
        //0 15 10 ? * * 每天10点15分触发 
        //0 15 10 * * ? 每天10点15分触发 
        //0 15 10 * * ? * 每天10点15分触发 
        //0 15 10 * * ? 2014 2014年每天10点15分触发 
        //0 * 14 * * ? 每天下午的 2点到2点59分每分触发 
        //0 0/5 14 * * ? 每天下午的2点到2点59分(整点开始，每隔5分触发) 
        //0 0/5 14,18 * * ? 每天下午的 18点到18点59分(整点开始，每隔5分触发) 
        //0 0-5 14 * * ? 每天下午的 2点到2点05分每分触发 
        //0 10,44 14 ? 3 WED 3月分每周三下午的 2点10分和2点44分触发 
        //0 15 10 ? * MON-FRI 从周一到周五每天上午的10点15分触发 
        //0 15 10 15 * ? 每月15号上午10点15分触发 
        //0 15 10 L * ? 每月最后一天的10点15分触发 
        //0 15 10 ? * 6L 每月最后一周的星期五的10点15分触发 
        //0 15 10 ? * 6L 2014-2025 从2014年到2025年每月最后一周的星期五的10点15分触发 
        //0 15 10 ? * 6#3 每月的第三周的星期五开始触发 
        //0 0 12 1/5 * ? 每月的第一个中午开始每隔5天触发一次 
        //0 11 11 11 11 ? 每年的11月11号 11点11分触发(光棍节) 
        //注意：
        //1、日和周字段不能同时设置为“?”；
        //2、当在日字段上设置为“*”，“,”，“-”，“*”，“?”，“/”，“L”，“W”之一时，周字段只能设置为“?”；
        //3、当在周字段上设置为“*”，“,”，“-”，“*”，“?”，“/”，“L”，“#”之一时，日字段只能设置为“?”。
        //4、年的设置项为可选，当不设置时为“*”。
        /// </summary>
        public String CronExpression
        {
            get;
            set;
        }
        
    }
}
