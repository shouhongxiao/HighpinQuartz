using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartzComponent
{
    public class JobBase
    {

        public IDictionary<String, object> map = null;

        public JobBase()
        {
            map = new Dictionary<String, object>();
            Init(map);
        }
        protected virtual void Init(IDictionary<String, object> map)
        {
            map.Add("a", 123);
        }

    }
}
