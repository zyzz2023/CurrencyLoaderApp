// CurrencyApp.Console/Program.cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Application.Services;
using Infrastructure;
using Infrastructure.External.Services;
using Application;
using Application.Interfaces;

namespace CurrencyApp.Console
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
                logger.LogCritical(ex, "Неожиданная ошибка при запуске приложения");
                throw;
            }
            finally
            {
                host.Dispose();
            }
        //}

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddInfrastructure(context.Configuration);
                    services.AddApplication();
                    services.AddScoped<ConsoleApplication>();
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
            var app = scope.ServiceProvider.GetRequiredService<ConsoleApplication>();
            await app.RunAsync();
        }
    }

    public class ConsoleApplication
    {
        private readonly ICurrencyService _currencyService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ConsoleApplication> _logger;

        public ConsoleApplication(
            ICurrencyService currencyService,
            IConfiguration configuration,
            ILogger<ConsoleApplication> logger)
        {
            _currencyService = currencyService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            _logger.LogInformation("=== Загрузчик курсов валют ЦБ РФ ===");

            try
            {
                var daysToLoad = _configuration.GetValue<int>("DaysToLoad");
                _logger.LogInformation("Загрузка курсов за {Days} дней...", daysToLoad);

                var success = await _currencyService.LoadAllCurrenciesForDays(daysToLoad);

                if (success)
                {
                    _logger.LogInformation("✅ Загрузка успешно завершена!");
                }
                else
                {
                    _logger.LogError("❌ Загрузка завершилась с ошибками");
                    Environment.ExitCode = 1;
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "💥 Критическая ошибка при выполнении");
                Environment.ExitCode = 1;
            }

            _logger.LogInformation("Нажмите любую клавишу для выхода...");
        }
    }
}