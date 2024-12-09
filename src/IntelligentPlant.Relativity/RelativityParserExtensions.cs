using System;
using System.Globalization;

namespace IntelligentPlant.Relativity {

    /// <summary>
    /// Extension methods for <see cref="IRelativityParser"/>.
    /// </summary>
    public static class RelativityParserExtensions {

        /// <summary>
        /// Tries to convert an absolute or relative date string to a UTC <see cref="DateTime"/> 
        /// value using the current time in the parser's time zone as the relative time origin.
        /// </summary>
        /// <param name="parser">
        ///   The parser.
        /// </param>
        /// <param name="dateString">
        ///   The date string.
        /// </param>
        /// <param name="utcDateTime">
        ///   The parsed UTC <see cref="DateTime"/>.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the date string was successfully parsed; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="parser"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        /// 
        /// <para>
        ///   Relative timestamps must be specified in a format that matches the configured 
        ///   <see cref="IRelativityParser.BaseTimeSettings"/> and <see cref="IRelativityParser.TimeOffsetSettings"/> 
        ///   for the parser.
        /// </para>
        /// 
        /// <para>
        ///   Absolute timestamps are parsed using the configured <see cref="IRelativityParser.CultureInfo"/>. For 
        ///   maximum compatibility, ISO 8601 format is recommended when specifying an absolute 
        ///   timestamp.
        /// </para>
        /// 
        /// </remarks>
        public static bool TryConvertToUtcDateTime(this IRelativityParser parser, string dateString, out DateTime utcDateTime) {
            if (parser == null) {
                throw new ArgumentNullException(nameof(parser));
            }
            return parser.TryConvertToUtcDateTime(dateString, null, out utcDateTime);
        }


        /// <summary>
        /// Converts an absolute or relative date string to a UTC <see cref="DateTime"/> 
        /// value using the current time in the parser's time zone as the relative time origin.
        /// </summary>
        /// <param name="parser">
        ///   The parser.
        /// </param>
        /// <param name="dateString">
        ///   The date string.
        /// </param>
        /// <returns>
        ///   The parsed UTC <see cref="DateTime"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="parser"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="dateString"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="FormatException">
        ///   <paramref name="dateString"/> is not a valid date string.
        /// </exception>
        /// <remarks>
        /// 
        /// <para>
        ///   Relative timestamps must be specified in a format that matches the configured 
        ///   <see cref="IRelativityParser.BaseTimeSettings"/> and <see cref="IRelativityParser.TimeOffsetSettings"/> 
        ///   for the parser.
        /// </para>
        /// 
        /// <para>
        ///   Absolute timestamps are parsed using the configured <see cref="IRelativityParser.CultureInfo"/>. For 
        ///   maximum compatibility, ISO 8601 format is recommended when specifying an absolute 
        ///   timestamp.
        /// </para>
        /// 
        /// </remarks>
        public static DateTime ConvertToUtcDateTime(this IRelativityParser parser, string dateString) {
            if (parser == null) {
                throw new ArgumentNullException(nameof(parser));
            }
            return parser.ConvertToUtcDateTime(dateString, null);
        }


        /// <summary>
        /// Tests if a string is a valid absolute or relative timestamp.
        /// </summary>
        /// <param name="parser">
        ///   The parser.
        /// </param>
        /// <param name="dateString">
        ///   The date string.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the date string is valid; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="parser"/> is <see langword="null"/>.
        /// </exception>
        public static bool IsValidDateTime(this IRelativityParser parser, string dateString) {
            if (parser == null) {
                throw new ArgumentNullException(nameof(parser));
            }
            return parser.TryConvertToUtcDateTime(dateString, out _);
        }


        /// <summary>
        /// Tests if a string is a valid time span literal or Relativity duration.
        /// </summary>
        /// <param name="parser">
        ///   The parser.
        /// </param>
        /// <param name="durationString">
        ///   The duration string.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the duration string is valid; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="parser"/> is <see langword="null"/>.
        /// </exception>
        public static bool IsValidTimeSpan(this IRelativityParser parser, string durationString) {
            if (parser == null) {
                throw new ArgumentNullException(nameof(parser));
            }
            return parser.TryConvertToTimeSpan(durationString, out _);
        }


        /// <summary>
        /// Converts a <see cref="TimeSpan"/> to a Relativity duration string.
        /// </summary>
        /// <param name="parser">
        ///   The parser.
        /// </param>
        /// <param name="timeSpan">
        ///   The time span.
        /// </param>
        /// <param name="decimalPlaces">
        ///   The number of decimal places to use when formatting the duration string. If greater 
        ///   than or equal to zero, the duration string will be rounded away from zero to the 
        ///   specified number of decimal places e.g. specifying one decimal place when a time 
        ///   span of <c>00:00:00.1234567</c> is specified will result in a duration string of 
        ///   <c>123.5MS</c> being returned (or its equivalent localized value).
        /// </param>
        /// <param name="unit">
        ///   The time unit to use in the duration string. Specify <see langword="null"/> to infer 
        ///   a unit based on the magnitude of the <paramref name="timeSpan"/>.
        /// </param>
        /// <returns>
        ///   The duration string.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="parser"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        /// 
        /// <para>
        ///   When <paramref name="unit"/> is <see langword="null"/>, the unit used in the 
        ///   duration string is determined by the overall magnitude of the <see cref="TimeSpan"/>. 
        ///   For example, a time span greater than or equal to one day will be formatted as whole 
        ///   and fractional days, a time span greater than or equal to one hour will be formatted 
        ///   as whole and fractional hours, and so on.
        /// </para>
        /// 
        /// <para>
        ///   You can specify the number of decimal places to round the duration to using the 
        ///   <paramref name="decimalPlaces"/> parameter. When a value of zero or greater is 
        ///   specified, the duration string will be rounded away from zero to the specified 
        ///   number of decimal places. For example, specifying one decimal place when a time 
        ///   span of <c>00:00:00.1234567</c> is specified will result in a duration string of 
        ///   <c>123.5MS</c> being returned (or its equivalent localized value).
        /// </para>
        /// 
        /// </remarks>
        public static string ConvertToDuration(this IRelativityParser parser, TimeSpan timeSpan, int decimalPlaces = -1, string? unit = null) {
            if (parser == null) {
                throw new ArgumentNullException(nameof(parser));
            }

            if (unit != null) {
                if (unit.Equals(parser.TimeOffsetSettings.Weeks, StringComparison.OrdinalIgnoreCase)) {
                    return Format(timeSpan.TotalDays / 7, parser.TimeOffsetSettings.Weeks!);
                }
                else if (unit.Equals(parser.TimeOffsetSettings.Days, StringComparison.OrdinalIgnoreCase)) {
                    return Format(timeSpan.TotalDays, parser.TimeOffsetSettings.Days!);
                }
                else if (unit.Equals(parser.TimeOffsetSettings.Hours, StringComparison.OrdinalIgnoreCase)) {
                    return Format(timeSpan.TotalHours, parser.TimeOffsetSettings.Hours!);
                }
                else if (unit.Equals(parser.TimeOffsetSettings.Minutes, StringComparison.OrdinalIgnoreCase)) {
                    return Format(timeSpan.TotalMinutes, parser.TimeOffsetSettings.Minutes!);
                }
                else if (unit.Equals(parser.TimeOffsetSettings.Seconds, StringComparison.OrdinalIgnoreCase)) {
                    return Format(timeSpan.TotalSeconds, parser.TimeOffsetSettings.Seconds!);
                }
                else if (unit.Equals(parser.TimeOffsetSettings.Milliseconds, StringComparison.OrdinalIgnoreCase)) {
                    return Format(timeSpan.TotalMilliseconds, parser.TimeOffsetSettings.Milliseconds!);
                }

                throw new InvalidOperationException("The specified time unit is not supported.");
            }

            if (parser.TimeOffsetSettings.Days != null && Math.Abs(timeSpan.TotalDays) >= 1) {
                return Format(timeSpan.TotalDays, parser.TimeOffsetSettings.Days);
            }

            if (parser.TimeOffsetSettings.Hours != null && Math.Abs(timeSpan.TotalHours) >= 1) {
                return Format(timeSpan.TotalHours, parser.TimeOffsetSettings.Hours);
            }

            if (parser.TimeOffsetSettings.Minutes != null && Math.Abs(timeSpan.TotalMinutes) >= 1) {
                return Format(timeSpan.TotalMinutes, parser.TimeOffsetSettings.Minutes);
            }

            if (parser.TimeOffsetSettings.Seconds != null && Math.Abs(timeSpan.Seconds) >= 1) {
                return Format(timeSpan.TotalSeconds, parser.TimeOffsetSettings.Seconds);
            }

            if (parser.TimeOffsetSettings.Milliseconds != null) {
                return Format(timeSpan.TotalMilliseconds, parser.TimeOffsetSettings.Milliseconds);
            }

            throw new InvalidOperationException("Could not determine the unit to use for the formatted duration string.");

            string Format(double value, string unit) {
                return string.Format(parser.CultureInfo, "{0}{1}", RoundAwayFromZeroIfRequired(value), unit);
            }

            double RoundAwayFromZeroIfRequired(double value) {
                if (decimalPlaces < 0) {
                    return value;
                }
                var factor = Math.Pow(10, decimalPlaces);
                return value > 0
                    ? Math.Ceiling(value * factor) / factor
                    : Math.Floor(value * factor) / factor;
            }
        }

    }
}
