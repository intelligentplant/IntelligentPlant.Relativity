using System;
using System.Globalization;
using System.Runtime;
using System.Text;

using IntelligentPlant.Relativity.Internal;

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
        /// Converts the specified <see cref="TimeSpan"/> to a Relativity duration string.
        /// </summary>
        /// <param name="parser">
        ///   The parser.
        /// </param>
        /// <param name="timeSpan">
        ///   The time span.
        /// </param>
        /// <returns>
        ///   The duration string.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="parser"/> is <see langword="null"/>.
        /// </exception>
        public static string ConvertToDuration(this IRelativityParser parser, TimeSpan timeSpan) {
            if (parser == null) {
                throw new ArgumentNullException(nameof(parser));
            }

            var builder = new StringBuilder();

            string? mostPreciseUnit = null;
            foreach (var availableUnit in new[] { parser.TimeOffsetSettings.Milliseconds, parser.TimeOffsetSettings.Seconds, parser.TimeOffsetSettings.Minutes, parser.TimeOffsetSettings.Hours, parser.TimeOffsetSettings.Days, parser.TimeOffsetSettings.Weeks }) {
                if (availableUnit == null) {
                    continue;
                }
                mostPreciseUnit = availableUnit;
                break;
            }

            if (parser.TimeOffsetSettings.Weeks != null && Math.Abs(timeSpan.TotalDays) >= 7) { 
                var weeks = mostPreciseUnit == parser.TimeOffsetSettings.Weeks
                    ? timeSpan.TotalDays / 7
                    : Math.Truncate(timeSpan.TotalDays / 7);
                builder.AppendFormat(parser.CultureInfo, "{0}{1}", weeks, parser.TimeOffsetSettings.Weeks);
                timeSpan = timeSpan.Subtract(TimeSpan.FromDays(weeks * 7));
            }

            if (parser.TimeOffsetSettings.Days != null && Math.Abs(timeSpan.TotalDays) >= 1) {
                var days = mostPreciseUnit == parser.TimeOffsetSettings.Days 
                    ? timeSpan.TotalDays 
                    : Math.Truncate(timeSpan.TotalDays);
                if (builder.Length > 0) {
                    builder.Append(" ");
                }
                builder.AppendFormat(parser.CultureInfo, "{0}{1}", days, parser.TimeOffsetSettings.Days);
                timeSpan = timeSpan.Subtract(TimeSpan.FromDays(days));
            }

            if (parser.TimeOffsetSettings.Hours != null && Math.Abs(timeSpan.TotalHours) >= 1) {
                var hours = mostPreciseUnit == parser.TimeOffsetSettings.Hours 
                    ? timeSpan.TotalHours 
                    : Math.Truncate(timeSpan.TotalHours);
                if (builder.Length > 0) {
                    builder.Append(" ");
                }
                builder.AppendFormat(parser.CultureInfo, "{0}{1}", hours, parser.TimeOffsetSettings.Hours);
                timeSpan = timeSpan.Subtract(TimeSpan.FromHours(hours));
            }

            if (parser.TimeOffsetSettings.Minutes != null && Math.Abs(timeSpan.TotalMinutes) >= 1) {
                var minutes = mostPreciseUnit == parser.TimeOffsetSettings.Minutes 
                    ? timeSpan.TotalMinutes 
                    : Math.Truncate(timeSpan.TotalMinutes);
                if (builder.Length > 0) {
                    builder.Append(" ");
                }
                builder.AppendFormat(parser.CultureInfo, "{0}{1}", minutes, parser.TimeOffsetSettings.Minutes);
                timeSpan = timeSpan.Subtract(TimeSpan.FromMinutes(minutes));
            }

            if (parser.TimeOffsetSettings.Seconds != null && Math.Abs(timeSpan.TotalSeconds) >= 1) {
                var seconds = mostPreciseUnit == parser.TimeOffsetSettings.Seconds
                    ? timeSpan.TotalSeconds
                    : Math.Truncate(timeSpan.TotalSeconds);
                if (builder.Length > 0) {
                    builder.Append(" ");
                }
                builder.AppendFormat(parser.CultureInfo, "{0}{1}", seconds, parser.TimeOffsetSettings.Seconds);
                timeSpan = timeSpan.Subtract(TimeSpan.FromSeconds(seconds));
            }

            if (parser.TimeOffsetSettings.Milliseconds != null && Math.Abs(timeSpan.TotalMilliseconds) > 0) {
                var milliseconds = timeSpan.TotalMilliseconds;
                if (builder.Length > 0) {
                    builder.Append(" ");
                }
                builder.AppendFormat(parser.CultureInfo, "{0}{1}", milliseconds, parser.TimeOffsetSettings.Milliseconds);
            }

            return builder.ToString();
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
        /// <param name="unit">
        ///   The time unit to use in the duration string. Specify <see cref="DurationUnitKind.Unspecified"/> 
        ///   to return a duration string that uses composite units (e.g. <c>3H 15M</c>).
        /// </param>
        /// <param name="decimalPlaces">
        ///   The number of decimal places to use when formatting the duration string. If greater 
        ///   than or equal to zero, the duration string will be rounded away from zero to the 
        ///   specified number of decimal places e.g. specifying one decimal place when a time 
        ///   span of <c>00:00:00.1234567</c> is specified will result in a duration string of 
        ///   <c>123.5MS</c> being returned (or its equivalent localized value). This parameter is 
        ///   ignored if <paramref name="unit"/> is <see cref="DurationUnitKind.Unspecified"/>.
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
        ///   You can specify the number of decimal places to round the duration to using the 
        ///   <paramref name="decimalPlaces"/> parameter. When a value of zero or greater is 
        ///   specified, the duration string will be rounded away from zero to the specified 
        ///   number of decimal places. For example, specifying one decimal place when a time 
        ///   span of <c>00:00:00.1234567</c> is specified will result in a duration string of 
        ///   <c>123.5MS</c> being returned (or its equivalent localized value).
        /// </para>
        /// 
        /// <para>
        ///   Note that the <paramref name="decimalPlaces"/> parameter is ignored if 
        ///   <paramref name="unit"/> is <see cref="DurationUnitKind.Unspecified"/>.
        /// </para>
        /// 
        /// </remarks>
        public static string ConvertToDuration(this IRelativityParser parser, TimeSpan timeSpan, DurationUnitKind unit, int decimalPlaces = -1) {
            if (parser == null) {
                throw new ArgumentNullException(nameof(parser));
            }

            if (unit == DurationUnitKind.Unspecified) {
                return ConvertToDuration(parser, timeSpan);
            }

            if (unit == DurationUnitKind.Weeks) {
                return Format(timeSpan.TotalDays / 7, parser.TimeOffsetSettings.Weeks!);
            }
            else if (unit == DurationUnitKind.Days) {
                return Format(timeSpan.TotalDays, parser.TimeOffsetSettings.Days!);
            }
            else if (unit == DurationUnitKind.Hours) {
                return Format(timeSpan.TotalHours, parser.TimeOffsetSettings.Hours!);
            }
            else if (unit == DurationUnitKind.Minutes) {
                return Format(timeSpan.TotalMinutes, parser.TimeOffsetSettings.Minutes!);
            }
            else if (unit == DurationUnitKind.Seconds) {
                return Format(timeSpan.TotalSeconds, parser.TimeOffsetSettings.Seconds!);
            }
            else if (unit == DurationUnitKind.Milliseconds) {
                return Format(timeSpan.TotalMilliseconds, parser.TimeOffsetSettings.Milliseconds!);
            }

            throw new InvalidOperationException("The specified time unit is not supported.");

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


        /// <summary>
        /// Gets the symbol for the specified relative time origin kind.
        /// </summary>
        /// <param name="settings">
        ///   The base time settings.
        /// </param>
        /// <param name="origin">
        ///   The relative time origin kind.
        /// </param>
        /// <returns>
        ///   The symbol for the specified relative time origin kind, or <see langword="null"/> if 
        ///   no symbol is available.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="settings"/> is <see langword="null"/>.
        /// </exception>
        public static string? GetSymbol(this RelativityBaseTimeSettings settings, RelativeTimeOriginKind origin) {
            if (settings == null) {
                throw new ArgumentNullException(nameof(settings));
            }

            return origin switch {
                RelativeTimeOriginKind.Now => settings.Now ?? settings.NowAlt,
                RelativeTimeOriginKind.CurrentSecond => settings.CurrentSecond,
                RelativeTimeOriginKind.CurrentMinute => settings.CurrentMinute,
                RelativeTimeOriginKind.CurrentHour => settings.CurrentHour,
                RelativeTimeOriginKind.CurrentDay => settings.CurrentDay,
                RelativeTimeOriginKind.CurrentWeek => settings.CurrentWeek,
                RelativeTimeOriginKind.CurrentMonth => settings.CurrentMonth,
                RelativeTimeOriginKind.CurrentYear => settings.CurrentYear,
                _ => null
            };
        }


        /// <summary>
        /// Gets the symbol for the specified relative time origin type.
        /// </summary>
        /// <param name="parser">
        ///   The parser
        /// </param>
        /// <param name="origin">
        ///   The relative time origin type.
        /// </param>
        /// <returns>
        ///   The symbol for the specified relative time origin kind, or <see langword="null"/> if 
        ///   no symbol is available.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="parser"/> is <see langword="null"/>.
        /// </exception>
        public static string? GetSymbol(this IRelativityParser parser, RelativeTimeOriginKind origin) => parser?.BaseTimeSettings.GetSymbol(origin);


        /// <summary>
        /// Gets the symbol for the specified duration unit type.
        /// </summary>
        /// <param name="settings">
        ///   The base time settings.
        /// </param>
        /// <param name="unit">
        ///   The duration unit type.
        /// </param>
        /// <returns>
        ///   The symbol for the specified duration unit type, or <see langword="null"/> if 
        ///   no symbol is available.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="settings"/> is <see langword="null"/>.
        /// </exception>
        public static string? GetSymbol(this RelativityTimeOffsetSettings settings, DurationUnitKind unit) {
            if (settings == null) {
                throw new ArgumentNullException(nameof(settings));
            }

            return unit switch {
                DurationUnitKind.Milliseconds => settings.Milliseconds,
                DurationUnitKind.Seconds => settings.Seconds,
                DurationUnitKind.Minutes => settings.Minutes,
                DurationUnitKind.Hours => settings.Hours,
                DurationUnitKind.Days => settings.Days,
                DurationUnitKind.Weeks => settings.Weeks,
                _ => null
            };
        }


        /// <summary>
        /// Gets the symbol for the specified duration unit type.
        /// </summary>
        /// <param name="parser">
        ///   The parser.
        /// </param>
        /// <param name="unit">
        ///   The duration unit type.
        /// </param>
        /// <returns>
        ///   The symbol for the specified duration unit type, or <see langword="null"/> if 
        ///   no symbol is available.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="parser"/> is <see langword="null"/>.
        /// </exception>
        public static string? GetSymbol(this IRelativityParser parser, DurationUnitKind unit) => parser?.TimeOffsetSettings.GetSymbol(unit);


        /// <summary>
        /// Gets the symbol for the specified relative time offset unit type.
        /// </summary>
        /// <param name="settings">
        ///   The base time settings.
        /// </param>
        /// <param name="unit">
        ///   The relative time offset unit type.
        /// </param>
        /// <returns>
        ///   The symbol for the specified relative time offset unit type, or <see langword="null"/> 
        ///   if no symbol is available.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="settings"/> is <see langword="null"/>.
        /// </exception>
        public static string? GetSymbol(this RelativityTimeOffsetSettings settings, RelativeTimeOffsetUnitKind unit) {
            if (settings == null) {
                throw new ArgumentNullException(nameof(settings));
            }

            return unit switch {
                RelativeTimeOffsetUnitKind.Milliseconds => settings.Milliseconds,
                RelativeTimeOffsetUnitKind.Seconds => settings.Seconds,
                RelativeTimeOffsetUnitKind.Minutes => settings.Minutes,
                RelativeTimeOffsetUnitKind.Hours => settings.Hours,
                RelativeTimeOffsetUnitKind.Days => settings.Days,
                RelativeTimeOffsetUnitKind.Weeks => settings.Weeks,
                RelativeTimeOffsetUnitKind.Months => settings.Months,
                RelativeTimeOffsetUnitKind.Years => settings.Years,
                _ => null
            };
        }


        /// <summary>
        /// Gets the symbol for the specified relative time offset unit type.
        /// </summary>
        /// <param name="parser">
        ///   The base time settings.
        /// </param>
        /// <param name="unit">
        ///   The relative time offset unit type.
        /// </param>
        /// <returns>
        ///   The symbol for the specified relative time offset unit type, or <see langword="null"/> 
        ///   if no symbol is available.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="parser"/> is <see langword="null"/>.
        /// </exception>
        public static string? GetSymbol(this IRelativityParser parser, RelativeTimeOffsetUnitKind unit) => parser?.TimeOffsetSettings.GetSymbol(unit);


        internal static string CreateEmptyDurationString(this IRelativityParser parser) {
            var units = 
                parser.TimeOffsetSettings.GetSymbol(DurationUnitKind.Milliseconds) ??
                parser.TimeOffsetSettings.GetSymbol(DurationUnitKind.Seconds) ??
                parser.TimeOffsetSettings.GetSymbol(DurationUnitKind.Minutes) ??
                parser.TimeOffsetSettings.GetSymbol(DurationUnitKind.Hours) ??
                parser.TimeOffsetSettings.GetSymbol(DurationUnitKind.Days) ??
                parser.TimeOffsetSettings.GetSymbol(DurationUnitKind.Weeks) ??
                throw new InvalidOperationException("No duration components are enabled.");

            return string.Format(parser.CultureInfo, "{0}{1}", 0, units);
        }

    }
}
