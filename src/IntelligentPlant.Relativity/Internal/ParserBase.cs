using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace IntelligentPlant.Relativity.Internal {

    /// <summary>
    /// Base class for <see cref="IRelativityParser"/> implementations.
    /// </summary>
    internal abstract class ParserBase : IRelativityParser {

        /// <inheritdoc/>
        public CultureInfo CultureInfo { get; }

        /// <inheritdoc/>
        public TimeZoneInfo TimeZone { get; }

        /// <inheritdoc/>
        public RelativityBaseTimeSettings BaseTimeSettings { get; }

        /// <inheritdoc/>
        public RelativityTimeOffsetSettings TimeOffsetSettings { get; }

        /// <summary>
        /// The regular expression used to parse relative date strings.
        /// </summary>
        internal Regex RelativeDateRegex { get; }

        /// <summary>
        /// The regular expression used to parse duration strings.
        /// </summary>
        internal Regex DurationRegex { get; }


        /// <summary>
        /// Creates a new <see cref="ParserBase"/> object.
        /// </summary>
        /// <param name="cultureInfo">
        ///   The culture to use when parsing strings.
        /// </param>
        /// <param name="timeZoneInfo">
        ///   The time zone to use when parsing relative dates.
        /// </param>
        /// <param name="baseTimeSettings">
        ///   The base time keyword settings.
        /// </param>
        /// <param name="timeOffsetSettings">
        ///   The time offset keyword settings.
        /// </param>
        /// <param name="relativeDateRegex">
        ///   The regular expression used to parse relative date strings.
        /// </param>
        /// <param name="durationRegex">
        ///   The regular expression used to parse duration strings.
        /// </param>
        protected ParserBase(
            CultureInfo cultureInfo, 
            TimeZoneInfo timeZoneInfo,
            RelativityBaseTimeSettings baseTimeSettings, 
            RelativityTimeOffsetSettings timeOffsetSettings,
            Regex relativeDateRegex,
            Regex durationRegex
        ) {
            CultureInfo = cultureInfo;
            TimeZone = timeZoneInfo;
            BaseTimeSettings = baseTimeSettings;
            TimeOffsetSettings = timeOffsetSettings;
            RelativeDateRegex = relativeDateRegex;
            DurationRegex = durationRegex;
        }


        /// <inheritdoc/>
        public bool TryConvertToUtcDateTime(string dateString, DateTime? origin, out DateTime utcDateTime) {
            utcDateTime = default;

            if (string.IsNullOrWhiteSpace(dateString)) {    
                return false;
            }

            if (DateTime.TryParse(dateString, CultureInfo, DateTimeStyles.None, out var dt)) {
                utcDateTime = ConvertDateTimeToUtc(dt);
                return true;
            }

            if (TryParseRelativeDateTime(dateString, origin, out dt)) {
                utcDateTime = ConvertDateTimeToUtc(dt);
                return true;
            }

            if (TryParseNumericDateTime(dateString, CultureInfo, out utcDateTime)) {
                return true;
            }

            return false;
        }


        /// <inheritdoc/>
        public DateTime ConvertToUtcDateTime(string dateString, DateTime? origin) {
            if (dateString == null) {
                throw new ArgumentNullException(nameof(dateString));
            }

            if (!TryConvertToUtcDateTime(dateString, origin, out var dt)) {
                throw new FormatException(string.Format(CultureInfo, Resources.Error_InvalidTimeStamp, dateString));
            }

            return dt;
        }


        /// <inheritdoc/>
        public bool TryConvertToTimeSpan(string durationString, out TimeSpan timeSpan) {
            timeSpan = default;
            if (string.IsNullOrWhiteSpace(durationString)) {
                return false;
            }

            if (TimeSpan.TryParse(durationString, CultureInfo, out timeSpan)) {
                return true;
            }

            if (TryParseDuration(durationString, out timeSpan)) {
                return true;
            }

            return false;
        }


        /// <inheritdoc/>
        public TimeSpan ConvertToTimeSpan(string durationString) {
            if (durationString == null) {
                throw new ArgumentNullException(nameof(durationString));
            }

            if (!TryConvertToTimeSpan(durationString, out var ts)) {
                throw new FormatException(string.Format(CultureInfo, Resources.Error_InvalidTimeSpan, durationString));
            }

            return ts;
        }


        /// <summary>
        /// Converts a <see cref="DateTime"/> to UTC.
        /// </summary>
        /// <param name="dt">
        ///   The date and time to convert.
        /// </param>
        /// <returns>
        ///   The date and time in UTC.
        /// </returns>
        /// <remarks>
        ///   <see cref="DateTime"/> instances with a <see cref="DateTime.Kind"/> property of 
        ///   <see cref="DateTimeKind.Unspecified"/> are assumed to be in the time zone of the 
        ///   parser.
        /// </remarks>
        private DateTime ConvertDateTimeToUtc(DateTime dt) {
            switch (dt.Kind) {
                case DateTimeKind.Utc:
                    return dt;
                case DateTimeKind.Local:
                    return dt.ToUniversalTime();
                default:
                    return TimeZoneInfo.ConvertTimeToUtc(dt, TimeZone);
            }
        }


        /// <summary>
        /// Converts a <see cref="DateTime"/> to the time zone of the parser.
        /// </summary>
        /// <param name="dt">
        ///   The date and time to convert.
        /// </param>
        /// <returns>
        ///   The date and time in the time zone of the parser.
        /// </returns>
        /// <remarks>
        ///   <see cref="DateTime"/> instances with a <see cref="DateTime.Kind"/> property of 
        ///   <see cref="DateTimeKind.Unspecified"/> are assumed to already be in the time zone of 
        ///   the parser.
        /// </remarks>
        private DateTime ConvertDateTimeToParserTimeZone(DateTime dt) {
            switch (dt.Kind) {
                case DateTimeKind.Utc:
                    return TimeZoneInfo.ConvertTimeFromUtc(dt, TimeZone);
                case DateTimeKind.Local:
                    var tz = TimeZone;
                    return tz.Equals(TimeZoneInfo.Local)
                        ? dt
                        : TimeZoneInfo.ConvertTime(dt, TimeZoneInfo.Local, tz);
                default:
                    // Assume that the date is already in the correct time zone if the kind is Unspecified.
                    return dt;
            }
        }


        /// <summary>
        /// Attempts to convert a timestamp expressed as milliseconds since 01 January 1970 into a UTC 
        /// <see cref="DateTime"/> instance.
        /// </summary>
        /// <param name="dateString">
        ///   The string.
        /// </param>
        /// <param name="cultureInfo">
        ///   The culture to use when parsing the string.
        /// </param>
        /// <param name="utcDateTime">
        ///   The UTC time stamp, if <paramref name="dateString"/> is a numeric value.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="dateString"/> is a numeric value, or <see langword="false"/>
        ///   otherwise.
        /// </returns>
        private static bool TryParseNumericDateTime(string dateString, CultureInfo cultureInfo, out DateTime utcDateTime) {
            if (double.TryParse(dateString, NumberStyles.Float, cultureInfo, out var milliseconds)) {
                utcDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(milliseconds);
                return true;
            }

            utcDateTime = default;
            return false;
        }


        /// <summary>
        /// Attempts to parse a relative date string.
        /// </summary>
        /// <param name="dateString">
        ///   The date string.
        /// </param>
        /// <param name="origin">
        ///   The base time to use when parsing the relative date string. If <see langword="null"/>, 
        ///   the current time for the <see cref="TimeZone"/> is used as the base time.
        /// </param>
        /// <param name="dateTime">
        ///   The parsed date and time, if successful.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the date string was parsed successfully, or <see langword="false"/> 
        ///   otherwise
        /// </returns>
        /// <remarks>
        ///   If <paramref name="origin"/> is non-<see langword="null"/>, it will be converted 
        ///   to the parser's time zone using <see cref="ConvertDateTimeToParserTimeZone"/>
        /// </remarks>
        private bool TryParseRelativeDateTime(string dateString, DateTime? origin, out DateTime dateTime) {
            var currentTimeParserTz = origin.HasValue
                ? ConvertDateTimeToParserTimeZone(origin.Value)
                : TimeZone.GetCurrentTime();

            var match = RelativeDateRegex.Match(dateString);
            if (match.Success) {
                var absoluteBaseDate = ConvertRelativeBaseTimeToAbsolute(match.Groups["base"].Value, currentTimeParserTz, CultureInfo, BaseTimeSettings);
                var unit = match.Groups["unit"].Value;
                var quantityRaw = match.Groups["count"].Value;
                var quantity = string.IsNullOrWhiteSpace(quantityRaw) 
                    ? 0 
                    : double.Parse(match.Groups["count"].Value, CultureInfo);
                var add = match.Groups["operator"].Success && "+".Equals(match.Groups["operator"].Value, StringComparison.Ordinal);

                dateTime = ApplyOffset(absoluteBaseDate, unit, quantity, add, TimeOffsetSettings);
                return true;
            }

            dateTime = default;
            return false;
        }


        /// <summary>
        /// Converts a relative base time to an absolute <see cref="DateTime"/>.
        /// </summary>
        /// <param name="baseTime">
        ///   The base time keyword to convert.
        /// </param>
        /// <param name="origin">
        ///   The base time that the <paramref name="baseTime"/> keyword is relative to.
        /// </param>
        /// <param name="cultureInfo">
        ///   The culture to use when determining the first day of the week.
        /// </param>
        /// <param name="baseTimeSettings">
        ///   The base time keyword settings.
        /// </param>
        /// <returns>
        ///   An absolute <see cref="DateTime"/> that corresponds to the <paramref name="baseTime"/> 
        ///   keyword relative to the <paramref name="origin"/>.
        /// </returns>
        /// <exception cref="FormatException">
        ///   <paramref name="baseTime"/> is not a valid base time keyword.
        /// </exception>
        private static DateTime ConvertRelativeBaseTimeToAbsolute(string baseTime, DateTime origin, CultureInfo cultureInfo, RelativityBaseTimeSettings baseTimeSettings) {
            var startOfWeek = cultureInfo.DateTimeFormat.FirstDayOfWeek;

            if (string.Equals(baseTime, baseTimeSettings.CurrentYear, StringComparison.OrdinalIgnoreCase)) {
                // Start of current year.
                return new DateTime(origin.Year, 1, 1, 0, 0, 0, origin.Kind);
            }
            if (string.Equals(baseTime, baseTimeSettings.CurrentMonth, StringComparison.OrdinalIgnoreCase)) {
                // Start of current month.
                return new DateTime(origin.Year, origin.Month, 1, 0, 0, 0, origin.Kind);
            }
            if (string.Equals(baseTime, baseTimeSettings.CurrentWeek, StringComparison.OrdinalIgnoreCase)) {
                // Start of current week.
                var diff = (7 + (origin.DayOfWeek - startOfWeek)) % 7;
                var startOfWeekDate = origin.AddDays(-1 * diff).Date;
                return new DateTime(startOfWeekDate.Year, startOfWeekDate.Month, startOfWeekDate.Day, 0, 0, 0, origin.Kind);
            }
            if (string.Equals(baseTime, baseTimeSettings.CurrentDay, StringComparison.OrdinalIgnoreCase)) {
                // Start of current day.
                return new DateTime(origin.Date.Year, origin.Date.Month, origin.Date.Day, 0, 0, 0, origin.Kind);
            }
            if (string.Equals(baseTime, baseTimeSettings.CurrentHour, StringComparison.OrdinalIgnoreCase)) {
                // Start of current hour.
                return new DateTime(origin.Year, origin.Month, origin.Day, origin.Hour, 0, 0, origin.Kind);
            }
            if (string.Equals(baseTime, baseTimeSettings.CurrentMinute, StringComparison.OrdinalIgnoreCase)) {
                // Start of current minute.
                return new DateTime(origin.Year, origin.Month, origin.Day, origin.Hour, origin.Minute, 0, origin.Kind);
            }
            if (string.Equals(baseTime, baseTimeSettings.CurrentSecond, StringComparison.OrdinalIgnoreCase)) {
                // Start of current second.
                return new DateTime(origin.Year, origin.Month, origin.Day, origin.Hour, origin.Minute, origin.Second, origin.Kind);
            }
            if (string.Equals(baseTime, baseTimeSettings.Now, StringComparison.OrdinalIgnoreCase) || string.Equals(baseTime, baseTimeSettings.NowAlt, StringComparison.OrdinalIgnoreCase)) {
                // Current time.
                return origin;
            }

            throw new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Error_InvalidBaseDate, baseTime));
        }


        /// <summary>
        /// Adjusts a <see cref="DateTime"/> based on the specified time unit and quantity.
        /// </summary>
        /// <param name="baseDate">
        ///   The <see cref="DateTime"/> to adjust.
        /// </param>
        /// <param name="unit">
        ///   The time unit.
        /// </param>
        /// <param name="quantity">
        ///   The time unit quantity.
        /// </param>
        /// <param name="add">
        ///   Indicates if the time span should be added to or removed from the <paramref name="baseDate"/>.
        /// </param>
        /// <param name="timeOffsetSettings">
        ///   The time offset settings.
        /// </param>
        /// <returns>
        ///   The adjusted <see cref="DateTime"/>.
        /// </returns>
        private static DateTime ApplyOffset(DateTime baseDate, string unit, double quantity, bool add, RelativityTimeOffsetSettings timeOffsetSettings) {
            if (quantity == 0) {
                return baseDate;
            }

            if (string.Equals(unit, timeOffsetSettings.Years, StringComparison.OrdinalIgnoreCase)) {
                var wholeQuantity = Convert.ToInt32(quantity);
                return add
                    ? baseDate.AddYears(wholeQuantity)
                    : baseDate.AddYears(-1 * wholeQuantity);
            }
            if (string.Equals(unit, timeOffsetSettings.Months, StringComparison.OrdinalIgnoreCase)) {
                var wholeQuantity = Convert.ToInt32(quantity);
                return add
                    ? baseDate.AddMonths(wholeQuantity)
                    : baseDate.AddMonths(-1 * wholeQuantity);
            }
            if (string.Equals(unit, timeOffsetSettings.Weeks, StringComparison.OrdinalIgnoreCase)) {
                return add
                    ? baseDate.AddDays(7 * quantity)
                    : baseDate.AddDays(-7 * quantity);
            }
            if (string.Equals(unit, timeOffsetSettings.Days, StringComparison.OrdinalIgnoreCase)) {
                return add
                    ? baseDate.AddDays(quantity)
                    : baseDate.AddDays(-1 * quantity);
            }
            if (string.Equals(unit, timeOffsetSettings.Hours, StringComparison.OrdinalIgnoreCase)) {
                return add
                    ? baseDate.AddHours(quantity)
                    : baseDate.AddHours(-1 * quantity);
            }
            if (string.Equals(unit, timeOffsetSettings.Minutes, StringComparison.OrdinalIgnoreCase)) {
                return add
                    ? baseDate.AddMinutes(quantity)
                    : baseDate.AddMinutes(-1 * quantity);
            }
            if (string.Equals(unit, timeOffsetSettings.Seconds, StringComparison.OrdinalIgnoreCase)) {
                return add
                    ? baseDate.AddSeconds(quantity)
                    : baseDate.AddSeconds(-1 * quantity);
            }
            if (string.Equals(unit, timeOffsetSettings.Milliseconds, StringComparison.OrdinalIgnoreCase)) {
                return add
                    ? baseDate.AddMilliseconds(quantity)
                    : baseDate.AddMilliseconds(-1 * quantity);
            }

            return baseDate;
        }


        /// <summary>
        /// Tries to parse a duration string.
        /// </summary>
        /// <param name="durationString">
        ///   The duration string.
        /// </param>
        /// <param name="timeSpan">
        ///   The parsed time span, if successful.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the duration string was parsed successfully, or 
        ///   <see langword="false"/> otherwise.
        /// </returns>
        private bool TryParseDuration(string durationString, out TimeSpan timeSpan) {
            var match = DurationRegex.Match(durationString);
            if (!match.Success) {
                timeSpan = default;
                return false;
            }

            var unit = match.Groups["unit"].Value;
            var quantity = Convert.ToDouble(match.Groups["count"].Value, CultureInfo);

            if (string.Equals(unit, TimeOffsetSettings.Weeks, StringComparison.OrdinalIgnoreCase)) {
                timeSpan = TimeSpan.FromTicks((long) (TimeSpan.TicksPerDay * 7 * quantity));
            }
            else if (string.Equals(unit, TimeOffsetSettings.Days, StringComparison.OrdinalIgnoreCase)) {
                timeSpan = TimeSpan.FromTicks((long) (TimeSpan.TicksPerDay * quantity));
            }
            else if (string.Equals(unit, TimeOffsetSettings.Hours, StringComparison.OrdinalIgnoreCase)) {
                timeSpan = TimeSpan.FromTicks((long) (TimeSpan.TicksPerHour * quantity));
            }
            else if (string.Equals(unit, TimeOffsetSettings.Minutes, StringComparison.OrdinalIgnoreCase)) {
                timeSpan = TimeSpan.FromTicks((long) (TimeSpan.TicksPerMinute * quantity));
            }
            else if (string.Equals(unit, TimeOffsetSettings.Seconds, StringComparison.OrdinalIgnoreCase)) {
                timeSpan = TimeSpan.FromTicks((long) (TimeSpan.TicksPerSecond * quantity));
            }
            else if (string.Equals(unit, TimeOffsetSettings.Milliseconds, StringComparison.OrdinalIgnoreCase)) {
                timeSpan = TimeSpan.FromTicks((long) (TimeSpan.TicksPerMillisecond * quantity));
            }
            else {
                timeSpan = default;
            }

            return true;
        }

    }
}
