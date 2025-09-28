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

namespace Infrastructure.External.Services
{
    public class CbrService : ICbrService
    {
        private readonly DailyInfoSoapClient _soapClient;
        private readonly ILogger<CbrService> _logger;

        public CbrService()
        {
            _soapClient = new DailyInfoSoapClient(DailyInfoSoapClient.EndpointConfiguration.DailyInfoSoap);
        }

        public async Task<IEnumerable<CurrencyRate>> GetAllRatesByDaysAsync(int days)
        {
            var allRates = new List<CurrencyRate>();
            var startDate = DateTime.Today;
            var endDate = startDate.AddDays(-(days - 1));
            

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
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

            return allRates;
        }
        private async Task<IEnumerable<CurrencyRate>> GetRatesForDateAsync(DateTime date)
        {
            var response = await _soapClient.GetCursOnDateAsync(date);
            return ParseRates(response, date);
        }
        private IEnumerable<CurrencyRate> ParseRates(ArrayOfXElement response, DateTime date)
        {
            var rates = new List<CurrencyRate>();

            foreach (var element in response.Nodes)
            {
                try
                {
                    var rate = ParseCurrencyRate(element, date);
                    if (rate != null)
                        rates.Add(rate);
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
            var value = decimal.Parse(element.Element("Vcurs")?.Value ?? "0",
                CultureInfo.InvariantCulture);

            if (string.IsNullOrEmpty(charCode) || value <= 0)
                return null;

            return CurrencyRate.Create(charCode, name, nominal, value);
        }
    }
}
