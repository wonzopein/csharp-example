using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_log4net.Common
{
    public static class Logger
    {
        static Logger()
        {
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("LoggerConfig.xml"));
            Application.Info("Logger Initailized.");
        }
        
        public static ILog Application { get => log4net.LogManager.GetLogger("APPLICATION"); }

    }
}
