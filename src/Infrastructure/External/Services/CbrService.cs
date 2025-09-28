using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using CbrSoapClient;
using System.Globalization;
using System.Xml.Linq;
using Infrastructure.Persistence.Repositories;
using System.Xml;

namespace Infrastructure.External.Services
{
    public class CbrService : ICbrService, IDisposable
    {
        private readonly DailyInfoSoapClient _soapClient;
        private readonly ILogger<CbrService> _logger;
        private readonly ICurrencyRepository _repository;

        public CbrService(ICurrencyRepository repository, ILogger<CbrService> logger)
        {
            _repository = repository;
            _logger = logger;
            _soapClient = new DailyInfoSoapClient(DailyInfoSoapClient.EndpointConfiguration.DailyInfoSoap);
        }
        public void Dispose()
        {
            _soapClient?.CloseAsync().Wait();
        }

        public async Task<bool> GetAllRatesByDaysAsync(int days)
        {
            var allRates = new List<CurrencyRate>();
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddDays(-(days - 1));

            _logger.LogInformation("Начало загрузки курсов за {Days} дней", days);

            for (var date = startDate; date >= endDate; date = date.AddDays(-1))
            {
                try
                {
                    var rates = await GetRatesForDateAsync(date);
                    allRates.AddRange(rates); 
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Не удалось получить курсы на {date}");
                }
            }

            try
            {
                await _repository.UpsertRangeAsync(allRates);
                //_logger.LogInformation($"Успешно сохранено {allRates.Count} курсов в БД");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка сохранения курсов в БД");
                return false;
            }

            // return allRates; // Потом это будет грузиться в базу данных
        }
        private async Task<IEnumerable<CurrencyRate>> GetRatesForDateAsync(DateTime date)
        {
            XmlNode response = await _soapClient.GetCursOnDateXMLAsync(date);
            if (response == null)
                return null;

            return ParseRates(response, date);
        }
        private IEnumerable<CurrencyRate> ParseRates(XmlNode response, DateTime date)
        {
            var rates = new List<CurrencyRate>();

            foreach(XmlNode element in response.ChildNodes)
            {
                try
                {
                    //var charCode = element.SelectSingleNode("CharCode")?.InnerText.Trim();
                    //var name = element.SelectSingleNode("Name")?.InnerText.Trim();
                    //var nominal = int.Parse(element.SelectSingleNode("Nominal")?.InnerText ?? "1");
                    //var valueStr = element.SelectSingleNode("Value")?.InnerText;
                    var charCode = element.SelectSingleNode("VchCode")?.InnerText.Trim()!;
                    var name = element.SelectSingleNode("Vname")?.InnerText.Trim()!;
                    var nominal = int.Parse(element.SelectSingleNode("Vnom")?.Value ?? "1");
                    var value = decimal.TryParse(element.SelectSingleNode("VunitRate")?
                                .InnerText!, CultureInfo.InvariantCulture, out decimal resRate) ? resRate : 0;

                    if (string.IsNullOrEmpty(charCode) || value <= 0)
                    {
                        continue;
                    }
                    
                    rates.Add(CurrencyRate.Create
                    (
                        charCode,
                        name ?? charCode,
                        nominal,
                        value,
                        date
                    ));
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Ошибка парсинга элемента валюты");
                }
            }

            return rates;
        }
        private CurrencyRate ParseCurrencyRate(XElement element, DateTime date)
        {
            var charCode = element.Element("VchCode")?.Value;
            var name = element.Element("Vname")?.Value;
            var nominal = int.Parse(element.Element("Vnom")?.Value ?? "1");
            var value = decimal.Parse(element.Element("Vcurs")?.Value ?? "0", CultureInfo.InvariantCulture);

            if (string.IsNullOrEmpty(charCode) || value <= 0)
                return null;

            return CurrencyRate.Create(charCode, name, nominal, value, date);
        }
    }
}
