using System;
using System.Threading.Tasks;

using Microsoft.Owin;

namespace IntelligentPlant.Relativity.Owin {

    /// <summary>
    /// <see cref="TimeZoneProvider"/> that reads the time zone from the query string of the request.
    /// </summary>
    public sealed class QueryStringTimeZoneProvider : TimeZoneProvider {

        /// <summary>
        /// The default query string key to use for the time zone.
        /// </summary>
        public const string DefaultQueryStringKey = "tz";

        /// <summary>
        /// The query string key to use for the time zone.
        /// </summary>
        public string QueryStringKey { get; }


        /// <summary>
        /// Creates a new <see cref="QueryStringTimeZoneProvider"/> instance.
        /// </summary>
        /// <param name="queryStringKey">
        ///   The query string key to use for the time zone. If <see langword="null"/> or white 
        ///   space, <see cref="DefaultQueryStringKey"/> will be used.
        /// </param>
        public QueryStringTimeZoneProvider(string? queryStringKey = null) {
            QueryStringKey = string.IsNullOrWhiteSpace(queryStringKey)
                ? DefaultQueryStringKey
                : queryStringKey!;
        }


        /// <inheritdoc/>
        public override Task<TimeZoneInfo?> GetTimeZoneAsync(IOwinContext context) {
            var tzRaw = context.Request.Query[QueryStringKey];
            if (!string.IsNullOrWhiteSpace(tzRaw)) {
                try {
                    var tz = TimeZoneInfo.FindSystemTimeZoneById(tzRaw!);
                    return Task.FromResult<TimeZoneInfo?>(tz);
                }
                catch { }
            }

            return Task.FromResult<TimeZoneInfo?>(null);
        }

    }
}
