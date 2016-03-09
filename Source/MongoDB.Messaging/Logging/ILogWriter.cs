using System;

namespace MongoDB.Messaging.Logging
{
    /// <summary>
    /// An interface defining a log writer.
    /// </summary>
    public interface ILogWriter
    {
        /// <summary>
        /// Writes the specified <see cref="LogData"/> to the underlying logger.
        /// </summary>
        /// <param name="logData">The log data to write.</param>
        void WriteLog(LogData logData);
    }
}