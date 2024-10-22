# IntelligentPlant.Relativity.DependencyInjection

This package provides extensions for registering Relativity services with Microsoft.Extensions.DependencyInjection.


# Getting Started

Use the `AddRelativity()` extension method for `IServiceCollection` to register Relativity services with the dependency injection container and return an `IRelativityBuilder` that can be used to perform additional configuration. For example:

```csharp
services.AddRelativity().AddParserConfiguration(new RelativityParserConfiguration() {
    CultureInfo = CultureInfo.GetCultureInfo("fi-FI"),
    BaseTimeSettings = new RelativityBaseTimeSettings(
        now: "NYT",
        currentSecond: "SEKUNTI",
        currentMinute: "MINUUTTI",
        currentHour: "TUNTI",
        currentDay: "PÄIVÄ",
        currentWeek: "VIIKKO",
        currentMonth: "KUUKAUSI",
        currentYear: "VUOSI"
    ),
    TimeOffsetSettings = new RelativityTimeOffsetSettings(
        milliseconds: "MS",
        seconds: "S",
        minutes: "M",
        hours: "T",
        days: "VK", // Vuorokausi i.e. a 24-hour period of time.
        weeks: "VI",
        months: "KK",
        years: "V"
    )
});
```
