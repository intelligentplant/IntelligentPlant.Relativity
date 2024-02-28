# IntelligentPlant.Relativity

A C# library for converting absolute and relative timestamp strings into UTC `DateTime` instances, and duration strings into `TimeSpan` instances.


# Getting Started

All parsers implement the [IRelativityParser](./src/IntelligentPlant.Relativity/IRelativityParser.cs) interface. `IRelativityParser` instances are created using an [IRelativityParserFactory](./src/IntelligentPlant.Relativity/IRelativityParserFactory.cs). The default implementation of `IRelativityParserFactory` is [RelativityParserFactory](./src/IntelligentPlant.Relativity/RelativityParserFactory.cs). You can use `RelativityParserFactory.Default` to obtain a default `IRelativityParserFactory` instance, or create your own instance of `RelativityParserFactory` directly.

`IRelativityParser` parses relative timestamps and durations using a given `CultureInfo` and `TimeZoneInfo`. The `CultureInfo` controls how absolute timestamps and `TimeSpan` literals are parsed, as well as providing keywords that define the base times and time periods that can be used when defining relative timestamps or durations.

To retrieve a parser instance for a specific culture, call the `GetParser` method on the factory:

```csharp
var enGBParser = factory.GetParser(CultureInfo.GetCultureInfo("en-GB"));
var fiFIParser = factory.GetParser(CultureInfo.GetCultureInfo("fi-FI"));
```

If a parser is not registered for a given culture (or any of its parent cultures), a default parser that uses `CultureInfo.InvariantCulture` will be used.

If a parser exists for a parent culture but not the requested culture (e.g. a registration exists for `en` but not `en-GB`), a copy of the parent culture's parser will be created that uses the requested `CultureInfo` instead.

If the time zone for the resolved parser is different to the requested time zone, a copy of the resolved parser will be created that uses the requested time zone instead.


# Parsing Timestamps

Both absolute and relative timestamps can be parsed. Absolute timestamps are parsed according to the parser's `CultureInfo`:

```csharp
var date = parser.ConvertToUtcDateTime("2019-11-15T08:56:25.0901821Z");
```

Relative timestamps are expressed in the format `base_time [+/- offset]`, where the `base_time` matches one of the entries in the parser's `BaseTime` property, and the `offset` is a number followed by one of the duration keywords defined in the `TimeOffset` property. White space inside the expression is ignored and parser uses case-insensitive matching against the `base_time` and `offset` values.

For the default (invariant culture) parser, the following `base_time` values can be used:

- `NOW` (or `*`) - current time.
- `SECOND` - start of the current second.
- `MINUTE` - start of the current minute.
- `HOUR` - start of the current hour.
- `DAY` - start of the current day.
- `WEEK` - start of the current week. The `CultureInfo` for the parser is used to determine what the first day of the week is.
- `MONTH` - start of the current month.
- `YEAR` - start of the current year.

The following units can be used in relative timestamps with the default parser. Both whole and fractional quantities are allowed unless otherwise stated:

- `MS` - milliseconds.
- `S` - seconds.
- `M` - minutes.
- `H` - hours.
- `D` - days.
- `W` - weeks.
- `MO` - months. Fractional quantities are not allowed.
- `Y` - years. Fractional quantities are not allowed.

Examples:

- `NOW + 15S` - current time plus 15 seconds.
- `*-10y` - current time minus 10 years.
- `day-0.5d` - start of current day minus 0.5 days (i.e. 12 hours).
- `MONTH` - start of the current month.
- `YEAR + 3MO` - start of current year plus 3 calendar months.

Base time values are converted to absolute times relative to a given `DateTime`. If no `DateTime` is specified when parsing a timestamp, the current time for the parser's time zone is used. The following example demonstrates how to parse a relative timestamp:

```csharp
var date = parser.ConvertToUtcDateTime("DAY+6H");
```

# Parsing Durations

The parser will parse literal time spans, as well as duration expressions. Literal time spans are parsed according to the parser's `CultureInfo`:

```csharp
var timeSpan = parser.ConvertToTimeSpan("3.19:03:27.775");
```

Duration expressions are expressed in the same way as an offset on a relative timestamp i.e. a quantity followed by one of the duration keywords defined in the `TimeOffset` property of the parser. The following units can be used in duration expressions with the default (invariant culture) parser. Both whole and fractional quantities are allowed unless otherwise stated:

- `MS` - milliseconds.
- `S` - seconds.
- `M` - minutes.
- `H` - hours.
- `D` - days.
- `W` - weeks.

Examples:

- `500ms` - 500 milliseconds.
- `15S` - 15 seconds.
- `0.5H` - 0.5 hours (i.e. 30 minutes).
- `1W` - 1 week.


# Registering Parsers

A default set of well-known parsers are registered with `RelativityParserFactory` when the factory is created. The [WellKnownParsers.csv](./src/IntelligentPlant.Relativity/WellKnownParsers.csv) file defines the cultures and keywords that are automatically registered.

Additional parser configurations can also be passed to the `RelativityParserFactory` constructor; these configurations will override any matching default parser registrations. The invariant culture parser cannot be overridden.

To register a parser for a given culture, call the `IRelativityParserFactory.TryRegisterParser` method:

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
        days: "P",
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

Note that the default (invariant culture) parser cannot be replaced.


# Creating Parsers For Alternative Time Zones

By default, all parsers use the system's local time zone when resolving base dates in relative timestamps. You can explicitly specify a time zone for a parser by passing a `TimeZoneInfo` instance to the `RelativityParserFactory.GetParser` method:

```csharp
var parser = factory.GetParser(CultureInfo.GetCultureInfo("en-GB"), TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
var date = parser.ConvertToUtcDateTime("DAY+6H"); // 6AM today in Pacific Standard Time.
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

If no value has been set for `RelativityParser.Current` for the current asynchronous context, the default invariant culture parser will be returned.
