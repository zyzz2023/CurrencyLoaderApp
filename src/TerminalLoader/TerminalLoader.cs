using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyLoaderApp.TerminalLoader
{
    public class TerminalLoader
    {
        private readonly ICurrencyService _currencyService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TerminalLoader> _logger;

        public TerminalLoader(ICurrencyService currencyService, IConfiguration configuration,
            ILogger<TerminalLoader> logger)
        {
            _currencyService = currencyService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            _logger.LogInformation("=======Загрузчик курсов валют ЦБ РФ======");

            try
            {
                var daysToLoad = _configuration.GetValue<int>("DaysToLoad");
                _logger.LogInformation($"Загрузка курсов за {daysToLoad} дней...");

                var success = await _currencyService.LoadAllCurrenciesForDays(daysToLoad);

                if (success)
                {
                    _logger.LogInformation("Загрузка успешно завершена");
                }
                else
                {
                    _logger.LogError("Загрузка завершилась с ошибками");
                    Environment.ExitCode = 1;
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Ошибка при выполнении");
                Environment.ExitCode = 1;
            }
        }
    }
}
