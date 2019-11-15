namespace IntelligentPlant.Relativity {

    /// <summary>
    /// Describes the identifiers that can be used to describe a period of time.
    /// </summary>
    public class RelativityTimeOffsetSettings {

        /// <summary>
        /// Years. Not available when parsing time span literals. Quanities must be specified 
        /// using whole numbers.
        /// </summary>
        public string Years { get; }

        /// <summary>
        /// Months. Not available when parsing time span literals. Quanities must be specified 
        /// using whole numbers.
        /// </summary>
        public string Months { get; }

        /// <summary>
        /// Weeks. Quanities must be specified using whole numbers.
        /// </summary>
        public string Weeks { get; }

        /// <summary>
        /// Days. Quanities can be specified using whole or fractional numbers.
        /// </summary>
        public string Days { get; }

        /// <summary>
        /// Hours. Quanities can be specified using whole or fractional numbers.
        /// </summary>
        public string Hours { get; }

        /// <summary>
        /// Minutes. Quanities can be specified using whole or fractional numbers.
        /// </summary>
        public string Minutes { get; }

        /// <summary>
        /// Seconds. Quanities can be specified using whole or fractional numbers.
        /// </summary>
        public string Seconds { get; }

        /// <summary>
        /// Milliseconds. Quanities can be specified using whole or fractional numbers.
        /// </summary>
        public string Milliseconds { get; }


        /// <summary>
        /// Creates a new <see cref="RelativityTimeOffsetSettings"/>.
        /// </summary>
        /// <param name="years">
        ///   The identifier to use for years. Specify <see langword="null"/> or white space to 
        ///   disable this unit.
        /// </param>
        /// <param name="months">
        ///   The identifier to use for months. Specify <see langword="null"/> or white space to 
        ///   disable this unit.
        /// </param>
        /// <param name="weeks">
        ///   The identifier to use for weeks. Specify <see langword="null"/> or white space to 
        ///   disable this unit.
        /// </param>
        /// <param name="days">
        ///   The identifier to use for days. Specify <see langword="null"/> or white space to 
        ///   disable this unit.
        /// </param>
        /// <param name="hours">
        ///   The identifier to use for hours. Specify <see langword="null"/> or white space to 
        ///   disable this unit.
        /// </param>
        /// <param name="minutes">
        ///   The identifier to use for minutes. Specify <see langword="null"/> or white space to 
        ///   disable this unit.
        /// </param>
        /// <param name="seconds">
        ///   The identifier to use for seconds. Specify <see langword="null"/> or white space to 
        ///   disable this unit.
        /// </param>
        /// <param name="milliseconds">
        ///   The identifier to use for milliseconds. Specify <see langword="null"/> or white space to 
        ///   disable this unit.
        /// </param>
        public RelativityTimeOffsetSettings(
            string years = "Y",
            string months = "MO",
            string weeks = "W",
            string days = "D",
            string hours = "H",
            string minutes = "M",
            string seconds = "S",
            string milliseconds = "MS"
        ) {
            Years = string.IsNullOrWhiteSpace(years) ? null : years;
            Months = string.IsNullOrWhiteSpace(months) ? null : months;
            Weeks = string.IsNullOrWhiteSpace(weeks) ? null : weeks;
            Days = string.IsNullOrWhiteSpace(days) ? null : days;
            Hours = string.IsNullOrWhiteSpace(hours) ? null : hours;
            Minutes = string.IsNullOrWhiteSpace(minutes) ? null : minutes;
            Seconds = string.IsNullOrWhiteSpace(seconds) ? null : seconds;
            Milliseconds = string.IsNullOrWhiteSpace(milliseconds) ? null : milliseconds;
        }

    }
}
