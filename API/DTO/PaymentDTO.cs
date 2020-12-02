using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Plexform.Base;
using Plexform.Audit;

namespace Plexform.DTO.Payment
{
	#region PAYMENTHDR
	[Table("PAYMENTHDR")]
	//[Audited]
	public class PAYMENTHDR : BaseDto<PAYMENTHDR>
	{
		[Key]
		[MaxLength(20), Required]
		public virtual string BizRegID { get; set; }
		[Key]
		[MaxLength(20), Required]
		public virtual string BizLocID { get; set; }
		[Key]
		[MaxLength(20), Required]
		public virtual string PaymentTransID { get; set; }
		[Key]
		[MaxLength(20), Required]
		public virtual string PaymentRef { get; set; }
		[Key]
		[MaxLength(20), Required]
		public virtual string MerchantCode { get; set; }
		[MaxLength(3), Required]
		public virtual string BaseCurrency { get; set; }
		[MaxLength(3), Required]
		public virtual string TransCurrency { get; set; }
		[Required]
		public virtual decimal? TransTotalAmt { get; set; }
		[Required]
		public virtual decimal? TransPaidAmt { get; set; }
		[Required]
		public virtual decimal? TransDueAmt { get; set; }
		[MaxLength(255), Required]
		public virtual string ProductDesc { get; set; }
		[MaxLength(50), Required]
		public virtual string UserName { get; set; }
		[MaxLength(80), Required]
		public virtual string UserEmail { get; set; }
		[MaxLength(16), Required]
		public virtual string UserContact { get; set; }
		[MaxLength(200), Required]
		public virtual string ResponseURL { get; set; }
		[MaxLength(200), Required]
		public virtual string AdditionalURL { get; set; }
		[MaxLength(255), Required]
		public virtual string Remark { get; set; }
		[MaxLength(100), Required]
		public virtual string Signature { get; set; }
		[Required]
		public virtual byte? TransStatus { get; set; }
		[Required]
		public virtual byte? Status { get; set; }
		[MaxLength(20), Required]
		public virtual string SyncCreateBy { get; set; }
	}
	#endregion

	#region PAYMENTDTL
	[Table("PAYMENTDTL")]
	//[Audited]
	public class PAYMENTDTL : BaseDto<PAYMENTDTL>
	{
		[Key]
		[MaxLength(20), Required]
		public virtual string BizLocID { get; set; }
		[Key]
		[MaxLength(20), Required]
		public virtual string BizRegID { get; set; }
		[Key]
		[MaxLength(20), Required]
		public virtual string PaymentID { get; set; }
		[Key]
		[MaxLength(20), Required]
		public virtual string TransRef { get; set; }
		[Key]
		[Range(0, 9999), Required]
		public virtual int? SeqNo { get; set; }
		[Column(TypeName = "datetime")]
		public virtual System.DateTime? NextDueDate { get; set; }
		[Required]
		public virtual decimal? NextDueAmount { get; set; }
		[Range(0, 9999), Required]
		public virtual int? RemindAttempt { get; set; }
		[Required]
		public virtual byte? IsLatest { get; set; }
		[Required]
		public virtual byte? AttemptCount { get; set; }
		[MaxLength(255), Required]
		public virtual string RemindLog { get; set; }
		[Required]
		public virtual decimal? LineTotal { get; set; }
		[Required]
		public virtual decimal? LineTax { get; set; }
		[Required]
		public virtual decimal? LineFee { get; set; }
		[Required]
		public virtual decimal? LineCharge { get; set; }
		[Required]
		public virtual decimal? LineVAT { get; set; }
		[Required]
		public virtual decimal? LineOth { get; set; }
		[Required]
		public virtual decimal? LineDisc { get; set; }
		[Required]
		public virtual decimal? LineGST { get; set; }
		[MaxLength(3), Required]
		public virtual string Currency { get; set; }
		[Required]
		public virtual byte? IsOverride { get; set; }
		[Required]
		public virtual byte? TransVoid { get; set; }
		[Required]
		public virtual byte? Status { get; set; }
		[MaxLength(20), Required]
		public virtual string SyncCreateBy { get; set; }
	}
	#endregion

	#region PAYMENTTENDER
	[Table("PAYMENTTENDER")]
	//[Audited]
	public class PAYMENTTENDER : BaseDto<PAYMENTTENDER>
	{
		[Key]
		[MaxLength(20), Required]
		public virtual string BizRegID { get; set; }
		[Key]
		[MaxLength(20), Required]
		public virtual string BizLocID { get; set; }
		[Key]
		[MaxLength(20), Required]
		public virtual string PaymentTransID { get; set; }
		[Key]
		[MaxLength(20), Required]
		public virtual string TenderID { get; set; }
		[Key]
		[MaxLength(10), Required]
		public virtual string TenderCode { get; set; }
		[Key]
		[Range(0, 9999), Required]
		public virtual int? SeqNo { get; set; }
		[Required]
		public virtual byte? TenderType { get; set; }
		[MaxLength(20), Required]
		public virtual string MerchantCode { get; set; }
		[MaxLength(20), Required]
		public virtual string TenderRef { get; set; }
		[MaxLength(255), Required]
		public virtual string RefNo { get; set; }
		[Column(TypeName = "datetime")]
		public virtual System.DateTime? TenderDate { get; set; }
		[MaxLength(3), Required]
		public virtual string TenderCurrency { get; set; }
		[MaxLength(3), Required]
		public virtual string BaseCurrency { get; set; }
		[Required]
		public virtual decimal? ExchgRate { get; set; }
		[Required]
		public virtual decimal? TenderAmt { get; set; }
		[MaxLength(20), Required]
		public virtual string FeeType { get; set; }
		[Required]
		public virtual decimal? FeeAmt { get; set; }
		[Required]
		public virtual decimal? PayAmt { get; set; }
		[Required]
		public virtual byte? TransStatus { get; set; }
		[Required]
		public virtual byte? Status { get; set; }
		[MaxLength(20), Required]
		public virtual string SyncCreateBy { get; set; }
	}
	#endregion

	#region PAYMENTLOG
	[Table("PAYMENTLOG")]
	//[Audited]
	public class PAYMENTLOG : BaseDto<PAYMENTLOG>
	{
		[Key]
		[MaxLength(20), Required]
		public virtual string BizRegID { get; set; }
		[Key]
		[MaxLength(20), Required]
		public virtual string BizLocID { get; set; }
		[Key]
		[MaxLength(20), Required]
		public virtual string LogID { get; set; }
		[Key]
		[MaxLength(20), Required]
		public virtual string PaymentTransID { get; set; }
		[Key]
		[MaxLength(20), Required]
		public virtual string TenderID { get; set; }
		[Key]
		[MaxLength(10), Required]
		public virtual string TenderCode { get; set; }
		[Key]
		[Range(0, 9999), Required]
		public virtual int? SeqNo { get; set; }
		[Column(TypeName = "datetime")]
		public virtual System.DateTime? LogDate { get; set; }
		[MaxLength(255), Required]
		public virtual string RefNo { get; set; }
		[MaxLength(20), Required]
		public virtual string LogRef { get; set; }
		[MaxLength(3), Required]
		public virtual string Currency { get; set; }
		[Required]
		public virtual decimal? LogAmt { get; set; }
		[MaxLength(10), Required]
		public virtual string AuthorizationCode { get; set; }
		[MaxLength(20), Required]
		public virtual string MerchantCode { get; set; }
		[MaxLength(50), Required]
		public virtual string BankCode { get; set; }
		[MaxLength(50), Required]
		public virtual string BankName { get; set; }
		[MaxLength(50), Required]
		public virtual string CheckSum { get; set; }
		[MaxLength(200), Required]
		public virtual string CheckSumString { get; set; }
		[Required]
		public virtual byte? Status { get; set; }
		[MaxLength(20), Required]
		public virtual string SyncCreateBy { get; set; }
	}
	#endregion

	#region TENDERTYPE
	[Table("TENDERTYPE")]
	//[Audited]
	public class TENDERTYPE : BaseDto<TENDERTYPE>
	{
		[Key]
		[MaxLength(10), Required]
		public virtual string TenderCode { get; set; }
		[Required]
		public virtual byte? TenderType { get; set; }
		[MaxLength(50), Required]
		public virtual string TenderDesc { get; set; }
		[Column(TypeName = "datetime")]
		public virtual System.DateTime? EffDate { get; set; }
		[Column(TypeName = "datetime")]
		public virtual System.DateTime? EndDate { get; set; }
		[Required]
		public virtual byte? Status { get; set; }
		[MaxLength(20), Required]
		public virtual string SyncCreateBy { get; set; }
	}
	#endregion
}
