using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Infrastructure.Persistence.Repositories
{
    public class CurrencyRateRepository : ICurrencyRepository
    {
        private readonly AppDbContext _context;

        public CurrencyRateRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddRatesAsync(IEnumerable<CurrencyRate> currencyRates)
        {
            await _context.AddRangeAsync(currencyRates);
        }
        public async Task<CurrencyRate?> GetByCodeAndDateAsync(string currencyCode, DateTime date)
        {
            var dateOnly = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);

            return await _context.CurrencyRates
                .FirstOrDefaultAsync(cr => cr.CurrencyCode == currencyCode && cr.Date.Date == dateOnly);
        }
        public async Task<CurrencyRate> GetByCodeAsync(string code)
        {
            var today = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);

            return await _context.CurrencyRates
                .FirstOrDefaultAsync(cr => cr.CurrencyCode == code && cr.Date.Date == today);
        }
        //public async Task<CurrencyRate> GetQRCodeAsync(string code)
        //{

        //}
        public async Task UpsertAsync(CurrencyRate rate)
        {
            var existing = await GetByCodeAndDateAsync(rate.CurrencyCode, rate.Date);

            if (existing != null)
            {
                existing.Value = rate.Value;
                existing.CurrencyName = rate.CurrencyName;
                existing.Nominal = rate.Nominal;
            }
            else
            {
                await _context.CurrencyRates.AddAsync(rate);
            }

            await _context.SaveChangesAsync();
        }
        public async Task UpsertRangeAsync(IEnumerable<CurrencyRate> rates)
        {
            foreach (var rate in rates)
            {
                var existing = await GetByCodeAndDateAsync(rate.CurrencyCode, rate.Date);

                if (existing != null)
                {
                    existing.Value = rate.Value;
                    existing.CurrencyName = rate.CurrencyName;
                    existing.Nominal = rate.Nominal;
                }
                else
                {
                    await _context.CurrencyRates.AddAsync(rate);
                }
            }

            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<CurrencyRate>> GetPagedAsync(int pageNumber, int pageSize, string? currencyCode, DateTime? onDate)
        {
            var query = _context.CurrencyRates.AsQueryable();

            if (!string.IsNullOrEmpty(currencyCode))
                query = query.Where(cr => cr.CurrencyCode == currencyCode);

            if (onDate.HasValue)
            {
                var utcDate = DateTime.SpecifyKind(onDate.Value.Date, DateTimeKind.Utc);
                query = query.Where(cr => cr.Date.Date == utcDate.Date);
            }

            return await query
                .OrderBy(cr => cr.CurrencyCode)
                .ThenByDescending(cr => cr.Date)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<int> GetCountAsync(string? currencyCode, DateTime? onDate)
        {
            var query = _context.CurrencyRates.AsQueryable();

            if (!string.IsNullOrEmpty(currencyCode))
                query = query.Where(cr => cr.CurrencyCode == currencyCode);

            if (onDate.HasValue)
            {
                var dateOnly = onDate.Value.Date;
                query = query.Where(cr => cr.Date.Date == dateOnly);
            }

            return await query.CountAsync();
        }
        public async Task<CurrencyRate?> GetLatestAsync(string currencyCode)
        {
            return await _context.CurrencyRates
                .Where(cr => cr.CurrencyCode == currencyCode)
                .OrderByDescending(cr => cr.Date)
                .FirstOrDefaultAsync();
        }
    }
}