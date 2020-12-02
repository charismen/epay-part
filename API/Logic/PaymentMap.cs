using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using LOGIC.Base;
using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Plexform.Base.Accounting.Payment.Map
{
	#region PAYMENTHDR
	[Abp.AutoMapper.AutoMapTo(typeof(DTO.Payment.PAYMENTHDR))]
	public class MapPAYMENTHDR : BaseMapId
	{
		[JsonProperty("bizregid")]
		[MaxLength(20), Required]
		public virtual string BizRegID { get; set; }
		[JsonProperty("bizlocid")]
		[MaxLength(20), Required]
		public virtual string BizLocID { get; set; }
		[JsonProperty("paymenttransid")]
		[MaxLength(20), Required]
		public virtual string PaymentTransID { get; set; }
		[JsonProperty("paymentref")]
		[MaxLength(20), Required]
		public virtual string PaymentRef { get; set; }
		[JsonProperty("merchantcode")]
		[MaxLength(20), Required]
		public virtual string MerchantCode { get; set; }
		[JsonProperty("basecurrency")]
		[MaxLength(3)]
		public virtual string BaseCurrency { get; set; }
		[JsonProperty("transcurrency")]
		[MaxLength(3)]
		public virtual string TransCurrency { get; set; }
		[JsonProperty("transtotalamt")]
		public virtual decimal? TransTotalAmt { get; set; }
		[JsonProperty("transpaidamt")]
		public virtual decimal? TransPaidAmt { get; set; }
		[JsonProperty("transdueamt")]
		public virtual decimal? TransDueAmt { get; set; }
		[JsonProperty("productdesc")]
		[MaxLength(255)]
		public virtual string ProductDesc { get; set; }
		[JsonProperty("username")]
		[MaxLength(50)]
		public virtual string UserName { get; set; }
		[JsonProperty("useremail")]
		[MaxLength(80)]
		public virtual string UserEmail { get; set; }
		[JsonProperty("usercontact")]
		[MaxLength(16)]
		public virtual string UserContact { get; set; }
		[JsonProperty("responseurl")]
		[MaxLength(200)]
		public virtual string ResponseURL { get; set; }
		[JsonProperty("additionalurl")]
		[MaxLength(200)]
		public virtual string AdditionalURL { get; set; }
		[JsonProperty("remark")]
		[MaxLength(255)]
		public virtual string Remark { get; set; }
		[JsonProperty("signature")]
		[MaxLength(100)]
		public virtual string Signature { get; set; }
		[JsonProperty("transstatus")]
		public virtual byte? TransStatus { get; set; }
		[JsonProperty("status")]
		public virtual byte? Status { get; set; }
		[JsonProperty("synccreateby")]
		[MaxLength(20)]
		public virtual string SyncCreateBy { get; set; }
	}
	#endregion

	#region PAYMENTDTL
	[Abp.AutoMapper.AutoMapTo(typeof(DTO.Payment.PAYMENTDTL))]
	public class MapPAYMENTDTL : BaseMapId
	{
		[JsonProperty("bizlocid")]
		[MaxLength(20), Required]
		public virtual string BizLocID { get; set; }
		[JsonProperty("bizregid")]
		[MaxLength(20), Required]
		public virtual string BizRegID { get; set; }
		[JsonProperty("paymentid")]
		[MaxLength(20), Required]
		public virtual string PaymentID { get; set; }
		[JsonProperty("transref")]
		[MaxLength(20), Required]
		public virtual string TransRef { get; set; }
		[JsonProperty("seqno")]
		[Range(0, 9999), Required]
		public virtual int? SeqNo { get; set; }
		[JsonProperty("nextduedate")]
		public virtual System.DateTime? NextDueDate { get; set; }
		[JsonProperty("nextdueamount")]
		public virtual decimal? NextDueAmount { get; set; }
		[JsonProperty("remindattempt")]
		[Range(0, 9999)]
		public virtual int? RemindAttempt { get; set; }
		[JsonProperty("islatest")]
		public virtual byte? IsLatest { get; set; }
		[JsonProperty("attemptcount")]
		public virtual byte? AttemptCount { get; set; }
		[JsonProperty("remindlog")]
		[MaxLength(255)]
		public virtual string RemindLog { get; set; }
		[JsonProperty("linetotal")]
		public virtual decimal? LineTotal { get; set; }
		[JsonProperty("linetax")]
		public virtual decimal? LineTax { get; set; }
		[JsonProperty("linefee")]
		public virtual decimal? LineFee { get; set; }
		[JsonProperty("linecharge")]
		public virtual decimal? LineCharge { get; set; }
		[JsonProperty("linevat")]
		public virtual decimal? LineVAT { get; set; }
		[JsonProperty("lineoth")]
		public virtual decimal? LineOth { get; set; }
		[JsonProperty("linedisc")]
		public virtual decimal? LineDisc { get; set; }
		[JsonProperty("linegst")]
		public virtual decimal? LineGST { get; set; }
		[JsonProperty("currency")]
		[MaxLength(3)]
		public virtual string Currency { get; set; }
		[JsonProperty("isoverride")]
		public virtual byte? IsOverride { get; set; }
		[JsonProperty("transvoid")]
		public virtual byte? TransVoid { get; set; }
		[JsonProperty("status")]
		public virtual byte? Status { get; set; }
		[JsonProperty("synccreateby")]
		[MaxLength(20)]
		public virtual string SyncCreateBy { get; set; }
	}
	#endregion

	#region PAYMENTTENDER
	[Abp.AutoMapper.AutoMapTo(typeof(DTO.Payment.PAYMENTTENDER))]
	public class MapPAYMENTTENDER : BaseMapId
	{
		[JsonProperty("bizregid")]
		[MaxLength(20), Required]
		public virtual string BizRegID { get; set; }
		[JsonProperty("bizlocid")]
		[MaxLength(20), Required]
		public virtual string BizLocID { get; set; }
		[JsonProperty("paymenttransid")]
		[MaxLength(20), Required]
		public virtual string PaymentTransID { get; set; }
		[JsonProperty("tenderid")]
		[MaxLength(20), Required]
		public virtual string TenderID { get; set; }
		[JsonProperty("tendercode")]
		[MaxLength(10), Required]
		public virtual string TenderCode { get; set; }
		[JsonProperty("seqno")]
		[Range(0, 9999), Required]
		public virtual int? SeqNo { get; set; }
		[JsonProperty("tendertype")]
		public virtual byte? TenderType { get; set; }
		[JsonProperty("merchantcode")]
		[MaxLength(20)]
		public virtual string MerchantCode { get; set; }
		[JsonProperty("tenderref")]
		[MaxLength(20)]
		public virtual string TenderRef { get; set; }
		[JsonProperty("refno")]
		[MaxLength(255)]
		public virtual string RefNo { get; set; }
		[JsonProperty("tenderdate")]
		public virtual System.DateTime? TenderDate { get; set; }
		[JsonProperty("tendercurrency")]
		[MaxLength(3)]
		public virtual string TenderCurrency { get; set; }
		[JsonProperty("basecurrency")]
		[MaxLength(3)]
		public virtual string BaseCurrency { get; set; }
		[JsonProperty("exchgrate")]
		public virtual decimal? ExchgRate { get; set; }
		[JsonProperty("tenderamt")]
		public virtual decimal? TenderAmt { get; set; }
		[JsonProperty("feetype")]
		[MaxLength(20)]
		public virtual string FeeType { get; set; }
		[JsonProperty("feeamt")]
		public virtual decimal? FeeAmt { get; set; }
		[JsonProperty("payamt")]
		public virtual decimal? PayAmt { get; set; }
		[JsonProperty("transstatus")]
		public virtual byte? TransStatus { get; set; }
		[JsonProperty("status")]
		public virtual byte? Status { get; set; }
		[JsonProperty("synccreateby")]
		[MaxLength(20)]
		public virtual string SyncCreateBy { get; set; }
	}
	#endregion

	#region PAYMENTLOG
	[Abp.AutoMapper.AutoMapTo(typeof(DTO.Payment.PAYMENTLOG))]
	public class MapPAYMENTLOG : BaseMapId
	{
		[JsonProperty("bizregid")]
		[MaxLength(20), Required]
		public virtual string BizRegID { get; set; }
		[JsonProperty("bizlocid")]
		[MaxLength(20), Required]
		public virtual string BizLocID { get; set; }
		[JsonProperty("logid")]
		[MaxLength(20), Required]
		public virtual string LogID { get; set; }
		[JsonProperty("paymenttransid")]
		[MaxLength(20), Required]
		public virtual string PaymentTransID { get; set; }
		[JsonProperty("tenderid")]
		[MaxLength(20), Required]
		public virtual string TenderID { get; set; }
		[JsonProperty("tendercode")]
		[MaxLength(10), Required]
		public virtual string TenderCode { get; set; }
		[JsonProperty("seqno")]
		[Range(0, 9999), Required]
		public virtual int? SeqNo { get; set; }
		[JsonProperty("logdate")]
		public virtual System.DateTime? LogDate { get; set; }
		[JsonProperty("refno")]
		[MaxLength(255)]
		public virtual string RefNo { get; set; }
		[JsonProperty("logref")]
		[MaxLength(20)]
		public virtual string LogRef { get; set; }
		[JsonProperty("currency")]
		[MaxLength(3)]
		public virtual string Currency { get; set; }
		[JsonProperty("logamt")]
		public virtual decimal? LogAmt { get; set; }
		[JsonProperty("authorizationcode")]
		[MaxLength(10)]
		public virtual string AuthorizationCode { get; set; }
		[JsonProperty("merchantcode")]
		[MaxLength(20)]
		public virtual string MerchantCode { get; set; }
		[JsonProperty("bankcode")]
		[MaxLength(50)]
		public virtual string BankCode { get; set; }
		[JsonProperty("bankname")]
		[MaxLength(50)]
		public virtual string BankName { get; set; }
		[JsonProperty("checksum")]
		[MaxLength(50)]
		public virtual string CheckSum { get; set; }
		[JsonProperty("checksumstring")]
		[MaxLength(200)]
		public virtual string CheckSumString { get; set; }
		[JsonProperty("status")]
		public virtual byte? Status { get; set; }
		[JsonProperty("synccreateby")]
		[MaxLength(20)]
		public virtual string SyncCreateBy { get; set; }
	}
	#endregion

	#region TENDERTYPE
	[Abp.AutoMapper.AutoMapTo(typeof(DTO.Payment.TENDERTYPE))]
	public class MapTENDERTYPE : BaseMapId
	{
		[JsonProperty("tendercode")]
		[MaxLength(10), Required]
		public virtual string TenderCode { get; set; }
		[JsonProperty("tendertype")]
		public virtual byte? TenderType { get; set; }
		[JsonProperty("tenderdesc")]
		[MaxLength(50)]
		public virtual string TenderDesc { get; set; }
		[JsonProperty("effdate")]
		public virtual System.DateTime? EffDate { get; set; }
		[JsonProperty("enddate")]
		public virtual System.DateTime? EndDate { get; set; }
		[JsonProperty("status")]
		public virtual byte? Status { get; set; }
		[JsonProperty("synccreateby")]
		[MaxLength(20)]
		public virtual string SyncCreateBy { get; set; }
	}
	#endregion
}
