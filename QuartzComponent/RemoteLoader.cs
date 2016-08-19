using QuartzComponent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuartzComponent
{
    public class RemoteLoader : MarshalByRefObject
    {
        private Assembly _assembly;
        public override object InitializeLifetimeService()
        {
            System.Runtime.Remoting.Lifetime.ILease aLease
                    = (System.Runtime.Remoting.Lifetime.ILease)base.InitializeLifetimeService();
            if (aLease.CurrentState == System.Runtime.Remoting.Lifetime.LeaseState.Initial)
            {
                // 不过期
              
                aLease.InitialLeaseTime = TimeSpan.FromHours(1000);
                aLease.RenewOnCallTime = TimeSpan.FromHours(1000);
                aLease.SponsorshipTimeout = TimeSpan.FromHours(1000);
            }
            return aLease;
        }
        public void LoadAssembly(string assemblyFile)
        {
            try
            {
                _assembly = Assembly.LoadFrom(assemblyFile);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public T GetInstance<T>(string typeName) where T : class
        {
            if (_assembly == null) return null;
            var type = _assembly.GetType(typeName);
            if (type == null) return null;
            return Activator.CreateInstance(type) as T;
        }

        public object GetInstance(string typeName)
        {
            if (_assembly == null) return null;
            foreach (var item in _assembly.GetTypes())
            {
                object[] objs = item.GetCustomAttributes(typeof(ScanSignal), true);
                if (objs != null && objs.Length > 0)
                {

                }
            }
            var type = _assembly.GetType(typeName);
            if (type == null) return null;
            return Activator.CreateInstance(type);
        }


        public void ExecuteMothod(string typeName, string methodName)
        {
            if (_assembly == null) return;
            var type = _assembly.GetType(typeName);
            var obj = Activator.CreateInstance(type);
            Expression<Action> lambda = Expression.Lambda<Action>(Expression.Call(Expression.Constant(obj), type.GetMethod(methodName)), null);
            lambda.Compile()();
        }

        public void ExecuteMothod(string typeName, string methodName, object[] args)
        {
            if (_assembly == null) return;
            var type = _assembly.GetType(typeName);
            var obj = Activator.CreateInstance(type);
            MethodInfo method = type.GetMethod(methodName);
            method.Invoke(obj, args);
        }

        public object ExecuteMothodWithResutl(string typeName, string methodName, object[] args)
        {
            if (_assembly == null) return null;
            var type = _assembly.GetType(typeName);
            var obj = Activator.CreateInstance(type);
            MethodInfo method = type.GetMethod(methodName);
            return method.Invoke(obj, args);
        }
    }

}
