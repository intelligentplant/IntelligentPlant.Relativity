using Microsoft.AspNetCore.Http;

namespace IntelligentPlant.Relativity.AspNetCore {

    /// <summary>
    /// Extensions for <see cref="HttpContext"/>.
    /// </summary>
    public static class HttpContextExtensions {

        /// <summary>
        /// Sets the time zone to use for parsing relative timestamps in the current request.
        /// </summary>
        /// <param name="context">
        ///   The HTTP context.
        /// </param>
        /// <param name="timeZone">
        ///   The time zone to use.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="context"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="timeZone"/> is <see langword="null"/>.
        /// </exception>
        public static void SetRequestTimeZone(this HttpContext context, TimeZoneInfo timeZone) {
            if (context == null) {
                throw new ArgumentNullException(nameof(context));
            }
            if (timeZone == null) {
                throw new ArgumentNullException(nameof(timeZone));
            }

            context.Items["IntelligentPlant.Relativity.TimeZone"] = timeZone;
        }


        /// <summary>
        /// Gets the time zone to use for parsing relative timestamps in the current request.
        /// </summary>
        /// <param name="context">
        ///   The HTTP context.
        /// </param>
        /// <returns>
        ///   The time zone to use, or <see langword="null"/> if no time zone has been set for the request.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="context"/> is <see langword="null"/>.
        /// </exception>
        public static TimeZoneInfo? GetRequestTimeZone(this HttpContext context) {
            if (context == null) {
                throw new ArgumentNullException(nameof(context));
            }

            return context.Items.TryGetValue("IntelligentPlant.Relativity.TimeZone", out var timeZone) 
                ? timeZone as TimeZoneInfo
                : null;
        }

    }
}
