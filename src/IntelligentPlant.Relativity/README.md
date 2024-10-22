# IntelligentPlant.Relativity

A C# library for converting absolute and relative timestamp strings into UTC `DateTime` instances, and duration strings into `TimeSpan` instances.


# Getting Started

The `IRelativityParser` interface defines a parser that can be used to convert relative timestamps and durations into absolute `DateTime` and `TimeSpan` instances. 

Ain `IRelativityParser` parses timestamps and durations using a given `CultureInfo` and `TimeZoneInfo`. The `CultureInfo` controls how absolute timestamps and `TimeSpan` literals are parsed, as well as providing keywords that define the base times and time periods that can be used when defining relative timestamps or durations. The `TimeZoneInfo` controls the time zone that is used when converting relative timestamps to absolute timestamps.

The static `RelativityParser` class provides quick access to several built-in parsers:

* `RelativityParser.Invariant` - a parser that uses `CultureInfo.InvariantCulture` and the system's local time zone when converting relative timestamps to absolute timestamps.
* `RelativityParser.InvariantUtc` - a parser that uses `CultureInfo.InvariantCulture` and UTC when converting relative timestamps to absolute timestamps.
* `RelativityParser.Current` - the parser for the current asynchronous context. See [Asynchronous Control Flow](#asynchronous-control-flow) for more information.

The `IRelativityParserFactory` interface defines a factory for creating custom `IRelativityParser` instances. The default implementation of `IRelativityParserFactory` is `RelativityParserFactory`. You can use the static `RelativityParserFactory.Default` property to obtain the default `IRelativityParserFactory` instance, or create your own instance of `RelativityParserFactory` directly.

Parser configurations for additional cultures can be registered with the `IRelativityParserFactory`. To retrieve a parser instance for a specific culture, call the `GetParser` method on the factory:

```csharp
// Get a culture-specific parser that uses the system's local time zone.
var enGBParser = factory.GetParser(CultureInfo.GetCultureInfo("en-GB"));
```

You can also specify a time zone for the parser to use when parsing relative timestamps:

```csharp
// Get a culture-specific parser that uses a specific time zone for relative 
// timestamp conversion.
var parser = factory.GetParser(CultureInfo.GetCultureInfo("en-GB"), 
    TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
```

If a parser configuration is not registered for a requested culture (or any of its parent cultures), the returned parser will use the default (invariant) base time and time offset keywords when parsing relative timestamps and durations.


# Parsing Timestamps

Both absolute and relative timestamps can be parsed. Absolute timestamps are parsed according to the parser's `CultureInfo`:

```csharp
var date = parser.ConvertToUtcDateTime("2019-11-15T08:56:25.0901821Z");
```

Relative timestamps are expressed in the format `base_time [+/- offset]`, where the `base_time` matches one of the keywords defined in the parser's `BaseTimeSettings` property, and the `offset` is a number followed by one of the duration keywords defined in the parser's `TimeOffsetSettings` property (e.g. `3D` for 3 days when using the invariant culture parser). White space inside the expression is ignored and the parser uses case-insensitive matching against the `base_time` and `offset` values. 

For invariant and English (`en`) culture parsers, the following `base_time` values can be used:

| Keyword | Description | Notes |
|---------|-------------| ----- |
| `NOW` (or `*`) | Current time. |
| `SECOND` | Start of the current second. |
| `MINUTE` | Start of the current minute. |
| `HOUR` | Start of the current hour. |
| `DAY` | Start of the current day. |
| `WEEK` | Start of the current week. | The `CultureInfo` for the parser is used to determine what the first day of the week is. |
| `MONTH` | Start of the current month. |
| `YEAR` | Start of the current year. |

The following units can be used in relative timestamps with invariant and English-language parsers:

| Keyword | Description | Fractional Quantities Allowed |
|---------|-------------| ----------------------------- |
| `MS` | Milliseconds | Yes |
| `S` | Seconds | Yes |
| `M` | Minutes | Yes |
| `H` | Hours | Yes |
| `D` | Days | Yes |
| `W` | Weeks | Yes |
| `MO` | Months | __No__ |
| `Y` | Years | __No__ |

Examples:

| Example | Description |
|---------|-------------|
| `NOW + 15S` | Current time plus 15 seconds. |
| `*-10y` | Current time minus 10 years. |
| `day-0.5d` | Start of current day minus 0.5 days (i.e. 12 hours). |
| `MONTH` | Start of the current month. |
| `YEAR + 3MO` | Start of current year plus 3 calendar months. |

> Note that the culture of the parser is also used when parsing offset quantities. For example, when using a parser with the `fi-FI` culture, the parser will expect a comma as a decimal separator when specifying a fractional quantity.

Base time values are converted to absolute times relative to a given `DateTime` origin. If no origin is specified when parsing a timestamp, the current time for the parser's time zone is used:

```csharp
// No origin specified; current time for the parser's time zone is used as the origin.
var date = parser.ConvertToUtcDateTime("DAY+6H");

// Use 10 days ago in the parser's time zone as the origin.
var date2 = parser.ConvertToUtcDateTime("DAY+6H", 
    parser.TimeZone.GetCurrentTime().AddDays(-10));
```

# Parsing Durations

The parser will parse literal time spans, as well as duration expressions. Literal time spans are parsed according to the parser's `CultureInfo`:

```csharp
var timeSpan = parser.ConvertToTimeSpan("3.19:03:27.775");
```

Duration expressions are expressed in the same way as an offset on a relative timestamp i.e. a quantity followed by one of the duration keywords defined in the `TimeOffsetSettings` property of the parser. The following units can be used in duration expressions with invariant and English-language parsers.

| Keyword | Description | Fractional Quantities Allowed |
|---------|-------------| ----------------------------- |
| `MS` | Milliseconds | Yes |
| `S` | Seconds | Yes |
| `M` | Minutes | Yes |
| `H` | Hours | Yes |
| `D` | Days | Yes |
| `W` | Weeks | Yes |

Examples:

| Example | Description |
|---------|-------------|
| `500ms` | 500 milliseconds. |
| `15S` | 15 seconds. |
| `0.5H` | 0.5 hours (i.e. 30 minutes). |
| `1W` | 1 week. |

> Note that the culture of the parser is also used when parsing duration expressions. For example, when using a parser with the `fi-FI` culture, the parser will expect a comma as a decimal separator when specifying a fractional quantity.


# Registering Parsers

A default set of well-known parser configurations are automatically registered with `RelativityParserFactory` when the factory is created.

Additional parser configurations can also be passed to the `RelativityParserFactory` constructor; these configurations will override any matching default parser registrations. The invariant culture parsers cannot be overridden.

To manually register a parser configuration for a given culture, call the `IRelativityParserFactory.TryRegisterParser` method:

```csharp
var fiFI = new RelativityParserConfiguration() {
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
};

var success = factory.TryRegisterParser(fiFI);
```

By default, existing parser registrations are not overwritten. To force registration, specify a value for the `replaceExisting` parameter:

```csharp
var success = factory.TryRegisterParser(fiFI, replaceExisting: true);
```


# Asynchronous Control Flow

You can use the static `RelativityParser.Current` property to get or set the parser for the current asynchronous context. This allows you to use the same parser anywhere within an asynchronous call stack without having to pass the parser as a parameter to each method. For example:

```csharp
private static async Task RunAsync(IRelativityParserFactory factory) {
    var factory = new RelativityParserFactory();
    var tasks = new[] { "Tokyo Standard Time", "Pacific Standard Time" }.Select(x => RunForTimeZoneAsync(factory, x));
    await Task.WhenAll(tasks);
}


private static async Task RunForTimeZoneAsync(IRelativityParserFactory factory, string timeZone) {
    RelativityParser.Current = factory.GetParser(CultureInfo.InvariantCulture, TimeZoneInfo.FindSystemTimeZoneById(timeZone));
    await PrintStartOfMonthAsync();
}


private static async Task PrintStartOfMonthAsync() {
    await Task.Delay(TimeSpan.FromSeconds(1));
    var date = RelativityParser.Current.ConvertToUtcDateTime("MONTH");
    Console.WriteLine($"{RelativityParser.Current.TimeZone.Id}: {date} UTC");
}

// Example output:
// Tokyo Standard Time: 31/01/2024 15:00:00 UTC
// Pacific Standard Time: 01/02/2024 08:00:00 UTC
```

If no value has been set for `RelativityParser.Current` for the current asynchronous context, `RelativityParser.InvariantUtc` will be returned.

In [ASP.NET Core](https://www.nuget.org/packages/IntelligentPlant.Relativity.AspNetCore) or [OWIN](https://www.nuget.org/packages/IntelligentPlant.Relativity.Owin) web applications, you can use middleware to automatically set `RelativityParser.Current` for each request based on the culture and time zone information specified in the request.
