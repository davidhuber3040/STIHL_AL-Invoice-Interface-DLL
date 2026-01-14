using AL_Invoice_Interface_DLL;
using AL_Invoice_Interface_DLL.financeService;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using ZXing.QrCode;

namespace Stihl.Albania.Fiscalization
{
   
    public static class Global
    {
        // --------------------------------------------------------------------
        // Konfiguration: Helper-Service (http://80.90.92.219:2014/...)
        // --------------------------------------------------------------------

        /// <summary>
        /// Basis-URL des Helper-Webservice, z.B. "http://80.90.92.219:2014".
        /// </summary>
        public static string HelperBaseUrl = string.Empty;
        public static string GenerateIICAndIICSignatureUrl = string.Empty;
        public static string SignXmlRequestUrl = string.Empty;
        public static string SignUblUrl = string.Empty;
        public static string FiscalizationServiceUrl = string.Empty;
        public static string EInvoiceServiceUrl = string.Empty;
        public static string iic = "";
        public static string iicSignature = "";
        public static string CertificatePassword = "";
        public static string CertificatePath = "";

        public static void _01SetBasicData( string certificatePassword, string certificatePath,string generateIICAndIICSignatureUrl,string signXmlRequestUrl,string signUblUrl, string fiscalizationServiceUrl,string eInvoiceServiceUrl)
        {

            GenerateIICAndIICSignatureUrl= generateIICAndIICSignatureUrl;
            SignXmlRequestUrl=signXmlRequestUrl;
            SignUblUrl=signUblUrl;  
            FiscalizationServiceUrl=fiscalizationServiceUrl;
            EInvoiceServiceUrl=eInvoiceServiceUrl;
            CertificatePassword = certificatePassword;  
            CertificatePath = certificatePath;
 


            // TLS-Einstellungen – für HTTPS nach außen
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            // Optional: alle Zertifikate akzeptieren (wie oft in NAV-Integrationen üblich).
            // Falls ihr produktiv eine saubere PKI habt, kannst du das aus Sicherheitsgründen entfernen.
            ServicePointManager.ServerCertificateValidationCallback = AcceptAllCertifications;
        }


        public static void _02GenerateIICAndIICSignature( string iicInput, string requestFilePathToSave, string responseFilePathToSave)
        {
            if (string.IsNullOrWhiteSpace(GenerateIICAndIICSignatureUrl))
                throw new InvalidOperationException("Global.GenerateIICAndIICSignatureUrl ist nicht gesetzt. Bitte zuerst _01SetBasicData aufrufen.");

            var req = new IicRequest { certPass = Global.CertificatePassword, certContent = FileToBase64(Global.CertificatePath), iicInput = iicInput };
            string jsonRequest = JsonSerializer.Serialize(req);

            // Request optional speichern
            if (!string.IsNullOrEmpty(requestFilePathToSave))
                File.WriteAllText(requestFilePathToSave, jsonRequest, Encoding.ASCII);

            string rawResponse = "";
            HttpPostJson(GenerateIICAndIICSignatureUrl, jsonRequest, ref rawResponse);

            // Response optional speichern
            if (!string.IsNullOrEmpty(responseFilePathToSave))
                File.WriteAllText(responseFilePathToSave, rawResponse, Encoding.ASCII);

            var resp = JsonSerializer.Deserialize<IicResponse>(rawResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (resp == null || string.IsNullOrWhiteSpace(resp.IIC) || string.IsNullOrWhiteSpace(resp.IICSignature))
                throw new InvalidOperationException("IIC oder IICSignature konnte aus der Antwort nicht gelesen werden. Response: " + rawResponse);

            iic = resp.IIC;
            iicSignature = resp.IICSignature;
        }

        public static string _03GetIIC()
        {
            return (iic);
        }

        public static string _04GetIICSignature()
        {
            return (iicSignature);
        }

        public static void _05CreateQRCode(string Filepath, int width, int height, string QRCode)
        {


            QrCodeEncodingOptions options = new QrCodeEncodingOptions();
            options.PureBarcode = true;
            options.Height = height;
            options.Width = width;




            var qr = new ZXing.BarcodeWriter();
            qr.Options = options;
            qr.Format = ZXing.BarcodeFormat.QR_CODE;
            var result = new Bitmap(qr.Write(QRCode));
            result.Save(Filepath, System.Drawing.Imaging.ImageFormat.Bmp);

        }

        private sealed class IicRequest
        {
            public string certPass { get; set; }
            public string certContent { get; set; }
            public string iicInput { get; set; }
        }

        private sealed class IicResponse
        {
            public string IIC { get; set; }
            public string IICSignature { get; set; }
        }



        // --------------------------------------------------------------------
        // HTTP-Helper
        // --------------------------------------------------------------------

        /// <summary>
        /// Führt einen HTTP POST mit JSON-Body aus und gibt den Response-Text zurück.
        /// Der komplette Response wird zusätzlich in rawResponse geschrieben
        /// (z.B. für Logging in NAV).
        /// </summary>
        public static  void HttpPostJson(string url, string jsonRequest, ref string rawResponse)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("URL must not be empty.", nameof(url));

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";

            if (!string.IsNullOrEmpty(jsonRequest))
            {
                byte[] bytes = Encoding.ASCII.GetBytes(jsonRequest);
                request.ContentLength = bytes.Length;
                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(bytes, 0, bytes.Length);
                }
            }

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var respStream = response.GetResponseStream())
                using (var reader = new StreamReader(respStream ?? Stream.Null, Encoding.ASCII))
                {
                    rawResponse = reader.ReadToEnd();
                   // return rawResponse;
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (var resp = (HttpWebResponse)ex.Response)
                    using (var respStream = resp.GetResponseStream())
                    using (var reader = new StreamReader(respStream ?? Stream.Null, Encoding.ASCII))
                    {
                        rawResponse = reader.ReadToEnd();
                    }
                }
                else
                {
                    rawResponse = ex.ToString();
                }

                // Fehler nach NAV weiterreichen – das erleichtert das Debugging.
               
            }
        }

        /// <summary>
        /// HTTP POST mit XML (z.B. für direkte SOAP-Calls).
        /// soapAction kann leer sein; falls nicht leer, wird ein SOAPAction-Header gesetzt.
        /// </summary>
        public static void HttpPostXml(string url, string xmlPayload, string soapAction, ref string rawResponse)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("URL must not be empty.", nameof(url));

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "text/xml; charset=utf-8";

            if (!string.IsNullOrWhiteSpace(soapAction))
                request.Headers.Add("SOAPAction", "\"" + soapAction + "\"");

            byte[] bytes = Encoding.ASCII.GetBytes(xmlPayload ?? string.Empty);
            request.ContentLength = bytes.Length;

            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(bytes, 0, bytes.Length);
            }

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var respStream = response.GetResponseStream())
                using (var reader = new StreamReader(respStream ?? Stream.Null, Encoding.ASCII))
                {
                    rawResponse = reader.ReadToEnd();
                  
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (var resp = (HttpWebResponse)ex.Response)
                    using (var respStream = resp.GetResponseStream())
                    using (var reader = new StreamReader(respStream ?? Stream.Null, Encoding.ASCII))
                    {
                        rawResponse = reader.ReadToEnd();
                    }
                }
                else
                {
                    rawResponse = ex.ToString();
                }

             
            }
        }

        // --------------------------------------------------------------------
        // Base64-Helper (oft praktisch direkt aus NAV heraus)
        // --------------------------------------------------------------------

        public static string EncodeBase64(string plainText)
        {
            if (plainText == null)
                return null;

            byte[] bytes = Encoding.ASCII.GetBytes(plainText);
            return Convert.ToBase64String(bytes);
        }

        public static string DecodeBase64(string base64Text)
        {
            if (base64Text == null)
                return null;

            byte[] bytes = Convert.FromBase64String(base64Text);
            return Encoding.ASCII.GetString(bytes);
        }

        public sealed class SignRequest
        {
            public string certPass { get; set; }
            public string certContent { get; set; }
            public string reqPayload { get; set; }
        }


        public static string BuildSoapEnvelope(string bodyXml)
        {
            if (!string.IsNullOrEmpty(bodyXml) && bodyXml.StartsWith("<?xml", StringComparison.Ordinal))
            {
                int idx = bodyXml.IndexOf("?>");
                if (idx >= 0)
                    bodyXml = bodyXml.Substring(idx + 2).TrimStart();
            }

            StringBuilder sb = new StringBuilder();

            //sb.Append(@"<?xml version=""1.0"" encoding=""utf-8""?>");
            sb.Append(@"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">");
            //sb.Append(@"xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" ");
            //
            //sb.Append(@"xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">");
            sb.Append("<s:Body>");
            sb.Append(bodyXml);
            sb.Append("</s:Body>");
            sb.Append("</s:Envelope>");

            return sb.ToString();
        }


        public static DateTime TruncateToSeconds(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Kind);
        }


        public static string FileToBase64(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("filePath darf nicht leer sein.", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Datei wurde nicht gefunden.", filePath);

            byte[] fileBytes = File.ReadAllBytes(filePath);
            return Convert.ToBase64String(fileBytes);
        }


        // --------------------------------------------------------------------
        // interne Helfer
        // --------------------------------------------------------------------




        private static bool AcceptAllCertifications(
            object sender,
            X509Certificate cert,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            // 1:1 wie in vielen Integration-DLLs – produktiv ggf. härter prüfen.
            return true;
        }
    }
}
