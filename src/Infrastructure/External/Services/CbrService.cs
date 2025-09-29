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

            _logger.LogInformation($"Начало загрузки курсов за {days} дней");

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
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка сохранения курсов в БД");
                return false;
            }
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
                    var charCode = element.SelectSingleNode("VchCode")?.InnerText.Trim()!;
                    var name = element.SelectSingleNode("Vname")?.InnerText.Trim()!;
                    var nominal = int.Parse(element.SelectSingleNode("Vnom")?.Value ?? "1");
                    var value = decimal.TryParse(element.SelectSingleNode("VunitRate")?
                                .InnerText!, CultureInfo.InvariantCulture, out decimal resRate) ? resRate : 0;

                    if (string.IsNullOrEmpty(charCode) || value <= 0)
                    {
                        continue;
                    }

                    var utcDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);

                    rates.Add(CurrencyRate.Create
                    (
                        charCode,
                        name ?? charCode,
                        nominal,
                        value,
                        utcDate
                    ));
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Ошибка парсинга элемента валюты");
                }
            }

            return rates;
        }
    }
}
