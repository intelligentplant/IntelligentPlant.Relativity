using System;
using System.Threading.Tasks;

using Microsoft.Owin;

namespace IntelligentPlant.Relativity.Owin {

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
        public abstract Task<TimeZoneInfo?> GetTimeZoneAsync(IOwinContext context);


        /// <summary>
        /// Gets a time zone by its IANA or Windows ID.
        /// </summary>
        /// <param name="timeZoneId">
        ///   The time zone ID.
        /// </param>
        /// <returns>
        ///   The time zone, or <see langword="null"/> if the time zone ID is not valid.
        /// </returns>
        /// <remarks>
        ///   This method uses <see cref="TimeZoneConverter.TZConvert"/> to resolve the time zone. 
        ///   Please refer to <a href="https://github.com/mattjohnsonpint/TimeZoneConverter"></a> for more 
        ///   information about how time zones are resolved.
        /// </remarks>
        protected TimeZoneInfo? GetTimeZoneById(string? timeZoneId) {
            if (string.IsNullOrWhiteSpace(timeZoneId)) {
                return null;
            }

            try {
                return TimeZoneConverter.TZConvert.GetTimeZoneInfo(timeZoneId!);
            }
            catch (TimeZoneNotFoundException) {
                return null;
            }
        }

    }
}
