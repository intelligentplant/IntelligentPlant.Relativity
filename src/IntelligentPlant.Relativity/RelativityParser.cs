using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace IntelligentPlant.Relativity {

    /// <summary>
    /// Class for parsing absolute and relative timestamps, and duration expressions into 
    /// <see cref="DateTime"/> and <see cref="TimeSpan"/> instances respectively.
    /// </summary>
    public class RelativityParser {

        /// <summary>
        /// Embedded file that contains culture-specific settings to load in at startup.
        /// </summary>
        private const string SettingsFileName = "Settings.csv";

        /// <summary>
        /// Escaped versions of characters that have special meanings in regular expressions unless escaped.
        /// </summary>
        private static readonly string[] s_regexSpecialCharacterEscapes = { @"\\", @"\.", @"\$", @"\^", @"\{", @"\[", @"\(", @"\|", @"\)", @"\*", @"\+", @"\?" };

        /// <summary>
        /// Registered parser instances.
        /// </summary>
        private static readonly ConcurrentDictionary<string, RelativityParser> s_parserInstances = new ConcurrentDictionary<string, RelativityParser>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// The default parser. Uses <see cref="CultureInfo.InvariantCulture"/> for parsing. 
        /// </summary>
        public static RelativityParser Default { get; } = new RelativityParser(
            CultureInfo.InvariantCulture,
            new RelativityBaseTimeSettings(),
            new RelativityTimeOffsetSettings()
        );

        /// <summary>
        /// The <see cref="System.Globalization.CultureInfo"/> used during parsing.
        /// </summary>
        public CultureInfo CultureInfo { get; }

        /// <summary>
        /// The configured base time keywords.
        /// </summary>
        public RelativityBaseTimeSettings BaseTime { get; }

        /// <summary>
        /// The configured time offset keywords.
        /// </summary>
        public RelativityTimeOffsetSettings TimeOffset { get; }

        /// <summary>
        /// The regular expression pattern used to match relative timestamps.
        /// </summary>
        public string RelativeTimestampRegexPattern { get; }

        /// <summary>
        /// The regular expression pattern used to match durations.
        /// </summary>
        public string DurationRegexPattern { get; }

        /// <summary>
        /// The regular expression for parsing duration expressions.
        /// </summary>
        private Regex _timeSpanRegex;

        /// <summary>
        /// The regular expression for parsing relative timestamps.
        /// </summary>
        private Regex _relativeDateTimeRegex;


        /// <summary>
        /// Class initializer.
        /// </summary>
        static RelativityParser() {
            LoadDefaultParsers();
        }


        /// <summary>
        /// Creates a new <see cref="RelativityParser"/> object.
        /// </summary>
        /// <param name="baseTimeSettings">
        ///   The base time keywords to use.
        /// </param>
        /// <param name="timeOffsetSettings">
        ///   The time offset keywords to use.
        /// </param>
        /// <param name="cultureInfo">
        ///   The <see cref="System.Globalization.CultureInfo"/> to use when parsing.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="baseTimeSettings"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="timeOffsetSettings"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="cultureInfo"/> is <see langword="null"/>.
        /// </exception>
        public RelativityParser(CultureInfo cultureInfo, RelativityBaseTimeSettings baseTimeSettings, RelativityTimeOffsetSettings timeOffsetSettings) {
            CultureInfo = cultureInfo ?? throw new ArgumentNullException(nameof(cultureInfo));
            BaseTime = baseTimeSettings ?? throw new ArgumentNullException(nameof(baseTimeSettings));
            TimeOffset = timeOffsetSettings ?? throw new ArgumentNullException(nameof(timeOffsetSettings));

            var timeSpanUnits = new[] {
                TimeOffset.Weeks,
                TimeOffset.Days,
                TimeOffset.Hours,
                TimeOffset.Minutes,
                TimeOffset.Seconds,
                TimeOffset.Milliseconds
            }.Where(x => x != null).ToArray();

            DurationRegexPattern = string.Concat(
                @"^\s*(?<count>[0-9]+)\s*(?<unit>",
                string.Join(
                    "|",
                    timeSpanUnits.Select(x => EscapeRegexSpecialCharacters(x))
                ),
                @")\s*$"
            );
            _timeSpanRegex = new Regex(DurationRegexPattern, RegexOptions.IgnoreCase);

            var baseTimes = new[] {
                BaseTime.CurrentYear,
                BaseTime.CurrentMonth,
                BaseTime.CurrentWeek,
                BaseTime.CurrentDay,
                BaseTime.CurrentHour,
                BaseTime.CurrentMinute,
                BaseTime.CurrentSecond,
                BaseTime.Now,
                BaseTime.NowAlt
            }.Where(x => x != null).ToArray();

            var timeOffsets = new[] {
                TimeOffset.Years,
                TimeOffset.Months,
                TimeOffset.Weeks,
                TimeOffset.Days,
                TimeOffset.Hours,
                TimeOffset.Minutes,
                TimeOffset.Seconds,
                TimeOffset.Milliseconds
            }.ToArray();

            var fractionalTimeOffsets = new[] {
                TimeOffset.Days,
                TimeOffset.Hours,
                TimeOffset.Minutes,
                TimeOffset.Seconds,
                TimeOffset.Milliseconds
            }.ToArray();

            RelativeTimestampRegexPattern = string.Concat(
                @"^\s*(?<base>",
                string.Join(
                    "|",
                    baseTimes.Select(x => EscapeRegexSpecialCharacters(x))
                ),
                @")\s*(?:(?<operator>\+|-)\s*",
                // Units that can be expresses as whole numbers. Guaranteed to have length > 0
                @"(?:(?<count>[0-9]+)\s*(?<unit>",
                string.Join(
                    "|",
                    timeOffsets.Select(x => EscapeRegexSpecialCharacters(x))
                ),
                @"))",
                fractionalTimeOffsets.Length == 0
                    ? string.Empty
                    : string.Concat(
                        // Units that can be expressed as fractions.
                        @"|(?:(?<count>[0-9]+",
                        EscapeRegexSpecialCharacters(CultureInfo.NumberFormat?.NumberDecimalSeparator ?? CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator),
                        @"[0-9]+)\s*(?<unit>",
                        string.Join(
                            "|",
                            fractionalTimeOffsets.Select(x => EscapeRegexSpecialCharacters(x))
                        ),
                        @"))"
                    ),
                @")?\s*$"
            );
            _relativeDateTimeRegex = new Regex(RelativeTimestampRegexPattern, RegexOptions.IgnoreCase);
        }


        /// <summary>
        /// Loads cached parser instances for cultures specified in the <see cref="SettingsFileName"/> 
        /// embedded CSV file.
        /// </summary>
        private static void LoadDefaultParsers() {
            using (var reader = new System.IO.StreamReader(typeof(RelativityParser).Assembly.GetManifestResourceStream(typeof(RelativityParser), SettingsFileName))) {
                // Skip header.
                reader.ReadLine();

                while (!reader.EndOfStream) {
                    var row = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(row) || row.StartsWith("#", StringComparison.Ordinal)) {
                        continue;
                    }

                    var settings = row.Split(',');
                    // Row should be 17 columns long:
                    // - culture
                    // - base time: current time
                    // - base time: start of current second
                    // - base time: start of current minute
                    // - base time: start of current hour
                    // - base time: start of current day
                    // - base time: start of current week
                    // - base time: start of current month
                    // - base time: start of current year
                    // - time offset: milliseconds
                    // - time offset: seconds
                    // - time offset: minutes
                    // - time offset: hours
                    // - time offset: days
                    // - time offset: weeks
                    // - time offset: months
                    // - time offset: years
                    if (settings.Length != 17) {
                        continue;
                    }

                    var parser = new RelativityParser(
                        CultureInfo.GetCultureInfo(settings[0]),
                        new RelativityBaseTimeSettings(
                            now: settings[1],
                            currentSecond: settings[2],
                            currentMinute: settings[3],
                            currentHour: settings[4],
                            currentDay: settings[5],
                            currentWeek: settings[6],
                            currentMonth: settings[7],
                            currentYear: settings[8]
                        ),
                        new RelativityTimeOffsetSettings(
                            milliseconds: settings[9],
                            seconds: settings[10],
                            minutes: settings[11],
                            hours: settings[12],
                            days: settings[13],
                            weeks: settings[14],
                            months: settings[15],
                            years: settings[16]
                        )
                    );

                    s_parserInstances[parser.CultureInfo.Name] = parser;
                }
            }

            s_parserInstances[Default.CultureInfo.Name] = Default;
        }


        /// <summary>
        /// Gets the registered <see cref="RelativityParser"/> for the specified culture. If a 
        /// matching parser is not found, <see cref="Default"/> will be returned.
        /// </summary>
        /// <param name="cultureInfo">
        ///   The culture.
        /// </param>
        /// <returns>
        ///   The <see cref="RelativityParser"/> for the culture.
        /// </returns>
        public static RelativityParser GetParser(CultureInfo cultureInfo) {
            if (cultureInfo == null) {
                return Default;
            }

            while (true) { 
                if (cultureInfo == null) {
                    break;
                }

                if (s_parserInstances.TryGetValue(cultureInfo.Name, out var parser)) {
                    return parser;
                }

                cultureInfo = cultureInfo.Parent;
            }

            return Default;
        }


        /// <summary>
        /// Gets the registered <see cref="RelativityParser"/> for the specified culture. If a 
        /// matching parser is not found, <see cref="Default"/> will be returned.
        /// </summary>
        /// <param name="cultureName">
        ///   The culture name, in <c>languagecode2-country/regioncode2</c> format (e.g. <c>en-GB</c>).
        /// </param>
        /// <returns>
        ///   The <see cref="RelativityParser"/> for the culture.
        /// </returns>
        public static RelativityParser GetParser(string cultureName) {
            return GetParser(string.IsNullOrWhiteSpace(cultureName) ? null : CultureInfo.GetCultureInfo(cultureName));
        }


        /// <summary>
        /// Gets the registered <see cref="RelativityParser"/> for the specified culture. If a 
        /// matching parser is not found, <see cref="Default"/> will be returned.
        /// </summary>
        /// <param name="lcid">
        ///   A locale identifier (LCID).
        /// </param>
        /// <returns>
        ///   The <see cref="RelativityParser"/> for the culture.
        /// </returns>
        public static RelativityParser GetParser(int lcid) {
            return GetParser(CultureInfo.GetCultureInfo(lcid));
        }


        /// <summary>
        /// Gets the culture identifiers that have a registered <see cref="RelativityParser"/>.
        /// </summary>
        /// <returns>
        ///   The registered culture identifiers.
        /// </returns>
        public static IEnumerable<string> GetAvailableCultures() {
            return s_parserInstances.Keys.OrderBy(x => x).ToArray();
        }


        /// <summary>
        /// Registers a <see cref="RelativityParser"/> instance for a particular culture.
        /// </summary>
        /// <param name="parser">
        ///   The parser.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="parser"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   The <see cref="CultureInfo"/> for the <paramref name="parser"/> is <see cref="CultureInfo.InvariantCulture"/>.
        /// </exception>
        public static void RegisterParser(RelativityParser parser) {
            if (parser == null) {
                throw new ArgumentNullException(nameof(parser));
            }
            if (string.Equals(parser.CultureInfo.Name, CultureInfo.InvariantCulture.Name, StringComparison.OrdinalIgnoreCase)) {
                throw new ArgumentException(Resources.Error_CannotReplaceInvariantCultureParser, nameof(parser));
            }

            s_parserInstances[parser.CultureInfo.Name] = parser;
        }


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


        #region [ DateTime Parsing ]

        // <summary>
        /// Determines whether a string is a relative time stamp.
        /// </summary>
        /// <param name="s">
        ///   The string.
        /// </param>
        /// <param name="m">
        ///   A regular expression match for the string.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the string is a valid relative timestamp, otherwise 
        ///   <see langword="false"/>.
        /// </returns>
        private bool IsRelativeDateTime(string dateString, out Match match) {
            match = _relativeDateTimeRegex.Match(dateString);
            return match.Success;
        }


        /// <summary>
        /// Determines if a string is a relative timestamp.
        /// </summary>
        /// <param name="dateString">
        ///   The string.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the string is a valid relative timestamp, otherwise 
        ///   <see langword="false"/>.
        /// </returns>
        /// <remarks>
        ///   Relative timestamps are specified in the format <c>[base] - [quantity][unit]</c> or 
        ///   <c>[base] + [quantity][unit]</c> where <c>[base]</c> represents the base time to offset 
        ///   from, <c>[quantity]</c> is a whole number greater than or equal to zero and <c>[unit]</c> 
        ///   is the unit that the offset is measured in.
        /// </remarks>
        public bool IsRelativeDateTime(string dateString) {
            if (string.IsNullOrWhiteSpace(dateString)) {
                return false;
            }
            return IsRelativeDateTime(dateString, out var _);
        }


        /// <summary>
        /// Determines whether a string is a valid absolute timestamp.
        /// </summary>
        /// <param name="dateString">
        ///   The string.
        /// </param>
        /// <param name="formatProvider">
        ///   An <see cref="IFormatProvider"/> to use when parsing the string.
        /// </param>
        /// <param name="dateTimeStyle">
        ///   A <see cref="DateTimeStyles"/> instance specifying flags to use while parsing dates.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the string is a valid absolute timestamp, or 
        ///   <see langword="false"/> otherwise.
        /// </returns>
        public bool IsAbsoluteDateTime(string dateString, DateTimeStyles dateTimeStyle) {
            return DateTime.TryParse(dateString, CultureInfo, dateTimeStyle, out var _);
        }


        /// <summary>
        /// Attempts to convert a timestamp expressed as milliseconds since 01 January 1970 into a UTC 
        /// <see cref="DateTime"/> instance.
        /// </summary>
        /// <param name="dateString">
        ///   The string.
        /// </param>
        /// <param name="utcDateTime">
        ///   The UTC time stamp, if <paramref name="dateString"/> is a numeric value.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="dateString"/> is a numeric value, or <see langword="false"/>
        ///   otherwise.
        /// </returns>
        private bool TryParseNumericDateTime(string dateString, out DateTime utcDateTime) {
            if (double.TryParse(dateString, NumberStyles.Float, CultureInfo, out var milliseconds)) {
                utcDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(milliseconds);
                return true;
            }

            utcDateTime = default;
            return false;
        }


        /// <summary>
        /// Determines whether a string is a valid absolute or relative timestamp.
        /// </summary>
        /// <param name="dateString">
        ///   The string.
        /// </param>
        /// <param name="dateTimeStyle">
        ///   A <see cref="DateTimeStyles"/> instance specifying flags to use while parsing dates.
        /// </param>
        /// <param name="match">
        ///   A regular expression match for the string.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the string is a valid timestamp, or <see langword="false"/> 
        ///   otherwise.
        /// </returns>
        /// <remarks>
        ///   If the string can be successfully parsed as an absolute timestamp, <paramref name="match"/> 
        ///   will be <see langword="null"/>.
        /// </remarks>
        private bool IsDateTime(string dateString, DateTimeStyles dateTimeStyle, out Match match) {
            if (string.IsNullOrWhiteSpace(dateString)) {
                match = null;
                return false;
            }

            if (IsAbsoluteDateTime(dateString, dateTimeStyle)) {
                match = null;
                return true;
            }
            if (TryParseNumericDateTime(dateString, out var _)) {
                match = null;
                return true;
            }

            return IsRelativeDateTime(dateString, out match);
        }


        /// <summary>
        /// Determines whether a string is a valid absolute or relative timestamp.
        /// </summary>
        /// <param name="dateString">
        ///   The string.
        /// </param>
        /// <param name="dateTimeStyle">
        ///   A <see cref="DateTimeStyles"/> instance specifying flags to use while parsing dates.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the string is a valid timestamp, or <see langword="false"/>
        ///   otherwise.
        /// </returns>
        public bool IsDateTime(string dateString, DateTimeStyles dateTimeStyle) {
            return IsDateTime(dateString, dateTimeStyle, out var _);
        }


        /// <summary>
        /// Determines whether a string is a valid absolute or relative timestamp.
        /// </summary>
        /// <param name="dateString">
        ///   The string.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the string is a valid timestamp, or <see langword="false"/> 
        ///   otherwise.
        /// </returns>
        public bool IsDateTime(string dateString) {
            return IsDateTime(dateString, DateTimeStyles.None, out var _);
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
        /// <returns>
        ///   The adjusted <see cref="DateTime"/>.
        /// </returns>
        private DateTime ApplyOffset(DateTime baseDate, string unit, double quantity, bool add) {
            if (string.Equals(unit, TimeOffset.Years, StringComparison.OrdinalIgnoreCase)) {
                var wholeQuantity = Convert.ToInt32(quantity);
                return add
                    ? baseDate.AddYears(wholeQuantity)
                    : baseDate.AddYears(-1 * wholeQuantity);
            }
            if (string.Equals(unit, TimeOffset.Months, StringComparison.OrdinalIgnoreCase)) {
                var wholeQuantity = Convert.ToInt32(quantity);
                return add
                    ? baseDate.AddMonths(wholeQuantity)
                    : baseDate.AddMonths(-1 * wholeQuantity);
            }
            if (string.Equals(unit, TimeOffset.Weeks, StringComparison.OrdinalIgnoreCase)) {
                var wholeQuantity = Convert.ToInt32(quantity);
                return add
                    ? baseDate.AddDays(7 * wholeQuantity)
                    : baseDate.AddYears(-7 * wholeQuantity);
            }
            if (string.Equals(unit, TimeOffset.Days, StringComparison.OrdinalIgnoreCase)) {
                return add
                    ? baseDate.AddDays(quantity)
                    : baseDate.AddDays(-1 * quantity);
            }
            if (string.Equals(unit, TimeOffset.Hours, StringComparison.OrdinalIgnoreCase)) {
                return add
                    ? baseDate.AddHours(quantity)
                    : baseDate.AddHours(-1 * quantity);
            }
            if (string.Equals(unit, TimeOffset.Minutes, StringComparison.OrdinalIgnoreCase)) {
                return add
                    ? baseDate.AddMinutes(quantity)
                    : baseDate.AddMinutes(-1 * quantity);
            }
            if (string.Equals(unit, TimeOffset.Seconds, StringComparison.OrdinalIgnoreCase)) {
                return add
                    ? baseDate.AddSeconds(quantity)
                    : baseDate.AddSeconds(-1 * quantity);
            }
            if (string.Equals(unit, TimeOffset.Milliseconds, StringComparison.OrdinalIgnoreCase)) {
                return add
                    ? baseDate.AddMilliseconds(quantity)
                    : baseDate.AddMilliseconds(-1 * quantity);
            }

            return baseDate;
        }


        /// <summary>
        /// Converts a base time keyword into a <see cref="DateTime"/>.
        /// </summary>
        /// <param name="baseTime">
        ///   The base time keyword. See <see cref="BaseTime"/> for valid values.
        /// </param>
        /// <param name="relativeTo">
        ///   The <see cref="DateTime"/> that the <paramref name="baseTime"/> is relative to.
        /// </param>
        /// <returns>
        ///   A <see cref="DateTime"/> that represents the specified base time.
        /// </returns>
        public DateTime ToAbsoluteDateTime(string baseTime, DateTime relativeTo) {
            if (baseTime == null) {
                throw new ArgumentNullException(nameof(baseTime));
            }
            var startOfWeek = CultureInfo.DateTimeFormat.FirstDayOfWeek;

            if (string.Equals(baseTime, BaseTime.CurrentYear, StringComparison.OrdinalIgnoreCase)) {
                // Start of current year.
                return new DateTime(relativeTo.Year, 1, 1, 0, 0, 0, relativeTo.Kind);
            }
            if (string.Equals(baseTime, BaseTime.CurrentMonth, StringComparison.OrdinalIgnoreCase)) {
                // Start of current month.
                return new DateTime(relativeTo.Year, relativeTo.Month, 1, 0, 0, 0, relativeTo.Kind);
            }
            if (string.Equals(baseTime, BaseTime.CurrentWeek, StringComparison.OrdinalIgnoreCase)) {
                // Start of current week.
                var diff = (7 + (relativeTo.DayOfWeek - startOfWeek)) % 7;
                var startOfWeekDate = relativeTo.AddDays(-1 * diff).Date;
                return new DateTime(startOfWeekDate.Year, startOfWeekDate.Month, startOfWeekDate.Day, 0, 0, 0, relativeTo.Kind);
            }
            if (string.Equals(baseTime, BaseTime.CurrentDay, StringComparison.OrdinalIgnoreCase)) {
                // Start of current day.
                return new DateTime(relativeTo.Date.Year, relativeTo.Date.Month, relativeTo.Date.Day, 0, 0, 0, relativeTo.Kind);
            }
            if (string.Equals(baseTime, BaseTime.CurrentHour, StringComparison.OrdinalIgnoreCase)) {
                // Start of current hour.
                return new DateTime(relativeTo.Year, relativeTo.Month, relativeTo.Day, relativeTo.Hour, 0, 0, relativeTo.Kind);
            }
            if (string.Equals(baseTime, BaseTime.CurrentMinute, StringComparison.OrdinalIgnoreCase)) {
                // Start of current minute.
                return new DateTime(relativeTo.Year, relativeTo.Month, relativeTo.Day, relativeTo.Hour, relativeTo.Minute, 0, relativeTo.Kind);
            }
            if (string.Equals(baseTime, BaseTime.CurrentSecond, StringComparison.OrdinalIgnoreCase)) {
                // Start of current second.
                return new DateTime(relativeTo.Year, relativeTo.Month, relativeTo.Day, relativeTo.Hour, relativeTo.Minute, relativeTo.Second, relativeTo.Kind);
            }
            if (string.Equals(baseTime, BaseTime.Now, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(baseTime, BaseTime.NowAlt, StringComparison.OrdinalIgnoreCase)) {
                // Current time.
                return relativeTo;
            }

            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.Error_InvalidBaseDate, baseTime), nameof(baseTime));
        }


        /// <summary>
        /// Converts an absolute or relative timestamp string into a UTC <see cref="DateTime"/> instance.
        /// </summary>
        /// <param name="dateString">
        ///   The date string.
        /// </param>
        /// <param name="timeZone">
        ///   The time zone that relative dates are assumed to be in. If <see langword="null"/>, 
        ///   <see cref="TimeZoneInfo.Local"/> will be used.
        /// </param>
        /// <param name="baseDateTime">
        ///   The base <see cref="DateTime"/> to use as the current time in a relative timestamp 
        ///   conversion. The <paramref name="baseDateTime"/> must be specified in the 
        ///   <paramref name="timeZone"/> passed to the method. Specify <see langword="null"/> to 
        ///   use the current time in the provided <paramref name="timeZone"/> as the base time.
        /// </param>
        /// <returns>
        ///   A UTC <see cref="DateTime"/> representing the timestamp string.
        /// </returns>
        /// <exception cref="FormatException">
        ///   The string is not a valid absolute or relative time stamp.
        /// </exception>
        /// <remarks>
        ///   Relative time stamps are specified in the format <c>[base] - [quantity][unit]</c> or 
        ///   <c>[base] + [quantity][unit]</c> where <c>[base]</c> represents the base time to offset 
        ///   from, <c>[quantity]</c> is a whole number greater than or equal to zero and <c>[unit]</c> 
        ///   is the unit that the offset is measured in.
        /// </remarks>
        public DateTime ToUtcDateTime(string dateString, TimeZoneInfo timeZone = null, DateTime? baseDateTime = null) {
            if (!TryConvertToUtcDateTime(dateString, out var dt, timeZone, baseDateTime)) {
                throw new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Error_InvalidTimeStamp, dateString));
            }

            return dt;
        }


        /// <summary>
        /// Attempts to parse the specified absolute or relative timestamp into a UTC <see cref="DateTime"/> 
        /// instance using the specified settings.
        /// </summary>
        /// <param name="dateString">
        ///   The time stamp.
        /// </param>
        /// <param name="dateTime">
        ///   The parsed date.
        ///  </param>
        /// <param name="timeZone">
        ///   The time zone that relative dates are assumed to be in. If <see langword="null"/>, 
        ///   <see cref="TimeZoneInfo.Local"/> will be used.
        /// </param>
        /// <param name="baseDateTime">
        ///   The base <see cref="DateTime"/> to use as the current time in a relative timestamp 
        ///   conversion. The <paramref name="baseDateTime"/> must be specified in the 
        ///   <paramref name="timeZone"/> passed to the method. Specify <see langword="null"/> to 
        ///   use the current time in the provided <paramref name="timeZone"/> as the base time.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the literal was successfully parsed, or <see langword="false"/>
        ///   otherwise.
        /// </returns>
        /// <remarks>
        ///   Relative timestamps are specified in the format <c>[base] - [quantity][unit]</c> or 
        ///   <c>[base] + [quantity][unit]</c> where <c>[base]</c> represents the base time to offset 
        ///   from, <c>[quantity]</c> is a whole number greater than or equal to zero and <c>[unit]</c> 
        ///   is the unit that the offset is measured in.
        /// </remarks>
        public bool TryConvertToUtcDateTime(string dateString, out DateTime dateTime, TimeZoneInfo timeZone = null, DateTime? baseDateTime = null) {
            if (string.IsNullOrWhiteSpace(dateString)) {
                dateTime = default;
                return false;
            }

            var dateTimeStyle = DateTimeStyles.None;

            if (!IsDateTime(dateString, dateTimeStyle, out var m)) {
                dateTime = default;
                return false;
            }

            if (TryParseNumericDateTime(dateString, out var dt)) {
                dateTime = timeZone.ConvertToUtc(dt);
                return true;
            }

            if (m == null) {
                dt = DateTime.Parse(dateString, CultureInfo, dateTimeStyle);
                dateTime = timeZone.ConvertToUtc(dt);
                return true;
            }


            if (timeZone == null) {
                timeZone = TimeZoneInfo.Local;
            }
            var now = baseDateTime ?? timeZone.GetCurrentTime();
            var baseDate = ToAbsoluteDateTime(m.Groups["base"].Value, now);

            DateTime adjustedDate;

            if (!m.Groups["operator"].Success) {
                adjustedDate = baseDate;
            }
            else {
                GetTimeSpanUnitAndCount(m, out var unit, out var quantity);
                if (string.IsNullOrWhiteSpace(unit) || double.IsNaN(quantity)) {
                    adjustedDate = baseDate;
                }
                else {
                    adjustedDate = ApplyOffset(baseDate, unit, quantity, !string.Equals(m.Groups["operator"].Value, "-", StringComparison.Ordinal));
                }
            }

            dateTime = timeZone.ConvertToUtc(adjustedDate);
            return true;
        }

        #endregion

        #region [ TimeSpan Parsing ]

        /// <summary>
        /// Determines whether a string is a valid long-hand or short-hand time span literal.
        /// </summary>
        /// <param name="timeSpanString">
        ///   The string.
        /// </param>
        /// <param name="match">
        ///   A regular expression match for the string.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the string is a valid time span, or <see langword="false"/>
        ///   othewise.
        /// </returns>
        /// <remarks>
        ///   If the string can be successfully parsed as a long-hand literal time span (e.g. "01:23:55"), 
        ///   <paramref name="match"/> will be <see langword="null"/>.
        /// </remarks>
        private bool IsTimeSpan(string timeSpanString, out Match match) {
            if (string.IsNullOrWhiteSpace(timeSpanString)) {
                match = null;
                return false;
            }

            if (TimeSpan.TryParse(timeSpanString, CultureInfo, out var _)) {
                match = null;
                return true;
            }

            match = _timeSpanRegex.Match(timeSpanString);
            return match.Success;
        }


        /// <summary>
        /// Determines whether a string is a valid time span.
        /// </summary>
        /// <param name="timeSpanString">
        ///   The string.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the string is a valid time span, or <see langword="false"/>
        ///   otherwise.
        /// </returns>
        public bool IsTimeSpan(string timeSpanString) {
            return IsTimeSpan(timeSpanString, out var _);
        }


        /// <summary>
        /// Gets the time unit and quantity from the provided regular expression match created 
        /// from <see cref="_timeSpanRegex"/>.
        /// </summary>
        /// <param name="timeSpanMatch">
        ///   The time span regex pattern match.
        /// </param>
        /// <param name="unit">
        ///   The time unit.
        /// </param>
        /// <param name="quantity">
        ///   The time unit quantity.
        /// </param>
        private void GetTimeSpanUnitAndCount(Match timeSpanMatch, out string unit, out int quantity) {
            unit = timeSpanMatch.Groups["unit"].Value;
            quantity = Convert.ToInt32(timeSpanMatch.Groups["count"].Value, CultureInfo);
        }


        /// <summary>
        /// Converts a match for a timespan regex into a <see cref="TimeSpan"/> instance.
        /// </summary>
        /// <param name="match">
        ///   The regex match.
        /// </param>
        /// <returns>
        ///   The matching time span.
        /// </returns>
        private TimeSpan ToTimeSpan(Match match) {
            GetTimeSpanUnitAndCount(match, out var unit, out var count);

            if (string.Equals(unit, TimeOffset.Weeks, StringComparison.OrdinalIgnoreCase)) {
                return TimeSpan.FromTicks(TimeSpan.TicksPerDay * 7 * count);
            }
            if (string.Equals(unit, TimeOffset.Days, StringComparison.OrdinalIgnoreCase)) {
                return TimeSpan.FromTicks(TimeSpan.TicksPerDay * count);
            }
            if (string.Equals(unit, TimeOffset.Hours, StringComparison.OrdinalIgnoreCase)) {
                return TimeSpan.FromTicks(TimeSpan.TicksPerHour * count);
            }
            if (string.Equals(unit, TimeOffset.Minutes, StringComparison.OrdinalIgnoreCase)) {
                return TimeSpan.FromTicks(TimeSpan.TicksPerMinute * count);
            }
            if (string.Equals(unit, TimeOffset.Seconds, StringComparison.OrdinalIgnoreCase)) {
                return TimeSpan.FromTicks(TimeSpan.TicksPerSecond* count);
            }
            if (string.Equals(unit, TimeOffset.Milliseconds, StringComparison.OrdinalIgnoreCase)) {
                return TimeSpan.FromTicks(TimeSpan.TicksPerMillisecond * count);
            }

            return TimeSpan.Zero;
        }


        /// <summary>
        /// Converts a duration literal into a <see cref="TimeSpan"/> instance.
        /// </summary>
        /// <param name="timeSpanString">
        ///   The string.
        /// </param>
        /// <returns>
        ///   A <see cref="TimeSpan"/>.
        /// </returns>
        /// <exception cref="FormatException">
        ///   The string is not a valid timespan.
        /// </exception>
        /// <remarks>
        ///   Initially, the method will attempt to parse the string using the 
        ///   <see cref="TimeSpan.TryParse(string, IFormatProvider, out TimeSpan)"/> method. This 
        ///   ensures that standard time span literals (e.g. <c>"365.00:00:00"</c>) are parsed in 
        ///   the standard way. If the string cannot be parsed in this way, it is tested to see if 
        ///   it is in the format <c>[duration][unit]</c>, where <c>[duration]</c> is a whole 
        ///   number greater than or equal to zero and <c>[unit]</c> is the unit that the duration 
        ///   is measured in.
        /// </remarks>
        public TimeSpan ToTimeSpan(string timeSpanString) {
            if (!TryConvertToTimeSpan(timeSpanString, out var ts)) {
                throw new FormatException(string.Format(CultureInfo.CurrentCulture, Resources.Error_InvalidTimeSpan, timeSpanString));
            }

            return ts;
        }


        /// <summary>
        /// Attempts to parse the specified long-hand or short-hand time span literal into a 
        /// <see cref="TimeSpan"/> instance.
        /// </summary>
        /// <param name="timeSpanString">
        ///   The string.
        /// </param>
        /// <param name="timeSpan">
        ///   The parsed time span.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the literal was successfully parsed, or <see langword="false"/>
        ///   otherwise.
        /// </returns>
        /// <remarks>
        ///   Initially, the method will attempt to parse the string using the 
        ///   <see cref="TimeSpan.TryParse(string, IFormatProvider, out TimeSpan)"/> method. This 
        ///   ensures that standard time span literals (e.g. <c>"365.00:00:00"</c>) are parsed in 
        ///   the standard way. If the string cannot be parsed in this way, it is tested to see if 
        ///   it is in the format <c>[duration][unit]</c>, where <c>[duration]</c> is a whole 
        ///   number greater than or equal to zero and <c>[unit]</c> is the unit that the duration 
        ///   is measured in.  
        /// </remarks>
        public bool TryConvertToTimeSpan(string timeSpanString, out TimeSpan timeSpan) {
            if (string.IsNullOrWhiteSpace(timeSpanString)) {
                timeSpan = default;
                return false;
            }

            if (!IsTimeSpan(timeSpanString, out var m)) {
                timeSpan = default;
                return false;
            }

            timeSpan = m == null
                ? TimeSpan.Parse(timeSpanString, CultureInfo)
                : ToTimeSpan(m);
            return true;
        }

        #endregion

    }
}
