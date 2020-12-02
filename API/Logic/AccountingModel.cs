using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using LOGIC.Base;
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using LOGIC.Shared.Collection;

namespace Plexform.Base.Accounting.Model
{
    #region XML
    #region REQUEST
    public class AuthenticationDTO
    {
        [JsonProperty("password")]
        public virtual string Password { get; set; }
        [JsonProperty("username")]
        public virtual string Username { get; set; }
    }

    public class GetProformaBillById
    {
        [JsonProperty("authenticationdto")]
        public virtual AuthenticationDTO AuthenticationDTO { get; set; }
        [JsonProperty("billid")]
        public virtual string BillId { get; set; }
    }

    public class GetCreditBillById
    {
        [JsonProperty("authenticationdto")]
        public AuthenticationDTO AuthenticationDTO { get; set; }
        [JsonProperty("billid")]
        public virtual string BillId { get; set; }
    }

    public class Config
    {
        [JsonProperty("customercode")]
        public virtual string CustomerCode { get; set; }
        [JsonProperty("limit")]
        public virtual string Limit { get; set; }
        [JsonProperty("offset")]
        public virtual string Offset { get; set; }
    }

    public class GetAllOutStandingBillByCustomerCode
    {
        [JsonProperty("authenticationdto")]
        public virtual AuthenticationDTO AuthenticationDTO { get; set; }
        [JsonProperty("config")]
        public virtual Config Config { get; set; }
    }
    #endregion

    #region RESPONSE
    public class TaxCategoryDTO
    {
        [JsonProperty("code")]
        public virtual string Code { get; set; }
        [JsonProperty("id")]
        public virtual string Id { get; set; }
        [JsonProperty("name")]
        public virtual string Name { get; set; }
        [JsonProperty("value")]
        public virtual string Value { get; set; }
        [JsonProperty("type")]
        public virtual string Type { get; set; }
    }

    public class CreditVoteDTO
    {
        [JsonProperty("code")]
        public virtual string Code { get; set; }
        [JsonProperty("id")]
        public virtual string Id { get; set; }
        [JsonProperty("taxcategorydto")]
        public virtual TaxCategoryDTO TaxCategoryDTO { get; set; }
        [JsonProperty("type")]
        public virtual string Type { get; set; }
    }

    public class CurrencyDTO
    {
        [JsonProperty("name")]
        public virtual string Name { get; set; }
        [JsonProperty("rate")]
        public virtual string Rate { get; set; }
        [JsonProperty("symbol")]
        public virtual string Symbol { get; set; }
        [JsonProperty("type")]
        public virtual string Type { get; set; }
    }

    public class CountDTO
    {
        [JsonProperty("balance")]
        public virtual string Balance { get; set; }
        [JsonProperty("countless30")]
        public virtual string CountLess30 { get; set; }
        [JsonProperty("countless60")]
        public virtual string CountLess60 { get; set; }
        [JsonProperty("countmore60")]
        public virtual string CountMore60 { get; set; }
        [JsonProperty("type")]
        public virtual string Type { get; set; }
    }

    public class CustomerDTO
    {
        [JsonProperty("address")]
        public virtual string Address { get; set; }
        [JsonProperty("balance")]
        public virtual string Balance { get; set; }
        [JsonProperty("code")]
        public virtual string Code { get; set; }
        [JsonProperty("creditlimit")]
        public virtual string CreditLimit { get; set; }
        [JsonProperty("pmtday")]
        public virtual string PmtDay { get; set; }
        [JsonProperty("countdto")]
        public virtual CountDTO CountDTO { get; set; }
        [JsonProperty("name")]
        public virtual string Name { get; set; }
        [JsonProperty("sundb")]
        public virtual string SunDb { get; set; }
        [JsonProperty("type")]
        public virtual string Type { get; set; }
        [JsonProperty("totalbill")]
        public virtual string TotalBill { get; set; }
    }

    public class DatabaseDTO
    {
        [JsonProperty("id")]
        public virtual string Id { get; set; }
        [JsonProperty("name")]
        public virtual string Name { get; set; }
        [JsonProperty("needproject")]
        public virtual string NeedProject { get; set; }
        [JsonProperty("type")]
        public virtual string Type { get; set; }
    }

    public class DeliveryReference
    {
        [JsonProperty("nil")]
        public virtual string Nil { get; set; }
    }

    public class Description
    {
        [JsonProperty("nil")]
        public virtual string Nil { get; set; }
    }

    public class FileUrl
    {
        [JsonProperty("nil")]
        public virtual string Nil { get; set; }
    }

    public class Files
    {
        [JsonProperty("description")]
        public virtual Description Description { get; set; }
        [JsonProperty("file")]
        public virtual string File { get; set; }
        [JsonProperty("fileurl")]
        public virtual FileUrl FileUrl { get; set; }
        [JsonProperty("id")]
        public virtual string Id { get; set; }
        [JsonProperty("type")]
        public virtual string Type { get; set; }
    }

    public class Items
    {
        [JsonProperty("amount")]
        public virtual string Amount { get; set; }
        [JsonProperty("code")]
        public virtual string Code { get; set; }
        [JsonProperty("description")]
        public virtual string Description { get; set; }
        [JsonProperty("id")]
        public virtual string Id { get; set; }
        [JsonProperty("quantity")]
        public virtual string Quantity { get; set; }
        [JsonProperty("unitprice")]
        public virtual string UnitPrice { get; set; }
        [JsonProperty("type")]
        public virtual string Type { get; set; }
    }

    public class OfficerDTO
    {
        [JsonProperty("id")]
        public virtual string Id { get; set; }
        [JsonProperty("name")]
        public virtual string Name { get; set; }
        [JsonProperty("type")]
        public virtual string Type { get; set; }
    }

    public class PoNumber
    {
        [JsonProperty("nil")]
        public virtual string Nil { get; set; }
    }

    public class Name
    {
        [JsonProperty("nil")]
        public virtual string Nil { get; set; }
    }

    public class UnitcmbDTO
    {
        [JsonProperty("nil")]
        public virtual string Nil { get; set; }
    }

    public class ProjectDTO
    {
        [JsonProperty("code")]
        public virtual string Code { get; set; }
        [JsonProperty("id")]
        public virtual string Id { get; set; }
        [JsonProperty("name")]
        public virtual Name Name { get; set; }
        [JsonProperty("unitcmbdto")]
        public virtual UnitcmbDTO UnitcmbDTO { get; set; }
        [JsonProperty("type")]
        public virtual string Type { get; set; }
    }

    public class UnitDTO
    {
        [JsonProperty("code")]
        public virtual string Code { get; set; }
        [JsonProperty("id")]
        public virtual string Id { get; set; }
        [JsonProperty("option")]
        public virtual string Option { get; set; }
        [JsonProperty("type")]
        public virtual string Type { get; set; }
    }

    public class Return
    {
        [JsonProperty("attention")]
        public virtual string Attention { get; set; }
        [JsonProperty("billno")]
        public virtual string BillNo { get; set; }
        [JsonProperty("creditvotedto")]
        public virtual CreditVoteDTO CreditVoteDTO { get; set; }
        [JsonProperty("currencydto")]
        public virtual CurrencyDTO CurrencyDTO { get; set; }
        [JsonProperty("customerdto")]
        public virtual CustomerDTO CustomerDTO { get; set; }
        [JsonProperty("customerreference")]
        public virtual string CustomerReference { get; set; }
        [JsonProperty("databasedto")]
        public virtual DatabaseDTO DatabaseDTO { get; set; }
        [JsonProperty("datereceived")]
        public virtual string DateReceived { get; set; }
        [JsonProperty("dateregistered")]
        public virtual string DateRegistered { get; set; }
        [JsonProperty("deliveryreference")]
        public virtual DeliveryReference Deliveryreference { get; set; }
        [JsonProperty("divisionreference")]
        public virtual string DivisionReference { get; set; }
        [JsonProperty("files")]
        public virtual Files Files { get; set; }
        [JsonProperty("id")]
        public virtual string Id { get; set; }
        [JsonProperty("items")]
        public virtual List<Items> Items { get; set; }
        [JsonProperty("labreference")]
        public virtual string LabReference { get; set; }
        [JsonProperty("officerdto")]
        public virtual OfficerDTO OfficerDTO { get; set; }
        [JsonProperty("ponumber")]
        public virtual PoNumber PoNumber { get; set; }
        [JsonProperty("projectdto")]
        public virtual ProjectDTO ProjectDTO { get; set; }
        [JsonProperty("unitdto")]
        public virtual UnitDTO UnitDTO { get; set; }
        [JsonProperty("type")]
        public virtual string Type { get; set; }
        [JsonProperty("ax21")]
        public virtual string Ax21 { get; set; }
        [JsonProperty("ax23")]
        public virtual string Ax23 { get; set; }
        [JsonProperty("xsi")]
        public virtual string Xsi { get; set; }
    }

    public class ReturnCreditGetCreditBillByIdResponse
    {
        [JsonProperty("attention")]
        public virtual string Attention { get; set; }
        [JsonProperty("balancelimit")]
        public virtual string BalanceLimit { get; set; }
        [JsonProperty("billNo")]
        public virtual string BillNo { get; set; }
        [JsonProperty("creditlimit")]
        public virtual string CreditLimit { get; set; }
        [JsonProperty("creditvotedto")]
        public virtual CreditVoteDTO CreditVoteDTO { get; set; }
        [JsonProperty("customerdto")]
        public virtual CustomerDTO CustomerDTO { get; set; }
        [JsonProperty("customerreference")]
        public virtual string CustomerReference { get; set; }
        [JsonProperty("databasedto")]
        public virtual DatabaseDTO DatabaseDTO { get; set; }
        [JsonProperty("datereceived")]
        public virtual string DateReceived { get; set; }
        [JsonProperty("dateregistered")]
        public virtual string DateRegistered { get; set; }
        [JsonProperty("deliveryreference")]
        public virtual DeliveryReference Deliveryreference { get; set; }
        [JsonProperty("divisionreference")]
        public virtual string DivisionReference { get; set; }
        [JsonProperty("id")]
        public virtual string Id { get; set; }
        [JsonProperty("items")]
        public virtual List<Items> Items { get; set; }
        [JsonProperty("labreference")]
        public virtual string LabReference { get; set; }
        [JsonProperty("officerdto")]
        public virtual OfficerDTO OfficerDTO { get; set; }
        [JsonProperty("projectdto")]
        public virtual ProjectDTO ProjectDTO { get; set; }
        [JsonProperty("totalbill")]
        public virtual string TotalBill { get; set; }
        [JsonProperty("unitdto")]
        public virtual UnitDTO UnitDTO { get; set; }
        [JsonProperty("type")]
        public virtual string Type { get; set; }
        [JsonProperty("ax21")]
        public virtual string Ax21 { get; set; }
        [JsonProperty("ax23")]
        public virtual string Ax23 { get; set; }
        [JsonProperty("xsi")]
        public virtual string Xsi { get; set; }
    }

    public class GetProformaBillByIdResponse
    {
        [JsonProperty("return")]
        public virtual Return Return { get; set; }
    }

    public class GetCreditBillByIdResponse
    {
        [JsonProperty("return")]
        public virtual ReturnCreditGetCreditBillByIdResponse Return { get; set; }
    }

    public class Data
    {
        [JsonProperty("amount")]
        public virtual string Amount { get; set; }
        [JsonProperty("balance")]
        public virtual string Balance { get; set; }
        [JsonProperty("billno")]
        public virtual string BillNo { get; set; }
        [JsonProperty("company")]
        public virtual string Company { get; set; }
        [JsonProperty("date")]
        public virtual string Date { get; set; }
        [JsonProperty("detail")]
        public virtual string Detail { get; set; }
        [JsonProperty("gst")]
        public virtual string Gst { get; set; }
        [JsonProperty("id")]
        public virtual string Id { get; set; }
        [JsonProperty("rounding")]
        public virtual string Rounding { get; set; }
        [JsonProperty("sinumber")]
        public virtual string SiNumber { get; set; }
        [JsonProperty("status")]
        public virtual string Status { get; set; }
        [JsonProperty("type")]
        public virtual string Type { get; set; }
    }

    public class ReturnGetAllOutStandingBillByCustomerCodeResponse
    {
        [JsonProperty("data")]
        public virtual List<Data> Data { get; set; }
        [JsonProperty("offset")]
        public virtual string Offset { get; set; }
        [JsonProperty("totallength")]
        public virtual string TotalLength { get; set; }
        [JsonProperty("type")]
        public virtual string Type { get; set; }
        [JsonProperty("ax21")]
        public virtual string Ax21 { get; set; }
        [JsonProperty("ax23")]
        public virtual string Ax23 { get; set; }
        [JsonProperty("xsi")]
        public virtual string Xsi { get; set; }
    }

    public class GetAllOutStandingBillByCustomerCodeResponse
    {
        [JsonProperty("returngetalloutstandingbillbycustomercoderesponse")]
        public virtual ReturnGetAllOutStandingBillByCustomerCodeResponse ReturnGetAllOutStandingBillByCustomerCodeResponse { get; set; }
    }

    public class Fault
    {
        [JsonProperty("faultcode")]
        public virtual string FaultCode { get; set; }
        [JsonProperty("faultstring")]
        public virtual string FaultString { get; set; }
        [JsonProperty("detail")]
        public virtual string Detail { get; set; }
    }
    #endregion RESPONSE


    public class Body
    {
        [JsonProperty("getproformabillbyid")]
        public virtual GetProformaBillById GetProformaBillById { get; set; }

        [JsonProperty("getproformabillbyidresponse")]
        public virtual GetProformaBillByIdResponse GetProformaBillByIdResponse { get; set; }

        [JsonProperty("getcreditbillbyid")]
        public virtual GetCreditBillById GetCreditBillById { get; set; }

        [JsonProperty("getcreditbillbyidresponse")]
        public virtual GetCreditBillByIdResponse GetCreditBillByIdResponse { get; set; }

        [JsonProperty("getalloutstandingbillbycustomercode")]
        public virtual GetAllOutStandingBillByCustomerCode GetAllOutStandingBillByCustomerCode { get; set; }

        [JsonProperty("getalloutstandingbillbycustomercoderesponse")]
        public virtual GetAllOutStandingBillByCustomerCodeResponse GetAllOutStandingBillByCustomerCodeResponse { get; set; }

        [JsonProperty("fault")]
        public virtual Fault Fault { get; set; }
    }

    //[Abp.AutoMapper.AutoMapTo(typeof(LOGIC.Account.AccountManager.Envelope))]
    public class Envelope
    {
        [JsonProperty("body")]
        public virtual Body Body { get; set; }
        [JsonProperty("soapenv")]
        public virtual string Soapenv { get; set; }
    }
    #endregion

    public class SuspendCustomerRequest
    {
        public string CustomerID { get; set; }
        public int Status { get; set; }
        public string UserID { get; set; }
        public string TransporterID { get; set; }
    }

    public class GetInvoiceParameter : GetListParameter
    {
        public string bizregid { get; set; }
        public string type { get; set; }
        public string transNo { get; set; }
        public string status { get; set; }
    }

    public class InvoiceFilter : GetListParameter
    {
        //public string invoiceId { get; set; }
        public int? status { get; set; }
        public string customerCode { get; set; }
        public string dateStart { get; set; }
        public string dateEnd { get; set; }
        public string invoiceNo { get; set; }
        public string filter { get; set; }
        //public string company { get; set; }
        //public string billType { get; set; }
        //public string dateStart { get; set; }
        //public string dateEnd { get; set; }
        //public byte status { get; set; }
        //public byte mode { get; set; }
    }

}
