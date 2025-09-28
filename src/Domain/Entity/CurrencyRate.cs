using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Domain.Entities
{
    public sealed class CurrencyRate : IEquatable<CurrencyRate>
    {
        public string CurrencyCode { get; set; }    
        public DateTime Date { get; set; }          
        public string CurrencyName { get; set; }
        public int Nominal { get; set; } = 1;
        public decimal Value { get; set; }

        public static CurrencyRate Create(string code, string name, int nominal, decimal value, DateTime date)
        {
            return new CurrencyRate
            {
                CurrencyCode = code,
                CurrencyName = name,
                Nominal = nominal,
                Value = value,
                Date = date
            };
        }

        public bool Equals(CurrencyRate? other)
        {
            if (other is null) return false;

            var dateOnly = DateTime.SpecifyKind(other.Date.Date, DateTimeKind.Utc);

            return CurrencyCode == other.CurrencyCode && Date.Date == other.Date.Date;
        }
    }
}
