using System;
using System.Globalization;
using System.Linq;
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
        internal static string CreateDurationRegexPattern(RelativityTimeOffsetSettings timeOffsetSettings, CultureInfo cultureInfo) {
            if (timeOffsetSettings == null) {
                throw new ArgumentNullException(nameof(timeOffsetSettings));
            }
            if (cultureInfo == null) {
                throw new ArgumentNullException(nameof(cultureInfo));
            }

            var timeSpanUnits = new[] {
                timeOffsetSettings.Weeks,
                timeOffsetSettings.Days,
                timeOffsetSettings.Hours,
                timeOffsetSettings.Minutes,
                timeOffsetSettings.Seconds,
                timeOffsetSettings.Milliseconds
            }.Where(x => x != null).ToArray();

            return string.Concat(
                @"^\s*(?:(?:",
                @"(?<count>[0-9]+)\s*(?<unit>",
                string.Join(
                    "|",
                    timeSpanUnits.Select(x => EscapeRegexSpecialCharacters(x!))
                ),
                @")",
                @")|(?:",
                @"(?<count>[0-9]+",
                EscapeRegexSpecialCharacters(cultureInfo.NumberFormat?.NumberDecimalSeparator ?? CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator),
                @"[0-9]+)\s*(?<unit>",
                string.Join(
                    "|",
                    timeSpanUnits.Select(x => EscapeRegexSpecialCharacters(x!))
                ),
                @")))\s*$"
            );
        }


        internal static string CreateRelativeDateTimeRegexPattern(RelativityBaseTimeSettings baseTimeSettings, RelativityTimeOffsetSettings timeOffsetSettings, CultureInfo cultureInfo) {
            if (baseTimeSettings == null) {
                throw new ArgumentNullException(nameof(baseTimeSettings));
            }
            if (timeOffsetSettings == null) {
                throw new ArgumentNullException(nameof(timeOffsetSettings));
            }
            if (cultureInfo == null) {
                throw new ArgumentNullException(nameof(cultureInfo));
            }
            
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

            var timeOffsets = new[] {
                timeOffsetSettings.Years,
                timeOffsetSettings.Months,
                timeOffsetSettings.Weeks,
                timeOffsetSettings.Days,
                timeOffsetSettings.Hours,
                timeOffsetSettings.Minutes,
                timeOffsetSettings.Seconds,
                timeOffsetSettings.Milliseconds
            }.Where(x => x != null).ToArray();

            var fractionalTimeOffsets = new[] {
                timeOffsetSettings.Weeks,
                timeOffsetSettings.Days,
                timeOffsetSettings.Hours,
                timeOffsetSettings.Minutes,
                timeOffsetSettings.Seconds,
                timeOffsetSettings.Milliseconds
            }.Where(x => x != null).ToArray();

            return string.Concat(
                @"^\s*(?<base>",
                string.Join(
                    "|",
                    baseTimes.Select(x => EscapeRegexSpecialCharacters(x!))
                ),
                @")\s*(?:(?<operator>\+|-)\s*",
                // Units that can be expresses as whole numbers. Guaranteed to have length > 0
                @"(?:(?<count>[0-9]+)\s*(?<unit>",
                string.Join(
                    "|",
                    timeOffsets.Select(x => EscapeRegexSpecialCharacters(x!))
                ),
                @")",
                fractionalTimeOffsets.Length == 0
                    ? string.Empty
                    : string.Concat(
                        // Units that can be expressed as fractions.
                        @"|(?:(?<count>[0-9]+",
                        EscapeRegexSpecialCharacters(cultureInfo.NumberFormat?.NumberDecimalSeparator ?? CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator),
                        @"[0-9]+)\s*(?<unit>",
                        string.Join(
                            "|",
                            fractionalTimeOffsets.Select(x => EscapeRegexSpecialCharacters(x!))
                        ),
                        @"))"
                    ),
                @"))?\s*$"
            );
        }

    }
}
