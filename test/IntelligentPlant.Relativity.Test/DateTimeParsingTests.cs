using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntelligentPlant.Relativity.Test {

    [TestClass]
    public class DateTimeParsingTests {

        private string GetDuration(RelativityParser parser, double quantity, Expression<Func<RelativityTimeOffsetSettings, string>> units) {
            var unitExpr = units.Compile().Invoke(parser.TimeOffset);
            return $"{quantity.ToString(parser.CultureInfo)}{unitExpr}";
        }


        private string GetRelativeExpression(RelativityParser parser, string baseTime, double quantity, Expression<Func<RelativityTimeOffsetSettings, string>> units) {
            var duration = GetDuration(parser, quantity, units);
            return quantity < 0
                ? baseTime + duration
                : baseTime + "+" + duration;
        }


        private DateTime GetBaseTime(DateTime now, string baseTimeType, DayOfWeek firstDayOfWeek) {
            switch (baseTimeType) {
                case nameof(RelativityBaseTimeSettings.Now):
                case nameof(RelativityBaseTimeSettings.NowAlt):
                    return now;
                case nameof(RelativityBaseTimeSettings.CurrentSecond):
                    return new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, 0, now.Kind);
                case nameof(RelativityBaseTimeSettings.CurrentMinute):
                    return new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, 0, now.Kind);
                case nameof(RelativityBaseTimeSettings.CurrentHour):
                    return new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, 0, now.Kind);
                case nameof(RelativityBaseTimeSettings.CurrentDay):
                    return new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, 0, now.Kind);
                case nameof(RelativityBaseTimeSettings.CurrentWeek):
                    var diff = (7 + (now.DayOfWeek - firstDayOfWeek)) % 7;
                    var startOfWeekDate = now.AddDays(-1 * diff).Date;
                    return new DateTime(startOfWeekDate.Year, startOfWeekDate.Month, startOfWeekDate.Day, 0, 0, 0, now.Kind);
                case nameof(RelativityBaseTimeSettings.CurrentMonth):
                    return new DateTime(now.Year, now.Month, 1, 0, 0, 0, 0, now.Kind);
                case nameof(RelativityBaseTimeSettings.CurrentYear):
                    return new DateTime(now.Year, 1, 1, 0, 0, 0, 0, now.Kind);
                default:
                    throw new ArgumentException("Invalid keyword name.", nameof(baseTimeType));
            }
        }


        private IDictionary<string, Action<DateTime, DateTime>> GetOffsetTests(RelativityParser parser, string baseTimeType) {
            var baseTimeKeyword = (string) typeof(RelativityBaseTimeSettings).GetProperty(baseTimeType).GetValue(parser.BaseTime);
            
            return new Dictionary<string, Action<DateTime, DateTime>>() {
                {
                    GetRelativeExpression(parser, baseTimeKeyword, -800, p => p.Milliseconds),
                    (now, parsed) => {
                        var baseTime = GetBaseTime(now, baseTimeType, parser.CultureInfo.DateTimeFormat.FirstDayOfWeek);
                        Assert.AreEqual(800, (baseTime - parsed).TotalMilliseconds);
                    }
                },
                {
                    GetRelativeExpression(parser, baseTimeKeyword, 567, p => p.Milliseconds),
                    (now, parsed) => {
                        var baseTime = GetBaseTime(now, baseTimeType, parser.CultureInfo.DateTimeFormat.FirstDayOfWeek);
                        Assert.AreEqual(567, (parsed - baseTime).TotalMilliseconds);
                    }
                },
                {
                    GetRelativeExpression(parser, baseTimeKeyword, -5, p => p.Seconds),
                    (now, parsed) => {
                        var baseTime = GetBaseTime(now, baseTimeType, parser.CultureInfo.DateTimeFormat.FirstDayOfWeek);
                        Assert.AreEqual(5, (baseTime - parsed).TotalSeconds);
                    }
                },
                {
                    GetRelativeExpression(parser, baseTimeKeyword, 10, p => p.Seconds),
                    (now, parsed) => {
                        var baseTime = GetBaseTime(now, baseTimeType, parser.CultureInfo.DateTimeFormat.FirstDayOfWeek);
                        Assert.AreEqual(10, (parsed - baseTime).TotalSeconds);
                    }
                },
                {
                    GetRelativeExpression(parser, baseTimeKeyword, 10.424, p => p.Minutes),
                    (now, parsed) => {
                        var baseTime = GetBaseTime(now, baseTimeType, parser.CultureInfo.DateTimeFormat.FirstDayOfWeek);
                        Assert.AreEqual(10.424, (parsed - baseTime).TotalMinutes, 0.001);
                    }
                },
                {
                    GetRelativeExpression(parser, baseTimeKeyword, -123, p => p.Minutes),
                    (now, parsed) => {
                        var baseTime = GetBaseTime(now, baseTimeType, parser.CultureInfo.DateTimeFormat.FirstDayOfWeek);
                        Assert.AreEqual(123, (baseTime - parsed).TotalMinutes);
                    }
                },
                {
                    GetRelativeExpression(parser, baseTimeKeyword, 887.134662, p => p.Hours),
                    (now, parsed) => {
                        var baseTime = GetBaseTime(now, baseTimeType, parser.CultureInfo.DateTimeFormat.FirstDayOfWeek);
                        Assert.AreEqual(887.134662, (parsed - baseTime).TotalHours, 0.00001);
                    }
                },
                {
                    GetRelativeExpression(parser, baseTimeKeyword, -12345.6789, p => p.Hours),
                    (now, parsed) => {
                        var baseTime = GetBaseTime(now, baseTimeType, parser.CultureInfo.DateTimeFormat.FirstDayOfWeek);
                        Assert.AreEqual(12345.6789, (baseTime - parsed).TotalHours, 0.0001);
                    }
                },
                {
                    GetRelativeExpression(parser, baseTimeKeyword, 3.5, p => p.Days),
                    (now, parsed) => {
                        var baseTime = GetBaseTime(now, baseTimeType, parser.CultureInfo.DateTimeFormat.FirstDayOfWeek);
                        Assert.AreEqual(3.5, (parsed - baseTime).TotalDays);
                    }
                },
                {
                    GetRelativeExpression(parser, baseTimeKeyword, -1.11665, p => p.Days),
                    (now, parsed) => {
                        var baseTime = GetBaseTime(now, baseTimeType, parser.CultureInfo.DateTimeFormat.FirstDayOfWeek);
                        Assert.AreEqual(1.11665, (baseTime - parsed).TotalDays);
                    }
                },
                {
                    GetRelativeExpression(parser, baseTimeKeyword, 10, p => p.Weeks),
                    (now, parsed) => {
                        var baseTime = GetBaseTime(now, baseTimeType, parser.CultureInfo.DateTimeFormat.FirstDayOfWeek);
                        Assert.AreEqual(10 * 7, (parsed - baseTime).TotalDays);
                    }
                },
                {
                    GetRelativeExpression(parser, baseTimeKeyword, -70, p => p.Weeks),
                    (now, parsed) => {
                        var baseTime = GetBaseTime(now, baseTimeType, parser.CultureInfo.DateTimeFormat.FirstDayOfWeek);
                        Assert.AreEqual(70 * 7, (baseTime - parsed).TotalDays);
                    }
                },
                {
                    GetRelativeExpression(parser, baseTimeKeyword, 9, p => p.Months),
                    (now, parsed) => {
                        var baseTime = GetBaseTime(now, baseTimeType, parser.CultureInfo.DateTimeFormat.FirstDayOfWeek);
                        Assert.IsTrue(baseTime.AddMonths(9) == parsed);
                    }
                },
                {
                    GetRelativeExpression(parser, baseTimeKeyword, -97, p => p.Months),
                    (now, parsed) => {
                        var baseTime = GetBaseTime(now, baseTimeType, parser.CultureInfo.DateTimeFormat.FirstDayOfWeek);
                        Assert.IsTrue(baseTime.AddMonths(-97) == parsed);
                    }
                },
                {
                    GetRelativeExpression(parser, baseTimeKeyword, 14, p => p.Years),
                    (now, parsed) => {
                        var baseTime = GetBaseTime(now, baseTimeType, parser.CultureInfo.DateTimeFormat.FirstDayOfWeek);
                        Assert.IsTrue(baseTime.AddYears(14) == parsed);
                    }
                },
                {
                    GetRelativeExpression(parser, baseTimeKeyword, -12, p => p.Years),
                    (now, parsed) => {
                        var baseTime = GetBaseTime(now, baseTimeType, parser.CultureInfo.DateTimeFormat.FirstDayOfWeek);
                        Assert.IsTrue(baseTime.AddYears(-12) == parsed);
                    }
                }
            };
        }


        private void DateTimeParseTest(RelativityParser parser, string dateString, TimeZoneInfo timeZone, Action<DateTime, DateTime> validator, bool convertBackToInputTimeZone) {
            var now = timeZone.GetCurrentTime();

            if (!parser.TryConvertToUtcDateTime(dateString, out var dt, timeZone, now)) {
                Assert.Fail("Not a valid DateTime: {0}", dateString);
            }

            var tzDisplayName = convertBackToInputTimeZone
                ? timeZone?.DisplayName ?? TimeZoneInfo.Local.DisplayName
                : TimeZoneInfo.Utc.DisplayName;

            if (convertBackToInputTimeZone) {
                dt = TimeZoneInfo.ConvertTimeFromUtc(dt, timeZone ?? TimeZoneInfo.Local);
            }

            var conversionSummary = $"{dateString} => {dt:dd-MMM-yy HH:mm:ss.fff} {tzDisplayName}";
            System.Diagnostics.Trace.WriteLine(conversionSummary);

            validator(now, dt);
        }


        [DataTestMethod]
        [DataRow("")] // Should fall back to invariant culture
        [DataRow("en-US")]
        [DataRow("en-GB")]
        [DataRow("fi-FI")]
        public void BaseRelativeDateTimeShouldBeParsed(string culture) {
            Assert.IsTrue(RelativityParser.TryGetParser(culture, out var parser));
            if (string.IsNullOrWhiteSpace(culture)) {
                Assert.AreEqual(CultureInfo.InvariantCulture.Name, parser.CultureInfo.Name);
            }
            else {
                Assert.AreEqual(culture, parser.CultureInfo.Name);
            }

            var tests = new Dictionary<string, Action<DateTime, DateTime>>() {
                {
                    parser.BaseTime.NowAlt,
                    (now, parsed) => Assert.AreEqual(0, (now - parsed).TotalSeconds)
                },
                {
                    parser.BaseTime.Now,
                    (now, parsed) => Assert.AreEqual(0, (now - parsed).TotalSeconds)
                },
                {
                    parser.BaseTime.CurrentSecond,
                    (now, parsed) => {
                        Assert.AreEqual(parsed.Millisecond, 0);
                        Assert.IsTrue((now - parsed).TotalSeconds < 1);
                    }
                },
                {
                    parser.BaseTime.CurrentMinute,
                    (now, parsed) => {
                        Assert.AreEqual(parsed.Millisecond, 0);
                        Assert.AreEqual(parsed.Second, 0);
                        Assert.IsTrue((now - parsed).TotalMinutes < 1);
                    }
                },
                {
                    parser.BaseTime.CurrentHour,
                    (now, parsed) => {
                        Assert.AreEqual(parsed.Millisecond, 0);
                        Assert.AreEqual(parsed.Second, 0);
                        Assert.AreEqual(parsed.Minute, 0);
                        Assert.IsTrue((now - parsed).TotalHours < 1);
                    }
                },
                {
                    parser.BaseTime.CurrentDay,
                    (now, parsed) => {
                        Assert.AreEqual(parsed.Millisecond, 0);
                        Assert.AreEqual(parsed.Second, 0);
                        Assert.AreEqual(parsed.Minute, 0);
                        Assert.AreEqual(parsed.Hour, 0);
                        Assert.IsTrue((now - parsed).TotalDays < 1);
                    }
                },
                {
                    parser.BaseTime.CurrentWeek,
                    (now, parsed) => {
                        Assert.AreEqual(parsed.Millisecond, 0);
                        Assert.AreEqual(parsed.Second, 0);
                        Assert.AreEqual(parsed.Minute, 0);
                        Assert.AreEqual(parsed.Hour, 0);
                        Assert.AreEqual(parsed.DayOfWeek, parser.CultureInfo.DateTimeFormat.FirstDayOfWeek);
                        Assert.IsTrue((now - parsed).TotalDays < 7);
                    }
                },
                {
                    parser.BaseTime.CurrentMonth,
                    (now, parsed) => {
                        Assert.AreEqual(parsed.Millisecond, 0);
                        Assert.AreEqual(parsed.Second, 0);
                        Assert.AreEqual(parsed.Minute, 0);
                        Assert.AreEqual(parsed.Hour, 0);
                        Assert.AreEqual(parsed.Day, 1);
                        Assert.IsTrue((now - parsed).TotalDays < 31);
                    }
                },
                {
                    parser.BaseTime.CurrentYear,
                    (now, parsed) => {
                        Assert.AreEqual(parsed.Millisecond, 0);
                        Assert.AreEqual(parsed.Second, 0);
                        Assert.AreEqual(parsed.Minute, 0);
                        Assert.AreEqual(parsed.Hour, 0);
                        Assert.AreEqual(parsed.Day, 1);
                        Assert.AreEqual(parsed.Month, 1);
                        Assert.IsTrue((now - parsed).TotalDays < 366);
                    }
                }
            };

            foreach (var item in tests) {
                DateTimeParseTest(parser, item.Key, TimeZoneInfo.Local, item.Value, true);
            }
        }


        [DataTestMethod]
        [DataRow("")] // Should fall back to invariant culture
        [DataRow("en-US")]
        [DataRow("en-GB")]
        [DataRow("fi-FI")]
        public void RelativeDateTimeWithOffsetShouldBeParsed(string culture) {
            Assert.IsTrue(RelativityParser.TryGetParser(culture, out var parser));
            if (string.IsNullOrWhiteSpace(culture)) {
                Assert.AreEqual(CultureInfo.InvariantCulture.Name, parser.CultureInfo.Name);
            }
            else {
                Assert.AreEqual(culture, parser.CultureInfo.Name);
            }

            var baseTimeTypes = new[] { 
                nameof(RelativityBaseTimeSettings.Now),
                nameof(RelativityBaseTimeSettings.NowAlt),
                nameof(RelativityBaseTimeSettings.CurrentSecond),
                nameof(RelativityBaseTimeSettings.CurrentMinute),
                nameof(RelativityBaseTimeSettings.CurrentHour),
                nameof(RelativityBaseTimeSettings.CurrentDay),
                nameof(RelativityBaseTimeSettings.CurrentWeek),
                nameof(RelativityBaseTimeSettings.CurrentMonth),
                nameof(RelativityBaseTimeSettings.CurrentYear),
            };

            foreach (var baseTimeType in baseTimeTypes) {
                var tests = GetOffsetTests(parser, baseTimeType);

                foreach (var item in tests) {
                    DateTimeParseTest(parser, item.Key, TimeZoneInfo.Local, item.Value, true);
                }
            }
        }


        [DataTestMethod]
        [DataRow("")] // Should fall back to invariant culture
        [DataRow("en-US")]
        [DataRow("en-GB")]
        [DataRow("fi-FI")]
        public void DurationShouldBeParsed(string culture) {
            Assert.IsTrue(RelativityParser.TryGetParser(culture, out var parser));
            if (string.IsNullOrWhiteSpace(culture)) {
                Assert.AreEqual(CultureInfo.InvariantCulture.Name, parser.CultureInfo.Name);
            }
            else {
                Assert.AreEqual(culture, parser.CultureInfo.Name);
            }

            string unparsed;
            TimeSpan parsed;

            unparsed = GetDuration(parser, 123, x => x.Milliseconds);
            parsed = parser.ToTimeSpan(unparsed);
            Assert.AreEqual(123, parsed.TotalMilliseconds);

            unparsed = GetDuration(parser, 123.456, x => x.Milliseconds);
            parsed = parser.ToTimeSpan(unparsed);
            Assert.AreEqual(123.456, parsed.TotalMilliseconds);

            unparsed = GetDuration(parser, 123, x => x.Seconds);
            parsed = parser.ToTimeSpan(unparsed);
            Assert.AreEqual(123, parsed.TotalSeconds);

            unparsed = GetDuration(parser, 123.456, x => x.Seconds);
            parsed = parser.ToTimeSpan(unparsed);
            Assert.AreEqual(123.456, parsed.TotalSeconds, 0.001);

            unparsed = GetDuration(parser, 123, x => x.Minutes);
            parsed = parser.ToTimeSpan(unparsed);
            Assert.AreEqual(123, parsed.TotalMinutes);

            unparsed = GetDuration(parser, 123.456, x => x.Minutes);
            parsed = parser.ToTimeSpan(unparsed);
            Assert.AreEqual(123.456, parsed.TotalMinutes);

            unparsed = GetDuration(parser, 123, x => x.Hours);
            parsed = parser.ToTimeSpan(unparsed);
            Assert.AreEqual(123, parsed.TotalHours);

            unparsed = GetDuration(parser, 123.456, x => x.Hours);
            parsed = parser.ToTimeSpan(unparsed);
            Assert.AreEqual(123.456, parsed.TotalHours, 0.001);

            unparsed = GetDuration(parser, 123, x => x.Days);
            parsed = parser.ToTimeSpan(unparsed);
            Assert.AreEqual(123, parsed.TotalDays);

            unparsed = GetDuration(parser, 123.456, x => x.Days);
            parsed = parser.ToTimeSpan(unparsed);
            Assert.AreEqual(123.456, parsed.TotalDays, 0.001);

            unparsed = GetDuration(parser, 123, x => x.Weeks);
            parsed = parser.ToTimeSpan(unparsed);
            Assert.AreEqual(123 * 7, parsed.TotalDays);

            unparsed = GetDuration(parser, 123.456, x => x.Weeks);
            parsed = parser.ToTimeSpan(unparsed);
            Assert.AreEqual(123.456 * 7, parsed.TotalDays);
        }

    }
}
