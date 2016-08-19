using Quartz;
using QuartzComponent;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartzHandle
{
    public class QuartzSchedule : IQuartzSchedule
    {

        public QuartzSchedule(IScheduler scheduler)
        {
            this.Scheduler = scheduler;
            QuartzTaskList = new ConcurrentDictionary<string, IQuartzTask>();
        }

        public ConcurrentDictionary<string, IQuartzTask> QuartzTaskList
        {
            get;
            set;
        }


        public IScheduler Scheduler
        {
            get;
            set;
        }

        public ScheduleStatus GetScheduleStatus
        {
            get;
            private set;
        }
        public IQuartzTask GetQuartTask(QuartzModel model)
        {
            //从列表里找到对应的
            QuartzTask quartzTask = null;
            if (this.CheckExists(model))
            {
                quartzTask = new QuartzTask(this, model);
                QuartzTaskList.TryAdd(model.JobName, quartzTask);
            }
            return quartzTask;
        }


        public void Start()
        {
            if (this.Scheduler.InStandbyMode || !this.Scheduler.IsStarted)
            {
                this.Scheduler.Start();
                this.Scheduler.Clear();
                this.GetScheduleStatus = ScheduleStatus.Starting;
            }
        }

        public void Stop()
        {
            if (this.Scheduler.IsStarted)
            {
                this.Scheduler.Standby();
                QuartzTaskList.Clear();
                //QuartzModelList.Clear();
                this.GetScheduleStatus = ScheduleStatus.Stop;
            }
        }

        public void Pause()
        {
            if (this.Scheduler.IsStarted)
            {
                this.Scheduler.PauseAll();
                this.GetScheduleStatus = ScheduleStatus.Pause;
            }
        }

        public void Resume()
        {
            if (this.Scheduler.IsStarted)
            {
                this.Scheduler.PauseAll();
                this.GetScheduleStatus = ScheduleStatus.Starting;
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IQuartzTask UpdateJob(QuartzModel oldmodel, QuartzModel newModel)
        {
            IQuartzTask quartzTask = null;
            if (this.CheckExists(oldmodel))
            {
                //需要加QuartzTaskList
                if (this.RemoveJob(oldmodel))
                    quartzTask = AddJob(newModel);
            }
            return quartzTask;
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IQuartzTask AddJob(QuartzModel model)
        {
            QuartzTask quartzTask = null;
            if (!this.CheckExists(model))
            {
                quartzTask = new QuartzTask(this, model);
                if (this.QuartzTaskList.TryAdd(model.JobName, quartzTask))
                {
                    this.Scheduler.ScheduleJob(model.JobDetail, model.Trigger);
                }
            }
            return quartzTask;
        }

        public bool CheckExists(QuartzModel model)
        {
            IQuartzTask model1 = null;
            bool bl = this.Scheduler.CheckExists(model.JobDetail.Key);
            bool b2 = this.QuartzTaskList.TryGetValue(model.JobName, out model1);
            return bl && b2;
        }

        public bool RemoveJob(QuartzModel model)
        {
            IQuartzTask model1 = null;
            //if (this.CheckExists(model))
            //{
            //if (this.QuartzTaskList.TryRemove(model.JobName, out model1))
            //{ 

            //}
            if (this.CheckExists(model))
                this.Scheduler.DeleteJob(model.JobDetail.Key);
            return this.QuartzTaskList.TryRemove(model.JobName, out model1);

            // }
        }
    }

    public enum ScheduleStatus
    {
        Starting = 0,
        Stop = 1,
        Pause = 2
    }
}
