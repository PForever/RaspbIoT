using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using log4net;
using log4net.Config;
using log4net.Repository;
namespace Log4NetUwp
{
    public static class Logger
    {

        public static void Init()
        {
            var ass = Assembly.GetCallingAssembly();
            ILoggerRepository rep = LogManager.GetRepository(ass);

            var c = Directory.GetCurrentDirectory();
            SetConfig(rep, @"\..\Config\Log4Net.config").Wait();
            //var t2 = File.Exists(@".\Log4Net.config");
        }

        public static async Task SetConfig(ILoggerRepository rep, string fileName)
        {
            // Use ONE of the following lines to get the file:
            //var sf = await Package.Current.InstalledLocation.TryGetItemAsync(fileName) as StorageFile;
            var sf = await ApplicationData.Current.GetPublisherCacheFolder(@".\..\Config\").GetFileAsync("Log4Net.config");

            var stream = await sf.OpenStreamForReadAsync();
            XmlConfigurator.Configure(rep, stream);
        }
    }
}
