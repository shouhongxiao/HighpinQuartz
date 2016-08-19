using Quartz;
using QuartzComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartzComponent
{
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class TaskBase : JobBase, IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            BeforeExecuteTask(context);
            ExecuteTask(context);
            AfterExecuteTask(context);
        }

        protected virtual void BeforeExecuteTask(IJobExecutionContext context)
        {

        }

        protected virtual void ExecuteTask(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            var d = dataMap.FirstOrDefault(x => x.Key == "DyNamicLoader");
            AssemblyDynamicLoader dyNamicLoader = d.Value as AssemblyDynamicLoader;
            var dm = dataMap.FirstOrDefault(x => x.Key == "DyNamicMethod");
            string methodName = dm.Value.ToString();
            Dictionary<string, object> maps = dataMap.ToDictionary(x => x.Key, y => y.Value);
            object[] objs = new object[1];
            dyNamicLoader.ExecuteMothod(methodName, "SerivceExecute", objs);
        }

        protected virtual void AfterExecuteTask(IJobExecutionContext context)
        {

        }
    }
}
