using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrencyController : ControllerBase
    {
        private readonly ILogger<CurrencyController> _logger;
        private readonly ICurrencyService _currencyService;

        public CurrencyController(ILogger<CurrencyController> logger, ICurrencyService currencyService)
        {
            _logger = logger;
            _currencyService = currencyService;
        }

        [HttpGet("currencies")]
        public async Task<ActionResult<PagedResponse<CurrencyRateDto>>> GetCurrencyExchangeRates(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? currencyCode = null,
            [FromQuery] string? onDate = null)
        {
            DateTime? parsedDate = null;

            try
            {
                if (!string.IsNullOrEmpty(onDate) && DateTime.TryParse(onDate, out var tempDate))
                {
                    parsedDate = DateTime.SpecifyKind(tempDate.Date, DateTimeKind.Utc);
                }
                var result = await _currencyService.GetExchangeRatesAsync(
                    pageNumber, pageSize, currencyCode, parsedDate);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения курсов валют");
                return StatusCode(500, new { error = "Внутренняя ошибка сервера" });
            }
        }

        [HttpGet("currency/{code}")]
        public async Task<ActionResult<CurrencyRateDto>> GetLatestCurrencyRateByCode([FromRoute] string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return BadRequest(new { error = "Код валюты не может быть пустым" });
            }

            try
            {
                var result = await _currencyService.GetRateByCodeAsync(code);

                if (result == null)
                {
                    return NotFound(new
                    {
                        error = $"Курс для валюты {code} не найден"
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения курса валюты");
                return StatusCode(500, new { error = "Внутренняя ошибка сервера" });
            }
        }

        [HttpGet("qrcbr")]
        [Produces("image/png")]
        public IActionResult GetQrCode()
        {
            try
            {
                var qrCodeBytes = _currencyService.GenerateQrCodeForCbr(DateTime.Today);
                return File(qrCodeBytes, "image/png");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка генерации QR-кода ЦБ РФ");
                return StatusCode(500, new { error = "Ошибка генерации QR-кода" });
            }
        }
        //[HttpGet(Name = "GetWeatherForecast")]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        //        TemperatureC = Random.Shared.Next(-20, 55),
        //        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}
    }
}
