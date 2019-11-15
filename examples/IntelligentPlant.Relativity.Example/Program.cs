using System;

namespace IntelligentPlant.Relativity.Example {
    class Program {

        private const string OutputDateFormat = "dd-MMM-yy HH:mm:ss.fffZ";

        private static readonly string[] ExampleDates = { 
            "NOW-1h",
            "*-1h",
            "DAY+1W",
            "second-5m",
            "Hour+1d",
            "MONTH-3MO",
            "Week-4d",
            "MINUTE-15m"
        };


        static void Main(string[] args) {
            foreach (var item in ExampleDates) {
                Console.WriteLine($"[{item}] => {RelativityParser.Default.ToUtcDateTime(item).ToString(OutputDateFormat)}");
            }
        }

    }
}
