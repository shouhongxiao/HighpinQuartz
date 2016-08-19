using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartzHandle
{
    public class QuartzModel
    {
        public String JobName
        {
            get;
            set;
        }

        public QuartzModel(IJobDetail jobDetail, ITrigger trigger)
        {
            this.JobDetail = jobDetail;
            this.Trigger = trigger;
        }

        public QuartzModel(string jobName,IJobDetail jobDetail, ITrigger trigger)
        {
            this.JobName = jobName;
            this.JobDetail = jobDetail;
            this.Trigger = trigger;
        }

        public IJobDetail JobDetail
        {
            get;
            set;
        }

        public ITrigger Trigger
        {
            get;
            set;
        }
    }
}
