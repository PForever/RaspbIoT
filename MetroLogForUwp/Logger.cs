using System;
using System.IO;
using System.Reflection;
using MetroLog;
using MetroLog.Targets;
using ILogger = LogSingleton.ILogger;
using ILog = MetroLog.ILogger;

namespace MetroLogForUwp
{
    public class Logger : LogSingleton.ILogger
    {
        public ILog Log { get; }

        public Logger()
        {
            LogManagerFactory.DefaultConfiguration.AddTarget(LogLevel.Trace, LogLevel.Fatal, new StreamingFileTarget());
            Log = LogManagerFactory.DefaultLogManager.GetLogger(this.GetType());
        }
        //public static void InitLogger()
        //{

        //}
        public void Debug(object message) => Log.Debug(message?.ToString() ?? "");
        public void Debug(object message, Exception exception) => Log.Debug(message?.ToString() ?? "", exception);
        public void DebugFormat(string format, params object[] args) => Log.Debug(String.Format(format, args));
        public void Error(object message) => Log.Error(message?.ToString() ?? "");
        public void Error(object message, Exception exception) => Log.Error(message?.ToString() ?? "", exception);
        public void ErrorFormat(string format, params object[] args) => Log.Error(String.Format(format, args));
        public void Fatal(object message) => Log.Fatal(message?.ToString() ?? "");
        public void Fatal(object message, Exception exception) => Log.Fatal(message?.ToString() ?? "", exception);
        public void FatalFormat(string format, params object[] args) => Log.Fatal(String.Format(format, args));
        public void Info(object message) => Log.Info(message?.ToString() ?? "");
        public void Info(object message, Exception exception) => Log.Info(message?.ToString() ?? "", exception);
        public void InfoFormat(string format, params object[] args) => Log.Info(String.Format(format, args));
        public void Warn(object message) => Log.Warn(message?.ToString() ?? "");
        public void Warn(object message, Exception exception) => Log.Warn(message?.ToString() ?? "", exception);
        public void WarnFormat(string format, params object[] args) => Log.Warn(String.Format(format, args));
    }
}
