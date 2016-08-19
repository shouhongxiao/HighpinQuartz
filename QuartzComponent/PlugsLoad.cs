using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartzComponent
{
    [Serializable]
    public class PlugsLoad
    {
        public string DllPath
        {
            get;
            set;
        }

        public string PlugPath
        {
            get;
            set;
        }

        public IDictionary<string, object> Maps
        {
            get;
            set;
        }

       
    }
}
