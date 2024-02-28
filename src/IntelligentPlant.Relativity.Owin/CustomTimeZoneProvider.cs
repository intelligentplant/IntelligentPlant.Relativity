namespace IntelligentPlant.Relativity.Owin {

    /// <summary>
    /// Base class for custom time zone providers.
    /// </summary>
    public abstract class CustomTimeZoneProvider : TimeZoneProvider { 
    
        /// <summary>
        /// Creates a new <see cref="CustomTimeZoneProvider"/> instance.
        /// </summary>
        protected CustomTimeZoneProvider() : base() { }
    
    }

}
