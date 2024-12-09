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
        /// <param name="truncate">
        ///   When <see langword="true"/>, the duration string will be rounded up to the nearest duration unit applicable to the <paramref name="timeSpan"/>.
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
        ///   When <paramref name="truncate"/> is <see langword="true"/>, the duration string will 
        ///   be rounded away from zero to the nearest duration unit applicable to the <paramref name="timeSpan"/>. 
        ///   For example, if the <paramref name="timeSpan"/> is greater than one day and <paramref name="truncate"/> 
        ///   is <see langword="true"/>, the duration string will be rounded up to the next day.
        /// </para>
        /// 
        /// <para>
        ///   Truncating the duration string is useful when you want to display a friendlier 
        ///   duration at the expense of accuracy. The duration string will never be rounded 
        ///   up or down further than to the nearest day.
        /// </para>
        /// 
        /// </remarks>
        public static string ConvertToDuration(this IRelativityParser parser, TimeSpan timeSpan, bool truncate = false) {
            if (parser == null) {
                throw new ArgumentNullException(nameof(parser));
            }

            if (Math.Abs(timeSpan.TotalDays) >= 1) {
                return string.Format(parser.CultureInfo, "{0}{1}", TruncateIfRequired(timeSpan.TotalDays), parser.TimeOffsetSettings.Days);
            }

            if (Math.Abs(timeSpan.TotalHours) >= 1) {
                return string.Format(parser.CultureInfo, "{0}{1}", TruncateIfRequired(timeSpan.TotalHours), parser.TimeOffsetSettings.Hours);
            }

            if (Math.Abs(timeSpan.TotalMinutes) >= 1) {
                return string.Format(parser.CultureInfo, "{0}{1}", TruncateIfRequired(timeSpan.TotalMinutes), parser.TimeOffsetSettings.Minutes);
            }

            if (Math.Abs(timeSpan.TotalSeconds) >= 1) {
                return string.Format(parser.CultureInfo, "{0}{1}", TruncateIfRequired(timeSpan.TotalSeconds), parser.TimeOffsetSettings.Seconds);
            }

            return string.Format(parser.CultureInfo, "{0}{1}", TruncateIfRequired(timeSpan.TotalMilliseconds), parser.TimeOffsetSettings.Milliseconds);

            double TruncateIfRequired(double value) => truncate 
                ? value > 0
                    ? Math.Ceiling(value)
                    : Math.Floor(value)
                : value;
        }

    }
}
