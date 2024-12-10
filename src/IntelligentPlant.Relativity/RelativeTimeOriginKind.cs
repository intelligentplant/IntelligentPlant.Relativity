namespace IntelligentPlant.Relativity {

    /// <summary>
    /// Describes the kind of a relative time origin.
    /// </summary>
    public enum RelativeTimeOriginKind {

        /// <summary>
        /// Unspecified.
        /// </summary>
        Unspecified,

        /// <summary>
        /// Current time.
        /// </summary>
        Now,

        /// <summary>
        /// The start of the current second.
        /// </summary>
        CurrentSecond,

        /// <summary>
        /// The start of the current minute.
        /// </summary>
        CurrentMinute,

        /// <summary>
        /// The start of the current hour.
        /// </summary>
        CurrentHour,

        /// <summary>
        /// The start of the current day.
        /// </summary>
        CurrentDay,

        /// <summary>
        /// The start of the current week.
        /// </summary>
        CurrentWeek,

        /// <summary>
        /// The start of the current month.
        /// </summary>
        CurrentMonth,

        /// <summary>
        /// The start of the current year.
        /// </summary>
        CurrentYear

    }

}
