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
        Task<bool> GetAllRatesByDaysAsync(int days);
    }
}
