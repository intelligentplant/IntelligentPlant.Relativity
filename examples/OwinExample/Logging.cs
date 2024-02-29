using System.Diagnostics;

using Microsoft.Owin.Logging;

namespace OwinExample {

    internal class OwinLoggerFactory : ILoggerFactory {

        private readonly Microsoft.Extensions.Logging.ILoggerFactory _factory;

        public OwinLoggerFactory(Microsoft.Extensions.Logging.ILoggerFactory factory) {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public ILogger Create(string name) {
            return new OwinLogger(_factory.CreateLogger(name));
        }

        private class OwinLogger : ILogger {

            private readonly Microsoft.Extensions.Logging.ILogger _logger;

            public OwinLogger(Microsoft.Extensions.Logging.ILogger logger) {
                _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
            }

            public bool WriteCore(TraceEventType eventType, int eventId, object state, Exception exception, Func<object, Exception, string> formatter) {
                bool TryGetMessage(out string message) {
                    message = formatter?.Invoke(state, exception)!;
                    return message != null;
                };

                string msg;

                switch (eventType) {
                    case TraceEventType.Critical:
                        if (!_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Critical)) {
                            return false;
                        }
                        if (TryGetMessage(out msg)) {
                            Microsoft.Extensions.Logging.LoggerExtensions.LogCritical(_logger, eventId, exception, msg);
                        }
                        break;
                    case TraceEventType.Error:
                        if (!_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Error)) {
                            return false;
                        }
                        if (TryGetMessage(out msg)) {
                            Microsoft.Extensions.Logging.LoggerExtensions.LogError(_logger, eventId, exception, msg);
                        }
                        break;
                    case TraceEventType.Warning:
                        if (!_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Warning)) {
                            return false;
                        }
                        if (TryGetMessage(out msg)) {
                            Microsoft.Extensions.Logging.LoggerExtensions.LogWarning(_logger, eventId, exception, msg);
                        }
                        break;
                    case TraceEventType.Information:
                        if (!_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information)) {
                            return false;
                        }
                        if (TryGetMessage(out msg)) {
                            Microsoft.Extensions.Logging.LoggerExtensions.LogInformation(_logger, eventId, exception, msg);
                        }
                        break;
                    case TraceEventType.Verbose:
                        if (!_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Trace)) {
                            return false;
                        }
                        if (TryGetMessage(out msg)) {
                            Microsoft.Extensions.Logging.LoggerExtensions.LogTrace(_logger, eventId, exception, msg);
                        }
                        break;
                    default:
                        if (!_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug)) {
                            return false;
                        }
                        if (TryGetMessage(out msg)) {
                            Microsoft.Extensions.Logging.LoggerExtensions.LogDebug(_logger, eventId, exception, msg);
                        }
                        break;
                }

                return true;
            }
        }

    }

}
