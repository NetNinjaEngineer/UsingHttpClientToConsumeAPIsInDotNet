﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Movies.Client.Services;

namespace Movies.Client;

class Program
{
    static async Task Main(string[] args)
    {
        IHost host = CreateHostBuilder(args).Build();
        var serviceProvider = host.Services;
        try
        {
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Host created.");
            await serviceProvider.GetService<IIntegrationService>().Run();
        }
        catch (Exception generalException)
        {
            var logger = serviceProvider.GetService<ILogger<Program>>();
            logger.LogError(generalException,
                "An exception happened while running the integration service.");
        }

        Console.ReadKey();

        await host.RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args).ConfigureServices(
            (serviceCollection) => ConfigureServices(serviceCollection));
    }

    private static void ConfigureServices(IServiceCollection serviceCollection)
    {
        // add loggers           
        serviceCollection.AddLogging(configure => configure.AddDebug().AddConsole());

        // register the integration service on our container with a 
        // scoped lifetime

        // For the CRUD demos
        serviceCollection.AddScoped<IIntegrationService, CRUDService>();

        // For the partial update demos
        //serviceCollection.AddScoped<IIntegrationService, PartialUpdateService>();

        // For the stream demos
        serviceCollection.AddScoped<IIntegrationService, StreamService>();

        // For the cancellation demos
        // serviceCollection.AddScoped<IIntegrationService, CancellationService>();

        // For the HttpClientFactory demos
        // serviceCollection.AddScoped<IIntegrationService, HttpClientFactoryInstanceManagementService>();

        // For the dealing with errors and faults demos
        // serviceCollection.AddScoped<IIntegrationService, DealingWithErrorsAndFaultsService>();

        // For the custom http handlers demos
        // serviceCollection.AddScoped<IIntegrationService, HttpHandlersService>();     
    }
}
