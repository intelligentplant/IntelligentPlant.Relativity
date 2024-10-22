# Migrating from v1 to v2

Version 2 of Relativity contains several breaking changes from version 1. This document provides guidance on how to migrate from version 1 to version 2.


# Parsers now implement IRelativityParser

The `RelativityParser` class has been made static in v2 and as such cannot be instantiated. Instead, parser instances now implement the new `IRelativityParser` interface which, combined with extension methods defined in the `RelativityParserExtensions` class, provides a similar (but not identical) API surface.

Notable method name changes include:

| v1 | v2 |
|----|----|
| `ToUtcDateTime` | `ConvertToUtcDateTime` |
| `ToTimeSpan` | `ConvertToTimeSpan` |


# Parsers are now created using a factory

In v1, parsers were created by calling the static `RelativityParser.TryGetParser` method. This method has been removed in v2, as it did not allow for the easy creation of parsers that used the same culture but different time zones.

In v2, non-default parsers are created using an `IRelativityParserFactory`. The `RelativityParserFactory` class provides a concrete implementation of the `IRelativityParserFactory` interface. The `RelativityParserFactory.Default` property provides access to a default factory instance.

Custom parser configurations are registered using the `IRelativityParserFactory.TryRegisterParser` method.

All `RelativityParserFactory` instances define a set of well-known parser configurations. These are defined in the [WellKnownParsers.csv](../src/IntelligentPlant.Relativity/WellKnownParsers.csv) file. This file is used by the [WellKnownParsers.tt](../src/IntelligentPlant.Relativity/WellKnownParsers.tt) text template file to generate compile-time source that registers the well-known parsers with the factory. Additional parser configurations can be passed to the factory constructor; well-known parser registrations can be replaced using this approach if desired.


# RelativityParser.Default has been removed

The `RelativityParser.Default` property has been removed in v2. Instead, the following static properties are defined:

* `RelativityParser.Invariant` - a parser that uses `CultureInfo.InvariantCulture` and the system's local time zone.
* `RelativityParser.InvariantUtc` - a parser that uses `CultureInfo.InvariantCulture` and UTC.
* `RelativityParser.Current` - the parser for the current asynchronous context. See below for more details.


# RelativityParser.Current is local to the asynchronous control flow

Instead of having to pass a parser instance around in your code, you can now use the `RelativityParser.Current` property to get or set the parser for the current asynchronous context. `RelativityParser.Current` will return `RelativityParser.InvariantUtc` if it has not been explicitly set for the current context.

`RelativityParser.Current` uses [AsyncLocal&lt;T&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.asynclocal-1) to store the parser instance for the current asynchronous context. This means that the parser instance is not shared between different asynchronous contexts (such as pipelines for different HTTP requests).
