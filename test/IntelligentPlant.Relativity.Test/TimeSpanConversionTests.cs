using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntelligentPlant.Relativity.Test {

    [TestClass]
    public class TimeSpanConversionTests {

        [DataTestMethod]
        [DataRow("1.18:00:00", false, "1.75D")]
        [DataRow("1.18:00:00", true, "2D")]
        [DataRow("18:30:00", false, "18.5H")]
        [DataRow("18:30:00", true, "19H")]
        [DataRow("00:15:45", false, "15.75M")]
        [DataRow("00:15:45", true, "16M")]
        [DataRow("00:00:01.250", false, "1.25S")]
        [DataRow("00:00:01.250", true, "2S")]
        [DataRow("00:00:00.2345678", false, "234.5678MS")]
        [DataRow("00:00:00.2345768", true, "235MS")]
        [DataRow("-1.18:00:00", false, "-1.75D")]
        [DataRow("-1.18:00:00", true, "-2D")]
        [DataRow("-18:30:00", false, "-18.5H")]
        [DataRow("-18:30:00", true, "-19H")]
        [DataRow("-00:15:45", false, "-15.75M")]
        [DataRow("-00:15:45", true, "-16M")]
        [DataRow("-00:00:01.250", false, "-1.25S")]
        [DataRow("-00:00:01.250", true, "-2S")]
        [DataRow("-00:00:00.2345678", false, "-234.5678MS")]
        [DataRow("-00:00:00.2345768", true, "-235MS")]
        public void ShouldConvertToDuration(string timeSpanLiteral, bool truncateDuration, string expectedDuration) { 
            var timeSpan = TimeSpan.Parse(timeSpanLiteral);
            var duration = RelativityParser.InvariantUtc.ConvertToDuration(timeSpan, truncateDuration);
            Assert.AreEqual(expectedDuration, duration);
        }


    }

}
