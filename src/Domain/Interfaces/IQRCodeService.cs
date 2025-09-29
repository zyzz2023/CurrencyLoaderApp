using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IQrCodeService
    {
        byte[] GenerateQrCode(string url, int pixelsPerModule = 20);
    }
}
