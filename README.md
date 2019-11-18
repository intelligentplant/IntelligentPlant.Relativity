# IntelligentPlant.Relativity

A C# library for converting absolute and relative timestamp strings into `DateTime` instances, and duration strings into `TimeSpan` instances.


# Getting Started

Conversion is performed using the [RelativityParser](./src/IntelligentPlant.Relativity/RelativityParser.cs) class.

The default parser instance (which uses `CultureInfo.InvariantCulture` when parsing) is accessed using the static `RelativityParser.Default` property. Alternatively, you can use one of the `TryGetParser` method overloads to try and retrieve the parser for a specific culture:

```csharp
var successEnGB = RelativityParser.TryGetParser("en-GB", out var enGB);
var successFiFI = RelativityParser.TryGetParser("fi-FI", out var fiFI);
```

If a parser is not registered for a given culture (or any of its parent cultures), you can use use `RelativityParser.Default`, or you can create and register a new parser for the culture using the static `TryRegisterParser` method. See below for more details. Registered parser instances are cached for re-use. 

If a parser exists for a parent culture but not the requested culture (e.g. a registration exists for `en` but not `en-GB`), a new entry will be added to the parser list for the more-specific culture that uses the same parser settings but a different `CultureInfo` to the entry for the parent culture.


# Parsing Timestamps

Both absolute and relative timestamps can be parsed. Absolute timestamps are parsed according to the parser's `CultureInfo`:

```csharp
var date = RelativityParser.Default.ToUtcDateTime("2019-11-15T08:56:25.0901821Z");
```

Relative timestamps are expressed in the format `base_time [+/- offset]`, where the `base_time` matches one of the entries in the parser's `BaseTime` property, and the `offset` is a number followed by one of the duration keywords defined in the `TimeOffset` property. White space inside the expression is ignored and parser uses case-insensitive matching against the `base_time` and `offset` values.

For the default parser, the following `base_time` values can be used:

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
- `W` - weeks. Fractional quantities are not allowed.
- `MO` - months. Fractional quantities are not allowed.
- `Y` - years. Fractional quantities are not allowed.

Examples:

- `NOW + 15S` - current time plus 15 seconds.
- `*-10y` - current time minus 10 years.
- `day-0.5d` - start of current day minus 0.5 days (i.e. 12 hours).
- `YEAR + 3MO` - start of current year plus 3 calendar months.


# Parsing Durations

The parser will parse literal time spans, as well as duration expressions. Literal time spans are parsed according to the parser's `CultureInfo`:

```csharp
var timeSpan = RelativityParser.Default.ToTimeSpan("3.19:03:27.775");
```

Duration expressions are expressed in the same way as an offset on a relative timestamp i.e. a quantity followed by one of the duration keywords defined in the `TimeOffset` property of the parser. The following units can be used in duration expressions with the default parser. Both whole and fractional quantities are allowed unless otherwise stated:

- `MS` - milliseconds.
- `S` - seconds.
- `M` - minutes.
- `H` - hours.
- `D` - days.
- `W` - weeks. Fractional quantities are not allowed.

Examples:

- `500ms` - 500 milliseconds.
- `15S` - 15 seconds.
- `0.5H` - 0.5 hours (i.e. 30 minutes).
- `1W` - 1 week.


# Registering Parsers

A default set of parsers are registered during the first call to the static `RelativityParser.GetAvailableCultures`, `RelativityParser.TryGetParser`, and `RelativityParser.TryRegisterParser` methods. The [Settings.csv](./src/IntelligentPlant.Relativity/Settings.csv) file defines the cultures and keywords that are automatically registered.

To register a non-default parser for a given culture, call the static `RelativityParser.TryRegisterParser` method:

```csharp
var fiFI = new RelativityParser(
    CultureInfo.GetCultureInfo("fi-FI"),
    new RelativityBaseTimeSettings(
        now: "NYT",
        currentSecond: "SEKUNTI",
        currentMinute: "MINUUTTI",
        currentHour: "TUNTI",
        currentDay: "PÄIVÄ",
        currentWeek: "VIIKKO",
        currentMonth: "KUUKAUSI",
        currentYear: "VUOSI"
    ),
    new RelativityTimeOffsetSettings(
        milliseconds: "MS",
        seconds: "S",
        minutes: "M",
        hours: "T",
        days: "P",
        weeks: "VI",
        months: "KK",
        years: "V"
    )
);

var success = RelativityParser.TryRegisterParser(fiFI);
```

By default, existing parser registrations are not overwritten. To force registration, specify a value for the `replaceExisting` parameter:

```csharp
var success = RelativityParser.TryRegisterParser(fiFI, replaceExisting: true);
```

Note that the default (invariant culture) parser cannot be replaced.
