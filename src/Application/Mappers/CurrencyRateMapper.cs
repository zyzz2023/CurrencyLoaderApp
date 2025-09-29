using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappers
{
    public static class CurrencyRateMapper
    {
        public static CurrencyRateDto ToDto(CurrencyRate rate)
        {
            return new CurrencyRateDto
            {
                CurrencyCode = rate.CurrencyCode,
                CurrencyName = rate.CurrencyName,
                Nominal = rate.Nominal,
                Value = rate.Value,
                Date = rate.Date
            };
        }
    }
}
