using System;
using System.IO;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ProjectsNow.QR
{
    public class Encoder
    {
        public string SellerName { get; set; }
        public string VATNumber { get; set; }
        public DateTime? Date { get; set; }
        public string Total { get; set; }
        public string VATValue { get; set; }


        public bool IsReady
        {
            get
            {
                if (SellerName == null ||
                    VATNumber == null ||
                    Date == null ||
                    Total == null ||
                    VATValue == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public string Base64Code()
        {
            if (!IsReady)
            {
                return null;
            }

            string code1 = HexCode(1, SellerName);
            string code2 = HexCode(2, VATNumber);
            string code3 = HexCode(3, Date.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss"));
            string code4 = HexCode(4, Total);
            string code5 = HexCode(5, VATValue);

            string code = $"{code1}{code2}{code3}{code4}{code5}";

            return Convert.ToBase64String(HexStringToHex(code));
        }
        private string HexCode(int tag, string value)
        {
            int length = value.Length;

            string tagHex = tag.ToString("X2");
            string lengthHex = length.ToString("X2");

            byte[] valueByte = Encoding.Default.GetBytes(value);
            var valueHex = BitConverter.ToString(valueByte);
            valueHex = valueHex.Replace("-", "");
            valueHex = valueHex.ToLower();

            return $"{tagHex}{lengthHex}{valueHex}";
        }
        private byte[] HexStringToHex(string inputHex)
        {
            var resultantArray = new byte[inputHex.Length / 2];
            for (var i = 0; i < resultantArray.Length; i++)
            {
                resultantArray[i] = System.Convert.ToByte(inputHex.Substring(i * 2, 2), 16);
            }
            return resultantArray;
        }
        public ImageSource GetQRImage()
        {
            string code = Base64Code();
            QRCoder.QRCodeGenerator codeGenerator = new();
            QRCoder.QRCodeData codeData = codeGenerator.CreateQrCode(code, QRCoder.QRCodeGenerator.ECCLevel.Q);
            QRCoder.QRCode QRCode = new(codeData);

            System.Drawing.Bitmap image = QRCode.GetGraphic(20);

            using MemoryStream memory = new();
            image.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
            memory.Position = 0;
            BitmapImage bitmapimage = new();
            bitmapimage.BeginInit();
            bitmapimage.StreamSource = memory;
            bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapimage.EndInit();

            return bitmapimage;
        }
    }
}
