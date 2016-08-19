using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartzComponent
{
    [Serializable]
    public class PlugModel
    {
       
        public string JobName
        {
            get;
            set;
        }

        public string GroupName
        {
            get;
            set;
        }

        public string CronexPression
        {
            get;
            set;
        }

        public int TimeSpan
        {
            get;
            set;
        }

        public string DllRelativeDir
        {
            set;
            get;
        }

        public string DllIQuartzServiceImplementFile
        {
            get;
            set;
        }
    }
}
