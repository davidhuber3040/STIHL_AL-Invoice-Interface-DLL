using System;
using System.Collections.Generic;

//using ZXing.QrCode;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Xml;
using UblSharp.CommonAggregateComponents;
using UblSharp.UnqualifiedDataTypes;
using UblSharp;
using System.ComponentModel.Design;
using System.Linq;
using System.Diagnostics;
using System.Text.Json;
using System.Security;
using System.Runtime.InteropServices.ComTypes;
using System.ComponentModel;
using System.Net.Security;

namespace AL_Invoice_Interface_DLL
{
    public class CreditNoteClass
    {

        CreditNoteType creditnote = new CreditNoteType();  
        public string resp;

        
        public void _310CreditNoteData(string UBLVersionID, string ID, string IssueDate, string InvTypelistID, string InvTypelistAgencyID, string InvTypeValue, string DocumentCurrencyCodelistID, string DocumentCurrencyCodelistAgencyID, string DocumentCurrencyCodeValue, string BillingReferenceID,string BillingReferenceIssueDate, string PeriodeDescriptionCode)
        {

            creditnote.UBLVersionID = UBLVersionID;
            creditnote.ID = ID;
            creditnote.IssueDate = IssueDate;
            creditnote.CreditNoteTypeCode = new CodeType
            {
                listID = InvTypelistID,
                listAgencyID = InvTypelistAgencyID,
                Value = InvTypeValue,
            };
            //invoice.TaxPointDate = TaxPointDate;
            creditnote.DocumentCurrencyCode = new CodeType
            {
                listID = DocumentCurrencyCodelistID,
                listAgencyID = DocumentCurrencyCodelistAgencyID,
                Value = DocumentCurrencyCodeValue,
            };

            creditnote.InvoicePeriod = new List<PeriodType>()
                {
                    new PeriodType
                    {
                        //StartDate = InvoicePeriodStartDate,
                        //EndDate = InvoicePeriodEndDate,
                       
                        DescriptionCode=new List<CodeType>()
                        {
                            new CodeType
                            {
                                Value=PeriodeDescriptionCode,
                            }
                        },
                        
                        
                    }
                };
            creditnote.BillingReference = new List<BillingReferenceType>()
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

            creditnote.Xmlns = new System.Xml.Serialization.XmlSerializerNamespaces(new[]
           {
                new XmlQualifiedName("cac","urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"),
                new XmlQualifiedName("cbc","urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"),
            });

        }
        public void _32AccSupParty(string EndpointID, string EndpointAgencyID, string EndpointValue, string PartyIdentificationID, string PartyIdentificationValue, string Name, string PostalAddressSchemeID, string PostalAddressSchemeAgencyID, string PostalAddressSchemeValue, string StreetName, string AdditionalStreetName, string CityName, string PostalZone, string CountrylistID, string CountrylistAgencyID, string CountrylistValue)

        {

            creditnote.AccountingSupplierParty = new SupplierPartyType
            {
                Party = new PartyType
                {
                    EndpointID = new IdentifierType
                    {
                        schemeID = EndpointID,
                        schemeAgencyID = EndpointAgencyID,
                        Value = EndpointValue
                    },
                    PartyIdentification = new List<PartyIdentificationType>()
                    {
                        new PartyIdentificationType
                        {
                            ID = new IdentifierType
                            {
                                schemeID = PartyIdentificationID,
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
                    PostalAddress = new AddressType
                    {
                        ID = new IdentifierType
                        {
                            schemeID = PostalAddressSchemeAgencyID,
                            schemeAgencyID = PostalAddressSchemeAgencyID,
                            Value = PostalAddressSchemeValue
                        },
                        //Postbox = "5467",
                        StreetName = StreetName,
                        AdditionalStreetName = AdditionalStreetName,
                        //BuildingNumber = "1",
                        //Department = "Revenue department",
                        CityName = CityName,
                        PostalZone = PostalZone,
                        //CountrySubentityCode = "RegionA",
                        Country = new CountryType
                        {
                            IdentificationCode = new CodeType
                            {
                                listID = CountrylistID,
                                listAgencyID = CountrylistAgencyID,
                                Value = CountrylistValue,
                            }
                        }
                    },
                }
            };
        }

        public void _33AccSupPartyTaxSch(string CompanyID, string CompanyAgencyID, string CompanyValue, string TaxSchemeID, string TaxSchemeAgencyID, string TaxSchemeValue, string PartyLegalRegistrationName, string PartyLegalID, string PartyLegalAgencyID, string PartyLegalValue, string PartyLegalRegisAddrCity, string PartyLegalRegisAddrCountryIdentCode)
        {


            creditnote.AccountingSupplierParty.Party.PartyTaxScheme = new List<PartyTaxSchemeType>()
                        {
                            new PartyTaxSchemeType
                            {
                                CompanyID = new IdentifierType
                                {
                                    schemeID = CompanyID,
                                    schemeAgencyID = CompanyAgencyID,
                                    Value = CompanyValue
                                },
                                TaxScheme = new TaxSchemeType
                                {
                                    ID = new IdentifierType
                                    {
                                        schemeID = TaxSchemeID,
                                        schemeAgencyID = TaxSchemeAgencyID,
                                        Value = TaxSchemeValue
                                    }
                                }
                            }
                        };
            creditnote.AccountingSupplierParty.Party.PartyLegalEntity = new List<PartyLegalEntityType>()
                        {
                            new PartyLegalEntityType
                            {
                                RegistrationName = PartyLegalRegistrationName,
                                CompanyID = new IdentifierType
                                {
                                    schemeID = PartyLegalID,
                                    schemeAgencyID = PartyLegalAgencyID,
                                    Value = PartyLegalValue
                                },
                                RegistrationAddress = new AddressType
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

        public void _34AccCustParty(string EndpointID, string EndpointAgencyID, string EndpointValue, string PartyIdentificationID, string PartyIdentificationValue, string Name, string PostalAddressSchemeID, string PostalAddressSchemeAgencyID, string PostalAddressSchemeValue, string StreetName, string AdditionalStreetName, string CityName, string PostalZone, string CountrylistID, string CountrylistAgencyID, string CountrylistValue)

        {

            creditnote.AccountingCustomerParty = new CustomerPartyType
            {
                Party = new PartyType
                {
                    EndpointID = new IdentifierType
                    {
                        schemeID = EndpointID,
                        schemeAgencyID = EndpointAgencyID,
                        Value = EndpointValue
                    },
                    PartyIdentification = new List<PartyIdentificationType>()
                    {
                        new PartyIdentificationType
                        {
                            ID = new IdentifierType
                            {
                                schemeID = PartyIdentificationID,
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
                    PostalAddress = new AddressType
                    {
                        ID = new IdentifierType
                        {
                            schemeID = PostalAddressSchemeAgencyID,
                            schemeAgencyID = PostalAddressSchemeAgencyID,
                            Value = PostalAddressSchemeValue
                        },
                        //Postbox = "5467",
                        StreetName = StreetName,
                        AdditionalStreetName = AdditionalStreetName,
                        //BuildingNumber = "1",
                        //Department = "Revenue department",
                        CityName = CityName,
                        PostalZone = PostalZone,
                        //CountrySubentityCode = "RegionA",
                        Country = new CountryType
                        {
                            IdentificationCode = new CodeType
                            {
                                listID = CountrylistID,
                                listAgencyID = CountrylistAgencyID,
                                Value = CountrylistValue,
                            }
                        }
                    },
                }
            };
        }
        public void _35AccCustPartyTaxSch(string CompanyID, string CompanyAgencyID, string CompanyValue, string TaxSchemeID, string TaxSchemeAgencyID, string TaxSchemeValue, string PartyLegalRegistrationName, string PartyLegalID, string PartyLegalAgencyID, string PartyLegalValue, string PartyLegalRegisAddrCity, string PartyLegalRegisAddrCountryIdentCode)
        {

            creditnote.AccountingCustomerParty.Party.PartyTaxScheme = new List<PartyTaxSchemeType>()
                        {
                            new PartyTaxSchemeType
                            {
                                CompanyID = new IdentifierType
                                {
                                    schemeID = CompanyID,
                                    schemeAgencyID = CompanyAgencyID,
                                    Value = CompanyValue
                                },
                                TaxScheme = new TaxSchemeType
                                {
                                    ID = new IdentifierType
                                    {
                                        schemeID = TaxSchemeID,
                                        schemeAgencyID = TaxSchemeAgencyID,
                                        Value = TaxSchemeValue
                                    }
                                }
                            }
                        };
            creditnote.AccountingCustomerParty.Party.PartyLegalEntity = new List<PartyLegalEntityType>()
                        {
                            new PartyLegalEntityType
                            {
                                RegistrationName = PartyLegalRegistrationName,
                                CompanyID = new IdentifierType
                                {
                                    schemeID = PartyLegalID,
                                    schemeAgencyID = PartyLegalAgencyID,
                                    Value = PartyLegalValue
                                },
                                RegistrationAddress = new AddressType
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

        public void _36PayeeParty(string PartyIdentificationID, string PartyIdentificationAgencyID, string PartyIdentificationValue, string Name, string PartyLegalID, string PartyLegalAgencyID, string PartyLegaValue)

        {

            creditnote.PayeeParty = new PartyType
            {
                PartyIdentification = new List<PartyIdentificationType>()
                {
                    new PartyIdentificationType
                    {
                        ID = new IdentifierType
                        {
                            schemeID = PartyIdentificationID,

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
                            schemeID = PartyLegalID,
                            schemeAgencyID = PartyLegalAgencyID,
                            Value = PartyLegaValue
                        }
                    }
                },
            };
        }

        public void _37Delivery(string ActualDeliveryDate, string DeliveryLocationID, string DeliveryLocationAgencyID, string DeliveryLocationValue, string StreetName, string AdditionalStreetName, string CityName, string PostalZone, string CountryIdentificationCode)
        {

            creditnote.Delivery = new List<DeliveryType>()
                {
                    new DeliveryType
                    {
                        ActualDeliveryDate = ActualDeliveryDate,
                        DeliveryLocation = new LocationType
                        {
                            ID = new IdentifierType
                            {
                                schemeID = DeliveryLocationID,
                                schemeAgencyID = DeliveryLocationAgencyID,
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

        public void _38PaymentMeans(string PaymentMeansID, string PaymentMeansValue, string PaymentDueDate, string PaymentChannelCode, string PaymentIDValue, string PayeeFinancialAccountID, string PayeeFinancialInstID)
        {

           
            creditnote.PaymentMeans = new List<PaymentMeansType>()
                {
                    new PaymentMeansType
                    {
                        PaymentMeansCode = new CodeType
                        {
                            listID =PaymentMeansID,
                            Value = PaymentMeansValue
                        },
                        PaymentDueDate = PaymentDueDate,
                        PaymentChannelCode = PaymentChannelCode,
                        PaymentID = new List<IdentifierType>()
                        {
                            new IdentifierType
                            {
                                Value = PaymentIDValue
                            }
                        },
                        PayeeFinancialAccount = new FinancialAccountType
                        {
                            ID = PayeeFinancialAccountID,
                            FinancialInstitutionBranch = new BranchType
                            {
                                FinancialInstitution = new FinancialInstitutionType
                                {
                                    ID = PayeeFinancialInstID
                                }
                            }
                        }
                    }
                };
           
        }

        public void _39TaxTotal(string currencyID, double Value)
        {
            creditnote.TaxTotal = new List<TaxTotalType>()
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
        public void _40AddTaxSubTotal(string TaxableAmountCurrencyID, double TaxableAmountValue, string TaxAmountCurrencyID, double TaxAmountValue, string TaxCategoryID, string TaxCategoryAgencyID, string TaxCategoryValue, double Percent, string TaxSchemeID, string TaxSchemeAgencyID, string TaxSchemeValue)
        {
            if (creditnote.TaxTotal.First().TaxSubtotal == null)
                creditnote.TaxTotal.First().TaxSubtotal = new List<TaxSubtotalType>();
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
                        schemeID = TaxCategoryID,
                        schemeAgencyID = TaxCategoryAgencyID,
                        Value = TaxCategoryValue
                    },
                    Percent = (decimal)Percent,
                    TaxScheme = new TaxSchemeType
                    {
                        ID = new IdentifierType
                        {
                            schemeID = TaxSchemeID,
                            schemeAgencyID = TaxSchemeAgencyID,
                            Value = TaxSchemeValue
                        }
                    }
                }
            };


            creditnote.TaxTotal.First().TaxSubtotal.Add(taxSub);


        }

        public void _41PaymentTermns(string Value)
        {
            creditnote.PaymentTerms = new List<PaymentTermsType>()
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

        public void _42LegalMonetary(string LineExtensionID, double LineExtensionValue, string TaxExclusiveID, double TaxExclusiveValue, string TaxInclusiveID, double TaxInclusive, string AllowanceTotalID, double AllowanceTotalValue, string ChargeTotalID, double ChargeTotalValue, string PrepaidID, double PrepaidValue, string PayableRoundingID, double PayableRoundingValue, string PayableID, double PayableValue)
        {
            creditnote.LegalMonetaryTotal = new MonetaryTotalType
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
                    Value = 1000M
                },
                PayableRoundingAmount = new AmountType
                {
                    currencyID = PayableRoundingID,
                    Value = (decimal)PrepaidValue
                },
                PayableAmount = new AmountType
                {
                    currencyID = PayableID,
                    Value = (decimal)PayableValue
        } };
    }

        public void _43AddCreditNoteLine(string ID, string InvQtyUnitCode, double InvQtyValue, string LineExtensionCurrencyID, double LineExtensionValue, string AccountingCost, string OrderLineReferenceID, string TaxTotalcurrencyID, double TaxTotalValue)
           
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
                      AccountingCost = AccountingCost,
                      OrderLineReference = new List<OrderLineReferenceType>()
                        {
                            new OrderLineReferenceType
                            {
                                LineID = OrderLineReferenceID
                            }
                        },
                      TaxTotal = new List<TaxTotalType>()
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
            if (creditnote.CreditNoteLine == null)
            {
                creditnote.CreditNoteLine = new List<CreditNoteLineType>();
            }
            creditnote.CreditNoteLine.Add(line);
  
            }
        
        public void _44AddCreditLineItem(string Name,string SellersItemIdentification,string StandardItemIdentificationID,string StandardItemIdentificationAgencyID, string StandardItemIdentificationValue, string ClassifiedTaxCategoryID, string ClassifiedTaxCategoryAgencyID,string ClassifiedTaxCategoryValue,double ClassifiedTaxPercent,string TaxSchemeID, string TaxSchemeAgencyID,string TaxSchemeValue,string PriceAmountCurrId,double PriceAmountValue,string BaseQuantityUnitCode,double BaseQuantityValue)
        {
            creditnote.CreditNoteLine.LastOrDefault().Item = new ItemType
            {
                /*Description = new List<TextType>()
                            {
                                new TextType
                                {
                                    languageID = "EN",
                                    Value = @"Processor: Intel Core 2 Duo SU9400 LV (1.4GHz). RAM:3MB. Screen 1440x900"
                                }
                            },*/
                Name = Name,
                SellersItemIdentification = new ItemIdentificationType
                {
                    ID = SellersItemIdentification
                },
                StandardItemIdentification = new ItemIdentificationType
                {
                    ID = new IdentifierType
                    {
                        schemeID = StandardItemIdentificationID,
                        schemeAgencyID = StandardItemIdentificationAgencyID,
                        Value = StandardItemIdentificationValue
                    }
                },
                /* CommodityClassification = new List<CommodityClassificationType>()
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
                             },*/
                ClassifiedTaxCategory = new List<TaxCategoryType>()
                            {
                                new TaxCategoryType
                                {
                                    ID = new IdentifierType
                                    {
                                        schemeID = ClassifiedTaxCategoryID,
                                        schemeAgencyID = ClassifiedTaxCategoryAgencyID,
                                        Value = ClassifiedTaxCategoryValue
                                    },
                                    Percent = (decimal) ClassifiedTaxPercent,
                                    TaxScheme = new TaxSchemeType
                                    {
                                        ID = new IdentifierType
                                        {
                                            schemeID = TaxSchemeID,
                                            schemeAgencyID = TaxSchemeAgencyID,
                                            Value = TaxSchemeValue
                                        }
                                    }
                                }
                            },
                /* AdditionalItemProperty = new List<ItemPropertyType>()
                             {
                                 new ItemPropertyType
                                 {
                                     Name = "Color",
                                     Value = "black"
                                 }
                             },*/
            };
            creditnote.CreditNoteLine.LastOrDefault().Price = new PriceType
            {
                PriceAmount = new AmountType
                {
                    currencyID = PriceAmountCurrId,
                    Value = (decimal)PriceAmountValue,
                },
                BaseQuantity = new QuantityType
                {
                    unitCode = BaseQuantityUnitCode,
                    Value = (decimal)BaseQuantityValue
                }/*,
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
                            },*/
            };
        }

        public void _45AddCreditLineItemAllowCharge(double MultiplierFactorNumeric, string currencyID, double Amount)
        {

            creditnote.CreditNoteLine.LastOrDefault().Price.AllowanceCharge = new List<AllowanceChargeType>()
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


        public void _86SaveCreditNote(string FilePath)
        {

            creditnote.Save(FilePath);
        }
        public string _87CreditNoteSendRequest(string RequestID,string RequestFilePathToSave, string ResponseFilePathToSave)
        {

            string Answer = "";
            MemoryStream str = new MemoryStream();
            creditnote.Save(str);
            str.Position = 0;
            StreamReader reader = new StreamReader(str);
            string text = reader.ReadToEnd();

            string strErrorCode = Global.HttpRequest(Global.URL, RequestID, text, ref Answer);

            if (ResponseFilePathToSave != "")
            {
                System.IO.File.WriteAllText(ResponseFilePathToSave, Answer);
            }

            if (strErrorCode != "")
            {
                return strErrorCode;
            }
            

            
        

            resp = Answer;

            return "";


        }

        public string _CreditNoteSendRequestTest(string Text,string RequestID, string RequestFilePathToSave, string ResponseFilePathToSave)
        {

            string Answer = "";
            

            string strErrorCode = Global.HttpRequest(Global.URL, RequestID, Text, ref Answer);

            if (ResponseFilePathToSave != "")
            {
                System.IO.File.WriteAllText(ResponseFilePathToSave, Answer);
            }

            if (strErrorCode != "")
            {
                return strErrorCode;
            }





            resp = Answer;

            return "";


        }

        public string _88GetResponseStatus()
        {
            return resp;
        }
      
     
    }



}
