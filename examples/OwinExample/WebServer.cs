using IntelligentPlant.Relativity;
using IntelligentPlant.Relativity.Owin;

using Microsoft.Extensions.Hosting;
using Microsoft.Owin.Logging;

using Owin;

namespace OwinExample {
    internal sealed class WebServer : BackgroundService {

        private readonly Microsoft.Extensions.Logging.ILoggerFactory _loggerFactory;


        public WebServer(Microsoft.Extensions.Logging.ILoggerFactory loggerFactory) {
            _loggerFactory = loggerFactory;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            var webHostStartOptions = new Microsoft.Owin.Hosting.StartOptions();

            using (Microsoft.Owin.Hosting.WebApp.Start(webHostStartOptions, app => ConfigureWebHost(app))) {
                try {
                    // Wait until shutdown.
                    await Task.Delay(Timeout.Infinite, stoppingToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException) {
                    if (!stoppingToken.IsCancellationRequested) {
                        throw;
                    }
                }
            }
        }


        private void ConfigureWebHost(IAppBuilder app) {
            app.SetLoggerFactory(new OwinLoggerFactory(_loggerFactory));

            var factory = new RelativityParserFactory();
            app.UseRelativity(factory, new QueryStringTimeZoneProvider(), new RequestHeaderTimeZoneProvider());

            app.Use((context, _) => {
                var timestamp = context.Request.Query["timestamp"];
                if (string.IsNullOrWhiteSpace(timestamp)) {
                    context.Response.StatusCode = 400;
                    context.Response.Write("The 'timestamp' query string parameter is required.");
                    return Task.CompletedTask;
                }

                if (!RelativityParser.Current.TryConvertToUtcDateTime(timestamp, out var utcDateTime)) {
                    context.Response.StatusCode = 400;
                    context.Response.Write("The 'timestamp' query string parameter is not a valid date/time.");
                    return Task.CompletedTask;
                }

                context.Response.ContentType = "application/json";
                context.Response.Write(System.Text.Json.JsonSerializer.Serialize(utcDateTime));
                return Task.CompletedTask;
            });
        }

    }
}
