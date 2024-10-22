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
        internal Parser(RelativityParserConfiguration config, TimeZoneInfo? timeZone) : this(
            config?.CultureInfo!, 
            timeZone ?? TimeZoneInfo.Local, 
            config?.BaseTimeSettings!, 
            config?.TimeOffsetSettings!
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
        internal Parser(
            CultureInfo cultureInfo,
            TimeZoneInfo timeZone,
            RelativityBaseTimeSettings baseTimeSettings,
            RelativityTimeOffsetSettings timeOffsetSettings
        ) : base(
            cultureInfo ?? throw new ArgumentNullException(nameof(cultureInfo)),
            timeZone ?? throw new ArgumentNullException(nameof(timeZone)),
            baseTimeSettings ?? throw new ArgumentNullException(nameof(baseTimeSettings)),
            timeOffsetSettings ?? throw new ArgumentNullException(nameof(timeOffsetSettings)),
            RegexHelper.GetOrCreateRelativeDateTimeRegex(baseTimeSettings!, timeOffsetSettings!, cultureInfo!),
            RegexHelper.GetOrCreateDurationRegex(timeOffsetSettings!, cultureInfo!)
        ) { }

    }
}
