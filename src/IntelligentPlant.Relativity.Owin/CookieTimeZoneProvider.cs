using System;
using System.Threading.Tasks;

using Microsoft.Owin;

namespace IntelligentPlant.Relativity.Owin {

    /// <summary>
    /// <see cref="TimeZoneProvider"/> implementation that reads the time zone from a cookie.
    /// </summary>
    public sealed class CookieTimeZoneProvider : TimeZoneProvider {

        /// <summary>
        /// The default name for the time zone cookie.
        /// </summary>
        public const string DefaultCookieName = ".Relativity.TimeZone";

        /// <summary>
        /// The name of the cookie to read the time zone from.
        /// </summary>
        public string CookieName { get; }


        /// <summary>
        /// Creates a new <see cref="CookieTimeZoneProvider"/> instance.
        /// </summary>
        /// <param name="cookieName">
        ///   The name of the cookie to read the time zone from. If <see langword="null"/> or white 
        ///   space, <see cref="DefaultCookieName"/> will be used.
        /// </param>
        public CookieTimeZoneProvider(string? cookieName = null) {
            CookieName = string.IsNullOrWhiteSpace(cookieName)
                ? DefaultCookieName
                : cookieName!;
        }


        /// <inheritdoc/>
        public override Task<TimeZoneInfo?> GetTimeZoneAsync(IOwinContext context) {
            return Task.FromResult(GetTimeZoneById(context.Request.Cookies[CookieName]));
        }


        /// <summary>
        /// Gets the value to use for the time zone cookie.
        /// </summary>
        /// <param name="timeZone">
        ///   The time zone.
        /// </param>
        /// <returns>
        ///   The value to use for the time zone cookie.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="timeZone"/> is <see langword="null"/>.
        /// </exception>
        public static string GetCookieValue(TimeZoneInfo timeZone) {
            if (timeZone == null) {
                throw new ArgumentNullException(nameof(timeZone));
            }

            return timeZone.Id;
        }

    }
}
