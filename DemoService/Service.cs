using DemoUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoService
{
    public class Service
    {

        public User GetU()
        {
          return  DemoFact.GetUser();
        }
    }
}
