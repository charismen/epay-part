using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using LOGIC.Base;
using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Plexform.Base.Accounting.Map
{
    #region XML Container
    #region REQUEST
    [XmlRoot(ElementName = "authenticationDTO")]
    public class AuthenticationDTO
    {
        [XmlElement(ElementName = "password")]
        public string Password { get; set; }
        [XmlElement(ElementName = "username")]
        public string Username { get; set; }
    }

    [XmlRoot(ElementName = "getProformaBillById")]
    public class GetProformaBillById
    {
        [XmlElement(ElementName = "authenticationDTO", Namespace = "urn:ifxforum-org:XSD:1")]
        public AuthenticationDTO AuthenticationDTO { get; set; }
        [XmlElement(ElementName = "billId", Namespace = "urn:ifxforum-org:XSD:1")]
        public string BillId { get; set; }
    }

    [XmlRoot(ElementName = "getCreditBillById")]
    public class GetCreditBillById
    {
        [XmlElement(ElementName = "authenticationDTO", Namespace = "urn:ifxforum-org:XSD:1")]
        public AuthenticationDTO AuthenticationDTO { get; set; }
        [XmlElement(ElementName = "billId", Namespace = "urn:ifxforum-org:XSD:1")]
        public string BillId { get; set; }
    }

    [XmlRoot(ElementName = "config")]
    public class Config
    {
        [XmlElement(ElementName = "customerCode")]
        public string CustomerCode { get; set; }
        [XmlElement(ElementName = "limit")]
        public string Limit { get; set; }
        [XmlElement(ElementName = "offset")]
        public string Offset { get; set; }
    }

    [XmlRoot(ElementName = "getAllOutStandingBillByCustomerCode")]
    public class GetAllOutStandingBillByCustomerCode
    {
        [XmlElement(ElementName = "authenticationDTO")]
        public AuthenticationDTO AuthenticationDTO { get; set; }
        [XmlElement(ElementName = "config")]
        public Config Config { get; set; }
    }

    [XmlRoot(ElementName = "paymentDTO")]
    public class PaymentDTO
    {
        [XmlElement(ElementName = "amount")]
        public string Amount { get; set; }
        [XmlElement(ElementName = "billId")]
        public string BillId { get; set; }
        [XmlElement(ElementName = "reference")]
        public string Reference { get; set; }
    }

    [XmlRoot(ElementName = "updatePayment", Namespace = "urn:ifxforum-org:XSD:1")]
    public class UpdatePayment
    {
        [XmlElement(ElementName = "authenticationDTO")]
        public AuthenticationDTO AuthenticationDTO { get; set; }
        [XmlElement(ElementName = "email")]
        public string Email { get; set; }
        [XmlElement(ElementName = "paymentDTO")]
        public PaymentDTO PaymentDTO { get; set; }
    }

    [XmlRoot(ElementName = "updatePaymentResponse", Namespace = "urn:ifxforum-org:XSD:1")]
    public class UpdatePaymentResponse
    {
        [XmlElement(ElementName = "returnupdatePaymentResponse")]
        public string ReturnString { get; set; }
    }
    #endregion REQUEST

    #region RESPONSE
    [XmlRoot(ElementName = "taxCategoryDTO")]
    public class TaxCategoryDTO
    {
        private string _Code;
        private string _Id;
        private string _Name;
        private string _Value;
        private string _Type;

        [XmlElement(ElementName = "code")]
        public string Code
        {
            get { return _Code; }
            set { _Code = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "id")]
        public string Id
        {
            get { return _Id; }
            set { _Id = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "name")]
        public string Name
        {
            get { return _Name; }
            set { _Name = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "value")]
        public string Value
        {
            get { return _Value; }
            set { _Value = value != null ? value.Trim() : value; }
        }
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Type
        {
            get { return _Type; }
            set { _Type = value != null ? value.Trim() : value; }
        }
    }

    [XmlRoot(ElementName = "creditVoteDTO")]
    public class CreditVoteDTO
    {
        private string _Code;
        private string _Id;
        private string _Type;

        [XmlElement(ElementName = "code")]
        public string Code
        {
            get { return _Code; }
            set { _Code = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "id")]
        public string Id
        {
            get { return _Id; }
            set { _Id = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "taxCategoryDTO")]
        public TaxCategoryDTO TaxCategoryDTO { get; set; }
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Type
        {
            get { return _Type; }
            set { _Type = value != null ? value.Trim() : value; }
        }
    }

    [XmlRoot(ElementName = "currencyDTO")]
    public class CurrencyDTO
    {
        private string _Name;
        private string _Rate;
        private string _Symbol;
        private string _Type;

        [XmlElement(ElementName = "name")]
        public string Name
        {
            get { return _Name; }
            set { _Name = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "rate")]
        public string Rate
        {
            get { return _Rate; }
            set { _Rate = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "symbol")]
        public string Symbol
        {
            get { return _Symbol; }
            set { _Symbol = value != null ? value.Trim() : value; }
        }
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Type
        {
            get { return _Type; }
            set { _Type = value != null ? value.Trim() : value; }
        }
    }

    [XmlRoot(ElementName = "countDTO")]
    public class CountDTO
    {
        private string _Balance;
        private string _CountLess30;
        private string _CountLess60;
        private string _CountMore60;
        private string _Type;

        [XmlElement(ElementName = "balance")]
        public string Balance
        {
            get { return _Balance; }
            set { _Balance = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "countLess30")]
        public string CountLess30
        {
            get { return _CountLess30; }
            set { _CountLess30 = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "countLess60")]
        public string CountLess60
        {
            get { return _CountLess60; }
            set { _CountLess60 = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "countMore60")]
        public string CountMore60
        {
            get { return _CountMore60; }
            set { _CountMore60 = value != null ? value.Trim() : value; }
        }
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Type
        {
            get { return _Type; }
            set { _Type = value != null ? value.Trim() : value; }
        }
    }

    [XmlRoot(ElementName = "customerDTO")]
    public class CustomerDTO
    {
        private string _Address;
        private string _Balance;
        private string _Code;
        private string _CreditLimit;
        private string _PmtDay;
        private string _Name;
        private string _SunDb;
        private string _Type;
        private string _TotalBill;

        [XmlElement(ElementName = "address")]
        public string Address
        {
            get { return _Address; }
            set { _Address = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "balance")]
        public string Balance
        {
            get { return _Balance; }
            set { _Balance = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "code")]
        public string Code
        {
            get { return _Code; }
            set { _Code = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "creditLimit")]
        public string CreditLimit
        {
            get { return _CreditLimit; }
            set { _CreditLimit = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "pmtDay")]
        public string PmtDay
        {
            get { return _PmtDay; }
            set { _PmtDay = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "countDTO")]
        public CountDTO CountDTO { get; set; }
        [XmlElement(ElementName = "name")]
        public string Name
        {
            get { return _Name; }
            set { _Name = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "sunDb")]
        public string SunDb
        {
            get { return _SunDb; }
            set { _SunDb = value != null ? value.Trim() : value; }
        }
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Type
        {
            get { return _Type; }
            set { _Type = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "totalBill")]
        public string TotalBill
        {
            get { return _TotalBill; }
            set { _TotalBill = value != null ? value.Trim() : value; }
        }
    }

    [XmlRoot(ElementName = "databaseDTO")]
    public class DatabaseDTO
    {
        private string _Id;
        private string _Name;
        private string _NeedProject;
        private string _Type;

        [XmlElement(ElementName = "id")]
        public string Id
        {
            get { return _Id; }
            set { _Id = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "name")]
        public string Name
        {
            get { return _Name; }
            set { _Name = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "needProject")]
        public string NeedProject
        {
            get { return _NeedProject; }
            set { _NeedProject = value != null ? value.Trim() : value; }
        }
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Type
        {
            get { return _Type; }
            set { _Type = value != null ? value.Trim() : value; }
        }
    }

    [XmlRoot(ElementName = "deliveryreference")]
    public class DeliveryReference
    {
        private string _Nil;

        [XmlAttribute(AttributeName = "nil", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Nil
        {
            get { return _Nil; }
            set { _Nil = value != null ? value.Trim() : value; }
        }
    }

    [XmlRoot(ElementName = "description")]
    public class Description
    {
        private string _Nil;

        [XmlAttribute(AttributeName = "nil", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Nil
        {
            get { return _Nil; }
            set { _Nil = value != null ? value.Trim() : value; }
        }
    }

    [XmlRoot(ElementName = "fileUrl")]
    public class FileUrl
    {
        private string _Nil;

        [XmlAttribute(AttributeName = "nil", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Nil
        {
            get { return _Nil; }
            set { _Nil = value != null ? value.Trim() : value; }
        }
    }

    [XmlRoot(ElementName = "files")]
    public class Files
    {
        private string _File;
        private string _Id;
        private string _Type;

        [XmlElement(ElementName = "description")]
        public Description Description { get; set; }
        [XmlElement(ElementName = "file")]
        public string File
        {
            get { return _File; }
            set { _File = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "fileUrl")]
        public FileUrl FileUrl { get; set; }
        [XmlElement(ElementName = "id")]
        public string Id
        {
            get { return _Id; }
            set { _Id = value != null ? value.Trim() : value; }
        }
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Type
        {
            get { return _Type; }
            set { _Type = value != null ? value.Trim() : value; }
        }
    }

    [XmlRoot(ElementName = "items")]
    public class Items
    {
        private string _Amount;
        private string _Code;
        private string _Description;
        private string _Id;
        private string _Quantity;
        private string _UnitPrice;
        private string _Type;

        [XmlElement(ElementName = "amount")]
        public string Amount
        {
            get { return _Amount; }
            set { _Amount = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "code")]
        public string Code
        {
            get { return _Code; }
            set { _Code = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "description")]
        public string Description
        {
            get { return _Description; }
            set { _Description = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "id")]
        public string Id
        {
            get { return _Id; }
            set { _Id = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "quantity")]
        public string Quantity
        {
            get { return _Quantity; }
            set { _Quantity = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "unitPrice")]
        public string UnitPrice
        {
            get { return _UnitPrice; }
            set { _UnitPrice = value != null ? value.Trim() : value; }
        }
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Type
        {
            get { return _Type; }
            set { _Type = value != null ? value.Trim() : value; }
        }
    }

    [XmlRoot(ElementName = "officerDTO")]
    public class OfficerDTO
    {
        private string _Id;
        private string _Name;
        private string _Type;

        [XmlElement(ElementName = "id")]
        public string Id
        {
            get { return _Id; }
            set { _Id = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "name")]
        public string Name
        {
            get { return _Name; }
            set { _Name = value != null ? value.Trim() : value; }
        }
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Type
        {
            get { return _Type; }
            set { _Type = value != null ? value.Trim() : value; }
        }
    }

    [XmlRoot(ElementName = "poNumber")]
    public class PoNumber
    {
        private string _Nil;

        [XmlAttribute(AttributeName = "nil", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Nil
        {
            get { return _Nil; }
            set { _Nil = value != null ? value.Trim() : value; }
        }
    }

    [XmlRoot(ElementName = "name")]
    public class Name
    {
        private string _Nil;

        [XmlAttribute(AttributeName = "nil", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Nil
        {
            get { return _Nil; }
            set { _Nil = value != null ? value.Trim() : value; }
        }
    }

    [XmlRoot(ElementName = "unitcmbDTO")]
    public class UnitcmbDTO
    {
        private string _Nil;

        [XmlAttribute(AttributeName = "nil", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Nil
        {
            get { return _Nil; }
            set { _Nil = value != null ? value.Trim() : value; }
        }
    }

    [XmlRoot(ElementName = "projectDTO")]
    public class ProjectDTO
    {
        private string _Code;
        private string _Id;
        private string _Type;

        [XmlElement(ElementName = "code")]
        public string Code
        {
            get { return _Code; }
            set { _Code = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "id")]
        public string Id
        {
            get { return _Id; }
            set { _Id = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "name")]
        public Name Name { get; set; }
        [XmlElement(ElementName = "unitcmbDTO")]
        public UnitcmbDTO UnitcmbDTO { get; set; }
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Type
        {
            get { return _Type; }
            set { _Type = value != null ? value.Trim() : value; }
        }
    }

    [XmlRoot(ElementName = "unitDTO")]
    public class UnitDTO
    {
        private string _Code;
        private string _Id;
        private string _Option;
        private string _Type;

        [XmlElement(ElementName = "code")]
        public string Code
        {
            get { return _Code; }
            set { _Code = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "id")]
        public string Id
        {
            get { return _Id; }
            set { _Id = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "option")]
        public string Option
        {
            get { return _Option; }
            set { _Option = value != null ? value.Trim() : value; }
        }
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Type
        {
            get { return _Type; }
            set { _Type = value != null ? value.Trim() : value; }
        }
    }

    [XmlRoot(ElementName = "return")]
    public class Return
    {
        private string _Attention { get; set; }
        private string _BillNo { get; set; }
        private string _CustomerReference;
        private string _DateReceived { get; set; }
        private string _DateRegistered { get; set; }
        private string _DivisionReference { get; set; }
        private string _Id { get; set; }
        private string _LabReference { get; set; }
        private string _Type { get; set; }
        private string _Ax21 { get; set; }
        private string _Ax23 { get; set; }
        private string _Xsi { get; set; }

        [XmlElement(ElementName = "attention")]
        public string Attention
        {
            get { return _Attention; }
            set { _Attention = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "billNo")]
        public string BillNo
        {
            get { return _BillNo; }
            set { _BillNo = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "creditVoteDTO")]
        public CreditVoteDTO CreditVoteDTO { get; set; }
        [XmlElement(ElementName = "currencyDTO")]
        public CurrencyDTO CurrencyDTO { get; set; }
        [XmlElement(ElementName = "customerDTO")]
        public CustomerDTO CustomerDTO { get; set; }
        [XmlElement(ElementName = "customerReference")]
        public string CustomerReference
        {
            get { return _CustomerReference; }
            set { _CustomerReference = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "databaseDTO")]
        public DatabaseDTO DatabaseDTO { get; set; }
        [XmlElement(ElementName = "dateReceived")]
        public string DateReceived
        {
            get { return _DateReceived; }
            set { _DateReceived = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "dateRegistered")]
        public string DateRegistered
        {
            get { return _DateRegistered; }
            set { _DateRegistered = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "deliveryreference")]
        public DeliveryReference Deliveryreference { get; set; }
        [XmlElement(ElementName = "divisionReference")]
        public string DivisionReference
        {
            get { return _DivisionReference; }
            set { _DivisionReference = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "files")]
        public Files Files { get; set; }
        [XmlElement(ElementName = "id")]
        public string Id
        {
            get { return _Id; }
            set { _Id = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "items")]
        public List<Items> Items { get; set; }
        [XmlElement(ElementName = "labReference")]
        public string LabReference
        {
            get { return _LabReference; }
            set { _LabReference = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "officerDTO")]
        public OfficerDTO OfficerDTO { get; set; }
        [XmlElement(ElementName = "poNumber")]
        public PoNumber PoNumber { get; set; }
        [XmlElement(ElementName = "projectDTO")]
        public ProjectDTO ProjectDTO { get; set; }
        [XmlElement(ElementName = "unitDTO")]
        public UnitDTO UnitDTO { get; set; }
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Type
        {
            get { return _Type; }
            set { _Type = value != null ? value.Trim() : value; }
        }
        [XmlAttribute(AttributeName = "ax21", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ax21
        {
            get { return _Ax21; }
            set { _Ax21 = value != null ? value.Trim() : value; }
        }
        [XmlAttribute(AttributeName = "ax23", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ax23
        {
            get { return _Ax23; }
            set { _Ax23 = value != null ? value.Trim() : value; }
        }
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi
        {
            get { return _Xsi; }
            set { _Xsi = value != null ? value.Trim() : value; }
        }
    }

    [XmlRoot(ElementName = "returngetCreditBillByIdResponse")]
    public class ReturnCreditGetCreditBillByIdResponse
    {
        private string _Attention;
        private string _BalanceLimit;
        private string _BillNo;
        private string _CreditLimit;
        private string _CustomerReference;
        private string _DateReceived;
        private string _DateRegistered;
        private string _DivisionReference;
        private string _Id;
        private string _LabReference;
        private string _TotalBill;
        private string _Type;
        private string _Ax21;
        private string _Ax23;
        private string _Xsi;

        [XmlElement(ElementName = "attention")]
        public string Attention
        {
            get { return _Attention; }
            set { _Attention = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "balanceLimit")]
        public string BalanceLimit
        {
            get { return _BalanceLimit; }
            set { _BalanceLimit = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "billNo")]
        public string BillNo
        {
            get { return _BillNo; }
            set { _BillNo = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "creditLimit")]
        public string CreditLimit
        {
            get { return _CreditLimit; }
            set { _CreditLimit = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "creditVoteDTO")]
        public CreditVoteDTO CreditVoteDTO { get; set; }
        [XmlElement(ElementName = "customerDTO")]
        public CustomerDTO CustomerDTO { get; set; }
        [XmlElement(ElementName = "customerReference")]
        public string CustomerReference { get; set; }
        [XmlElement(ElementName = "databaseDTO")]
        public DatabaseDTO DatabaseDTO { get; set; }
        [XmlElement(ElementName = "dateReceived")]
        public string DateReceived
        {
            get { return _DateReceived; }
            set { _DateReceived = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "dateRegistered")]
        public string DateRegistered
        {
            get { return _DateRegistered; }
            set { _DateRegistered = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "deliveryreference")]
        public DeliveryReference Deliveryreference { get; set; }
        [XmlElement(ElementName = "divisionReference")]
        public string DivisionReference
        {
            get { return _DivisionReference; }
            set { _DivisionReference = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "id")]
        public string Id
        {
            get { return _Id; }
            set { _Id = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "items")]
        public List<Items> Items { get; set; }
        [XmlElement(ElementName = "labReference")]
        public string LabReference
        {
            get { return _LabReference; }
            set { _LabReference = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "officerDTO")]
        public OfficerDTO OfficerDTO { get; set; }
        [XmlElement(ElementName = "projectDTO")]
        public ProjectDTO ProjectDTO { get; set; }
        [XmlElement(ElementName = "totalBill")]
        public string TotalBill
        {
            get { return _TotalBill; }
            set { _TotalBill = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "unitDTO")]
        public UnitDTO UnitDTO { get; set; }
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Type
        {
            get { return _Type; }
            set { _Type = value != null ? value.Trim() : value; }
        }
        [XmlAttribute(AttributeName = "ax21", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ax21
        {
            get { return _Ax21; }
            set { _Ax21 = value != null ? value.Trim() : value; }
        }
        [XmlAttribute(AttributeName = "ax23", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ax23
        {
            get { return _Ax23; }
            set { _Ax23 = value != null ? value.Trim() : value; }
        }
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi
        {
            get { return _Xsi; }
            set { _Xsi = value != null ? value.Trim() : value; }
        }
    }

    [XmlRoot(ElementName = "getProformaBillByIdResponse", Namespace = "urn:ifxforum-org:XSD:1")]
    public class GetProformaBillByIdResponse
    {
        [XmlElement(ElementName = "return")]
        public Return Return { get; set; }
    }

    [XmlRoot(ElementName = "getCreditBillByIdResponse", Namespace = "urn:ifxforum-org:XSD:1")]
    public class GetCreditBillByIdResponse
    {
        [XmlElement(ElementName = "returngetCreditBillByIdResponse")]
        public ReturnCreditGetCreditBillByIdResponse Return { get; set; }
    }

    [XmlRoot(ElementName = "data")]
    public class Data
    {
        private string _Amount;
        private string _Balance;
        private string _BillNo;
        private string _Company;
        private string _Date;
        private string _Detail;
        private string _Gst;
        private string _Id;
        private string _Rounding;
        private string _SiNumber;
        private string _Status;
        private string _Type;

        [XmlElement(ElementName = "amount")]
        public string Amount
        {
            get { return _Amount; }
            set { _Amount = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "balance")]
        public string Balance
        {
            get { return _Balance; }
            set { _Balance = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "billNo")]
        public string BillNo
        {
            get { return _BillNo; }
            set { _BillNo = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "company")]
        public string Company
        {
            get { return _Company; }
            set { _Company = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "date")]
        public string Date
        {
            get { return _Date; }
            set { _Date = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "detail")]
        public string Detail
        {
            get { return _Detail; }
            set { _Detail = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "gst")]
        public string Gst
        {
            get { return _Gst; }
            set { _Gst = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "id")]
        public string Id
        {
            get { return _Id; }
            set { _Id = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "rounding")]
        public string Rounding
        {
            get { return _Rounding; }
            set { _Rounding = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "siNumber")]
        public string SiNumber
        {
            get { return _SiNumber; }
            set { _SiNumber = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "status")]
        public string Status
        {
            get { return _Status; }
            set { _Status = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "type")]
        public string Type
        {
            get { return _Type; }
            set { _Type = value != null ? value.Trim() : value; }
        }
    }

    [XmlRoot(ElementName = "returngetAllOutStandingBillByCustomerCodeResponse")]
    public class ReturnGetAllOutStandingBillByCustomerCodeResponse
    {
        private string _Offset;
        private string _TotalLength;
        private string _Type;
        private string _Ax21;
        private string _Ax23;
        private string _Xsi;

        [XmlElement(ElementName = "data")]
        public List<Data> Data { get; set; }
        [XmlElement(ElementName = "offset")]
        public string Offset
        {
            get { return _Offset; }
            set { _Offset = value != null ? value.Trim() : value; }
        }
        [XmlElement(ElementName = "totalLength")]
        public string TotalLength
        {
            get { return _TotalLength; }
            set { _TotalLength = value != null ? value.Trim() : value; }
        }
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Type
        {
            get { return _Type; }
            set { _Type = value != null ? value.Trim() : value; }
        }
        [XmlAttribute(AttributeName = "ax21", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ax21
        {
            get { return _Ax21; }
            set { _Ax21 = value != null ? value.Trim() : value; }
        }
        [XmlAttribute(AttributeName = "ax23", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ax23
        {
            get { return _Ax23; }
            set { _Ax23 = value != null ? value.Trim() : value; }
        }
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi
        {
            get { return _Xsi; }
            set { _Xsi = value != null ? value.Trim() : value; }
        }
    }

    [XmlRoot(ElementName = "getAllOutStandingBillByCustomerCodeResponse", Namespace = "urn:ifxforum-org:XSD:1")]
    public class GetAllOutStandingBillByCustomerCodeResponse
    {
        [XmlElement(ElementName = "returngetAllOutStandingBillByCustomerCodeResponse")]
        public ReturnGetAllOutStandingBillByCustomerCodeResponse ReturnGetAllOutStandingBillByCustomerCodeResponse { get; set; }
    }

    [XmlRoot(ElementName = "Fault", Namespace = "urn:ifxforum-org:XSD:1")]
    public class Fault
    {
        [XmlElement(ElementName = "faultcode")]
        public string FaultCode { get; set; }
        [XmlElement(ElementName = "faultstring")]
        public string FaultString { get; set; }
        [XmlElement(ElementName = "detail")]
        public string Detail { get; set; }
        [XmlElement(ElementName = "reason")]
        public string Reason { get; set; }
    }
    #endregion RESPONSE

    #region MAIN
    [XmlRoot(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class Body
    {
        [XmlElement(ElementName = "getProformaBillById", Namespace = "urn:ifxforum-org:XSD:1")]
        public GetProformaBillById GetProformaBillById { get; set; }

        [XmlElement(ElementName = "getProformaBillByIdResponse", Namespace = "urn:ifxforum-org:XSD:1")]
        public GetProformaBillByIdResponse GetProformaBillByIdResponse { get; set; }

        [XmlElement(ElementName = "getCreditBillById", Namespace = "urn:ifxforum-org:XSD:1")]
        public GetCreditBillById GetCreditBillById { get; set; }

        [XmlElement(ElementName = "getCreditBillByIdResponse", Namespace = "urn:ifxforum-org:XSD:1")]
        public GetCreditBillByIdResponse GetCreditBillByIdResponse { get; set; }

        [XmlElement(ElementName = "getAllOutStandingBillByCustomerCode", Namespace = "urn:ifxforum-org:XSD:1")]
        public GetAllOutStandingBillByCustomerCode GetAllOutStandingBillByCustomerCode { get; set; }

        [XmlElement(ElementName = "getAllOutStandingBillByCustomerCodeResponse", Namespace = "urn:ifxforum-org:XSD:1")]
        public GetAllOutStandingBillByCustomerCodeResponse GetAllOutStandingBillByCustomerCodeResponse { get; set; }

        [XmlElement(ElementName = "updatePayment", Namespace = "urn:ifxforum-org:XSD:1")]
        public UpdatePayment UpdatePayment { get; set; }

        [XmlElement(ElementName = "updatePaymentResponse", Namespace = "urn:ifxforum-org:XSD:1")]
        public UpdatePaymentResponse UpdatePaymentResponse { get; set; }

        [XmlElement(ElementName = "Fault", Namespace = "urn:ifxforum-org:XSD:1")]
        public Fault Fault { get; set; }
    }

    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class Envelope
    {
        [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public Body Body { get; set; }
        [XmlAttribute(AttributeName = "soapenv", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Soapenv { get; set; }
    }
    #endregion MAIN
    #endregion XML Container

    public class POSTXMLResponse
    {
        public bool IsSuccess { get; set; }
        public string XMLResult { get; set; }
    }

    public class POSTUpdatePayment
    {
        public string Email { get; set; }
        public string Amount { get; set; }
        public string BillID { get; set; }
        public string Reference { get; set; }
    }

    public class MapFPXBody
    {
        public string fpx_buyerAccNo { get; set; }
        public string fpx_buyerBankBranch { get; set; }
        public string fpx_buyerBankId { get; set; }
        public string fpx_buyerEmail { get; set; }
        public string fpx_buyerIban { get; set; }
        public string fpx_buyerId { get; set; }
        public string fpx_buyerName { get; set; }
        public string fpx_makerName { get; set; }
        public string fpx_msgToken { get; set; }
        public string fpx_msgType { get; set; }
        public string fpx_productDesc { get; set; }
        public string fpx_sellerBankCode { get; set; }
        public string fpx_sellerExId { get; set; }
        public string fpx_sellerExOrderNo { get; set; }
        public string fpx_sellerId { get; set; }
        public string fpx_sellerOrderNo { get; set; }
        public string fpx_sellerTxnTime { get; set; }
        public string fpx_txnAmount { get; set; }
        public string fpx_txnCurrency { get; set; }
        public string fpx_version { get; set; }
    }

    public class MapMBBBody
    {
        public string MERCHANT_ACC_NO { get; set; }
        public string MERCHANT_TRANID { get; set; }
        public string AMOUNT { get; set; }
        public string TRANSACTION_TYPE { get; set; }
        public string TXN_SIGNATURE { get; set; }
        public string RESPONSE_TYPE { get; set; }
        public string RETURN_URL { get; set; }
        public string TXN_DESC { get; set; }
        public string CUSTOMER_ID { get; set; }
        public string FR_HIGHRISK_EMAIL { get; set; }
        public string FR_HIGHRISK_COUNTRY { get; set; }
        public string FR_BILLING_ADDRESS { get; set; }
        public string FR_SHIPPING_ADDRESS { get; set; }
        public string FR_SHIPPING_COST { get; set; }
        public string FR_PURCHASE_HOUR { get; set; }
        public string FR_CUSTOMER_IP { get; set; }
        public string TRANSACTION_ID { get; set; }
    }

    public class MapListCreditBills
    {
        [StringLength(200)]
        [JsonProperty("description")]
        [Grid(Width = 250)]
        public virtual string Description { get; set; }

        [StringLength(20)]
        [JsonProperty("billno")]
        [Grid(Width = 200)]
        public virtual string BillNo { get; set; }

        [StringLength(20)]
        [JsonProperty("treference")]
        [Grid(Width = 300)]
        public virtual string TReference { get; set; }

        [StringLength(20)]
        [JsonProperty("d_c")]
        [Grid(Width = 130, IsSortable = false)]
        public virtual string D_C { get; set; }

        [StringLength(20)]
        [JsonProperty("add_code")]
        [Grid(Width = 100, IsSortable = false)]
        public virtual string Add_Code { get; set; }

        [StringLength(20)]
        [JsonProperty("rec_type")]
        [Grid(Width = 100, IsSortable = false)]
        public virtual string Rec_Type { get; set; }

        [StringLength(20)]
        [JsonProperty("trans_date")]
        [Grid(Width = 100, IsSortable = false)]
        public virtual string Trans_Date { get; set; }

        [StringLength(20)]
        [JsonProperty("amount")]
        [Grid(Width = 100, IsSortable = false)]
        public virtual string Amount { get; set; }

        [StringLength(20)]
        [JsonProperty("accnt_code")]
        [Grid(Width = 160)]
        public virtual string Accnt_Code { get; set; }

        [StringLength(20)]
        [JsonProperty("jrnal_no")]
        [Grid(Width = 130)]
        public virtual string Jrnal_No { get; set; }

        [StringLength(50)]
        [JsonProperty("trans_ref")]
        [Grid(Width = 300)]
        public virtual string Trans_Ref { get; set; }

        [StringLength(50)]
        [JsonProperty("sun_db")]
        [Grid(IsVisible = false)]
        public virtual string Sun_Db { get; set; }

        [StringLength(50)]
        [JsonProperty("allocation")]
        [Grid(IsVisible = false)]
        public virtual string Allocation { get; set; }

        [StringLength(50)]
        [JsonProperty("inv_date")]
        [Grid(IsVisible = false)]
        public virtual string Inv_Date { get; set; }

        [StringLength(50)]
        [JsonProperty("status")]
        [Grid(IsVisible = false)]
        public virtual string Status { get; set; }

        [StringLength(50)]
        [JsonProperty("comments")]
        [Grid(IsVisible = false)]
        public virtual string Comments { get; set; }

        [StringLength(50)]
        [JsonProperty("cust_code")]
        [Grid(IsVisible = false)]
        public virtual string Cust_Code { get; set; }

        [StringLength(50)]
        [JsonProperty("trans_val")]
        [Grid(IsVisible = false)]
        public virtual string Trans_Val { get; set; }

        [StringLength(50)]
        [JsonProperty("del_date")]
        [Grid(IsVisible = false)]
        public virtual string Del_Date { get; set; }

        [StringLength(50)]
        [JsonProperty("anal_t1")]
        [Grid(IsVisible = false)]
        public virtual string Anal_T1 { get; set; }

        [StringLength(50)]
        [JsonProperty("cust_ref")]
        [Grid(IsVisible = false)]
        public virtual string Cust_Ref { get; set; }

        [StringLength(50)]
        [JsonProperty("address_1")]
        [Grid(IsVisible = false)]
        public virtual string Address_1 { get; set; }

        [StringLength(50)]
        [JsonProperty("address_2")]
        [Grid(IsVisible = false)]
        public virtual string Address_2 { get; set; }

        [StringLength(50)]
        [JsonProperty("address_3")]
        [Grid(IsVisible = false)]
        public virtual string Address_3 { get; set; }

        [StringLength(50)]
        [JsonProperty("address_4")]
        [Grid(IsVisible = false)]
        public virtual string Address_4 { get; set; }

        [StringLength(50)]
        [JsonProperty("address_5")]
        [Grid(IsVisible = false)]
        public virtual string Address_5 { get; set; }

        [StringLength(50)]
        [JsonProperty("address_6")]
        [Grid(IsVisible = false)]
        public virtual string Address_6 { get; set; }

        [StringLength(50)]
        [JsonProperty("e_mail")]
        [Grid(IsVisible = false)]
        public virtual string E_Mail { get; set; }
    }

	public class MapListCreditBillsDetail
    {
        [StringLength(20)]
        [JsonProperty("rec_type")]
        [Grid(Width = 130)]
        public virtual string REC_TYPE { get; set; }

        [StringLength(20)]
        [JsonProperty("trans_line")]
        [Grid(Width = 300)]
        public virtual string TRANS_REF { get; set; }

        [StringLength(20)]
        [JsonProperty("inv_no")]
        [Grid(Width = 200)]
        public virtual string INV_NO { get; set; }

        [StringLength(20)]
        [JsonProperty("accnt_code")]
        [Grid(Width = 100)]
        public virtual string ACCNT_CODE { get; set; }

        [StringLength(50)]
        [JsonProperty("item_code")]
        [Grid(Width = 100)]
        public virtual string ITEM_CODE { get; set; }

        [StringLength(500)]
        [JsonProperty("descriptn")]
        [Grid(Width = 250)]
        public virtual string DESCRIPTN { get; set; }

        [StringLength(20)]
        [JsonProperty("value_1")]
        [Grid(Width = 150)]
        public virtual string VALUE_1 { get; set; }

        [StringLength(20)]
        [JsonProperty("value_2")]
        [Grid(Width = 150)]
        public virtual string VALUE_2 { get; set; }

        [StringLength(20)]
        [JsonProperty("value_10")]
        [Grid(Width = 300)]
        public virtual string VALUE_10 { get; set; }

        [StringLength(20)]
        [JsonProperty("inv_date")]
        [Grid(Width = 100)]
        public virtual string INV_DATE { get; set; }

        [StringLength(20)]
        [JsonProperty("id_entered")]
        [Grid(Width = 100)]
        public virtual string ID_ENTERED { get; set; }

        [StringLength(20)]
        [JsonProperty("id_invoiced")]
        [Grid(Width = 100)]
        public virtual string ID_INVOICED { get; set; }

        [StringLength(20)]
        [JsonProperty("anal_m1")]
        [Grid(Width = 130)]
        public virtual string ANAL_M1 { get; set; }

        [StringLength(20)]
        [JsonProperty("anal_m3")]
        [Grid(Width = 130)]
        public virtual string ANAL_M3 { get; set; }

        [StringLength(20)]
        [JsonProperty("anal_m6")]
        [Grid(Width = 130)]
        public virtual string ANAL_M6 { get; set; }

        [StringLength(20)]
        [JsonProperty("anal_m9")]
        [Grid(Width = 130)]
        public virtual string ANAL_M9 { get; set; }
    }

    public class MapCreditBillTax
    {
        [JsonProperty("amount")]
        public virtual string AMOUNT { get; set; }
    }

    public class MapListInvoice
    {
        [Required, StringLength(50)]
        [JsonProperty("billno")]
        [Grid(Width = 155, IsSortable = false)]
        public virtual string BillNo { get; set; }

        [JsonProperty("bizregid")]
        [StringLength(50)]
        public virtual string BizRegID { get; set; }

        [JsonProperty("company")]
        [StringLength(4000)]
        [Grid(Width = 200, Permissions = "Pages.Account.Payment.Offline")]
        public virtual string Company { get; set; }

        [JsonProperty("date")]
        [Grid(Width = 100, DatetimeFormat = "DD/MM/YYYY")]
        public virtual System.DateTime Date { get; set; }

        [JsonProperty("id")]
        [StringLength(25)]
        public virtual string Id { get; set; }

        [JsonProperty("detail")]
        [StringLength(4000)]
        [Grid(Width = 225, IsSortable = false)]
        public virtual string Detail { get; set; }

        [JsonProperty("sinumber")]
        [StringLength(25)]
        public virtual string SiNumber { get; set; }

        [JsonProperty("gst")]
        public virtual decimal? Gst { get; set; }

        [JsonProperty("amount")]
        //[Grid(Width = 100, IsSortable = false, Alignment = ColumnsAlignment.Right)]
        public virtual decimal? Amount { get; set; }

        [JsonProperty("totalamounts")]
        [Grid(Width = 100, IsSortable = false, Alignment = ColumnsAlignment.Right)]
        public virtual decimal? TotalAmounts { get; set; }

        [JsonProperty("balance")]
        public virtual decimal? Balance { get; set; }

        [JsonProperty("rounding")]
        public virtual decimal? Rounding { get; set; }

        [JsonProperty("type")]
        [StringLength(25)]
        [Grid(Width = 100, Alignment = ColumnsAlignment.Center)]
        public virtual string Type { get; set; }

        [JsonProperty("status")]
        [StringLength(50)]
        [Grid(Width = 145, StatusBadges = new string[] { "Pending Payment|info", "Paid|success", "Acceptance By UKA|primary", "Officer Approval|primary" })]
        public virtual string Status { get; set; }

        [JsonProperty("creditbillcount")]
        public virtual int CreditBillCount { get; set; }

        [JsonProperty("creditbillamount")]
        public virtual decimal? CreditBillAmount { get; set; }

        [JsonProperty("proformacount")]
        public virtual int ProformaCount { get; set; }

        [JsonProperty("proformaamount")]
        public virtual decimal? ProformaAmount { get; set; }

        [JsonProperty("totalamount")]
        public virtual decimal? TotalAmount { get; set; }
    }

    public class MapListInvoice_A
    {
        [Required, StringLength(50)]
        [JsonProperty("invoiceno")]
        [Grid(Width = 115, Alignment = ColumnsAlignment.Center, IsSortable = false)]
        public virtual string InvoiceNo { get; set; }

        [JsonProperty("swttano")]
        [Grid(Width = 100, IsSortable = false, Permissions = "Pages.Account.Invoicelist.Approver")]
        public virtual string SWTTANo { get; set; }

        [JsonProperty("company")]
        [StringLength(4000)]
        [Grid(Width = 225, IsSortable = false, Permissions = "Pages.Account.Invoicelist.Approver")]
        public virtual string Company { get; set; }

        [JsonProperty("date")]
        [Grid(Width = 125, Alignment = ColumnsAlignment.Center, DatetimeFormat = "DD/MM/YYYY")]
        public virtual System.DateTime Date { get; set; }

        [JsonProperty("fpxid")]
        public virtual string FPXId { get; set; }

        [JsonProperty("amount")]
        [Grid(Width = 125, IsSortable = false)]
        public virtual decimal? Amount { get; set; }

        [JsonProperty("bank")]
        public virtual string Bank { get; set; }

        [JsonProperty("status")]
        [StringLength(50)]
        [Grid(Width = 100,Permissions = "Pages.Account.Invoicelist.Approver")]
        public virtual string Status { get; set; }

        [JsonProperty("totalinvoice")]
        public virtual int TotalInvoice { get; set; }

        [JsonProperty("totaloverdue")]
        public virtual int TotalOverdue { get; set; }

        [JsonProperty("totaloverdueamount")]
        public virtual decimal? TotalOverdueAmount { get; set; }

        [JsonProperty("totalamount")]
        public virtual decimal? TotalAmount { get; set; }

        [JsonProperty("overduepercent")]
        public virtual decimal? OverduePercent { get; set; }
    }

    public class MapListInvoiceDetail
    {
        [Required, StringLength(50)]
        [JsonProperty("itemcode")]
        [Grid(Alignment = ColumnsAlignment.Center, IsSortable = false)]
        public virtual string ItemCode { get; set; }

        [JsonProperty("description")]
        [Grid(IsSortable = false)]
        public virtual string Description { get; set; }

        [JsonProperty("taxcode")]
        [Grid(IsSortable = false)]
        public virtual string TaxCode { get; set; }

        [JsonProperty("quantity")]
        [Grid(IsSortable = false)]
        public virtual int Quantity { get; set; }

        [JsonProperty("unitprice")]
        [Grid(IsSortable = false, Alignment = ColumnsAlignment.Right)]
        public virtual decimal? UnitPrice { get; set; }

        [JsonProperty("subtotal")]
        [Grid(IsSortable = false, Alignment = ColumnsAlignment.Right)]
        public virtual decimal? SubTotal { get; set; }
    }

    public class MapListInvoicePayDetail
    {
        [Required, StringLength(50)]
        [JsonProperty("documentno")]
        [Grid(IsSortable = false)]
        public virtual string DocumentNo { get; set; }

        [JsonProperty("documenttypecode")]
        [Grid]
        public virtual byte? DocumentTypeCode { get; set; }

        [JsonProperty("documenttype")]
        [Grid(Width = 250, IsSortable = false)]
        public virtual string DocumentType { get; set; }

        [JsonProperty("amount")]
        [Grid(IsSortable = false, Alignment = ColumnsAlignment.Right)]
        public virtual decimal? Amount { get; set; }

        [JsonProperty("documentdate")]
        [Grid(DatetimeFormat = "DD/MM/YYYY")]
        public virtual System.DateTime DocumentDate { get; set; }
    }

    public class MapListTransactionHistory
    {
        [Required, StringLength(50)]
        [JsonProperty("billno")]
        [Grid(Width = 155)]
        public virtual string BillNo { get; set; }

        [StringLength(20)]
        [JsonProperty("transno")]
        [Grid(Width = 125)]
        public virtual string TransNo { get; set; }

        [StringLength(4000)]
        [JsonProperty("company")]
        [Grid(Width = 200, Permissions = "Pages.Account.Payment.Offline")]
        public virtual string Company { get; set; }

        [JsonProperty("acctno")]
        [Grid]
        public virtual string AcctNo { get; set; }

        [JsonProperty("transdate")]
        [Grid(Width = 100, Alignment = ColumnsAlignment.Center, DatetimeFormat = "DD/MM/YYYY")]
        public virtual System.DateTime? TransDate { get; set; }

        [JsonProperty("amount")]
        [Grid(Width = 100)]
        public virtual decimal? Amount { get; set; }

        [JsonProperty("transstatus")]
        public virtual byte? TransStatus { get; set; }

        [JsonProperty("status")]
        public virtual byte? Status { get; set; }

        [JsonProperty("fpxstatus")]
        [StringLength(50)]
        public virtual string FpxStatus { get; set; }

        [JsonProperty("paidstatus")]
        [StringLength(50)]
        [Grid(Width = 115, Alignment = ColumnsAlignment.Center, StatusBadges = new string[] { "Pending|info", "Void|warning", "Paid|success", "Rejected|danger", "Cancelled|danger" })]
        public virtual string PaidStatus { get; set; }

        [JsonProperty("type")]
        [StringLength(25)]
        [Grid(Width = 100, Alignment = ColumnsAlignment.Center)]
        public virtual string Type { get; set; }

        [JsonProperty("createdate")]
        public virtual System.DateTime? CreateDate { get; set; }

        [JsonProperty("invSvcID")]
        public virtual string InSvcID { get; set; }
    }

    public class MapListTransactionHistory_A
    {
        [Required, StringLength(20)]
        [JsonProperty("transno")]
        [Grid(Alignment = ColumnsAlignment.Center, IsSortable = false)]
        public virtual string TransNo { get; set; }

        [StringLength(4000)]
        [JsonProperty("company")]
        [Grid(Permissions = "Pages.Account.Invoicelist.Approver")]
        public virtual string Company { get; set; }

        [JsonProperty("acctno")]
        [Grid]
        public virtual string AcctNo { get; set; }

        [JsonProperty("transdate")]
        [Grid(DatetimeFormat = "DD/MM/YYYY", Alignment = ColumnsAlignment.Center)]
        public virtual System.DateTime? TransDate { get; set; }

        [StringLength(50)]
        [JsonProperty("fpxid")]
        [Grid(Alignment = ColumnsAlignment.Center, IsSortable = false)]
        public virtual string FPXID { get; set; }

        [JsonProperty("amount")]
        [Grid]
        public virtual decimal? Amount { get; set; }

        [JsonProperty("bank")]
        [Grid(Alignment = ColumnsAlignment.Center, IsSortable = false)]
        public virtual string Bank { get; set; }

        [JsonProperty("transstatus")]
        public virtual byte? TransStatus { get; set; }

        [JsonProperty("status")]
        public virtual byte? Status { get; set; }

        [JsonProperty("fpxstatus")]
        [StringLength(50)]
        [Grid(IsVisible = false, Alignment = ColumnsAlignment.Center)]
        public virtual string FpxStatus { get; set; }

        [JsonProperty("paidstatusq")]
        [StringLength(50)]
        [Grid(Alignment = ColumnsAlignment.Center, StatusBadges = new string[] { "Pending|info", "Cancelled|warning", "Paid|success", "Rejected|danger", "Fail|danger" })]
        public virtual string PaidStatusQ { get; set; }

        [JsonProperty("type")]
        [StringLength(25)]
        public virtual string Type { get; set; }

        [JsonProperty("createdate")]
        public virtual System.DateTime? CreateDate { get; set; }

        [JsonProperty("invSvcID")]
        public virtual string InSvcID { get; set; }
    }

    public class MapListPendingPaymentStatus
    {
        [Required, StringLength(20)]
        [JsonProperty("transno")]
        public virtual string TransNo { get; set; }

        [Required, StringLength(50)]
        [JsonProperty("billno")]
        [Grid]
        public virtual string BillNo { get; set; }

        [JsonProperty("bizregid")]
        [StringLength(50)]
        public virtual string BizRegID { get; set; }

        [JsonProperty("batchcode")]
        [StringLength(20)]
        public virtual string BatchCode { get; set; }

        [JsonProperty("monthcode")]
        [StringLength(10)]
        public virtual string MonthCode { get; set; }

        [JsonProperty("shiftcode")]
        [StringLength(10)]
        public virtual string ShiftCode { get; set; }

        [JsonProperty("transdate")]
        [Grid(DatetimeFormat = "YYYY/MM/DD")]
        public virtual System.DateTime? TransDate { get; set; }

        [JsonProperty("transstarttime")]
        [StringLength(6)]
        public virtual string TransStartTime { get; set; }

        [JsonProperty("paidamount")]
        public virtual decimal? PaidAmount { get; set; }

        [JsonProperty("invoiceamount")]
        public virtual decimal? InvoiceAmount { get; set; }

        [JsonProperty("custprivilege")]
        [StringLength(255)]
        public virtual string CustPrivilege { get; set; }

        [JsonProperty("paymentstatus")]
        public virtual byte? PaymentStatus { get; set; }

        [JsonProperty("invoicestatus")]
        public virtual byte? InvoiceStatus { get; set; }

        [JsonProperty("refinfo")]
        [StringLength(4000)]
        public virtual string RefInfo { get; set; }

        [JsonProperty("ccrefinfo")]
        [StringLength(255)]
        public virtual string CCrefInfo { get; set; }
    }

    public class MapListTransactionDetail
    {
        [Required, StringLength(50)]
        [JsonProperty("billno")]
        [Grid]
        public virtual string BillNo { get; set; }

        [StringLength(20)]
        [JsonProperty("transno")]
        [Grid]
        public virtual string TransNo { get; set; }

        [StringLength(4000)]
        [JsonProperty("company")]
        [Grid]
        public virtual string Company { get; set; }

        [JsonProperty("acctno")]
        [Grid]
        public virtual string AcctNo { get; set; }

        [JsonProperty("transdate")]
        [Grid(DatetimeFormat = "YYYY/MM/DD")]
        public virtual System.DateTime? TransDate { get; set; }

        [JsonProperty("amount")]
        [Grid]
        public virtual decimal? Amount { get; set; }

        [JsonProperty("transstatus")]
        public virtual byte? TransStatus { get; set; }

        [JsonProperty("status")]
        public virtual byte? Status { get; set; }

        [JsonProperty("CodeDesc")]
        [StringLength(50)]
        [Grid]
        public virtual string CodeDesc { get; set; }

        [JsonProperty("createdate")]
        public virtual System.DateTime? CreateDate { get; set; }

    }

    public class MapFPXBankList
    {
        [JsonProperty("bankcode")]
        [StringLength(50)]
        [Grid]
        public virtual string BankCode { get; set; }

        [JsonProperty("bankdisplayname")]
        [StringLength(200)]
        [Grid]
        public virtual string BankDisplayName { get; set; }
    }

    public class MapListDashboardInvoice
    {
        [JsonProperty("id")]
        [StringLength(25)]
        public virtual string Id { get; set; }

        [Required, StringLength(50)]
        [JsonProperty("billNo")]
        [Grid(Width = 150, IsSortable = false)]
        public virtual string BillNo { get; set; }

        [JsonProperty("bizregid")]
        [StringLength(50)]
        public virtual string BizRegID { get; set; }

        [JsonProperty("company")]
        [StringLength(4000)]
        [Grid(Width = 200, Permissions = "Pages.Account.Payment.Offline")]
        public virtual string Company { get; set; }

        [JsonProperty("type")]
        [StringLength(25)]
        [Grid(Width = 125, Alignment = ColumnsAlignment.Center)]
        public virtual string Type { get; set; }

        [JsonProperty("date")]
        [Grid(Width = 100, DatetimeFormat = "DD/MM/YYYY")]
        public virtual System.DateTime Date { get; set; }

        [JsonProperty("status")]
        [StringLength(50)]
        [Grid(Width = 145, StatusBadges = new string[] { "Pending Payment|info", "Paid|success", "Acceptance By UKA|primary", "Officer Approval|primary" })]
        public virtual string Status { get; set; }

        [JsonProperty("amount")]
        //[Grid(Width = 100, IsSortable = false, Alignment = ColumnsAlignment.Right)]
        public virtual decimal? Amount { get; set; }

        [JsonProperty("totalamounts")]
        [Grid(Width = 100, IsSortable = false, Alignment = ColumnsAlignment.Right)]
        public virtual decimal? TotalAmounts { get; set; }

        [JsonProperty("detail")]
        [StringLength(4000)]
        public virtual string Detail { get; set; }

        [JsonProperty("siNumber")]
        [StringLength(25)]
        public virtual string SiNumber { get; set; }

        [JsonProperty("gst")]
        public virtual decimal? Gst { get; set; }

        [JsonProperty("balance")]
        public virtual decimal? Balance { get; set; }

        [JsonProperty("rounding")]
        public virtual decimal? Rounding { get; set; }

        [JsonProperty("creditbillcount")]
        public virtual int CreditBillCount { get; set; }

        [JsonProperty("creditbillamount")]
        public virtual decimal? CreditBillAmount { get; set; }

        [JsonProperty("proformacount")]
        public virtual int ProformaCount { get; set; }

        [JsonProperty("proformaamount")]
        public virtual decimal? ProformaAmount { get; set; }

        [JsonProperty("totalamount")]
        public virtual decimal? TotalAmount { get; set; }
    }

    public class MapListCustomer
    {
        [StringLength(20)]
        [JsonProperty("add_code")]
        [Grid]
        public virtual string ADD_CODE { get; set; }

        [StringLength(50)]
        [JsonProperty("address_1")]
        [Grid]
        public virtual string ADDRESS_1 { get; set; }

        [StringLength(50)]
        [JsonProperty("address_3")]
        [Grid]
        public virtual string ADDRESS_3 { get; set; }

        [StringLength(50)]
        [JsonProperty("address_4")]
        [Grid]
        public virtual string ADDRESS_4 { get; set; }

        [StringLength(50)]
        [JsonProperty("address_5")]
        [Grid]
        public virtual string ADDRESS_5 { get; set; }

        [StringLength(50)]
        [JsonProperty("address_6")]
        [Grid]
        public virtual string ADDRESS_6 { get; set; }

        [StringLength(50)]
        [JsonProperty("telephone")]
        [Grid]
        public virtual string TELEPHONE { get; set; }

        [StringLength(50)]
        [JsonProperty("contact")]
        [Grid]
        public virtual string CONTACT { get; set; }

        [StringLength(50)]
        [JsonProperty("e_mail")]
        [Grid]
        public virtual string E_MAIL { get; set; }

        /*
        [StringLength(20)]
        [JsonProperty("bizregid")]
        [Grid]
        public virtual string BizRegID { get; set; }

        [StringLength(50)]
        [JsonProperty("companyname")]
        [Grid]
        public virtual string CompanyName { get; set; }

        [StringLength(20)]
        [JsonProperty("initname")]      
        public virtual string InitName { get; set; }

        [StringLength(20)]
        [JsonProperty("regno")]
        public virtual string RegNo { get; set; }

        [StringLength(50)]
        [JsonProperty("address1")]
        [Grid]
        public virtual string Address1 { get; set; }

        [StringLength(50)]
        [JsonProperty("address2")]
        public virtual string Address2 { get; set; }

        [StringLength(50)]
        [JsonProperty("address3")]
        public virtual string Address3 { get; set; }

        [StringLength(50)]
        [JsonProperty("address4")]
        public virtual string Address4 { get; set; }

        [StringLength(20)]
        [JsonProperty("postalcode")]
        public virtual string PostalCode { get; set; }

        [StringLength(10)]
        [JsonProperty("country")]
        public virtual string Country { get; set; }

        [StringLength(10)]
        [JsonProperty("state")]
        public virtual string State { get; set; }

        [StringLength(10)]
        [JsonProperty("city")]
        public virtual string City { get; set; }

        [StringLength(20)]
        [JsonProperty("area")]
        public virtual string Area { get; set; }

        [StringLength(20)]
        [JsonProperty("telno")]
        public virtual string TelNo { get; set; }
        
        [StringLength(20)]
        [JsonProperty("faxno")]
        public virtual string FaxNo { get; set; }

        [StringLength(50)]
        [JsonProperty("email")]
        public virtual string Email { get; set; }

        [StringLength(50)]
        [JsonProperty("contactpersonemail")]
        public virtual string ContactPersonEmail { get; set; }

        [StringLength(20)]
        [JsonProperty("contactpersontelno")]
        public virtual string ContactPersonTelNo { get; set; }

        [StringLength(20)]
        [JsonProperty("acctno")]
        [Grid]
        public virtual string AcctNo { get; set; }
        */
    }

    #region TRANSCDHDR
    [Abp.AutoMapper.AutoMapTo(typeof(DTO.Accounting.TRANSCDHDR))]
    public class MapTRANSCDHDR : BaseMapId
    {
        [JsonProperty("monthcode")]
        [MaxLength(10), Required]
        public virtual string MonthCode { get; set; }
        [JsonProperty("pbtcode")]
        [MaxLength(20), Required]
        public virtual string PBTCode { get; set; }
        [JsonProperty("batchcode")]
        [MaxLength(20), Required]
        public virtual string BatchCode { get; set; }
        [JsonProperty("transtype")]
        [MaxLength(3)]
        public virtual string TransType { get; set; }
        [JsonProperty("remark")]
        [MaxLength(4000)]
        public virtual string Remark { get; set; }
        [JsonProperty("totalrec1")]
        [Range(0, 9999)]
        public virtual int? TotalRec1 { get; set; }
        [JsonProperty("totalrec2")]
        [Range(0, 9999)]
        public virtual int? TotalRec2 { get; set; }
        [JsonProperty("totalrec3")]
        [Range(0, 9999)]
        public virtual int? TotalRec3 { get; set; }
        [JsonProperty("totalrec4")]
        [Range(0, 9999)]
        public virtual int? TotalRec4 { get; set; }
        [JsonProperty("totalrec5")]
        [Range(0, 9999)]
        public virtual int? TotalRec5 { get; set; }
        [JsonProperty("totalamt1")]
        public virtual decimal? TotalAmt1 { get; set; }
        [JsonProperty("totalamt2")]
        public virtual decimal? TotalAmt2 { get; set; }
        [JsonProperty("totalamt3")]
        public virtual decimal? TotalAmt3 { get; set; }
        [JsonProperty("totalamt4")]
        public virtual decimal? TotalAmt4 { get; set; }
        [JsonProperty("totalamt5")]
        public virtual decimal? TotalAmt5 { get; set; }
        [JsonProperty("status")]
        public virtual byte? Status { get; set; }
    }
    #endregion

    #region TRANSCDITEM
    [Abp.AutoMapper.AutoMapTo(typeof(DTO.Accounting.TRANSCDITEM))]
    public class MapTRANSCDITEM : BaseMapId
    {
        [JsonProperty("monthcode")]
        [MaxLength(10), Required]
        public virtual string MonthCode { get; set; }
        [JsonProperty("pbtcode")]
        [MaxLength(20), Required]
        public virtual string PBTCode { get; set; }
        [JsonProperty("bizregid")]
        [MaxLength(20), Required]
        public virtual string BizRegID { get; set; }
        [JsonProperty("batchcode")]
        [MaxLength(20), Required]
        public virtual string BatchCode { get; set; }
        [JsonProperty("seqno")]
        [Range(0, 9999), Required]
        public virtual int? SeqNo { get; set; }
        [JsonProperty("companyname")]
        [MaxLength(200)]
        public virtual string CompanyName { get; set; }
        [JsonProperty("contractid")]
        [MaxLength(200)]
        public virtual string ContractID { get; set; }
        [JsonProperty("cndndate")]
        public virtual System.DateTime? CNDNDate { get; set; }
        [JsonProperty("itemtype")]
        [MaxLength(3)]
        public virtual string ItemType { get; set; }
        [JsonProperty("itemcode")]
        [MaxLength(20)]
        public virtual string ItemCode { get; set; }
        [JsonProperty("itemdesc")]
        [MaxLength(4000)]
        public virtual string ItemDesc { get; set; }
        [JsonProperty("amount1")]
        public virtual decimal? Amount1 { get; set; }
        [JsonProperty("amount2")]
        public virtual decimal? Amount2 { get; set; }
        [JsonProperty("remark1")]
        [MaxLength(4000)]
        public virtual string Remark1 { get; set; }
        [JsonProperty("remark2")]
        [MaxLength(4000)]
        public virtual string Remark2 { get; set; }
        [JsonProperty("svctype")]
        [MaxLength(10)]
        public virtual string SvcType { get; set; }
        [JsonProperty("proctype")]
        [MaxLength(10)]
        public virtual string ProcType { get; set; }
        [JsonProperty("errcode")]
        [MaxLength(200)]
        public virtual string ErrCode { get; set; }
        [JsonProperty("status")]
        public virtual byte? Status { get; set; }
        [JsonProperty("currency")]
        [MaxLength(3)]
        public virtual string Currency { get; set; }
        [JsonProperty("filepath")]
        [MaxLength(4000)]
        public virtual string FilePath { get; set; }
    }
    #endregion
    public class MapListPdfInvoice
    {
        [JsonProperty("Status")]
        [StringLength(20)]
        [Grid(Width = 100, Alignment = ColumnsAlignment.Center, StatusBadges = new string[] { "PAID|paid", "NEAR OVERDUE|near-overdue", "UNPAID|unpaid", "OVERDUE|overdue", "OPEN|open" })]
        public virtual string Status { get; set; }

        //[JsonProperty("DocType")]
        //[StringLength(20)]
        //[Grid(Width = 100, Alignment = ColumnsAlignment.Center, StatusBadges = new string[] { "Invoice|info", "Manual Payment|success", "Debit Note|warning", "Credit Note|danger" })]
        //public virtual string DocType { get; set; }

        [JsonProperty("DocType")]
        [StringLength(20)]
        [Grid(IsVisible = false)]
        public virtual string DocType { get; set; }

        [StringLength(50)]
        [JsonProperty("BizRegID")]
        [Grid(IsVisible = false)]
        public virtual string BizRegID { get; set; }

        [JsonProperty("BizLocID")]
        [StringLength(50)]
        [Grid(IsVisible = false)]
        public virtual string BizLocID { get; set; }

        [StringLength(20)]
        [JsonProperty("SWTTANo")]
        [Grid(Width = 120)]
        public virtual string SWTTANo { get; set; }

        [JsonProperty("BusinessName")]
        [StringLength(4000)]
        [Grid(Width = 250)]
        public virtual string BusinessName { get; set; }

        [JsonProperty("ClinicName")]
        [StringLength(50)]
        [Grid(Width = 250)]
        public virtual string ClinicName { get; set; }

        [JsonProperty("TransNo")]
        [StringLength(20)]
        [Grid(Width = 125)]
        public virtual string TransNo { get; set; }

        [JsonProperty("InvoiceNo")]
        [StringLength(20)]
        [Grid(Width = 125)]
        public virtual string InvoiceNo { get; set; }

        [JsonProperty("DocumentNo")]
        [StringLength(50)]
        [Grid(IsVisible = false)]
        public virtual string DocumentNo { get; set; }
        
        [JsonProperty("InvoiceDate")]
        [Grid(Width = 130, Alignment = ColumnsAlignment.Center, DatetimeFormat = "DD/MM/YYYY")]
        public virtual System.DateTime InvoiceDate { get; set; }

        [JsonProperty("ItemType")]
        [StringLength(5)]
        [Grid(IsVisible = false)]
        public virtual string ItemType { get; set; }

        [JsonProperty("Amount")]
        [Grid(Width = 140, HasCurrency = true)]
        public virtual decimal? Amount { get; set; }

        [JsonProperty("InvoiceAmt")]
        [Grid(IsVisible = false)]
        public virtual decimal? InvoiceAmt { get; set; }

        [JsonProperty("DebitNoteAmt")]
        [Grid(IsVisible = false)]
        public virtual decimal? DebitNoteAmt { get; set; }

        [JsonProperty("CreditNoteAmt")]
        [Grid(IsVisible = false)]
        public virtual decimal? CreditNoteAmt { get; set; }

        [JsonProperty("ManualPaymentAmt")]
        [Grid(IsVisible = false)]
        public virtual decimal? ManualPaymentAmt { get; set; }

        [JsonProperty("PathRef")]
        [StringLength(200)]
        [Grid(IsVisible = false)]
        public virtual string PathRef { get; set; }

        [JsonProperty("PhysicalPath")]
        [StringLength(200)]
        [Grid(IsVisible = false)]
        public virtual string PhysicalPath { get; set; }

        [JsonProperty("ServerPath")]
        [StringLength(200)]
        [Grid(IsVisible = false)]
        public virtual string ServerPath { get; set; }
    }

    public class MapListInvoiceDetails
    {
        [JsonProperty("ItemType")]
        [StringLength(10)]
        [Grid(IsVisible = false)]
        public virtual string ItemType { get; set; }

        //[JsonProperty("DocType")]
        //[StringLength(20)]
        //[Grid(Width = 100, Alignment = ColumnsAlignment.Center, StatusBadges = new string[] { "Invoice|info", "Manual Payment|success", "Debit Note|warning", "Credit Note|danger" })]
        //public virtual string DocType { get; set; }

        [JsonProperty("DocumentType")]
        [StringLength(20)]
        [Grid(Width = 140, Alignment = ColumnsAlignment.Left, IsSortable = false)]
        public virtual string DocumentType { get; set; }

        [JsonProperty("DocumentDate")]
        [Grid(Width = 140, Alignment = ColumnsAlignment.Center, IsSortable = false, DatetimeFormat = "DD/MM/YYYY")]
        public virtual System.DateTime? DocumentDate { get; set; }

        [JsonProperty("DocumentNo")]
        [StringLength(20)]
        [Grid(Width = 140, Alignment = ColumnsAlignment.Left, IsSortable = false)]
        public virtual string DocumentNo { get; set; }

        //[JsonProperty("DebitValue")]
        //[Grid(Width = 120, IsSortable = false)]
        //public virtual decimal? DebitValue { get; set; }

        //[JsonProperty("CreditValue")]
        //[Grid(Width = 120, IsSortable = false)]
        //public virtual decimal? CreditValue { get; set; }

        [JsonProperty("Amount")]
        [Grid(Width = 130, IsSortable = false, HasCurrency = true)]
        public virtual decimal? Amount { get; set; }

        [JsonProperty("Currency")]
        [StringLength(10)]
        [Grid(IsVisible = false)]
        public virtual string Currency { get; set; }

        [JsonProperty("FilePath")]
        [StringLength(200)]
        [Grid(IsVisible = false)]
        public virtual string FilePath { get; set; }

        [JsonProperty("PathRef")]
        [StringLength(200)]
        [Grid(IsVisible = false)]
        public virtual string PathRef { get; set; }
    }
    public class MapValidateStatus
    {
        public string DebitAuthCode { get; set; }
        public string Message { get; set; }
        public string CheckSum { get; set; }
    }

    #region DATABASE MAPPING
    //#region INVOICEHDR
    //[Abp.AutoMapper.AutoMapTo(typeof(DTO.Accounting.INVOICEHDR))]
    //public class MapINVOICEHDR
    //{
    //    [Required, StringLength(10)]
    //    [JsonProperty("monthcode")]
    //    public virtual string MonthCode { get; set; }
    //    [Required, StringLength(20)]
    //    [JsonProperty("pbtcode")]
    //    public virtual string PBTCode { get; set; }
    //    [Required, StringLength(20)]
    //    [JsonProperty("batchcode")]
    //    public virtual string BatchCode { get; set; }
    //    [JsonProperty("totalrec1")]
    //    [Range(0, 4)]
    //    public virtual int? TotalRec1 { get; set; }
    //    [JsonProperty("totalrec2")]
    //    [Range(0, 4)]
    //    public virtual int? TotalRec2 { get; set; }
    //    [JsonProperty("totalrec3")]
    //    [Range(0, 4)]
    //    public virtual int? TotalRec3 { get; set; }
    //    [JsonProperty("totalrec4")]
    //    [Range(0, 4)]
    //    public virtual int? TotalRec4 { get; set; }
    //    [JsonProperty("totalrec5")]
    //    [Range(0, 4)]
    //    public virtual int? TotalRec5 { get; set; }
    //    [JsonProperty("totalamt1")]
    //    [MaxLength(9)]
    //    public virtual decimal? TotalAmt1 { get; set; }
    //    [JsonProperty("totalamt2")]
    //    [MaxLength(9)]
    //    public virtual decimal? TotalAmt2 { get; set; }
    //    [JsonProperty("totalamt3")]
    //    [MaxLength(9)]
    //    public virtual decimal? TotalAmt3 { get; set; }
    //    [JsonProperty("totalamt4")]
    //    [MaxLength(9)]
    //    public virtual decimal? TotalAmt4 { get; set; }
    //    [JsonProperty("totalamt5")]
    //    [MaxLength(9)]
    //    public virtual decimal? TotalAmt5 { get; set; }
    //    [JsonProperty("createdate")]
    //    public virtual System.DateTime? CreateDate { get; set; }
    //    [JsonProperty("createby")]
    //    [StringLength(20)]
    //    public virtual string CreateBy { get; set; }
    //    [JsonProperty("lastupdate")]
    //    public virtual System.DateTime? LastUpdate { get; set; }
    //    [JsonProperty("updateby")]
    //    [StringLength(20)]
    //    public virtual string UpdateBy { get; set; }
    //    [JsonProperty("synccreate")]
    //    public virtual System.DateTime? SyncCreate { get; set; }
    //    [JsonProperty("synclastupd")]
    //    public virtual System.DateTime? SyncLastUpd { get; set; }
    //    [JsonProperty("ishost")]
    //    public virtual byte? IsHost { get; set; }
    //    [JsonProperty("status")]
    //    public virtual byte? Status { get; set; }
    //    [JsonProperty("active")]
    //    public virtual byte? Active { get; set; }
    //    [JsonProperty("inuse")]
    //    public virtual byte? Inuse { get; set; }
    //    [JsonProperty("flag")]
    //    public virtual byte? Flag { get; set; }
    //    [JsonProperty("lastsyncby")]
    //    [StringLength(20)]
    //    public virtual string LastSyncBy { get; set; }
    //    [JsonProperty("tid")]
    //    [Range(0, 4)]
    //    public virtual int? TID { get; set; }
    //}
    //#endregion

    #region INVOICEHDR
    [Abp.AutoMapper.AutoMapTo(typeof(DTO.Accounting.INVOICEHDR))]
    public class MapINVOICEHDR
    {
        [Required, StringLength(20)]
        [JsonProperty("bizregid")]
        public virtual string BizRegID { get; set; }
        [Required, StringLength(20)]
        [JsonProperty("bizlocid")]
        public virtual string BizLocID { get; set; }
        [Required, StringLength(20)]
        [JsonProperty("transno")]
        public virtual string TransNo { get; set; }
        [JsonProperty("termid")]
        public virtual int? TermID { get; set; }
        [JsonProperty("transtype")]
        public virtual byte? TransType { get; set; }
        [JsonProperty("billno")]
        [StringLength(20)]
        public virtual string BillNo { get; set; }
        [JsonProperty("custpkgid")]
        [StringLength(50)]
        public virtual string CustPkgID { get; set; }
        [JsonProperty("cashierid")]
        [StringLength(255)]
        public virtual string CashierID { get; set; }
        [JsonProperty("shiftcode")]
        [StringLength(10)]
        public virtual string ShiftCode { get; set; }
        [JsonProperty("totalserver")]
        public virtual int? TotalServer { get; set; }
        [JsonProperty("serverid")]
        [StringLength(50)]
        public virtual string ServerID { get; set; }
        [JsonProperty("emptype")]
        public virtual byte? EmpType { get; set; }
        [JsonProperty("tillid")]
        public virtual int? TillID { get; set; }
        [JsonProperty("transdate")]
        public virtual System.DateTime? TransDate { get; set; }
        [JsonProperty("transstarttime")]
        [StringLength(6)]
        public virtual string TransStartTime { get; set; }
        [JsonProperty("transendtime")]
        [StringLength(6)]
        public virtual string TransEndTime { get; set; }
        [JsonProperty("transpaytime")]
        [StringLength(6)]
        public virtual string TransPayTime { get; set; }
        [JsonProperty("transamt")]
        public virtual decimal? TransAmt { get; set; }
        [JsonProperty("transsubtotal")]
        public virtual decimal? TransSubTotal { get; set; }
        [JsonProperty("transamtrnd")]
        public virtual decimal? TransAmtRnd { get; set; }
        [JsonProperty("transamtsave")]
        public virtual decimal? TransAmtSave { get; set; }
        [JsonProperty("transamtorg")]
        public virtual decimal? TransAmtOrg { get; set; }
        [JsonProperty("transchgamt")]
        public virtual decimal? TransChgAmt { get; set; }
        [JsonProperty("transdisccode")]
        [StringLength(10)]
        public virtual string TransDiscCode { get; set; }
        [JsonProperty("transdisctype")]
        public virtual byte? TransDiscType { get; set; }
        [JsonProperty("transdiscvalue")]
        public virtual decimal? TransDiscValue { get; set; }
        [JsonProperty("transdiscamt")]
        public virtual decimal? TransDiscAmt { get; set; }
        [JsonProperty("transdiscauth")]
        [StringLength(20)]
        public virtual string TransDiscAuth { get; set; }
        [JsonProperty("transdiscreasoncode")]
        [StringLength(50)]
        public virtual string TransDiscReasonCode { get; set; }
        [JsonProperty("transdiscremark")]
        [StringLength(50)]
        public virtual string TransDiscRemark { get; set; }
        [JsonProperty("transamtspdisc")]
        public virtual decimal? TransAmtSpDisc { get; set; }
        [JsonProperty("transvaluespdisc")]
        public virtual decimal? TransValueSpDisc { get; set; }
        [JsonProperty("authspdisc")]
        [StringLength(20)]
        public virtual string AuthSpDisc { get; set; }
        [JsonProperty("spdiscreasoncode")]
        [StringLength(10)]
        public virtual string SpDiscReasonCode { get; set; }
        [JsonProperty("spdiscremark")]
        [StringLength(200)]
        public virtual string SpDiscRemark { get; set; }
        [JsonProperty("customerid")]
        [StringLength(20)]
        public virtual string CustomerID { get; set; }
        [JsonProperty("custtype")]
        public virtual byte? CustType { get; set; }
        [JsonProperty("custprivilege")]
        [StringLength(255)]
        public virtual string CustPrivilege { get; set; }
        [JsonProperty("acctno")]
        [StringLength(50)]
        public virtual string AcctNo { get; set; }
        [JsonProperty("totalpoints")]
        public virtual decimal? TotalPoints { get; set; }
        [JsonProperty("insvcid")]
        [StringLength(20)]
        public virtual string InSvcID { get; set; }
        [JsonProperty("transreasoncode")]
        [StringLength(50)]
        public virtual string TransReasonCode { get; set; }
        [JsonProperty("transremark")]
        [StringLength(255)]
        public virtual string TransRemark { get; set; }
        [JsonProperty("transstatus")]
        [StringLength(20)]
        public virtual string TransStatus { get; set; }
        [JsonProperty("posted")]
        public virtual byte? Posted { get; set; }
        [JsonProperty("postdate")]
        public virtual System.DateTime? PostDate { get; set; }
        [JsonProperty("training")]
        public virtual byte? Training { get; set; }
        [JsonProperty("profiled")]
        public virtual byte? Profiled { get; set; }
        [JsonProperty("livecal")]
        public virtual byte? LiveCal { get; set; }
        [JsonProperty("createdate")]
        public virtual System.DateTime? CreateDate { get; set; }
        [JsonProperty("createby")]
        [StringLength(20)]
        public virtual string CreateBy { get; set; }
        [JsonProperty("lastupdate")]
        public virtual System.DateTime? LastUpdate { get; set; }
        [JsonProperty("updateby")]
        [StringLength(20)]
        public virtual string UpdateBy { get; set; }
        [JsonProperty("status")]
        public virtual byte? Status { get; set; }
        [JsonProperty("transvoid")]
        public virtual byte? TransVoid { get; set; }
        [JsonProperty("flag")]
        public virtual byte? Flag { get; set; }
        [JsonProperty("synccreate")]
        public virtual System.DateTime? SyncCreate { get; set; }
        [JsonProperty("synclastupd")]
        public virtual System.DateTime? SyncLastUpd { get; set; }
        [JsonProperty("tblno")]
        [StringLength(10)]
        public virtual string TblNo { get; set; }
        [JsonProperty("tblpax")]
        public virtual int? TblPax { get; set; }
        [JsonProperty("transpointsstatus")]
        public virtual byte? TransPointsStatus { get; set; }
        [JsonProperty("ishost")]
        public virtual byte? IsHost { get; set; }
        [JsonProperty("lastsyncby")]
        public virtual byte? LastSyncBy { get; set; }
        [JsonProperty("transpoints")]
        public virtual decimal? TransPoints { get; set; }
        [JsonProperty("synccreateby")]
        [StringLength(10)]
        public virtual string SyncCreateBy { get; set; }
        [JsonProperty("active")]
        public virtual byte? Active { get; set; }
        [JsonProperty("inuse")]
        public virtual byte? Inuse { get; set; }
        [JsonProperty("tid")]
        public virtual int? TID { get; set; }
    }
    #endregion

    //#region INVOICEITEM
    //[Abp.AutoMapper.AutoMapTo(typeof(DTO.Accounting.INVOICEITEM))]
    //public class MapINVOICEITEM
    //{
    //    [Required, StringLength(10)]
    //    [JsonProperty("monthcode")]
    //    public virtual string MonthCode { get; set; }
    //    [Required, StringLength(20)]
    //    [JsonProperty("pbtcode")]
    //    public virtual string PBTCode { get; set; }
    //    [Required, StringLength(20)]
    //    [JsonProperty("bizregid")]
    //    public virtual string BizRegID { get; set; }
    //    [Required, StringLength(20)]
    //    [JsonProperty("batchcode")]
    //    public virtual string BatchCode { get; set; }
    //    [Required, Range(0, 4)]
    //    [JsonProperty("seqno")]
    //    public virtual int? SeqNo { get; set; }
    //    [JsonProperty("companyname")]
    //    [StringLength(200)]
    //    public virtual string CompanyName { get; set; }
    //    [JsonProperty("contractid")]
    //    [StringLength(200)]
    //    public virtual string ContractID { get; set; }
    //    [JsonProperty("invdate")]
    //    public virtual System.DateTime? InvDate { get; set; }
    //    [JsonProperty("itemtype")]
    //    [StringLength(3)]
    //    public virtual string ItemType { get; set; }
    //    [JsonProperty("itemcode")]
    //    [StringLength(20)]
    //    public virtual string ItemCode { get; set; }
    //    [JsonProperty("itemdesc")]
    //    [StringLength(4000)]
    //    public virtual string ItemDesc { get; set; }
    //    [JsonProperty("amount1")]
    //    [MaxLength(9)]
    //    public virtual decimal? Amount1 { get; set; }
    //    [JsonProperty("amount2")]
    //    [MaxLength(9)]
    //    public virtual decimal? Amount2 { get; set; }
    //    [JsonProperty("remark1")]
    //    [StringLength(4000)]
    //    public virtual string Remark1 { get; set; }
    //    [JsonProperty("remark2")]
    //    [StringLength(4000)]
    //    public virtual string Remark2 { get; set; }
    //    [JsonProperty("svctype")]
    //    [StringLength(10)]
    //    public virtual string SvcType { get; set; }
    //    [JsonProperty("proctype")]
    //    [StringLength(10)]
    //    public virtual string ProcType { get; set; }
    //    [JsonProperty("errcode")]
    //    [StringLength(200)]
    //    public virtual string ErrCode { get; set; }
    //    [JsonProperty("currency")]
    //    [StringLength(3)]
    //    public virtual string Currency { get; set; }
    //    [JsonProperty("filepath")]
    //    [StringLength(4000)]
    //    public virtual string FilePath { get; set; }
    //    [JsonProperty("createdate")]
    //    public virtual System.DateTime? CreateDate { get; set; }
    //    [JsonProperty("createby")]
    //    [StringLength(20)]
    //    public virtual string CreateBy { get; set; }
    //    [JsonProperty("lastupdate")]
    //    public virtual System.DateTime? LastUpdate { get; set; }
    //    [JsonProperty("updateby")]
    //    [StringLength(20)]
    //    public virtual string UpdateBy { get; set; }
    //    [JsonProperty("ishost")]
    //    public virtual byte? IsHost { get; set; }
    //    [JsonProperty("status")]
    //    public virtual byte? Status { get; set; }
    //    [JsonProperty("active")]
    //    public virtual byte? Active { get; set; }
    //    [JsonProperty("inuse")]
    //    public virtual byte? Inuse { get; set; }
    //    [JsonProperty("flag")]
    //    public virtual byte? Flag { get; set; }
    //    [JsonProperty("synccreate")]
    //    public virtual System.DateTime? SyncCreate { get; set; }
    //    [JsonProperty("synclastupd")]
    //    public virtual System.DateTime? SyncLastUpd { get; set; }
    //    [JsonProperty("lastsyncby")]
    //    [StringLength(20)]
    //    public virtual string LastSyncBy { get; set; }
    //    [JsonProperty("tid")]
    //    [Range(0, 4)]
    //    public virtual int? TID { get; set; }
    //}
    //#endregion

    #region INVOICEDTL
    [Abp.AutoMapper.AutoMapTo(typeof(DTO.Accounting.INVOICEDTL))]
    public class MapINVOICEDTL
    {
        [Required, StringLength(20)]
        [JsonProperty("bizlocid")]
        public virtual string BizLocID { get; set; }
        [Required, StringLength(20)]
        [JsonProperty("bizregid")]
        public virtual string BizRegID { get; set; }
        [Required, StringLength(20)]
        [JsonProperty("transno")]
        public virtual string TransNo { get; set; }
        [Required, Range(0, 4)]
        [JsonProperty("transseq")]
        public virtual int? TransSeq { get; set; }
        [Required, StringLength(20)]
        [JsonProperty("itemcode")]
        public virtual string ItemCode { get; set; }
        [JsonProperty("termid")]
        public virtual int? TermID { get; set; }
        [JsonProperty("refseq")]
        public virtual int? RefSeq { get; set; }
        [JsonProperty("iscal")]
        public virtual byte? IsCal { get; set; }
        [JsonProperty("billno")]
        [StringLength(20)]
        public virtual string BillNo { get; set; }
        [JsonProperty("entrytime")]
        [StringLength(6)]
        public virtual string EntryTime { get; set; }
        [JsonProperty("stkcode")]
        [StringLength(20)]
        public virtual string StkCode { get; set; }
        [JsonProperty("stkdesc")]
        [StringLength(50)]
        public virtual string StkDesc { get; set; }
        [JsonProperty("stktype")]
        [StringLength(3)]
        public virtual string StkType { get; set; }
        [JsonProperty("behvtype")]
        public virtual byte? BehvType { get; set; }
        [JsonProperty("itemtype")]
        [StringLength(20)]
        public virtual string ItemType { get; set; }
        [JsonProperty("qty")]
        public virtual int? Qty { get; set; }
        [JsonProperty("unitcost")]
        public virtual decimal? UnitCost { get; set; }
        [JsonProperty("orgprice")]
        public virtual decimal? OrgPrice { get; set; }
        [JsonProperty("nettprice")]
        public virtual decimal? NettPrice { get; set; }
        [JsonProperty("tolamt")]
        public virtual decimal? TolAmt { get; set; }
        [JsonProperty("discamt")]
        public virtual decimal? DiscAmt { get; set; }
        [JsonProperty("nettamt")]
        public virtual decimal? NettAmt { get; set; }
        [JsonProperty("subpoints")]
        public virtual decimal? SubPoints { get; set; }
        [JsonProperty("points")]
        public virtual decimal? Points { get; set; }
        [JsonProperty("priceoverrided")]
        public virtual byte? PriceOverrided { get; set; }
        [JsonProperty("discounted")]
        public virtual byte? Discounted { get; set; }
        [JsonProperty("taxable")]
        public virtual byte? Taxable { get; set; }
        [JsonProperty("returncode")]
        [StringLength(10)]
        public virtual string ReturnCode { get; set; }
        [JsonProperty("remark")]
        [StringLength(4000)]
        public virtual string Remark { get; set; }
        [JsonProperty("serialno")]
        [StringLength(50)]
        public virtual string SerialNo { get; set; }
        [JsonProperty("priceauthid")]
        [StringLength(20)]
        public virtual string PriceAuthID { get; set; }
        [JsonProperty("transvoid")]
        public virtual byte? TransVoid { get; set; }
        [JsonProperty("posted")]
        public virtual byte? Posted { get; set; }
        [JsonProperty("livecal")]
        public virtual byte? LiveCal { get; set; }
        [JsonProperty("flag")]
        public virtual byte? Flag { get; set; }
        [JsonProperty("custpkgid")]
        [StringLength(20)]
        public virtual string CustPkgID { get; set; }
        [JsonProperty("prcdisplaytype")]
        public virtual byte? PrcDisplayType { get; set; }
        [JsonProperty("promocode")]
        [StringLength(20)]
        public virtual string PromoCode { get; set; }
        [JsonProperty("createdate")]
        public virtual System.DateTime? CreateDate { get; set; }
        [JsonProperty("createby")]
        [StringLength(20)]
        public virtual string CreateBy { get; set; }
        [JsonProperty("lastupdate")]
        public virtual System.DateTime? LastUpdate { get; set; }
        [JsonProperty("updateby")]
        [StringLength(20)]
        public virtual string UpdateBy { get; set; }
        [JsonProperty("status")]
        public virtual byte? Status { get; set; }
        [JsonProperty("synccreate")]
        public virtual System.DateTime? SyncCreate { get; set; }
        [JsonProperty("synclastupd")]
        public virtual System.DateTime? SyncLastUpd { get; set; }
        [JsonProperty("ishost")]
        public virtual byte? IsHost { get; set; }
        [JsonProperty("lastsyncby")]
        [StringLength(20)]
        public virtual string LastSyncBy { get; set; }
        [JsonProperty("synccreateby")]
        [StringLength(20)]
        public virtual string SyncCreateBy { get; set; }
        [JsonProperty("excode1")]
        [StringLength(10)]
        public virtual string ExCode1 { get; set; }
        [JsonProperty("excode2")]
        [StringLength(10)]
        public virtual string ExCode2 { get; set; }
        [JsonProperty("baserate")]
        public virtual decimal? BaseRate { get; set; }
        [JsonProperty("corate1h")]
        public virtual decimal? CoRate1H { get; set; }
        [JsonProperty("corate1g")]
        public virtual decimal? CoRate1G { get; set; }
        [JsonProperty("corate2h")]
        public virtual decimal? CoRate2H { get; set; }
        [JsonProperty("corate2g")]
        public virtual decimal? CoRate2G { get; set; }
        [JsonProperty("corate3h")]
        public virtual decimal? CoRate3H { get; set; }
        [JsonProperty("corate3g")]
        public virtual decimal? CoRate3G { get; set; }
        [JsonProperty("corate4h")]
        public virtual decimal? CoRate4H { get; set; }
        [JsonProperty("corate4g")]
        public virtual decimal? CoRate4G { get; set; }
        [JsonProperty("corate5h")]
        public virtual decimal? CoRate5H { get; set; }
        [JsonProperty("corate5g")]
        public virtual decimal? CoRate5G { get; set; }
        [JsonProperty("warserialno")]
        [StringLength(20)]
        public virtual string WarSerialNo { get; set; }
        [JsonProperty("active")]
        public virtual byte? Active { get; set; }
        [JsonProperty("inuse")]
        public virtual byte? Inuse { get; set; }
        [JsonProperty("tid")]
        public virtual int? TID { get; set; }
        [JsonProperty("pathref")]
        [MaxLength(255)]
        public virtual string PathRef { get; set; }

    }
    #endregion

    #region INVOICETAX
    [Abp.AutoMapper.AutoMapTo(typeof(DTO.Accounting.INVOICETAX))]
    public class MapINVOICETAX
    {
        [Required, StringLength(20)]
        [JsonProperty("bizregid")]
        public virtual string BizRegID { get; set; }
        [Required, StringLength(20)]
        [JsonProperty("bizlocid")]
        public virtual string BizLocID { get; set; }
        [Required, StringLength(20)]
        [JsonProperty("transno")]
        public virtual string TransNo { get; set; }
        [Required, Range(0, 4)]
        [JsonProperty("taxcode")]
        public virtual int? TaxCode { get; set; }
        [JsonProperty("termid")]
        public virtual int? TermID { get; set; }
        [JsonProperty("taxamt")]
        public virtual decimal? TaxAmt { get; set; }
        [JsonProperty("taxrate")]
        public virtual decimal? TaxRate { get; set; }
        [JsonProperty("taxinex")]
        public virtual byte? TaxInEx { get; set; }
        [JsonProperty("taxcharge")]
        public virtual decimal? TaxCharge { get; set; }
        [JsonProperty("taxremark")]
        [StringLength(255)]
        public virtual string TaxRemark { get; set; }
        [JsonProperty("transvoid")]
        public virtual byte? TransVoid { get; set; }
        [JsonProperty("posted")]
        public virtual byte? Posted { get; set; }
        [JsonProperty("createdate")]
        public virtual System.DateTime? CreateDate { get; set; }
        [JsonProperty("createby")]
        [StringLength(20)]
        public virtual string CreateBy { get; set; }
        [JsonProperty("synccreate")]
        public virtual System.DateTime? SyncCreate { get; set; }
        [JsonProperty("synclastupd")]
        public virtual System.DateTime? SyncLastUpd { get; set; }
        [JsonProperty("ishost")]
        public virtual byte? IsHost { get; set; }
        [JsonProperty("lastsyncby")]
        [StringLength(20)]
        public virtual string LastSyncBy { get; set; }
        [JsonProperty("points")]
        public virtual int? Points { get; set; }
        [JsonProperty("synccreateby")]
        [StringLength(20)]
        public virtual string SyncCreateBy { get; set; }
        [JsonProperty("active")]
        public virtual byte? Active { get; set; }
        [JsonProperty("inuse")]
        public virtual byte? Inuse { get; set; }
        [JsonProperty("tid")]
        public virtual int? TID { get; set; }
    }
    #endregion

    #region TRANSHDR
    [Abp.AutoMapper.AutoMapTo(typeof(DTO.Accounting.TRANSHDR))]
    public class MapTRANSHDR
    {
        [Required, StringLength(20)]
        [JsonProperty("bizregid")]
        public virtual string BizRegID { get; set; }
        [Required, StringLength(20)]
        [JsonProperty("bizlocid")]
        public virtual string BizLocID { get; set; }
        [Required, Range(0, 2)]
        [JsonProperty("termid")]
        public virtual int? TermID { get; set; }
        [Required, StringLength(20)]
        [JsonProperty("transno")]
        public virtual string TransNo { get; set; }
        [JsonProperty("transtype")]
        public virtual byte? TransType { get; set; }
        [JsonProperty("billno")]
        [StringLength(20)]
        public virtual string BillNo { get; set; }
        [JsonProperty("custpkgid")]
        [StringLength(50)]
        public virtual string CustPkgID { get; set; }
        [JsonProperty("cashierid")]
        [StringLength(50)]
        public virtual string CashierID { get; set; }
        [JsonProperty("shiftcode")]
        [StringLength(255)]
        public virtual string ShiftCode { get; set; }
        [JsonProperty("totalserver")]
        public virtual int? TotalServer { get; set; }
        [JsonProperty("serverid")]
        [StringLength(50)]
        public virtual string ServerID { get; set; }
        [JsonProperty("emptype")]
        public virtual byte? EmpType { get; set; }
        [JsonProperty("tillid")]
        public virtual int? TillID { get; set; }
        [JsonProperty("transdate")]
        public virtual System.DateTime? TransDate { get; set; }
        [JsonProperty("transstarttime")]
        [StringLength(6)]
        public virtual string TransStartTime { get; set; }
        [JsonProperty("transendtime")]
        [StringLength(6)]
        public virtual string TransEndTime { get; set; }
        [JsonProperty("transpaytime")]
        [StringLength(6)]
        public virtual string TransPayTime { get; set; }
        [JsonProperty("transamt")]
        public virtual decimal? TransAmt { get; set; }
        [JsonProperty("transsubtotal")]
        public virtual decimal? TransSubTotal { get; set; }
        [JsonProperty("transamtrnd")]
        public virtual decimal? TransAmtRnd { get; set; }
        [JsonProperty("transamtsave")]
        public virtual decimal? TransAmtSave { get; set; }
        [JsonProperty("transamtorg")]
        public virtual decimal? TransAmtOrg { get; set; }
        [JsonProperty("transchgamt")]
        public virtual decimal? TransChgAmt { get; set; }
        [JsonProperty("transdisccode")]
        [StringLength(10)]
        public virtual string TransDiscCode { get; set; }
        [JsonProperty("transdisctype")]
        public virtual byte? TransDiscType { get; set; }
        [JsonProperty("transdiscvalue")]
        public virtual decimal? TransDiscValue { get; set; }
        [JsonProperty("transdiscamt")]
        public virtual decimal? TransDiscAmt { get; set; }
        [JsonProperty("transdiscauth")]
        [StringLength(20)]
        public virtual string TransDiscAuth { get; set; }
        [JsonProperty("transdiscreasoncode")]
        [StringLength(50)]
        public virtual string TransDiscReasonCode { get; set; }
        [JsonProperty("transdiscremark")]
        [StringLength(50)]
        public virtual string TransDiscRemark { get; set; }
        [JsonProperty("transamtspdisc")]
        public virtual decimal? TransAmtSpDisc { get; set; }
        [JsonProperty("transvaluespdisc")]
        public virtual decimal? TransValueSpDisc { get; set; }
        [JsonProperty("authspdisc")]
        [StringLength(20)]
        public virtual string AuthSpDisc { get; set; }
        [JsonProperty("spdiscreasoncode")]
        [StringLength(10)]
        public virtual string SpDiscReasonCode { get; set; }
        [JsonProperty("spdiscremark")]
        [StringLength(200)]
        public virtual string SpDiscRemark { get; set; }
        [JsonProperty("customerid")]
        [StringLength(20)]
        public virtual string CustomerID { get; set; }
        [JsonProperty("custtype")]
        public virtual byte? CustType { get; set; }
        [JsonProperty("custprivilege")]
        [StringLength(255)]
        public virtual string CustPrivilege { get; set; }
        [JsonProperty("acctno")]
        [StringLength(50)]
        public virtual string AcctNo { get; set; }
        [JsonProperty("totalpoints")]
        public virtual decimal? TotalPoints { get; set; }
        [JsonProperty("insvcid")]
        [StringLength(20)]
        public virtual string InSvcID { get; set; }
        [JsonProperty("transreasoncode")]
        [StringLength(50)]
        public virtual string TransReasonCode { get; set; }
        [JsonProperty("transremark")]
        [StringLength(255)]
        public virtual string TransRemark { get; set; }
        [JsonProperty("transstatus")]
        [StringLength(20)]
        public virtual string TransStatus { get; set; }
        [JsonProperty("posted")]
        public virtual byte? Posted { get; set; }
        [JsonProperty("postdate")]
        public virtual System.DateTime? PostDate { get; set; }
        [JsonProperty("training")]
        public virtual byte? Training { get; set; }
        [JsonProperty("profiled")]
        public virtual byte? Profiled { get; set; }
        [JsonProperty("livecal")]
        public virtual byte? LiveCal { get; set; }
        [JsonProperty("createdate")]
        public virtual System.DateTime? CreateDate { get; set; }
        [JsonProperty("createby")]
        [StringLength(20)]
        public virtual string CreateBy { get; set; }
        [JsonProperty("lastupdate")]
        public virtual System.DateTime? LastUpdate { get; set; }
        [JsonProperty("updateby")]
        [StringLength(20)]
        public virtual string UpdateBy { get; set; }
        [JsonProperty("status")]
        public virtual byte? Status { get; set; }
        [JsonProperty("transvoid")]
        public virtual byte? TransVoid { get; set; }
        [JsonProperty("flag")]
        public virtual byte? Flag { get; set; }
        [JsonProperty("synccreate")]
        public virtual System.DateTime? SyncCreate { get; set; }
        [JsonProperty("synclastupd")]
        public virtual System.DateTime? SyncLastUpd { get; set; }
        [JsonProperty("tblno")]
        [StringLength(10)]
        public virtual string TblNo { get; set; }
        [JsonProperty("tblpax")]
        public virtual int? TblPax { get; set; }
        [JsonProperty("transpointsstatus")]
        public virtual byte? TransPointsStatus { get; set; }
        [JsonProperty("ishost")]
        public virtual byte? IsHost { get; set; }
        [JsonProperty("lastsyncby")]
        public virtual byte? LastSyncBy { get; set; }
        [JsonProperty("transpoints")]
        public virtual decimal? TransPoints { get; set; }
        [JsonProperty("synccreateby")]
        [StringLength(10)]
        public virtual string SyncCreateBy { get; set; }
        [JsonProperty("tid")]
        public virtual int? TID { get; set; }
    }
    #endregion

    #region TRANSDTL
    [Abp.AutoMapper.AutoMapTo(typeof(DTO.Accounting.TRANSDTL))]
    public class MapTRANSDTL
    {
        [Required, StringLength(20)]
        [JsonProperty("bizlocid")]
        public virtual string BizLocID { get; set; }
        [Required, Range(0, 2)]
        [JsonProperty("termid")]
        public virtual int? TermID { get; set; }
        [Required, StringLength(20)]
        [JsonProperty("transno")]
        public virtual string TransNo { get; set; }
        [Required, Range(0, 100)]
        [JsonProperty("transseq")]
        public virtual int? TransSeq { get; set; }
        [JsonProperty("bizregid")]
        [StringLength(20)]
        public virtual string BizRegID { get; set; }
        [JsonProperty("refseq")]
        public virtual int? RefSeq { get; set; }
        [JsonProperty("iscal")]
        public virtual byte? IsCal { get; set; }
        [JsonProperty("billno")]
        [StringLength(20)]
        public virtual string BillNo { get; set; }
        [JsonProperty("entrytime")]
        [StringLength(6)]
        public virtual string EntryTime { get; set; }
        [JsonProperty("stkcode")]
        [StringLength(20)]
        public virtual string StkCode { get; set; }
        [JsonProperty("stkdesc")]
        [StringLength(50)]
        public virtual string StkDesc { get; set; }
        [JsonProperty("stktype")]
        [StringLength(3)]
        public virtual string StkType { get; set; }
        [JsonProperty("behvtype")]
        public virtual byte? BehvType { get; set; }
        [JsonProperty("itemtype")]
        public virtual byte? ItemType { get; set; }
        [JsonProperty("itemcode")]
        [StringLength(20)]
        public virtual string ItemCode { get; set; }
        [JsonProperty("qty")]
        public virtual int? Qty { get; set; }
        [JsonProperty("unitcost")]
        public virtual decimal? UnitCost { get; set; }
        [JsonProperty("orgprice")]
        public virtual decimal? OrgPrice { get; set; }
        [JsonProperty("nettprice")]
        public virtual decimal? NettPrice { get; set; }
        [JsonProperty("tolamt")]
        public virtual decimal? TolAmt { get; set; }
        [JsonProperty("discamt")]
        public virtual decimal? DiscAmt { get; set; }
        [JsonProperty("nettamt")]
        public virtual decimal? NettAmt { get; set; }
        [JsonProperty("subpoints")]
        public virtual decimal? SubPoints { get; set; }
        [JsonProperty("points")]
        public virtual decimal? Points { get; set; }
        [JsonProperty("priceoverrided")]
        public virtual byte? PriceOverrided { get; set; }
        [JsonProperty("discounted")]
        public virtual byte? Discounted { get; set; }
        [JsonProperty("taxable")]
        public virtual byte? Taxable { get; set; }
        [JsonProperty("returncode")]
        [StringLength(10)]
        public virtual string ReturnCode { get; set; }
        [JsonProperty("remark")]
        [StringLength(4000)]
        public virtual string Remark { get; set; }
        [JsonProperty("serialno")]
        [StringLength(50)]
        public virtual string SerialNo { get; set; }
        [JsonProperty("priceauthid")]
        [StringLength(20)]
        public virtual string PriceAuthID { get; set; }
        [JsonProperty("transvoid")]
        public virtual byte? TransVoid { get; set; }
        [JsonProperty("posted")]
        public virtual byte? Posted { get; set; }
        [JsonProperty("livecal")]
        public virtual byte? LiveCal { get; set; }
        [JsonProperty("flag")]
        public virtual byte? Flag { get; set; }
        [JsonProperty("custpkgid")]
        [StringLength(20)]
        public virtual string CustPkgID { get; set; }
        [JsonProperty("prcdisplaytype")]
        public virtual byte? PrcDisplayType { get; set; }
        [JsonProperty("promocode")]
        [StringLength(20)]
        public virtual string PromoCode { get; set; }
        [JsonProperty("createdate")]
        public virtual System.DateTime? CreateDate { get; set; }
        [JsonProperty("lastupdate")]
        public virtual System.DateTime? LastUpdate { get; set; }
        [JsonProperty("updateby")]
        [StringLength(20)]
        public virtual string UpdateBy { get; set; }
        [JsonProperty("status")]
        public virtual byte? Status { get; set; }
        [JsonProperty("synccreate")]
        public virtual System.DateTime? SyncCreate { get; set; }
        [JsonProperty("synclastupd")]
        public virtual System.DateTime? SyncLastUpd { get; set; }
        [JsonProperty("ishost")]
        public virtual byte? IsHost { get; set; }
        [JsonProperty("lastsyncby")]
        [StringLength(20)]
        public virtual string LastSyncBy { get; set; }
        [JsonProperty("synccreateby")]
        [StringLength(20)]
        public virtual string SyncCreateBy { get; set; }
        [JsonProperty("excode1")]
        [StringLength(10)]
        public virtual string ExCode1 { get; set; }
        [JsonProperty("excode2")]
        [StringLength(10)]
        public virtual string ExCode2 { get; set; }
        [JsonProperty("baserate")]
        public virtual decimal? BaseRate { get; set; }
        [JsonProperty("corate1h")]
        public virtual decimal? CoRate1H { get; set; }
        [JsonProperty("corate1g")]
        public virtual decimal? CoRate1G { get; set; }
        [JsonProperty("corate2h")]
        public virtual decimal? CoRate2H { get; set; }
        [JsonProperty("corate2g")]
        public virtual decimal? CoRate2G { get; set; }
        [JsonProperty("corate3h")]
        public virtual decimal? CoRate3H { get; set; }
        [JsonProperty("corate3g")]
        public virtual decimal? CoRate3G { get; set; }
        [JsonProperty("corate4h")]
        public virtual decimal? CoRate4H { get; set; }
        [JsonProperty("corate4g")]
        public virtual decimal? CoRate4G { get; set; }
        [JsonProperty("corate5h")]
        public virtual decimal? CoRate5H { get; set; }
        [JsonProperty("corate5g")]
        public virtual decimal? CoRate5G { get; set; }
        [JsonProperty("warserialno")]
        [StringLength(20)]
        public virtual string WarSerialNo { get; set; }
        [JsonProperty("tid")]
        public virtual int? TID { get; set; }
    }
    #endregion

    #region TRANSFPX
    [Abp.AutoMapper.AutoMapTo(typeof(DTO.Accounting.TRANSFPX))]
    public class MapTRANSFPX
    {
        [Required, StringLength(20)]
        [JsonProperty("bizregid")]
        public virtual string BizRegID { get; set; }
        [Required, StringLength(20)]
        [JsonProperty("bizlocid")]
        public virtual string BizLocID { get; set; }
        [Required, Range(0, 2)]
        [JsonProperty("termid")]
        public virtual int? TermID { get; set; }
        [Required, StringLength(20)]
        [JsonProperty("transno")]
        public virtual string TransNo { get; set; }
        [Required, Range(0, 4)]
        [JsonProperty("transseq")]
        public virtual int? TransSeq { get; set; }
        [JsonProperty("transdate")]
        public virtual System.DateTime? TransDate { get; set; }
        [JsonProperty("transtime")]
        [StringLength(6)]
        public virtual string TransTime { get; set; }
        [JsonProperty("tenderid")]
        [StringLength(50)]
        public virtual string TenderID { get; set; }
        [JsonProperty("tenderamt")]
        public virtual decimal? TenderAmt { get; set; }
        [JsonProperty("exchgrate")]
        public virtual decimal? ExchgRate { get; set; }
        [JsonProperty("tenderdue")]
        public virtual decimal? TenderDue { get; set; }
        [JsonProperty("changeamt")]
        public virtual decimal? ChangeAmt { get; set; }
        [JsonProperty("refinfo")]
        [StringLength(4000)]
        public virtual string RefInfo { get; set; }
        [JsonProperty("ccrefinfo")]
        [StringLength(255)]
        public virtual string CCrefInfo { get; set; }
        [JsonProperty("expdate")]
        public virtual System.DateTime? ExpDate { get; set; }
        [JsonProperty("custname")]
        [StringLength(255)]
        public virtual string CustName { get; set; }
        [JsonProperty("clearingdate")]
        public virtual System.DateTime? ClearingDate { get; set; }
        [JsonProperty("transvoid")]
        public virtual byte? TransVoid { get; set; }
        [JsonProperty("posted")]
        public virtual byte? Posted { get; set; }
        [JsonProperty("createdate")]
        public virtual System.DateTime? CreateDate { get; set; }
        [JsonProperty("createby")]
        [StringLength(20)]
        public virtual string CreateBy { get; set; }
        [JsonProperty("lastupdate")]
        public virtual System.DateTime? LastUpdate { get; set; }
        [JsonProperty("updateby")]
        [StringLength(20)]
        public virtual string UpdateBy { get; set; }
        [JsonProperty("ishost")]
        public virtual byte? IsHost { get; set; }
        [JsonProperty("lastsyncby")]
        public virtual byte? LastSyncBy { get; set; }
        [JsonProperty("active")]
        public virtual byte? Active { get; set; }
        [JsonProperty("inuse")]
        public virtual byte? Inuse { get; set; }
        [JsonProperty("flag")]
        public virtual byte? Flag { get; set; }
        [JsonProperty("synccreate")]
        public virtual System.DateTime? SyncCreate { get; set; }
        [JsonProperty("synclastupd")]
        public virtual System.DateTime? SyncLastUpd { get; set; }
        [JsonProperty("tid")]
        public virtual int? TID { get; set; }
    }
    #endregion

    #region TRANSTENDER
    [Abp.AutoMapper.AutoMapTo(typeof(DTO.Accounting.TRANSTENDER))]
    public class MapTRANSTENDER
    {
        [Required, StringLength(20)]
        [JsonProperty("bizregid")]
        public virtual string BizRegID { get; set; }
        [Required, StringLength(20)]
        [JsonProperty("bizlocid")]
        public virtual string BizLocID { get; set; }
        [Required, Range(0, 2)]
        [JsonProperty("termid")]
        public virtual int? TermID { get; set; }
        [Required, StringLength(20)]
        [JsonProperty("transno")]
        public virtual string TransNo { get; set; }
        [Required, Range(0, 100)]
        [JsonProperty("transseq")]
        public virtual int? TransSeq { get; set; }
        [JsonProperty("billno")]
        [StringLength(20)]
        public virtual string BillNo { get; set; }
        [JsonProperty("transdate")]
        public virtual System.DateTime? TransDate { get; set; }
        [JsonProperty("transtime")]
        [StringLength(6)]
        public virtual string TransTime { get; set; }
        [JsonProperty("tendertype")]
        [StringLength(10)]
        public virtual string TenderType { get; set; }
        [JsonProperty("tenderid")]
        [StringLength(10)]
        public virtual string TenderID { get; set; }
        [JsonProperty("tenderamt")]
        public virtual decimal? TenderAmt { get; set; }
        [JsonProperty("exchgrate")]
        public virtual decimal? ExchgRate { get; set; }
        [JsonProperty("tenderdue")]
        public virtual decimal? TenderDue { get; set; }
        [JsonProperty("changeamt")]
        public virtual decimal? ChangeAmt { get; set; }
        [JsonProperty("currency")]
        [StringLength(3)]
        public virtual string Currency { get; set; }
        [JsonProperty("externalid")]
        [StringLength(255)]
        public virtual string ExternalID { get; set; }
        [JsonProperty("merchantid")]
        [StringLength(255)]
        public virtual string MerchantID { get; set; }
        [JsonProperty("refno")]
        [StringLength(255)]
        public virtual string RefNo { get; set; }
        [JsonProperty("refkey")]
        [StringLength(255)]
        public virtual string RefKey { get; set; }
        [JsonProperty("reftype")]
        [StringLength(20)]
        public virtual string RefType { get; set; }
        [JsonProperty("reftoken")]
        [StringLength(4000)]
        public virtual string RefToken { get; set; }
        [JsonProperty("refremark")]
        [StringLength(200)]
        public virtual string RefRemark { get; set; }
        [JsonProperty("refremark2")]
        [StringLength(200)]
        public virtual string RefRemark2 { get; set; }
        [JsonProperty("refremark3")]
        [StringLength(200)]
        public virtual string RefRemark3 { get; set; }
        [JsonProperty("refoth1")]
        [StringLength(255)]
        public virtual string RefOth1 { get; set; }
        [JsonProperty("refoth2")]
        [StringLength(255)]
        public virtual string RefOth2 { get; set; }
        [JsonProperty("refoth3")]
        [StringLength(255)]
        public virtual string RefOth3 { get; set; }
        [JsonProperty("custname")]
        [StringLength(200)]
        public virtual string CustName { get; set; }
        [JsonProperty("cardno")]
        [StringLength(255)]
        public virtual string CardNo { get; set; }
        [JsonProperty("bankcode")]
        [StringLength(50)]
        public virtual string BankCode { get; set; }
        [JsonProperty("bankaccno")]
        [StringLength(50)]
        public virtual string BankAccNo { get; set; }
        [JsonProperty("respcode")]
        [StringLength(50)]
        public virtual string RespCode { get; set; }
        [JsonProperty("authcode")]
        [StringLength(4000)]
        public virtual string AuthCode { get; set; }
        [JsonProperty("traceno")]
        [StringLength(255)]
        public virtual string TraceNo { get; set; }
        [JsonProperty("expdate")]
        public virtual System.DateTime? ExpDate { get; set; }
        [JsonProperty("clearingdate")]
        public virtual System.DateTime? ClearingDate { get; set; }
        [JsonProperty("isapproved")]
        public virtual byte? IsApproved { get; set; }
        [JsonProperty("approvedby")]
        [StringLength(20)]
        public virtual string ApprovedBy { get; set; }
        [JsonProperty("approveddate")]
        public virtual System.DateTime? ApprovedDate { get; set; }
        [JsonProperty("approvalflow")]
        [StringLength(20)]
        public virtual string ApprovalFlow { get; set; }
        [JsonProperty("transstatus")]
        public virtual byte? TransStatus { get; set; }
        [JsonProperty("transvoid")]
        public virtual byte? TransVoid { get; set; }
        [JsonProperty("posted")]
        public virtual byte? Posted { get; set; }
        [JsonProperty("createdate")]
        public virtual System.DateTime? CreateDate { get; set; }
        [JsonProperty("createby")]
        [StringLength(20)]
        public virtual string CreateBy { get; set; }
        [JsonProperty("lastupdate")]
        public virtual System.DateTime? LastUpdate { get; set; }
        [JsonProperty("updateby")]
        [StringLength(20)]
        public virtual string UpdateBy { get; set; }
        [JsonProperty("synccreate")]
        public virtual System.DateTime? SyncCreate { get; set; }
        [JsonProperty("synclastupd")]
        public virtual System.DateTime? SyncLastUpd { get; set; }
        [JsonProperty("ishost")]
        public virtual byte? IsHost { get; set; }
        [JsonProperty("lastsyncby")]
        [StringLength(20)]
        public virtual string LastSyncBy { get; set; }
    }
    #endregion

    #region TENDER
    [Abp.AutoMapper.AutoMapTo(typeof(DTO.Accounting.TENDER))]
    public class MapTENDER
    {
        [Required, StringLength(10)]
        [JsonProperty("tenderid")]
        public virtual string TenderID { get; set; }
        [JsonProperty("tendertype")]
        public virtual byte? TenderType { get; set; }
        [JsonProperty("tenderdesc")]
        [StringLength(50)]
        public virtual string TenderDesc { get; set; }
        [JsonProperty("currencycode")]
        [StringLength(3)]
        public virtual string CurrencyCode { get; set; }
        [JsonProperty("tenderprompt")]
        [StringLength(50)]
        public virtual string TenderPrompt { get; set; }
        [JsonProperty("refprompt")]
        [StringLength(50)]
        public virtual string RefPrompt { get; set; }
        [JsonProperty("defvalue")]
        public virtual decimal? DefValue { get; set; }
        [JsonProperty("pickup1")]
        public virtual decimal? Pickup1 { get; set; }
        [JsonProperty("pickup2")]
        public virtual decimal? Pickup2 { get; set; }
        [JsonProperty("mintenderamt")]
        public virtual decimal? MinTenderAmt { get; set; }
        [JsonProperty("maxtenderamt")]
        public virtual decimal? MaxTenderAmt { get; set; }
        [JsonProperty("allowpickup")]
        public virtual byte? AllowPickup { get; set; }
        [JsonProperty("allowfloat")]
        public virtual byte? AllowFloat { get; set; }
        [JsonProperty("allowovertender")]
        public virtual byte? AllowOverTender { get; set; }
        [JsonProperty("opendrawer")]
        public virtual byte? OpenDrawer { get; set; }
        [JsonProperty("trackrefno")]
        public virtual byte? TrackRefNo { get; set; }
        [JsonProperty("trackclrdate")]
        public virtual byte? TrackClrDate { get; set; }
        [JsonProperty("createdate")]
        public virtual System.DateTime? CreateDate { get; set; }
        [JsonProperty("createby")]
        [StringLength(20)]
        public virtual string CreateBy { get; set; }
        [JsonProperty("lastupdate")]
        public virtual System.DateTime? LastUpdate { get; set; }
        [JsonProperty("updateby")]
        [StringLength(20)]
        public virtual string UpdateBy { get; set; }
        [JsonProperty("active")]
        public virtual byte? Active { get; set; }
        [JsonProperty("inuse")]
        public virtual byte? Inuse { get; set; }
        [JsonProperty("flag")]
        public virtual byte? Flag { get; set; }
        [JsonProperty("ishost")]
        public virtual byte? IsHost { get; set; }
        [JsonProperty("effdate")]
        public virtual System.DateTime? EffDate { get; set; }
        [JsonProperty("enddate")]
        public virtual System.DateTime? EndDate { get; set; }
        [JsonProperty("synccreate")]
        public virtual System.DateTime? SyncCreate { get; set; }
        [JsonProperty("synclastupd")]
        public virtual System.DateTime? SyncLastUpd { get; set; }
        [JsonProperty("lastsyncby")]
        public virtual byte? LastSyncBy { get; set; }
        [JsonProperty("trackappcode")]
        public virtual byte? TrackAppCode { get; set; }
        [JsonProperty("tid")]
        public virtual int? TID { get; set; }
    }
    #endregion

    #region TRANSCASHBILLHDR
    //[Abp.AutoMapper.AutoMapTo(typeof(DTO.Accounting.TRANSCASHBILLHDR))]
    //public class MapTRANSCASHBILLHDR
    //{
    //    [Required, StringLength(20)]
    //    [JsonProperty("bizregid")]
    //    public virtual string BizRegID { get; set; }
    //    [Required, StringLength(20)]
    //    [JsonProperty("bizlocid")]
    //    public virtual string BizLocID { get; set; }
    //    [Required, StringLength(20)]
    //    [JsonProperty("billno")]
    //    public virtual string BillNo { get; set; }
    //    [Required, StringLength(20)]
    //    [JsonProperty("transno")]
    //    public virtual string TransNo { get; set; }
    //    [JsonProperty("termid")]
    //    public virtual int? TermID { get; set; }
    //    [JsonProperty("transtype")]
    //    public virtual byte? TransType { get; set; }
    //    [JsonProperty("custpkgid")]
    //    [StringLength(20)]
    //    public virtual string CustPkgID { get; set; }
    //    [JsonProperty("cashierid")]
    //    [StringLength(20)]
    //    public virtual string CashierID { get; set; }
    //    [JsonProperty("shiftcode")]
    //    [StringLength(10)]
    //    public virtual string ShiftCode { get; set; }
    //    [JsonProperty("totalserver")]
    //    public virtual int? TotalServer { get; set; }
    //    [JsonProperty("serverid")]
    //    [StringLength(20)]
    //    public virtual string ServerID { get; set; }
    //    [JsonProperty("emptype")]
    //    public virtual byte? EmpType { get; set; }
    //    [JsonProperty("tillid")]
    //    public virtual int? TillID { get; set; }
    //    [JsonProperty("transdate")]
    //    public virtual System.DateTime? TransDate { get; set; }
    //    [JsonProperty("transstarttime")]
    //    [StringLength(6)]
    //    public virtual string TransStartTime { get; set; }
    //    [JsonProperty("transendtime")]
    //    [StringLength(6)]
    //    public virtual string TransEndTime { get; set; }
    //    [JsonProperty("transpaytime")]
    //    [StringLength(6)]
    //    public virtual string TransPayTime { get; set; }
    //    [JsonProperty("transamt")]
    //    public virtual decimal? TransAmt { get; set; }
    //    [JsonProperty("transsubtotal")]
    //    public virtual decimal? TransSubTotal { get; set; }
    //    [JsonProperty("transamtrnd")]
    //    public virtual decimal? TransAmtRnd { get; set; }
    //    [JsonProperty("transamtsave")]
    //    public virtual decimal? TransAmtSave { get; set; }
    //    [JsonProperty("transamtorg")]
    //    public virtual decimal? TransAmtOrg { get; set; }
    //    [JsonProperty("transchgamt")]
    //    public virtual decimal? TransChgAmt { get; set; }
    //    [JsonProperty("transdisccode")]
    //    [StringLength(10)]
    //    public virtual string TransDiscCode { get; set; }
    //    [JsonProperty("transdisctype")]
    //    public virtual byte? TransDiscType { get; set; }
    //    [JsonProperty("transdiscvalue")]
    //    public virtual decimal? TransDiscValue { get; set; }
    //    [JsonProperty("transdiscamt")]
    //    public virtual decimal? TransDiscAmt { get; set; }
    //    [JsonProperty("transdiscauth")]
    //    [StringLength(20)]
    //    public virtual string TransDiscAuth { get; set; }
    //    [JsonProperty("transdiscreasoncode")]
    //    [StringLength(50)]
    //    public virtual string TransDiscReasonCode { get; set; }
    //    [JsonProperty("transdiscremark")]
    //    [StringLength(50)]
    //    public virtual string TransDiscRemark { get; set; }
    //    [JsonProperty("transamtspdisc")]
    //    public virtual decimal? TransAmtSpDisc { get; set; }
    //    [JsonProperty("transvaluespdisc")]
    //    public virtual decimal? TransValueSpDisc { get; set; }
    //    [JsonProperty("authspdisc")]
    //    [StringLength(20)]
    //    public virtual string AuthSpDisc { get; set; }
    //    [JsonProperty("spdiscreasoncode")]
    //    [StringLength(10)]
    //    public virtual string SpDiscReasonCode { get; set; }
    //    [JsonProperty("spdiscremark")]
    //    [StringLength(200)]
    //    public virtual string SpDiscRemark { get; set; }
    //    [JsonProperty("customerid")]
    //    [StringLength(20)]
    //    public virtual string CustomerID { get; set; }
    //    [JsonProperty("custtype")]
    //    public virtual byte? CustType { get; set; }
    //    [JsonProperty("custprivilege")]
    //    [StringLength(255)]
    //    public virtual string CustPrivilege { get; set; }
    //    [JsonProperty("acctno")]
    //    [StringLength(20)]
    //    public virtual string AcctNo { get; set; }
    //    [JsonProperty("totalpoints")]
    //    public virtual decimal? TotalPoints { get; set; }
    //    [JsonProperty("insvcid")]
    //    [StringLength(20)]
    //    public virtual string InSvcID { get; set; }
    //    [JsonProperty("transreasoncode")]
    //    [StringLength(20)]
    //    public virtual string TransReasonCode { get; set; }
    //    [JsonProperty("transremark")]
    //    [StringLength(255)]
    //    public virtual string TransRemark { get; set; }
    //    [JsonProperty("transstatus")]
    //    [StringLength(20)]
    //    public virtual string TransStatus { get; set; }
    //    [JsonProperty("posted")]
    //    public virtual byte? Posted { get; set; }
    //    [JsonProperty("postdate")]
    //    public virtual System.DateTime? PostDate { get; set; }
    //    [JsonProperty("training")]
    //    public virtual byte? Training { get; set; }
    //    [JsonProperty("profiled")]
    //    public virtual byte? Profiled { get; set; }
    //    [JsonProperty("livecal")]
    //    public virtual byte? LiveCal { get; set; }
    //    [JsonProperty("createdate")]
    //    public virtual System.DateTime? CreateDate { get; set; }
    //    [JsonProperty("createby")]
    //    [StringLength(20)]
    //    public virtual string CreateBy { get; set; }
    //    [JsonProperty("lastupdate")]
    //    public virtual System.DateTime? LastUpdate { get; set; }
    //    [JsonProperty("updateby")]
    //    [StringLength(20)]
    //    public virtual string UpdateBy { get; set; }
    //    [JsonProperty("status")]
    //    public virtual byte? Status { get; set; }
    //    [JsonProperty("transvoid")]
    //    public virtual byte? TransVoid { get; set; }
    //    [JsonProperty("flag")]
    //    public virtual byte? Flag { get; set; }
    //    [JsonProperty("synccreate")]
    //    public virtual System.DateTime? SyncCreate { get; set; }
    //    [JsonProperty("synclastupd")]
    //    public virtual System.DateTime? SyncLastUpd { get; set; }
    //    [JsonProperty("tblno")]
    //    [StringLength(10)]
    //    public virtual string TblNo { get; set; }
    //    [JsonProperty("tblpax")]
    //    public virtual int? TblPax { get; set; }
    //    [JsonProperty("transpointsstatus")]
    //    public virtual byte? TransPointsStatus { get; set; }
    //    [JsonProperty("ishost")]
    //    public virtual byte? IsHost { get; set; }
    //    [JsonProperty("lastsyncby")]
    //    public virtual byte? LastSyncBy { get; set; }
    //    [JsonProperty("transpoints")]
    //    public virtual decimal? TransPoints { get; set; }
    //    [JsonProperty("synccreateby")]
    //    [StringLength(10)]
    //    public virtual string SyncCreateBy { get; set; }
    //    [JsonProperty("tid")]
    //    public virtual int? TID { get; set; }
    //}
    #endregion

    #region TRANSCASHBILLDTL
    //[Abp.AutoMapper.AutoMapTo(typeof(DTO.Accounting.TRANSCASHBILLDTL))]
    //public class MapTRANSCASHBILLDTL
    //{
    //    [Required, StringLength(20)]
    //    [JsonProperty("bizlocid")]
    //    public virtual string BizLocID { get; set; }
    //    [Required, StringLength(20)]
    //    [JsonProperty("transno")]
    //    public virtual string TransNo { get; set; }
    //    [Required, Range(0, 4)]
    //    [JsonProperty("transseq")]
    //    public virtual int? TransSeq { get; set; }
    //    [Required, StringLength(20)]
    //    [JsonProperty("itemcode")]
    //    public virtual string ItemCode { get; set; }
    //    [JsonProperty("bizregid")]
    //    [StringLength(20)]
    //    public virtual string BizRegID { get; set; }
    //    [JsonProperty("termid")]
    //    public virtual int? TermID { get; set; }
    //    [JsonProperty("refseq")]
    //    public virtual int? RefSeq { get; set; }
    //    [JsonProperty("iscal")]
    //    public virtual byte? IsCal { get; set; }
    //    [JsonProperty("billno")]
    //    [StringLength(20)]
    //    public virtual string BillNo { get; set; }
    //    [JsonProperty("entrytime")]
    //    [StringLength(6)]
    //    public virtual string EntryTime { get; set; }
    //    [JsonProperty("stkcode")]
    //    [StringLength(20)]
    //    public virtual string StkCode { get; set; }
    //    [JsonProperty("stkdesc")]
    //    [StringLength(50)]
    //    public virtual string StkDesc { get; set; }
    //    [JsonProperty("stktype")]
    //    [StringLength(3)]
    //    public virtual string StkType { get; set; }
    //    [JsonProperty("behvtype")]
    //    public virtual byte? BehvType { get; set; }
    //    [JsonProperty("itemtype")]
    //    public virtual byte? ItemType { get; set; }
    //    [JsonProperty("qty")]
    //    public virtual int? Qty { get; set; }
    //    [JsonProperty("unitcost")]
    //    public virtual decimal? UnitCost { get; set; }
    //    [JsonProperty("orgprice")]
    //    public virtual decimal? OrgPrice { get; set; }
    //    [JsonProperty("nettprice")]
    //    public virtual decimal? NettPrice { get; set; }
    //    [JsonProperty("tolamt")]
    //    public virtual decimal? TolAmt { get; set; }
    //    [JsonProperty("discamt")]
    //    public virtual decimal? DiscAmt { get; set; }
    //    [JsonProperty("nettamt")]
    //    public virtual decimal? NettAmt { get; set; }
    //    [JsonProperty("subpoints")]
    //    public virtual decimal? SubPoints { get; set; }
    //    [JsonProperty("points")]
    //    public virtual decimal? Points { get; set; }
    //    [JsonProperty("priceoverrided")]
    //    public virtual byte? PriceOverrided { get; set; }
    //    [JsonProperty("discounted")]
    //    public virtual byte? Discounted { get; set; }
    //    [JsonProperty("taxable")]
    //    public virtual byte? Taxable { get; set; }
    //    [JsonProperty("returncode")]
    //    [StringLength(10)]
    //    public virtual string ReturnCode { get; set; }
    //    [JsonProperty("remark")]
    //    [StringLength(4000)]
    //    public virtual string Remark { get; set; }
    //    [JsonProperty("serialno")]
    //    [StringLength(50)]
    //    public virtual string SerialNo { get; set; }
    //    [JsonProperty("priceauthid")]
    //    [StringLength(20)]
    //    public virtual string PriceAuthID { get; set; }
    //    [JsonProperty("transvoid")]
    //    public virtual byte? TransVoid { get; set; }
    //    [JsonProperty("posted")]
    //    public virtual byte? Posted { get; set; }
    //    [JsonProperty("livecal")]
    //    public virtual byte? LiveCal { get; set; }
    //    [JsonProperty("flag")]
    //    public virtual byte? Flag { get; set; }
    //    [JsonProperty("custpkgid")]
    //    [StringLength(20)]
    //    public virtual string CustPkgID { get; set; }
    //    [JsonProperty("prcdisplaytype")]
    //    public virtual byte? PrcDisplayType { get; set; }
    //    [JsonProperty("promocode")]
    //    [StringLength(20)]
    //    public virtual string PromoCode { get; set; }
    //    [JsonProperty("createdate")]
    //    public virtual System.DateTime? CreateDate { get; set; }
    //    [JsonProperty("lastupdate")]
    //    public virtual System.DateTime? LastUpdate { get; set; }
    //    [JsonProperty("updateby")]
    //    [StringLength(20)]
    //    public virtual string UpdateBy { get; set; }
    //    [JsonProperty("status")]
    //    public virtual byte? Status { get; set; }
    //    [JsonProperty("synccreate")]
    //    public virtual System.DateTime? SyncCreate { get; set; }
    //    [JsonProperty("synclastupd")]
    //    public virtual System.DateTime? SyncLastUpd { get; set; }
    //    [JsonProperty("ishost")]
    //    public virtual byte? IsHost { get; set; }
    //    [JsonProperty("lastsyncby")]
    //    [StringLength(20)]
    //    public virtual string LastSyncBy { get; set; }
    //    [JsonProperty("synccreateby")]
    //    [StringLength(20)]
    //    public virtual string SyncCreateBy { get; set; }
    //    [JsonProperty("excode1")]
    //    [StringLength(10)]
    //    public virtual string ExCode1 { get; set; }
    //    [JsonProperty("excode2")]
    //    [StringLength(10)]
    //    public virtual string ExCode2 { get; set; }
    //    [JsonProperty("baserate")]
    //    public virtual decimal? BaseRate { get; set; }
    //    [JsonProperty("corate1h")]
    //    public virtual decimal? CoRate1H { get; set; }
    //    [JsonProperty("corate1g")]
    //    public virtual decimal? CoRate1G { get; set; }
    //    [JsonProperty("corate2h")]
    //    public virtual decimal? CoRate2H { get; set; }
    //    [JsonProperty("corate2g")]
    //    public virtual decimal? CoRate2G { get; set; }
    //    [JsonProperty("corate3h")]
    //    public virtual decimal? CoRate3H { get; set; }
    //    [JsonProperty("corate3g")]
    //    public virtual decimal? CoRate3G { get; set; }
    //    [JsonProperty("corate4h")]
    //    public virtual decimal? CoRate4H { get; set; }
    //    [JsonProperty("corate4g")]
    //    public virtual decimal? CoRate4G { get; set; }
    //    [JsonProperty("corate5h")]
    //    public virtual decimal? CoRate5H { get; set; }
    //    [JsonProperty("corate5g")]
    //    public virtual decimal? CoRate5G { get; set; }
    //    [JsonProperty("warserialno")]
    //    [StringLength(20)]
    //    public virtual string WarSerialNo { get; set; }
    //    [JsonProperty("tid")]
    //    public virtual int? TID { get; set; }
    //}
    #endregion

    #region TRANSCDNHDR
    //[Abp.AutoMapper.AutoMapTo(typeof(DTO.Accounting.TRANSCDNHDR))]
    //public class MapTRANSCDNHDR
    //{
    //    [Required, StringLength(20)]
    //    [JsonProperty("bizregid")]
    //    public virtual string BizRegID { get; set; }
    //    [Required, StringLength(20)]
    //    [JsonProperty("bizlocid")]
    //    public virtual string BizLocID { get; set; }
    //    [Required, StringLength(20)]
    //    [JsonProperty("billno")]
    //    public virtual string BillNo { get; set; }
    //    [Required, StringLength(20)]
    //    [JsonProperty("transno")]
    //    public virtual string TransNo { get; set; }
    //    [JsonProperty("termid")]
    //    public virtual int? TermID { get; set; }
    //    [JsonProperty("transtype")]
    //    public virtual byte? TransType { get; set; }
    //    [JsonProperty("custpkgid")]
    //    [StringLength(20)]
    //    public virtual string CustPkgID { get; set; }
    //    [JsonProperty("cashierid")]
    //    [StringLength(20)]
    //    public virtual string CashierID { get; set; }
    //    [JsonProperty("shiftcode")]
    //    [StringLength(10)]
    //    public virtual string ShiftCode { get; set; }
    //    [JsonProperty("totalserver")]
    //    public virtual int? TotalServer { get; set; }
    //    [JsonProperty("serverid")]
    //    [StringLength(20)]
    //    public virtual string ServerID { get; set; }
    //    [JsonProperty("emptype")]
    //    public virtual byte? EmpType { get; set; }
    //    [JsonProperty("tillid")]
    //    public virtual int? TillID { get; set; }
    //    [JsonProperty("transdate")]
    //    public virtual System.DateTime? TransDate { get; set; }
    //    [JsonProperty("transstarttime")]
    //    [StringLength(6)]
    //    public virtual string TransStartTime { get; set; }
    //    [JsonProperty("transendtime")]
    //    [StringLength(6)]
    //    public virtual string TransEndTime { get; set; }
    //    [JsonProperty("transpaytime")]
    //    [StringLength(6)]
    //    public virtual string TransPayTime { get; set; }
    //    [JsonProperty("transamt")]
    //    public virtual decimal? TransAmt { get; set; }
    //    [JsonProperty("transsubtotal")]
    //    public virtual decimal? TransSubTotal { get; set; }
    //    [JsonProperty("transamtrnd")]
    //    public virtual decimal? TransAmtRnd { get; set; }
    //    [JsonProperty("transamtsave")]
    //    public virtual decimal? TransAmtSave { get; set; }
    //    [JsonProperty("transamtorg")]
    //    public virtual decimal? TransAmtOrg { get; set; }
    //    [JsonProperty("transchgamt")]
    //    public virtual decimal? TransChgAmt { get; set; }
    //    [JsonProperty("transdisccode")]
    //    [StringLength(10)]
    //    public virtual string TransDiscCode { get; set; }
    //    [JsonProperty("transdisctype")]
    //    public virtual byte? TransDiscType { get; set; }
    //    [JsonProperty("transdiscvalue")]
    //    public virtual decimal? TransDiscValue { get; set; }
    //    [JsonProperty("transdiscamt")]
    //    public virtual decimal? TransDiscAmt { get; set; }
    //    [JsonProperty("transdiscauth")]
    //    [StringLength(20)]
    //    public virtual string TransDiscAuth { get; set; }
    //    [JsonProperty("transdiscreasoncode")]
    //    [StringLength(50)]
    //    public virtual string TransDiscReasonCode { get; set; }
    //    [JsonProperty("transdiscremark")]
    //    [StringLength(50)]
    //    public virtual string TransDiscRemark { get; set; }
    //    [JsonProperty("transamtspdisc")]
    //    public virtual decimal? TransAmtSpDisc { get; set; }
    //    [JsonProperty("transvaluespdisc")]
    //    public virtual decimal? TransValueSpDisc { get; set; }
    //    [JsonProperty("authspdisc")]
    //    [StringLength(20)]
    //    public virtual string AuthSpDisc { get; set; }
    //    [JsonProperty("spdiscreasoncode")]
    //    [StringLength(10)]
    //    public virtual string SpDiscReasonCode { get; set; }
    //    [JsonProperty("spdiscremark")]
    //    [StringLength(200)]
    //    public virtual string SpDiscRemark { get; set; }
    //    [JsonProperty("customerid")]
    //    [StringLength(20)]
    //    public virtual string CustomerID { get; set; }
    //    [JsonProperty("custtype")]
    //    public virtual byte? CustType { get; set; }
    //    [JsonProperty("custprivilege")]
    //    [StringLength(255)]
    //    public virtual string CustPrivilege { get; set; }
    //    [JsonProperty("acctno")]
    //    [StringLength(20)]
    //    public virtual string AcctNo { get; set; }
    //    [JsonProperty("totalpoints")]
    //    public virtual decimal? TotalPoints { get; set; }
    //    [JsonProperty("insvcid")]
    //    [StringLength(20)]
    //    public virtual string InSvcID { get; set; }
    //    [JsonProperty("transreasoncode")]
    //    [StringLength(20)]
    //    public virtual string TransReasonCode { get; set; }
    //    [JsonProperty("transremark")]
    //    [StringLength(255)]
    //    public virtual string TransRemark { get; set; }
    //    [JsonProperty("transstatus")]
    //    [StringLength(20)]
    //    public virtual string TransStatus { get; set; }
    //    [JsonProperty("posted")]
    //    public virtual byte? Posted { get; set; }
    //    [JsonProperty("postdate")]
    //    public virtual System.DateTime? PostDate { get; set; }
    //    [JsonProperty("training")]
    //    public virtual byte? Training { get; set; }
    //    [JsonProperty("profiled")]
    //    public virtual byte? Profiled { get; set; }
    //    [JsonProperty("livecal")]
    //    public virtual byte? LiveCal { get; set; }
    //    [JsonProperty("createdate")]
    //    public virtual System.DateTime? CreateDate { get; set; }
    //    [JsonProperty("createby")]
    //    [StringLength(20)]
    //    public virtual string CreateBy { get; set; }
    //    [JsonProperty("lastupdate")]
    //    public virtual System.DateTime? LastUpdate { get; set; }
    //    [JsonProperty("updateby")]
    //    [StringLength(20)]
    //    public virtual string UpdateBy { get; set; }
    //    [JsonProperty("status")]
    //    public virtual byte? Status { get; set; }
    //    [JsonProperty("transvoid")]
    //    public virtual byte? TransVoid { get; set; }
    //    [JsonProperty("flag")]
    //    public virtual byte? Flag { get; set; }
    //    [JsonProperty("synccreate")]
    //    public virtual System.DateTime? SyncCreate { get; set; }
    //    [JsonProperty("synclastupd")]
    //    public virtual System.DateTime? SyncLastUpd { get; set; }
    //    [JsonProperty("tblno")]
    //    [StringLength(10)]
    //    public virtual string TblNo { get; set; }
    //    [JsonProperty("tblpax")]
    //    public virtual int? TblPax { get; set; }
    //    [JsonProperty("transpointsstatus")]
    //    public virtual byte? TransPointsStatus { get; set; }
    //    [JsonProperty("ishost")]
    //    public virtual byte? IsHost { get; set; }
    //    [JsonProperty("lastsyncby")]
    //    public virtual byte? LastSyncBy { get; set; }
    //    [JsonProperty("transpoints")]
    //    public virtual decimal? TransPoints { get; set; }
    //    [JsonProperty("synccreateby")]
    //    [StringLength(10)]
    //    public virtual string SyncCreateBy { get; set; }
    //    [JsonProperty("tid")]
    //    public virtual int? TID { get; set; }
    //}
    #endregion

    #region TRANSCDNDTL
    //[Abp.AutoMapper.AutoMapTo(typeof(DTO.Accounting.TRANSCDNDTL))]
    //public class MapTRANSCDNDTL
    //{
    //    [Required, StringLength(20)]
    //    [JsonProperty("bizlocid")]
    //    public virtual string BizLocID { get; set; }
    //    [Required, StringLength(20)]
    //    [JsonProperty("transno")]
    //    public virtual string TransNo { get; set; }
    //    [Required, Range(0, 4)]
    //    [JsonProperty("transseq")]
    //    public virtual int? TransSeq { get; set; }
    //    [Required, StringLength(20)]
    //    [JsonProperty("itemcode")]
    //    public virtual string ItemCode { get; set; }
    //    [JsonProperty("bizregid")]
    //    [StringLength(20)]
    //    public virtual string BizRegID { get; set; }
    //    [JsonProperty("termid")]
    //    public virtual int? TermID { get; set; }
    //    [JsonProperty("refseq")]
    //    public virtual int? RefSeq { get; set; }
    //    [JsonProperty("iscal")]
    //    public virtual byte? IsCal { get; set; }
    //    [JsonProperty("billno")]
    //    [StringLength(20)]
    //    public virtual string BillNo { get; set; }
    //    [JsonProperty("entrytime")]
    //    [StringLength(6)]
    //    public virtual string EntryTime { get; set; }
    //    [JsonProperty("stkcode")]
    //    [StringLength(20)]
    //    public virtual string StkCode { get; set; }
    //    [JsonProperty("stkdesc")]
    //    [StringLength(50)]
    //    public virtual string StkDesc { get; set; }
    //    [JsonProperty("stktype")]
    //    [StringLength(3)]
    //    public virtual string StkType { get; set; }
    //    [JsonProperty("behvtype")]
    //    public virtual byte? BehvType { get; set; }
    //    [JsonProperty("itemtype")]
    //    public virtual byte? ItemType { get; set; }
    //    [JsonProperty("qty")]
    //    public virtual int? Qty { get; set; }
    //    [JsonProperty("unitcost")]
    //    public virtual decimal? UnitCost { get; set; }
    //    [JsonProperty("orgprice")]
    //    public virtual decimal? OrgPrice { get; set; }
    //    [JsonProperty("nettprice")]
    //    public virtual decimal? NettPrice { get; set; }
    //    [JsonProperty("tolamt")]
    //    public virtual decimal? TolAmt { get; set; }
    //    [JsonProperty("discamt")]
    //    public virtual decimal? DiscAmt { get; set; }
    //    [JsonProperty("nettamt")]
    //    public virtual decimal? NettAmt { get; set; }
    //    [JsonProperty("subpoints")]
    //    public virtual decimal? SubPoints { get; set; }
    //    [JsonProperty("points")]
    //    public virtual decimal? Points { get; set; }
    //    [JsonProperty("priceoverrided")]
    //    public virtual byte? PriceOverrided { get; set; }
    //    [JsonProperty("discounted")]
    //    public virtual byte? Discounted { get; set; }
    //    [JsonProperty("taxable")]
    //    public virtual byte? Taxable { get; set; }
    //    [JsonProperty("returncode")]
    //    [StringLength(10)]
    //    public virtual string ReturnCode { get; set; }
    //    [JsonProperty("remark")]
    //    [StringLength(4000)]
    //    public virtual string Remark { get; set; }
    //    [JsonProperty("serialno")]
    //    [StringLength(50)]
    //    public virtual string SerialNo { get; set; }
    //    [JsonProperty("priceauthid")]
    //    [StringLength(20)]
    //    public virtual string PriceAuthID { get; set; }
    //    [JsonProperty("transvoid")]
    //    public virtual byte? TransVoid { get; set; }
    //    [JsonProperty("posted")]
    //    public virtual byte? Posted { get; set; }
    //    [JsonProperty("livecal")]
    //    public virtual byte? LiveCal { get; set; }
    //    [JsonProperty("flag")]
    //    public virtual byte? Flag { get; set; }
    //    [JsonProperty("custpkgid")]
    //    [StringLength(20)]
    //    public virtual string CustPkgID { get; set; }
    //    [JsonProperty("prcdisplaytype")]
    //    public virtual byte? PrcDisplayType { get; set; }
    //    [JsonProperty("promocode")]
    //    [StringLength(20)]
    //    public virtual string PromoCode { get; set; }
    //    [JsonProperty("createdate")]
    //    public virtual System.DateTime? CreateDate { get; set; }
    //    [JsonProperty("lastupdate")]
    //    public virtual System.DateTime? LastUpdate { get; set; }
    //    [JsonProperty("updateby")]
    //    [StringLength(20)]
    //    public virtual string UpdateBy { get; set; }
    //    [JsonProperty("status")]
    //    public virtual byte? Status { get; set; }
    //    [JsonProperty("synccreate")]
    //    public virtual System.DateTime? SyncCreate { get; set; }
    //    [JsonProperty("synclastupd")]
    //    public virtual System.DateTime? SyncLastUpd { get; set; }
    //    [JsonProperty("ishost")]
    //    public virtual byte? IsHost { get; set; }
    //    [JsonProperty("lastsyncby")]
    //    [StringLength(20)]
    //    public virtual string LastSyncBy { get; set; }
    //    [JsonProperty("synccreateby")]
    //    [StringLength(20)]
    //    public virtual string SyncCreateBy { get; set; }
    //    [JsonProperty("excode1")]
    //    [StringLength(10)]
    //    public virtual string ExCode1 { get; set; }
    //    [JsonProperty("excode2")]
    //    [StringLength(10)]
    //    public virtual string ExCode2 { get; set; }
    //    [JsonProperty("baserate")]
    //    public virtual decimal? BaseRate { get; set; }
    //    [JsonProperty("corate1h")]
    //    public virtual decimal? CoRate1H { get; set; }
    //    [JsonProperty("corate1g")]
    //    public virtual decimal? CoRate1G { get; set; }
    //    [JsonProperty("corate2h")]
    //    public virtual decimal? CoRate2H { get; set; }
    //    [JsonProperty("corate2g")]
    //    public virtual decimal? CoRate2G { get; set; }
    //    [JsonProperty("corate3h")]
    //    public virtual decimal? CoRate3H { get; set; }
    //    [JsonProperty("corate3g")]
    //    public virtual decimal? CoRate3G { get; set; }
    //    [JsonProperty("corate4h")]
    //    public virtual decimal? CoRate4H { get; set; }
    //    [JsonProperty("corate4g")]
    //    public virtual decimal? CoRate4G { get; set; }
    //    [JsonProperty("corate5h")]
    //    public virtual decimal? CoRate5H { get; set; }
    //    [JsonProperty("corate5g")]
    //    public virtual decimal? CoRate5G { get; set; }
    //    [JsonProperty("warserialno")]
    //    [StringLength(20)]
    //    public virtual string WarSerialNo { get; set; }
    //    [JsonProperty("tid")]
    //    public virtual int? TID { get; set; }
    //}
    #endregion

    #region SYSCODEB
    [Abp.AutoMapper.AutoMapTo(typeof(DTO.Accounting.SYSCODEB))]
    public class MapSYSCODEB
    {
        [Required, StringLength(10)]
        [JsonProperty("branchid")]
        public virtual string BranchID { get; set; }
        [Required, StringLength(20)]
        [JsonProperty("syscode")]
        public virtual string SysCode { get; set; }
        [JsonProperty("sysdesc")]
        [StringLength(50)]
        public virtual string SysDesc { get; set; }
        [JsonProperty("prefix")]
        [StringLength(10)]
        public virtual string Prefix { get; set; }
        [JsonProperty("spcode")]
        [StringLength(10)]
        public virtual string SpCode { get; set; }
        [JsonProperty("runno")]
        public virtual int? RunNo { get; set; }
        [JsonProperty("nolength")]
        public virtual byte? NoLength { get; set; }
        [JsonProperty("nopos")]
        public virtual byte? NoPos { get; set; }
        [JsonProperty("postfix")]
        [StringLength(10)]
        public virtual string Postfix { get; set; }
        [JsonProperty("lastupdate")]
        public virtual System.DateTime? LastUpdate { get; set; }
        [JsonProperty("updateby")]
        [StringLength(20)]
        public virtual string UpdateBy { get; set; }
        [JsonProperty("status")]
        public virtual byte? Status { get; set; }
        [JsonProperty("sysid")]
        public virtual byte? SysID { get; set; }
        [JsonProperty("checkformat")]
        [StringLength(20)]
        public virtual string CheckFormat { get; set; }
        [JsonProperty("synccreate")]
        public virtual System.DateTime? SyncCreate { get; set; }
        [JsonProperty("synclastupd")]
        public virtual System.DateTime? SyncLastUpd { get; set; }
        [JsonProperty("ishost")]
        public virtual byte? IsHost { get; set; }
        [JsonProperty("lastsyncby")]
        [StringLength(10)]
        public virtual string LastSyncBy { get; set; }
    }
    #endregion

    #endregion

    public class FPXPayment
    {
        public MapTRANSHDR TransHdr { get; set; }
        public List<MapTRANSDTL> TransDtl { get; set; }
        public MapTRANSFPX TransFpx { get; set; }
        public string InvoiceID { get; set; }
    }

    public class Payment
    {
        public MapTRANSHDR TransHdr { get; set; }
        public List<MapTRANSDTL> TransDtl { get; set; }
        public string SiNumber { get; set; }
        public string TenderID { get; set; }
        public string BankCode { get; set; }
        public string RequestMessage { get; set; }
        //public List<dynamic> TransDtlHdr { get; set; }
        public List<dynamic> OfflineInfoList { get; set; }
        public byte Mode { get; set; }
    }

    public class FPXResponse
    {
        public string fpx_buyerBankBranch { get; set; }
        public string fpx_buyerBankId { get; set; }
        public string fpx_buyerIban { get; set; }
        public string fpx_buyerId { get; set; }
        public string fpx_buyerName { get; set; }
        public string fpx_creditAuthCode { get; set; }
        public string fpx_creditAuthNo { get; set; }
        public string fpx_debitAuthCode { get; set; }
        public string fpx_debitAuthNo { get; set; }
        public string fpx_fpxTxnId { get; set; }
        public string fpx_fpxTxnTime { get; set; }
        public string fpx_makerName { get; set; }
        public string fpx_msgToken { get; set; }
        public string fpx_msgType { get; set; }
        public string fpx_sellerExId { get; set; }
        public string fpx_sellerExOrderNo { get; set; }
        public string fpx_sellerId { get; set; }
        public string fpx_sellerOrderNo { get; set; }
        public string fpx_sellerTxnTime { get; set; }
        public string fpx_txnAmount { get; set; }
        public string fpx_txnCurrency { get; set; }
        public string fpx_checkSum { get; set; }
        public string fpx_checkSumString { get; set; }
    }

    public class StructTrans
    {
        public string Prefix { get; set; }
        public string SpCode { get; set; }
        public int Length { get; set; }
        public Int16 NoPos { get; set; }
        public int Runno { get; set; }
        public string PostFix { get; set; }
    }

    public class FPXSender
    {
        public string fpx_msgType { get; set; }
        public string fpx_msgToken { get; set; }
        public string fpx_sellerExId { get; set; }
        public string fpx_sellerExOrderNo { get; set; }
        public string fpx_sellerOrderNo { get; set; }
        public string fpx_sellerTxnTime { get; set; }
        public string fpx_sellerId { get; set; }
        public string fpx_sellerBankCode { get; set; }
        public string fpx_txnCurrency { get; set; }
        public string fpx_txnAmount { get; set; }
        public string fpx_buyerEmail { get; set; }
        public string fpx_buyerId { get; set; }
        public string fpx_buyerName { get; set; }
        public string fpx_buyerAccNo { get; set; }
        public string fpx_makerName { get; set; }
        public string fpx_buyerIban { get; set; }
        public string fpx_productDesc { get; set; }
        public string fpx_version { get; set; }
    }

    public class MBCCResponse
    {
        public string TRANSACTION_ID { get; set; }
        public string TXN_STATUS { get; set; }
        public string MERCHANT_ACC_NO { get; set; }
        public string TXN_SIGNATURE { get; set; }
        public string TXN_SIGNATURE2 { get; set; }
        public string TRAN_DATE { get; set; }
        public string MERCHANT_TRANID { get; set; }
        public string RESPONSE_CODE { get; set; }
        public string RESPONSE_DESC { get; set; }
        public string CUSTOMER_ID { get; set; }
        public string AUTH_ID { get; set; }
        public string AUTH_DATE { get; set; }
        public string FR_LEVEL { get; set; }
        public string HOST_RESPONSE_CODE { get; set; }
        public string HOST_RESPONSE_DESC { get; set; }
    }

    #region TENDERTYPE
    [Abp.AutoMapper.AutoMapTo(typeof(DTO.Accounting.TENDERTYPE))]
    public class MapTENDERTYPE
    {
        [Required,]
        [JsonProperty("tendertype")]
        public virtual byte TenderType { get; set; }
        [JsonProperty("tendertypedesc")]
        [Required, StringLength(20)]
        public virtual string TenderTypeDesc { get; set; }
        [JsonProperty("lastupdate")]
        public virtual System.DateTime? LastUpdate { get; set; }
        [JsonProperty("active")]
        [Required]
        public virtual byte Active { get; set; }
        [JsonProperty("rowguid")]
        [Required, MaxLength(16)]
        public virtual System.Guid rowguid { get; set; }
        [JsonProperty("synccreate")]
        [Required]
        public virtual System.DateTime SyncCreate { get; set; }
        [JsonProperty("synclastupd")]
        [Required]
        public virtual System.DateTime SyncLastUpd { get; set; }
        [JsonProperty("ishost")]
        [Required]
        public virtual byte IsHost { get; set; }
    }
    #endregion

    #region PROCTASK
    [Abp.AutoMapper.AutoMapTo(typeof(DTO.Accounting.PROCTASK))]
    public class MapPROCTASK
    {
        [Required, StringLength(20)]
        [JsonProperty("prosegid")]
        public virtual string ProSegID { get; set; }
        [Required, Range(0, 8)]
        [JsonProperty("taskid")]
        public virtual long? TaskID { get; set; }
        [Required, StringLength(20)]
        [JsonProperty("agentid")]
        public virtual string AgentID { get; set; }
        [Required,]
        [JsonProperty("tasktype")]
        public virtual byte? TaskType { get; set; }
        [Required, StringLength(3)]
        [JsonProperty("subtype")]
        public virtual string SubType { get; set; }
        [Required]
        [JsonProperty("taskstartdate")]
        public virtual System.DateTime? TaskStartDate { get; set; }
        [JsonProperty("taskenddate")]
        public virtual System.DateTime? TaskEndDate { get; set; }
        [JsonProperty("taskvalue1")]
        [StringLength(4000)]
        public virtual string TaskValue1 { get; set; }
        [JsonProperty("taskvalue2")]
        [StringLength(4000)]
        public virtual string TaskValue2 { get; set; }
        [JsonProperty("status")]
        public virtual byte? Status { get; set; }
        [JsonProperty("result")]
        [StringLength(4000)]
        public virtual string Result { get; set; }
        [JsonProperty("batchno")]
        [StringLength(50)]
        public virtual string BatchNo { get; set; }
        [JsonProperty("msgid")]
        [StringLength(200)]
        public virtual string MsgID { get; set; }
        [JsonProperty("transid")]
        [StringLength(20)]
        public virtual string TransID { get; set; }
        [JsonProperty("recordlocator")]
        [StringLength(20)]
        public virtual string RecordLocator { get; set; }
        [JsonProperty("queuecode")]
        [StringLength(50)]
        public virtual string QueueCode { get; set; }
        [JsonProperty("createddate")]
        public virtual System.DateTime? CreatedDate { get; set; }
        [JsonProperty("emailtype")]
        public virtual byte? EmailType { get; set; }
        [JsonProperty("emailaddress")]
        [StringLength(266)]
        public virtual string EmailAddress { get; set; }
        [JsonProperty("expirydate")]
        public virtual System.DateTime? ExpiryDate { get; set; }
        [JsonProperty("currency")]
        [StringLength(3)]
        public virtual string Currency { get; set; }
        [JsonProperty("balancedue")]
        public virtual decimal? BalanceDue { get; set; }
        [JsonProperty("paymentamt")]
        public virtual decimal? PaymentAmt { get; set; }
        [JsonProperty("transtotalamt")]
        public virtual decimal? TransTotalAmt { get; set; }
        [JsonProperty("attemptcountsender")]
        public virtual byte? AttemptCountSender { get; set; }
        [JsonProperty("attemptcountsenderdate")]
        public virtual System.DateTime? AttemptCountSenderDate { get; set; }
        [JsonProperty("issuccess")]
        public virtual byte? IsSuccess { get; set; }
        [JsonProperty("finisheddate")]
        public virtual System.DateTime? FinishedDate { get; set; }
        [JsonProperty("failedremark")]
        [StringLength(255)]
        public virtual string FailedRemark { get; set; }
        [JsonProperty("approvedby")]
        [StringLength(100)]
        public virtual string ApprovedBy { get; set; }
        [JsonProperty("approveddate")]
        public virtual System.DateTime? ApprovedDate { get; set; }
        [JsonProperty("updatedby")]
        [StringLength(100)]
        public virtual string UpdatedBy { get; set; }
        [JsonProperty("updateddate")]
        public virtual System.DateTime? UpdatedDate { get; set; }
        [JsonProperty("isdeleted")]
        public virtual byte? IsDeleted { get; set; }
        [JsonProperty("synccreate")]
        public virtual System.DateTime? SyncCreate { get; set; }
        [JsonProperty("synclastupd")]
        public virtual System.DateTime? SyncLastUpd { get; set; }
        [JsonProperty("flag")]
        public virtual byte? Flag { get; set; }
    }
    #endregion

    public class MapSuspendHistory
    {
        [StringLength(20)]
        [JsonProperty("transporterID")]
        [Grid(Width = 20)]
        public virtual string TransporterID { get; set; }

        [StringLength(200)]
        [JsonProperty("transporter")]
        [Grid(Width = 200)]
        public virtual string Transporter { get; set; }

        [JsonProperty("taskType")]
        public virtual int TaskType { get; set; }

        [StringLength(100)]
        [JsonProperty("task")]
        [Grid(Width = 100)]
        public virtual string Task { get; set; }

        [JsonProperty("taskDate")]
        public virtual System.DateTime TaskDate { get; set; }

        [StringLength(20)]
        [JsonProperty("customerID")]
        public virtual string CustomerID { get; set; }

        [StringLength(200)]
        [JsonProperty("customer")]
        [Grid(Width = 200)]
        public virtual string Customer { get; set; }
    }

    public class MapTransactionReport
    {
        [JsonProperty("accountcode")]
        public virtual string AccountCode { get; set; }

        [JsonProperty("exported")]
        public virtual string Exported { get; set; }

        [JsonProperty("transstatus")]
        public virtual string TransStatus { get; set; }

        [JsonProperty("companyname")]
        public virtual string CompanyName { get; set; }

        [JsonProperty("transdate")]
        public virtual string TransDate { get; set; }

        [JsonProperty("transno")]
        public virtual string TransNo { get; set; }

        [JsonProperty("invoiceno")]
        public virtual string InvoiceNo { get; set; }

        [JsonProperty("paymentmethod")]
        public virtual string PaymentMethod { get; set; }

        [JsonProperty("amount")]
        public virtual decimal? Amount { get; set; }
    }

    #region INVOICEHDR_OLD
    [Abp.AutoMapper.AutoMapTo(typeof(DTO.Accounting.INVOICEHDR_OLD))]
    public class MapINVOICEHDR_OLD : BaseMapId
    {
        [JsonProperty("monthcode")]
        [MaxLength(10), Required]
        public virtual string MonthCode { get; set; }
        [JsonProperty("pbtcode")]
        [MaxLength(20), Required]
        public virtual string PBTCode { get; set; }
        [JsonProperty("batchcode")]
        [MaxLength(20), Required]
        public virtual string BatchCode { get; set; }
        [JsonProperty("totalrec1")]
        [Range(0, 9999)]
        public virtual int? TotalRec1 { get; set; }
        [JsonProperty("totalrec2")]
        [Range(0, 9999)]
        public virtual int? TotalRec2 { get; set; }
        [JsonProperty("totalrec3")]
        [Range(0, 9999)]
        public virtual int? TotalRec3 { get; set; }
        [JsonProperty("totalrec4")]
        [Range(0, 9999)]
        public virtual int? TotalRec4 { get; set; }
        [JsonProperty("totalrec5")]
        [Range(0, 9999)]
        public virtual int? TotalRec5 { get; set; }
        [JsonProperty("totalamt1")]
        public virtual decimal? TotalAmt1 { get; set; }
        [JsonProperty("totalamt2")]
        public virtual decimal? TotalAmt2 { get; set; }
        [JsonProperty("totalamt3")]
        public virtual decimal? TotalAmt3 { get; set; }
        [JsonProperty("totalamt4")]
        public virtual decimal? TotalAmt4 { get; set; }
        [JsonProperty("totalamt5")]
        public virtual decimal? TotalAmt5 { get; set; }
        [JsonProperty("status")]
        public virtual byte? Status { get; set; }
    }
    #endregion

    #region INVOICEITEM
    [Abp.AutoMapper.AutoMapTo(typeof(DTO.Accounting.INVOICEITEM))]
    public class MapINVOICEITEM : BaseMapId
    {
        [JsonProperty("monthcode")]
        [MaxLength(10), Required]
        public virtual string MonthCode { get; set; }
        [JsonProperty("pbtcode")]
        [MaxLength(20), Required]
        public virtual string PBTCode { get; set; }
        [JsonProperty("bizregid")]
        [MaxLength(20), Required]
        public virtual string BizRegID { get; set; }
        [JsonProperty("batchcode")]
        [MaxLength(20), Required]
        public virtual string BatchCode { get; set; }
        [JsonProperty("seqno")]
        [Range(0, 9999), Required]
        public virtual int? SeqNo { get; set; }
        [JsonProperty("companyname")]
        [MaxLength(200)]
        public virtual string CompanyName { get; set; }
        [JsonProperty("contractid")]
        [MaxLength(200)]
        public virtual string ContractID { get; set; }
        [JsonProperty("invdate")]
        public virtual System.DateTime? InvDate { get; set; }
        [JsonProperty("expirydate")]
        public virtual System.DateTime? ExpiryDate { get; set; }
        [JsonProperty("itemtype")]
        [MaxLength(3)]        
        public virtual string ItemType { get; set; }
        [JsonProperty("itemcode")]
        [MaxLength(20)]
        public virtual string ItemCode { get; set; }
        [JsonProperty("itemdesc")]
        [MaxLength(4000)]
        public virtual string ItemDesc { get; set; }
        [JsonProperty("amount1")]
        public virtual decimal? Amount1 { get; set; }
        [JsonProperty("amount2")]
        public virtual decimal? Amount2 { get; set; }
        [JsonProperty("remark1")]
        [MaxLength(4000)]
        public virtual string Remark1 { get; set; }
        [JsonProperty("remark2")]
        [MaxLength(4000)]
        public virtual string Remark2 { get; set; }
        [JsonProperty("svctype")]
        [MaxLength(10)]
        public virtual string SvcType { get; set; }
        [JsonProperty("proctype")]
        [MaxLength(10)]
        public virtual string ProcType { get; set; }
        [JsonProperty("errcode")]
        [MaxLength(200)]
        public virtual string ErrCode { get; set; }
        [JsonProperty("currency")]
        [MaxLength(3)]
        public virtual string Currency { get; set; }
        [JsonProperty("filepath")]
        [MaxLength(4000)]
        public virtual string FilePath { get; set; }
        [JsonProperty("lastupdateby")]
        [MaxLength(20)]
        public virtual string LastUpdateBy { get; set; }
        [JsonProperty("status")]
        public virtual byte? Status { get; set; }
    }
    #endregion

    #region Map InvoiceQ
    public class MapInvoiceQList
    {
        [JsonProperty("invoiceno")]
        [Grid(Width = 65)]
        public virtual string InvoiceNo { get; set; }

        [JsonProperty("swttano")]
        public virtual string SWTTANo { get; set; }

        [JsonProperty("company")]
        public virtual string Company { get; set; }

        [JsonProperty("date")]
        [Grid(Width = 75, DatetimeFormat = "DD/MM/YYYY")]
        public virtual System.DateTime issuedDate { get; set; }

        [JsonProperty("duedate")]
        [Grid(Width = 58, DatetimeFormat = "DD/MM/YYYY")]
        public virtual System.DateTime DueDate { get; set; }

        [JsonProperty("invoicestatus")]
        [Grid(Width = 82, IsVisible = true, Alignment = ColumnsAlignment.Center, StatusBadges = new string[] { "Paid|paid", "Open|open", "In Progress|in-progress", "Overdue|overdue" })]
        public virtual string InvoiceStatus { get; set; }

        [JsonProperty("invoiceaging")]
        [Grid(Width = 75, Alignment = ColumnsAlignment.Right)]
        public virtual string InvoiceAging { get; set; }

        [JsonProperty("fpxid")]
        public virtual string FPXId { get; set; }

        [JsonProperty("amount")]
        [Grid(Width = 85, HasCurrency = true)]
        public virtual decimal? Amount { get; set; }

        [JsonProperty("bank")]
        public virtual string Bank { get; set; }

        [JsonProperty("status")]
        public virtual string Status { get; set; }

        [JsonProperty("totalinvoice")]
        public virtual decimal? TotalInvoice { get; set; }

        [JsonProperty("totaloverdue")]
        public virtual decimal? TotalOverdue { get; set; }

        [JsonProperty("totaloverdueamount")]
        public virtual decimal? TotalOverdueAmount { get; set; }

        [JsonProperty("overduepercent")]
        public virtual decimal? OverduePercent { get; set; }

        [JsonProperty("totalamount")]
        public virtual decimal? TotalAmount { get; set; }

        [JsonProperty("createdate")]
        public virtual System.DateTime CreateDate { get; set; }

        [JsonProperty("PathRef")]
        public virtual string PathRef { get; set; }

        [JsonProperty("ServerPath")]
        public virtual string ServerPath { get; set; }
    }
    #endregion

    #region map FPX old sync
    public class MapCheckFpxList
    {

        [JsonProperty("fpx_debitAuthCode")]

        public virtual string fpx_debitAuthCode { get; set; }

        [JsonProperty("fpx_debitAuthCodeDesc")]
        public virtual string fpx_debitAuthCodeDesc { get; set; }

        [JsonProperty("FpxBizRegID")]
        public virtual string FpxBizRegID { get; set; }

        [JsonProperty("FpxBizLocID")]
        public virtual string FpxBizLocID { get; set; }
        [JsonProperty("FpxCustName")]
        public virtual string FpxCustName { get; set; }
        [JsonProperty("FpxTransNo")]
        public virtual string FpxTransNo { get; set; }
        [JsonProperty("FpxTransDate")]
        public virtual System.DateTime FpxTransDate { get; set; }
        [JsonProperty("FpxAmount")]
        public virtual decimal? FpxAmount { get; set; }

        [JsonProperty("FpxCCrefInfo")]
        public virtual string FpxCCrefInfo { get; set; }
        [JsonProperty("InvcBatchCodes")]
        public virtual string InvcBatchCodes { get; set; }
        [JsonProperty("InvcStatus")]
        public virtual string InvcStatus { get; set; }
        [JsonProperty("InvcInuse")]
        public virtual string InvcInuse { get; set; }
        [JsonProperty("InvcTotalAmount")]
        public virtual decimal? InvcTotalAmount { get; set; }


    }

    public class CheckFpxFailedResponse
    {
        [JsonProperty("totalSync")]
        public int totalSync { get; set; }

        [JsonProperty("SuccessfulySync")]
        public int SuccessfulySync { get; set; }

        [JsonProperty("FailedSync")]
        public int FailedSync { get; set; }

        [JsonProperty("duration")]
        public string duration { get; set; }

        [JsonProperty("SuccessTransNo")]
        public string SuccessTransNo { get; set; }

        [JsonProperty("failTransno")]
        public string failTransno { get; set; }
    }

    #endregion
}
