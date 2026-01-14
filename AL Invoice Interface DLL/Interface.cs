using Stihl.Albania.Fiscalization;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

[assembly: ClassInterface(ClassInterfaceType.AutoDispatch)]

namespace AL_Invoice_Interface_DLL
{
    [Guid("5a60efd8-bcdd-4e3e-b788-716c6f69491f")]
    public interface AL_Invoice_Interface_DLL_Interface
    {
        // --------------------------------------------------------------------
        // Global / Basisdaten
        // --------------------------------------------------------------------
        [DispId(1)]
        void _01SetBasicData(string certificatePassword, string certificatePath, string generateIICAndIICSignatureUrl, string signXmlRequestUrl, string signUblUrl, string fiscalizationServiceUrl, string eInvoiceServiceUrl);

        [DispId(2)]
        void _02GenerateIICAndIICSignature( string iicInput, string requestFilePathToSave, string responseFilePathToSave);


        [DispId(3)]
        string _03GetIIC();



        [DispId(4)]
        string _04GetIICSignature();

        [DispId(5)]
         void _05CreateQRCode(string Filepath, int width, int height, string QRCode);

        // --------------------------------------------------------------------
        // InvoiceClass – Fiskalisierungsrechnung
        // --------------------------------------------------------------------
        [DispId(30)]
        void _30NewInvoice();

        [DispId(31)]
        void _31InvoiceData(string uuid, string sendDateTime, string typeOfInv, string issueDateTime, string invNum, string invOrdNum, string iic, string iicSignature, bool isSimplifiedInv, bool isEinvoice, string currencyCode, double exchangeRate, bool currencyIsBuying, string operatorCode, string businUnitCode, string tcrCode, string softCode);

        [DispId(32)]
        void _32SetSeller(string idType, string idNum, string name, string address, string town, string country);

        [DispId(33)]
        void _33InvoiceData_2(DateTime SupplyDateOrPeriodStart, DateTime SupplyDateOrPeriodEnd, bool IsIssuerInVAT);

        [DispId(34)]
        void _34SetBuyer(string idType, string idNum, string name, string address, string town, string country);

        [DispId(35)]
        void _35AddLine(string name, string code, string unitOfMeasure, double quantity, double priceBeforeVat, double vatRate, double vatAmount, double priceAfterVat, bool isReverseCharge);

        [DispId(36)]
        void _36AddPayMethod(string type, double amount, string companyCardNumber);

        [DispId(37)]
        void _37AddVoucherNumber(string voucherNumber);

        [DispId(38)]
        void _38SetTotals(double totPriceWoVat, double totVatAmt, bool TotVATAmtSpecified, double totPrice);

        [DispId(39)]
        void _39AddSameTaxType(int numOfItems, double VATRate, bool VATRateSpecified, string exemptFromVATSameTaxItemSType, double priceBefVAT, double VATAmt, bool VATAmtSpecified);

        [DispId(41)]
        void _41SaveInvoiceRequest(string filePath);

        [DispId(42)]
        void _42SignSoapRequest(string RequestFilePathToSave, string ResponseFilePathToSave);

        [DispId(43)]
        void _43InvoiceSendRequest(string RequestFilePathToSave, string ResponseFilePathToSave);

        // --------------------------------------------------------------------
        // EInvoiceClass – UBL / eInvoice
        // --------------------------------------------------------------------
        [DispId(50)]
        void _50NewInvoice();

        [DispId(51)]
        void _51InvoiceData(string uuid, string sendDateTime, string UBLVersionID, string ID, string IssueDate, string InvTypelistID, string InvTypelistAgencyID, string InvTypeValue, string DocumentCurrencyCodelistID, string DocumentCurrencyCodelistAgencyID, string DocumentCurrencyCodeValue, string OrderReferenceID, string PeriodeDescriptionCode);

        [DispId(52)]
        void _52SetAlFiscalNotes(string issueDateTime, string operatorCode, string businessUnitCode, string softwareCode, bool isBadDebtInv, double amountWoVatLek, double amountWithVatLek, double sumOfTaxableAmountLek, string fic, string taxPointDate);



        [DispId(53)]
        void _53AccSupParty(string EndpointID, string EndpointAgencyID, string EndpointValue, string PartyIdentificationID, string PartyIdentificationValue, string Name, string PostalAddressSchemeID, string PostalAddressSchemeAgencyID, string PostalAddressSchemeValue, string StreetName, string AdditionalStreetName, string CityName, string PostalZone, string CountrylistID, string CountrylistAgencyID, string CountrylistValue);

        [DispId(54)]
        void _54AccSupPartyTaxSch(string CompanyID, string CompanyAgencyID, string CompanyValue, string TaxSchemeID, string TaxSchemeAgencyID, string TaxSchemeValue, string PartyLegalRegistrationName, string PartyLegalID, string PartyLegalAgencyID, string PartyLegalValue, string PartyLegalRegisAddrCity, string PartyLegalRegisAddrCountryIdentCode);

        [DispId(55)]
        void _55AccCustParty(string EndpointID, string EndpointAgencyID, string EndpointValue, string PartyIdentificationID, string PartyIdentificationValue, string Name, string PostalAddressSchemeID, string PostalAddressSchemeAgencyID, string PostalAddressSchemeValue, string StreetName, string AdditionalStreetName, string CityName, string PostalZone, string CountrylistID, string CountrylistAgencyID, string CountrylistValue);

        [DispId(56)]
        void _56AccCustPartyTaxSch(string CompanyID, string CompanyAgencyID, string CompanyValue, string TaxSchemeID, string TaxSchemeAgencyID, string TaxSchemeValue, string PartyLegalRegistrationName, string PartyLegalID, string PartyLegalAgencyID, string PartyLegalValue, string PartyLegalRegisAddrCity, string PartyLegalRegisAddrCountryIdentCode);

        [DispId(57)]
        void _57PayeeParty(string PartyIdentificationID, string PartyIdentificationAgencyID, string PartyIdentificationValue, string Name, string PartyLegalID, string PartyLegalAgencyID, string PartyLegaValue);

        [DispId(58)]
        void _58Delivery(string ActualDeliveryDate, string DeliveryLocationID, string DeliveryLocationAgencyID, string DeliveryLocationValue, string StreetName, string AdditionalStreetName, string CityName, string PostalZone, string CountryIdentificationCode);

        [DispId(59)]
        void _59PaymentMeans(string PaymentMeansID, string PaymentMeansValue, string PaymentDueDate, string PaymentChannelCode, string PaymentIDValue, string PayeeFinancialAccountID, string PayeeFinancialInstID);

        [DispId(60)]
        void _60TaxTotal(string currencyID, double Value);

        [DispId(61)]
        void _61AddTaxSubTotal(string TaxableAmountCurrencyID, double TaxableAmountValue, string TaxAmountCurrencyID, double TaxAmountValue, string TaxCategoryID, string TaxCategoryAgencyID, string TaxCategoryValue, double Percent, string TaxSchemeID, string TaxSchemeAgencyID, string TaxSchemeValue);

        [DispId(62)]
        void _62PaymentTermns(string Value);

        [DispId(63)]
        void _63LegalMonetary(string LineExtensionID, double LineExtensionValue, string TaxExclusiveID, double TaxExclusiveValue, string TaxInclusiveID, double TaxInclusive, string AllowanceTotalID, double AllowanceTotalValue, string ChargeTotalID, double ChargeTotalValue, string PrepaidID, double PrepaidValue, string PayableRoundingID, double PayableRoundingValue, string PayableID, double PayableValue);

        [DispId(64)]
        void _64AddInvoiceLine(string ID, string InvQtyUnitCode, double InvQtyValue, string LineExtensionCurrencyID, double LineExtensionValue, string AccountingCost, string OrderLineReferenceID, string TaxTotalcurrencyID, double TaxTotalValue);

        [DispId(65)]
        void _65AddInvLineItem(string Name, string SellersItemIdentification, string StandardItemIdentificationID, string StandardItemIdentificationAgencyID, string StandardItemIdentificationValue, string ClassifiedTaxCategoryID, string ClassifiedTaxCategoryAgencyID, string ClassifiedTaxCategoryValue, double ClassifiedTaxPercent, string TaxSchemeID, string TaxSchemeAgencyID, string TaxSchemeValue, string PriceAmountCurrId, double PriceAmountValue, string BaseQuantityUnitCode, double BaseQuantityValue);

        [DispId(66)]
        void _66AddInvLineItemAllowCharge(double MultiplierFactorNumeric, string currencyID, double Amount);

       

        [DispId(67)]
        void _67SaveInvoice(string FilePath);

        [DispId(68)]
        void _68SignUBLInvoice(string RequestFilePathToSave, string ResponseFilePathToSave);

        [DispId(69)]
         void _69SignSoapRequest(string RequestFilePathToSave, string ResponseFilePathToSave);

        [DispId(70)]
        string _70InvoiceSendRequest(string requestUUID, string RequestFilePathToSave, string ResponseFilePathToSave);
    }

    // Events-Interface (leer, wie bei dir)
    [Guid("db79e3cf-b4a5-4182-967e-486043409577"), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface AL_Invoice_Interface_DLL_Events
    {
    }

    [Guid("265edc68-167b-4595-9eed-3954f7c9a811")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComSourceInterfaces(typeof(AL_Invoice_Interface_DLL_Events))]
    public class AL_Invoice_Interface_DLL_Class : AL_Invoice_Interface_DLL_Interface
    {
        private readonly InvoiceClass _invoice;
        private readonly EInvoiceClass _eInvoice;

        public AL_Invoice_Interface_DLL_Class()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            _invoice = new InvoiceClass();
            _eInvoice = new EInvoiceClass();
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                var name = new AssemblyName(args.Name);
                File.WriteAllText(@"c:\temp\test.txt", name.Name);

                if (name.Name == "System.Runtime.CompilerServices.Unsafe")
                {
                    var path = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;
                    return Assembly.LoadFile(Path.Combine(path.FullName, "System.Runtime.CompilerServices.Unsafe.dll"));
                }

                if (name.Name == "UblSharp")
                {
                    var path = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;
                    return Assembly.LoadFile(Path.Combine(path.FullName, "UblSharpSigned.dll"));
                }
                if (name.Name == "Microsoft.Bcl.AsyncInterfaces")
                {
                    var path = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;
                    return Assembly.LoadFile(Path.Combine(path.FullName, "Microsoft.Bcl.AsyncInterfaces.dll"));
                }
                if (name.Name == "System.Threading.Tasks.Extensions")
                {
                    var path = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;
                    return Assembly.LoadFile(Path.Combine(path.FullName, "System.Threading.Tasks.Extensions.dll"));
                }
                if (name.Name == "System.Text.Encodings.Web")
                {
                    var path = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;
                    return Assembly.LoadFile(Path.Combine(path.FullName, "System.Text.Encodings.Web.dll"));
                }
                if (name.Name == "System.Memory")
                {
                    var path = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;
                    return Assembly.LoadFile(Path.Combine(path.FullName, "System.Memory.dll"));
                }




            }
            catch (Exception ex)
            {
                File.WriteAllText(@"c:\temp\test.txt", ex.Message);
            }

            return null;
        }

        // --------------------------------------------------------------------
        // Global / Basisdaten
        // --------------------------------------------------------------------
        public void _01SetBasicData(string certificatePassword, string certificatePath, string generateIICAndIICSignatureUrl, string signXmlRequestUrl, string signUblUrl, string fiscalizationServiceUrl, string eInvoiceServiceUrl)
        {
            Global._01SetBasicData( certificatePassword,  certificatePath, generateIICAndIICSignatureUrl, signXmlRequestUrl, signUblUrl, fiscalizationServiceUrl, eInvoiceServiceUrl);
        }


        public  void _02GenerateIICAndIICSignature( string iicInput, string requestFilePathToSave, string responseFilePathToSave)
        {
            Global._02GenerateIICAndIICSignature( iicInput, requestFilePathToSave, responseFilePathToSave);
        }

        public  string _03GetIIC()
        {
            return(Global._03GetIIC());
        }

        public  string _04GetIICSignature()
        {
            return (Global._04GetIICSignature());
        }

        public void _05CreateQRCode(string Filepath, int width, int height, string QRCode)
        {
            Global._05CreateQRCode( Filepath,  width,  height,  QRCode);
        }

        

        // --------------------------------------------------------------------
        // InvoiceClass – Fiskalisierung
        // --------------------------------------------------------------------
        public void _30NewInvoice()
        {
            _invoice._30NewInvoice();
        }

        public void _31InvoiceData(string uuid, string sendDateTime, string typeOfInv, string issueDateTime, string invNum, string invOrdNum, string iic, string iicSignature, bool isSimplifiedInv, bool isEinvoice, string currencyCode, double exchangeRate, bool currencyIsBuying, string operatorCode, string businUnitCode, string tcrCode, string softCode)
        {
            _invoice._31InvoiceData(uuid, sendDateTime, typeOfInv, issueDateTime, invNum, invOrdNum, iic, iicSignature, isSimplifiedInv, isEinvoice, currencyCode, exchangeRate, currencyIsBuying, operatorCode, businUnitCode, tcrCode, softCode);

        }

        public void _32SetSeller(string idType, string idNum, string name, string address, string town, string country)
        {
            _invoice._32SetSeller(idType, idNum, name, address, town, country);
        }

        public void _33InvoiceData_2(DateTime SupplyDateOrPeriodStart, DateTime SupplyDateOrPeriodEnd, bool IsIssuerInVAT)
        {
            _invoice._33InvoiceData_2(SupplyDateOrPeriodStart, SupplyDateOrPeriodEnd, IsIssuerInVAT);
        }

        public void _34SetBuyer(string idType, string idNum, string name, string address, string town, string country)
        {
            _invoice._34SetBuyer(idType, idNum, name, address, town, country);
        }

        public void _35AddLine(string name, string code, string unitOfMeasure, double quantity, double priceBeforeVat, double vatRate, double vatAmount, double priceAfterVat, bool isReverseCharge)
        {
            _invoice._35AddLine(name, code, unitOfMeasure, quantity, priceBeforeVat, vatRate, vatAmount, priceAfterVat, isReverseCharge);
        }

        public void _36AddPayMethod(string type, double amount, string companyCardNumber)
        {
            _invoice._36AddPayMethod(type, amount, companyCardNumber);
        }

        public void _37AddVoucherNumber(string voucherNumber)
        {
            _invoice._37AddVoucherNumber(voucherNumber);
        }

        public void _38SetTotals(double totPriceWoVat, double totVatAmt, bool TotVATAmtSpecified, double totPrice)
        {
            _invoice._38SetTotals(totPriceWoVat, totVatAmt, TotVATAmtSpecified, totPrice);
        }

        public void _39AddSameTaxType(int numOfItems, double VATRate, bool VATRateSpecified, string exemptFromVATSameTaxItemSType, double priceBefVAT, double VATAmt, bool VATAmtSpecified)
        {
            _invoice._39AddSameTaxType( numOfItems,  VATRate,  VATRateSpecified,  exemptFromVATSameTaxItemSType,  priceBefVAT,  VATAmt,  VATAmtSpecified);
        }

        public void _41SaveInvoiceRequest(string filePath)
        {
            _invoice._41SaveInvoiceRequest(filePath);
        }

        public void _42SignSoapRequest(string RequestFilePathToSave, string ResponseFilePathToSave)
        {
            _invoice._42SignSoapRequest(RequestFilePathToSave, ResponseFilePathToSave);
        }

        public void _43InvoiceSendRequest(string RequestFilePathToSave, string ResponseFilePathToSave)
        {
            _invoice._43InvoiceSendRequest(RequestFilePathToSave, ResponseFilePathToSave);
        }

        // --------------------------------------------------------------------
        // EInvoiceClass – UBL / eInvoice
        // --------------------------------------------------------------------
        public void _50NewInvoice()
        {
            _eInvoice._50NewInvoice();
        }

        public void _51InvoiceData(string uuid, string sendDateTime, string UBLVersionID, string ID, string IssueDate, string InvTypelistID, string InvTypelistAgencyID, string InvTypeValue, string DocumentCurrencyCodelistID, string DocumentCurrencyCodelistAgencyID, string DocumentCurrencyCodeValue, string OrderReferenceID, string PeriodeDescriptionCode)
        {
            _eInvoice._51InvoiceData(uuid, sendDateTime, UBLVersionID, ID, IssueDate, InvTypelistID, InvTypelistAgencyID, InvTypeValue, DocumentCurrencyCodelistID, DocumentCurrencyCodelistAgencyID, DocumentCurrencyCodeValue, OrderReferenceID, PeriodeDescriptionCode);
        }

        public void _52SetAlFiscalNotes(string issueDateTime, string operatorCode, string businessUnitCode, string softwareCode, bool isBadDebtInv, double amountWoVatLek, double amountWithVatLek, double sumOfTaxableAmountLek, string fic, string taxPointDate)
        {
            _eInvoice._52SetAlFiscalNotes(issueDateTime, operatorCode, businessUnitCode, softwareCode, isBadDebtInv, amountWoVatLek, amountWithVatLek, sumOfTaxableAmountLek, fic, taxPointDate);
       
        }


        public void _53AccSupParty(string EndpointID, string EndpointAgencyID, string EndpointValue, string PartyIdentificationID, string PartyIdentificationValue, string Name, string PostalAddressSchemeID, string PostalAddressSchemeAgencyID, string PostalAddressSchemeValue, string StreetName, string AdditionalStreetName, string CityName, string PostalZone, string CountrylistID, string CountrylistAgencyID, string CountrylistValue)
        {
            _eInvoice._53AccSupParty(EndpointID, EndpointAgencyID, EndpointValue, PartyIdentificationID, PartyIdentificationValue, Name, PostalAddressSchemeID, PostalAddressSchemeAgencyID, PostalAddressSchemeValue, StreetName, AdditionalStreetName, CityName, PostalZone, CountrylistID, CountrylistAgencyID, CountrylistValue);
        }

        public void _54AccSupPartyTaxSch(string CompanyID, string CompanyAgencyID, string CompanyValue, string TaxSchemeID, string TaxSchemeAgencyID, string TaxSchemeValue, string PartyLegalRegistrationName, string PartyLegalID, string PartyLegalAgencyID, string PartyLegalValue, string PartyLegalRegisAddrCity, string PartyLegalRegisAddrCountryIdentCode)
        {
            _eInvoice._54AccSupPartyTaxSch(CompanyID, CompanyAgencyID, CompanyValue, TaxSchemeID, TaxSchemeAgencyID, TaxSchemeValue, PartyLegalRegistrationName, PartyLegalID, PartyLegalAgencyID, PartyLegalValue, PartyLegalRegisAddrCity, PartyLegalRegisAddrCountryIdentCode);
        }

        public void _55AccCustParty(string EndpointID, string EndpointAgencyID, string EndpointValue, string PartyIdentificationID, string PartyIdentificationValue, string Name, string PostalAddressSchemeID, string PostalAddressSchemeAgencyID, string PostalAddressSchemeValue, string StreetName, string AdditionalStreetName, string CityName, string PostalZone, string CountrylistID, string CountrylistAgencyID, string CountrylistValue)
        {
            _eInvoice._55AccCustParty(EndpointID, EndpointAgencyID, EndpointValue, PartyIdentificationID, PartyIdentificationValue, Name, PostalAddressSchemeID, PostalAddressSchemeAgencyID, PostalAddressSchemeValue, StreetName, AdditionalStreetName, CityName, PostalZone, CountrylistID, CountrylistAgencyID, CountrylistValue);
        }

        public void _56AccCustPartyTaxSch(string CompanyID, string CompanyAgencyID, string CompanyValue, string TaxSchemeID, string TaxSchemeAgencyID, string TaxSchemeValue, string PartyLegalRegistrationName, string PartyLegalID, string PartyLegalAgencyID, string PartyLegalValue, string PartyLegalRegisAddrCity, string PartyLegalRegisAddrCountryIdentCode)
        {
            _eInvoice._56AccCustPartyTaxSch(CompanyID, CompanyAgencyID, CompanyValue, TaxSchemeID, TaxSchemeAgencyID, TaxSchemeValue, PartyLegalRegistrationName, PartyLegalID, PartyLegalAgencyID, PartyLegalValue, PartyLegalRegisAddrCity, PartyLegalRegisAddrCountryIdentCode);
        }

        public void _57PayeeParty(string PartyIdentificationID, string PartyIdentificationAgencyID, string PartyIdentificationValue, string Name, string PartyLegalID, string PartyLegalAgencyID, string PartyLegaValue)
        {
            _eInvoice._57PayeeParty(PartyIdentificationID, PartyIdentificationAgencyID, PartyIdentificationValue, Name, PartyLegalID, PartyLegalAgencyID, PartyLegaValue);
        }

        public void _58Delivery(string ActualDeliveryDate, string DeliveryLocationID, string DeliveryLocationAgencyID, string DeliveryLocationValue, string StreetName, string AdditionalStreetName, string CityName, string PostalZone, string CountryIdentificationCode)
        {
            _eInvoice._58Delivery(ActualDeliveryDate, DeliveryLocationID, DeliveryLocationAgencyID, DeliveryLocationValue, StreetName, AdditionalStreetName, CityName, PostalZone, CountryIdentificationCode);
        }

        public void _59PaymentMeans(string PaymentMeansID, string PaymentMeansValue, string PaymentDueDate, string PaymentChannelCode, string PaymentIDValue, string PayeeFinancialAccountID, string PayeeFinancialInstID)
        {
            _eInvoice._59PaymentMeans(PaymentMeansID, PaymentMeansValue, PaymentDueDate, PaymentChannelCode, PaymentIDValue, PayeeFinancialAccountID, PayeeFinancialInstID);
        }

        public void _60TaxTotal(string currencyID, double Value)
        {
            _eInvoice._60TaxTotal(currencyID, Value);
        }

        public void _61AddTaxSubTotal(string TaxableAmountCurrencyID, double TaxableAmountValue, string TaxAmountCurrencyID, double TaxAmountValue, string TaxCategoryID, string TaxCategoryAgencyID, string TaxCategoryValue, double Percent, string TaxSchemeID, string TaxSchemeAgencyID, string TaxSchemeValue)
        {
            _eInvoice._61AddTaxSubTotal(TaxableAmountCurrencyID, TaxableAmountValue, TaxAmountCurrencyID, TaxAmountValue, TaxCategoryID, TaxCategoryAgencyID, TaxCategoryValue, Percent, TaxSchemeID, TaxSchemeAgencyID, TaxSchemeValue);
        }

        public void _62PaymentTermns(string Value)
        {
            _eInvoice._62PaymentTermns(Value);
        }

        public void _63LegalMonetary(string LineExtensionID, double LineExtensionValue, string TaxExclusiveID, double TaxExclusiveValue, string TaxInclusiveID, double TaxInclusive, string AllowanceTotalID, double AllowanceTotalValue, string ChargeTotalID, double ChargeTotalValue, string PrepaidID, double PrepaidValue, string PayableRoundingID, double PayableRoundingValue, string PayableID, double PayableValue)
        {
            _eInvoice._63LegalMonetary(LineExtensionID, LineExtensionValue, TaxExclusiveID, TaxExclusiveValue, TaxInclusiveID, TaxInclusive, AllowanceTotalID, AllowanceTotalValue, ChargeTotalID, ChargeTotalValue, PrepaidID, PrepaidValue, PayableRoundingID, PayableRoundingValue, PayableID, PayableValue);
        }

        public void _64AddInvoiceLine(string ID, string InvQtyUnitCode, double InvQtyValue, string LineExtensionCurrencyID, double LineExtensionValue, string AccountingCost, string OrderLineReferenceID, string TaxTotalcurrencyID, double TaxTotalValue)
        {
            _eInvoice._64AddInvoiceLine(ID, InvQtyUnitCode, InvQtyValue, LineExtensionCurrencyID, LineExtensionValue, AccountingCost, OrderLineReferenceID, TaxTotalcurrencyID, TaxTotalValue);
        }

        public void _65AddInvLineItem(string Name, string SellersItemIdentification, string StandardItemIdentificationID, string StandardItemIdentificationAgencyID, string StandardItemIdentificationValue, string ClassifiedTaxCategoryID, string ClassifiedTaxCategoryAgencyID, string ClassifiedTaxCategoryValue, double ClassifiedTaxPercent, string TaxSchemeID, string TaxSchemeAgencyID, string TaxSchemeValue, string PriceAmountCurrId, double PriceAmountValue, string BaseQuantityUnitCode, double BaseQuantityValue)
        {
            _eInvoice._65AddInvLineItem(Name, SellersItemIdentification, StandardItemIdentificationID, StandardItemIdentificationAgencyID, StandardItemIdentificationValue, ClassifiedTaxCategoryID, ClassifiedTaxCategoryAgencyID, ClassifiedTaxCategoryValue, ClassifiedTaxPercent, TaxSchemeID, TaxSchemeAgencyID, TaxSchemeValue, PriceAmountCurrId, PriceAmountValue, BaseQuantityUnitCode, BaseQuantityValue);
        }

        public void _66AddInvLineItemAllowCharge(double MultiplierFactorNumeric, string currencyID, double Amount)
        {
            _eInvoice._66AddInvLineItemAllowCharge(MultiplierFactorNumeric, currencyID, Amount);
        }

       

        public void _67SaveInvoice(string FilePath)
        {
            _eInvoice._67SaveInvoice(FilePath);
        }

        public void _68SignUBLInvoice(string RequestFilePathToSave, string ResponseFilePathToSave)
        {
            _eInvoice._68SignUBLInvoice(RequestFilePathToSave, ResponseFilePathToSave);
        }

        public void _69SignSoapRequest(string RequestFilePathToSave, string ResponseFilePathToSave)
        {
            _eInvoice._69SignSoapRequest( RequestFilePathToSave,ResponseFilePathToSave);
        }

        public string _70InvoiceSendRequest(string requestUUID, string RequestFilePathToSave, string ResponseFilePathToSave)
        {
            return _eInvoice._70InvoiceSendRequest(requestUUID, RequestFilePathToSave, ResponseFilePathToSave);
        }
    }
}
