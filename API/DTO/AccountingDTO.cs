using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Plexform.Base;
using Plexform.Audit;

namespace Plexform.DTO.Accounting
{
    #region SYSCODEB
    [Table("SYSCODEB")]
    public class SYSCODEB
    {
        [NotMapped]
        private DateTime _datetimenow;

        [Key]
        [Column(Order = 0)]
        [Required, StringLength(10)]
        public virtual string BranchID { get; set; }

        [Key]
        [Column(Order = 1)]
        [Required, StringLength(20)]
        public virtual string SysCode { get; set; }

        [Column(Order = 2), StringLength(50)]
        public virtual string SysDesc { get; set; }

        [Column(Order = 3), StringLength(10)]
        public virtual string Prefix { get; set; }

        [Column(Order = 4), StringLength(10)]
        public virtual string SpCode { get; set; }

        [Column(Order = 5), Range(0, 4)]
        public virtual int? RunNo { get; set; }

        [Column(Order = 6)]
        public virtual byte? NoLength { get; set; }

        [Column(Order = 7)]
        public virtual byte? NoPos { get; set; }

        [Column(Order = 8), StringLength(10)]
        public virtual string Postfix { get; set; }

        [Column(Order = 9, TypeName = "datetime")]
        public virtual System.DateTime? LastUpdate { get { return _datetimenow; } set { _datetimenow = DateTime.Now; } }

        [Column(Order = 10), StringLength(20)]
        public virtual string UpdateBy { get; set; }

        [Column(Order = 11)]
        public virtual byte? Status { get; set; }

        [Column(Order = 12)]
        public virtual byte? SysID { get; set; }

        [Column(Order = 13), StringLength(20)]
        public virtual string CheckFormat { get; set; }

        [Column(Order = 14), MaxLength(16)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual System.Guid rowguid { get; set; }

        [Column(Order = 15, TypeName = "datetime")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual System.DateTime? SyncCreate { get; set; }

        [Column(Order = 16, TypeName = "datetime")]
        public virtual System.DateTime? SyncLastUpd { get { return _datetimenow; } set { _datetimenow = DateTime.Now; } }

        [Column(Order = 17)]
        public virtual byte? IsHost { get; set; }

        [Column(Order = 18), StringLength(10)]
        public virtual string LastSyncBy { get; set; }

    }
    #endregion

    #region INVOICEHDR
    [Table("INVOICEHDR")]
    public class INVOICEHDR
    {
        [NotMapped]
        private DateTime _datetimenow;

        [Key]
        [Column(Order = 0)]
        [Required, StringLength(20)]
        public virtual string BizRegID { get; set; }

        [Key]
        [Column(Order = 1)]
        [Required, StringLength(20)]
        public virtual string BizLocID { get; set; }

        [Key]
        [Column(Order = 2)]
        [Required, StringLength(20)]
        public virtual string TransNo { get; set; }

        [Column(Order = 3), Range(0, 2)]
        public virtual int? TermID { get; set; }

        [Column(Order = 4)]
        public virtual byte? TransType { get; set; }

        [Column(Order = 5), StringLength(20)]
        public virtual string BillNo { get; set; }

        [Column(Order = 6), StringLength(50)]
        public virtual string CustPkgID { get; set; }

        [Column(Order = 7), StringLength(255)]
        public virtual string CashierID { get; set; }

        [Column(Order = 8), StringLength(10)]
        public virtual string ShiftCode { get; set; }

        [Column(Order = 9), Range(0, 4)]
        public virtual int? TotalServer { get; set; }

        [Column(Order = 10), StringLength(50)]
        public virtual string ServerID { get; set; }

        [Column(Order = 11)]
        public virtual byte? EmpType { get; set; }

        [Column(Order = 12), Range(0, 4)]
        public virtual int? TillID { get; set; }

        [Column(Order = 13, TypeName = "datetime")]
        public virtual System.DateTime? TransDate { get; set; }

        [Column(Order = 14), StringLength(6)]
        public virtual string TransStartTime { get; set; }

        [Column(Order = 15), StringLength(6)]
        public virtual string TransEndTime { get; set; }

        [Column(Order = 16), StringLength(6)]
        public virtual string TransPayTime { get; set; }

        [Column(Order = 17), MaxLength(9)]
        public virtual decimal? TransAmt { get; set; }

        [Column(Order = 18), MaxLength(9)]
        public virtual decimal? TransSubTotal { get; set; }

        [Column(Order = 19), MaxLength(9)]
        public virtual decimal? TransAmtRnd { get; set; }

        [Column(Order = 20), MaxLength(9)]
        public virtual decimal? TransAmtSave { get; set; }

        [Column(Order = 21), MaxLength(9)]
        public virtual decimal? TransAmtOrg { get; set; }

        [Column(Order = 22), MaxLength(9)]
        public virtual decimal? TransChgAmt { get; set; }

        [Column(Order = 23), StringLength(10)]
        public virtual string TransDiscCode { get; set; }

        [Column(Order = 24)]
        public virtual byte? TransDiscType { get; set; }

        [Column(Order = 25), MaxLength(9)]
        public virtual decimal? TransDiscValue { get; set; }

        [Column(Order = 26), MaxLength(9)]
        public virtual decimal? TransDiscAmt { get; set; }

        [Column(Order = 27), StringLength(20)]
        public virtual string TransDiscAuth { get; set; }

        [Column(Order = 28), StringLength(50)]
        public virtual string TransDiscReasonCode { get; set; }

        [Column(Order = 29), StringLength(50)]
        public virtual string TransDiscRemark { get; set; }

        [Column(Order = 30), MaxLength(9)]
        public virtual decimal? TransAmtSpDisc { get; set; }

        [Column(Order = 31), MaxLength(9)]
        public virtual decimal? TransValueSpDisc { get; set; }

        [Column(Order = 32), StringLength(20)]
        public virtual string AuthSpDisc { get; set; }

        [Column(Order = 33), StringLength(10)]
        public virtual string SpDiscReasonCode { get; set; }

        [Column(Order = 34), StringLength(200)]
        public virtual string SpDiscRemark { get; set; }

        [Column(Order = 35), StringLength(20)]
        public virtual string CustomerID { get; set; }

        [Column(Order = 36)]
        public virtual byte? CustType { get; set; }

        [Column(Order = 37), StringLength(255)]
        public virtual string CustPrivilege { get; set; }

        [Column(Order = 38), StringLength(50)]
        public virtual string AcctNo { get; set; }

        [Column(Order = 39), MaxLength(9)]
        public virtual decimal? TotalPoints { get; set; }

        [Column(Order = 40), StringLength(20)]
        public virtual string InSvcID { get; set; }

        [Column(Order = 41), StringLength(50)]
        public virtual string TransReasonCode { get; set; }

        [Column(Order = 42), StringLength(255)]
        public virtual string TransRemark { get; set; }

        [Column(Order = 43), StringLength(20)]
        public virtual string TransStatus { get; set; }

        [Column(Order = 44)]
        public virtual byte? Posted { get; set; }

        [Column(Order = 45, TypeName = "datetime")]
        public virtual System.DateTime? PostDate { get; set; }

        [Column(Order = 46)]
        public virtual byte? Training { get; set; }

        [Column(Order = 47)]
        public virtual byte? Profiled { get; set; }

        [Column(Order = 48)]
        public virtual byte? LiveCal { get; set; }

        [Column(Order = 49, TypeName = "datetime")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual System.DateTime? CreateDate { get; set; }

        [Column(Order = 50), StringLength(20)]
        public virtual string CreateBy { get; set; }

        [Column(Order = 51, TypeName = "datetime")]
        public virtual System.DateTime? LastUpdate { get { return _datetimenow; } set { _datetimenow = DateTime.Now; } }

        [Column(Order = 52), StringLength(20)]
        public virtual string UpdateBy { get; set; }

        [Column(Order = 53)]
        public virtual byte? Status { get; set; }

        [Column(Order = 54)]
        public virtual byte? TransVoid { get; set; }

        [Column(Order = 55)]
        public virtual byte? Flag { get; set; }

        [Column(Order = 56), MaxLength(16)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual System.Guid rowguid { get; set; }

        [Column(Order = 57, TypeName = "datetime")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual System.DateTime? SyncCreate { get; set; }

        [Column(Order = 58, TypeName = "datetime")]
        public virtual System.DateTime? SyncLastUpd { get { return _datetimenow; } set { _datetimenow = DateTime.Now; } }

        [Column(Order = 59), StringLength(10)]
        public virtual string TblNo { get; set; }

        [Column(Order = 60), Range(0, 4)]
        public virtual int? TblPax { get; set; }

        [Column(Order = 61)]
        public virtual byte? TransPointsStatus { get; set; }

        [Column(Order = 62)]
        public virtual byte? IsHost { get; set; }

        [Column(Order = 63)]
        public virtual byte? LastSyncBy { get; set; }

        [Column(Order = 64), MaxLength(9)]
        public virtual decimal? TransPoints { get; set; }

        [Column(Order = 65), StringLength(10)]
        public virtual string SyncCreateBy { get; set; }

        [Column(Order = 66)]
        public virtual byte? Active { get; set; }

        [Column(Order = 67)]
        public virtual byte? Inuse { get; set; }

        [Column(Order = 68), Range(0, 4)]
        public virtual int? TID { get; set; }

    }
    #endregion

    #region INVOICEDTL
    [Table("INVOICEDTL")]
    public class INVOICEDTL
    {
        [NotMapped]
        private DateTime _datetimenow;

        [Key]
        [Column(Order = 0)]
        [Required, StringLength(20)]
        public virtual string BizLocID { get; set; }

        [Key]
        [Column(Order = 1)]
        [Required, StringLength(20)]
        public virtual string BizRegID { get; set; }

        [Key]
        [Column(Order = 2)]
        [Required, StringLength(20)]
        public virtual string TransNo { get; set; }

        [Key]
        [Column(Order = 3)]
        [Required, Range(0, 4)]
        public virtual int? TransSeq { get; set; }

        [Key]
        [Column(Order = 4)]
        [Required, StringLength(20)]
        public virtual string ItemCode { get; set; }

        [Column(Order = 5), Range(0, 2)]
        public virtual int? TermID { get; set; }

        [Column(Order = 6), Range(0, 4)]
        public virtual int? RefSeq { get; set; }

        [Column(Order = 7)]
        public virtual byte? IsCal { get; set; }

        [Column(Order = 8), StringLength(20)]
        public virtual string BillNo { get; set; }

        [Column(Order = 9), StringLength(6)]
        public virtual string EntryTime { get; set; }

        [Column(Order = 10), StringLength(20)]
        public virtual string StkCode { get; set; }

        [Column(Order = 11), StringLength(50)]
        public virtual string StkDesc { get; set; }

        [Column(Order = 12), StringLength(3)]
        public virtual string StkType { get; set; }

        [Column(Order = 13)]
        public virtual byte? BehvType { get; set; }

        [Column(Order = 14), StringLength(20)]
        public virtual string ItemType { get; set; }

        [Column(Order = 15), Range(0, 4)]
        public virtual int? Qty { get; set; }

        [Column(Order = 16), MaxLength(9)]
        public virtual decimal? UnitCost { get; set; }

        [Column(Order = 17), MaxLength(9)]
        public virtual decimal? OrgPrice { get; set; }

        [Column(Order = 18), MaxLength(9)]
        public virtual decimal? NettPrice { get; set; }

        [Column(Order = 19), MaxLength(9)]
        public virtual decimal? TolAmt { get; set; }

        [Column(Order = 20), MaxLength(9)]
        public virtual decimal? DiscAmt { get; set; }

        [Column(Order = 21), MaxLength(9)]
        public virtual decimal? NettAmt { get; set; }

        [Column(Order = 22), MaxLength(9)]
        public virtual decimal? SubPoints { get; set; }

        [Column(Order = 23), MaxLength(9)]
        public virtual decimal? Points { get; set; }

        [Column(Order = 24)]
        public virtual byte? PriceOverrided { get; set; }

        [Column(Order = 25)]
        public virtual byte? Discounted { get; set; }

        [Column(Order = 26)]
        public virtual byte? Taxable { get; set; }

        [Column(Order = 27), StringLength(10)]
        public virtual string ReturnCode { get; set; }

        [Column(Order = 28), StringLength(4000)]
        public virtual string Remark { get; set; }

        [Column(Order = 29), StringLength(50)]
        public virtual string SerialNo { get; set; }

        [Column(Order = 30), StringLength(20)]
        public virtual string PriceAuthID { get; set; }

        [Column(Order = 31)]
        public virtual byte? TransVoid { get; set; }

        [Column(Order = 32)]
        public virtual byte? Posted { get; set; }

        [Column(Order = 33)]
        public virtual byte? LiveCal { get; set; }

        [Column(Order = 34)]
        public virtual byte? Flag { get; set; }

        [Column(Order = 35), StringLength(20)]
        public virtual string CustPkgID { get; set; }

        [Column(Order = 36)]
        public virtual byte? PrcDisplayType { get; set; }

        [Column(Order = 37), StringLength(20)]
        public virtual string PromoCode { get; set; }

        [Column(Order = 38, TypeName = "datetime")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual System.DateTime? CreateDate { get; set; }

        [Column(Order = 39), StringLength(20)]
        public virtual string CreateBy { get; set; }

        [Column(Order = 40, TypeName = "datetime")]
        public virtual System.DateTime? LastUpdate { get { return _datetimenow; } set { _datetimenow = DateTime.Now; } }

        [Column(Order = 41), StringLength(20)]
        public virtual string UpdateBy { get; set; }

        [Column(Order = 42)]
        public virtual byte? Status { get; set; }

        [Column(Order = 43), MaxLength(16)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual System.Guid rowguid { get; set; }

        [Column(Order = 44, TypeName = "datetime")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual System.DateTime? SyncCreate { get; set; }

        [Column(Order = 45, TypeName = "datetime")]
        public virtual System.DateTime? SyncLastUpd { get { return _datetimenow; } set { _datetimenow = DateTime.Now; } }

        [Column(Order = 46)]
        public virtual byte? IsHost { get; set; }

        [Column(Order = 47), StringLength(20)]
        public virtual string LastSyncBy { get; set; }

        [Column(Order = 48), StringLength(20)]
        public virtual string SyncCreateBy { get; set; }

        [Column(Order = 49), StringLength(10)]
        public virtual string ExCode1 { get; set; }

        [Column(Order = 50), StringLength(10)]
        public virtual string ExCode2 { get; set; }

        [Column(Order = 51), MaxLength(9)]
        public virtual decimal? BaseRate { get; set; }

        [Column(Order = 52), MaxLength(9)]
        public virtual decimal? CoRate1H { get; set; }

        [Column(Order = 53), MaxLength(9)]
        public virtual decimal? CoRate1G { get; set; }

        [Column(Order = 54), MaxLength(9)]
        public virtual decimal? CoRate2H { get; set; }

        [Column(Order = 55), MaxLength(9)]
        public virtual decimal? CoRate2G { get; set; }

        [Column(Order = 56), MaxLength(9)]
        public virtual decimal? CoRate3H { get; set; }

        [Column(Order = 57), MaxLength(9)]
        public virtual decimal? CoRate3G { get; set; }

        [Column(Order = 58), MaxLength(9)]
        public virtual decimal? CoRate4H { get; set; }

        [Column(Order = 59), MaxLength(9)]
        public virtual decimal? CoRate4G { get; set; }

        [Column(Order = 60), MaxLength(9)]
        public virtual decimal? CoRate5H { get; set; }

        [Column(Order = 61), MaxLength(9)]
        public virtual decimal? CoRate5G { get; set; }

        [Column(Order = 62), StringLength(20)]
        public virtual string WarSerialNo { get; set; }

        [Column(Order = 63)]
        public virtual byte? Active { get; set; }

        [Column(Order = 64)]
        public virtual byte? Inuse { get; set; }

        [Column(Order = 65), Range(0, 4)]
        public virtual int? TID { get; set; }

        [Column(Order = 66), MaxLength(255)]
        public virtual string PathRef { get; set; }

    }
    #endregion

    #region INVOICETAX
    [Table("INVOICETAX")]
    public class INVOICETAX
    {
        [NotMapped]
        private DateTime _datetimenow;

        [Key]
        [Column(Order = 0)]
        [Required, StringLength(20)]
        public virtual string BizRegID { get; set; }

        [Key]
        [Column(Order = 1)]
        [Required, StringLength(20)]
        public virtual string BizLocID { get; set; }

        [Key]
        [Column(Order = 2)]
        [Required, StringLength(20)]
        public virtual string TransNo { get; set; }

        [Key]
        [Column(Order = 3)]
        [Required, Range(0, 4)]
        public virtual int? TaxCode { get; set; }

        [Column(Order = 4), Range(0, 2)]
        public virtual int? TermID { get; set; }

        [Column(Order = 5), MaxLength(9)]
        public virtual decimal? TaxAmt { get; set; }

        [Column(Order = 6), MaxLength(5)]
        public virtual decimal? TaxRate { get; set; }

        [Column(Order = 7)]
        public virtual byte? TaxInEx { get; set; }

        [Column(Order = 8), MaxLength(9)]
        public virtual decimal? TaxCharge { get; set; }

        [Column(Order = 9), StringLength(255)]
        public virtual string TaxRemark { get; set; }

        [Column(Order = 10)]
        public virtual byte? TransVoid { get; set; }

        [Column(Order = 11)]
        public virtual byte? Posted { get; set; }

        [Column(Order = 12, TypeName = "datetime")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual System.DateTime? CreateDate { get; set; }

        [Column(Order = 13), StringLength(20)]
        public virtual string CreateBy { get; set; }

        [Column(Order = 14), MaxLength(16)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual System.Guid rowguid { get; set; }

        [Column(Order = 15, TypeName = "datetime")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual System.DateTime? SyncCreate { get; set; }

        [Column(Order = 16, TypeName = "datetime")]
        public virtual System.DateTime? SyncLastUpd { get { return _datetimenow; } set { _datetimenow = DateTime.Now; } }

        [Column(Order = 17)]
        public virtual byte? IsHost { get; set; }

        [Column(Order = 18), StringLength(20)]
        public virtual string LastSyncBy { get; set; }

        [Column(Order = 19), Range(0, 4)]
        public virtual int? Points { get; set; }

        [Column(Order = 20), StringLength(20)]
        public virtual string SyncCreateBy { get; set; }

        [Column(Order = 21)]
        public virtual byte? Active { get; set; }

        [Column(Order = 22)]
        public virtual byte? Inuse { get; set; }

        [Column(Order = 23), Range(0, 4)]
        public virtual int? TID { get; set; }

    }
    #endregion

    #region TRANSHDR
    [Table("TRANSHDR")]
    public class TRANSHDR
    {
        [NotMapped]
        private DateTime _datetimenow;

        [Key]
        [Column(Order = 0)]
        [Required, StringLength(20)]
        public virtual string BizRegID { get; set; }

        [Key]
        [Column(Order = 1)]
        [Required, StringLength(20)]
        public virtual string BizLocID { get; set; }

        [Key]
        [Column(Order = 2)]
        [Required, Range(0, 2)]
        public virtual int? TermID { get; set; }

        [Key]
        [Column(Order = 3)]
        [Required, StringLength(20)]
        public virtual string TransNo { get; set; }

        [Column(Order = 4)]
        public virtual byte? TransType { get; set; }

        [Column(Order = 5), StringLength(20)]
        public virtual string BillNo { get; set; }

        [Column(Order = 6), StringLength(50)]
        public virtual string CustPkgID { get; set; }

        [Column(Order = 7), StringLength(50)]
        public virtual string CashierID { get; set; }

        [Column(Order = 8), StringLength(255)]
        public virtual string ShiftCode { get; set; }

        [Column(Order = 9), Range(0, 4)]
        public virtual int? TotalServer { get; set; }

        [Column(Order = 10), StringLength(50)]
        public virtual string ServerID { get; set; }

        [Column(Order = 11)]
        public virtual byte? EmpType { get; set; }

        [Column(Order = 12), Range(0, 4)]
        public virtual int? TillID { get; set; }

        [Column(Order = 13, TypeName = "datetime")]
        public virtual System.DateTime? TransDate { get; set; }

        [Column(Order = 14), StringLength(6)]
        public virtual string TransStartTime { get; set; }

        [Column(Order = 15), StringLength(6)]
        public virtual string TransEndTime { get; set; }

        [Column(Order = 16), StringLength(6)]
        public virtual string TransPayTime { get; set; }

        [Column(Order = 17), MaxLength(9)]
        public virtual decimal? TransAmt { get; set; }

        [Column(Order = 18), MaxLength(9)]
        public virtual decimal? TransSubTotal { get; set; }

        [Column(Order = 19), MaxLength(9)]
        public virtual decimal? TransAmtRnd { get; set; }

        [Column(Order = 20), MaxLength(9)]
        public virtual decimal? TransAmtSave { get; set; }

        [Column(Order = 21), MaxLength(9)]
        public virtual decimal? TransAmtOrg { get; set; }

        [Column(Order = 22), MaxLength(9)]
        public virtual decimal? TransChgAmt { get; set; }

        [Column(Order = 23), StringLength(10)]
        public virtual string TransDiscCode { get; set; }

        [Column(Order = 24)]
        public virtual byte? TransDiscType { get; set; }

        [Column(Order = 25), MaxLength(9)]
        public virtual decimal? TransDiscValue { get; set; }

        [Column(Order = 26), MaxLength(9)]
        public virtual decimal? TransDiscAmt { get; set; }

        [Column(Order = 27), StringLength(20)]
        public virtual string TransDiscAuth { get; set; }

        [Column(Order = 28), StringLength(50)]
        public virtual string TransDiscReasonCode { get; set; }

        [Column(Order = 29), StringLength(50)]
        public virtual string TransDiscRemark { get; set; }

        [Column(Order = 30), MaxLength(9)]
        public virtual decimal? TransAmtSpDisc { get; set; }

        [Column(Order = 31), MaxLength(9)]
        public virtual decimal? TransValueSpDisc { get; set; }

        [Column(Order = 32), StringLength(20)]
        public virtual string AuthSpDisc { get; set; }

        [Column(Order = 33), StringLength(10)]
        public virtual string SpDiscReasonCode { get; set; }

        [Column(Order = 34), StringLength(200)]
        public virtual string SpDiscRemark { get; set; }

        [Column(Order = 35), StringLength(20)]
        public virtual string CustomerID { get; set; }

        [Column(Order = 36)]
        public virtual byte? CustType { get; set; }

        [Column(Order = 37), StringLength(255)]
        public virtual string CustPrivilege { get; set; }

        [Column(Order = 38), StringLength(50)]
        public virtual string AcctNo { get; set; }

        [Column(Order = 39), MaxLength(9)]
        public virtual decimal? TotalPoints { get; set; }

        [Column(Order = 40), StringLength(20)]
        public virtual string InSvcID { get; set; }

        [Column(Order = 41), StringLength(50)]
        public virtual string TransReasonCode { get; set; }

        [Column(Order = 42), StringLength(255)]
        public virtual string TransRemark { get; set; }

        [Column(Order = 43), StringLength(20)]
        public virtual string TransStatus { get; set; }

        [Column(Order = 44)]
        public virtual byte? Posted { get; set; }

        [Column(Order = 45, TypeName = "datetime")]
        public virtual System.DateTime? PostDate { get; set; }

        [Column(Order = 46)]
        public virtual byte? Training { get; set; }

        [Column(Order = 47)]
        public virtual byte? Profiled { get; set; }

        [Column(Order = 48)]
        public virtual byte? LiveCal { get; set; }

        [Column(Order = 49, TypeName = "datetime")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual System.DateTime? CreateDate { get; set; }

        [Column(Order = 50), StringLength(20)]
        public virtual string CreateBy { get; set; }

        [Column(Order = 51, TypeName = "datetime")]
        public virtual System.DateTime? LastUpdate { get { return _datetimenow; } set { _datetimenow = DateTime.Now; } }

        [Column(Order = 52), StringLength(20)]
        public virtual string UpdateBy { get; set; }

        [Column(Order = 53)]
        public virtual byte? Status { get; set; }

        [Column(Order = 54)]
        public virtual byte? TransVoid { get; set; }

        [Column(Order = 55)]
        public virtual byte? Flag { get; set; }

        [Column(Order = 56), MaxLength(16)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual System.Guid rowguid { get; set; }

        [Column(Order = 57, TypeName = "datetime")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual System.DateTime? SyncCreate { get; set; }

        [Column(Order = 58, TypeName = "datetime")]
        public virtual System.DateTime? SyncLastUpd { get { return _datetimenow; } set { _datetimenow = DateTime.Now; } }

        [Column(Order = 59), StringLength(10)]
        public virtual string TblNo { get; set; }

        [Column(Order = 60), Range(0, 4)]
        public virtual int? TblPax { get; set; }

        [Column(Order = 61)]
        public virtual byte? TransPointsStatus { get; set; }

        [Column(Order = 62)]
        public virtual byte? IsHost { get; set; }

        [Column(Order = 63)]
        public virtual byte? LastSyncBy { get; set; }

        [Column(Order = 64), MaxLength(9)]
        public virtual decimal? TransPoints { get; set; }

        [Column(Order = 65), StringLength(10)]
        public virtual string SyncCreateBy { get; set; }

        [Column(Order = 66), Range(0, 4)]
        public virtual int? TID { get; set; }

    }
    #endregion

    #region TRANSDTL
    [Table("TRANSDTL")]
    public class TRANSDTL
    {
        [NotMapped]
        private DateTime _datetimenow;

        [Key]
        [Column(Order = 0)]
        [Required, StringLength(20)]
        public virtual string BizLocID { get; set; }

        [Key]
        [Column(Order = 1)]
        [Required, Range(0, 2)]
        public virtual int? TermID { get; set; }

        [Key]
        [Column(Order = 2)]
        [Required, StringLength(20)]
        public virtual string TransNo { get; set; }

        [Key]
        [Column(Order = 3)]
        [Required, Range(0, 100)]
        public virtual int? TransSeq { get; set; }

        [Column(Order = 4), StringLength(20)]
        public virtual string BizRegID { get; set; }

        [Column(Order = 5), Range(0, 4)]
        public virtual int? RefSeq { get; set; }

        [Column(Order = 6)]
        public virtual byte? IsCal { get; set; }

        [Column(Order = 7), StringLength(20)]
        public virtual string BillNo { get; set; }

        [Column(Order = 8), StringLength(6)]
        public virtual string EntryTime { get; set; }

        [Column(Order = 9), StringLength(20)]
        public virtual string StkCode { get; set; }

        [Column(Order = 10), StringLength(50)]
        public virtual string StkDesc { get; set; }

        [Column(Order = 11), StringLength(3)]
        public virtual string StkType { get; set; }

        [Column(Order = 12)]
        public virtual byte? BehvType { get; set; }

        [Column(Order = 13)]
        public virtual byte? ItemType { get; set; }

        [Column(Order = 14), StringLength(20)]
        public virtual string ItemCode { get; set; }

        [Column(Order = 15), Range(0, 4)]
        public virtual int? Qty { get; set; }

        [Column(Order = 16), MaxLength(9)]
        public virtual decimal? UnitCost { get; set; }

        [Column(Order = 17), MaxLength(9)]
        public virtual decimal? OrgPrice { get; set; }

        [Column(Order = 18), MaxLength(9)]
        public virtual decimal? NettPrice { get; set; }

        [Column(Order = 19), MaxLength(9)]
        public virtual decimal? TolAmt { get; set; }

        [Column(Order = 20), MaxLength(9)]
        public virtual decimal? DiscAmt { get; set; }

        [Column(Order = 21), MaxLength(9)]
        public virtual decimal? NettAmt { get; set; }

        [Column(Order = 22), MaxLength(9)]
        public virtual decimal? SubPoints { get; set; }

        [Column(Order = 23), MaxLength(9)]
        public virtual decimal? Points { get; set; }

        [Column(Order = 24)]
        public virtual byte? PriceOverrided { get; set; }

        [Column(Order = 25)]
        public virtual byte? Discounted { get; set; }

        [Column(Order = 26)]
        public virtual byte? Taxable { get; set; }

        [Column(Order = 27), StringLength(10)]
        public virtual string ReturnCode { get; set; }

        [Column(Order = 28), StringLength(4000)]
        public virtual string Remark { get; set; }

        [Column(Order = 29), StringLength(50)]
        public virtual string SerialNo { get; set; }

        [Column(Order = 30), StringLength(20)]
        public virtual string PriceAuthID { get; set; }

        [Column(Order = 31)]
        public virtual byte? TransVoid { get; set; }

        [Column(Order = 32)]
        public virtual byte? Posted { get; set; }

        [Column(Order = 33)]
        public virtual byte? LiveCal { get; set; }

        [Column(Order = 34)]
        public virtual byte? Flag { get; set; }

        [Column(Order = 35), StringLength(20)]
        public virtual string CustPkgID { get; set; }

        [Column(Order = 36)]
        public virtual byte? PrcDisplayType { get; set; }

        [Column(Order = 37), StringLength(20)]
        public virtual string PromoCode { get; set; }

        [Column(Order = 38, TypeName = "datetime")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual System.DateTime? CreateDate { get; set; }

        [Column(Order = 39, TypeName = "datetime")]
        public virtual System.DateTime? LastUpdate { get { return _datetimenow; } set { _datetimenow = DateTime.Now; } }

        [Column(Order = 40), StringLength(20)]
        public virtual string UpdateBy { get; set; }

        [Column(Order = 41)]
        public virtual byte? Status { get; set; }

        [Column(Order = 42), MaxLength(16)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual System.Guid rowguid { get; set; }

        [Column(Order = 43, TypeName = "datetime")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual System.DateTime? SyncCreate { get; set; }

        [Column(Order = 44, TypeName = "datetime")]
        public virtual System.DateTime? SyncLastUpd { get { return _datetimenow; } set { _datetimenow = DateTime.Now; } }

        [Column(Order = 45)]
        public virtual byte? IsHost { get; set; }

        [Column(Order = 46), StringLength(20)]
        public virtual string LastSyncBy { get; set; }

        [Column(Order = 47), StringLength(20)]
        public virtual string SyncCreateBy { get; set; }

        [Column(Order = 48), StringLength(10)]
        public virtual string ExCode1 { get; set; }

        [Column(Order = 49), StringLength(10)]
        public virtual string ExCode2 { get; set; }

        [Column(Order = 50), MaxLength(9)]
        public virtual decimal? BaseRate { get; set; }

        [Column(Order = 51), MaxLength(9)]
        public virtual decimal? CoRate1H { get; set; }

        [Column(Order = 52), MaxLength(9)]
        public virtual decimal? CoRate1G { get; set; }

        [Column(Order = 53), MaxLength(9)]
        public virtual decimal? CoRate2H { get; set; }

        [Column(Order = 54), MaxLength(9)]
        public virtual decimal? CoRate2G { get; set; }

        [Column(Order = 55), MaxLength(9)]
        public virtual decimal? CoRate3H { get; set; }

        [Column(Order = 56), MaxLength(9)]
        public virtual decimal? CoRate3G { get; set; }

        [Column(Order = 57), MaxLength(9)]
        public virtual decimal? CoRate4H { get; set; }

        [Column(Order = 58), MaxLength(9)]
        public virtual decimal? CoRate4G { get; set; }

        [Column(Order = 59), MaxLength(9)]
        public virtual decimal? CoRate5H { get; set; }

        [Column(Order = 60), MaxLength(9)]
        public virtual decimal? CoRate5G { get; set; }

        [Column(Order = 61), StringLength(20)]
        public virtual string WarSerialNo { get; set; }

        [Column(Order = 62), Range(0, 4)]
        public virtual int? TID { get; set; }

    }
    #endregion

    #region TRANSFPX
    [Table("TRANSFPX")]
    public class TRANSFPX
    {
        [NotMapped]
        private DateTime _datetimenow;

        [Key]
        [Column(Order = 0)]
        [Required, StringLength(20)]
        public virtual string BizRegID { get; set; }

        [Key]
        [Column(Order = 1)]
        [Required, StringLength(20)]
        public virtual string BizLocID { get; set; }

        [Key]
        [Column(Order = 2)]
        [Required, Range(0, 2)]
        public virtual int? TermID { get; set; }

        [Key]
        [Column(Order = 3)]
        [Required, StringLength(20)]
        public virtual string TransNo { get; set; }

        [Key]
        [Column(Order = 4)]
        [Required, Range(0, 4)]
        public virtual int? TransSeq { get; set; }

        [Column(Order = 5, TypeName = "datetime")]
        public virtual System.DateTime? TransDate { get; set; }

        [Column(Order = 6), StringLength(6)]
        public virtual string TransTime { get; set; }

        [Column(Order = 7), StringLength(50)]
        public virtual string TenderID { get; set; }

        [Column(Order = 8), MaxLength(9)]
        public virtual decimal? TenderAmt { get; set; }

        [Column(Order = 9), MaxLength(5)]
        public virtual decimal? ExchgRate { get; set; }

        [Column(Order = 10), MaxLength(9)]
        public virtual decimal? TenderDue { get; set; }

        [Column(Order = 11), MaxLength(9)]
        public virtual decimal? ChangeAmt { get; set; }

        [Column(Order = 12), StringLength(4000)]
        public virtual string RefInfo { get; set; }

        [Column(Order = 13), StringLength(255)]
        public virtual string CCrefInfo { get; set; }

        [Column(Order = 14, TypeName = "datetime")]
        public virtual System.DateTime? ExpDate { get; set; }

        [Column(Order = 15), StringLength(255)]
        public virtual string CustName { get; set; }

        [Column(Order = 16, TypeName = "datetime")]
        public virtual System.DateTime? ClearingDate { get; set; }

        [Column(Order = 17)]
        public virtual byte? TransVoid { get; set; }

        [Column(Order = 18)]
        public virtual byte? Posted { get; set; }

        [Column(Order = 19, TypeName = "datetime")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual System.DateTime? CreateDate { get; set; }

        [Column(Order = 20), StringLength(20)]
        public virtual string CreateBy { get; set; }

        [Column(Order = 21, TypeName = "datetime")]
        public virtual System.DateTime? LastUpdate { get { return _datetimenow; } set { _datetimenow = DateTime.Now; } }

        [Column(Order = 22), StringLength(20)]
        public virtual string UpdateBy { get; set; }

        [Column(Order = 23)]
        public virtual byte? IsHost { get; set; }

        [Column(Order = 24)]
        public virtual byte? LastSyncBy { get; set; }

        [Column(Order = 25)]
        public virtual byte? Active { get; set; }

        [Column(Order = 26)]
        public virtual byte? Inuse { get; set; }

        [Column(Order = 27)]
        public virtual byte? Flag { get; set; }

        [Column(Order = 28, TypeName = "datetime")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual System.DateTime? SyncCreate { get; set; }

        [Column(Order = 29, TypeName = "datetime")]
        public virtual System.DateTime? SyncLastUpd { get { return _datetimenow; } set { _datetimenow = DateTime.Now; } }

        [Column(Order = 30), MaxLength(16)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual System.Guid rowguid { get; set; }

        [Column(Order = 31), Range(0, 4)]
        public virtual int? TID { get; set; }

    }
    #endregion

    #region TRANSTENDER
    [Table("TRANSTENDER")]
    public class TRANSTENDER
    {
        [NotMapped]
        private DateTime _datetimenow;

        [Key]
        [Column(Order = 0)]
        [Required, StringLength(20)]
        public virtual string BizRegID { get; set; }

        [Key]
        [Column(Order = 1)]
        [Required, StringLength(20)]
        public virtual string BizLocID { get; set; }

        [Key]
        [Column(Order = 2)]
        [Required, Range(0, 2)]
        public virtual int? TermID { get; set; }

        [Key]
        [Column(Order = 3)]
        [Required, StringLength(20)]
        public virtual string TransNo { get; set; }

        [Key]
        [Column(Order = 4)]
        [Required, Range(0, 100)]
        public virtual int? TransSeq { get; set; }

        [Column(Order = 5), StringLength(20)]
        public virtual string BillNo { get; set; }

        [Column(Order = 6, TypeName = "datetime")]
        public virtual System.DateTime? TransDate { get; set; }

        [Column(Order = 7), StringLength(6)]
        public virtual string TransTime { get; set; }

        [Column(Order = 8), StringLength(10)]
        public virtual string TenderType { get; set; }

        [Column(Order = 9), StringLength(10)]
        public virtual string TenderID { get; set; }

        [Column(Order = 10), MaxLength(9)]
        public virtual decimal? TenderAmt { get; set; }

        [Column(Order = 11), MaxLength(5)]
        public virtual decimal? ExchgRate { get; set; }

        [Column(Order = 12), MaxLength(9)]
        public virtual decimal? TenderDue { get; set; }

        [Column(Order = 13), MaxLength(9)]
        public virtual decimal? ChangeAmt { get; set; }

        [Column(Order = 14), StringLength(3)]
        public virtual string Currency { get; set; }

        [Column(Order = 15), StringLength(255)]
        public virtual string ExternalID { get; set; }

        [Column(Order = 16), StringLength(255)]
        public virtual string MerchantID { get; set; }

        [Column(Order = 17), StringLength(255)]
        public virtual string RefNo { get; set; }

        [Column(Order = 18), StringLength(255)]
        public virtual string RefKey { get; set; }

        [Column(Order = 19), StringLength(20)]
        public virtual string RefType { get; set; }

        [Column(Order = 20), StringLength(4000)]
        public virtual string RefToken { get; set; }

        [Column(Order = 21), StringLength(200)]
        public virtual string RefRemark { get; set; }

        [Column(Order = 22), StringLength(200)]
        public virtual string RefRemark2 { get; set; }

        [Column(Order = 23), StringLength(200)]
        public virtual string RefRemark3 { get; set; }

        [Column(Order = 24), StringLength(255)]
        public virtual string RefOth1 { get; set; }

        [Column(Order = 25), StringLength(255)]
        public virtual string RefOth2 { get; set; }

        [Column(Order = 26), StringLength(255)]
        public virtual string RefOth3 { get; set; }

        [Column(Order = 27), StringLength(200)]
        public virtual string CustName { get; set; }

        [Column(Order = 28), StringLength(255)]
        public virtual string CardNo { get; set; }

        [Column(Order = 29), StringLength(50)]
        public virtual string BankCode { get; set; }

        [Column(Order = 30), StringLength(50)]
        public virtual string BankAccNo { get; set; }

        [Column(Order = 31), StringLength(50)]
        public virtual string RespCode { get; set; }

        [Column(Order = 32), StringLength(4000)]
        public virtual string AuthCode { get; set; }

        [Column(Order = 33), StringLength(255)]
        public virtual string TraceNo { get; set; }

        [Column(Order = 34, TypeName = "datetime")]
        public virtual System.DateTime? ExpDate { get; set; }

        [Column(Order = 35, TypeName = "datetime")]
        public virtual System.DateTime? ClearingDate { get; set; }

        [Column(Order = 36)]
        public virtual byte? IsApproved { get; set; }

        [Column(Order = 37), StringLength(20)]
        public virtual string ApprovedBy { get; set; }

        [Column(Order = 38, TypeName = "datetime")]
        public virtual System.DateTime? ApprovedDate { get; set; }

        [Column(Order = 39), StringLength(20)]
        public virtual string ApprovalFlow { get; set; }

        [Column(Order = 40)]
        public virtual byte? TransStatus { get; set; }

        [Column(Order = 41)]
        public virtual byte? TransVoid { get; set; }

        [Column(Order = 42)]
        public virtual byte? Posted { get; set; }

        [Column(Order = 43), MaxLength(16)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual System.Guid rowguid { get; set; }

        [Column(Order = 44, TypeName = "datetime")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual System.DateTime? CreateDate { get; set; }

        [Column(Order = 45), StringLength(20)]
        public virtual string CreateBy { get; set; }

        [Column(Order = 46, TypeName = "datetime")]
        public virtual System.DateTime? LastUpdate { get { return _datetimenow; } set { _datetimenow = DateTime.Now; } }

        [Column(Order = 47), StringLength(20)]
        public virtual string UpdateBy { get; set; }

        [Column(Order = 48, TypeName = "datetime")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual System.DateTime? SyncCreate { get; set; }

        [Column(Order = 49, TypeName = "datetime")]
        public virtual System.DateTime? SyncLastUpd { get { return _datetimenow; } set { _datetimenow = DateTime.Now; } }

        [Column(Order = 50)]
        public virtual byte? IsHost { get; set; }

        [Column(Order = 51), StringLength(20)]
        public virtual string LastSyncBy { get; set; }

    }
    #endregion

    #region TENDER
    [Table("TENDER")]
    public class TENDER
    {
        [NotMapped]
        private DateTime _datetimenow;

        [Key]
        [Column(Order = 0)]
        [Required, StringLength(10)]
        public virtual string TenderID { get; set; }

        [Column(Order = 1)]
        public virtual byte? TenderType { get; set; }

        [Column(Order = 2), StringLength(50)]
        public virtual string TenderDesc { get; set; }

        [Column(Order = 3), StringLength(3)]
        public virtual string CurrencyCode { get; set; }

        [Column(Order = 4), StringLength(50)]
        public virtual string TenderPrompt { get; set; }

        [Column(Order = 5), StringLength(50)]
        public virtual string RefPrompt { get; set; }

        [Column(Order = 6), MaxLength(9)]
        public virtual decimal? DefValue { get; set; }

        [Column(Order = 7), MaxLength(9)]
        public virtual decimal? Pickup1 { get; set; }

        [Column(Order = 8), MaxLength(9)]
        public virtual decimal? Pickup2 { get; set; }

        [Column(Order = 9), MaxLength(9)]
        public virtual decimal? MinTenderAmt { get; set; }

        [Column(Order = 10), MaxLength(9)]
        public virtual decimal? MaxTenderAmt { get; set; }

        [Column(Order = 11)]
        public virtual byte? AllowPickup { get; set; }

        [Column(Order = 12)]
        public virtual byte? AllowFloat { get; set; }

        [Column(Order = 13)]
        public virtual byte? AllowOverTender { get; set; }

        [Column(Order = 14)]
        public virtual byte? OpenDrawer { get; set; }

        [Column(Order = 15)]
        public virtual byte? TrackRefNo { get; set; }

        [Column(Order = 16)]
        public virtual byte? TrackClrDate { get; set; }

        [Column(Order = 17, TypeName = "datetime")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual System.DateTime? CreateDate { get; set; }

        [Column(Order = 18), StringLength(20)]
        public virtual string CreateBy { get; set; }

        [Column(Order = 19, TypeName = "datetime")]
        public virtual System.DateTime? LastUpdate { get { return _datetimenow; } set { _datetimenow = DateTime.Now; } }

        [Column(Order = 20), StringLength(20)]
        public virtual string UpdateBy { get; set; }

        [Column(Order = 21)]
        public virtual byte? Active { get; set; }

        [Column(Order = 22)]
        public virtual byte? Inuse { get; set; }

        [Column(Order = 23)]
        public virtual byte? Flag { get; set; }

        [Column(Order = 24)]
        public virtual byte? IsHost { get; set; }

        [Column(Order = 25), MaxLength(16)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual System.Guid rowguid { get; set; }

        [Column(Order = 26, TypeName = "datetime")]
        public virtual System.DateTime? EffDate { get; set; }

        [Column(Order = 27, TypeName = "datetime")]
        public virtual System.DateTime? EndDate { get; set; }

        [Column(Order = 28, TypeName = "datetime")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual System.DateTime? SyncCreate { get; set; }

        [Column(Order = 29, TypeName = "datetime")]
        public virtual System.DateTime? SyncLastUpd { get { return _datetimenow; } set { _datetimenow = DateTime.Now; } }

        [Column(Order = 30)]
        public virtual byte? LastSyncBy { get; set; }

        [Column(Order = 31)]
        public virtual byte? TrackAppCode { get; set; }

        [Column(Order = 32), Range(0, 4)]
        public virtual int? TID { get; set; }

    }
    #endregion

    #region TENDERTYPE
    [Table("TENDERTYPE")]
    public class TENDERTYPE
    {
        [NotMapped]
        private DateTime _datetimenow;

        [Key]
        [Column(Order = 0)]
        [Required,]
        public virtual byte TenderType { get; set; }

        [Required, Column(Order = 1), StringLength(20)]
        public virtual string TenderTypeDesc { get; set; }

        [Column(Order = 2, TypeName = "datetime")]
        public virtual System.DateTime? LastUpdate { get { return _datetimenow; } set { _datetimenow = DateTime.Now; } }

        [Required, Column(Order = 3)]
        public virtual byte Active { get; set; }

        [Required, Column(Order = 4), MaxLength(16)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual System.Guid rowguid { get; set; }

        [Required, Column(Order = 5, TypeName = "datetime")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual System.DateTime SyncCreate { get; set; }

        [Required, Column(Order = 6, TypeName = "datetime")]
        public virtual System.DateTime SyncLastUpd { get { return _datetimenow; } set { _datetimenow = DateTime.Now; } }

        [Required, Column(Order = 7)]
        public virtual byte IsHost { get; set; }

    }
    #endregion

    #region PROCTASK
    [Table("PROCTASK")]
    public class PROCTASK
    {
        [NotMapped]
        private DateTime _datetimenow;

        [Key]
        [Column(Order = 0)]
        [Required, StringLength(20)]
        public virtual string ProSegID { get; set; }

        [Key]
        [Column(Order = 1)]
        [Required, Range(0, 8)]
        public virtual long? TaskID { get; set; }

        [Key]
        [Column(Order = 2)]
        [Required, StringLength(20)]
        public virtual string AgentID { get; set; }

        [Key]
        [Column(Order = 3)]
        [Required,]
        public virtual byte? TaskType { get; set; }

        [Key]
        [Column(Order = 4)]
        [Required, StringLength(3)]
        public virtual string SubType { get; set; }

        [Key]
        [Column(Order = 5, TypeName = "datetime")]
        [Required]
        public virtual System.DateTime? TaskStartDate { get; set; }

        [Column(Order = 6, TypeName = "datetime")]
        public virtual System.DateTime? TaskEndDate { get; set; }

        [Column(Order = 7), StringLength(4000)]
        public virtual string TaskValue1 { get; set; }

        [Column(Order = 8), StringLength(4000)]
        public virtual string TaskValue2 { get; set; }

        [Column(Order = 9)]
        public virtual byte? Status { get; set; }

        [Column(Order = 10), StringLength(4000)]
        public virtual string Result { get; set; }

        [Column(Order = 11), StringLength(50)]
        public virtual string BatchNo { get; set; }

        [Column(Order = 12), StringLength(200)]
        public virtual string MsgID { get; set; }

        [Column(Order = 13), StringLength(20)]
        public virtual string TransID { get; set; }

        [Column(Order = 14), StringLength(20)]
        public virtual string RecordLocator { get; set; }

        [Column(Order = 15), StringLength(50)]
        public virtual string QueueCode { get; set; }

        [Column(Order = 16, TypeName = "datetime")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual System.DateTime? CreatedDate { get; set; }

        [Column(Order = 17)]
        public virtual byte? EmailType { get; set; }

        [Column(Order = 18), StringLength(266)]
        public virtual string EmailAddress { get; set; }

        [Column(Order = 19, TypeName = "datetime")]
        public virtual System.DateTime? ExpiryDate { get; set; }

        [Column(Order = 20), StringLength(3)]
        public virtual string Currency { get; set; }

        [Column(Order = 21), MaxLength(9)]
        public virtual decimal? BalanceDue { get; set; }

        [Column(Order = 22), MaxLength(9)]
        public virtual decimal? PaymentAmt { get; set; }

        [Column(Order = 23), MaxLength(9)]
        public virtual decimal? TransTotalAmt { get; set; }

        [Column(Order = 24)]
        public virtual byte? AttemptCountSender { get; set; }

        [Column(Order = 25, TypeName = "datetime")]
        public virtual System.DateTime? AttemptCountSenderDate { get; set; }

        [Column(Order = 26)]
        public virtual byte? IsSuccess { get; set; }

        [Column(Order = 27, TypeName = "datetime")]
        public virtual System.DateTime? FinishedDate { get; set; }

        [Column(Order = 28), StringLength(255)]
        public virtual string FailedRemark { get; set; }

        [Column(Order = 29), StringLength(100)]
        public virtual string ApprovedBy { get; set; }

        [Column(Order = 30, TypeName = "datetime")]
        public virtual System.DateTime? ApprovedDate { get; set; }

        [Column(Order = 31), StringLength(100)]
        public virtual string UpdatedBy { get; set; }

        [Column(Order = 32, TypeName = "datetime")]
        public virtual System.DateTime? UpdatedDate { get { return _datetimenow; } set { _datetimenow = DateTime.Now; } }

        [Column(Order = 33)]
        public virtual byte? IsDeleted { get; set; }

        [Column(Order = 34, TypeName = "datetime")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual System.DateTime? SyncCreate { get; set; }

        [Column(Order = 35, TypeName = "datetime")]
        public virtual System.DateTime? SyncLastUpd { get { return _datetimenow; } set { _datetimenow = DateTime.Now; } }

        [Column(Order = 36), MaxLength(16)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual System.Guid rowguid { get; set; }

        [Column(Order = 37)]
        public virtual byte? Flag { get; set; }

    }
    #endregion

    #region INVOICEHDR_OLD
    [Table("INVOICEHDR_OLD")]
    //[Audited]
    public class INVOICEHDR_OLD : BaseDto<INVOICEHDR_OLD>
    {
        [Key]
        [MaxLength(10), Required]
        public virtual string MonthCode { get; set; }
        [Key]
        [MaxLength(20), Required]
        public virtual string PBTCode { get; set; }
        [Key]
        [MaxLength(20), Required]
        public virtual string BatchCode { get; set; }
        [Range(0, 9999), Required]
        public virtual int? TotalRec1 { get; set; }
        [Range(0, 9999), Required]
        public virtual int? TotalRec2 { get; set; }
        [Range(0, 9999), Required]
        public virtual int? TotalRec3 { get; set; }
        [Range(0, 9999), Required]
        public virtual int? TotalRec4 { get; set; }
        [Range(0, 9999), Required]
        public virtual int? TotalRec5 { get; set; }
        [Required]
        public virtual decimal? TotalAmt1 { get; set; }
        [Required]
        public virtual decimal? TotalAmt2 { get; set; }
        [Required]
        public virtual decimal? TotalAmt3 { get; set; }
        [Required]
        public virtual decimal? TotalAmt4 { get; set; }
        [Required]
        public virtual decimal? TotalAmt5 { get; set; }
        [Required]
        public virtual byte? Status { get; set; }
    }
    #endregion

    #region INVOICEITEM
    [Table("INVOICEITEM")]
    [Audited]
    public class INVOICEITEM : BaseDto<INVOICEITEM>
    {
        [Key]
        [MaxLength(10), Required]
        public virtual string MonthCode { get; set; }
        [Key]
        [MaxLength(20), Required]
        public virtual string PBTCode { get; set; }
        [Key]
        [MaxLength(20), Required]
        public virtual string BizRegID { get; set; }
        [Key]
        [MaxLength(20), Required]
        public virtual string BatchCode { get; set; }
        [Key]
        [Range(0, 9999), Required]
        public virtual int? SeqNo { get; set; }
        [MaxLength(200), Required]
        public virtual string CompanyName { get; set; }
        [MaxLength(200), Required]
        public virtual string ContractID { get; set; }
        [Column(TypeName = "datetime")]
        public virtual System.DateTime? InvDate { get; set; }
        [Column(TypeName = "datetime")]
        public virtual System.DateTime? ExpiryDate { get; set; }
        [MaxLength(3), Required]
        public virtual string ItemType { get; set; }
        [MaxLength(20), Required]
        public virtual string ItemCode { get; set; }
        [MaxLength(4000), Required]
        public virtual string ItemDesc { get; set; }
        [Required]
        public virtual decimal? Amount1 { get; set; }
        [Required]
        public virtual decimal? Amount2 { get; set; }
        [MaxLength(4000), Required]
        public virtual string Remark1 { get; set; }
        [MaxLength(4000), Required]
        public virtual string Remark2 { get; set; }
        [MaxLength(10), Required]
        public virtual string SvcType { get; set; }
        [MaxLength(10), Required]
        public virtual string ProcType { get; set; }
        [MaxLength(200), Required]
        public virtual string ErrCode { get; set; }
        [MaxLength(3), Required]
        public virtual string Currency { get; set; }
        [MaxLength(4000), Required]
        public virtual string FilePath { get; set; }
        [MaxLength(20), Required]
        public virtual string LastUpdateBy { get; set; }
        [Required]
        public virtual byte? Status { get; set; }
    }
    #endregion

    #region TRANSCDHDR
    [Table("TRANSCDHDR")]
    //[Audited]
    public class TRANSCDHDR : BaseDto<TRANSCDHDR>
    {
        [Key]
        [MaxLength(10), Required]
        public virtual string MonthCode { get; set; }
        [Key]
        [MaxLength(20), Required]
        public virtual string PBTCode { get; set; }
        [Key]
        [MaxLength(20), Required]
        public virtual string BatchCode { get; set; }
        [MaxLength(3), Required]
        public virtual string TransType { get; set; }
        [MaxLength(4000)]
        public virtual string Remark { get; set; }
        [Range(0, 9999), Required]
        public virtual int? TotalRec1 { get; set; }
        [Range(0, 9999), Required]
        public virtual int? TotalRec2 { get; set; }
        [Range(0, 9999), Required]
        public virtual int? TotalRec3 { get; set; }
        [Range(0, 9999), Required]
        public virtual int? TotalRec4 { get; set; }
        [Range(0, 9999), Required]
        public virtual int? TotalRec5 { get; set; }
        [Required]
        public virtual decimal? TotalAmt1 { get; set; }
        [Required]
        public virtual decimal? TotalAmt2 { get; set; }
        [Required]
        public virtual decimal? TotalAmt3 { get; set; }
        [Required]
        public virtual decimal? TotalAmt4 { get; set; }
        [Required]
        public virtual decimal? TotalAmt5 { get; set; }
        [Required]
        public virtual byte? Status { get; set; }
    }
    #endregion

    #region TRANSCDITEM
    [Table("TRANSCDITEM")]
    //[Audited]
    public class TRANSCDITEM : BaseDto<TRANSCDITEM>
    {
        [Key]
        [MaxLength(10), Required]
        public virtual string MonthCode { get; set; }
        [Key]
        [MaxLength(20), Required]
        public virtual string PBTCode { get; set; }
        [Key]
        [MaxLength(20), Required]
        public virtual string BizRegID { get; set; }
        [Key]
        [MaxLength(20), Required]
        public virtual string BatchCode { get; set; }
        [Key]
        [Range(0, 9999), Required]
        public virtual int? SeqNo { get; set; }
        [MaxLength(200), Required]
        public virtual string CompanyName { get; set; }
        [MaxLength(200), Required]
        public virtual string ContractID { get; set; }
        [Column(TypeName = "datetime")]
        public virtual System.DateTime? CNDNDate { get; set; }
        [MaxLength(3), Required]
        public virtual string ItemType { get; set; }
        [MaxLength(20), Required]
        public virtual string ItemCode { get; set; }
        [MaxLength(4000), Required]
        public virtual string ItemDesc { get; set; }
        [Required]
        public virtual decimal? Amount1 { get; set; }
        [Required]
        public virtual decimal? Amount2 { get; set; }
        [MaxLength(4000), Required]
        public virtual string Remark1 { get; set; }
        [MaxLength(4000), Required]
        public virtual string Remark2 { get; set; }
        [MaxLength(10), Required]
        public virtual string SvcType { get; set; }
        [MaxLength(10), Required]
        public virtual string ProcType { get; set; }
        [MaxLength(200), Required]
        public virtual string ErrCode { get; set; }
        [Required]
        public virtual byte? Status { get; set; }
        [MaxLength(3), Required]
        public virtual string Currency { get; set; }
        [MaxLength(4000), Required]
        public virtual string FilePath { get; set; }
    }
    #endregion
}