using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using CbrSoapClient;
using Domain.Interfaces;
using Infrastructure;
using Infrastructure.External.Services;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ICbrService _cbrService;
        private readonly ILogger<CbrService> _logger;

        public CurrencyService(ICbrService cbrService, ILogger<CurrencyService> logger)
        {
            _cbrService = cbrService ?? throw new ArgumentNullException(nameof(cbrService));
        }

        public async Task<bool> LoadAllCurrenciesForDays(int days)
        {
            return await _cbrService.GetAllRatesByDaysAsync(days);
        }
    }
}
