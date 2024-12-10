using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntelligentPlant.Relativity.Test {

    [TestClass]
    public class TimeSpanConversionTests {

        [DataTestMethod]
        [DataRow("1.18:00:00", DurationUnitKind.Days, 2, "1.75D")]
        [DataRow("1.18:00:00", DurationUnitKind.Days, 1, "1.8D")]
        [DataRow("1.18:00:00", DurationUnitKind.Days, 0, "2D")]
        [DataRow("18:30:00", DurationUnitKind.Hours, 1, "18.5H")]
        [DataRow("18:30:00", DurationUnitKind.Hours, 0, "19H")]
        [DataRow("00:15:45", DurationUnitKind.Minutes, 2, "15.75M")]
        [DataRow("00:15:45", DurationUnitKind.Minutes, 1, "15.8M")]
        [DataRow("00:15:45", DurationUnitKind.Minutes, 0, "16M")]
        [DataRow("00:00:01.250", DurationUnitKind.Seconds, 2, "1.25S")]
        [DataRow("00:00:01.250", DurationUnitKind.Seconds, 1, "1.3S")]
        [DataRow("00:00:01.250", DurationUnitKind.Seconds, 0, "2S")]
        [DataRow("00:00:00.2345678", DurationUnitKind.Milliseconds, -1, "234.5678MS")]
        [DataRow("00:00:00.2345768", DurationUnitKind.Milliseconds, 0, "235MS")]
        [DataRow("-1.18:00:00", DurationUnitKind.Days, 2, "-1.75D")]
        [DataRow("-1.18:00:00", DurationUnitKind.Days, 1, "-1.8D")]
        [DataRow("-1.18:00:00", DurationUnitKind.Days, 0, "-2D")]
        [DataRow("-18:30:00", DurationUnitKind.Hours, 1, "-18.5H")]
        [DataRow("-18:30:00", DurationUnitKind.Hours, 0, "-19H")]
        [DataRow("-00:15:45", DurationUnitKind.Minutes, 2, "-15.75M")]
        [DataRow("-00:15:45", DurationUnitKind.Minutes, 1, "-15.8M")]
        [DataRow("-00:15:45", DurationUnitKind.Minutes, 0, "-16M")]
        [DataRow("-00:00:01.250", DurationUnitKind.Seconds, 2, "-1.25S")]
        [DataRow("-00:00:01.250", DurationUnitKind.Seconds, 1, "-1.3S")]
        [DataRow("-00:00:01.250", DurationUnitKind.Seconds, 0, "-2S")]
        [DataRow("-00:00:00.2345678", DurationUnitKind.Milliseconds, -1, "-234.5678MS")]
        [DataRow("-00:00:00.2345768", DurationUnitKind.Milliseconds, 0, "-235MS")]
        public void ShouldConvertToDurationWithSpecifiedPrecision(string timeSpanLiteral, DurationUnitKind unit, int decimalPlaces, string expectedDuration) {
            var timeSpan = TimeSpan.Parse(timeSpanLiteral);
            var duration = RelativityParser.InvariantUtc.ConvertToDuration(timeSpan, unit, decimalPlaces);
            Assert.AreEqual(expectedDuration, duration);
        }


        [DataTestMethod]
        [DataRow("1.18:00:00", DurationUnitKind.Unspecified, "1D 18H")]
        [DataRow("1.18:00:00", DurationUnitKind.Hours, "42H")]
        [DataRow("01:30:00", DurationUnitKind.Unspecified, "1H 30M")]
        [DataRow("01:30:00", DurationUnitKind.Minutes, "90M")]
        [DataRow("00:05:30", DurationUnitKind.Unspecified, "5M 30S")]
        [DataRow("00:05:30", DurationUnitKind.Seconds, "330S")]
        [DataRow("00:00:01.500", DurationUnitKind.Unspecified, "1S 500MS")]
        [DataRow("00:00:01.500", DurationUnitKind.Milliseconds, "1500MS")]
        public void ShouldConvertToDurationWithSpecifiedUnits(string timeSpanLiteral, DurationUnitKind unit, string expectedDuration) {
            var timeSpan = TimeSpan.Parse(timeSpanLiteral);
            var duration = RelativityParser.InvariantUtc.ConvertToDuration(timeSpan, unit: unit);
            Assert.AreEqual(expectedDuration, duration);
        }

    }

}
