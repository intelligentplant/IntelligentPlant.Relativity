using System;
using System.Globalization;

namespace IntelligentPlant.Relativity {

    /// <summary>
    /// A factory for creating <see cref="IRelativityParser"/> instances.
    /// </summary>
    public interface IRelativityParserFactory {

        /// <summary>
        /// Gets a <see cref="IRelativityParser"/> instance for the specified culture and time zone.
        /// </summary>
        /// <param name="cultureInfo">
        ///   The culture to use for parsing timestamp and duration strings. Specify <see langword="null"/> 
        ///   to request a parser for the invariant culture.
        /// </param>
        /// <param name="timeZone">
        ///   The time zone to use when parsing relative dates. Specify <see langword="null"/> to 
        ///   use <see cref="TimeZoneInfo.Local"/>.
        /// </param>
        /// <returns>
        ///   An <see cref="IRelativityParser"/> instance that uses the specified culture and time 
        ///   zone.
        /// </returns>
        IRelativityParser GetParser(CultureInfo? cultureInfo, TimeZoneInfo? timeZone);


        /// <summary>
        /// Tries to register a parser
        /// </summary>
        /// <param name="parser">
        ///   The parser configuration.
        /// </param>
        /// <param name="replaceExisting">
        ///   Specifies if the parser should replace an existing parser with the same culture.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the parser was successfully registered; otherwise, 
        ///   <see langword="false"/>.
        /// </returns>
        bool TryRegisterParser(RelativityParserConfiguration parser, bool replaceExisting = false);

    }

}
