using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using QRCoder;

namespace Infrastructure.External.Services
{
    public class QrCodeService : IQrCodeService
    {
        private readonly ILogger<QrCodeService> _logger;

        public QrCodeService(ILogger<QrCodeService> logger)
        {
            _logger = logger;
        }

        public byte[] GenerateQrCode(string url, int pixelsPerModule = 20)
        {
            try
            {
                using var qrGenerator = new QRCodeGenerator();
                using var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                using var qrCode = new PngByteQRCode(qrCodeData);

                var qrCodeBytes = qrCode.GetGraphic(pixelsPerModule);

                _logger.LogInformation($"QR-код успешно сгенерирован для URL: {url}");
                return qrCodeBytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка генерации QR-кода для URL: {url}");
                throw;
            }
        }
    }
}
