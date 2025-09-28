using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public sealed class CurrencyRate : IEquatable<CurrencyRate>
    {
        public string CurrencyCode { get; private set; }
        public string CurrencyName { get; private set; }
        public int Nominal { get; private set; }
        public decimal Value { get; private set; }

        public string RequestDate { get; private set; } = DateTime.UtcNow.ToShortDateString();

        public static CurrencyRate Create(string currencyCode, string currencyName, int nominal, decimal value)
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
