using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace IntelligentPlant.Relativity.AspNetCore {

    /// <summary>
    /// <see cref="TimeZoneProvider"/> that reads the time zone from a request header.
    /// </summary>
    /// <remarks>
    ///   The time zone header can be specified as a comma-separated list of time zone IDs, with 
    ///   an associated quality value. The time zone with the highest quality value that can be resolved will be used.
    /// </remarks>
    public sealed class RequestHeaderTimeZoneProvider : TimeZoneProvider {

        /// <summary>
        /// The default name of the time zone header.
        /// </summary>
        public const string DefaultHeaderName = "X-TimeZone";

        /// <summary>
        /// The name of the header to read the time zone from.
        /// </summary>
        public string HeaderName { get; }


        /// <summary>
        /// Creates a new <see cref="RequestHeaderTimeZoneProvider"/> instance.
        /// </summary>
        /// <param name="headerName">
        ///   The name of the header to read the time zone from. If <see langword="null"/> or white 
        ///   space, <see cref="DefaultHeaderName"/> will be used.
        /// </param>
        internal RequestHeaderTimeZoneProvider(string? headerName) {
            HeaderName = string.IsNullOrWhiteSpace(headerName)
                ? DefaultHeaderName
                : headerName;
        }


        /// <inheritdoc/>
        public override Task<TimeZoneInfo?> GetTimeZoneAsync(HttpContext context) {
            var tzHeaderValsRaw = context.Request.Headers.GetCommaSeparatedValues(HeaderName);
            if (tzHeaderValsRaw.Length > 0 && StringWithQualityHeaderValue.TryParseList(tzHeaderValsRaw, out var tzHeaderVals)) {
                foreach (var item in tzHeaderVals.OrderByDescending(x => x.Quality)) {
                    if (item.Value.Value == null) {
                        continue;
                    }
                    if (TimeZoneInfo.TryFindSystemTimeZoneById(item.Value.Value, out var tz)) {
                        return Task.FromResult<TimeZoneInfo?>(tz);
                    }
                }
            }


            return Task.FromResult<TimeZoneInfo?>(null);
        }

    }
}
