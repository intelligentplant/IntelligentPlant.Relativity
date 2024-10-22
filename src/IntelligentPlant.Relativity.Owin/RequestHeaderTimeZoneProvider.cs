using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Microsoft.Owin;

namespace IntelligentPlant.Relativity.Owin {

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
        public RequestHeaderTimeZoneProvider(string? headerName = null) {
            HeaderName = string.IsNullOrWhiteSpace(headerName)
                ? DefaultHeaderName
                : headerName!;
        }


        /// <inheritdoc/>
        public override ValueTask<TimeZoneInfo?> GetTimeZoneAsync(IOwinContext context) {
            var tzHeaderValsRaw = context.Request.Headers.GetCommaSeparatedValues(HeaderName);
            if (tzHeaderValsRaw?.Count > 0) {
                var tzHeaderVals = new List<StringWithQualityHeaderValue>();

                foreach (var item in tzHeaderValsRaw) {
                    if (!StringWithQualityHeaderValue.TryParse(item, out var val) || string.IsNullOrWhiteSpace(val.Value)) {
                        continue;
                    }
                    tzHeaderVals.Add(val);
                    if (tzHeaderVals.Count >= 10) {
                        break;
                    }
                }

                foreach (var item in tzHeaderVals.OrderByDescending(x => x.Quality)) {
                    try {
                        var tz = GetTimeZoneById(item.Value!);
                        if (tz != null) {
                            return new ValueTask<TimeZoneInfo?>(tz);
                        }
                    }
                    catch { }
                }
            }

            return new ValueTask<TimeZoneInfo?>((TimeZoneInfo?) null);
        }

    }
}
