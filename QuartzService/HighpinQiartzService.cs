using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace QuartzService
{
    public partial class HighpinQiartzService : ServiceBase
    {
        public HighpinQiartzService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            QuartzBootStrap.BootStrap();
        }

        protected override void OnStop()
        {
        }
    }
}
