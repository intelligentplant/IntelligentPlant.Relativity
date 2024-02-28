using System;
using System.Globalization;

namespace IntelligentPlant.Relativity {

    /// <summary>
    /// Extension methods for <see cref="IRelativityParserFactory"/>.
    /// </summary>
    public static class RelativityParserFactoryExtensions {

        /// <summary>
        /// Gets a <see cref="IRelativityParser"/> instance for the specified culture and the time zone 
        /// of the local machine.
        /// </summary>
        /// <param name="factory">
        ///   The parser factory.
        /// </param>
        /// <param name="cultureInfo">
        ///   The culture to use for parsing timestamp and duration strings. Specify <see langword="null"/> 
        ///   to request a parser for the invariant culture.
        /// </param>
        /// <returns>
        ///   An <see cref="IRelativityParser"/> instance that uses the specified culture and the 
        ///   local machine time zone.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="factory"/> is <see langword="null"/>.
        /// </exception>
        public static IRelativityParser GetParser(this IRelativityParserFactory factory, CultureInfo? cultureInfo) {
            if (factory == null) {
                throw new ArgumentNullException(nameof(factory));
            }

            return factory.GetParser(cultureInfo, null);
        }


        /// <summary>
        /// Gets a <see cref="IRelativityParser"/> instance for the specified culture and time zone.
        /// </summary>
        /// <param name="factory">
        ///   The parser factory.
        /// </param>
        /// <param name="cultureName">
        ///   The culture to use for parsing timestamp and duration strings. Specify <see langword="null"/> 
        ///   or white space to request a parser for the invariant culture.
        /// </param>
        /// <param name="timeZone">
        ///   The time zone to use when parsing relative dates. Specify <see langword="null"/> to 
        ///   use <see cref="TimeZoneInfo.Local"/>.
        /// </param>
        /// <returns>
        ///   An <see cref="IRelativityParser"/> instance that uses the specified culture and time 
        ///   zone.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="factory"/> is <see langword="null"/>.
        /// </exception>
        public static IRelativityParser GetParser(this IRelativityParserFactory factory, string cultureName, TimeZoneInfo? timeZone) {
            if (factory == null) {
                throw new ArgumentNullException(nameof(factory));
            }

            var culture = string.IsNullOrWhiteSpace(cultureName) 
                ? null 
                : CultureInfo.GetCultureInfo(cultureName);

            return factory.GetParser(culture, timeZone);
        }


        /// <summary>
        /// Gets a <see cref="IRelativityParser"/> instance for the specified culture and the time zone 
        /// of the local machine.
        /// </summary>
        /// <param name="factory">
        ///   The parser factory.
        /// </param>
        /// <param name="cultureName">
        ///   The culture to use for parsing timestamp and duration strings. Specify <see langword="null"/> 
        ///   to request a parser for the invariant culture.
        /// </param>
        /// <returns>
        ///   An <see cref="IRelativityParser"/> instance that uses the specified culture and the 
        ///   local machine time zone.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="factory"/> is <see langword="null"/>.
        /// </exception>
        public static IRelativityParser GetParser(this IRelativityParserFactory factory, string cultureName) {
            if (factory == null) {
                throw new ArgumentNullException(nameof(factory));
            }

            return factory.GetParser(cultureName, null);
        }


        /// <summary>
        /// Gets a <see cref="IRelativityParser"/> instance for the specified culture and time zone.
        /// </summary>
        /// <param name="factory">
        ///   The parser factory.
        /// </param>
        /// <param name="lcid">
        ///   The locale identifier (LCID) for the culture to use for parsing timestamp and 
        ///   duration strings.
        /// </param>
        /// <param name="timeZone">
        ///   The time zone to use when parsing relative dates. Specify <see langword="null"/> to 
        ///   use <see cref="TimeZoneInfo.Local"/>.
        /// </param>
        /// <returns>
        ///   An <see cref="IRelativityParser"/> instance that uses the specified culture and time 
        ///   zone.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="factory"/> is <see langword="null"/>.
        /// </exception>
        public static IRelativityParser GetParser(this IRelativityParserFactory factory, int lcid, TimeZoneInfo? timeZone) {
            if (factory == null) {
                throw new ArgumentNullException(nameof(factory));
            }

            return factory.GetParser(CultureInfo.GetCultureInfo(lcid), timeZone);
        }


        /// <summary>
        /// Gets a <see cref="IRelativityParser"/> instance for the specified culture and the time zone 
        /// of the local machine.
        /// </summary>
        /// <param name="factory">
        ///   The parser factory.
        /// </param>
        /// <param name="lcid">
        ///   The locale identifier (LCID) for the culture to use for parsing timestamp and 
        ///   duration strings.
        /// </param>
        /// <returns>
        ///   An <see cref="IRelativityParser"/> instance that uses the specified culture and the 
        ///   local machine time zone.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="factory"/> is <see langword="null"/>.
        /// </exception>
        public static IRelativityParser GetParser(this IRelativityParserFactory factory, int lcid) {
            if (factory == null) {
                throw new ArgumentNullException(nameof(factory));
            }

            return factory.GetParser(lcid, null);
        }

    }
}
