using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.Messaging.Logging
{
    /// <summary>
    /// Extension methods for <see cref="ILogger"/>
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Write a trace level log with specified message formatting <see langword="delegate" />.
        /// </summary>
        /// <param name="logger">The logger to write to.</param>
        /// <param name="messageFormatter">The <see langword="delegate" /> used to generate the log message.</param>
        public static void Trace(this ILogger logger, Func<string> messageFormatter)
        {
            logger.Trace().Message(messageFormatter).Write();
        }

        /// <summary>
        /// Write a debug level log with specified message formatting <see langword="delegate"/>.
        /// </summary>
        /// <param name="messageFormatter">The <see langword="delegate"/> used to generate the log message.</param>
        /// <param name="logger">The logger to write to.</param>
        public static void Debug(this ILogger logger, Func<string> messageFormatter)
        {
            logger.Debug().Message(messageFormatter).Write();
        }

        /// <summary>
        /// Write a info level log with specified message formatting <see langword="delegate"/>.
        /// </summary>
        /// <param name="messageFormatter">The <see langword="delegate"/> used to generate the log message.</param>
        /// <param name="logger">The logger to write to.</param>
        public static void Info(this ILogger logger, Func<string> messageFormatter)
        {
            logger.Info().Message(messageFormatter).Write();
        }

        /// <summary>
        /// Write a warn level log with specified message formatting <see langword="delegate"/>.
        /// </summary>
        /// <param name="messageFormatter">The <see langword="delegate"/> used to generate the log message.</param>
        /// <param name="logger">The logger to write to.</param>
        public static void Warn(this ILogger logger, Func<string> messageFormatter)
        {
            logger.Warn().Message(messageFormatter).Write();
        }

        /// <summary>
        /// Write a error level log with specified message formatting <see langword="delegate"/>.
        /// </summary>
        /// <param name="messageFormatter">The <see langword="delegate"/> used to generate the log message.</param>
        /// <param name="logger">The logger to write to.</param>
        public static void Error(this ILogger logger, Func<string> messageFormatter)
        {
            logger.Error().Message(messageFormatter).Write();
        }

        /// <summary>
        /// Write a fatal level log with specified message formatting <see langword="delegate"/>.
        /// </summary>
        /// <param name="messageFormatter">The <see langword="delegate"/> used to generate the log message.</param>
        /// <param name="logger">The logger to write to.</param>
        public static void Fatal(this ILogger logger, Func<string> messageFormatter)
        {
            logger.Fatal().Message(messageFormatter).Write();
        }
    }
}
