using System;

/// <summary>
/// Helper class for DateTime conversions.
/// </summary>

namespace AstroTrack.Common
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// Converts a Unix timestamp to a DateTime object.
        /// </summary>
        public static DateTime FromUnixTimestamp(long timestamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(timestamp).ToLocalTime();
            return dtDateTime;
        }

        /// <summary>
        /// Converts a DateTime object to a Unix timestamp.
        /// </summary>
        public static long ToUnixTimestamp(DateTime dateTime)
        {
            var date = dateTime.ToUniversalTime();
            var ticks = date.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).Ticks;
            var ts = ticks / TimeSpan.TicksPerSecond;
            return ts;
        }
    }
}
