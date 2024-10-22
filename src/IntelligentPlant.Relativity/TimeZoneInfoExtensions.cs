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

    }
}
