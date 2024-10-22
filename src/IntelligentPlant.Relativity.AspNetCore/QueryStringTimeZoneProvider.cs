using Microsoft.AspNetCore.Http;

namespace IntelligentPlant.Relativity.AspNetCore {

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
        internal QueryStringTimeZoneProvider(string? queryStringKey) {
            QueryStringKey = string.IsNullOrWhiteSpace(queryStringKey)
                ? DefaultQueryStringKey
                : queryStringKey;
        }


        /// <inheritdoc/>
        public override ValueTask<TimeZoneInfo?> GetTimeZoneAsync(HttpContext context) {
            if (context.Request.Query.TryGetValue(QueryStringKey, out var tzQs) && !string.IsNullOrWhiteSpace(tzQs) && TimeZoneInfo.TryFindSystemTimeZoneById(tzQs!, out var tz)) {
                return new ValueTask<TimeZoneInfo?>(tz);
            }

            return new ValueTask<TimeZoneInfo?>((TimeZoneInfo?) null);
        }

    }
}
