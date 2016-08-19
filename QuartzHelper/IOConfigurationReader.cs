using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartzHelper
{
    public class IOConfigurationReader : IDisposable
    {
        private static IOConfigurationReader instance = null;
        private static readonly object instanceLocker = new object();
        private static Boolean disposed = false;
        private Dictionary<String, ConfigSection> ConfigSet = null;
        private const string DKEY = "mikecheers";
        /// <summary>
        /// 默认是否以加密方式读取
        /// </summary>
        private const Boolean DefaultEncrypted = false;
        /// <summary>
        /// 默认是否读取注释
        /// </summary>
        private const Boolean DefaultReadComment = false;

        private IOConfigurationReader() { }

        private static object threadSafeLocker = new object();

        public static IOConfigurationReader GetInstance()
        {
            if (null == instance || disposed)
                lock (instanceLocker)
                    if (null == instance || disposed)
                        instance = new IOConfigurationReader();
            return instance;
        }

        public Dictionary<String, String> Read(String configFilePath)
        {
            return Read(configFilePath, DefaultEncrypted);
        }

        public Dictionary<String, String> Read(String configFilePath, Boolean encrypted)
        {
            return Read(configFilePath, encrypted, DefaultReadComment);
        }

        public Dictionary<String, String> Read(String configFilePath, Boolean encrypted, Boolean readcomment)
        {
            lock (threadSafeLocker)
            {
                if (String.IsNullOrWhiteSpace(configFilePath))
                    throw new ArgumentNullException("Invalide configFilePath");

                FileInfo file = new FileInfo(configFilePath);

                if (!file.Exists)
                    throw new ArgumentException("Config file not exists", configFilePath);
                if (null == ConfigSet)
                    ConfigSet = new Dictionary<string, ConfigSection>();
                String key = configFilePath.ToUpper().Trim();

                if (ConfigSet.ContainsKey(key))
                {
                    if (ConfigSet[key].Timestamp.Equals(file.LastWriteTime))
                        return ConfigSet[key].Items;
                    else
                        ConfigSet.Remove(key);
                }

                var section = new ConfigSection() { Timestamp = file.LastWriteTime, Items = new Dictionary<string, string>() };

                if (encrypted)
                {
                    //FileStream stream = file.OpenRead();
                    //using (BinaryReader reader = new BinaryReader(stream))
                    //    while (stream.Position < stream.Length)
                    //    {
                    //        string line = DEncryptHelper.AESDecrypt(StreamHelper.ReadString(reader, Encoding.Unicode), DKEY, Encoding.Unicode);

                    //        AnlizeSectionLine(ref section, line, readcomment);
                    //    }
                }
                else
                {
                    using (StreamReader reader = file.OpenText())
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();

                            AnlizeSectionLine(ref section, line, readcomment);
                        }
                }

                if (ConfigSet.ContainsKey(key))
                    throw new Exception(string.Format("The key '{0}' has already exists in the config file '{1}'", key, configFilePath));

                ConfigSet.Add(key, section);

                return ConfigSet[key].Items;
            }
        }

        private static void AnlizeSectionLine(ref ConfigSection section, string line, Boolean readcomment)
        {
            if (String.IsNullOrWhiteSpace(line)) return;
            if (line.StartsWith("#"))
                if (readcomment)
                    line = String.Format("#Comment_{0}={1}", section.Items.Count, line);
                else
                    return;

            var array = line.Split("=".ToCharArray(), 2, StringSplitOptions.RemoveEmptyEntries);
            if (array.Length != 2) return;

            if (!String.IsNullOrWhiteSpace(array[1]))
                section.Items.Add(array[0].ToUpper().Trim(), array[1].Trim());
        }

        public void Dispose()
        {
            disposed = true;
            ConfigSet = null;
        }
    }
}
