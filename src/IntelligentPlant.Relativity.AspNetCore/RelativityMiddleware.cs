using System.Globalization;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IntelligentPlant.Relativity.AspNetCore {

    /// <summary>
    /// Middleware for setting <see cref="RelativityParser.Current"/> based on the culture and 
    /// time zone for the current request.
    /// </summary>
    internal partial class RelativityMiddleware {

        /// <summary>
        /// Logging.
        /// </summary>
        private readonly ILogger<RelativityMiddleware> _logger;

        /// <summary>
        /// The next middleware in the pipeline.
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// The parser factory.
        /// </summary>
        private readonly IRelativityParserFactory _parserFactory;


        /// <summary>
        /// Creates a new <see cref="RelativityMiddleware"/> instance.
        /// </summary>
        /// <param name="next">
        ///   The next middleware in the pipeline.
        /// </param>
        /// <param name="parserFactory">
        ///   The parser factory.
        /// </param>
        /// <param name="logger">
        ///   The logger.
        /// </param>
        public RelativityMiddleware(RequestDelegate next, IRelativityParserFactory parserFactory, ILogger<RelativityMiddleware> logger) {
            _next = next;
            _parserFactory = parserFactory;
            _logger = logger;
        }


        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">
        ///   The current HTTP context.
        /// </param>
        /// <returns>
        ///   A <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        public async Task InvokeAsync(HttpContext context) {
            TimeZoneInfo? tz = null;

            foreach (var provider in GetTimeZoneProviders(context)) {
                tz = await provider.GetTimeZoneAsync(context);
                if (tz != null) {
                    break;
                }
            }

            if (tz == null) {
                tz = TimeZoneInfo.Utc;
            }

            context.SetRequestTimeZone(tz);
            RelativityParser.Current = _parserFactory.GetParser(CultureInfo.CurrentCulture, tz);

            LogRelativityParserSet(RelativityParser.Current.CultureInfo.Name, RelativityParser.Current.TimeZone.Id);
            
            await _next(context);
        }


        /// <summary>
        /// Gets the time zone providers to use for the current request.
        /// </summary>
        /// <param name="context">
        ///   The current HTTP context.
        /// </param>
        /// <returns>
        ///   The time zone providers to use for the current request.
        /// </returns>
        private static IEnumerable<TimeZoneProvider> GetTimeZoneProviders(HttpContext context) {
            // Custom providers first, in registration order.
            foreach (var item in context.RequestServices.GetServices<CustomTimeZoneProvider>()) {
                yield return item;
            }

            TimeZoneProvider? provider;

            provider = context.RequestServices.GetService<QueryStringTimeZoneProvider>();
            if (provider != null) {
                yield return provider;
            }

            provider = context.RequestServices.GetService<CookieTimeZoneProvider>();
            if (provider != null) {
                yield return provider;
            }

            provider = context.RequestServices.GetService<RequestHeaderTimeZoneProvider>();
            if (provider != null) {
                yield return provider;
            }

            provider = context.RequestServices.GetService<UserClaimTimeZoneProvider>();
            if (provider != null) {
                yield return provider;
            }
        }


        [LoggerMessage(1, LogLevel.Debug, "Relativity parser settings: Culture={culture}, Time Zone={timeZone}")]
        partial void LogRelativityParserSet(string culture, string timeZone);

    }
}
