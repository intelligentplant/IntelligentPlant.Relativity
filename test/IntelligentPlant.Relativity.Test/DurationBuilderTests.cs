using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntelligentPlant.Relativity.Test {

    [TestClass]
    public class DurationBuilderTests {

        [TestMethod]
        public void ShouldBuildDurationString() {
            var duration = new DurationBuilder()
                .Weeks(1)
                .Days(2)
                .Hours(3)
                .Minutes(4)
                .Seconds(5)
                .Milliseconds(6)
                .Build();

            Assert.AreEqual("1W 2D 3H 4M 5S 6MS", duration);
        }


        [TestMethod]
        public void ShouldBuildEmptyDurationString() {
            var duration = new DurationBuilder()
                .Build();

            Assert.AreEqual("0MS", duration);
        }


        [TestMethod]
        public void ShouldIgnoreZeroUnits() {
            var duration = new DurationBuilder()
                .Weeks(0)
                .Days(2)
                .Hours(3)
                .Minutes(0)
                .Seconds(0)
                .Milliseconds(6)
                .Build();

            Assert.AreEqual("2D 3H 6MS", duration);
        }

    }

}
