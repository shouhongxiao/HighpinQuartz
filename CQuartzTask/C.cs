using Quartz;
using QuartzComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQuartzTask
{

    public class C : IQuartzService
    {
        public void SerivceExecute(object[] objs)
        {
            Console.WriteLine("CCCC" + DateTime.Now);
          
        }
    }
}
