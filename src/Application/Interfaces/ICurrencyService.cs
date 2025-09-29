using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interfaces
{
    public interface ICurrencyService
    {
        Task<bool> LoadAllCurrenciesForDays(int days);
        Task<PagedResponse<CurrencyRateDto>> GetExchangeRatesAsync(int pageNumber = 1,int pageSize = 20,
            string? currencyCode = null,
            DateTime? onDate = null);
        Task<CurrencyRateDto> GetRateByCodeAsync(string code);
        public byte[] GenerateQrCodeForCbr(DateTime date);
    }
}
