using System;
using MongoDB.Messaging.Logging;

namespace MongoDB.Messaging.Tests.Logging
{
    /// <summary>
    /// NLog writer adapter
    /// </summary>
    public class NLogAdapter : ILogWriter
    {
        /// <summary>
        /// Writes the log.
        /// </summary>
        /// <param name="logData">The log data.</param>
        public void WriteLog(LogData logData)
        {
            new TraceLogWriter().WriteLog(logData);
        }
    }
}
