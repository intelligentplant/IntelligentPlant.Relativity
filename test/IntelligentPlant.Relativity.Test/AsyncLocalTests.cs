using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntelligentPlant.Relativity.Test {

    [TestClass]
    public class AsyncLocalTests {

        public TestContext TestContext { get; set; }


        [TestMethod]
        public async Task RelativityParserCurrentShouldHaveCorrectTimeZone() {
            var factory = new RelativityParserFactory();
            var timeZones = TimeZoneInfo.GetSystemTimeZones();

            var tasks = timeZones.Select(async x => {
                await RunForTimeZoneAsync(factory, x, TestContext.CancellationTokenSource.Token).ConfigureAwait(false);
            }).ToList();

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }


        private static async Task RunForTimeZoneAsync(IRelativityParserFactory factory, TimeZoneInfo timeZone, CancellationToken cancellationToken) {
            var parser = factory.GetParser(null, timeZone);
            RelativityParser.Current = parser;
            await Task.Yield();
            cancellationToken.ThrowIfCancellationRequested();
            Assert.AreSame(parser, RelativityParser.Current, $"Expected time zone '{parser.TimeZone.Id}' but actual time zone was '{RelativityParser.Current.TimeZone.Id}'.");
            var startOfYear = RelativityParser.Current.ConvertToUtcDateTime("YEAR");
            Debug.WriteLine($"[{timeZone.Id}] Start of year: {startOfYear:yyyy-MM-ddTHH:mm:ss}Z");
        }

    }

}
