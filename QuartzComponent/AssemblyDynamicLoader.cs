using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuartzComponent
{
    public class AssemblyDynamicLoader
    {
        private AppDomain appDomain;
        private RemoteLoader remoteLoader;
        public AssemblyDynamicLoader(string domainName)
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationName = domainName + "ApplicationLoader";
            setup.ApplicationBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, domainName);
            setup.PrivateBinPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, domainName, "private");
            setup.CachePath = setup.ApplicationBase;
            setup.ShadowCopyFiles = "true";
            setup.ShadowCopyDirectories = setup.ApplicationBase;
            this.appDomain = AppDomain.CreateDomain(domainName, null, setup);
            String name = Assembly.GetExecutingAssembly().GetName().FullName;
            this.remoteLoader = (RemoteLoader)this.appDomain.CreateInstanceAndUnwrap(name, typeof(RemoteLoader).FullName);
        }

        public void LoadAssembly(string assemblyFile)
        {
            remoteLoader.LoadAssembly(assemblyFile);
        }

        public T GetInstance<T>(string typeName) where T : class
        {
            if (remoteLoader == null) return null;
            return remoteLoader.GetInstance<T>(typeName);
        }

        public object GetInstance(string typeName)
        {
            if (remoteLoader == null) return null;
            return remoteLoader.GetInstance(typeName);
        }

        public void ExecuteMothod(string typeName, string methodName)
        {
            remoteLoader.ExecuteMothod(typeName, methodName);
        }

        public void ExecuteMothod(string typeName, string methodName, object[] args)
        {
            try
            {
                remoteLoader.ExecuteMothod(typeName, methodName, args);
            }
            catch (Exception oe)
            {
                Debug.WriteLine(oe.Message);
            }
        }

        public object ExecuteMothodWithResutl(string typeName, string methodName, object[] args)
        {
            object obj = remoteLoader.ExecuteMothodWithResutl(typeName, methodName, args);

            return obj;
        }

        public void Unload()
        {
            try
            {
                if (appDomain == null) return;
                AppDomain.Unload(this.appDomain);
                this.appDomain = null;
            }
            catch (CannotUnloadAppDomainException ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }

}
