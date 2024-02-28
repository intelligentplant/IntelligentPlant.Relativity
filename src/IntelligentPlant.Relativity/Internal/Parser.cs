using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace IntelligentPlant.Relativity.Internal {

    /// <summary>
    /// Default implementation of <see cref="IRelativityParser"/>.
    /// </summary>
    internal sealed class Parser : ParserBase {

        /// <summary>
        /// Creates a new <see cref="Parser"/> instance.
        /// </summary>
        /// <param name="config">
        ///   The parser configuration.
        /// </param>
        /// <param name="timeZone">
        ///   The parser time zone.
        /// </param>
        internal Parser(RelativityParserConfiguration config, TimeZoneInfo? timeZone) : base(
            config?.CultureInfo!, 
            timeZone ?? TimeZoneInfo.Local, 
            config?.BaseTimeSettings!, 
            config?.TimeOffsetSettings!,
            CompileRegex(RegexHelper.CreateRelativeDateTimeRegexPattern(config?.BaseTimeSettings!, config?.TimeOffsetSettings!, config?.CultureInfo!)),
            CompileRegex(RegexHelper.CreateDurationRegexPattern(config?.TimeOffsetSettings!, config?.CultureInfo!))
        ) { }


        /// <summary>
        /// Creates a new <see cref="Parser"/> instance.
        /// </summary>
        /// <param name="cultureInfo">
        ///   The culture to use for parsing timestamps and durations.
        /// </param>
        /// <param name="timeZone">
        ///   The time zone to use when parsing relative dates.
        /// </param>
        /// <param name="baseTimeSettings">
        ///   The base time keyword settings.
        /// </param>
        /// <param name="timeOffsetSettings">
        ///   The time offset keyword settings.
        /// </param>
        /// <param name="relativeDateTimeRegex">
        ///   The regular expression for parsing relative date and time strings.
        /// </param>
        /// <param name="durationRegex">
        ///   The regular expression for parsing duration strings.
        /// </param>
        internal Parser(
            CultureInfo cultureInfo,
            TimeZoneInfo timeZone,
            RelativityBaseTimeSettings baseTimeSettings,
            RelativityTimeOffsetSettings timeOffsetSettings,
            Regex relativeDateTimeRegex,
            Regex durationRegex
        ) : base(
            cultureInfo,
            timeZone,
            baseTimeSettings,
            timeOffsetSettings,
            relativeDateTimeRegex,
            durationRegex
        ) { }


        /// <summary>
        /// Creates a compiled regular expression for the specified pattern.
        /// </summary>
        /// <param name="pattern">
        ///   The pattern.
        /// </param>
        /// <returns>
        ///   The compiled regular expression.
        /// </returns>
        private static Regex CompileRegex(string pattern) {
            return new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
        }

    }
}
