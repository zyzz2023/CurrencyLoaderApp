using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
            if (pageNumber < 1)
                return BadRequest(new { error = "Номер страницы должен быть больше 0" });

            if (pageSize < 1 || pageSize > 100)
                return BadRequest(new { error = "Размер страницы должен быть от 1 до 100" });

            if (!string.IsNullOrEmpty(currencyCode) && currencyCode.Length > 5)
                return BadRequest(new { error = "Код валюты не может быть длиннее 5 символов" });

            DateTime? parsedDate = null;
            if (!string.IsNullOrEmpty(onDate))
            {
                if (!DateTime.TryParse(onDate, out var tempDate))
                    return BadRequest(new { error = "Неверный формат даты" });

                parsedDate = DateTime.SpecifyKind(tempDate.Date, DateTimeKind.Utc);
            }

            try
            {
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
                return BadRequest(new { error = "Код валюты не может быть пустым" });

            if (code.Length > 5)
                return BadRequest(new { error = "Код валюты не может быть длиннее 5 символов" });

            try
            {
                var result = await _currencyService.GetRateByCodeAsync(code);

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
    }
}
