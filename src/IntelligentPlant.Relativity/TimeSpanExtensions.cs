using System;

namespace IntelligentPlant.Relativity {

    /// <summary>
    /// Extensions for <see cref="TimeSpan"/>.
    /// </summary>
    public static class TimeSpanExtensions {

        /// <summary>
        /// Converts the <see cref="TimeSpan"/> to a Relativity duration string.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="TimeSpan"/>.
        /// </param>
        /// <param name="parser">
        ///   The <see cref="IRelativityParser"/> to use when converting the <see cref="TimeSpan"/>. 
        ///   Specify <see langword="null"/> to use <see cref="RelativityParser.InvariantUtc"/>.
        /// </param>
        /// <returns>
        ///   A Relativity duration string that represents the <see cref="TimeSpan"/>.
        /// </returns>
        public static string ToDurationString(this TimeSpan value, IRelativityParser? parser = null) {
            return (parser ?? RelativityParser.InvariantUtc).ConvertToDuration(value);
        }


        /// <summary>
        /// Converts the <see cref="TimeSpan"/> to a Relativity duration string that uses the specified time unit.
        /// </summary>
        /// <param name="value">
        ///   The <see cref="TimeSpan"/>.
        /// </param>
        /// <param name="parser">
        ///   The <see cref="IRelativityParser"/> to use when converting the <see cref="TimeSpan"/>. 
        ///   Specify <see langword="null"/> to use <see cref="RelativityParser.InvariantUtc"/>.
        /// </param>
        /// <param name="unit">
        ///   The time unit to use when converting the <see cref="TimeSpan"/>. You can specify any 
        ///   valid duration unit defined by the <paramref name="parser"/>.
        /// </param>
        /// <param name="decimalPlaces">
        ///   The number of decimal places to use when converting the <see cref="TimeSpan"/>. For 
        ///   example, specifying one decimal place when a time span of <c>00:00:00.1234567</c> is 
        ///   specified will result in a duration string of <c>123.5MS</c> being returned (or its 
        ///   equivalent localized value).
        /// </param>
        /// <returns>
        ///   A Relativity duration string that represents the <see cref="TimeSpan"/>.
        /// </returns>
        public static string ToDurationString(this TimeSpan value, IRelativityParser? parser, DurationUnitKind unit, int decimalPlaces = -1) { 
            return (parser ?? RelativityParser.InvariantUtc).ConvertToDuration(value, unit, decimalPlaces);
        }

    }
}
