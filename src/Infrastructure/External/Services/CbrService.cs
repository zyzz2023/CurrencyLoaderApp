using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CbrSoapClient;
using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.External.Services
{
    public class CbrService : ICbrService
    {
        private readonly DailyInfoSoapClient _soapClient;
        
        public CbrService()
        {
            _soapClient = new DailyInfoSoapClient(DailyInfoSoapClient.EndpointConfiguration.DailyInfoSoap);
        }

        public async Task<IEnumerable<CurrencyRate>> GetAllRatesAsync()
        {
            try
            {

            }
            catch (Exception ex)
            {

            }
        }
    }
}
