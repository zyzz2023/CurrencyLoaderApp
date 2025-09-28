using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ICurrencyRepository
    {
        Task AddRatesAsync(IEnumerable<CurrencyRate> currencyRates);
        Task<CurrencyRate?> GetByCodeAndDateAsync(string currencyCode, DateTime date);
        Task<CurrencyRate> GetByCodeAsync(string code);
        Task UpsertAsync(CurrencyRate rate);
        Task UpsertRangeAsync(IEnumerable<CurrencyRate> rates);
        Task<IEnumerable<CurrencyRate>> GetPagedAsync(int pageNumber, int pageSize, string? currencyCode, DateTime? onDate);
        Task<CurrencyRate?> GetLatestAsync(string currencyCode);
        //Task<CurrencyRate> GetQRCodeAsync(string code);
    }
}
