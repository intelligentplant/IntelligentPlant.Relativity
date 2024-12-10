using System;
using System.Text;

namespace IntelligentPlant.Relativity {

    /// <summary>
    /// A builder for creating duration strings.
    /// </summary>
    public sealed class DurationBuilder {

        /// <summary>
        /// The parser to use when building the duration string.
        /// </summary>
        private readonly IRelativityParser _parser;

        private double? _milliseconds;

        private double? _seconds;

        private double? _minutes;

        private double? _hours;

        private double? _days;

        private double? _weeks;


        /// <summary>
        /// Creates a new <see cref="DurationBuilder"/> instance.
        /// </summary>
        /// <param name="parser">
        ///   The parser to use when building the duration string. Specify <see langword="null"/> 
        ///   to use <see cref="RelativityParser.InvariantUtc"/>.
        /// </param>
        /// <exception cref="ArgumentException">
        ///   The parser does not define symbols for any duration components.
        /// </exception>
        public DurationBuilder(IRelativityParser? parser = null) {
            _parser = parser ?? RelativityParser.InvariantUtc;
            if (_parser.TimeOffsetSettings.Weeks == null &&
                _parser.TimeOffsetSettings.Days == null &&
                _parser.TimeOffsetSettings.Hours == null &&
                _parser.TimeOffsetSettings.Minutes == null &&
                _parser.TimeOffsetSettings.Seconds == null &&
                _parser.TimeOffsetSettings.Milliseconds == null) {
                throw new ArgumentException("No duration components are enabled.", nameof(parser));
            }
        }


        private void ThrowOnUndefinedSymbol(string? symbol) {
            if (symbol == null) {
                throw new InvalidOperationException("Parser does not define a symbol for this duration unit.");
            }
        }


        private void ThrowOnInvalidValue(double value) {
            if (value < 0) {
                throw new ArgumentOutOfRangeException(nameof(value), "Duration values cannot be less than zero.");
            }
        }


        /// <summary>
        /// Sets the value of a duration component.
        /// </summary>
        /// <param name="unit">
        ///   The duration component to set.
        /// </param>
        /// <param name="value">
        ///   The duration value.
        /// </param>
        /// <returns>
        ///   The builder.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="unit"/> is <see cref="DurationUnitKind.Unspecified"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="value"/> is less than zero.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   The <see cref="IRelativityParser"/> for the builder does not define a symbol for the 
        ///   specified <paramref name="unit"/>.
        /// </exception>
        public DurationBuilder SetComponent(DurationUnitKind unit, double value) {
            ThrowOnInvalidValue(value);

            switch (unit) {
                case DurationUnitKind.Weeks:
                    ThrowOnUndefinedSymbol(_parser.TimeOffsetSettings.Weeks);
                    _weeks = value;
                    break;
                case DurationUnitKind.Days:
                    ThrowOnUndefinedSymbol(_parser.TimeOffsetSettings.Days);
                    _days = value;
                    break;
                case DurationUnitKind.Hours:
                    ThrowOnUndefinedSymbol(_parser.TimeOffsetSettings.Hours);
                    _hours = value;
                    break;
                case DurationUnitKind.Minutes:
                    ThrowOnUndefinedSymbol(_parser.TimeOffsetSettings.Minutes);
                    _minutes = value;
                    break;
                case DurationUnitKind.Seconds:
                    ThrowOnUndefinedSymbol(_parser.TimeOffsetSettings.Seconds);
                    _seconds = value;
                    break;
                case DurationUnitKind.Milliseconds:
                    ThrowOnUndefinedSymbol(_parser.TimeOffsetSettings.Milliseconds);
                    _milliseconds = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unit));
            }

            return this;
        }


        /// <summary>
        /// Sets the number of weeks in the duration.
        /// </summary>
        /// <param name="value">
        ///   The number of weeks.
        /// </param>
        /// <returns>
        ///   The builder.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="value"/> is less than zero.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   The <see cref="IRelativityParser"/> for the builder does not define a symbol for weeks.
        /// </exception>
        public DurationBuilder Weeks(double value) => SetComponent(DurationUnitKind.Weeks, value);


        /// <summary>
        /// Sets the number of days in the duration.
        /// </summary>
        /// <param name="value">
        ///   The number of days.
        /// </param>
        /// <returns>
        ///   The builder.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="value"/> is less than zero.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   The <see cref="IRelativityParser"/> for the builder does not define a symbol for days.
        /// </exception>
        public DurationBuilder Days(double value) => SetComponent(DurationUnitKind.Days, value);


        /// <summary>
        /// Sets the number of hours in the duration.
        /// </summary>
        /// <param name="value">
        ///   The number of hours.
        /// </param>
        /// <returns>
        ///   The builder.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="value"/> is less than zero.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   The <see cref="IRelativityParser"/> for the builder does not define a symbol for hours.
        /// </exception>
        public DurationBuilder Hours(double value) => SetComponent(DurationUnitKind.Hours, value);


        /// <summary>
        /// Sets the number of minutes in the duration.
        /// </summary>
        /// <param name="value">
        ///   The number of minutes.
        /// </param>
        /// <returns>
        ///   The builder.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="value"/> is less than zero.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   The <see cref="IRelativityParser"/> for the builder does not define a symbol for minutes.
        /// </exception>
        public DurationBuilder Minutes(double value) => SetComponent(DurationUnitKind.Minutes, value);


        /// <summary>
        /// Sets the number of seconds in the duration.
        /// </summary>
        /// <param name="value">
        ///   The number of seconds.
        /// </param>
        /// <returns>
        ///   The builder.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="value"/> is less than zero.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   The <see cref="IRelativityParser"/> for the builder does not define a symbol for seconds.
        /// </exception>
        public DurationBuilder Seconds(double value) => SetComponent(DurationUnitKind.Seconds, value);


        /// <summary>
        /// Sets the number of milliseconds in the duration.
        /// </summary>
        /// <param name="value">
        ///   The number of milliseconds.
        /// </param>
        /// <returns>
        ///   The builder.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="value"/> is less than zero.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   The <see cref="IRelativityParser"/> for the builder does not define a symbol for milliseconds.
        /// </exception>
        public DurationBuilder Milliseconds(double value) => SetComponent(DurationUnitKind.Milliseconds, value);


        /// <summary>
        /// Builds the duration string.
        /// </summary>
        /// <returns>
        ///   The duration string.
        /// </returns>
        public string Build() {
            if (!_milliseconds.HasValue && !_seconds.HasValue && !_minutes.HasValue && !_hours.HasValue && !_days.HasValue && !_weeks.HasValue) {
                return _parser.CreateEmptyDurationString();
            }

            var sb = new StringBuilder();
            var appendSeparator = false;

            if (_weeks > 0) {
                appendSeparator = true;
                sb.AppendFormat(_parser.CultureInfo, "{0}{1}", _weeks, _parser.GetSymbol(DurationUnitKind.Weeks));
            }

            if (_days > 0) {
                if (appendSeparator) {
                    sb.Append(" ");
                }
                else {
                    appendSeparator = true;
                }
                sb.AppendFormat(_parser.CultureInfo, "{0}{1}", _days, _parser.GetSymbol(DurationUnitKind.Days));
            }

            if (_hours > 0) {
                if (appendSeparator) {
                    sb.Append(" ");
                }
                else {
                    appendSeparator = true;
                }
                sb.AppendFormat(_parser.CultureInfo, "{0}{1}", _hours, _parser.GetSymbol(DurationUnitKind.Hours));
            }

            if (_minutes > 0) {
                if (appendSeparator) {
                    sb.Append(" ");
                }
                else {
                    appendSeparator = true;
                }
                sb.AppendFormat(_parser.CultureInfo, "{0}{1}", _minutes, _parser.GetSymbol(DurationUnitKind.Minutes));
            }

            if (_seconds > 0) {
                if (appendSeparator) {
                    sb.Append(" ");
                }
                else {
                    appendSeparator = true;
                }
                sb.AppendFormat(_parser.CultureInfo, "{0}{1}", _seconds, _parser.GetSymbol(DurationUnitKind.Seconds));
            }

            if (_milliseconds > 0) {
                if (appendSeparator) {
                    sb.Append(" ");
                }
                else {
                    appendSeparator = true;
                }
                sb.AppendFormat(_parser.CultureInfo, "{0}{1}", _milliseconds, _parser.GetSymbol(DurationUnitKind.Milliseconds));
            }

            return sb.Length == 0
                ? _parser.CreateEmptyDurationString()
                : sb.ToString();

        }

    }
}
