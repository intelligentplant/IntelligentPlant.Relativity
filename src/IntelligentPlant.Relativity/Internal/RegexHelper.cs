using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IntelligentPlant.Relativity.Internal {

    /// <summary>
    /// Helper methods for creating regular expressions for parsing relative timestamps and durations.
    /// </summary>
    internal static class RegexHelper {

        /// <summary>
        /// Escaped versions of characters that have special meanings in regular expressions unless escaped.
        /// </summary>
        private static readonly string[] s_regexSpecialCharacterEscapes = { @"\\", @"\.", @"\$", @"\^", @"\{", @"\[", @"\(", @"\|", @"\)", @"\*", @"\+", @"\?" };

        /// <summary>
        /// Holds precompiled regular expressions so that we only create an entry once for each 
        /// combination of culture and keywords.
        /// </summary>
        private static readonly ConcurrentDictionary<string, Regex> s_regexCache = new ConcurrentDictionary<string, Regex>(StringComparer.OrdinalIgnoreCase);


        /// <summary>
        /// Escapes any characters in the specified string that are special characters in regular 
        /// expressions.
        /// </summary>
        /// <param name="s">
        ///   The string.
        /// </param>
        /// <returns>
        ///   The escaped string.
        /// </returns>
        private static string EscapeRegexSpecialCharacters(string s) {
            return s_regexSpecialCharacterEscapes.Aggregate(s, (current, specialCharacter) => Regex.Replace(current, specialCharacter, specialCharacter));
        }


        /// <summary>
        /// Gets or creates a regular expression for parsing durations.
        /// </summary>
        /// <param name="timeOffsetSettings">
        ///   The time offset settings.
        /// </param>
        /// <param name="cultureInfo">
        ///   The culture to use for parsing numbers.
        /// </param>
        /// <returns>
        ///   A regular expression for parsing durations.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="timeOffsetSettings"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="cultureInfo"/> is <see langword="null"/>.
        /// </exception>
        internal static Regex GetOrCreateDurationRegex(RelativityTimeOffsetSettings timeOffsetSettings, CultureInfo cultureInfo) {
            if (timeOffsetSettings == null) {
                throw new ArgumentNullException(nameof(timeOffsetSettings));
            }
            if (cultureInfo == null) {
                throw new ArgumentNullException(nameof(cultureInfo));
            }

            // Key consists of the time offset settings and the decimal separator for the culture.
            var decimalSeparator = cultureInfo.NumberFormat?.NumberDecimalSeparator ?? CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator;
            var key = $"to:{Convert.ToBase64String(Encoding.UTF8.GetBytes(timeOffsetSettings.ToString()))}:{decimalSeparator}";
            return s_regexCache.GetOrAdd(key, _ => CompileRegex(CreateDurationRegexPattern(timeOffsetSettings, cultureInfo)));
        }


        /// <summary>
        /// Gets or creates a regular expression for parsing relative timestamps.
        /// </summary>
        /// <param name="baseTimeSettings">
        ///   The base time settings.
        /// </param>
        /// <param name="timeOffsetSettings">
        ///   The time offset settings.
        /// </param>
        /// <param name="cultureInfo">
        ///   The culture to use for parsing numbers.
        /// </param>
        /// <returns>
        ///   A regular expression for parsing relative timestamps.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="baseTimeSettings"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="timeOffsetSettings"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="cultureInfo"/> is <see langword="null"/>.
        /// </exception>
        internal static Regex GetOrCreateRelativeDateTimeRegex(RelativityBaseTimeSettings baseTimeSettings, RelativityTimeOffsetSettings timeOffsetSettings, CultureInfo cultureInfo) {
            if (baseTimeSettings == null) {
                throw new ArgumentNullException(nameof(baseTimeSettings));
            }
            if (timeOffsetSettings == null) {
                throw new ArgumentNullException(nameof(timeOffsetSettings));
            }
            if (cultureInfo == null) {
                throw new ArgumentNullException(nameof(cultureInfo));
            }

            // Key consists of the base time settings, the time offset settings and the decimal separator for the culture.
            var decimalSeparator = cultureInfo.NumberFormat?.NumberDecimalSeparator ?? CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator;
            var key = $"bt:{Convert.ToBase64String(Encoding.UTF8.GetBytes(baseTimeSettings.ToString()))}:{Convert.ToBase64String(Encoding.UTF8.GetBytes(timeOffsetSettings.ToString()))}:{decimalSeparator}";
            return s_regexCache.GetOrAdd(key, _ => CompileRegex(CreateRelativeDateTimeRegexPattern(baseTimeSettings, timeOffsetSettings, cultureInfo)));
        }


        /// <summary>
        /// Creates a regular expression capture group for a duration unit.
        /// </summary>
        /// <param name="name">
        ///   The name of the capture group.
        /// </param>
        /// <param name="symbol">
        ///   The symbol for the unit.
        /// </param>
        /// <param name="allowFractional">
        ///   Specifies whether fractional values are allowed.
        /// </param>
        /// <param name="cultureInfo">
        ///   The culture to use for parsing numbers.
        /// </param>
        /// <returns>
        ///   The regular expression capture group.
        /// </returns>
        private static string CreateDurationUnitRegexPattern(string name, string symbol, bool allowFractional, CultureInfo cultureInfo) {
            return allowFractional
                ? $@"(?:(?<{name}>\d+(?:{EscapeRegexSpecialCharacters(cultureInfo.NumberFormat?.NumberDecimalSeparator ?? CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator)}\d+)?)\s*{EscapeRegexSpecialCharacters(symbol)})?"
                : $@"(?:(?<{name}>\d+)\s*{EscapeRegexSpecialCharacters(symbol)})?";
        }


        /// <summary>
        /// Creates a regular expression pattern for parsing durations.
        /// </summary>
        /// <param name="timeOffsetSettings">
        ///   The time offset settings.
        /// </param>
        /// <param name="cultureInfo">
        ///   The culture to use for parsing numbers.
        /// </param>
        /// <returns>
        ///   A regular expression pattern for parsing durations.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="timeOffsetSettings"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="cultureInfo"/> is <see langword="null"/>.
        /// </exception>
        private static string CreateDurationRegexPattern(RelativityTimeOffsetSettings timeOffsetSettings, CultureInfo cultureInfo) {
            var timeSpanUnits = new [] {
                new DurationUnit("weeks", timeOffsetSettings.Weeks, true),
                new DurationUnit("days", timeOffsetSettings.Days, true),
                new DurationUnit("hours", timeOffsetSettings.Hours, true),
                new DurationUnit("minutes", timeOffsetSettings.Minutes, true),
                new DurationUnit("seconds", timeOffsetSettings.Seconds, true),
                new DurationUnit("milliseconds", timeOffsetSettings.Milliseconds, true)
            }.Where(x => x.Symbol != null);

            return string.Concat(@"^\s*", string.Join(@"\s*", timeSpanUnits.Select(x => CreateDurationUnitRegexPattern(x.Name, x.Symbol!, x.AllowFractional, cultureInfo))), @"\s*$");
        }


        /// <summary>
        /// Creates a regular expression pattern for parsing relative timestamps.
        /// </summary>
        /// <param name="baseTimeSettings">
        ///   The base time settings.
        /// </param>
        /// <param name="timeOffsetSettings">
        ///   The time offset settings.
        /// </param>
        /// <param name="cultureInfo">
        ///   The culture to use for parsing numbers.
        /// </param>
        /// <returns>
        ///   A regular expression pattern for parsing durations.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="baseTimeSettings"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="timeOffsetSettings"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="cultureInfo"/> is <see langword="null"/>.
        /// </exception>
        private static string CreateRelativeDateTimeRegexPattern(RelativityBaseTimeSettings baseTimeSettings, RelativityTimeOffsetSettings timeOffsetSettings, CultureInfo cultureInfo) {
            var baseTimes = new[] {
                baseTimeSettings.CurrentYear,
                baseTimeSettings.CurrentMonth,
                baseTimeSettings.CurrentWeek,
                baseTimeSettings.CurrentDay,
                baseTimeSettings.CurrentHour,
                baseTimeSettings.CurrentMinute,
                baseTimeSettings.CurrentSecond,
                baseTimeSettings.Now,
                baseTimeSettings.NowAlt
            }.Where(x => x != null).ToArray();

            var timeOffsetUnits = new[] {
                new DurationUnit("years", timeOffsetSettings.Years, false),
                new DurationUnit("months", timeOffsetSettings.Months, false),
                new DurationUnit("weeks", timeOffsetSettings.Weeks, true),
                new DurationUnit("days", timeOffsetSettings.Days, true),
                new DurationUnit("hours", timeOffsetSettings.Hours, true),
                new DurationUnit("minutes", timeOffsetSettings.Minutes, true),
                new DurationUnit("seconds", timeOffsetSettings.Seconds, true),
                new DurationUnit("milliseconds", timeOffsetSettings.Milliseconds, true)
            }.Where(x => x.Symbol != null);

            return string.Concat(
                @"^\s*(?<base>",
                string.Join(
                    "|",
                    baseTimes.Select(x => EscapeRegexSpecialCharacters(x!))
                ),
                @")\s*(?:(?<operator>\+|-)\s*",
                string.Join(@"\s*", timeOffsetUnits.Select(x => CreateDurationUnitRegexPattern(x.Name, x.Symbol!, x.AllowFractional, cultureInfo))),
                @")?\s*$"
            );
        }


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


        private readonly struct DurationUnit { 
        
            public string Name { get; }

            public string? Symbol { get; }

            public bool AllowFractional { get; }


            public DurationUnit(string name, string? symbol, bool allowFractional) {
                Name = name;
                Symbol = symbol;
                AllowFractional = allowFractional;
            }

        }

    }
}
