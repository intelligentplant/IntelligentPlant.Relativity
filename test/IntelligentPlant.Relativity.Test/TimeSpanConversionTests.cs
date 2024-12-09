using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntelligentPlant.Relativity.Test {

    [TestClass]
    public class TimeSpanConversionTests {

        [DataTestMethod]
        [DataRow("1.18:00:00", 2, "1.75D")]
        [DataRow("1.18:00:00", 1, "1.8D")]
        [DataRow("1.18:00:00", 0, "2D")]
        [DataRow("18:30:00", 1, "18.5H")]
        [DataRow("18:30:00", 0, "19H")]
        [DataRow("00:15:45", 2, "15.75M")]
        [DataRow("00:15:45", 1, "15.8M")]
        [DataRow("00:15:45", 0, "16M")]
        [DataRow("00:00:01.250", 2, "1.25S")]
        [DataRow("00:00:01.250", 1, "1.3S")]
        [DataRow("00:00:01.250", 0, "2S")]
        [DataRow("00:00:00.2345678", -1, "234.5678MS")]
        [DataRow("00:00:00.2345768", 0, "235MS")]
        [DataRow("-1.18:00:00", 2, "-1.75D")]
        [DataRow("-1.18:00:00", 1, "-1.8D")]
        [DataRow("-1.18:00:00", 0, "-2D")]
        [DataRow("-18:30:00", 1, "-18.5H")]
        [DataRow("-18:30:00", 0, "-19H")]
        [DataRow("-00:15:45", 2, "-15.75M")]
        [DataRow("-00:15:45", 1, "-15.8M")]
        [DataRow("-00:15:45", 0, "-16M")]
        [DataRow("-00:00:01.250", 2, "-1.25S")]
        [DataRow("-00:00:01.250", 1, "-1.3S")]
        [DataRow("-00:00:01.250", 0, "-2S")]
        [DataRow("-00:00:00.2345678", -1, "-234.5678MS")]
        [DataRow("-00:00:00.2345768", 0, "-235MS")]
        public void ShouldConvertToDurationWithSpecifiedPrecision(string timeSpanLiteral, int decimalPlaces, string expectedDuration) {
            var timeSpan = TimeSpan.Parse(timeSpanLiteral);
            var duration = RelativityParser.InvariantUtc.ConvertToDuration(timeSpan, decimalPlaces: decimalPlaces);
            Assert.AreEqual(expectedDuration, duration);
        }


        [DataTestMethod]
        [DataRow("1.18:00:00", "H", "42H")]
        [DataRow("01:30:00", "M", "90M")]
        [DataRow("00:05:30", "S", "330S")]
        [DataRow("00:00:01.500", "MS", "1500MS")]
        public void ShouldConvertToDurationWithSpecifiedUnits(string timeSpanLiteral, string unit, string expectedDuration) {
            var timeSpan = TimeSpan.Parse(timeSpanLiteral);
            var duration = RelativityParser.InvariantUtc.ConvertToDuration(timeSpan, unit: unit);
            Assert.AreEqual(expectedDuration, duration);
        }


    }

}
