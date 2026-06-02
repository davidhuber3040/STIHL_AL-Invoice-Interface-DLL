using AL_Invoice_Interface_DLL.eInvoiceService;
using AL_Invoice_Interface_DLL.financeService;
using Stihl.Albania.Fiscalization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using UblSharp;
using UblSharp.CommonAggregateComponents;
using UblSharp.CommonExtensionComponents;
using UblSharp.UnqualifiedDataTypes;


namespace AL_Invoice_Interface_DLL
{
    public class ECreditNoteClass
    {

        RegisterEinvoiceRequest _request = new RegisterEinvoiceRequest();
        public UblSharp.CreditNoteType _creditnote = new UblSharp.CreditNoteType();
        public string _signedUBLInvoice = "";

        public string _signedSoapRequest = "";




        public void _350NewCreditNote()
        {
  
            _request = new RegisterEinvoiceRequest();
            _creditnote = new CreditNoteType();
            _request.Header=new RegisterEinvoiceRequestHeaderType();
            _request.EinvoiceEnvelope=new EinvoiceEnvelopeType();
            _signedSoapRequest = "";
            _signedUBLInvoice = "";

            string uuid = Guid.NewGuid().ToString();
            _request.Header.UUID = uuid;
            _request.Header.SendDateTime=Global.TruncateToSeconds(DateTime.Now).ToString();


        }

        public void _351CreditNoteData(string uuid, string sendDateTime, string UBLVersionID, string ID,string IssueDate, string DueDate, string InvTypelistID, string InvTypelistAgencyID, string InvTypeValue, string DocumentCurrencyCodelistID, string DocumentCurrencyCodelistAgencyID, string DocumentCurrencyCodeValue,string TaxCurrencyCode, string BillingReferenceID, string BillingReferenceIssueDate)
        {

            _request.Header.UUID = uuid;
            _request.Header.SendDateTime = sendDateTime;


            var doc = new XmlDocument();
            var sigInfo = doc.CreateElement(
                "SignatureInformation",
                "urn:oasis:names:specification:ubl:schema:xsd:SignatureAggregateComponents-2"
            );



            UBLExtensionType ublExt = new UBLExtensionType();
            ublExt.ExtensionContent = sigInfo;

            _creditnote.UBLExtensions = new List<UBLExtensionType>();
            _creditnote.UBLExtensions.Add(ublExt);

            _creditnote.CustomizationID = "urn:cen.eu:en16931:2017";

            _creditnote.UBLVersionID = UBLVersionID=="" ? null : UBLVersionID;
            _creditnote.ID = ID;
            _creditnote.IssueDate = IssueDate;
           // _creditnote.due = DueDate;


            _creditnote.ProfileID = "P2";

            _creditnote.TaxCurrencyCode = TaxCurrencyCode;

            _creditnote.BillingReference = new List<BillingReferenceType>()
            {
                new BillingReferenceType
                {
                    InvoiceDocumentReference = new DocumentReferenceType()
                    {
                        ID = BillingReferenceID,
                        IssueDate = BillingReferenceIssueDate
                    },
                }

            };



            _creditnote.CreditNoteTypeCode = new CodeType
            {
                listID = string.IsNullOrWhiteSpace(InvTypelistID) ?  null : InvTypelistID,
                listAgencyID = string.IsNullOrWhiteSpace(InvTypelistAgencyID) ? null : InvTypelistAgencyID,
                Value = string.IsNullOrWhiteSpace(InvTypeValue) ? null : InvTypeValue,
            };
            //_creditnote.TaxPointDate = TaxPointDate;
            _creditnote.DocumentCurrencyCode = new CodeType
            {
                listID = string.IsNullOrWhiteSpace(DocumentCurrencyCodelistID) ?  null : DocumentCurrencyCodelistID,
                listAgencyID = string.IsNullOrWhiteSpace(DocumentCurrencyCodelistAgencyID) ? null : DocumentCurrencyCodelistAgencyID,
                Value = string.IsNullOrWhiteSpace(DocumentCurrencyCodeValue) ? null :  DocumentCurrencyCodeValue,
            };


            /*
            _creditnote.OrderReference = new OrderReferenceType
            {
                ID = OrderReferenceID,
            };*/

           
            _creditnote.Xmlns = new System.Xml.Serialization.XmlSerializerNamespaces(new[]
           {
                new XmlQualifiedName("cac","urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"),
                new XmlQualifiedName("cbc","urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"),
            });

        }


        public void _352SetAlFiscalNotes(string issueDateTime, string operatorCode, string businessUnitCode, string softwareCode, bool isBadDebtInv, double amountWoVatLek, double amountWithVatLek, double sumOfTaxableAmountLek, string fic, string taxPointDate, string IIC, string iicSignature)
        {
            string issueDateTimeStr = issueDateTime;//.ToString("yyyy-MM-dd'T'HH:mm:sszzz", CultureInfo.InvariantCulture);
            string badDebtStr = isBadDebtInv ? "true" : "false";
            string amountWoVatStr = amountWoVatLek.ToString("0.00", CultureInfo.InvariantCulture);
            string amountWithVatStr = amountWithVatLek.ToString("0.00", CultureInfo.InvariantCulture);
            string sumTaxableStr = sumOfTaxableAmountLek.ToString("0.00", CultureInfo.InvariantCulture);

            _creditnote.Note = new List<TextType>
    {
        new TextType { Value = $"IssueDateTime={issueDateTimeStr}#AAI#" },
        new TextType { Value = $"OperatorCode={operatorCode}" },
        new TextType { Value = $"BusinessUnitCode={businessUnitCode}#AAI#" },
        new TextType { Value = $"SoftwareCode={softwareCode}#AAI#" },
        new TextType { Value = $"IsBadDebtInv={badDebtStr}#AAI#" },
        new TextType { Value = $"AmountWoVatLek={amountWoVatStr}#AAI#" },
        new TextType { Value = $"AmountWithVatLek={amountWithVatStr}#AAI#" },
        new TextType { Value = $"SumOfTaxableAmountLek={sumTaxableStr}#AAI#" },
        new TextType { Value = $"IIC={IIC}#AAI#" },
        new TextType { Value = $"FIC={fic}" },
        new TextType { Value = $"IICSignature={iicSignature}#AAI#" }
    };

            // <cbc:TaxPointDate>2025-12-04</cbc:TaxPointDate>
            _creditnote.TaxPointDate = taxPointDate;//.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            _creditnote.InvoicePeriod = new List<PeriodType>()
                {
                    new PeriodType
                    {
                        StartDate = taxPointDate,
                        EndDate = taxPointDate,

                    }
                };


        }



        public void _353AccSupParty(string EndpointID, string EndpointAgencyID, string EndpointValue, string PartyIdentificationID, string PartyIdentificationValue, string Name, string PostalAddressSchemeID, string PostalAddressSchemeAgencyID, string PostalAddressSchemeValue, string StreetName, string AdditionalStreetName, string CityName, string PostalZone, string CountrylistID, string CountrylistAgencyID, string CountrylistValue)

        {

            _creditnote.AccountingSupplierParty = new SupplierPartyType
            {
                Party = new PartyType
                {
                    EndpointID = new IdentifierType
                    {
                         schemeID = string.IsNullOrWhiteSpace(EndpointID) ? null : EndpointID,
           
                        schemeAgencyID = string.IsNullOrWhiteSpace(EndpointAgencyID) ? null :  EndpointAgencyID,
                        Value = EndpointValue
                    },
                    PartyIdentification = new List<PartyIdentificationType>()
                    {
                        new PartyIdentificationType
                        {
                            ID = new IdentifierType()
                            {

                                schemeID = string.IsNullOrWhiteSpace(PartyIdentificationID) ? null :  PartyIdentificationID,
                                Value = string.IsNullOrWhiteSpace(PartyIdentificationValue) ? null : PartyIdentificationValue
                            }
                        }
                    },
                    PartyName = new List<PartyNameType>()
                    {
                        new PartyNameType
                        {
                            Name = Name
                        }
                    },
                    PostalAddress = new AddressType
                    {
                        ID = new IdentifierType
                        {
                            schemeID = string.IsNullOrWhiteSpace(PostalAddressSchemeAgencyID) ? null :  PostalAddressSchemeAgencyID,
                            schemeAgencyID = string.IsNullOrWhiteSpace(PostalAddressSchemeAgencyID) ? null :  PostalAddressSchemeAgencyID,
                            Value = string.IsNullOrWhiteSpace(PostalAddressSchemeValue) ? null :  PostalAddressSchemeValue
                        },
                        //Postbox = "5467",
                        StreetName = StreetName,
                        AdditionalStreetName = string.IsNullOrWhiteSpace(AdditionalStreetName) ? null :  AdditionalStreetName,
                        //BuildingNumber = "1",
                        //Department = "Revenue department",
                        CityName = string.IsNullOrWhiteSpace(CityName) ? null :  CityName,
                        PostalZone = string.IsNullOrWhiteSpace(PostalZone) ? null :  PostalZone,
                        //CountrySubentityCode = "RegionA",
                        Country = new CountryType
                        {
                            IdentificationCode = new CodeType
                            {
                                listID = string.IsNullOrWhiteSpace(CountrylistID) ? null : CountrylistID,
                                listAgencyID = string.IsNullOrWhiteSpace(CountrylistAgencyID) ? null : CountrylistAgencyID,
                                Value = string.IsNullOrWhiteSpace(CountrylistValue) ? null : CountrylistValue,
                            }
                        }
                    },
                }
            };
        }

        public void _354AccSupPartyTaxSch(string CompanyID, string CompanyAgencyID, string CompanyValue, string TaxSchemeID, string TaxSchemeAgencyID, string TaxSchemeValue, string PartyLegalRegistrationName, string PartyLegalID, string PartyLegalAgencyID, string PartyLegalValue, string PartyLegalRegisAddrCity, string PartyLegalRegisAddrCountryIdentCode)
        {
            

            _creditnote.AccountingSupplierParty.Party.PartyTaxScheme = new List<PartyTaxSchemeType>()
                        {
                            new PartyTaxSchemeType
                            {
                                CompanyID = new IdentifierType
                                {
                                    schemeID =  string.IsNullOrWhiteSpace(CompanyID) ? null : CompanyID,
                                    schemeAgencyID = string.IsNullOrWhiteSpace(CompanyAgencyID) ? null : CompanyAgencyID,
                                    Value = string.IsNullOrWhiteSpace(CompanyValue) ? null : CompanyValue
                                },
                                TaxScheme = new TaxSchemeType
                                {
                                    ID = new IdentifierType
                                    {
                                        schemeID = string.IsNullOrWhiteSpace(TaxSchemeID) ? null : TaxSchemeID,
                                        schemeAgencyID = string.IsNullOrWhiteSpace(TaxSchemeAgencyID) ? null : TaxSchemeAgencyID,
                                        Value = string.IsNullOrWhiteSpace(TaxSchemeValue) ? null : TaxSchemeValue
                                    }
                                }
                            }
                        };
            _creditnote.AccountingSupplierParty.Party.PartyLegalEntity = new List<PartyLegalEntityType>()
                        {
                            new PartyLegalEntityType
                            {
                                RegistrationName = PartyLegalRegistrationName,
                                CompanyID = new IdentifierType
                                {
                                    schemeID = string.IsNullOrWhiteSpace(PartyLegalID) ? null :PartyLegalID,
                                    schemeAgencyID = string.IsNullOrWhiteSpace(PartyLegalAgencyID) ? null : PartyLegalAgencyID,
                                    Value = string.IsNullOrWhiteSpace(PartyLegalValue) ? null : PartyLegalValue
                                },
                                RegistrationAddress = (PartyLegalRegisAddrCity=="" && PartyLegalRegisAddrCountryIdentCode=="") ? null : new AddressType
                                {
                                    CityName = PartyLegalRegisAddrCity,
                                    //CountrySubentity = "RegionA",
                                    Country = new CountryType
                                    {
                                        IdentificationCode = PartyLegalRegisAddrCountryIdentCode
                                    }
                                }

                            }

                };
        }

        public void _355AccCustParty(string EndpointID, string EndpointAgencyID, string EndpointValue, string PartyIdentificationID, string PartyIdentificationValue, string Name, string PostalAddressSchemeID, string PostalAddressSchemeAgencyID, string PostalAddressSchemeValue, string StreetName, string AdditionalStreetName, string CityName, string PostalZone, string CountrylistID, string CountrylistAgencyID, string CountrylistValue)

        {

            _creditnote.AccountingCustomerParty = new CustomerPartyType
            {
                Party = new PartyType
                {
                    EndpointID = new IdentifierType
                    {
                        schemeID = string.IsNullOrWhiteSpace(EndpointID) ? null : EndpointID,
                        schemeAgencyID = string.IsNullOrWhiteSpace(EndpointAgencyID) ? null : EndpointAgencyID,
                        Value = string.IsNullOrWhiteSpace(EndpointValue) ? null : EndpointValue
                    },
                    PartyIdentification = new List<PartyIdentificationType>()
                    {
                        new PartyIdentificationType
                        {
                            ID = new IdentifierType
                            {
                                schemeID = string.IsNullOrWhiteSpace(PartyIdentificationID) ? null : PartyIdentificationID,
                                Value = string.IsNullOrWhiteSpace(PartyIdentificationValue) ? null : PartyIdentificationValue
                            }
                        }
                    },
                    PartyName = new List<PartyNameType>()
                    {
                        new PartyNameType
                        {
                            Name = Name
                        }
                    },
                    PostalAddress = new AddressType
                    {
                        /*
                        ID = new IdentifierType
                        {
                            schemeID = string.IsNullOrWhiteSpace(PostalAddressSchemeAgencyID) ? null :  PostalAddressSchemeAgencyID,
                            schemeAgencyID = string.IsNullOrWhiteSpace(PostalAddressSchemeAgencyID) ? null :  PostalAddressSchemeAgencyID,
                            Value = PostalAddressSchemeValue
                        },*/

                        //Postbox = "5467",
                        StreetName = StreetName=="" ? null : StreetName,
                        AdditionalStreetName = AdditionalStreetName=="" ? null :AdditionalStreetName,
                        //BuildingNumber = "1",
                        //Department = "Revenue department",
                        CityName = CityName=="" ? null :CityName,
                        PostalZone = PostalZone=="" ? null : PostalZone,
                        //CountrySubentityCode = "RegionA",
                        Country = CountrylistValue=="" ? null : new CountryType
                        {
                            IdentificationCode = new CodeType
                            {
                                listID = string.IsNullOrWhiteSpace(CountrylistID) ? null :  CountrylistID,
                                listAgencyID = string.IsNullOrWhiteSpace(CountrylistAgencyID) ? null :  CountrylistAgencyID,
                                Value = CountrylistValue,
                            }
                        }
                    },
                }
            };
        }
        public void _356AccCustPartyTaxSch(string CompanyID, string CompanyAgencyID, string CompanyValue, string TaxSchemeID, string TaxSchemeAgencyID, string TaxSchemeValue, string PartyLegalRegistrationName, string PartyLegalID, string PartyLegalAgencyID, string PartyLegalValue, string PartyLegalRegisAddrCity, string PartyLegalRegisAddrCountryIdentCode)
        {

            _creditnote.AccountingCustomerParty.Party.PartyTaxScheme = new List<PartyTaxSchemeType>()
                        {
                            new PartyTaxSchemeType
                            {
                                CompanyID = new IdentifierType
                                {
                                    schemeID = string.IsNullOrWhiteSpace(CompanyID) ? null : CompanyID,
                                    schemeAgencyID = string.IsNullOrWhiteSpace(CompanyAgencyID) ? null : CompanyAgencyID,
                                    Value = CompanyValue
                                },
                                TaxScheme = new TaxSchemeType
                                {
                                    ID = new IdentifierType
                                    {
                                        schemeID = string.IsNullOrWhiteSpace(TaxSchemeID) ? null : TaxSchemeID,
                                        schemeAgencyID = string.IsNullOrWhiteSpace(TaxSchemeAgencyID) ? null : TaxSchemeAgencyID,
                                        Value = TaxSchemeValue
                                    }
                                }
                            }
                        };
            _creditnote.AccountingCustomerParty.Party.PartyLegalEntity = new List<PartyLegalEntityType>()
                        {
                            new PartyLegalEntityType
                            {
                                RegistrationName = PartyLegalRegistrationName=="" ? null : PartyLegalRegistrationName,
                                CompanyID =PartyLegalValue== ""? null : new IdentifierType
                                {
                                    schemeID = string.IsNullOrWhiteSpace(PartyLegalID) ? null : PartyLegalID,
                                    schemeAgencyID = string.IsNullOrWhiteSpace(PartyLegalAgencyID) ? null : PartyLegalAgencyID,
                                    Value = PartyLegalValue
                                },
                                RegistrationAddress =PartyLegalRegisAddrCountryIdentCode=="" && PartyLegalRegisAddrCity=="" ? null:  new AddressType
                                {
                                    CityName = PartyLegalRegisAddrCity=="" ? null :PartyLegalRegisAddrCity,
                                    //CountrySubentity = "RegionA",
                                    Country =PartyLegalRegisAddrCountryIdentCode=="" ? null :  new CountryType
                                    {
                                        IdentificationCode = PartyLegalRegisAddrCountryIdentCode
                                    }
                                }

                            }

                }; 
        }

        public void _357PayeeParty(string PartyIdentificationID, string PartyIdentificationAgencyID, string PartyIdentificationValue, string Name, string PartyLegalID, string PartyLegalAgencyID, string PartyLegaValue)

        {

            _creditnote.PayeeParty = new PartyType
            {
                PartyIdentification = new List<PartyIdentificationType>()
                {
                    new PartyIdentificationType
                    {
                        ID = new IdentifierType
                        {
                            schemeID = string.IsNullOrWhiteSpace(PartyIdentificationID) ? null : PartyIdentificationID,

                            Value = PartyIdentificationValue
                        }
                    }
                },
                PartyName = new List<PartyNameType>()
                {
                    new PartyNameType
                    {
                        Name = Name
                    }
                },
                PartyLegalEntity = new List<PartyLegalEntityType>()
                {
                    new PartyLegalEntityType
                    {
                        CompanyID = new IdentifierType
                        {
                            schemeID = string.IsNullOrWhiteSpace(PartyLegalID) ? null : PartyLegalID,
                            schemeAgencyID = string.IsNullOrWhiteSpace(PartyLegalAgencyID) ? null : PartyLegalAgencyID,
                            Value = PartyLegaValue
                        }
                    }
                },
            };
        }

        public void _358Delivery(string ActualDeliveryDate, string DeliveryLocationID, string DeliveryLocationAgencyID, string DeliveryLocationValue, string StreetName, string AdditionalStreetName, string CityName, string PostalZone, string CountryIdentificationCode)
        {

            _creditnote.Delivery = new List<DeliveryType>()
                {
                    new DeliveryType
                    {
                        ActualDeliveryDate = ActualDeliveryDate,
                        DeliveryLocation = new LocationType
                        {
                            ID = new IdentifierType
                            {
                                schemeID = string.IsNullOrWhiteSpace(DeliveryLocationID) ? null : DeliveryLocationID,
                                schemeAgencyID = string.IsNullOrWhiteSpace(DeliveryLocationAgencyID) ? null : DeliveryLocationAgencyID,
                                Value = DeliveryLocationValue
                            },
                            Address = new AddressType
                            {
                                StreetName = StreetName,
                                AdditionalStreetName = AdditionalStreetName,
                                //BuildingNumber = "12",
                                CityName = CityName,
                                PostalZone = PostalZone,
                                //CountrySubentity = "RegionC",
                                Country = new CountryType
                                {
                                    IdentificationCode = CountryIdentificationCode
                                }
                            }
                        }
                    }
        };

        }

        public void _359PaymentMeans(string PaymentMeansID, string PaymentMeansValue, string PaymentDueDate, string PaymentChannelCode, string PaymentIDValue, string PayeeFinancialAccountID, string PayeeFinancialInstID, string PayeeFinancialAccountName)
        {
            if (PaymentDueDate!="")
            {
                //_creditnote.DueDate=PaymentDueDate;
            }

            _creditnote.PaymentMeans = new List<PaymentMeansType>()
                {
                    new PaymentMeansType
                    {
                        PaymentMeansCode = PaymentMeansID=="" && PaymentMeansValue=="" ? null :  new CodeType
                        {
                            listID =string.IsNullOrWhiteSpace(PaymentMeansID) ? null : PaymentMeansID,
                            Value = string.IsNullOrWhiteSpace(PaymentMeansValue) ? null :PaymentMeansValue
                        },

                        PaymentDueDate = string.IsNullOrWhiteSpace(PaymentDueDate) ? null :PaymentDueDate,
                        PaymentChannelCode = string.IsNullOrWhiteSpace(PaymentChannelCode) ? null :PaymentChannelCode,
                        PaymentID = string.IsNullOrWhiteSpace(PaymentIDValue) ? null : new List<IdentifierType>()
                        {
                            new IdentifierType
                            {
                                Value = PaymentIDValue
                            }
                        },

                        PayeeFinancialAccount = PayeeFinancialAccountID=="" && PayeeFinancialAccountName=="" && PayeeFinancialInstID=="" ? null : new FinancialAccountType
                        {
                            ID = string.IsNullOrWhiteSpace(PayeeFinancialAccountID) ? null :PayeeFinancialAccountID,
                            Name= string.IsNullOrWhiteSpace(PayeeFinancialAccountName) ? null : PayeeFinancialAccountName,
                            FinancialInstitutionBranch =string.IsNullOrWhiteSpace(PayeeFinancialInstID) ? null : new BranchType
                            {
                                ID= PayeeFinancialInstID
                                /*
                                FinancialInstitution = PayeeFinancialInstID=="" ? null :  new FinancialInstitutionType
                                {
                                    ID = PayeeFinancialInstID
                                }*/
                            }
                        }
                    }
                };   
           
        }

        public void _360TaxTotal(string currencyID, double Value)
        {
            _creditnote.TaxTotal = new List<TaxTotalType>()
                {
                    new TaxTotalType
                    {
                        TaxAmount = new AmountType
                        {
                            currencyID = currencyID,
                            Value = (decimal) Value
                        },
                       
                    }
                };
        }
        public void _361AddTaxSubTotal(string TaxableAmountCurrencyID, double TaxableAmountValue, string TaxAmountCurrencyID, double TaxAmountValue, string TaxCategoryID, string TaxCategoryAgencyID, string TaxCategoryValue, double Percent, string TaxSchemeID, string TaxSchemeAgencyID, string TaxSchemeValue, string TaxExemptionReasonCode, string TaxExemptionReason)
        {
            if (_creditnote.TaxTotal.First().TaxSubtotal == null)
                _creditnote.TaxTotal.First().TaxSubtotal = new List<TaxSubtotalType>();
            TaxSubtotalType taxSub = new TaxSubtotalType
            {
                TaxableAmount = new AmountType
                {
                    currencyID = TaxableAmountCurrencyID,
                    Value = (decimal)TaxableAmountValue
                },
                TaxAmount = new AmountType
                {
                    currencyID = TaxAmountCurrencyID,
                    Value = (decimal)TaxAmountValue
                },
                TaxCategory = new TaxCategoryType
                {
                    ID = new IdentifierType
                    {
                        schemeID = string.IsNullOrWhiteSpace(TaxCategoryID) ? null :  TaxCategoryID,
                        schemeAgencyID = string.IsNullOrWhiteSpace(TaxCategoryAgencyID) ? null :  TaxCategoryAgencyID,
                        Value = TaxCategoryValue
                    },
                    Percent = (decimal)Percent,
                    TaxScheme = new TaxSchemeType
                    {
                        ID = new IdentifierType
                        {
                            schemeID = string.IsNullOrWhiteSpace(TaxSchemeID) ? null :  TaxSchemeID,
                            schemeAgencyID = string.IsNullOrWhiteSpace(TaxSchemeAgencyID) ? null :  TaxSchemeAgencyID,
                            Value = TaxSchemeValue
                        }
                    }
                }
            };


            if (!string.IsNullOrWhiteSpace(TaxExemptionReasonCode))
            {
                taxSub.TaxCategory.TaxExemptionReasonCode = new CodeType
                {
                    Value = TaxExemptionReasonCode
                };
            }

            if (!string.IsNullOrWhiteSpace(TaxExemptionReason))
            {
                taxSub.TaxCategory.TaxExemptionReason = new List<TextType>
        {
            new TextType
            {
                Value = TaxExemptionReason
            }
        };
            }



            _creditnote.TaxTotal.First().TaxSubtotal.Add(taxSub);


        }

        public void _362PaymentTermns(string Value)
        {
            _creditnote.PaymentTerms = new List<PaymentTermsType>()
                {
                    new PaymentTermsType
                    {
                        Note = new List<TextType>()
                        {
                            new TextType
                            {
                                Value = Value
                            }
                        },
                    }
                };
        }

        public void _363LegalMonetary(string LineExtensionID, double LineExtensionValue, string TaxExclusiveID, double TaxExclusiveValue, string TaxInclusiveID, double TaxInclusive, string AllowanceTotalID, double AllowanceTotalValue, string ChargeTotalID, double ChargeTotalValue, string PrepaidID, double PrepaidValue, string PayableRoundingID, double PayableRoundingValue, string PayableID, double PayableValue)
        {
            _creditnote.LegalMonetaryTotal = new MonetaryTotalType
            {
                LineExtensionAmount = new AmountType
                {
                    currencyID = LineExtensionID,
                    Value = (decimal)LineExtensionValue
                },
                TaxExclusiveAmount = new AmountType
                {
                    currencyID = LineExtensionID,
                    Value = (decimal)TaxExclusiveValue,
                },
                TaxInclusiveAmount = new AmountType
                {
                    currencyID = TaxInclusiveID,
                    Value = (decimal)TaxInclusive
                },
                AllowanceTotalAmount = new AmountType
                {
                    currencyID = AllowanceTotalID,
                    Value = (decimal)AllowanceTotalValue,
                },
                ChargeTotalAmount = new AmountType
                {
                    currencyID = ChargeTotalID,
                    Value = (decimal)ChargeTotalValue
                },
                PrepaidAmount = new AmountType
                {
                    currencyID = PrepaidID,
                    Value = (decimal)PrepaidValue
                },
                PayableRoundingAmount = new AmountType
                {
                    currencyID = PayableRoundingID,
                    Value = (decimal)PayableRoundingValue
                },
                PayableAmount = new AmountType
                {
                    currencyID = PayableID,
                    Value = (decimal)PayableValue
        } };
    }

        public void _364AddInvoiceLine(string ID, string InvQtyUnitCode, double InvQtyValue, string LineExtensionCurrencyID, double LineExtensionValue, string AccountingCost, string OrderLineReferenceID, string TaxTotalcurrencyID, double TaxTotalValue)
           
        {



            CreditNoteLineType line =
                  new CreditNoteLineType
                  {
                      ID = ID,
                     
                      /*Note = new List<TextType>()
                      {
                          new TextType
                          {
                              Value = Note
                          }
                      },*/
                      
                      CreditedQuantity = new QuantityType
                      {
                          unitCode = InvQtyUnitCode,
                          Value = (decimal)InvQtyValue
                      },
                      LineExtensionAmount = new AmountType
                      {
                          currencyID = LineExtensionCurrencyID,
                          Value = (decimal)LineExtensionValue,
                      },
                      AccountingCost = string.IsNullOrWhiteSpace(AccountingCost) ? null :  AccountingCost,
                      OrderLineReference = OrderLineReferenceID=="" ? null : new List<OrderLineReferenceType>()
                        {
                            new OrderLineReferenceType
                            {
                                LineID = OrderLineReferenceID
                            }
                        },
                      TaxTotal = TaxTotalValue==0 ? null :  new List<TaxTotalType>()
                        {
                            new TaxTotalType
                            {
                                TaxAmount = new AmountType
                                {
                                    currencyID = TaxTotalcurrencyID,
                                    Value = (decimal) TaxTotalValue
                                }
                            }
                        },
                      

                  };

            line.ID.schemeID = "0160";
            if (_creditnote.CreditNoteLine == null)
            {
                _creditnote.CreditNoteLine = new List<CreditNoteLineType>();
            }
            _creditnote.CreditNoteLine.Add(line);
  
            }
        
        public void _365AddInvLineItem(string Name,string SellersItemIdentification,string StandardItemIdentificationID,string StandardItemIdentificationAgencyID, string StandardItemIdentificationValue, string ClassifiedTaxCategoryID, string ClassifiedTaxCategoryAgencyID,string ClassifiedTaxCategoryValue,double ClassifiedTaxPercent,string TaxSchemeID, string TaxSchemeAgencyID,string TaxSchemeValue,string PriceAmountCurrId,double PriceAmountValue,string BaseQuantityUnitCode,double BaseQuantityValue, string TaxExemptionReasonCode, string TaxExemptionReason)
        {
            _creditnote.CreditNoteLine.LastOrDefault().Item = new ItemType
            {
              
                Name = Name,
                SellersItemIdentification = SellersItemIdentification=="" ? null : new ItemIdentificationType
                {
                    ID = SellersItemIdentification
                },
                StandardItemIdentification = (StandardItemIdentificationID=="" && StandardItemIdentificationAgencyID=="" && StandardItemIdentificationValue=="") ? null : new ItemIdentificationType
                {
                    ID = new IdentifierType
                    {
                        schemeID = string.IsNullOrWhiteSpace(StandardItemIdentificationID) ? null : StandardItemIdentificationID,
                        schemeAgencyID = string.IsNullOrWhiteSpace(StandardItemIdentificationAgencyID) ? null : StandardItemIdentificationAgencyID,
                        Value = string.IsNullOrWhiteSpace(StandardItemIdentificationValue) ? null :  StandardItemIdentificationValue
                    }
                },
              
                ClassifiedTaxCategory = new List<TaxCategoryType>()
                            {
                                new TaxCategoryType
                                {
                                    ID = new IdentifierType
                                    {
                                        schemeID = string.IsNullOrWhiteSpace(ClassifiedTaxCategoryID) ? null : ClassifiedTaxCategoryID,
                                        schemeAgencyID = string.IsNullOrWhiteSpace(ClassifiedTaxCategoryAgencyID) ? null : ClassifiedTaxCategoryAgencyID,
                                        Value = string.IsNullOrWhiteSpace(ClassifiedTaxCategoryValue) ? null :  ClassifiedTaxCategoryValue
                                    },
                                    Percent = (decimal) ClassifiedTaxPercent,
                                     TaxExemptionReasonCode = string.IsNullOrWhiteSpace(TaxExemptionReasonCode) ? null : new CodeType
                                    {
                                        Value = TaxExemptionReasonCode
                                    },

                                    TaxExemptionReason = string.IsNullOrWhiteSpace(TaxExemptionReason) ? null : new List<TextType>
                                    {
                                        new TextType
                                        {
                                            Value = TaxExemptionReason
                                        }
                                    },
                                    TaxScheme = new TaxSchemeType
                                    {
                                        ID = new IdentifierType
                                        {
                                            schemeID = string.IsNullOrWhiteSpace(TaxSchemeID) ? null :  TaxSchemeID,
                                            schemeAgencyID = string.IsNullOrWhiteSpace(TaxSchemeAgencyID) ? null :  TaxSchemeAgencyID,
                                            Value = string.IsNullOrWhiteSpace(TaxSchemeValue) ? null :  TaxSchemeValue
                                        }
                                    }
                                }
                            },
               
            };
            _creditnote.CreditNoteLine.LastOrDefault().Price = new PriceType
            {
                PriceAmount = new AmountType
                {
                    currencyID = PriceAmountCurrId,
                    Value = (decimal)PriceAmountValue,
                },
                BaseQuantity = BaseQuantityValue==0 ? null : new QuantityType
                {
                    unitCode = BaseQuantityUnitCode,
                    Value = (decimal)BaseQuantityValue
                }            };
        }

        public void _366AddInvLineItemAllowCharge(double MultiplierFactorNumeric, string currencyID, double Amount)
        {
            _creditnote.CreditNoteLine.LastOrDefault().AllowanceCharge = new List<AllowanceChargeType>()
            {
                new AllowanceChargeType
                {
                    ChargeIndicator = false,
                    MultiplierFactorNumeric = (decimal) MultiplierFactorNumeric,
                    Amount = new AmountType
                    {
                        currencyID = currencyID,
                        Value = (decimal) Amount
                    }
                       
                }
                
            };
        }


       
        public void _367SaveInvoice(string FilePath)
        {
            string xml=GetUBLXml();
        
            File.WriteAllText(FilePath, xml);
        }


        public void _368SignUBLInvoice(string RequestFilePathToSave, string ResponseFilePathToSave)
        {
            // unsigned XML → Base64
            string xmlBase64 = Convert.ToBase64String(Encoding.ASCII.GetBytes(GetUBLXml()));


            var req = new Global.SignRequest { certPass = Global.CertificatePassword, certContent = Global.FileToBase64(Global.CertificatePath), reqPayload = xmlBase64 };
            string jsonRequest = JsonSerializer.Serialize(req);

            // Request optional speichern
            if (!string.IsNullOrEmpty(RequestFilePathToSave))
                File.WriteAllText(RequestFilePathToSave, jsonRequest, Encoding.ASCII);

            string rawResponse = "";
            Global.HttpPostJson(Global.SignUblUrl, jsonRequest, ref rawResponse);


            // Response optional speichern
            if (!string.IsNullOrEmpty(ResponseFilePathToSave))
                File.WriteAllText(ResponseFilePathToSave, rawResponse, Encoding.ASCII);


            var doc = JsonDocument.Parse(rawResponse);
            string signedSoapBase64 = doc.RootElement.GetProperty("SignedUBL").GetString();

            // Base64 → XML-String
            string signedSoapXml = Encoding.ASCII.GetString(Convert.FromBase64String(signedSoapBase64));

            // z.B. im InvoiceClass:
            _signedUBLInvoice = signedSoapXml;

        }

        public void _369SignSoapRequest(string RequestFilePathToSave, string ResponseFilePathToSave)
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
            _signedSoapRequest = signedSoapXml;


        }


        



        public string _370CreditNoteSendRequest(string requestUUID, string RequestFilePathToSave, string ResponseFilePathToSave)
        {


            string strRequest = _signedSoapRequest;


            if (RequestFilePathToSave != "")
            {
                File.WriteAllText(RequestFilePathToSave, strRequest, Encoding.ASCII);
            }

            string fiscalResponse = "";


            Global.HttpPostXml(Global.EInvoiceServiceUrl, strRequest, Global.FiscalizationServiceUrl, ref fiscalResponse);

            // 4) Response optional speichern
            if (ResponseFilePathToSave != "")
                File.WriteAllText(ResponseFilePathToSave, fiscalResponse, Encoding.ASCII);

            string FaultCode, FaultText, DetailCode = "";
            (FaultCode, FaultText, DetailCode)=  ReadSoapFault(fiscalResponse);

            string ret = FaultCode + " " + DetailCode + " " + FaultText;

            




            return ret.Trim();

            

            /*
            RegisterEinvoiceResponse EInvoiceResponse;
   
            var serializerResp = new XmlSerializer(typeof(RegisterEinvoiceResponse));
            using (var reader = new StringReader(fiscalResponse))
            {
                //TryReadSoapFault(reader);

                EInvoiceResponse = (RegisterEinvoiceResponse)serializerResp.Deserialize(reader);
            }

            return EInvoiceResponse.;
            */

            /*
            string ublXml;
            if (!string.IsNullOrEmpty(_signedUBLRequest))
                ublXml = _signedUBLRequest;       // Ergebnis von _65SignEInvoiceRequest
            else
                ublXml = BuildSoapEnvelope(GetXml());         // _creditnote.GetXml()

                      
            RegisterEinvoiceRequest request = new RegisterEinvoiceRequest();
            request.Header=new RegisterEinvoiceRequestHeaderType();
            request.Header.UUID = requestUUID;
            request.Header.SendDateTime = Global.TruncateToSeconds(DateTime.Now);

        


            string bodyXml = "";
            XmlSerializer serializer = new XmlSerializer(typeof(RegisterEinvoiceRequest));

            using (Utf8StringWriter sw = new Utf8StringWriter())
            {
                serializer.Serialize(sw, request);
                bodyXml = sw.ToString();
            }


            if (!string.IsNullOrEmpty(RequestFilePathToSave))
            {

               

                File.WriteAllText(RequestFilePathToSave, bodyXml, Encoding.UTF8);
            }


            string Response="";

            Global.HttpPostXml(Global.EInvoiceServiceUrl, bodyXml, Global.EInvoiceServiceUrl, ref Response);

            // 4) Response optional speichern
            if (ResponseFilePathToSave != "")
                File.WriteAllText(ResponseFilePathToSave, Response, Encoding.UTF8);


            RegisterEinvoiceResponse EInvoiceResponse;

            var serializerResp = new XmlSerializer(typeof(RegisterEinvoiceResponse));
            using (var reader = new StringReader(Response))
            {
                EInvoiceResponse = (RegisterEinvoiceResponse)serializerResp.Deserialize(reader);
            }

           

            // 4) Service aufrufen
            eInvoiceService.EinvoiceService srv = new eInvoiceService.EinvoiceService();

            srv.Url = Global.EInvoiceServiceUrl;

            RegisterEinvoiceResponse response;

            try
            {
                response = srv.registerEinvoice(request);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();

                sb.AppendLine("Top-Level Exception:");
                sb.AppendLine(ex.GetType().FullName);
                sb.AppendLine(ex.Message);
                sb.AppendLine(ex.StackTrace);

                if (ex.InnerException != null)
                {
                    sb.AppendLine();
                    sb.AppendLine("Inner Exception:");
                    sb.AppendLine(ex.InnerException.GetType().FullName);
                    sb.AppendLine(ex.InnerException.Message);
                    sb.AppendLine(ex.InnerException.StackTrace);
                }

                if (!string.IsNullOrEmpty(ResponseFilePathToSave))
                    File.WriteAllText(ResponseFilePathToSave, sb.ToString(), Encoding.UTF8);

                return "";
               
            }
            */



            // 6) EIC o.ä. merken und zurückgeben
            //
            //resp = EInvoiceResponse.EIC;
            //return EInvoiceResponse.EIC;


        }

        /*
        public string _68GetResponseStatus()
        {
            return resp;
        }
        */


        public string test()
        {
            string test = System.IO.File.ReadAllText(@"C:\Temp\AL\NEU\Response.txt");
            string FaultCode, FaultText, DetailCode = "";
            (FaultCode, FaultText, DetailCode) = ReadSoapFault(test);


            return FaultCode + FaultText + DetailCode;
        }



        public static (string FaultCode, string FaultText, string DetailCode) ReadSoapFault(string soapXml)
        {
            var doc = XDocument.Parse(soapXml);

            XNamespace soap = "http://schemas.xmlsoap.org/soap/envelope/";

            var fault = doc.Descendants(soap + "Fault").FirstOrDefault();
            if (fault == null) return (null, null, null);

            var faultCode = fault.Element("faultcode")?.Value;
            var faultText = fault.Element("faultstring")?.Value;
            var detailCode = fault.Element("detail")?.Element("code")?.Value;

            return (faultCode, faultText, detailCode);
        }



        public sealed class SoapFaultInfo
        {
            public string FaultCode { get; set; }
            public string FaultString { get; set; }
            public string DetailCode { get; set; }
            public string RequestUUID { get; set; }
            public string ResponseUUID { get; set; }
        }

        public static SoapFaultInfo TryReadSoapFault(string soapXml)
        {
            var doc = XDocument.Parse(soapXml, LoadOptions.None);
            XNamespace soap = "http://schemas.xmlsoap.org/soap/envelope/";

            var fault = doc.Descendants(soap + "Fault").FirstOrDefault();
            if (fault == null) return null;

            return new SoapFaultInfo
            {
                FaultCode = (string)fault.Element("faultcode"),
                FaultString = (string)fault.Element("faultstring"),
                DetailCode = (string)fault.Element("detail")?.Element("code"),
                RequestUUID = (string)fault.Element("detail")?.Element("requestUUID"),
                ResponseUUID = (string)fault.Element("detail")?.Element("responseUUID")
            };
        }


        private string GetUBLXml()
        {

            MemoryStream str = new MemoryStream();
            _creditnote.Save(str);
            str.Position = 0;
            StreamReader reader = new StreamReader(str);
            string text = reader.ReadToEnd();
            text=text.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>","");

            text = text.Replace("xmlns:sbc=\"urn:oasis:names:specification:ubl:schema:xsd:SignatureBasicComponents-2\" xmlns:sig=\"urn:oasis:names:specification:ubl:schema:xsd:CommonSignatureComponents-2\"", "");




            return text;    

            /*
            _request.Invoice = _creditnote;

            if (_request == null)
                throw new InvalidOperationException("Es wurde noch kein RegisterInvoiceRequest aufgebaut. Bitte zuerst _50NewInvoice usw. aufrufen.");

            const string fiscalNs = "https://eFiskalizimi.tatime.gov.al/FiscalizationService/schema";

            // WICHTIG: XmlRootAttribute mit Namespace übergeben
            var root = new XmlRootAttribute("RegisterInvoiceRequest");
            root.Namespace = fiscalNs;

            XmlSerializer serializer = new XmlSerializer(typeof(RegisterInvoiceRequest), root);

            using (Utf8StringWriter sw = new Utf8StringWriter())
            {
                serializer.Serialize(sw, _request);
                return sw.ToString();
            }*/
        }


        private string GetRequestXml()
        {

            _request.EinvoiceEnvelope = new EinvoiceEnvelopeType();
            _request.EinvoiceEnvelope.ItemElementName = ItemChoiceType.UblCreditNote;
            _request.EinvoiceEnvelope.Item = Encoding.ASCII.GetBytes(_signedUBLInvoice);//Encoding.UTF8.GetBytes(GetUBLXml());
        

            if (_request == null)
                throw new InvalidOperationException("Es wurde noch kein RegisterInvoiceRequest aufgebaut. Bitte zuerst _30NewInvoice usw. aufrufen.");

            string fiscalNs = "https://Einvoice.tatime.gov.al/EinvoiceService/schema";


            // WICHTIG: XmlRootAttribute mit Namespace übergeben
            var root = new XmlRootAttribute("RegisterEinvoiceRequest");
            root.Namespace = fiscalNs;

            XmlSerializer serializer = new XmlSerializer(typeof(RegisterEinvoiceRequest), root);

            using (ASCIIStringWriter sw = new ASCIIStringWriter())
            {
                serializer.Serialize(sw, _request);
                return sw.ToString();
            }
        }




        private class ASCIIStringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.ASCII;
        }

      
    }



}
