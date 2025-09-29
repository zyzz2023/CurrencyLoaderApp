using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain.Interfaces
{
    public interface ICbrService
    {
        /// <summary>
        /// Getting currency exchange rates for n number of days starting from the current day
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        Task<bool> GetAllRatesByDaysAsync(int days);
    }
}
