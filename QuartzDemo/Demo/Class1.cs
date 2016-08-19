using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    public class Class1
    {
        public static void Main(string[] args)
        {
            var ac = new A();
            IList<C> cc = new List<C>();
            for (int i = 0; i < 40; i++)
            {
                cc.Add(new C() { x = i, y = "i", z = DateTime.Now.AddMinutes(i) });
            }
            ac.ccclist = cc;

            IList<A> l1=  new List<A>();
            l1.Add(ac);

           

    }

    public class A
    {
        public string aa;
        public string bb;
        public DateTime cc;
        public IList<C> ccclist;
    }

    public class C
    {
        public int x;
        public string y;
        public DateTime z;
    }
}
