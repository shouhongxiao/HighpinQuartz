using DemoUser;
using Quartz;
using QuartzComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BQuartzTask
{
    public class B : IQuartzService
    {

        static int i = 0;
        public void SerivceExecute(object[] objs)
        {
            Console.WriteLine("BBBB" + DateTime.Now);
            User user = DemoService.DemoFact.GetUser();
            Console.WriteLine(user.age);
            Console.WriteLine("i=="+i++);
        }
    }
}
