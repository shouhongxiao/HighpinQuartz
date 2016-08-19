using Quartz;
using QuartzHandle;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartzComponent
{
    public interface IQuartzSchedule
    {

        //ConcurrentDictionary<string, QuartzModel> QuartzModelList
        //{
        //    get;
        //    set;
        //}
        IQuartzTask GetQuartTask(QuartzModel model);
        void Start();
        void Stop();
        void Pause();

        void Resume();

        bool CheckExists(QuartzModel model);

        IQuartzTask UpdateJob(QuartzModel oldmodel, QuartzModel newModel);

        IQuartzTask AddJob(QuartzModel model);

        IScheduler Scheduler
        {
            get;
            set;
        }

        bool RemoveJob(QuartzModel model);
        ConcurrentDictionary<string, IQuartzTask> QuartzTaskList
        {
            get;
            set;
        }
    }
}
