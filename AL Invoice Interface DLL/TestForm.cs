
using Stihl.Albania.Fiscalization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;


using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace AL_Invoice_Interface_DLL
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            

        }

        private System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                var name = new AssemblyName(args.Name);
                System.IO.File.WriteAllText(@"c:\temp\test.txt", name.Name);
                if (name.Name == "System.Runtime.CompilerServices.Unsafe")
                {
                    var Path = new FileInfo((System.Reflection.Assembly.GetExecutingAssembly().Location)).Directory;
                    return Assembly.LoadFile(Path.FullName + "\\System.Runtime.CompilerServices.Unsafe.dll");

                    //return typeof(System.Runtime.CompilerServices.Unsafe).Assembly;
                }
                if (name.Name == "UblSharp")
                {
                    var Path = new FileInfo((System.Reflection.Assembly.GetExecutingAssembly().Location)).Directory;
                    return Assembly.LoadFile(Path.FullName + "\\UblSharpSigned.dll");

                    //return typeof(System.Runtime.CompilerServices.Unsafe).Assembly;
                }
            }
            catch (System.Exception ex)
            {
                System.IO.File.WriteAllText(@"c:\temp\test.txt", ex.Message);
            }
            return null;
        }

       

        private void button1_Click(object sender, EventArgs e)
        {


            // 1) Globales Basis-Setup
            Global._01SetBasicData("bR3#H5uFg!aL", @"C:\Temp\AL\INTEGRATED TECH SOLUTIONS.p12", "http://80.90.92.219:2014/api/Values/GenerateIICAndIICSignature", "http://80.90.92.219:2014/api/Values/SignAndSOAPMessage", "http://80.90.92.219:2014/api/Values/SignUBL", "https://efiskalizimi-test.tatime.gov.al/FiscalizationService-v3", "https://Einvoice-test.tatime.gov.al/EinvoiceService-v1");

            // 2) Neue InvoiceClass-Instanz
            InvoiceClass inv = new InvoiceClass();

            // 3) Neue Rechnung starten
            inv._30NewInvoice();

           // inv.test();

            // 4) Header / Invoice-Daten
            string uuid = Guid.NewGuid().ToString();
            DateTime now = DateTime.Now;

            inv._31InvoiceData(uuid, Global.TruncateToSeconds(now).ToString(), "NONCASH", Global.TruncateToSeconds(now).ToString(), "1/2025/ab001cd002", "1", "351249D647C96C1247131D7D82045F21", "2CE4FAE786367754E7BD36B63C53A91C601588D4961774356BE94D8035BF06FD7811F3D23A1A172DCF7F77137527DEB23A8318E290408C629E2BC18347C5025925615C7487AEBA88C6B76DC287EB576D35E610B113E4C1F8A058D437CA7A10A61062FF1C5E5B3A156C3697FAD2436FC628B13ED0C514EEF43B260C08D0513F0A8478F2A3E7C9232C2589945258944FD35C8F249A0114C314FA78471484A249272306C6E818787CCA4F36451B064892AFDA5AA415BC06FE808495B6EDDCDE5F8AA516681FD8479E3FF72FF61A9DDB7C3F57DFEF6F2F7BFBD3AC4D3CAC47D893C13E58FA98A837F29C5E11FB182E495D2232347FC05CFDB83D911CB7FB3E827BDF", false, false, "EUR", 1.0, false, "bm679fd207", "nk877lp690", "", "co442rd108");

            // 5) Seller setzen
            inv._32SetSeller("NUIS", "L71862031", "STIHL Albania Sh.p.k.", "Rruga Dëshmorët e 4 Shkurtit", "Tirana", "ALB");

            // 6) Buyer setzen
            inv._34SetBuyer("NUIS", "K12345678", "Demo Customer SHPK", "Demo Street 1", "Tirana", "ALB");

            // 7) Position hinzufügen
            inv._35AddLine("Demo-Artikel", "A0001", "pcs", 2.00, 100.00, 20.00, 20.00, 120.00, false,"TYPE_2");

            // 8) Zahlungsart hinzufügen
            inv._36AddPayMethod("TRANSFER", 120.0, "");

            // 9) Unsigned XML speichern
            inv._41SaveInvoiceRequest(@"C:\Temp\AL\al_unsigned.xml");

            // 10) SignAndSOAPMessage aufrufen (JSON Request / Response loggen)
            //inv._62SignSoapRequest(@"C:\Temp\AL\sign_request.json", @"C:\Temp\AL\sign_response.xml");

            // 11) Signierte SOAP an FiscalizationService schicken
            inv._43InvoiceSendRequest(@"C:\Temp\AL\soap_request.xml", @"C:\Temp\AL\soap_response.xml");
        }

        private void TestForm_Load(object sender, EventArgs e)
        {

        }

        private void bttUpload_Click(object sender, EventArgs e)
        {
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            
            Global._01SetBasicData("bR3#H5uFg!aL", @"C:\Temp\AL\INTEGRATED TECH SOLUTIONS.p12", "http://80.90.92.219:2014/api/Values/GenerateIICAndIICSignature", "http://80.90.92.219:2014/api/Values/SignAndSOAPMessage", "http://80.90.92.219:2014/api/Values/SignUBL", "https://efiskalizimi-test.tatime.gov.al/FiscalizationService-v3", "https://Einvoice-test.tatime.gov.al/EinvoiceService-v1");

            EInvoiceClass eInvoiceClass = new EInvoiceClass();
            eInvoiceClass._50NewInvoice();
            eInvoiceClass._invoice=eInvoiceClass.Create();

            eInvoiceClass._64AddInvoiceLine(
        "1",
        "C62",
        1,
        "EUR",
        100,
        "",
        "",
        "EUR",
        0
    );
            eInvoiceClass._65AddInvLineItem("Demo-Artikel", "A0001", "GTIN", "9", "1234567890124", "UN/ECE 5305", "6", "S", 20.0, "UN/ECE 5153", "6", "VAT", "EUR", 50.0, "pcs", 1.0,"TAXEXCODE","TAXREASON");


            eInvoiceClass._67SaveInvoice("c:\\temp\\AL\\UBLINvoice.xml");

            //eInvoiceClass._70InvoiceSendRequest("351249D647C96C1247131D7D82045F21", @"C:\Temp\AL\ubl_request.xml", @"C:\Temp\AL\ubl_response.xml");


        }
        private void button3_Click(object sender, EventArgs e)
        {

            Global._01SetBasicData("bR3#H5uFg!aL", @"C:\Temp\AL\INTEGRATED TECH SOLUTIONS.p12", "http://80.90.92.219:2014/api/Values/GenerateIICAndIICSignature", "http://80.90.92.219:2014/api/Values/SignAndSOAPMessage", "http://80.90.92.219:2014/api/Values/SignUBL", "https://efiskalizimi-test.tatime.gov.al/FiscalizationService-v3", "https://Einvoice-test.tatime.gov.al/EinvoiceService-v1");

            Global._02GenerateIICAndIICSignature( "M11614005F|2025-12-04T10:06:40+01:00|69|nk877lp690||co442rd108|10000.00", @"C:\Temp\AL\IIC_request.xml", @"C:\Temp\AL\IIC_response.xml");
            MessageBox.Show( Global._03GetIIC());
            MessageBox.Show(Global._04GetIICSignature());

            InvoiceClass inv = new InvoiceClass();

            // 3) Neue Rechnung starten
            inv._30NewInvoice();

            // 4) Header / Invoice-Daten
            string uuid = Guid.NewGuid().ToString();
            DateTime now = DateTime.Now;

            inv._31InvoiceData(uuid, Global.TruncateToSeconds(now).ToString(), "NONCASH", Global.TruncateToSeconds(now).ToString(), "1/2025/ab001cd002", "1", Global.iic,Global.iicSignature, false, false, "EUR", 1.0, false, "bm679fd207", "nk877lp690", "", "co442rd108");

            // 5) Seller setzen
            inv._32SetSeller("NUIS", "L71862031", "STIHL Albania Sh.p.k.", "Rruga Dëshmorët e 4 Shkurtit", "Tirana", "ALB");

            // 6) Buyer setzen
            inv._34SetBuyer("NUIS", "K12345678", "Demo Customer SHPK", "Demo Street 1", "Tirana", "ALB");

            // 7) Position hinzufügen
            inv._35AddLine("Demo-Artikel", "A0001", "pcs", 2.00, 100.00, 20.00, 20.00, 120.00, false,"TYPE_2");

            // 8) Zahlungsart hinzufügen
            inv._36AddPayMethod("TRANSFER", 120.0, "");

            // 9) Unsigned XML speichern
            // inv._41SaveInvoiceRequest(@"C:\Temp\AL\al_unsigned.xml");

            // 10) SignAndSOAPMessage aufrufen (JSON Request / Response loggen)
            //inv._62SignSoapRequest(@"C:\Temp\AL\sign_request.json", @"C:\Temp\AL\sign_response.xml");

            // 11) Signierte SOAP an FiscalizationService schicken
            inv._41SaveInvoiceRequest(@"C:\Temp\AL\InvoiceSoapRequest.xml");

            inv._42SignSoapRequest(@"C:\Temp\AL\soap_request_signed.xml", @"C:\Temp\AL\soap_response_Signed.xml");

            inv._43InvoiceSendRequest(@"C:\Temp\AL\Invoice_soap_request.xml", @"C:\Temp\AL\Invoice_Soap_response.xml");



        }

        private void button4_Click(object sender, EventArgs e)
        {

            /*
            Global._01SetBasicData("bR3#H5uFg!aL", @"C:\Temp\AL\INTEGRATED TECH SOLUTIONS.p12", "http://80.90.92.219:2014/api/Values/GenerateIICAndIICSignature", "http://80.90.92.219:2014/api/Values/SignAndSOAPMessage", "http://80.90.92.219:2014/api/Values/SignUBL", "https://efiskalizimi-test.tatime.gov.al/FiscalizationService-v3", "https://Einvoice-test.tatime.gov.al/EinvoiceService-v1");

            Global._02GenerateIICAndIICSignature("M11614005F|2025-12-04T10:06:40+01:00|69|nk877lp690||co442rd108|10000.00", @"C:\Temp\AL\IIC_request.xml", @"C:\Temp\AL\IIC_response.xml");
            MessageBox.Show(Global._03GetIIC());
            MessageBox.Show(Global._04GetIICSignature());
            */

            var einv = new EInvoiceClass();
            


            einv.test();

            DateTime now = DateTime.Now;
            DateTime nowTrunc = Global.TruncateToSeconds(now);
            string uuid = Guid.NewGuid().ToString();

            einv._50NewInvoice();

            // 2) Kopf / InvoiceData
            einv._51InvoiceData(uuid, nowTrunc.ToString(), "2.1", "1/2025/ab001cd002", nowTrunc.ToString("yyyy-MM-dd"),nowTrunc.ToString("yyyy-MM-dd"), "UN/ECE 1001 Subset", "6", "380", "ISO 4217 Alpha", "6", "EUR", "ALL");

            einv._52SetAlFiscalNotes(new DateTime(2025, 12, 4, 16, 13, 47, DateTimeKind.Local).ToString(), "bm679fd207", "nk877lp690", "co442rd108", false, 10000.00, 10000.00, 10000.00, uuid, new DateTime(2025, 12, 4).ToString(),"45435345","3454354353453543453453453");


            // 3) Seller
            einv._53AccSupParty("GLN", "9", "1234567890123", "NUIS", "L71862031", "STIHL Albania Sh.p.k.", "GLN", "9", "1231412341324", "Rruga Dëshmorët e 4 Shkurtit", "", "Tirana", "1001", "ISO3166-1", "6", "AL");
            einv._54AccSupPartyTaxSch("ALVAT", "ZZZ", "L71862031", "UN/ECE 5153", "6", "VAT", "STIHL Albania Sh.p.k.", "NUIS", "ZZZ", "L71862031", "Tirana", "AL");

            // 4) Buyer
            einv._55AccCustParty("GLN", "9", "9876543210987", "NUIS", "K12345678", "Demo Customer SHPK", "GLN", "9", "9876543210000", "Demo Street 1", "", "Tirana", "1001", "ISO3166-1", "6", "AL");
            einv._56AccCustPartyTaxSch("ALVAT", "ZZZ", "K12345678", "UN/ECE 5153", "6", "VAT", "Demo Customer SHPK", "NUIS", "ZZZ", "K12345678", "Tirana", "AL");

            // 5) Payee (Zahlungsempfänger)
            einv._57PayeeParty("NUIS", "ZZZ", "L71862031", "STIHL Albania Sh.p.k.", "NUIS", "ZZZ", "L71862031");

            // 6) Delivery
            einv._58Delivery(nowTrunc.ToString("yyyy-MM-dd"), "GLN", "9", "1231412341324", "Rruga Dëshmorët e 4 Shkurtit", "", "Tirana", "1001", "AL");

            // 7) Zahlungsart
            einv._59PaymentMeans("UN/ECE 4461", "31", nowTrunc.AddDays(14).ToString("yyyy-MM-dd"), "IBAN", "Payref1", "AL1212341234123412", "ALSTB000","Name");

            // 8) TaxTotal + TaxSubtotal (100 netto, 20 Steuer)
            einv._60TaxTotal("EUR", 20.0);
            einv._61AddTaxSubTotal("EUR", 100.0, "EUR", 20.00, "UN/ECE 5305", "6", "S", 20.00, "UN/ECE 5153", "6", "VAT","TYPE_2","TEST");

            // 9) LegalMonetary (100 netto, 20 Steuer, 120 brutto)
            einv._63LegalMonetary("10",10,"EUR", 100.0, "EUR", 100.0, "EUR", 120.0, "EUR", 0.0, "EUR", 0.0, "EUR", 0.0, "EUR", 120.0);

            // 10) Zeile + Item + AllowanceCharge
            einv._64AddInvoiceLine("1", "pcs", 2.0, "EUR", 100.0, "", "1", "EUR", 20.0);
            einv._65AddInvLineItem("Demo-Artikel", "A0001", "GTIN", "9", "1234567890124", "UN/ECE 5305", "6", "S", 20.0, "UN/ECE 5153", "6", "VAT", "EUR", 50.0, "pcs", 1.0, "TYPE_2", "TEST");
            einv._66AddInvLineItemAllowCharge(0.0, "EUR", 0.0);

            // 11) UBL speichern
            einv._67SaveInvoice(@"C:\Temp\AL\UBLINvoice.xml");

            // 12) UBL signieren (Request / Response loggen)
            einv._68SignUBLInvoice(@"C:\Temp\AL\1sign_UBL_request.json", @"C:\Temp\AL\2sign_UBL_response.json");
            einv._69SignSoapRequest(@"C:\Temp\AL\3sign_UBL_SOAP_request.json", @"C:\Temp\AL\4sign_UBL_SOAP_response.json");


            // 13) Signierte SOAP an EinvoiceService senden
            string eic = einv._70InvoiceSendRequest(uuid, @"C:\Temp\AL\einvoice_soap_request.xml", @"C:\Temp\AL\einvoice_soap_response.xml");

            Console.WriteLine("EInvoice-Service EIC: " + eic);

            /*
            EInvoiceClass eInvoiceClass = new EInvoiceClass();
            eInvoiceClass._50NewInvoice();
            eInvoiceClass._invoice = eInvoiceClass.Create();
            eInvoiceClass._66SaveInvoice("c:\\temp\\AL\\UBLINvoice.xml");
            eInvoiceClass._67SignEInvoiceRequest(@"C:\Temp\AL\ubl_Sign_request.xml", @"C:\Temp\AL\ubl_Sign_response.xml");

            eInvoiceClass._68InvoiceSendRequest("351249D647C96C1247131D7D82045F21", @"C:\Temp\AL\ubl_request.xml", @"C:\Temp\AL\ubl_response.xml");
            */


        }

        private void button5_Click(object sender, EventArgs e)
        {
            var einv = new ECreditNoteClass();



            

            DateTime now = DateTime.Now;
            DateTime nowTrunc = Global.TruncateToSeconds(now);
            string uuid = Guid.NewGuid().ToString();

            einv._350NewCreditNote();

            // 2) Kopf / InvoiceData
            einv._351CreditNoteData(uuid, nowTrunc.ToString(), "2.1", "1/2025/ab001cd002", nowTrunc.ToString("yyyy-MM-dd"), nowTrunc.ToString("yyyy-MM-dd"), "UN/ECE 1001 Subset", "6", "380", "ISO 4217 Alpha", "6", "EUR", "ALL","dfasdf","2025-01-01");

           //einv._52SetAlFiscalNotes(new DateTime(2025, 12, 4, 16, 13, 47, DateTimeKind.Local).ToString(), "bm679fd207", "nk877lp690", "co442rd108", false, 10000.00, 10000.00, 10000.00, uuid, new DateTime(2025, 12, 4).ToString(), "45435345", "3454354353453543453453453");

        }
    }
}

