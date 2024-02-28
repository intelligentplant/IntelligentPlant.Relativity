﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
//
//     Modify the WellKnownParsers.csv file to change the default parsers.
// </auto-generated>
// ------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;

namespace IntelligentPlant.Relativity {

    internal static class WellKnownParsers {

        internal static IEnumerable<RelativityParserConfiguration> GetParserDefinitions() {

            yield return new RelativityParserConfiguration() {
                CultureInfo = CultureInfo.GetCultureInfo("en"),
                BaseTimeSettings = new RelativityBaseTimeSettings(
                    now: "NOW",
                    currentSecond: "SECOND",
                    currentMinute: "MINUTE",
                    currentHour: "HOUR",
                    currentDay: "DAY",
                    currentWeek: "WEEK",
                    currentMonth: "MONTH",
                    currentYear: "YEAR"
                ),
                TimeOffsetSettings = new RelativityTimeOffsetSettings(
                    milliseconds: "MS",
                    seconds: "S",
                    minutes: "M",
                    hours: "H",
                    days: "D",
                    weeks: "W",
                    months: "MO",
                    years: "Y"
                )
            };
            
        }
    }
}