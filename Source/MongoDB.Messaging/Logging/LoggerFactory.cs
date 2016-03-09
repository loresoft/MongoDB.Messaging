using System;

namespace MongoDB.Messaging.Logging
{
    /// <summary>
    /// A factory for creating <see cref="ILogger"/> instances using <typeparamref name="T"/> as the name of the logger.
    /// </summary>
    /// <typeparam name="T">The type used to name the <see cref="ILogger"/>.</typeparam>
    public interface ILoggerFactory<T>
    {
        /// <summary>
        /// Create an instance of <see cref="ILogger"/> using <typeparamref name="T"/> as the logger name.
        /// </summary>
        /// <returns>An instace of <see cref="ILogger"/>.</returns>
        ILogger CreateLogger();
    }

    /// <summary>
    /// A factory for creating <see cref="ILogger"/> instances using <typeparamref name="T"/> as the name of the logger.
    /// </summary>
    /// <typeparam name="T">The type used to name the <see cref="ILogger"/>.</typeparam>
    public class LoggerFactory<T> : ILoggerFactory<T>
    {
        // lazy singleton of logger
        private static readonly Lazy<ILogger> _logger = new Lazy<ILogger>(Logger.CreateLogger<T>);

        /// <summary>
        /// Create an instance of <see cref="ILogger" /> using <typeparamref name="T" /> as the logger name.
        /// </summary>
        /// <returns>
        /// An instace of <see cref="ILogger" />.
        /// </returns>
        public ILogger CreateLogger()
        {
            return _logger.Value;
        }
    }
}
