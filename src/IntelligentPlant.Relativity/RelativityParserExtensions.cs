using System;

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

    }
}
