using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartzComponent
{
    public interface IQuartzBuilder
    {
        bool Update(string jobName);

        IQuartzTask Add(string jobName);
    }
}
