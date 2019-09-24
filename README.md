# csharp-example

## 01. csharp-log4net
C#에서 Log4net을 간편하게 사용하기 위해 설정해본다.
(예제는 WPF 프로젝트)

- LoggerConfig.xml : Log4net 설정파일
- Logger.cs : Static Logger
- MainWindow.xaml.cs : Logger 테스트

#### csharp-example/csharp-log4net/LoggerConfig.xml
```XML
<?xml version="1.0" encoding="utf-8"?>
<!-- This section contains the log4net configuration settings -->
<log4net>

  <!-- Logger : Console -->
  <appender name="Console" type="log4net.Appender.ConsoleAppender">
    <layout type="log4net.Layout.PatternLayout">
      <conversionPattern value="%date %5level %10logger %class{1}:%line - %m%n" />
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
      <conversionPattern value="%date %5level %10logger %class{1}:%line - %m%n" />
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
      <conversionPattern value="%date %5level %10logger %class{1}:%line - %m%n" />
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

</log4net>
```

#### csharp-example/csharp-log4net/Common/Logger.cs
```c#
public static class Logger
{
    static Logger()
    {
        log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("LoggerConfig.xml"));
        Application.Info("Logger Initailized.");
    }

    public static ILog Application { get => log4net.LogManager.GetLogger("APPLICATION"); }

}
````

#### csharp-example/csharp-log4net/MainWindow.xaml.cs
```C#
using csharp_log4net.Common;

,...

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
    }
}
```
