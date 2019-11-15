using System;

namespace IntelligentPlant.Relativity {

    /// <summary>
    /// Extension methods for <see cref="TimeZoneInfo"/>.
    /// </summary>
    public static class TimeZoneInfoExtensions {

        /// <summary>
        /// Gets the current time in the specified time zone.
        /// </summary>
        /// <param name="tz">
        ///   The time zone. If <see langword="null"/>, the local system time zone is assumed.
        /// </param>
        /// <returns>
        ///   The current time in the specified time zone.
        /// </returns>
        public static DateTime GetCurrentTime(this TimeZoneInfo tz) {
            if (tz == null) {
                tz = TimeZoneInfo.Local;
            }

            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
        }


        /// <summary>
        /// Converts a time in the specified time zone to UTC.
        /// </summary>
        /// <param name="tz">
        ///   The time zone. If <see langword="null"/>, the local system time zone is assumed.
        /// </param>
        /// <param name="date">
        ///   The time in the time zone to convert to UTC.
        /// </param>
        /// <returns>
        ///   The equivalent UTC <see cref="DateTime"/>.
        /// </returns>
        public static DateTime ConvertToUtc(this TimeZoneInfo tz, DateTime date) {
            if (date.Kind == DateTimeKind.Utc) {
                return date;
            }

            if (TimeZoneInfo.Utc.Equals(tz)) {
                return new DateTime(date.Ticks, DateTimeKind.Utc);
            }

            if (tz == null) {
                tz = TimeZoneInfo.Local;
            }

            return TimeZoneInfo.ConvertTimeToUtc(date, tz);
        }

    }
}
