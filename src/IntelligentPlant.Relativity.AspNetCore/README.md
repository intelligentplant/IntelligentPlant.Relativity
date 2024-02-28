# IntelligentPlant.Relativity.AspNetCore

This package provides ASP.NET Core middleware for setting `RelativityParser.Current` for an HTTP request. This allows timestamps and durations to be parsed anywhere in the request using `RelativityParser.Current`.


# Configuration

## Registering Time Zone Providers

The middleware uses time zone providers (derived from [TimeZoneProvider](./TimeZoneProvider.cs)) to determine the time zone to use when configuring the Relativity parser for the request. Use the extension methods defined in [RelativityBuilderExtensions](./RelativityBuilderExtensions.cs) to register time zone providers for the middleware:

```csharp
builder.AddQueryStringTimeZoneProvider()
    .AddRequestHeaderTimeZoneProvider();
```

The middleware will use the first time zone provider that returns a valid time zone. If no time zone provider returns a valid time zone, UTC will be used. The order of precedence for the time zone providers from most-specific to least-specific is as follows:

  * Custom time zone providers, in registration order
  * Query string
  * Cookie
  * Request header
  * User claim


## Registering the Middleware

To register the middleware, use the `UseRelativity()` extension method for the `IApplicationBuilder` class:

```csharp
app.UseRelativity();
```

If you are using request localization, you should register the middleware after the request localization middleware to ensure that the culture for the request is set before the Relativity parser is configured:

```csharp
app.UseRequestLocalization();
app.UseRelativity();
```


# Using the Parser

You can access the parser for the current request using `RelativityParser.Current`. For example:

```csharp
var dt = RelativityParser.Current.ConvertToUtcDateTime("DAY+6H");
```
