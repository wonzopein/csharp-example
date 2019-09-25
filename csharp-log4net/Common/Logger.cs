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
        static Dictionary<String, ILog> _logger = new Dictionary<string, ILog>() {
            {"APPLICATION", log4net.LogManager.GetLogger("APPLICATION")},
            {"DEVICE", log4net.LogManager.GetLogger("DEVICE")}
        };

        static Logger()
        {
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("LoggerConfig.xml"));
        }
        
        public static ILog Application { get => _logger["APPLICATION"]; }

        public static ILog Device { get => _logger["DEVICE"]; }

    }
}
