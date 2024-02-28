using System.ComponentModel.DataAnnotations;
using System.Globalization;

#nullable disable warnings

namespace IntelligentPlant.Relativity {

    /// <summary>
    /// Describes the configuration for an <see cref="IRelativityParser"/>.
    /// </summary>
    public class RelativityParserConfiguration {

        /// <summary>
        /// The culture for the parser.
        /// </summary>
        [Required]
        public CultureInfo CultureInfo { get; set; } = CultureInfo.CurrentCulture;

        /// <summary>
        /// The base time keyword settings for the parser.
        /// </summary>
        [Required]
        public RelativityBaseTimeSettings BaseTimeSettings { get; set; } = new RelativityBaseTimeSettings();

        /// <summary>
        /// The time offset and duration keyword settings for the parser.
        /// </summary>
        [Required]
        public RelativityTimeOffsetSettings TimeOffsetSettings { get; set; } = new RelativityTimeOffsetSettings();

    }

}
