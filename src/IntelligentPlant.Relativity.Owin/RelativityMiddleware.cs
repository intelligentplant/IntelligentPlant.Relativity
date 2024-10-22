using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

using Microsoft.Owin;
using Microsoft.Owin.Logging;

using Owin;

namespace IntelligentPlant.Relativity.Owin {

    /// <summary>
    /// OWIN middleware for setting <see cref="RelativityParser.Current"/> on an HTTP request.
    /// </summary>
    internal sealed class RelativityMiddleware : OwinMiddleware {

        /// <summary>
        /// The parser factory.
        /// </summary>
        private readonly IRelativityParserFactory _factory;

        /// <summary>
        /// The time zone providers.
        /// </summary>
        private readonly IEnumerable<TimeZoneProvider> _timeZoneProviders;

        /// <summary>
        /// Logging.
        /// </summary>
        private readonly ILogger _logger;


        /// <summary>
        /// Creates a new <see cref="RelativityMiddleware"/> instance.
        /// </summary>
        /// <param name="next">
        ///   The next middleware in the pipeline.
        /// </param>
        /// <param name="app">
        ///   The application builder.
        /// </param>
        /// <param name="factory">
        ///   The parser factory.
        /// </param>
        /// <param name="timeZoneProviders">
        ///   The time zone providers.
        /// </param>
        public RelativityMiddleware(
            OwinMiddleware next, 
            IAppBuilder app,
            IRelativityParserFactory factory,
            IEnumerable<TimeZoneProvider> timeZoneProviders
        ) : base(next) {
            _logger = app.CreateLogger<RelativityMiddleware>();
            _factory = factory;
            _timeZoneProviders = timeZoneProviders ?? Array.Empty<TimeZoneProvider>();
        }


        /// <inheritdoc/>
        public override async Task Invoke(IOwinContext context) {
            TimeZoneInfo? tz = null;

            foreach (var provider in _timeZoneProviders) {
                tz = await provider.GetTimeZoneAsync(context).ConfigureAwait(false);
                if (tz != null) {
                    break;
                }
            }

            RelativityParser.Current = _factory.GetParser(CultureInfo.CurrentCulture, tz ?? TimeZoneInfo.Utc);

            if (_logger.IsEnabled(System.Diagnostics.TraceEventType.Verbose)) {
                _logger.WriteVerbose($"Relativity parser settings: Culture={RelativityParser.Current.CultureInfo.Name}, Time Zone={RelativityParser.Current.TimeZone.Id}");
            }

            await Next.Invoke(context).ConfigureAwait(false);
        }

    }
}
