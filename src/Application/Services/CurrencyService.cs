using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Application.Mappers;
using CbrSoapClient;
using Domain.Interfaces;
using Infrastructure;
using Infrastructure.External.Services;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ICbrService _cbrService;
        private readonly IQrCodeService _qrCodeService;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly ILogger<CurrencyService> _logger;

        public CurrencyService(
            ICbrService cbrService, 
            ILogger<CurrencyService> logger, 
            ICurrencyRepository currencyRepository, 
            IQrCodeService qrCodeService)
        {
            _cbrService = cbrService;
            _qrCodeService = qrCodeService;
            _currencyRepository = currencyRepository;
            _logger = logger;
        }

        public async Task<PagedResponse<CurrencyRateDto>> GetExchangeRatesAsync(
            int pageNumber = 1,
            int pageSize = 20,
            string? currencyCode = null,
            DateTime? onDate = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            try
            {
                var rates = await _currencyRepository.GetPagedAsync(pageNumber, pageSize, currencyCode, onDate);
                var totalCount = await _currencyRepository.GetCountAsync(currencyCode, onDate);

                var items = rates.Select(CurrencyRateMapper.ToDto).ToList();

                return new PagedResponse<CurrencyRateDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка курсов валют");
                throw;
            }
        }
        public async Task<CurrencyRateDto> GetRateByCodeAsync(string code)
        {
            try
            {
                var result = await _currencyRepository.GetLatestAsync(code);
                if (result == null)
                {
                    _logger.LogWarning($"Курс для валюты {code} не найден");
                    return null;
                }
                return CurrencyRateMapper.ToDto(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка курсов валют");
                throw;
            }
        }
        public async Task<bool> LoadAllCurrenciesForDays(int days)
        {
            return await _cbrService.GetAllRatesByDaysAsync(days);
        }
        public byte[] GenerateQrCodeForCbr(DateTime date)
        {
            var url = $"https://www.cbr.ru/scripts/XML_daily.asp?date_req={date:dd/MM/yyyy}";
            _logger.LogInformation("Генерация QR-кода для ЦБ РФ на дату: {Date}", date.ToString("dd.MM.yyyy"));

            return _qrCodeService.GenerateQrCode(url, 15);
        }
    }
}
