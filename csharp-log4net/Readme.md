# csharp-example

## 01. csharp-log4net
C#에서 Log4net을 간편하게 사용하기 위해 설정해본다.
(예제는 WPF 프로젝트)

- LoggerConfig.xml : Log4net 설정파일
- Logger.cs : Static Logger
- MainWindow.xaml.cs : Logger 테스트

#### csharp-example/csharp-log4net/LoggerConfig.xml

PatternLayout Class, [log4net_Layout_PatternLayout Doc](https://logging.apache.org/log4net/release/sdk/html/T_log4net_Layout_PatternLayout.htm)
```XML
<?xml version="1.0" encoding="utf-8"?>
<!-- This section contains the log4net configuration settings -->
<log4net>

  <!-- Logger : Console -->
  <appender name="Console" type="log4net.Appender.ConsoleAppender">
    <layout type="log4net.Layout.PatternLayout">
      <!--<conversionPattern value="%date %5level %-11logger% %class{1}:%line - %m%n" />-->
      <conversionPattern value="%date %-5level %-11logger% %15class{1}:%-4line - %m%n" />
    </layout>
  </appender>

  <!-- Logger : All file -->
  <appender name="All" type="log4net.Appender.RollingFileAppender">
    <file value="Log\" />
    <appendToFile value="true" />
    <datePattern value="yyyy-MM-dd-'All.log'" />
    <staticLogFileName value="false"/>
    <rollingStyle value="Date" />
    <layout type="log4net.Layout.PatternLayout">
      <conversionPattern value="%date %-5level %-11logger% %15class{1}:%-4line - %m%n" />
    </layout>
  </appender>
  
  <!-- Logger : Application file -->
  <appender name="Application" type="log4net.Appender.RollingFileAppender">
    <file value="Log\" />
    <appendToFile value="true" />
    <datePattern value="yyyy-MM-dd-'Application.log'" />
    <staticLogFileName value="false"/>
    <rollingStyle value="Date" />
    <layout type="log4net.Layout.PatternLayout">
      <conversionPattern value="%date %-5level %-11logger% %15class{1}:%-4line - %m%n" />
    </layout>
  </appender>

  <!-- Logger : Device file -->
  <appender name="Device" type="log4net.Appender.RollingFileAppender">
    <file value="Log\" />
    <appendToFile value="true" />
    <datePattern value="yyyy-MM-dd-'Device.log'" />
    <staticLogFileName value="false"/>
    <rollingStyle value="Date" />
    <layout type="log4net.Layout.PatternLayout">
      <conversionPattern value="%date %-5level %-11logger% %15class{1}:%-4line - %m%n" />
    </layout>
  </appender>

  <root>
    <level value="DEBUG" />	<!-- DEBUG < INFORMATION < WARNING < ERROR < FATAL -->
  </root>

  <logger name="APPLICATION">
    <level value="ALL" />
    <appender-ref ref="All" />
    <appender-ref ref="Application" />
    <appender-ref ref="Console" />
  </logger>  
  
  <logger name="DEVICE">
    <level value="ALL" />
    <appender-ref ref="All" />
    <appender-ref ref="Device" />
    <appender-ref ref="Console" />
  </logger>

</log4net>
```

#### csharp-example/csharp-log4net/Common/Logger.cs
```c#
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

````

#### csharp-example/csharp-log4net/MainWindow.xaml.cs
```C#
using csharp_log4net.Common;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        LogTest();
    }

    private void LogTest()
    {
        Logger.Application.Info("Info.");
        Logger.Application.Debug("Debug.");
        Logger.Application.Warn("Warn.");
        Logger.Application.Error("Error.");
        Logger.Application.Fatal("Fatal.");

        Logger.Device.Info("Info.");
        Logger.Device.Debug("Debug.");
        Logger.Device.Warn("Warn.");
        Logger.Device.Error("Error.");
        Logger.Device.Fatal("Fatal.");
    }
}
```
#### result
```
2019-09-25 09:25:42,973 INFO  APPLICATION      MainWindow:33   - Info.
2019-09-25 09:25:43,016 DEBUG APPLICATION      MainWindow:34   - Debug.
2019-09-25 09:25:43,017 WARN  APPLICATION      MainWindow:35   - Warn.
2019-09-25 09:25:43,018 ERROR APPLICATION      MainWindow:36   - Error.
2019-09-25 09:25:43,019 FATAL APPLICATION      MainWindow:37   - Fatal.
2019-09-25 09:25:43,020 INFO  DEVICE           MainWindow:39   - Info.
2019-09-25 09:25:43,021 DEBUG DEVICE           MainWindow:40   - Debug.
2019-09-25 09:25:43,022 WARN  DEVICE           MainWindow:41   - Warn.
2019-09-25 09:25:43,023 ERROR DEVICE           MainWindow:42   - Error.
2019-09-25 09:25:43,024 FATAL DEVICE           MainWindow:43   - Fatal.
```