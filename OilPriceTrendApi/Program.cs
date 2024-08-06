using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using OilPriceTrendApi.Extensions;
using OilPriceTrendApi.Models;
using OilPriceTrendApi.Models.Validators;
using OilPriceTrendApi.Services;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    logger.Info("Initializing application.");

    var builder = WebApplication.CreateBuilder(args);

    // Configure logging
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Logging.AddConsole();
    builder.Host.UseNLog();

    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddFluentValidationClientsideAdapters();

    // Configure Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "OilPriceTrend API", Version = "v1" });
    });

    var configuration = builder.Configuration;

    string oilPriceConfig_ClientName = configuration["OilPriceTrendsConfig:HttpClientName"];
    string oilPriceConfig_Url = configuration["OilPriceTrendsConfig:Url"];

    // Validate configuration values
    if (string.IsNullOrWhiteSpace(oilPriceConfig_ClientName))
    {
        throw new ArgumentException("OilPriceTrendsConfig:HttpClientName cannot be null or whitespace.");
    }

    if (string.IsNullOrWhiteSpace(oilPriceConfig_Url))
    {
        throw new ArgumentException("OilPriceTrendsConfig:Url cannot be null or whitespace.");
    }

    builder.Services.AddHttpClient(oilPriceConfig_ClientName, client =>
    {
        client.BaseAddress = new Uri(oilPriceConfig_Url);
    });

    builder.Services.AddMemoryCache();

    // Register services
    builder.Services.AddTransient<IOilPriceService, OilPriceService>();

    var cacheDuration = configuration.GetValue<int>("OilPriceTrendsConfig:CacheDurationInMilliseconds");
    if (cacheDuration > 0)
    {
        builder.Services.AddDecorator<IOilPriceService, OilPriceServiceDecoratorCache>();
    }

    // Register validators
    builder.Services.AddTransient<IValidator<GetOilPriceTrendParams>, GetOilPriceTrendParamsValidator>();

    var app = builder.Build();

    if (cacheDuration > 0)
    {
        // Preload data into cache if cache duration is set
        logger.Info("Preloading data into cache.");
        var oilPriceService = app.Services.GetRequiredService<IOilPriceService>();
        await oilPriceService.GetOilPriceDataAsync(DateTime.Today.AddYears(-200), DateTime.Today.AddYears(1)).ConfigureAwait(false);
    }

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "OilPriceTrend API V1");
        });
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    try
    {
        logger.Info("Starting application.");

        await app.RunAsync().ConfigureAwait(false);
    }
    catch (Exception ex)
    {
        logger.Error(ex, "An error occurred during application execution.");
        throw;
    }
    finally
    {
        logger.Info("Shutting down application.");
        LogManager.Shutdown();
    }
}
catch (Exception exception)
{
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}
