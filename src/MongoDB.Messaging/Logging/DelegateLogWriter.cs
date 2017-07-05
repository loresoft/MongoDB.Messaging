using System;

namespace MongoDB.Messaging.Logging
{
    /// <summary>
    /// A <see langword="delegate"/> log writer.
    /// </summary>
    public class DelegateLogWriter : ILogWriter
    {
        private readonly Action<LogData> _logAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateLogWriter"/> class.
        /// </summary>
        public DelegateLogWriter() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateLogWriter"/> class.
        /// </summary>
        /// <param name="logAction">The log action.</param>
        public DelegateLogWriter(Action<LogData> logAction)
        {
            _logAction = logAction ?? DebugWrite;
        }

        /// <summary>
        /// Writes the specified <see cref="LogData"/> to the underlying logger.
        /// </summary>
        /// <param name="logData">The log data to write.</param>
        public void WriteLog(LogData logData)
        {
            _logAction?.Invoke(logData);
        }

        private static void DebugWrite(LogData logData)
        {
            System.Diagnostics.Debug.WriteLine(logData);
        }

    }
}