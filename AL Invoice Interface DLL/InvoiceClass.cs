using AL_Invoice_Interface_DLL.financeService;
using Stihl.Albania.Fiscalization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace AL_Invoice_Interface_DLL
{
    public class InvoiceClass
    {
        RegisterInvoiceRequest _request = new RegisterInvoiceRequest();
        InvoiceType _invoice;
        private string _SignedSoapRequest = "";

        public string resp;

        public void _30NewInvoice()
        {
            _request = new RegisterInvoiceRequest();
            _invoice = new InvoiceType();
            _request.Header = new RegisterInvoiceRequestHeaderType();
            _SignedSoapRequest = "";


         }

        public void _31InvoiceData(string uuid, string sendDateTime, string typeOfInv, string issueDateTime, string invNum, string invOrdNum, string iic, string iicSignature, bool isSimplifiedInv, bool isEinvoice, string currencyCode, double exchangeRate, bool currencyIsBuying, string operatorCode, string businUnitCode, string tcrCode, string softCode


            )
        {
            // Header
            _request.Header.UUID = uuid;
            _request.Header.SendDateTime = sendDateTime;

            // Invoice-Basisteil
            _invoice.TypeOfInv = (InvoiceSType)Enum.Parse(typeof(InvoiceSType), typeOfInv);    // "CASH" / "NONCASH"
            _invoice.IIC = iic;
            _invoice.IICSignature = iicSignature;
            _invoice.IssueDateTime = issueDateTime;
            _invoice.InvNum = invNum;
            _invoice.InvOrdNum = invOrdNum;
            _invoice.IsSimplifiedInv = isSimplifiedInv;
            _invoice.IsEinvoice = isEinvoice;
            _invoice.IsEinvoiceSpecified= true;

           



            if (operatorCode != "")
                _invoice.OperatorCode = operatorCode;
            if (businUnitCode != "")
                _invoice.BusinUnitCode = businUnitCode;
            if (tcrCode != "")
                _invoice.TCRCode = tcrCode;
            if (softCode != "")
                _invoice.SoftCode = softCode;

            // Währung optional, wenn != ALL
            if (currencyCode != "")
            {
                var cur = new CurrencyType
                {
                    Code = (CurrencyCodeSType)Enum.Parse(typeof(CurrencyCodeSType), currencyCode),
                    ExRate = exchangeRate
                };

                cur.IsBuying = currencyIsBuying;
                _invoice.Currency = cur;
            }
        }

        public void _32SetSeller(string idType, string idNum, string name, string address, string town, string country)
        {
            _invoice.Seller = new SellerType
            {
                IDType = (IDTypeSType)Enum.Parse(typeof(IDTypeSType), idType),
                IDNum = idNum,
                
                Name = name,
                Address = address,
                Town = town,
                Country = (CountryCodeSType)Enum.Parse(typeof(CountryCodeSType), country),
                CountrySpecified = true
            };
        }

        public void _33InvoiceData_2(DateTime SupplyDateOrPeriodStart, DateTime SupplyDateOrPeriodEnd, bool IsIssuerInVAT)
        {
            _invoice.SupplyDateOrPeriod = new SupplyDateOrPeriodType();
            _invoice.SupplyDateOrPeriod.Start = SupplyDateOrPeriodStart;
            _invoice.SupplyDateOrPeriod.End = SupplyDateOrPeriodEnd;

            _invoice.IsIssuerInVAT = IsIssuerInVAT;
           
        }


       

        public void _34SetBuyer(string idType, string idNum, string name, string address, string town, string country)
        {
            _invoice.Buyer = new BuyerType
            {
                IDType = (IDTypeSType)Enum.Parse(typeof(IDTypeSType), idType),
                IDNum = idNum,
                Name = name,
                Address = address,
                Town = town,
                Country = (CountryCodeSType)Enum.Parse(typeof(CountryCodeSType), country),
                CountrySpecified = true
            };
        }

        public void _35AddLine(string name, string code, string unitOfMeasure, double quantity, double priceBeforeVat, double vatRate, double vatAmount, double priceAfterVat, bool isReverseCharge)
        {
            InvoiceItemType line = new InvoiceItemType();

            // Grunddaten
            line.N = name;
            line.C = code;
            line.U = unitOfMeasure;
            line.Q = ToMoney(quantity);

            // Gesamtpreise
            line.PB = ToMoney(priceBeforeVat);   // Total vor USt
            line.PA = ToMoney(priceAfterVat);    // Total nach USt

            // VAT-Infos
            line.VR = ToMoney(vatRate);
            line.VRSpecified = true;

            line.VA = ToMoney(vatAmount);
            line.VASpecified = true;

            // Reverse Charge (auf RR gemappt)
            line.RR = isReverseCharge;
            line.RRSpecified = true;

            // R (Rabatt), IN (Included VAT), EX (Exempt) lassen wir in dieser Funktion bewusst leer

            if (_invoice.Items == null)
            {
                _invoice.Items = new InvoiceItemType[] { line };
            }
            else
            {
                InvoiceItemType[] oldItems = _invoice.Items;
                InvoiceItemType[] newItems = new InvoiceItemType[oldItems.Length + 1];
                Array.Copy(oldItems, newItems, oldItems.Length);
                newItems[newItems.Length - 1] = line;
                _invoice.Items = newItems;
            }
        }

        public void _36AddPayMethod(string type, double amount, string companyCardNumber)
        {
            var pm = new PayMethodType
            {
                Type = (PaymentMethodTypeSType)Enum.Parse(typeof(PaymentMethodTypeSType), type),
                Amt = ToMoney(amount)
            };

            if (companyCardNumber != "")
                pm.CompCard = companyCardNumber;

            if (_invoice.PayMethods == null)
            {
                _invoice.PayMethods = new PayMethodType[] { pm };
            }
            else
            {
                List<PayMethodType> pms = _invoice.PayMethods.ToList();
                pms.Add(pm);
                _invoice.PayMethods = pms.ToArray();
            }
        }

        public void _37AddVoucherNumber(string voucherNumber)
        {
            // Hinweis: Append() ändert das Array nicht – das musst du bei Bedarf noch sauber umbauen.
            VoucherType voucherType = new VoucherType();
            voucherType.Num = voucherNumber;
            _invoice.PayMethods.Last().Vouchers.Append(voucherType);
        }

        public void _38SetTotals(double totPriceWoVat, double totVatAmt, bool TotVATAmtSpecified,  double totPrice)
        {
         
            _invoice.TotPriceWoVAT = ToMoney(totPriceWoVat);
           
            _invoice.TotVATAmt = ToMoney(totVatAmt);
            _invoice.TotVATAmtSpecified = TotVATAmtSpecified;


            _invoice.TotPrice = ToMoney(totPrice);
         
          
        }

      

        public void _39AddSameTaxType(int numOfItems, double VATRate, bool VATRateSpecified ,  string exemptFromVATSameTaxItemSType, double priceBefVAT, double VATAmt, bool VATAmtSpecified)
        {
            SameTaxType st = new SameTaxType();

            // Anzahl der Positionen mit gleichem Steuersatz
            st.NumOfItems = numOfItems;

            // Gesamtpreis vor USt
            st.PriceBefVAT = ToMoney(priceBefVAT);

            // Steuersatz (Parameter vATAmt ist hier als Rate zu verstehen)
            if (VATRateSpecified)
            {
                st.VATRate = ToMoney(VATRate);
                st.VATRateSpecified = VATRateSpecified;
            }

         

            // Steuerbefreiung (optional)
            if (!string.IsNullOrEmpty(exemptFromVATSameTaxItemSType))
            {
                st.ExemptFromVAT = (ExemptFromVATSameTaxItemSType)Enum.Parse(typeof(ExemptFromVATSameTaxItemSType), exemptFromVATSameTaxItemSType);
                st.ExemptFromVATSpecified = true;
            }

            // USt-Betrag
            if (VATAmtSpecified)
            {
                st.VATAmt = ToMoney(VATAmt);
                st.VATAmtSpecified = true;
            }

            // An die Rechnung anhängen – Pattern wie bei PayMethods
            if (_invoice.SameTaxes == null)
            {
                _invoice.SameTaxes = new SameTaxType[] { st };
            }
            else
            {
                List<SameTaxType> sts = _invoice.SameTaxes.ToList();
                sts.Add(st);
                _invoice.SameTaxes = sts.ToArray();
            }
        }


        public void _41SaveInvoiceRequest(string filePath)
        {
            //string xml = GetRequestXml();
            string xml=Global.BuildSoapEnvelope(GetRequestXml());
            File.WriteAllText(filePath, xml, Encoding.ASCII);
        }

       

        public void _42SignSoapRequest(string RequestFilePathToSave,string ResponseFilePathToSave)
        {

            string xmlBase64 = Convert.ToBase64String(Encoding.ASCII.GetBytes(Global.BuildSoapEnvelope(GetRequestXml())));


            var req = new Global.SignRequest { certPass = Global.CertificatePassword, certContent = Global.FileToBase64(Global.CertificatePath), reqPayload = xmlBase64 };
            string jsonRequest = JsonSerializer.Serialize(req);

            // Request optional speichern
            if (!string.IsNullOrEmpty(RequestFilePathToSave))
                File.WriteAllText(RequestFilePathToSave, jsonRequest, Encoding.ASCII);

            string rawResponse = "";
            Global.HttpPostJson(Global.SignXmlRequestUrl, jsonRequest, ref rawResponse);

           
            // Response optional speichern
            if (!string.IsNullOrEmpty(ResponseFilePathToSave))
                 File.WriteAllText(ResponseFilePathToSave, rawResponse, Encoding.ASCII);


            var doc = JsonDocument.Parse(rawResponse);
            string signedSoapBase64 = doc.RootElement.GetProperty("SignedSoap").GetString();

            // Base64 → XML-String
            string signedSoapXml = Encoding.ASCII.GetString(Convert.FromBase64String(signedSoapBase64));
            
            // z.B. im InvoiceClass:
            _SignedSoapRequest = signedSoapXml;


            /*
            var resp = JsonSerializer.Deserialize<IicResponse>(rawResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (resp == null || string.IsNullOrWhiteSpace(resp.IIC) || string.IsNullOrWhiteSpace(resp.IICSignature))
                throw new InvalidOperationException("IIC oder IICSignature konnte aus der Antwort nicht gelesen werden. Response: " + rawResponse);
            */





            // unsigned XML → Base64

            /*
            string jsonBody = "{\"XmlRequest\":\"" + xmlBase64 + "\"}";

            string helperResponse = "";
            // URL für SignAndSOAPMessage muss in Global._01SetBasicData gesetzt werden (SignXmlRequestRelativeUrl)

            string url = Global.SignXmlRequestUrl;

            if (RequestFilePathToSave!="")
            {
                File.WriteAllText(RequestFilePathToSave, jsonBody, Encoding.UTF8);
            }


            Global.HttpPostJson(url, jsonBody, ref helperResponse);

            
            if (ResponseFilePathToSave != "")
            {
                File.WriteAllText(ResponseFilePathToSave, helperResponse, Encoding.UTF8);
            }

            _SignSoapRequest = helperResponse;
            */

        }

        public void _43InvoiceSendRequest(string RequestFilePathToSave, string ResponseFilePathToSave)
        {
            if (_SignedSoapRequest == "") 
            {
                _SignedSoapRequest = Global.BuildSoapEnvelope(GetRequestXml());
                  

            }
           

            if (RequestFilePathToSave != "")
            {
                File.WriteAllText(RequestFilePathToSave, _SignedSoapRequest, Encoding.ASCII);
            }

            string fiscalResponse = "";
            
            /*
          
            try
            { 
            financeService.FiscalizationService serv = new FiscalizationService();
            serv.Url = Global.FiscalizationServiceUrl;
                _request.Invoice = _invoice;
                //File.WriteAllText(RequestFilePathToSave, GetRequestXml(), Encoding.UTF8);
              
            serv.registerInvoice(_request);
            }
            catch (System.Exception ex)
            {

            }
           */



            Global.HttpPostXml(Global.FiscalizationServiceUrl, _SignedSoapRequest, Global.FiscalizationServiceUrl, ref fiscalResponse);

            // 4) Response optional speichern
            if (ResponseFilePathToSave != "")
                File.WriteAllText(ResponseFilePathToSave, fiscalResponse, Encoding.ASCII);

        }
       

        private decimal ToMoney(decimal value)
        {
            return decimal.Parse(value.ToString("0.00", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
        }

        private decimal ToMoney(double value)
        {
            return ToMoney((decimal)value);
        }

        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }

        private class AsciiStringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.ASCII;
        }
        private string GetRequestXml()
        {
            _request.Invoice = _invoice;

            if (_request == null)
                throw new InvalidOperationException("Es wurde noch kein RegisterInvoiceRequest aufgebaut. Bitte zuerst _50NewInvoice usw. aufrufen.");

            string fiscalNs = "https://eFiskalizimi.tatime.gov.al/FiscalizationService/schema";




            // WICHTIG: XmlRootAttribute mit Namespace übergeben
            var root = new XmlRootAttribute("RegisterInvoiceRequest");
            root.Namespace = fiscalNs;

            XmlSerializer serializer = new XmlSerializer(typeof(RegisterInvoiceRequest), root);

            using (AsciiStringWriter sw = new AsciiStringWriter())
            {
                serializer.Serialize(sw, _request);
                return sw.ToString();
            }
        }



      



    }
}
