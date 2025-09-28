using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CurrencyRate : IEquatable<CurrencyRate>
    {
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public int Nominal { get; set; }
        public decimal Value { get; set; }

        public string RequestDate { get; set; } = DateTime.UtcNow.ToShortDateString();

        public CurrencyRate Create(string currencyCode, string currencyName, int nominal, decimal value)
        {
            return new CurrencyRate
            {
                CurrencyCode = currencyCode,
                CurrencyName = currencyName,
                Nominal = nominal,
                Value = value
            };
        }

        public bool Equals(CurrencyRate? other)
        {
            if (other is null) return false;

            return (CurrencyCode == other.CurrencyCode) && (RequestDate == other.RequestDate)
                && (Value == other.Value);
        }
    }
}
