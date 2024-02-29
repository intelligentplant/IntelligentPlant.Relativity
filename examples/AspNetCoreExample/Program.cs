using IntelligentPlant.Relativity;
using IntelligentPlant.Relativity.AspNetCore;
using IntelligentPlant.Relativity.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRequestLocalization(options => { 
    var supportedCultures = new[] { "en-GB", "fi-FI" };
    options.SetDefaultCulture(supportedCultures[0])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
});

builder.Services.AddRelativity()
    .AddQueryStringTimeZoneProvider()
    .AddRequestHeaderTimeZoneProvider();

var app = builder.Build();

app.UseRequestLocalization();
app.UseRelativity();

app.MapGet("/", (string? timestamp) => { 
    if (string.IsNullOrWhiteSpace(timestamp)) {
        return Results.BadRequest("The 'timestamp' query string parameter is required.");
    }

    if (!RelativityParser.Current.TryConvertToUtcDateTime(timestamp, out var utcDateTime)) {
        return Results.BadRequest("The 'timestamp' query string parameter is not a valid date/time.");
    }

    return Results.Ok(utcDateTime);
});

app.Run();
