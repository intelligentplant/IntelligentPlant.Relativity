using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

using IntelligentPlant.Relativity.Internal;

namespace IntelligentPlant.Relativity {

    /// <summary>
    /// Default implementation of <see cref="IRelativityParserFactory"/>.
    /// </summary>
    public sealed class RelativityParserFactory : IRelativityParserFactory {

        /// <summary>
        /// The default <see cref="IRelativityParserFactory"/> instance.
        /// </summary>
        public static IRelativityParserFactory Default { get; } = new RelativityParserFactory();


        /// <summary>
        /// The default parser configurations, indexed by culture name (e.g. en-GB).
        /// </summary>
        private readonly ConcurrentDictionary<string, RelativityParserConfiguration> _defaultParsers = new ConcurrentDictionary<string, RelativityParserConfiguration>(StringComparer.OrdinalIgnoreCase);


        /// <summary>
        /// Creates a new <see cref="RelativityParserFactory"/> instance with pre-registered 
        /// entries for well-known and additional custom parser configurations.
        /// </summary>
        /// <param name="parsers">
        ///   The parsers to register with the factory, in addition to the default well-known 
        ///   parser configurations.
        /// </param>
        public RelativityParserFactory(IEnumerable<RelativityParserConfiguration>? parsers) {
            foreach (var config in WellKnownParsers.GetParserDefinitions()) {
                _defaultParsers[config.CultureInfo.Name] = config;
            }

            if (parsers != null) {
                foreach (var config in parsers) {
                    if (config == null) {
                        continue;
                    }
                    Validator.ValidateObject(config, new ValidationContext(config), true);
                    _defaultParsers[config.CultureInfo.Name] = config;
                }
            }
        }


        /// <summary>
        /// Creates a new <see cref="RelativityParserFactory"/> instance with pre-registered 
        /// entries for well-known parser configurations.
        /// </summary>
        public RelativityParserFactory() : this(null) { }


        /// <inheritdoc/>
        public bool TryRegisterParser(RelativityParserConfiguration config, bool replaceExisting = false) {
            if (config == null) {
                throw new ArgumentNullException(nameof(config));
            }
            Validator.ValidateObject(config, new ValidationContext(config), true);

            if (CultureInfo.InvariantCulture.Equals(config.CultureInfo)) {
                throw new ArgumentException(Resources.Error_CannotReplaceInvariantCultureParser, nameof(config));
            }

            var registered = _defaultParsers.AddOrUpdate(config.CultureInfo.Name, _ => config, (_, existing) => replaceExisting ? config : existing);
            return registered == config;
        }


        /// <inheritdoc/>
        public IRelativityParser GetParser(CultureInfo? cultureInfo, TimeZoneInfo? timeZone) {
            if (cultureInfo == null) {
                cultureInfo = CultureInfo.InvariantCulture;
            }
            if (timeZone == null) {
                timeZone = TimeZoneInfo.Local;
            }

            if (cultureInfo == null || cultureInfo.Name.Equals(CultureInfo.InvariantCulture.Name, StringComparison.OrdinalIgnoreCase)) {
                // Invariant culture parser requested.
                return GetInvariantParser(timeZone);
            }

            // Culture-specific parser requested.
            return GetParserCore(cultureInfo, timeZone);
        }


        /// <summary>
        /// Gets an invariant culture parser for the specified time zone.
        /// </summary>
        /// <param name="timeZone">
        ///   The time zone.
        /// </param>
        /// <returns>
        ///   The parser.
        /// </returns>
        private IRelativityParser GetInvariantParser(TimeZoneInfo timeZone) {
            // For local or UTC time zones, we can use the built-in invariant parsers. Otherwise,
            // we will clone the local invariant parser with a new time zone.
            return timeZone.Equals(TimeZoneInfo.Local)
                ? RelativityParser.Invariant
                : timeZone.Equals(TimeZoneInfo.Utc)
                    ? RelativityParser.InvariantUtc
                    : CreateParser(RelativityParser.Invariant, null, timeZone);
        }


        /// <summary>
        /// Retrieves an <see cref="IRelativityParser"/> for the specified culture and time zone.
        /// </summary>
        /// <param name="cultureInfo">
        ///   The culture.
        /// </param>
        /// <param name="timeZone">
        ///   The time zone.
        /// </param>
        /// <returns>
        ///   The <see cref="IRelativityParser"/> for the culture.
        /// </returns>
        /// <remarks>
        /// 
        /// <para>
        ///   If a parser exists for a parent culture of the requested culture (e.g. the requested 
        ///   culture is <c>en-GB</c> and a parser exists for <c>en</c>), a new parser for the 
        ///   requested child culture will be created using the settings from the parent culture.
        /// </para>
        /// 
        /// <para>
        ///   If no parser exists for the requested culture, a parser for the invariant culture 
        ///   will be returned.
        /// </para>
        /// 
        /// </remarks>
        private IRelativityParser GetParserCore(CultureInfo cultureInfo, TimeZoneInfo timeZone) {
            var ci = cultureInfo;

            while (true) {
                if (ci == null || CultureInfo.InvariantCulture.Name.Equals(ci.Name, StringComparison.OrdinalIgnoreCase)) {
                    break;
                }

                if (_defaultParsers.TryGetValue(ci.Name, out var p)) {
                    if (!string.Equals(ci.Name, cultureInfo.Name, StringComparison.OrdinalIgnoreCase)) {
                        // We have found an entry for a parent culture of the one that was originally 
                        // requested (e.g. "en" instead of "en-GB"). We'll create and return an 
                        // entry for the specific culture that was requested, in case e.g. the 
                        // specific culture uses a different first day of week to the parent culture.
                        return CreateParser(p, cultureInfo, timeZone);
                    }

                    // We found an exact match for the requested culture.
                    return CreateParser(p, null, timeZone);
                }

                ci = ci.Parent;
            }

            return CreateParser(RelativityParser.InvariantUtc, cultureInfo, timeZone);
        }


        /// <summary>
        /// Creates an <see cref="IRelativityParser"/> from a <see cref="RelativityParserConfiguration"/>.
        /// </summary>
        /// <param name="configuration">
        ///   The parser configuration.
        /// </param>
        /// <param name="cultureInfo">
        ///   The culture to use for the parser, or <see langword="null"/> to use the culture 
        ///   specified in the <paramref name="configuration"/>.
        /// </param>
        /// <param name="timeZone">
        ///   The time zone to use for the parser.
        /// </param>
        /// <returns>
        ///   A new <see cref="IRelativityParser"/> instance.
        /// </returns>
        private IRelativityParser CreateParser(RelativityParserConfiguration configuration, CultureInfo? cultureInfo, TimeZoneInfo timeZone) {
            return new Parser(cultureInfo ?? configuration.CultureInfo, timeZone, configuration.BaseTimeSettings, configuration.TimeOffsetSettings);
        }


        /// <summary>
        /// Creates an <see cref="IRelativityParser"/> from an existing <see cref="IRelativityParser"/>.
        /// </summary>
        /// <param name="existing">
        ///   The existing parser.
        /// </param>
        /// <param name="cultureInfo">
        ///   The culture to use for the parser, or <see langword="null"/> to use the culture 
        ///   specified in the existing parser.
        /// </param>
        /// <param name="timeZone">
        ///   The time zone to use for the parser.
        /// </param>
        /// <returns>
        ///   A new <see cref="IRelativityParser"/> instance.
        /// </returns>
        private IRelativityParser CreateParser(IRelativityParser existing, CultureInfo? cultureInfo, TimeZoneInfo timeZone) {
            return new Parser(cultureInfo ?? existing.CultureInfo, timeZone, existing.BaseTimeSettings, existing.TimeOffsetSettings);
        }

    }
}
