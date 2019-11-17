using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntelligentPlant.Relativity.Test {
    [TestClass]
    public class DateTimeParsingTests {

        private void DateTimeParseTest(RelativityParser parser, string dateString, TimeZoneInfo timeZone, Action<string, DateTime> validator, bool convertBackToInputTimeZone) {
            if (!parser.TryConvertToUtcDateTime(dateString, out var dt, timeZone)) {
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

            validator(conversionSummary, dt);
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

            var tests = new Dictionary<string, Action<string, DateTime>>() {
                {
                    parser.BaseTime.NowAlt,
                    (summary, dt) => Assert.AreEqual(0, (DateTime.Now - dt).TotalSeconds, 0.2)
                },
                {
                    parser.BaseTime.Now,
                    (summary, dt) => Assert.AreEqual(0, (DateTime.Now - dt).TotalSeconds, 0.2)
                },
                {
                    parser.BaseTime.CurrentSecond,
                    (summary, dt) => {
                        Assert.AreEqual(dt.Millisecond, 0);
                        // Delta of 2 because it's feasible that the relative timestamp could be 
                        // parsed just before the start of a new second, and the callback could 
                        // run just after.
                        Assert.IsTrue((DateTime.Now - dt).TotalSeconds < 2);
                    }
                },
                {
                    parser.BaseTime.CurrentMinute,
                    (summary, dt) => {
                        Assert.AreEqual(dt.Millisecond, 0);
                        Assert.AreEqual(dt.Second, 0);
                        // Delta of 2 because it's feasible that the relative timestamp could be 
                        // parsed just before the start of a new minute, and the callback could 
                        // run just after.
                        Assert.IsTrue((DateTime.Now - dt).TotalMinutes < 2);
                    }
                },
                {
                    parser.BaseTime.CurrentHour,
                    (summary, dt) => {
                        Assert.AreEqual(dt.Millisecond, 0);
                        Assert.AreEqual(dt.Second, 0);
                        Assert.AreEqual(dt.Minute, 0);
                        // Delta of 2 because it's feasible that the relative timestamp could be 
                        // parsed just before the start of a new hour, and the callback could 
                        // run just after.
                        Assert.IsTrue((DateTime.Now - dt).TotalHours < 2);
                    }
                },
                {
                    parser.BaseTime.CurrentDay,
                    (summary, dt) => {
                        Assert.AreEqual(dt.Millisecond, 0);
                        Assert.AreEqual(dt.Second, 0);
                        Assert.AreEqual(dt.Minute, 0);
                        Assert.AreEqual(dt.Hour, 0);
                        // Delta of 2 because it's feasible that the relative timestamp could be 
                        // parsed just before the start of a new day, and the callback could 
                        // run just after.
                        Assert.IsTrue((DateTime.Now - dt).TotalDays < 2);
                    }
                },
                {
                    parser.BaseTime.CurrentWeek,
                    (summary, dt) => {
                        Assert.AreEqual(dt.Millisecond, 0);
                        Assert.AreEqual(dt.Second, 0);
                        Assert.AreEqual(dt.Minute, 0);
                        Assert.AreEqual(dt.Hour, 0);
                        Assert.AreEqual(dt.DayOfWeek, parser.CultureInfo.DateTimeFormat.FirstDayOfWeek);
                        // Delta of 8 because it's feasible that the relative timestamp could be 
                        // parsed just before the start of a new day, and the callback could 
                        // run just after.
                        Assert.IsTrue((DateTime.Now - dt).TotalDays < 8);
                    }
                },
                {
                    parser.BaseTime.CurrentMonth,
                    (summary, dt) => {
                        Assert.AreEqual(dt.Millisecond, 0);
                        Assert.AreEqual(dt.Second, 0);
                        Assert.AreEqual(dt.Minute, 0);
                        Assert.AreEqual(dt.Hour, 0);
                        Assert.AreEqual(dt.Day, 1);
                        // Delta of 32 because it's feasible that the relative timestamp could be 
                        // parsed just before the start of a new month, and the callback could 
                        // run just after.
                        Assert.IsTrue((DateTime.Now - dt).TotalDays < 32);
                    }
                },
                {
                    parser.BaseTime.CurrentYear,
                    (summary, dt) => {
                        Assert.AreEqual(dt.Millisecond, 0);
                        Assert.AreEqual(dt.Second, 0);
                        Assert.AreEqual(dt.Minute, 0);
                        Assert.AreEqual(dt.Hour, 0);
                        Assert.AreEqual(dt.Day, 1);
                        Assert.AreEqual(dt.Month, 1);
                        // Delta of 367 because it's feasible that the relative timestamp could be 
                        // parsed just before the start of a new year, and the callback could 
                        // run just after.
                        Assert.IsTrue((DateTime.Now - dt).TotalDays < 367);
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
        public void RelativeDateTimeWithOffsetFromCurrentTimeShouldBeParsed(string culture) {
            Assert.IsTrue(RelativityParser.TryGetParser(culture, out var parser));
            if (string.IsNullOrWhiteSpace(culture)) {
                Assert.AreEqual(CultureInfo.InvariantCulture.Name, parser.CultureInfo.Name);
            }
            else {
                Assert.AreEqual(culture, parser.CultureInfo.Name);
            }

            var tests = new Dictionary<string, Action<string, DateTime>>() {
                {
                    "-800ms",
                    (summary, dt) => {
                        Assert.AreEqual(800, (DateTime.Now - dt).TotalMilliseconds, 100);
                    }
                },
                {
                    "+567ms",
                    (summary, dt) => {
                        Assert.AreEqual(567, (dt - DateTime.Now).TotalMilliseconds, 100);
                    }
                },
                {
                    "-5s",
                    (summary, dt) => {
                        Assert.AreEqual(5, (DateTime.Now - dt).TotalSeconds, 0.1);
                    }
                },
                {
                    "+10s",
                    (summary, dt) => {
                        Assert.AreEqual(10, (dt - DateTime.Now).TotalSeconds, 0.1);
                    }
                },

            };

            foreach (var item in tests) {
                DateTimeParseTest(parser, parser.BaseTime.Now + item.Key, TimeZoneInfo.Local, item.Value, true);
            }
        }

    }
}
