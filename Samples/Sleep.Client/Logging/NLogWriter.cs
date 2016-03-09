using System;
using MongoDB.Messaging.Logging;

namespace Sleep.Client.Logging
{
    /// <summary>
    /// NLog log writer adapter 
    /// </summary>
    public class NLogWriter : ILogWriter
    {
        /// <summary>
        /// Writes the specified LogData to NLog.
        /// </summary>
        /// <param name="logData">The log data.</param>
        public void WriteLog(LogData logData)
        {
            var logEvent = ToLogEvent(logData);
            var name = logData.Logger ?? typeof(NLogWriter).FullName;

            var logger = global::NLog.LogManager.GetLogger(name);
            logger.Log(logEvent);
        }


        /// <summary>
        /// Converts the LogData to LogEventInfo.
        /// </summary>
        /// <param name="logData">The log data.</param>
        /// <returns></returns>
        public static global::NLog.LogEventInfo ToLogEvent(LogData logData)
        {
            var logEvent = new global::NLog.LogEventInfo();
            logEvent.TimeStamp = DateTime.Now;
            logEvent.Level = ToLogLevel(logData.LogLevel);
            logEvent.LoggerName = logData.Logger;
            logEvent.Exception = logData.Exception;
            logEvent.FormatProvider = logData.FormatProvider;
            logEvent.Parameters = logData.Parameters;
            logEvent.Message = FormatMessage(logData);

            if (logData.Properties != null)
                foreach (var property in logData.Properties)
                    logEvent.Properties[property.Key] = property.Value;

            logEvent.Properties["CallerMemberName"] = logData.MemberName;
            logEvent.Properties["CallerFilePath"] = logData.FilePath;
            logEvent.Properties["CallerLineNumber"] = logData.LineNumber;

            return logEvent;
        }

        /// <summary>
        /// Converts the LogLevel to NLog.LogLevel
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <returns></returns>
        public static global::NLog.LogLevel ToLogLevel(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Fatal: return global::NLog.LogLevel.Fatal;
                case LogLevel.Error: return global::NLog.LogLevel.Error;
                case LogLevel.Warn: return global::NLog.LogLevel.Warn;
                case LogLevel.Info: return global::NLog.LogLevel.Info;
                case LogLevel.Trace: return global::NLog.LogLevel.Trace;
            }

            return global::NLog.LogLevel.Debug;
        }


        private static string FormatMessage(LogData logData)
        {
            string message = null;
            try
            {
                message = logData.MessageFormatter != null
                    ? logData.MessageFormatter()
                    : logData.Message;
            }
            catch (Exception)
            {
                // don't throw error
            }

            return message ?? string.Empty;
        }

        private static readonly Lazy<NLogWriter> _current = new Lazy<NLogWriter>(() => new NLogWriter());

        /// <summary>
        /// Gets the current singleton instance of <see cref="NLogWriter"/>.
        /// </summary>
        /// <value>The current singleton instance.</value>
        /// <remarks>
        /// An instance of <see cref="NLogWriter"/> wont be created until the very first 
        /// call to the sealed class. This is a CLR optimization that
        /// provides a properly lazy-loading singleton. 
        /// </remarks>
        public static NLogWriter Default
        {
            get { return _current.Value; }
        }

    }
}
