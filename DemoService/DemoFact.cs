using DemoUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoService
{
   public  class DemoFact
    {

       public static User GetUser()
       {
        return new User(){age=33,sex=true,name="shoushou"};
       }

       public int GetAge()
       {
           return 33;
       }
    }


   
}
