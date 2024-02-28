using System;

using IntelligentPlant.Relativity;

namespace System {

    /// <summary>
    /// Extension methods for <see cref="string"/>.
    /// </summary>
    public static class RelativityStringExtensions {

        /// <summary>
        /// Tries to convert an absolute or relative date string to a UTC <see cref="DateTime"/> 
        /// value.
        /// </summary>
        /// <param name="dateString">
        ///   The date string.
        /// </param>
        /// <param name="parser">
        ///   The parser. Specify <see langword="null"/> to use <see cref="RelativityParser.Current"/>.
        /// </param>
        /// <param name="origin">
        ///   The origin date and time to use when parsing relative dates. Specify <see langword="null"/> 
        ///   to use the current date and time in the parser's <see cref="IRelativityParser.TimeZone"/>.
        /// </param>
        /// <param name="utcDateTime">
        ///   The parsed UTC <see cref="DateTime"/>.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the date string was successfully parsed; otherwise, <see langword="false"/>.
        /// </returns>
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
        public static bool TryConvertToUtcDateTime(this string dateString, IRelativityParser? parser, DateTime? origin, out DateTime utcDateTime) {
            return (parser ?? RelativityParser.Current).TryConvertToUtcDateTime(dateString, origin, out utcDateTime);
        }


        /// <summary>
        /// Converts an absolute or relative date string to a UTC <see cref="DateTime"/> value.
        /// </summary>
        /// <param name="dateString">
        ///   The date string.
        /// </param>
        /// <param name="parser">
        ///   The parser. Specify <see langword="null"/> to use <see cref="RelativityParser.Current"/>.
        /// </param>
        /// <param name="origin">
        ///   The origin date and time to use when parsing relative dates. Specify <see langword="null"/> 
        ///   to use the current date and time in the parser's <see cref="TimeZone"/>.
        /// </param>
        /// <returns>
        ///   The parsed UTC <see cref="DateTime"/>.
        /// </returns>
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
        public static DateTime ConvertToUtcDateTime(this string dateString, IRelativityParser? parser, DateTime? origin) {
            return (parser ?? RelativityParser.Current).ConvertToUtcDateTime(dateString, origin);
        }


        /// <summary>
        /// Tries to convert an absolute or relative date string to a UTC <see cref="DateTime"/> 
        /// value using the current time in the specified <see cref="IRelativityParser"/>'s time 
        /// zone as the relative time origin.
        /// </summary>
        /// <param name="dateString">
        ///   The date string.
        /// </param>
        /// <param name="parser">
        ///   The parser. Specify <see langword="null"/> to use <see cref="RelativityParser.Current"/>.
        /// </param>
        /// <param name="utcDateTime">
        ///   The parsed UTC <see cref="DateTime"/>.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the date string was successfully parsed; otherwise, <see langword="false"/>.
        /// </returns>
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
        public static bool TryConvertToUtcDateTime(this string dateString, IRelativityParser? parser, out DateTime utcDateTime) {
            return (parser ?? RelativityParser.Current).TryConvertToUtcDateTime(dateString, out utcDateTime);
        }


        /// <summary>
        /// Converts an absolute or relative date string to a UTC <see cref="DateTime"/> 
        /// value using the current time in the specified <see cref="IRelativityParser"/>'s time 
        /// zone as the relative time origin.
        /// </summary>
        /// <param name="dateString">
        ///   The date string.
        /// </param>
        /// <param name="parser">
        ///   The parser. Specify <see langword="null"/> to use <see cref="RelativityParser.Current"/>.
        /// </param>
        /// <returns>
        ///   The parsed UTC <see cref="DateTime"/>.
        /// </returns>
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
        public static DateTime ConvertToUtcDateTime(this string dateString, IRelativityParser? parser) {
            return (parser ?? RelativityParser.Current).ConvertToUtcDateTime(dateString);
        }


        /// <summary>
        /// Tries to convert a time span literal or Relativity duration string to a <see cref="TimeSpan"/> 
        /// value.
        /// </summary>
        /// <param name="durationString">
        ///   The time span literal or duration string.
        /// </param>
        /// <param name="parser">
        ///   The parser. Specify <see langword="null"/> to use <see cref="RelativityParser.Current"/>.
        /// </param>
        /// <param name="timeSpan">
        ///   The parsed <see cref="TimeSpan"/>.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the duration was successfully parsed; otherwise, 
        ///   <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// 
        /// <para>
        ///   Duration strings must be specified in a format that matches the configured 
        ///   <see cref="IRelativityParser.TimeOffsetSettings"/> for the parser.
        /// </para>
        /// 
        /// <para>
        ///   Time span literals are parsed using the configured <see cref="IRelativityParser.CultureInfo"/>.
        /// </para>
        ///
        /// </remarks>
        public static bool TryConvertToTimeSpan(this string durationString, IRelativityParser? parser, out TimeSpan timeSpan) {
            return (parser ?? RelativityParser.Current).TryConvertToTimeSpan(durationString, out timeSpan);
        }


        /// <summary>
        /// Converts a time span literal or Relativity duration string to a <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="durationString">
        ///   The time span literal or duration string.
        /// </param>
        /// <param name="parser">
        ///   The parser. Specify <see langword="null"/> to use <see cref="RelativityParser.Current"/>.
        /// </param>
        /// <returns>
        ///   The parsed <see cref="TimeSpan"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="durationString"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="FormatException">
        ///   <paramref name="durationString"/> is not a valid timespan or Relativity duration string.
        /// </exception>
        /// <remarks>
        /// 
        /// <para>
        ///   Duration strings must be specified in a format that matches the configured 
        ///   <see cref="IRelativityParser.TimeOffsetSettings"/> for the parser.
        /// </para>
        /// 
        /// <para>
        ///   Time span literals are parsed using the configured <see cref="IRelativityParser.CultureInfo"/>.
        /// </para>
        ///
        /// </remarks>
        public static TimeSpan ConvertToTimeSpan(this string durationString, IRelativityParser? parser) {
            return (parser ?? RelativityParser.Current).ConvertToTimeSpan(durationString);
        }

    }
}
