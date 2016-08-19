using Quartz;
using QuartzComponent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartzHandle
{
    public class QuartzTask : IQuartzTask
    {
        public QuartzTask(QuartzSchedule quartzSchedule, QuartzModel quartzModel)
        {
            this.QSchedule = quartzSchedule;
            this.QTModel = quartzModel;
        }

        private QuartzSchedule QSchedule
        {
            get;
            set;
        }

        private QuartzModel QTModel
        {
            get;
            set;
        }


        public TriggerState JobStatus
        {
            get
            {
                return
                    this.QSchedule.Scheduler.GetTriggerState
                     (
                         this.QTModel.Trigger.Key
                     );
            }
        }

        public bool StopJob()
        {
            bool bl = false;
            bl = this.QSchedule.Scheduler.UnscheduleJob
                (
                 this.QTModel.Trigger.Key
                );
            if (bl)
            {
                bl = this.QSchedule.RemoveJob(this.QTModel);
            }
            return bl;
        }

        public bool ReStartJob()
        {
            if (!this.QSchedule.CheckExists(this.QTModel))
            {
                return this.QSchedule.AddJob(this.QTModel) != null;
            }
            return false;
        }

        public void Pause()
        {
            this.QSchedule.Scheduler.PauseJob
                (
                  this.QTModel.JobDetail.Key
                );
        }

        public void Resume()
        {
            this.QSchedule.Scheduler.ResumeJob
                (
                    this.QTModel.JobDetail.Key
                );
        }

        public void ImmediatelyJob()
        {
            this.QSchedule.Scheduler.TriggerJob
               (
                   this.QTModel.JobDetail.Key
               );
        }


        public string GetNexeTime()
        {
            var timeOffset = this.QSchedule.Scheduler.GetTrigger(this.QTModel.Trigger.Key);
            if (timeOffset != null)
                return timeOffset.GetNextFireTimeUtc().Value.LocalDateTime.ToString();
            return "";
        }

        public string JobName
        {
            get
            {
                return this.QTModel.JobName;
            }
        }

        public string GroupName
        {
            get
            {
                return this.QTModel.JobDetail.Key.Group;
            }
        }

        //public DateTime? GetPreviousTime
        //{
        //    get
        //    {
        //        return 
        //        {  
        //            if (this.QSchedule.Scheduler.GetTrigger(this.QTModel.Trigger.Key).GetPreviousFireTimeUtc()!=null)
        //            {
        //                return null;
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// 必须在停止状态完成，不能在别的状态更新
        /// </summary>
    }
}
