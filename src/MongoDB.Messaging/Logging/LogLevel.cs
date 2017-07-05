using System;

namespace MongoDB.Messaging.Logging
{
    /// <summary>
    /// Defines available log levels.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>Trace log level.</summary>
        Trace = 0,
        /// <summary>Debug log level.</summary>
        Debug = 1,
        /// <summary>Info log level.</summary>
        Info = 2,
        /// <summary>Warn log level.</summary>
        Warn = 3,
        /// <summary>Error log level.</summary>
        Error = 4,
        /// <summary>Fatal log level.</summary>
        Fatal = 5,
    }
}