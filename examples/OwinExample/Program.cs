using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using OwinExample;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging => logging.ClearProviders().AddConsole().SetMinimumLevel(LogLevel.Trace))
    .ConfigureServices((context, services) => {
        services.AddHostedService<WebServer>();
    });

var host = builder.Build();

host.Run();
