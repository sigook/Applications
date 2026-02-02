using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace SigookFunctions.Services
{
    public static class QrCodeGenerator
    {
        public static byte[] GenerateByteArray(string text)
        {
            Bitmap image = GenerateImage(text);
            return ImageToByte(image);
        }

        public static Bitmap GenerateImage(string text)
        {
            var generator = new QRCodeGenerator();
            QRCodeData qrCodeData = generator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(10);
            return qrCodeImage;
        }

        private static byte[] ImageToByte(Image image)
        {
            using (var stream = new MemoryStream())
            {
                image.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }
    }
}