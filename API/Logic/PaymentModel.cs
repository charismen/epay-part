using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using LOGIC.Base;
using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Plexform.Base.Accounting.Payment.Model
{
    public class AddPaymentModel
    {
        public string MerchantCode { get; set; }
        public string MerchantTransNo { get; set; }
        public string TransCurrency { get; set; }
        public string TransAmt { get; set; }
        public string ProductDesc { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserContact { get; set; }
        public string Remark { get; set; }
        public string Signature { get; set; }
        public string ResponseURL { get; set; }
        public string AdditionalURL { get; set; }
    }

	//public class AddPayment
	//{
	//    public Payment.Map.MapPAYMENTHDR PaymentHDR { get; set; }
	//    public List<Payment.Map.MapPAYMENTHDR> PaymentDTL { get; set; }
	//    public Payment.Map.MapPAYMENTTENDER PaymentTender { get; set; }
	//    public Payment.Map.MapPAYMENTLOG PaymentLog { get; set; }
	//}

	public class AddTenderModel
	{
		public List<Payment.Map.MapPAYMENTTENDER> PaymentTender { get; set; }
		public List<Payment.Map.MapPAYMENTLOG> PaymentLog { get; set; }
	}


	#region FPX
	public class FPXBodyModel
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

	public class FPXBankListModel
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

	public class FPXResponseModel
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
	#endregion

	public class ValidateStatusModel
	{
		public string DebitAuthCode { get; set; }
		public string Message { get; set; }
		public string CheckSum { get; set; }
	}

    #region MayBank Credit Card
    public class MayBankCCResponseModel
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
    #endregion
}