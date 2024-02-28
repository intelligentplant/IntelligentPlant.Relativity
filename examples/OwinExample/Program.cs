using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using OwinExample;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) => {
        services.AddHostedService<WebServer>();
    });

var host = builder.Build();

host.Run();
