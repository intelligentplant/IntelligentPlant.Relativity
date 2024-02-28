using System;
using System.Linq;
using System.Threading.Tasks;

namespace IntelligentPlant.Relativity.Example {
    class Program {

        private const string OutputDateFormat = "dd-MMM-yy HH:mm:ss.fff";

        private static readonly string[] ExampleDates = {
            "NOW-1h",
            "*-1h",
            "DAY+1W",
            "second-5m",
            "Hour+1d",
            "MONTH",
            "MONTH-3MO",
            "Week-4d",
            "MINUTE-15m",
            "YEAR"
        };


        static void Main(string[] args) {
            var factory = new RelativityParserFactory();
            var parser = factory.GetParser("en-GB");

            foreach (var item in ExampleDates) {
                Console.WriteLine($"[{item}] => {TimeZoneInfo.ConvertTimeFromUtc(parser.ConvertToUtcDateTime(item), TimeZoneInfo.Local).ToString(OutputDateFormat)} {TimeZoneInfo.Local.Id}");
            }
        }

    }
}
