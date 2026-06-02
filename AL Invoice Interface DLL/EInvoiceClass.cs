using AL_Invoice_Interface_DLL.eInvoiceService;
using AL_Invoice_Interface_DLL.financeService;
using Stihl.Albania.Fiscalization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using InvoiceType = UblSharp.InvoiceType;


namespace AL_Invoice_Interface_DLL
{
    public class EInvoiceClass
    {

        RegisterEinvoiceRequest _request = new RegisterEinvoiceRequest();
        public UblSharp.InvoiceType _invoice = new UblSharp.InvoiceType();
        public string _signedUBLInvoice = "";

        public string _signedSoapRequest = "";








        public void _50NewInvoice()
        {
            _request = new RegisterEinvoiceRequest();
            _invoice = new InvoiceType();
            _request.Header=new RegisterEinvoiceRequestHeaderType();
            _request.EinvoiceEnvelope=new EinvoiceEnvelopeType();
            _signedSoapRequest = "";
            _signedUBLInvoice = "";

            string uuid = Guid.NewGuid().ToString();
            _request.Header.UUID = uuid;
            _request.Header.SendDateTime=Global.TruncateToSeconds(DateTime.Now).ToString();


        }

        public void _51InvoiceData(string uuid, string sendDateTime, string UBLVersionID, string ID,string IssueDate, string DueDate, string InvTypelistID, string InvTypelistAgencyID, string InvTypeValue, string DocumentCurrencyCodelistID, string DocumentCurrencyCodelistAgencyID, string DocumentCurrencyCodeValue,string TaxCurrencyCode)
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

            _invoice.UBLExtensions = new List<UBLExtensionType>();
            _invoice.UBLExtensions.Add(ublExt);

            _invoice.CustomizationID = "urn:cen.eu:en16931:2017";

            _invoice.UBLVersionID = UBLVersionID=="" ? null : UBLVersionID;
            _invoice.ID = ID;
            _invoice.IssueDate = IssueDate;
            _invoice.DueDate = DueDate;


            _invoice.ProfileID = "P2";

            _invoice.TaxCurrencyCode = TaxCurrencyCode;
      


            _invoice.InvoiceTypeCode = new CodeType
            {
                listID = string.IsNullOrWhiteSpace(InvTypelistID) ?  null : InvTypelistID,
                listAgencyID = string.IsNullOrWhiteSpace(InvTypelistAgencyID) ? null : InvTypelistAgencyID,
                Value = string.IsNullOrWhiteSpace(InvTypeValue) ? null : InvTypeValue,
            };
            //_invoice.TaxPointDate = TaxPointDate;
            _invoice.DocumentCurrencyCode = new CodeType
            {
                listID = string.IsNullOrWhiteSpace(DocumentCurrencyCodelistID) ?  null : DocumentCurrencyCodelistID,
                listAgencyID = string.IsNullOrWhiteSpace(DocumentCurrencyCodelistAgencyID) ? null : DocumentCurrencyCodelistAgencyID,
                Value = string.IsNullOrWhiteSpace(DocumentCurrencyCodeValue) ? null :  DocumentCurrencyCodeValue,
            };


            /*
            _invoice.OrderReference = new OrderReferenceType
            {
                ID = OrderReferenceID,
            };*/

           
            _invoice.Xmlns = new System.Xml.Serialization.XmlSerializerNamespaces(new[]
           {
                new XmlQualifiedName("cac","urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"),
                new XmlQualifiedName("cbc","urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"),
            });

        }


        public void _52SetAlFiscalNotes(string issueDateTime, string operatorCode, string businessUnitCode, string softwareCode, bool isBadDebtInv, double amountWoVatLek, double amountWithVatLek, double sumOfTaxableAmountLek, string fic, string taxPointDate, string IIC, string iicSignature)
        {
            string issueDateTimeStr = issueDateTime;//.ToString("yyyy-MM-dd'T'HH:mm:sszzz", CultureInfo.InvariantCulture);
            string badDebtStr = isBadDebtInv ? "true" : "false";
            string amountWoVatStr = amountWoVatLek.ToString("0.00", CultureInfo.InvariantCulture);
            string amountWithVatStr = amountWithVatLek.ToString("0.00", CultureInfo.InvariantCulture);
            string sumTaxableStr = sumOfTaxableAmountLek.ToString("0.00", CultureInfo.InvariantCulture);

            _invoice.Note = new List<TextType>
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
            _invoice.TaxPointDate = taxPointDate;//.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            _invoice.InvoicePeriod = new List<PeriodType>()
                {
                    new PeriodType
                    {
                        StartDate = taxPointDate,
                        EndDate = taxPointDate,

                    }
                };


        }



        public void _53AccSupParty(string EndpointID, string EndpointAgencyID, string EndpointValue, string PartyIdentificationID, string PartyIdentificationValue, string Name, string PostalAddressSchemeID, string PostalAddressSchemeAgencyID, string PostalAddressSchemeValue, string StreetName, string AdditionalStreetName, string CityName, string PostalZone, string CountrylistID, string CountrylistAgencyID, string CountrylistValue)

        {

            _invoice.AccountingSupplierParty = new SupplierPartyType
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

        public void _54AccSupPartyTaxSch(string CompanyID, string CompanyAgencyID, string CompanyValue, string TaxSchemeID, string TaxSchemeAgencyID, string TaxSchemeValue, string PartyLegalRegistrationName, string PartyLegalID, string PartyLegalAgencyID, string PartyLegalValue, string PartyLegalRegisAddrCity, string PartyLegalRegisAddrCountryIdentCode)
        {
            

            _invoice.AccountingSupplierParty.Party.PartyTaxScheme = new List<PartyTaxSchemeType>()
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
            _invoice.AccountingSupplierParty.Party.PartyLegalEntity = new List<PartyLegalEntityType>()
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

        public void _55AccCustParty(string EndpointID, string EndpointAgencyID, string EndpointValue, string PartyIdentificationID, string PartyIdentificationValue, string Name, string PostalAddressSchemeID, string PostalAddressSchemeAgencyID, string PostalAddressSchemeValue, string StreetName, string AdditionalStreetName, string CityName, string PostalZone, string CountrylistID, string CountrylistAgencyID, string CountrylistValue)

        {

            _invoice.AccountingCustomerParty = new CustomerPartyType
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
        public void _56AccCustPartyTaxSch(string CompanyID, string CompanyAgencyID, string CompanyValue, string TaxSchemeID, string TaxSchemeAgencyID, string TaxSchemeValue, string PartyLegalRegistrationName, string PartyLegalID, string PartyLegalAgencyID, string PartyLegalValue, string PartyLegalRegisAddrCity, string PartyLegalRegisAddrCountryIdentCode)
        {

            _invoice.AccountingCustomerParty.Party.PartyTaxScheme = new List<PartyTaxSchemeType>()
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
            _invoice.AccountingCustomerParty.Party.PartyLegalEntity = new List<PartyLegalEntityType>()
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

        public void _57PayeeParty(string PartyIdentificationID, string PartyIdentificationAgencyID, string PartyIdentificationValue, string Name, string PartyLegalID, string PartyLegalAgencyID, string PartyLegaValue)

        {

            _invoice.PayeeParty = new PartyType
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

        public void _58Delivery(string ActualDeliveryDate, string DeliveryLocationID, string DeliveryLocationAgencyID, string DeliveryLocationValue, string StreetName, string AdditionalStreetName, string CityName, string PostalZone, string CountryIdentificationCode)
        {

            _invoice.Delivery = new List<DeliveryType>()
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

        public void _59PaymentMeans(string PaymentMeansID, string PaymentMeansValue, string PaymentDueDate, string PaymentChannelCode, string PaymentIDValue, string PayeeFinancialAccountID, string PayeeFinancialInstID, string PayeeFinancialAccountName)
        {
            if (PaymentDueDate!="")
            {
                _invoice.DueDate=PaymentDueDate;
            }

            _invoice.PaymentMeans = new List<PaymentMeansType>()
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

        public void _60TaxTotal(string currencyID, double Value)
        {
            _invoice.TaxTotal = new List<TaxTotalType>()
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

        public void _61AddTaxSubTotal(string TaxableAmountCurrencyID, double TaxableAmountValue, string TaxAmountCurrencyID, double TaxAmountValue, string TaxCategoryID, string TaxCategoryAgencyID, string TaxCategoryValue, double Percent, string TaxSchemeID, string TaxSchemeAgencyID, string TaxSchemeValue, string TaxExemptionReasonCode, string TaxExemptionReason)
        {
            if (_invoice.TaxTotal == null || !_invoice.TaxTotal.Any())
                _invoice.TaxTotal = new List<TaxTotalType> { new TaxTotalType() };

            if (_invoice.TaxTotal.First().TaxSubtotal == null)
                _invoice.TaxTotal.First().TaxSubtotal = new List<TaxSubtotalType>();

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
                        schemeID = string.IsNullOrWhiteSpace(TaxCategoryID) ? null : TaxCategoryID,
                        schemeAgencyID = string.IsNullOrWhiteSpace(TaxCategoryAgencyID) ? null : TaxCategoryAgencyID,
                        Value = TaxCategoryValue
                    },
                    Percent = (decimal)Percent,
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

            _invoice.TaxTotal.First().TaxSubtotal.Add(taxSub);
        }

        public void _62PaymentTermns(string Value)
        {
            _invoice.PaymentTerms = new List<PaymentTermsType>()
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

        public void _63LegalMonetary(string LineExtensionID, double LineExtensionValue, string TaxExclusiveID, double TaxExclusiveValue, string TaxInclusiveID, double TaxInclusive, string AllowanceTotalID, double AllowanceTotalValue, string ChargeTotalID, double ChargeTotalValue, string PrepaidID, double PrepaidValue, string PayableRoundingID, double PayableRoundingValue, string PayableID, double PayableValue)
        {
            _invoice.LegalMonetaryTotal = new MonetaryTotalType
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

        public void _64AddInvoiceLine(string ID, string InvQtyUnitCode, double InvQtyValue, string LineExtensionCurrencyID, double LineExtensionValue, string AccountingCost, string OrderLineReferenceID, string TaxTotalcurrencyID, double TaxTotalValue/*, string TaxExemptionReasonCode, string TaxExemptionReason*/)
        {
            //bool hasTaxExemption = !string.IsNullOrWhiteSpace(TaxExemptionReasonCode) || !string.IsNullOrWhiteSpace(TaxExemptionReason);
            bool createTaxTotal = TaxTotalValue != 0;// || hasTaxExemption;

            InvoiceLineType line =
                new InvoiceLineType
                {
                    ID = ID,

                    InvoicedQuantity = new QuantityType
                    {
                        unitCode = InvQtyUnitCode,
                        Value = (decimal)InvQtyValue
                    },

                    LineExtensionAmount = new AmountType
                    {
                        currencyID = LineExtensionCurrencyID,
                        Value = (decimal)LineExtensionValue
                    },

                    AccountingCost = string.IsNullOrWhiteSpace(AccountingCost) ? null : AccountingCost,

                    OrderLineReference = string.IsNullOrWhiteSpace(OrderLineReferenceID) ? null : new List<OrderLineReferenceType>
                    {
                new OrderLineReferenceType
                {
                    LineID = OrderLineReferenceID
                }
                    },

                    TaxTotal = createTaxTotal ? new List<TaxTotalType>
                    {
                new TaxTotalType
                {
                    TaxAmount = new AmountType
                    {
                        currencyID = TaxTotalcurrencyID,
                        Value = (decimal)TaxTotalValue
                    }/*,

                    TaxSubtotal = new List<TaxSubtotalType>
                    {
                        new TaxSubtotalType
                        {
                            TaxableAmount = new AmountType
                            {
                                currencyID = LineExtensionCurrencyID,
                                Value = (decimal)LineExtensionValue
                            },

                            TaxAmount = new AmountType
                            {
                                currencyID = TaxTotalcurrencyID,
                                Value = (decimal)TaxTotalValue
                            }
                           
                            ,

                            TaxCategory = new TaxCategoryType
                            {
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
                                }
                            }
                        }
                    }*/
                }
                    } : null
                };

            line.ID.schemeID = "0160";

            if (_invoice.InvoiceLine == null)
                _invoice.InvoiceLine = new List<InvoiceLineType>();

            _invoice.InvoiceLine.Add(line);
        }

        public void _65AddInvLineItem(string Name,string SellersItemIdentification,string StandardItemIdentificationID,string StandardItemIdentificationAgencyID, string StandardItemIdentificationValue, string ClassifiedTaxCategoryID, string ClassifiedTaxCategoryAgencyID,string ClassifiedTaxCategoryValue,double ClassifiedTaxPercent,string TaxSchemeID, string TaxSchemeAgencyID,string TaxSchemeValue,string PriceAmountCurrId,double PriceAmountValue,string BaseQuantityUnitCode,double BaseQuantityValue, string TaxExemptionReasonCode, string TaxExemptionReason)
        {
            _invoice.InvoiceLine.LastOrDefault().Item = new ItemType
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
            _invoice.InvoiceLine.LastOrDefault().Price = new PriceType
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

        public void _66AddInvLineItemAllowCharge(double MultiplierFactorNumeric, string currencyID, double Amount)
        {
            _invoice.InvoiceLine.LastOrDefault().AllowanceCharge = new List<AllowanceChargeType>()
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


       
        public void _67SaveInvoice(string FilePath)
        {
            string xml=GetUBLXml();
        
            File.WriteAllText(FilePath, xml);
        }


        public void _68SignUBLInvoice(string RequestFilePathToSave, string ResponseFilePathToSave)
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

        public void _69SignSoapRequest(string RequestFilePathToSave, string ResponseFilePathToSave)
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


        



        public string _70InvoiceSendRequest(string requestUUID, string RequestFilePathToSave, string ResponseFilePathToSave)
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
                ublXml = BuildSoapEnvelope(GetXml());         // _invoice.GetXml()

                      
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
            _invoice.Save(str);
            str.Position = 0;
            StreamReader reader = new StreamReader(str);
            string text = reader.ReadToEnd();
            text=text.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>","");

            text = text.Replace("xmlns:sbc=\"urn:oasis:names:specification:ubl:schema:xsd:SignatureBasicComponents-2\" xmlns:sig=\"urn:oasis:names:specification:ubl:schema:xsd:CommonSignatureComponents-2\"", "");




            return text;    

            /*
            _request.Invoice = _invoice;

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
            _request.EinvoiceEnvelope.ItemElementName = ItemChoiceType.UblInvoice;
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

       




        public UblSharp.InvoiceType Create()
        {
            var doc = new InvoiceType
            {
                UBLVersionID = "2.1",
                ID = "TOSL108",
                IssueDate = "2009-12-15",
                InvoiceTypeCode = new CodeType
                {
                    listID = "UN/ECE 1001 Subset",
                    listAgencyID = "6",
                    Value = "380"
                },
                Note = new List<TextType>()
                {
                    new TextType
                    {
                        languageID = "en",
                        Value = "Ordered in our booth at the convention."
                    }
                },
                TaxPointDate = "2009-11-30",
                DocumentCurrencyCode = new CodeType
                {
                    listID = "ISO 4217 Alpha",
                    listAgencyID = "6",
                    Value = "EUR"
                },
                AccountingCost = "Project cost code 123",
                InvoicePeriod = new List<PeriodType>()
                {
                    new PeriodType
                    {
                        StartDate = "2009-11-01",
                        EndDate = "2009-11-30"
                    }
                },
                OrderReference = new OrderReferenceType
                {
                    ID = "123"
                },
                ContractDocumentReference = new List<DocumentReferenceType>()
                {
                    new DocumentReferenceType
                    {
                        ID = "Contract321",
                        DocumentType = "Framework agreement"
                    }
                },
                AdditionalDocumentReference = new List<DocumentReferenceType>()
                {
                    new DocumentReferenceType
                    {
                        ID = "Doc1",
                        DocumentType = "Timesheet",
                        Attachment = new AttachmentType
                        {
                            ExternalReference = new ExternalReferenceType
                            {
                                URI = "http://www.suppliersite.eu/sheet001.html"
                            }
                        }
                    },
                    new DocumentReferenceType
                    {
                        ID = "Doc2",
                        DocumentType = "Drawing",
                        Attachment = new AttachmentType
                        {
                            EmbeddedDocumentBinaryObject = new BinaryObjectType
                            {
                                mimeCode = "application/pdf",
                                Value = Convert.FromBase64String("UjBsR09EbGhjZ0dTQUxNQUFBUUNBRU1tQ1p0dU1GUXhEUzhi")
                            }
                        }
                    }
                },
                AccountingSupplierParty = new SupplierPartyType
                {
                    Party = new PartyType
                    {
                        EndpointID = new IdentifierType
                        {
                            schemeID = "GLN",
                            schemeAgencyID = "9",
                            Value = "1234567890123"
                        },
                        PartyIdentification = new List<PartyIdentificationType>()
                        {
                            new PartyIdentificationType
                            {
                                ID = new IdentifierType
                                {
                                    schemeID = "ZZZ",
                                    Value = "Supp123"
                                }
                            }
                        },
                        PartyName = new List<PartyNameType>()
                        {
                            new PartyNameType
                            {
                                Name = "Salescompany ltd."
                            }
                        },
                        PostalAddress = new AddressType
                        {
                            ID = new IdentifierType
                            {
                                schemeID = "GLN",
                                schemeAgencyID = "9",
                                Value = "1231412341324"
                            },
                            Postbox = "5467",
                            StreetName = "Main street",
                            AdditionalStreetName = "Suite 123",
                            BuildingNumber = "1",
                            Department = "Revenue department",
                            CityName = "Big city",
                            PostalZone = "54321",
                            CountrySubentityCode = "RegionA",
                            Country = new CountryType
                            {
                                IdentificationCode = new CodeType
                                {
                                    listID = "ISO3166-1",
                                    listAgencyID = "6",
                                    Value = "DK"
                                }
                            }
                        },
                        PartyTaxScheme = new List<PartyTaxSchemeType>()
                        {
                            new PartyTaxSchemeType
                            {
                                CompanyID = new IdentifierType
                                {
                                    schemeID = "DKVAT",
                                    schemeAgencyID = "ZZZ",
                                    Value = "DK12345"
                                },
                                TaxScheme = new TaxSchemeType
                                {
                                    ID = new IdentifierType
                                    {
                                        schemeID = "UN/ECE 5153",
                                        schemeAgencyID = "6",
                                        Value = "VAT"
                                    }
                                }
                            }
                        },
                        PartyLegalEntity = new List<PartyLegalEntityType>()
                        {
                            new PartyLegalEntityType
                            {
                                RegistrationName = "The Sellercompany Incorporated",
                                CompanyID = new IdentifierType
                                {
                                    schemeID = "CVR",
                                    schemeAgencyID = "ZZZ",
                                    Value = "5402697509"
                                },
                                RegistrationAddress = new AddressType
                                {
                                    CityName = "Big city",
                                    CountrySubentity = "RegionA",
                                    Country = new CountryType
                                    {
                                        IdentificationCode = "DK"
                                    }
                                }
                                
                            }
                        },
                        Contact = new ContactType
                        {
                            Telephone = "4621230",
                            Telefax = "4621231",
                            ElectronicMail = "antonio@salescompany.dk"
                        },
                        Person = new List<PersonType>()
                        {
                            new PersonType
                            {
                                FirstName = "Antonio",
                                FamilyName = "M",
                                MiddleName = "Salemacher",
                                JobTitle = "Sales manager"
                            }
                        },
                    }
                },
                AccountingCustomerParty = new CustomerPartyType
                {
                    Party = new PartyType
                    {
                        EndpointID = new IdentifierType
                        {
                            schemeID = "GLN",
                            schemeAgencyID = "9",
                            Value = "1234567987654"
                        },
                        PartyIdentification = new List<PartyIdentificationType>()
                        {
                            new PartyIdentificationType
                            {
                                ID = new IdentifierType
                                {
                                    schemeID = "ZZZ",
                                    Value = "345KS5324"
                                }
                            }
                        },
                        PartyName = new List<PartyNameType>()
                        {
                            new PartyNameType
                            {
                                Name = "Buyercompany ltd"
                            }
                        },
                        PostalAddress = new AddressType
                        {
                            ID = new IdentifierType
                            {
                                schemeID = "GLN",
                                schemeAgencyID = "9",
                                Value = "1238764941386"
                            },
                            Postbox = "123",
                            StreetName = "Anystreet",
                            AdditionalStreetName = "Back door",
                            BuildingNumber = "8",
                            Department = "Accounting department",
                            CityName = "Anytown",
                            PostalZone = "101",
                            CountrySubentity = "RegionB",
                            Country = new CountryType
                            {
                                IdentificationCode = new CodeType
                                {
                                    listID = "ISO3166-1",
                                    listAgencyID = "6",
                                    Value = "BE"
                                }
                            }
                        },
                        PartyTaxScheme = new List<PartyTaxSchemeType>()
                        {
                            new PartyTaxSchemeType
                            {
                                CompanyID = new IdentifierType
                                {
                                    schemeID = "BEVAT",
                                    schemeAgencyID = "ZZZ",
                                    Value = "BE54321"
                                },
                                TaxScheme = new TaxSchemeType
                                {
                                    ID = new IdentifierType
                                    {
                                        schemeID = "UN/ECE 5153",
                                        schemeAgencyID = "6",
                                        Value = "VAT"
                                    }
                                }
                            }
                        },
                        PartyLegalEntity = new List<PartyLegalEntityType>()
                        {
                            new PartyLegalEntityType
                            {
                                RegistrationName = "The buyercompany inc.",
                                CompanyID = new IdentifierType
                                {
                                    schemeAgencyID = "ZZZ",
                                    schemeID = "ZZZ",
                                    Value = "5645342123"
                                },
                                RegistrationAddress = new AddressType
                                {
                                    CityName = "Mainplace",
                                    CountrySubentity = "RegionB",
                                    Country = new CountryType
                                    {
                                        IdentificationCode = "BE"
                                    }
                                }
                            }
                        },
                        Contact = new ContactType
                        {
                            Telephone = "5121230",
                            Telefax = "5121231",
                            ElectronicMail = "john@buyercompany.eu"
                        },
                        Person = new List<PersonType>()
                        {
                            new PersonType
                            {
                                FirstName = "John",
                                FamilyName = "X",
                                MiddleName = "Doe",
                                JobTitle = "Purchasing manager"
                            }
                        },
                    }
                },
                PayeeParty = new PartyType
                {
                    PartyIdentification = new List<PartyIdentificationType>()
                    {
                        new PartyIdentificationType
                        {
                            ID = new IdentifierType
                            {
                                schemeID = "GLN",
                                schemeAgencyID = "9",
                                Value = "098740918237"
                            }
                        }
                    },
                    PartyName = new List<PartyNameType>()
                    {
                        new PartyNameType
                        {
                            Name = "Ebeneser Scrooge Inc."
                        }
                    },
                    PartyLegalEntity = new List<PartyLegalEntityType>()
                    {
                        new PartyLegalEntityType
                        {
                            CompanyID = new IdentifierType
                            {
                                schemeID = "UK:CH",
                                schemeAgencyID = "ZZZ",
                                Value = "6411982340"
                            }
                        }
                    },
                },
                Delivery = new List<DeliveryType>()
                {
                    new DeliveryType
                    {
                        ActualDeliveryDate = "2009-12-15",
                        DeliveryLocation = new LocationType
                        {
                            ID = new IdentifierType
                            {
                                schemeID = "GLN",
                                schemeAgencyID = "9",
                                Value = "6754238987648"
                            },
                            Address = new AddressType
                            {
                                StreetName = "Deliverystreet",
                                AdditionalStreetName = "Side door",
                                BuildingNumber = "12",
                                CityName = "DeliveryCity",
                                PostalZone = "523427",
                                CountrySubentity = "RegionC",
                                Country = new CountryType
                                {
                                    IdentificationCode = "BE"
                                }
                            }
                        }
                    }
                },
                PaymentMeans = new List<PaymentMeansType>()
                {
                    new PaymentMeansType
                    {
                        PaymentMeansCode = new CodeType
                        {
                            listID = "UN/ECE 4461",
                            Value = "31"
                        },
                        PaymentDueDate = "2009-12-31",
                        PaymentChannelCode = "IBAN",
                        PaymentID = new List<IdentifierType>()
                        {
                            new IdentifierType
                            {
                                Value = "Payref1"
                            }
                        },
                        PayeeFinancialAccount = new FinancialAccountType
                        {
                            ID = "DK1212341234123412",
                            FinancialInstitutionBranch = new BranchType
                            {
                                FinancialInstitution = new FinancialInstitutionType
                                {
                                    ID = "DKDKABCD"
                                }
                            }
                        }
                    }
                },
                PaymentTerms = new List<PaymentTermsType>()
                {
                    new PaymentTermsType
                    {
                        Note = new List<TextType>()
                        {
                            new TextType
                            {
                                Value = "Penalty percentage 10% from due date"
                            }
                        },
                    }
                },
                AllowanceCharge = new List<AllowanceChargeType>()
                {
                    new AllowanceChargeType
                    {
                        ChargeIndicator = true,
                        AllowanceChargeReason = new List<TextType>()
                        {
                            new TextType
                            {
                                Value = "Packing cost"
                            }
                        },
                        Amount = new AmountType
                        {
                            currencyID = "EUR",
                            Value = 100M
                        }
                    },
                    new AllowanceChargeType
                    {
                        ChargeIndicator = false,
                        AllowanceChargeReason = new List<TextType>()
                        {
                            new TextType
                            {
                                Value = "Promotion discount"
                            }
                        },
                        Amount = new AmountType
                        {
                            currencyID = "EUR",
                            Value = 100M
                        }
                    }
                },
                TaxTotal = new List<TaxTotalType>()
                {
                    new TaxTotalType
                    {
                        TaxAmount = new AmountType
                        {
                            currencyID = "EUR",
                            Value = 292.20M
                        },
                        TaxSubtotal = new List<TaxSubtotalType>()
                        {
                            new TaxSubtotalType
                            {
                                TaxableAmount = new AmountType
                                {
                                    currencyID = "EUR",
                                    Value = 1460.5M
                                },
                                TaxAmount = new AmountType
                                {
                                    currencyID = "EUR",
                                    Value = 292.1M
                                },
                                TaxCategory = new TaxCategoryType
                                {
                                    ID = new IdentifierType
                                    {
                                        schemeID = "UN/ECE 5305",
                                        schemeAgencyID = "6",
                                        Value = "S"
                                    },
                                    Percent = 20M,
                                    TaxScheme = new TaxSchemeType
                                    {
                                        ID = new IdentifierType
                                        {
                                            schemeID = "UN/ECE 5153",
                                            schemeAgencyID = "6",
                                            Value = "VAT"
                                        }
                                    }
                                }
                            },
                            new TaxSubtotalType
                            {
                                TaxableAmount = new AmountType
                                {
                                    currencyID = "EUR",
                                    Value = 1M
                                },
                                TaxAmount = new AmountType
                                {
                                    currencyID = "EUR",
                                    Value = 0.1M
                                },
                                TaxCategory = new TaxCategoryType
                                {
                                    ID = new IdentifierType
                                    {
                                        schemeID = "UN/ECE 5305",
                                        schemeAgencyID = "6",
                                        Value = "AA"
                                    },
                                    Percent = 10M,
                                    TaxScheme = new TaxSchemeType
                                    {
                                        ID = new IdentifierType
                                        {
                                            schemeID = "UN/ECE 5153",
                                            schemeAgencyID = "6",
                                            Value = "VAT"
                                        }
                                    }
                                }
                            },
                            new TaxSubtotalType
                            {
                                TaxableAmount = new AmountType
                                {
                                    currencyID = "EUR",
                                    Value = -25M
                                },
                                TaxAmount = new AmountType
                                {
                                    currencyID = "EUR",
                                    Value = 0M
                                },
                                TaxCategory = new TaxCategoryType
                                {
                                    ID = new IdentifierType
                                    {
                                        schemeID = "UN/ECE 5305",
                                        schemeAgencyID = "6",
                                        Value = "E"
                                    },
                                    Percent = 0M,
                                    TaxExemptionReasonCode = new CodeType
                                    {
                                        listID = "CWA 15577",
                                        listAgencyID = "ZZZ",
                                        Value = "AAM"
                                    },
                                    TaxExemptionReason = new List<TextType>()
                                    {
                                        new TextType
                                        {
                                            Value = "Exempt New Means of Transport"
                                        }
                                    },
                                    TaxScheme = new TaxSchemeType
                                    {
                                        ID = new IdentifierType
                                        {
                                            schemeID = "UN/ECE 5153",
                                            schemeAgencyID = "6",
                                            Value = "VAT"
                                        }
                                    }
                                }
                            }
                        },
                    }
                },
                LegalMonetaryTotal = new MonetaryTotalType
                {
                    LineExtensionAmount = new AmountType
                    {
                        currencyID = "EUR",
                        Value = 1436.5M
                    },
                    TaxExclusiveAmount = new AmountType
                    {
                        currencyID = "EUR",
                        Value = 1436.5M
                    },
                    TaxInclusiveAmount = new AmountType
                    {
                        currencyID = "EUR",
                        Value = 1729M
                    },
                    AllowanceTotalAmount = new AmountType
                    {
                        currencyID = "EUR",
                        Value = 100M
                    },
                    ChargeTotalAmount = new AmountType
                    {
                        currencyID = "EUR",
                        Value = 100M
                    },
                    PrepaidAmount = new AmountType
                    {
                        currencyID = "EUR",
                        Value = 1000M
                    },
                    PayableRoundingAmount = new AmountType
                    {
                        currencyID = "EUR",
                        Value = 0.30M
                    },
                    PayableAmount = new AmountType
                    {
                        currencyID = "EUR",
                        Value = 729M
                    }
                },
                InvoiceLine = new List<InvoiceLineType>()
                {
                    new InvoiceLineType
                    {
                        ID = "1",
                        Note = new List<TextType>()
                        {
                            new TextType
                            {
                                Value = "Scratch on box"
                            }
                        },
                        InvoicedQuantity = new QuantityType
                        {
                            unitCode = "C62",
                            Value = 1M
                        },
                        LineExtensionAmount = new AmountType
                        {
                            currencyID = "EUR",
                            Value = 1273M
                        },
                        AccountingCost = "BookingCode001",
                        OrderLineReference = new List<OrderLineReferenceType>()
                        {
                            new OrderLineReferenceType
                            {
                                LineID = "1"
                            }
                        },
                        AllowanceCharge = new List<AllowanceChargeType>()
                        {
                            new AllowanceChargeType
                            {
                                ChargeIndicator = false,
                                AllowanceChargeReason = new List<TextType>()
                                {
                                    new TextType
                                    {
                                        Value = "Damage"
                                    }
                                },
                                Amount = new AmountType
                                {
                                    currencyID = "EUR",
                                    Value = 12M
                                }
                            },
                            new AllowanceChargeType
                            {
                                ChargeIndicator = true,
                                AllowanceChargeReason = new List<TextType>()
                                {
                                    new TextType
                                    {
                                        Value = "Testing"
                                    }
                                },
                                Amount = new AmountType
                                {
                                    currencyID = "EUR",
                                    Value = 10M
                                }
                            }
                        },
                        TaxTotal = new List<TaxTotalType>()
                        {
                            new TaxTotalType
                            {
                                TaxAmount = new AmountType
                                {
                                    currencyID = "EUR",
                                    Value = 254.6M
                                }
                            }
                        },
                        Item = new ItemType
                        {
                            Description = new List<TextType>()
                            {
                                new TextType
                                {
                                    languageID = "EN",
                                    Value = @"Processor: Intel Core 2 Duo SU9400 LV (1.4GHz). RAM:
				3MB. Screen 1440x900"
                                }
                            },
                            Name = "Labtop computer",
                            SellersItemIdentification = new ItemIdentificationType
                            {
                                ID = "JB007"
                            },
                            StandardItemIdentification = new ItemIdentificationType
                            {
                                ID = new IdentifierType
                                {
                                    schemeID = "GTIN",
                                    schemeAgencyID = "9",
                                    Value = "1234567890124"
                                }
                            },
                            CommodityClassification = new List<CommodityClassificationType>()
                            {
                                new CommodityClassificationType
                                {
                                    ItemClassificationCode = new CodeType
                                    {
                                        listAgencyID = "113",
                                        listID = "UNSPSC",
                                        Value = "12344321"
                                    }
                                },
                                new CommodityClassificationType
                                {
                                    ItemClassificationCode = new CodeType
                                    {
                                        listAgencyID = "2",
                                        listID = "CPV",
                                        Value = "65434568"
                                    }
                                }
                            },
                            ClassifiedTaxCategory = new List<TaxCategoryType>()
                            {
                                new TaxCategoryType
                                {
                                    ID = new IdentifierType
                                    {
                                        schemeID = "UN/ECE 5305",
                                        schemeAgencyID = "6",
                                        Value = "S"
                                    },
                                    Percent = 20M,
                                    TaxScheme = new TaxSchemeType
                                    {
                                        ID = new IdentifierType
                                        {
                                            schemeID = "UN/ECE 5153",
                                            schemeAgencyID = "6",
                                            Value = "VAT"
                                        }
                                    }
                                }
                            },
                            AdditionalItemProperty = new List<ItemPropertyType>()
                            {
                                new ItemPropertyType
                                {
                                    Name = "Color",
                                    Value = "black"
                                }
                            },
                        },
                        Price = new PriceType
                        {
                            PriceAmount = new AmountType
                            {
                                currencyID = "EUR",
                                Value = 1273M
                            },
                            BaseQuantity = new QuantityType
                            {
                                unitCode = "C62",
                                Value = 1M
                            },
                            AllowanceCharge = new List<AllowanceChargeType>()
                            {
                                new AllowanceChargeType
                                {
                                    ChargeIndicator = false,
                                    AllowanceChargeReason = new List<TextType>()
                                    {
                                        new TextType
                                        {
                                            Value = "Contract"
                                        }
                                    },
                                    MultiplierFactorNumeric = 0.15M,
                                    Amount = new AmountType
                                    {
                                        currencyID = "EUR",
                                        Value = 225M
                                    },
                                    BaseAmount = new AmountType
                                    {
                                        currencyID = "EUR",
                                        Value = 1500M
                                    }
                                }
                            },
                        }
                    },
                    new InvoiceLineType
                    {
                        ID = "2",
                        Note = new List<TextType>()
                        {
                            new TextType
                            {
                                Value = "Cover is slightly damaged."
                            }
                        },
                        InvoicedQuantity = new QuantityType
                        {
                            unitCode = "C62",
                            Value = -1M
                        },
                        LineExtensionAmount = new AmountType
                        {
                            currencyID = "EUR",
                            Value = -3.96M
                        },
                        OrderLineReference = new List<OrderLineReferenceType>()
                        {
                            new OrderLineReferenceType
                            {
                                LineID = "5"
                            }
                        },
                        TaxTotal = new List<TaxTotalType>()
                        {
                            new TaxTotalType
                            {
                                TaxAmount = new AmountType
                                {
                                    currencyID = "EUR",
                                    Value = -0.396M
                                }
                            }
                        },
                        Item = new ItemType
                        {
                            Name = "Returned \"Advanced computing\" book",
                            SellersItemIdentification = new ItemIdentificationType
                            {
                                ID = "JB008"
                            },
                            StandardItemIdentification = new ItemIdentificationType
                            {
                                ID = new IdentifierType
                                {
                                    schemeID = "GTIN",
                                    schemeAgencyID = "9",
                                    Value = "1234567890125"
                                }
                            },
                            CommodityClassification = new List<CommodityClassificationType>()
                            {
                                new CommodityClassificationType
                                {
                                    ItemClassificationCode = new CodeType
                                    {
                                        listAgencyID = "113",
                                        listID = "UNSPSC",
                                        Value = "32344324"
                                    }
                                },
                                new CommodityClassificationType
                                {
                                    ItemClassificationCode = new CodeType
                                    {
                                        listAgencyID = "2",
                                        listID = "CPV",
                                        Value = "65434567"
                                    }
                                }
                            },
                            ClassifiedTaxCategory = new List<TaxCategoryType>()
                            {
                                new TaxCategoryType
                                {
                                    ID = new IdentifierType
                                    {
                                        schemeID = "UN/ECE 5305",
                                        schemeAgencyID = "6",
                                        Value = "AA"
                                    },
                                    Percent = 10M,
                                    TaxScheme = new TaxSchemeType
                                    {
                                        ID = new IdentifierType
                                        {
                                            schemeID = "UN/ECE 5153",
                                            schemeAgencyID = "6",
                                            Value = "VAT"
                                        }
                                    }
                                }
                            },
                        },
                        Price = new PriceType
                        {
                            PriceAmount = new AmountType
                            {
                                currencyID = "EUR",
                                Value = 3.96M
                            },
                            BaseQuantity = new QuantityType
                            {
                                unitCode = "C62",
                                Value = 1M
                            }
                        }
                    },
                    new InvoiceLineType
                    {
                        ID = "3",
                        InvoicedQuantity = new QuantityType
                        {
                            unitCode = "C62",
                            Value = 2M
                        },
                        LineExtensionAmount = new AmountType
                        {
                            currencyID = "EUR",
                            Value = 4.96M
                        },
                        OrderLineReference = new List<OrderLineReferenceType>()
                        {
                            new OrderLineReferenceType
                            {
                                LineID = "3"
                            }
                        },
                        TaxTotal = new List<TaxTotalType>()
                        {
                            new TaxTotalType
                            {
                                TaxAmount = new AmountType
                                {
                                    currencyID = "EUR",
                                    Value = 0.496M
                                }
                            }
                        },
                        Item = new ItemType
                        {
                            Name = "\"Computing for dummies\" book",
                            SellersItemIdentification = new ItemIdentificationType
                            {
                                ID = "JB009"
                            },
                            StandardItemIdentification = new ItemIdentificationType
                            {
                                ID = new IdentifierType
                                {
                                    schemeID = "GTIN",
                                    schemeAgencyID = "9",
                                    Value = "1234567890126"
                                }
                            },
                            CommodityClassification = new List<CommodityClassificationType>()
                            {
                                new CommodityClassificationType
                                {
                                    ItemClassificationCode = new CodeType
                                    {
                                        listAgencyID = "113",
                                        listID = "UNSPSC",
                                        Value = "32344324"
                                    }
                                },
                                new CommodityClassificationType
                                {
                                    ItemClassificationCode = new CodeType
                                    {
                                        listAgencyID = "2",
                                        listID = "CPV",
                                        Value = "65434566"
                                    }
                                }
                            },
                            ClassifiedTaxCategory = new List<TaxCategoryType>()
                            {
                                new TaxCategoryType
                                {
                                    ID = new IdentifierType
                                    {
                                        schemeID = "UN/ECE 5305",
                                        schemeAgencyID = "6",
                                        Value = "AA"
                                    },
                                    Percent = 10M,
                                    TaxScheme = new TaxSchemeType
                                    {
                                        ID = new IdentifierType
                                        {
                                            schemeID = "UN/ECE 5153",
                                            schemeAgencyID = "6",
                                            Value = "VAT"
                                        }
                                    }
                                }
                            },
                        },
                        Price = new PriceType
                        {
                            PriceAmount = new AmountType
                            {
                                currencyID = "EUR",
                                Value = 2.48M
                            },
                            BaseQuantity = new QuantityType
                            {
                                unitCode = "C62",
                                Value = 1M
                            },
                            AllowanceCharge = new List<AllowanceChargeType>()
                            {
                                new AllowanceChargeType
                                {
                                    ChargeIndicator = false,
                                    AllowanceChargeReason = new List<TextType>()
                                    {
                                        new TextType
                                        {
                                            Value = "Contract"
                                        }
                                    },
                                    MultiplierFactorNumeric = 0.1M,
                                    Amount = new AmountType
                                    {
                                        currencyID = "EUR",
                                        Value = 0.275M
                                    },
                                    BaseAmount = new AmountType
                                    {
                                        currencyID = "EUR",
                                        Value = 2.75M
                                    }
                                }
                            },
                        }
                    },
                    new InvoiceLineType
                    {
                        ID = "4",
                        InvoicedQuantity = new QuantityType
                        {
                            unitCode = "C62",
                            Value = -1M
                        },
                        LineExtensionAmount = new AmountType
                        {
                            currencyID = "EUR",
                            Value = -25M
                        },
                        OrderLineReference = new List<OrderLineReferenceType>()
                        {
                            new OrderLineReferenceType
                            {
                                LineID = "2"
                            }
                        },
                        TaxTotal = new List<TaxTotalType>()
                        {
                            new TaxTotalType
                            {
                                TaxAmount = new AmountType
                                {
                                    currencyID = "EUR",
                                    Value = 0M
                                }
                            }
                        },
                        Item = new ItemType
                        {
                            Name = "Returned IBM 5150 desktop",
                            SellersItemIdentification = new ItemIdentificationType
                            {
                                ID = "JB010"
                            },
                            StandardItemIdentification = new ItemIdentificationType
                            {
                                ID = new IdentifierType
                                {
                                    schemeID = "GTIN",
                                    schemeAgencyID = "9",
                                    Value = "1234567890127"
                                }
                            },
                            CommodityClassification = new List<CommodityClassificationType>()
                            {
                                new CommodityClassificationType
                                {
                                    ItemClassificationCode = new CodeType
                                    {
                                        listAgencyID = "113",
                                        listID = "UNSPSC",
                                        Value = "12344322"
                                    }
                                },
                                new CommodityClassificationType
                                {
                                    ItemClassificationCode = new CodeType
                                    {
                                        listAgencyID = "2",
                                        listID = "CPV",
                                        Value = "65434565"
                                    }
                                }
                            },
                            ClassifiedTaxCategory = new List<TaxCategoryType>()
                            {
                                new TaxCategoryType
                                {
                                    ID = new IdentifierType
                                    {
                                        schemeID = "UN/ECE 5305",
                                        schemeAgencyID = "6",
                                        Value = "E"
                                    },
                                    Percent = 0M,
                                    TaxScheme = new TaxSchemeType
                                    {
                                        ID = new IdentifierType
                                        {
                                            schemeID = "UN/ECE 5153",
                                            schemeAgencyID = "6",
                                            Value = "VAT"
                                        }
                                    }
                                }
                            },
                        },
                        Price = new PriceType
                        {
                            PriceAmount = new AmountType
                            {
                                currencyID = "EUR",
                                Value = 25M
                            },
                            BaseQuantity = new QuantityType
                            {
                                unitCode = "C62",
                                Value = 1M
                            }
                        }
                    },
                    new InvoiceLineType
                    {
                        ID = "5",
                        InvoicedQuantity = new QuantityType
                        {
                            unitCode = "C62",
                            Value = 250M
                        },
                        LineExtensionAmount = new AmountType
                        {
                            currencyID = "EUR",
                            Value = 187.5M
                        },
                        AccountingCost = "BookingCode002",
                        OrderLineReference = new List<OrderLineReferenceType>()
                        {
                            new OrderLineReferenceType
                            {
                                LineID = "4"
                            }
                        },
                        TaxTotal = new List<TaxTotalType>()
                        {
                            new TaxTotalType
                            {
                                TaxAmount = new AmountType
                                {
                                    currencyID = "EUR",
                                    Value = 37.5M
                                }
                            }
                        },
                        Item = new ItemType
                        {
                            Name = "Network cable",
                            SellersItemIdentification = new ItemIdentificationType
                            {
                                ID = "JB011"
                            },
                            StandardItemIdentification = new ItemIdentificationType
                            {
                                ID = new IdentifierType
                                {
                                    schemeID = "GTIN",
                                    schemeAgencyID = "9",
                                    Value = "1234567890128"
                                }
                            },
                            CommodityClassification = new List<CommodityClassificationType>()
                            {
                                new CommodityClassificationType
                                {
                                    ItemClassificationCode = new CodeType
                                    {
                                        listAgencyID = "113",
                                        listID = "UNSPSC",
                                        Value = "12344325"
                                    }
                                },
                                new CommodityClassificationType
                                {
                                    ItemClassificationCode = new CodeType
                                    {
                                        listAgencyID = "2",
                                        listID = "CPV",
                                        Value = "65434564"
                                    }
                                }
                            },
                            ClassifiedTaxCategory = new List<TaxCategoryType>()
                            {
                                new TaxCategoryType
                                {
                                    ID = new IdentifierType
                                    {
                                        schemeID = "UN/ECE 5305",
                                        schemeAgencyID = "6",
                                        Value = "S"
                                    },
                                    Percent = 20M,
                                    TaxScheme = new TaxSchemeType
                                    {
                                        ID = new IdentifierType
                                        {
                                            schemeID = "UN/ECE 5153",
                                            schemeAgencyID = "6",
                                            Value = "VAT"
                                        }
                                    }
                                }
                            },
                            AdditionalItemProperty = new List<ItemPropertyType>()
                            {
                                new ItemPropertyType
                                {
                                    Name = "Type",
                                    Value = "Cat5"
                                }
                            },
                        },
                        Price = new PriceType
                        {
                            PriceAmount = new AmountType
                            {
                                currencyID = "EUR",
                                Value = 0.75M
                            },
                            BaseQuantity = new QuantityType
                            {
                                unitCode = "C62",
                                Value = 1M
                            }
                        }
                    }
                },
            };
            doc.Xmlns = new System.Xml.Serialization.XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("cac","urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"),
                new XmlQualifiedName("cbc","urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"),
            });
            return doc;
        }
    }



}
