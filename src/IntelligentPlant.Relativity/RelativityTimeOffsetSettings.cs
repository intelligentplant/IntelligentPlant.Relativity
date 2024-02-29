﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace IntelligentPlant.Relativity {

    /// <summary>
    /// Describes the identifiers that can be used to describe a period of time.
    /// </summary>
    public sealed class RelativityTimeOffsetSettings {

        /// <summary>
        /// Years. Not available when parsing time span literals. Quantities must be specified 
        /// using whole numbers. When <see langword="null"/>, this unit cannot be used.
        /// </summary>
        [MaxLength(30)]
        public string? Years { get; }

        /// <summary>
        /// Months. Not available when parsing time span literals. Quantities must be specified 
        /// using whole numbers. When <see langword="null"/>, this unit cannot be used.
        /// </summary>
        [MaxLength(30)]
        public string? Months { get; }

        /// <summary>
        /// Weeks. Quantities can be specified using whole or fractional numbers. When 
        /// <see langword="null"/>, this unit cannot be used.
        /// </summary>
        [MaxLength(30)]
        public string? Weeks { get; }

        /// <summary>
        /// Days. Quantities can be specified using whole or fractional numbers. When 
        /// <see langword="null"/>, this unit cannot be used.
        /// </summary>
        [MaxLength(30)]
        public string? Days { get; }

        /// <summary>
        /// Hours. Quantities can be specified using whole or fractional numbers. When 
        /// <see langword="null"/>, this unit cannot be used.
        /// </summary>
        [MaxLength(30)]
        public string? Hours { get; }

        /// <summary>
        /// Minutes. Quantities can be specified using whole or fractional numbers. When 
        /// <see langword="null"/>, this unit cannot be used.
        /// </summary>
        [MaxLength(30)]
        public string? Minutes { get; }

        /// <summary>
        /// Seconds. Quantities can be specified using whole or fractional numbers. When 
        /// <see langword="null"/>, this unit cannot be used.
        /// </summary>
        [MaxLength(30)]
        public string? Seconds { get; }

        /// <summary>
        /// Milliseconds. Quantities can be specified using whole or fractional numbers. When 
        /// <see langword="null"/>, this unit cannot be used.
        /// </summary>
        [MaxLength(30)]
        public string? Milliseconds { get; }


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
        /// <exception cref="ArgumentException">
        ///   All specified units are <see langword="null"/> or white space.
        /// </exception>
        public RelativityTimeOffsetSettings(
            string milliseconds = "MS",
            string seconds = "S",
            string minutes = "M",
            string hours = "H",
            string days = "D",
            string weeks = "W",
            string months = "MO",
            string years = "Y"
        ) {
            Milliseconds = string.IsNullOrWhiteSpace(milliseconds) ? null : milliseconds;
            Seconds = string.IsNullOrWhiteSpace(seconds) ? null : seconds;
            Minutes = string.IsNullOrWhiteSpace(minutes) ? null : minutes;
            Hours = string.IsNullOrWhiteSpace(hours) ? null : hours;
            Days = string.IsNullOrWhiteSpace(days) ? null : days;
            Weeks = string.IsNullOrWhiteSpace(weeks) ? null : weeks;
            Months = string.IsNullOrWhiteSpace(months) ? null : months;
            Years = string.IsNullOrWhiteSpace(years) ? null : years;

            var timeSpanUnits = new[] { Milliseconds, Seconds, Minutes, Hours, Days, Weeks };
            if (timeSpanUnits.All(x => x == null)) {
                throw new ArgumentException(Resources.Error_AtLeastOneTimeSpanUnitIsRequired);
            }
        }

    }
}
