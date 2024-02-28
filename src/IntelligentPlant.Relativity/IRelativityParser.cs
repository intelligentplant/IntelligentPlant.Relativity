using System;
using System.Globalization;

namespace IntelligentPlant.Relativity {

    /// <summary>
    /// A Relativity parser.
    /// </summary>
    public interface IRelativityParser {

        /// <summary>
        /// The culture info used when parsing numbers, absolute dates and time span literals.
        /// </summary>
        CultureInfo CultureInfo { get; }

        /// <summary>
        /// The time zone used when parsing relative dates.
        /// </summary>
        TimeZoneInfo TimeZone { get; }

        /// <summary>
        /// The base time keyword settings.
        /// </summary>
        RelativityBaseTimeSettings BaseTimeSettings { get; }

        /// <summary>
        /// The time offset and duration keyword settings.
        /// </summary>
        RelativityTimeOffsetSettings TimeOffsetSettings { get; }


        /// <summary>
        /// Tries to convert an absolute or relative date string to a UTC <see cref="DateTime"/> value.
        /// </summary>
        /// <param name="dateString">
        ///   The date string.
        /// </param>
        /// <param name="origin">
        ///   The origin date and time to use when parsing relative dates. Specify <see langword="null"/> 
        ///   to use the current date and time in the parser's <see cref="TimeZone"/>.
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
        ///   Relative timestamps must be specified in a format that matches the configured <see cref="BaseTimeSettings"/> 
        ///   and <see cref="TimeOffsetSettings"/> for the parser.
        /// </para>
        /// 
        /// <para>
        ///   Absolute timestamps are parsed using the configured <see cref="CultureInfo"/>. For 
        ///   maximum compatibility, ISO 8601 format is recommended when specifying an absolute 
        ///   timestamp.
        /// </para>
        /// 
        /// </remarks>
        bool TryConvertToUtcDateTime(string dateString, DateTime? origin, out DateTime utcDateTime);


        /// <summary>
        /// Converts an absolute or relative date string to a UTC <see cref="DateTime"/> value.
        /// </summary>
        /// <param name="dateString">
        ///   The date string.
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
        ///   Relative timestamps must be specified in a format that matches the configured <see cref="BaseTimeSettings"/> 
        ///   and <see cref="TimeOffsetSettings"/> for the parser.
        /// </para>
        /// 
        /// <para>
        ///   Absolute timestamps are parsed using the configured <see cref="CultureInfo"/>. For 
        ///   maximum compatibility, ISO 8601 format is recommended when specifying an absolute 
        ///   timestamp.
        /// </para>
        /// 
        /// </remarks>
        DateTime ConvertToUtcDateTime(string dateString, DateTime? origin);


        /// <summary>
        /// Tries to convert a time span literal or Relativity duration string to a <see cref="TimeSpan"/> 
        /// value.
        /// </summary>
        /// <param name="durationString">
        ///   The time span literal or duration string.
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
        ///   <see cref="TimeOffsetSettings"/> for the parser.
        /// </para>
        /// 
        /// <para>
        ///   Time span literals are parsed using the configured <see cref="CultureInfo"/>.
        /// </para>
        ///
        /// </remarks>
        bool TryConvertToTimeSpan(string durationString, out TimeSpan timeSpan);


        /// <summary>
        /// Converts a time span literal or Relativity duration string to a <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="durationString">
        ///   The time span literal or duration string.
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
        ///   <see cref="TimeOffsetSettings"/> for the parser.
        /// </para>
        /// 
        /// <para>
        ///   Time span literals are parsed using the configured <see cref="CultureInfo"/>.
        /// </para>
        ///
        /// </remarks>
        TimeSpan ConvertToTimeSpan(string durationString);

    }
}
