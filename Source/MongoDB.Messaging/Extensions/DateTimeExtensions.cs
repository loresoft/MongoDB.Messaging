using System;

namespace MongoDB.Messaging.Extensions
{
    /// <summary>
    /// <see cref="DateTime"/> extension methods
    /// </summary>
    public static class DateTimeExtensions
    {
        private const long UnixEpochSeconds = 62135596800;
        private const long UnixEpochTicks = 621355968000000000;

        /// <summary>
        /// Returns the number of seconds that have elapsed since 1970-01-01T00:00:00Z.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>The number of seconds that have elapsed since 1970-01-01T00:00:00Z.</returns>
        public static long ToUnixTimeSeconds(this DateTime dateTime)
        {
            long seconds = dateTime.ToUniversalTime().Ticks / TimeSpan.TicksPerSecond;
            return seconds - UnixEpochSeconds;
        }

        /// <summary>
        /// Converts a Unix time expressed as the number of seconds that have elapsed since 1970-01-01T00:00:00Z to a <see cref="DateTime"/> value.
        /// </summary>
        /// <param name="seconds">A Unix time, expressed as the number of seconds that have elapsed since 1970-01-01T00:00:00Z.</param>
        /// <returns>A date and time value that represents the same moment in time as the Unix time.</returns>
        public static DateTime FromUnixTimeSeconds(this long seconds)
        {
            long ticks = seconds * TimeSpan.TicksPerSecond + UnixEpochTicks;
            return new DateTime(ticks, DateTimeKind.Utc);
        }
    }
}