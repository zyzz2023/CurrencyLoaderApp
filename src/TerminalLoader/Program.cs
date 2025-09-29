using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Application.Services;
using Infrastructure;
using Infrastructure.External.Services;
using Application;
using Application.Interfaces;

namespace CurrencyLoaderApp.TerminalLoader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            try
            {
                await RunApplicationAsync(host);
            }
            catch (Exception ex)
            {
                var logger = host.Services.GetRequiredService<ILogger<Program>>();
                logger.LogCritical(ex, "Ошибка при запуске приложения");
                throw;
            }
            finally
            {
                host.Dispose();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    config.AddUserSecrets<Program>();
                    config.AddEnvironmentVariables();
                    var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddInfrastructure(context.Configuration);
                    services.AddApplication();
                    services.AddScoped<TerminalLoader>();
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                });
        }
        private static async Task RunApplicationAsync(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var app = scope.ServiceProvider.GetRequiredService<TerminalLoader>();
            await app.RunAsync();
        }
    }
}