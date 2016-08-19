using QuartzComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DQuartzTask
{
    public class D : IQuartzService
    {
        public void SerivceExecute(object[] objs)
        {
            Console.WriteLine("DDDD" + DateTime.Now);
        }
    }
}
