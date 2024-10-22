# IntelligentPlant.Relativity.Owin

This package provides OWIN middleware for setting `RelativityParser.Current` for an HTTP request. This allows timestamps and durations to be parsed anywhere in the request using `RelativityParser.Current`.


# Getting Started

The middleware uses time zone providers (derived from `TimeZoneProvider`) to determine the time zone to use when configuring the Relativity parser for the request. Use the extension methods defined in `AppBuilderExtensions` to register the middleware, `IRelativityParserFactory` and the time zone providers to use:

```csharp
app.UseRelativity(RelativityParserFactory.Default,
    // Set time zone using 'tz' query string parameter
    new QueryStringTimeZoneProvider(),
    // Set time zone using 'X-TimeZone' request header
    new RequestHeaderTimeZoneProvider());
```

Unlike the [ASP.NET Core](https://www.nuget.org/packages/IntelligentPlant.Relativity.AspNetCore) package, the time zone providers are always run in the order that they are specified. The first time zone provider that returns a valid time zone will be used. If no time zone provider returns a valid time zone, UTC will be used.

Note that OWIN does not have built-in capababilities for setting `CultureInfo.CurrentCulture` or `CultureInfo.CurrentUICulture` for the current request. Therefore, you will need to set these manually in the application pipeline prior to the Relativity middleware if you want to allow culture-specific Relativity parsers to be used based on e.g. the `Accept-Language` header specified by the calling user agent.


# Using the Parser

You can access the parser for the current request using `RelativityParser.Current`. For example:

```csharp
var dt = RelativityParser.Current.ConvertToUtcDateTime("DAY+6H");
```
