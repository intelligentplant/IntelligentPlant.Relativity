using Microsoft.AspNetCore.Http;

namespace IntelligentPlant.Relativity.AspNetCore {

    /// <summary>
    /// <see cref="TimeZoneProvider"/> is used to supply the time zone to use when configuring 
    /// <see cref="RelativityParser.Current"/> for a request.
    /// </summary>
    public abstract class TimeZoneProvider {

        /// <summary>
        /// Creates a new <see cref="TimeZoneProvider"/> instance.
        /// </summary>
        internal TimeZoneProvider() { }


        /// <summary>
        /// Gets the time zone to use for the specified HTTP context.
        /// </summary>
        /// <param name="context">
        ///   The HTTP context.
        /// </param>
        /// <returns>
        ///   The time zone to use for the request.
        /// </returns>
        public abstract Task<TimeZoneInfo?> GetTimeZoneAsync(HttpContext context);

    }
}
