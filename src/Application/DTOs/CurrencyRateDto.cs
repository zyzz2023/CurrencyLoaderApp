using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CurrencyRateDto
    {
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public int Nominal { get; set; }
        public decimal Value { get; set; }
        public DateTime Date { get; set; }
    }
    public class PagedResponse<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
