using Quartz;
using QuartzHandle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartzComponent
{
    public interface IQuartzTask
    {
        bool StopJob();

        bool ReStartJob();

        TriggerState JobStatus
        {
            get;
        }
        void Pause();

        void Resume();


        void ImmediatelyJob();


        string GetNexeTime();

        string JobName
        {
            get;
        }

        string GroupName
        {
            get;
        }
    }
}
