using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace IntelligentPlant.Relativity {

    /// <summary>
    /// Describes the identifiers that can be used as base times in a relative timestamp.
    /// </summary>
    public sealed class RelativityBaseTimeSettings {

        /// <summary>
        /// The start of the current calendar year. When <see langword="null"/>, this base time 
        /// cannot be used.
        /// </summary>
        [MaxLength(30)]
        public string? CurrentYear { get; }

        /// <summary>
        /// The start of the current month. When <see langword="null"/>, this base time 
        /// cannot be used.
        /// </summary>
        [MaxLength(30)]
        public string? CurrentMonth { get; }

        /// <summary>
        /// The start of the current week. When <see langword="null"/>, this base time 
        /// cannot be used.
        /// </summary>
        [MaxLength(30)]
        public string? CurrentWeek { get; }

        /// <summary>
        /// The start of the current day. When <see langword="null"/>, this base time 
        /// cannot be used.
        /// </summary>
        [MaxLength(30)]
        public string? CurrentDay { get; }

        /// <summary>
        /// The start of the current hour. When <see langword="null"/>, this base time 
        /// cannot be used.
        /// </summary>
        [MaxLength(30)]
        public string? CurrentHour { get; }

        /// <summary>
        /// The start of the current minute. When <see langword="null"/>, this base time 
        /// cannot be used.
        /// </summary>
        [MaxLength(30)]
        public string? CurrentMinute { get; }

        /// <summary>
        /// The start of the current second. When <see langword="null"/>, this base time 
        /// cannot be used.
        /// </summary>
        [MaxLength(30)]
        public string? CurrentSecond { get; }

        /// <summary>
        /// The current time. When <see langword="null"/>, this base time cannot be used.
        /// </summary>
        [MaxLength(30)]
        public string? Now { get; }

        /// <summary>
        /// The current time (alternative format). This keyword is constant and is always 
        /// available.
        /// </summary>
        public string? NowAlt { get { return "*"; } }


        /// <summary>
        /// Creates a new <see cref="RelativityBaseTimeSettings"/>.
        /// </summary>
        /// <param name="now">
        ///   The identifier to use for the current time. Specify <see langword="null"/> or white 
        ///   space to mark this base time as unavailable.
        /// </param>
        /// <param name="currentSecond">
        ///   The identifier to use for the start of the current second. Specify <see langword="null"/> 
        ///   or white space to mark this base time as unavailable.
        /// </param>
        /// <param name="currentMinute">
        ///   The identifier to use for the start of the current minute. Specify <see langword="null"/> 
        ///   or white space to mark this base time as unavailable.
        /// </param>
        /// <param name="currentHour">
        ///   The identifier to use for the start of the current hour. Specify <see langword="null"/> 
        ///   or white space to mark this base time as unavailable.
        /// </param>
        /// <param name="currentDay">
        ///   The identifier to use for the start of the current day. Specify <see langword="null"/> 
        ///   or white space to mark this base time as unavailable.
        /// </param>
        /// <param name="currentWeek">
        ///   The identifier to use for the start of the current week. Specify <see langword="null"/> 
        ///   or white space to mark this base time as unavailable.
        /// </param>
        /// <param name="currentMonth">
        ///   The identifier to use for the start of the current month. Specify <see langword="null"/> 
        ///   or white space to mark this base time as unavailable.
        /// </param>
        /// <param name="currentYear">
        ///   The identifier to use for the start of the current year. Specify <see langword="null"/> 
        ///   or white space to mark this base time as unavailable.
        /// </param>
        public RelativityBaseTimeSettings(
            string now = "NOW",
            string currentSecond = "SECOND",
            string currentMinute = "MINUTE",
            string currentHour = "HOUR",
            string currentDay = "DAY",
            string currentWeek = "WEEK",
            string currentMonth = "MONTH",
            string currentYear = "YEAR"
        ) {
            Now = string.IsNullOrWhiteSpace(now) ? null : now;
            CurrentSecond = string.IsNullOrWhiteSpace(currentSecond) ? null : currentSecond;
            CurrentMinute = string.IsNullOrWhiteSpace(currentMinute) ? null : currentMinute;
            CurrentHour = string.IsNullOrWhiteSpace(currentHour) ? null : currentHour;
            CurrentDay = string.IsNullOrWhiteSpace(currentDay) ? null : currentDay;
            CurrentWeek = string.IsNullOrWhiteSpace(currentWeek) ? null : currentWeek;
            CurrentMonth = string.IsNullOrWhiteSpace(currentMonth) ? null : currentMonth;
            CurrentYear = string.IsNullOrWhiteSpace(currentYear) ? null : currentYear;
        }


        /// <summary>
        /// Gets the defined keywords.
        /// </summary>
        /// <returns>
        ///   The defined keywords.
        /// </returns>
        private IEnumerable<KeyValuePair<string, string>> GetDefinedKeywords() {
            if (Now != null) {
                yield return new KeyValuePair<string, string>(nameof(Now), Now);
            }
            if (CurrentSecond != null) {
                yield return new KeyValuePair<string, string>(nameof(CurrentSecond), CurrentSecond);
            }
            if (CurrentMinute != null) {
                yield return new KeyValuePair<string, string>(nameof(CurrentMinute), CurrentMinute);
            }
            if (CurrentHour != null) {
                yield return new KeyValuePair<string, string>(nameof(CurrentHour), CurrentHour);
            }
            if (CurrentDay != null) {
                yield return new KeyValuePair<string, string>(nameof(CurrentDay), CurrentDay);
            }
            if (CurrentWeek != null) {
                yield return new KeyValuePair<string, string>(nameof(CurrentWeek), CurrentWeek);
            }
            if (CurrentMonth != null) {
                yield return new KeyValuePair<string, string>(nameof(CurrentMonth), CurrentMonth);
            }
            if (CurrentYear != null) {
                yield return new KeyValuePair<string, string>(nameof(CurrentYear), CurrentYear);
            }
        }


        /// <inheritdoc/>
        public override string ToString() {
            var sb = new StringBuilder();

            sb.Append("{");
            var first = true;
            foreach (var keyword in GetDefinedKeywords()) {
                if (first) {
                    first = false;
                    sb.Append(' ');
                }
                else {
                    sb.Append(", ");
                }
                sb.Append($"\"{keyword.Key}\": \"{keyword.Value}\"");
            }
            sb.Append(" }");

            return sb.ToString();
        }

    }
}
