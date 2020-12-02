using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plexform.MultiTenancy;
using Plexform.Notifications;
using Abp.Web.Models;
using LOGIC.Base;
using Abp.Domain.Repositories;
using Plexform;
using Microsoft.AspNetCore.Hosting;
using Plexform.Authorization.Roles;
using Abp.UI;
using Abp.ObjectMapping;
using Castle.Core.Logging;
using LOGIC.Shared.Collection;
using LOGIC.Shared;
using System.Collections.Generic;
using Plexform.Base.Core.Entity;
using Plexform.Base.Core.General;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using System.Net;
using System.Security.Authentication;
using System.Collections.Specialized;
using System.Web;
using System.Transactions;
using LOGIC.Repo;
using Abp.Localization;
using Plexform.Authorization.Users;
using Plexform.Base.Core.User;
using Plexform.Base.Core.Helper;
using Plexform.Base.Core;
using Plexform.Base.Accounting.Map;
using Plexform.Base.Accounting.Repo;
using Plexform.Base.Accounting.Filter;
using Plexform.Base.Core.Entity.Repo;
using Plexform.Base.Core.General.Repo;
using Microsoft.EntityFrameworkCore;
using Plexform.Base.Accounting.XML;
using Plexform.Base.Core.Entity.Model;
using Abp.AspNetCore.Mvc.Authorization;
using Hangfire;

namespace Plexform.Base.Accounting.Controller
{
    [Route("api/[controller]/[action]")]
    public class AccountingController : BaseController
    {
        #region Declaration
        public ILogger Log { get; set; }
        private readonly IRealTimeCommunicator _realtimeService;
        private readonly IPlexformNotifier _plexformNotifier;
        private readonly ILocalizationManager _localizationManager;
        private readonly ITxtExporter _txtExporter;
        private readonly RoleManager _roleManager;
        private readonly UserManager _userManager;
        private readonly AccountingManager _accountingManager;
        private readonly AccountingEmailer _accountingEmailer;
        private readonly GeneralManager _generalManager;
        private readonly Plexform.INT.SAP.KA.IntegrationManager _sapManager;
        #endregion Declaration

        public AccountingController(
            IRepository<Role> roleRepository,
            IAppNotifier appNotifier,
            IRealTimeCommunicator realtimeService,
            IPlexformNotifier plexformNotifier,
            ICoreEmailer plexformEmailer,
            IWebHostEnvironment env,
            IAppFolders appFolders,
            IObjectMapper objectMapper,
            ILocalizationManager localizationManager,
            IExcelExporter excelExporter,
            ITxtExporter txtExporter,
            BizUserManager bizUser,
            UserManager userManager,
            TenantManager tenantManager,
            RoleManager roleManager,
            AccountingManager accountingManager,
            AccountingEmailer accountingEmailer,
            GeneralManager generalManager,
            Plexform.INT.SAP.KA.IntegrationManager sapManager
            ) : base(roleRepository, appNotifier, env, appFolders, objectMapper, tenantManager, excelExporter, localizationManager)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                Log = NullLogger.Instance;
                _realtimeService = realtimeService;
                _plexformNotifier = plexformNotifier;
                _localizationManager = localizationManager;
                _roleManager = roleManager;
                _userManager = userManager;
                _accountingManager = accountingManager;
                _accountingEmailer = accountingEmailer;
                _generalManager = generalManager;
                _txtExporter = txtExporter;
                _sapManager = sapManager;
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingController), ex);
            }
        }

        #region Public
        #region HangFire Job All Outstanding
        [HttpGet]
        //[AbpMvcAuthorize] Public Service
        public async Task<JsonResult> JobGetAllOutstanding(string customerCode, Int32 limit, Int32 offset)
        {
            try
            {
                var result = await _accountingManager.JobGetAllOutstanding(customerCode, limit, offset);
                return Json(result);
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpGet]
        //[AbpMvcAuthorize] Public Service
        public async Task<JsonResult> ScheduleJobGetAllOutstanding()
        {
            try
            {
                var result = await _accountingManager.ScheduleJobGetAllOutstanding();
                return Json(result);
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }
        #endregion

        #region Transaction
        [HttpPost]
        //[AbpMvcAuthorize]
        public async Task<JsonResult> UpdateTransaction([FromBody] string TransNo)
        {
            try
            {
                var result = await _accountingManager.UpdateTransaction(TransNo);
                return Json(result);
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new { success = false }));
            }
        }

        [HttpPost]
        //[AbpMvcAuthorize]
        public async Task<JsonResult> JobUpdateTransaction([FromBody] string TransNo)
        {
            try
            {
                var result = BackgroundJob.Enqueue(() => _accountingManager.UpdateTransaction(TransNo));
                return Json(result);
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new { success = false }));
            }
        }
        #endregion

        [HttpGet]
        public async Task<JsonResult> PaymentSuccessNotification(string emailAddress, string paymentMethod, string totalAmount, string companyName, string transno)
        {
            try
            {
                await _accountingEmailer.PaymentSuccess(emailAddress, paymentMethod, totalAmount, companyName, transno);
                return Json(true);
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpGet]
        public async Task<JsonResult> ResolveTransNo(string trx)
        {
            try
            {
                var result = GeneratorHelper.DecryptQueryParam(trx, "transno");
                return Json(result);
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        #region Payment
        #region FPX
        [HttpGet]
        //[AbpMvcAuthorize]
        public async Task<JsonResult> GetFPXBankList(System.String fpx_msgToken)
        {
            try
            {
                var result = await _accountingManager.GetFPXBankList(fpx_msgToken);
                return Json(result);
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new { success = false }));
            }
        }

        [HttpPost]
        //[AbpMvcAuthorize]
        public async Task<JsonResult> SaveBankListToFile()
        {
            try
            {
                var result = await _accountingManager.SaveBankListToFile();
                return Json(result);
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpGet]
        //[AbpMvcAuthorize]
        public async Task<JsonResult> ValidateFPXStatus(System.String refInfo)
        {
            try
            {
                var result = await _accountingManager.ValidateFPXStatus(refInfo);
                return Json(result);
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new { success = false }));
            }
        }

        [HttpPost]
        //[AbpMvcAuthorize]
        public async Task<JsonResult> GetFPXCheckSum([FromBody] MapFPXBody fpxBody)
        {
            try
            {
                var result = await _accountingManager.GetFPXCheckSum(fpxBody);
                return Json(result);
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new { success = false }));
            }
        }
        #endregion

        [HttpGet]
        //[AbpMvcAuthorize]
        public async Task<JsonResult> GetPaymentHistoryDetails(string transNo)
        {
            try
            {
                var result = await _accountingManager.GetPaymentHistoryDetails(transNo);
                return Json(result);
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpGet]
        //[AbpMvcAuthorize]
        public async Task<JsonResult> GetResponseDetails(string transNo, bool creditCard, bool offPay)
        {
            try
            {
                var result = await _accountingManager.GetResponseDetails(transNo, creditCard, offPay);
                return Json(result);
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        #region MayBank
        [HttpGet]
        //[AbpMvcAuthorize]
        public async Task<JsonResult> validateMBBStatus(System.String refInfo)
        {
            try
            {
                var result = await _accountingManager.ValidateMBBStatus(refInfo);
                return Json(result);
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new { success = false }));
            }
        }

        [HttpGet]
        //[AbpMvcAuthorize]
        public async Task<JsonResult> GetMBBSignature(string merchantID, string amount)
        {
            try
            {
                var result = await _accountingManager.GetMBBSignature(merchantID, amount);
                return Json(result);
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new { success = false }));
            }
        }

        [HttpPost]
        //[AbpMvcAuthorize]
        public async Task<IActionResult> MBCCGetResponse([FromForm] MBCCResponse data)
        {
            try
            {
                var result = await _accountingManager.MBCCGetResponse(data);
                Response.Redirect(_appConfiguration["App:ClientRootAddress"] + "account/payment-response?orderNo=" + data.MERCHANT_TRANID + "&creditCard=true");
                return Json(result);
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }
        #endregion
        #endregion

        #region Customer
        [HttpGet]
        //[AbpMvcAuthorize]
        public async Task<JsonResult> GetCustomer(GetCustomerSunDB input)
        {
            try
            {
                var result = await _accountingManager.GetCustomer(input);
                return Json(result);
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }
        #endregion
        #endregion

        #region Apps
        #region Invoice
        [HttpGet]
        [AbpMvcAuthorize]
        public async Task<JsonResult> GetDashboardInvoice(GetListParameter input, string customerCode)
        {
            try
            {
                var result = await _accountingManager.GetDashboardInvoice(input, customerCode);
                return Json(result);
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpGet]
        [AbpMvcAuthorize]
        public async Task<JsonResult> GetInvoice(InvoiceFilter input)
        {
            try
            {
                var result = await _accountingManager.GetInvoice(input);
                return Json(result);
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpGet]
        [AbpMvcAuthorize]
        public async Task<JsonResult> GetInvoiceDetails(string transNo, byte status, byte detail)
        {
            try
            {
                var result = await _accountingManager.GetInvoiceDetails(transNo, status, detail);
                return Json(result);
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        #endregion

        #region Transaction
        [HttpPost]
        [AbpMvcAuthorize]
        public async Task<JsonResult> AddTransaction([FromBody] Accounting.Map.Payment json)
        {
            try
            {
                var result = await _accountingManager.AddTransaction(json);
                return Json(result);
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }
        #endregion

        #region Payment History
        [HttpGet]
        [AbpMvcAuthorize]
        public async Task<JsonResult> GetTransactionHistory(GetListParameter input, string customerID, byte mode)
        {
            try
            {
                var result = await _accountingManager.GetTransactionHistory(input, customerID, mode);
                return Json(result);
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }
        #endregion

        #region Tender --Should be Not here, move to Inventory
        [HttpGet]
        [AbpMvcAuthorize]
        public async Task<JsonResult> GetTenderList(GetListParameter input)
        {
            try
            {
                var result = await _accountingManager.GetTenderList(input);
                return Json(result);
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpGet]
        [AbpMvcAuthorize]
        public async Task<JsonResult> GetPaymentMethodCombo()
        {
            try
            {
                var result = await _accountingManager.GetPaymentMethodCombo();
                return Json(result);
            }

            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        #endregion

        #region Generate Doc Code
        [HttpGet]
        [AbpMvcAuthorize]
        public async Task<JsonResult> GenerateDocCode(string Branch, string TypeCode)
        {
            try
            {
                var result = await _accountingManager.GenerateDocCode(Branch, TypeCode);
                return Json(result);
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }
        #endregion

        #region TXT
        [HttpGet]
        [AbpMvcAuthorize]
        public async Task<JsonResult> GetTxt(string transNo, float TaxAmt, float Rounding)
        {
            try
            {
                var result = await _accountingManager.GetTxt(transNo, TaxAmt, Rounding);
                return Json(result);
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpGet]
        [AbpMvcAuthorize]
        public async Task<JsonResult> GetTextFileString(string transNo, float TaxAmt, float Rounding)
        {
            try
            {
                var result = await _accountingManager.GetTextFileString(transNo);
                return Json(result);
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }
        #endregion

        #region Report
        [HttpGet]
        [AbpMvcAuthorize]
        public async Task<JsonResult> GetTransactionReport(TransReport input)
        {
            try
            {
                var result = await _accountingManager.GetTransactionReport(input);
                return Json(result);
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        #endregion
        #endregion

        #region SUNDB Experimental
        //[HttpGet]
        //public async Task<JsonResult> GetCreditBills(Int32 isView, GetCreditBills input)
        //{
        //    try
        //    {
        //        var scheduleLogic = new PAYMENT(_env, _connectionString);
        //        scheduleLogic.Select<MapListCreditBills>(x =>
        //            new
        //            {
        //                x.Description,
        //                x.BillNo,
        //                x.TReference,
        //                x.D_C,
        //                x.Add_Code,
        //                x.Rec_Type,
        //                x.Trans_Date,
        //                x.Amount,
        //                x.Accnt_Code,
        //                x.Jrnal_No,
        //                x.Trans_Ref,
        //                x.Sun_Db,
        //                x.Allocation,
        //                x.Inv_Date,
        //                x.Status,
        //                x.Comments,
        //                x.Cust_Code,
        //                x.Trans_Val,
        //                x.Del_Date,
        //                x.Anal_T1,
        //                x.Cust_Ref,
        //                x.Address_1,
        //                x.Address_2,
        //                x.Address_3,
        //                x.Address_4,
        //                x.Address_5,
        //                x.Address_6,
        //                x.E_Mail
        //            });

        //        if (isView == 1)
        //        {
        //            var data = scheduleLogic.StoredProcedureView<GetCreditBills, MapListCreditBills>("dbo.GetCreditBills", input);
        //            data.TotalCount = data.Items.Count();
        //            return Json(new AjaxResponse(data));
        //        }
        //        else
        //        {
        //            var dataMap = scheduleLogic.StoredProcedureViewMap<MapListCreditBills, GetCreditBills>("dbo.GetCreditBills", input).ToList();
        //            return Json(new AjaxResponse(dataMap));
        //        }
        //    }
        //    catch (UserFriendlyException ex)
        //    {
        //        Log.Error(nameof(AccountingController), ex);
        //        return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
        //    }
        //}

        //[HttpGet]
        //public async Task<JsonResult> getCreditBillsDetail(Int32 detail, GetCreditBillDetails input)
        //{
        //    try
        //    {
        //        var scheduleLogic = new PAYMENT(_env, _connectionString);
        //        scheduleLogic.Select<MapListCreditBillsDetail>(x =>
        //            new
        //            {
        //                x.REC_TYPE,
        //                x.TRANS_REF,
        //                x.INV_NO,
        //                x.ACCNT_CODE,
        //                x.DESCRIPTN,
        //                x.VALUE_1,
        //                x.VALUE_2,
        //                x.VALUE_10,
        //                x.INV_DATE,
        //                x.ID_ENTERED,
        //                x.ID_INVOICED,
        //                x.ANAL_M1,
        //                x.ANAL_M3,
        //                x.ANAL_M6
        //            });
        //        if (detail == 1)
        //        {
        //            var data = scheduleLogic.StoredProcedureView<GetCreditBillDetails, MapListCreditBillsDetail>("dbo.GetCreditBillsDetail", input);
        //            data.TotalCount = data.Items.Count();
        //            return Json(new AjaxResponse(data));
        //        }
        //        else
        //        {
        //            var dataMap = scheduleLogic.StoredProcedureViewMap<MapListCreditBillsDetail, GetCreditBillDetails>("dbo.GetCreditBillsDetail", input).ToList();
        //            return Json(new AjaxResponse(dataMap));
        //        }
        //    }
        //    catch (UserFriendlyException ex)
        //    {
        //        Log.Error(nameof(AccountingController), ex);
        //        return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
        //    }
        //}

        //[HttpGet]
        //public async Task<JsonResult> getCreditBillsTax(Int32 isView, GetCreditBillDetails input)
        //{
        //    try
        //    {
        //        var scheduleLogic = new PAYMENT(_env, _connectionString);
        //        scheduleLogic.Select<MapCreditBillTax>(x =>
        //            new
        //            {
        //                x.AMOUNT
        //            });
        //        if (isView == 1)
        //        {
        //            var data = scheduleLogic.StoredProcedureView<GetCreditBillDetails, MapCreditBillTax>("dbo.GetCreditBillsTax", input);
        //            data.TotalCount = data.Items.Count();
        //            return Json(new AjaxResponse(data));
        //        }
        //        else
        //        {
        //            var dataMap = scheduleLogic.StoredProcedureViewMap<MapCreditBillTax, GetCreditBillDetails>("dbo.GetCreditBillsTax", input).ToList();
        //            return Json(new AjaxResponse(dataMap));
        //        }
        //    }
        //    catch (UserFriendlyException ex)
        //    {
        //        Log.Error(nameof(AccountingController), ex);
        //        return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
        //    }
        //}
        #endregion

        #region FpxPayment -- Will Delete it Soon
        //[HttpPost]
        //public async Task<JsonResult> AddFPXPayment([FromBody] FPXPayment json)
        //{
        //    try
        //    {
        //        var dataTransHDR = _objectMapper.Map<DTO.Accounting.TRANSHDR>(json.TransHdr);
        //        var dataTransDTL = _objectMapper.Map<IList<DTO.Accounting.TRANSDTL>>(json.TransDtl);
        //        var dataTransFPX = _objectMapper.Map<DTO.Accounting.TRANSFPX>(json.TransFpx);
        //        var transHDRLogic = new TRANSHDR(_env, _connectionString);
        //        var transDTLLogic = new TRANSDTL(_env, _connectionString);
        //        var transFPXLogic = new TRANSFPX(_env, _connectionString);
        //        var invoiceHDRLogic = new INVOICEHDR(_env, _connectionString);
        //        //var invoiceITEMLogic = new INVOICEITEM(_env, _connectionString);
        //        var invoiceDTLLogic = new INVOICEDTL(_env, _connectionString);

        //        await transHDRLogic.Create(dataTransHDR);

        //        foreach (var row in dataTransDTL)
        //        {
        //            await transDTLLogic.Create(row);
        //        }

        //        await transFPXLogic.Create(dataTransFPX);

        //        var PBTCode = json.InvoiceID;

        //        var invoiceHDR = invoiceHDRLogic.Retrieve(x => x.TransNo == PBTCode);
        //        if (invoiceHDR != null)
        //        {
        //            invoiceHDR.LastUpdate = DateTime.Now;
        //            invoiceHDR.Inuse = 1;

        //            await invoiceHDRLogic.Update(invoiceHDR);
        //        }

        //        var invoiceDTL = invoiceDTLLogic.Retrieve(x => x.TransNo == PBTCode);
        //        if (invoiceDTL != null)
        //        {
        //            invoiceDTL.LastUpdate = DateTime.Now;
        //            invoiceDTL.Inuse = 1;

        //            await invoiceDTLLogic.Update(invoiceDTL);
        //        }

        //        return Json(new AjaxResponse(new { success = true }));
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(nameof(AccountingController), ex);
        //        return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
        //    }
        //}

        //[HttpPost]
        //public async Task<JsonResult> FpxPayment([FromBody] FPXSender json)
        //{
        //    try
        //    {
        //        string key_file = _CertificateDir + SellerExId + ".key"; // Server.MapPath("~/res/fpx/MercKey.key");
        //        string cert_file = _CertificateDir + SellerExId + ".cer"; // Server.MapPath("~/res/fpx/ExCert.cer");

        //        FPXCertificateController.Initialize(key_file, new SingleFileCertificateProvider(cert_file));

        //        var fpx_merch = new FPXMerchant() { MerchantID = _SellerId, ExchangeID = _SellerExId, BankCode = "01" }; // { MerchantID = "[MercID]", ExchangeID = "[Exchange Code]", BankCode = "01" };
        //        var fpx_buyer = new FPXBuyer()
        //        {
        //            BuyerEmail = json.fpx_buyerEmail,
        //            BuyerID = json.fpx_buyerId,
        //            BuyerName = json.fpx_buyerName,
        //            BuyerIBAN = json.fpx_buyerIban,
        //            BuyerAccountNo = json.fpx_buyerAccNo,
        //            BuyerBankID = json.fpx_sellerBankCode
        //        };
        //        var fpx_transaction = new FPXTransaction(fpx_merch)
        //        {
        //            Amount = Convert.ToDecimal(json.fpx_txnAmount), // 120.45m, // Amount
        //            Currency = FPXSupportedCurrencies.MYR,
        //            OrderNo = json.fpx_sellerOrderNo, // "[Order No]",
        //            Date = DateTime.Today,
        //            ProductDescription = json.fpx_productDesc, // "[Description]",
        //            Buyer = fpx_buyer,
        //            ExOrderNo = json.fpx_sellerExOrderNo // "[Extra Order No]"
        //        };
        //        var fpx_message = new FPXMessage(fpx_transaction);


        //        //var fpx_res = new FPXResponse(this);
        //        //if (fpx_res.PaymentIsSuccessful)
        //        //{
        //        //    // successful transaction           
        //        //}
        //        //else
        //        //{
        //        //    // transaction failed
        //        //}

        //        return Json(new AjaxResponse(fpx_message));

        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(nameof(AccountingController), ex);
        //        return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
        //    }
        //}

        //[HttpPost]
        //public async Task<JsonResult> updatePayment([FromBody] POSTUpdatePayment data)
        //{
        //    Envelope requestEnvelope = new Envelope();
        //    Envelope responseEnvelope = new Envelope();

        //    try
        //    {
        //        requestEnvelope = new Envelope
        //        {
        //            Body = new Body
        //            {
        //                UpdatePayment = new UpdatePayment
        //                {
        //                    AuthenticationDTO = new AuthenticationDTO
        //                    {
        //                        Username = _appConfiguration["SUNSystem:Username"],
        //                        Password = _appConfiguration["SUNSystem:Password"]
        //                    },
        //                    Email = data.Email,
        //                    PaymentDTO = new PaymentDTO
        //                    {
        //                        Amount = data.Amount,
        //                        BillId = data.BillID,
        //                        Reference = data.Reference
        //                    }
        //                }
        //            }
        //        };

        //        var sunSystem = new SUNSystemConnection();
        //        responseEnvelope = await sunSystem.XMLSender("updatePayment", requestEnvelope, nameof(updatePayment), nameof(updatePayment));
        //        return Json(new AjaxResponse(responseEnvelope));
        //    }
        //    catch (UserFriendlyException ex)
        //    {
        //        Log.Error(nameof(AccountingController), ex);
        //        return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
        //    }
        //}
        #endregion

        #region FPX No Longer Use -- Will Delete it Soon
        //[HttpPost]
        //public async Task<IActionResult> FpxGetResponse([FromForm] FPXResponse data)
        //{
        //    try
        //    {
        //        string tempBizRegID;
        //        string tempBizLocID;

        //        var bizEntityLogic = new Core.Entity.Repo.BIZENTITY(_env, _connectionString);
        //        var bizLocateLogic = new Core.Entity.Repo.BIZLOCATE(_env, _connectionString);
        //        var transFPXLogic = new TRANSFPX(_env, _connectionString);
        //        var transHDRLogic = new TRANSHDR(_env, _connectionString);
        //        var transDTLLogic = new TRANSDTL(_env, _connectionString);
        //        var transTenderLogic = new TRANSTENDER(_env, _connectionString);

        //        data.fpx_checkSumString = data.fpx_buyerBankBranch + "|" + data.fpx_buyerBankId + "|" + data.fpx_buyerIban + "|" + data.fpx_buyerId + "|" + data.fpx_buyerName + "|";
        //        data.fpx_checkSumString += data.fpx_creditAuthCode + "|" + data.fpx_creditAuthNo + "|" + data.fpx_debitAuthCode + "|" + data.fpx_debitAuthNo + "|" + data.fpx_fpxTxnId + "|";
        //        data.fpx_checkSumString += data.fpx_fpxTxnTime + "|" + data.fpx_makerName + "|" + data.fpx_msgToken + "|" + data.fpx_msgType + "|" + data.fpx_sellerExId + "|" + data.fpx_sellerExOrderNo + "|";
        //        data.fpx_checkSumString += data.fpx_sellerId + "|" + data.fpx_sellerOrderNo + "|" + data.fpx_sellerTxnTime + "|" + data.fpx_txnAmount + "|" + data.fpx_txnCurrency;
        //        data.fpx_checkSumString = data.fpx_checkSumString.Trim();

        //        DateTime trxDate = DateTime.ParseExact(data.fpx_fpxTxnTime.Substring(0, 8), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

        //        var tempTransNo = data.fpx_sellerOrderNo;
        //        var transData = transHDRLogic.Retrieve(x => x.TransNo == tempTransNo);
        //        if (transData != null)
        //        {
        //            tempBizRegID = transData.BizRegID;
        //            tempBizLocID = transData.BizLocID;

        //            var dataTransFPX = new DTO.Accounting.TRANSFPX
        //            {
        //                BizRegID = tempBizRegID,
        //                BizLocID = tempBizLocID,
        //                TermID = 1,
        //                TransNo = data.fpx_sellerOrderNo,
        //                TransSeq = 1,
        //                TransDate = trxDate,
        //                TransTime = data.fpx_fpxTxnTime.Substring(8, 4),
        //                TenderID = data.fpx_debitAuthCode,
        //                TenderAmt = Convert.ToDecimal(data.fpx_txnAmount),
        //                RefInfo = data.fpx_checkSum,
        //                CCrefInfo = data.fpx_checkSumString,
        //                CustName = data.fpx_buyerName,
        //                CreateDate = DateTime.Now,
        //                LastUpdate = DateTime.Now,
        //                SyncLastUpd = DateTime.Now,
        //                Active = 1,
        //                Posted = 1,
        //                Flag = 1,
        //                Inuse = 0,
        //                IsHost = 0,
        //            };

        //            await transFPXLogic.Create(dataTransFPX);

        //            Byte? paidStatus;
        //            Byte? isApproved;
        //            if (data.fpx_debitAuthCode == "00")
        //            {
        //                paidStatus = 2;
        //                isApproved = 1;
        //            }
        //            else if (data.fpx_debitAuthCode == "09" || data.fpx_debitAuthCode == "99")
        //            {
        //                paidStatus = 1;
        //                isApproved = 0;
        //            }
        //            else
        //            {
        //                paidStatus = 3;
        //                isApproved = 0;
        //            }

        //            var trxNo = data.fpx_sellerOrderNo;

        //            var dataTransHDR = transHDRLogic.Retrieve(x => x.TransNo == trxNo);
        //            if (dataTransHDR != null)
        //            {
        //                //dataTransHDR.TransReasonCode = data.fpx_fpxTxnId;
        //                dataTransHDR.LastUpdate = DateTime.Now;
        //                dataTransHDR.Status = paidStatus;
        //                dataTransHDR.ShiftCode = data.fpx_debitAuthCode;
        //                dataTransHDR.TransStatus = data.fpx_debitAuthCode;
        //                await transHDRLogic.Update(dataTransHDR);
        //            }

        //            var dataTransTender = transTenderLogic.Retrieve(x => x.TransNo == trxNo);
        //            if (dataTransTender != null)
        //            {
        //                dataTransTender.IsApproved = isApproved;
        //                dataTransTender.TransStatus = paidStatus;
        //                dataTransTender.ExternalID = data.fpx_fpxTxnId;
        //                dataTransTender.AuthCode = data.fpx_checkSum;
        //                dataTransTender.RespCode = data.fpx_debitAuthCode;
        //                dataTransTender.ApprovedDate = DateTime.Now;
        //                dataTransTender.LastUpdate = DateTime.Now;
        //                dataTransTender.SyncLastUpd = DateTime.Now;
        //                await transTenderLogic.Update(dataTransTender);
        //            }

        //            var dataTransDTL = transDTLLogic.RetrieveAll(x => x.TransNo.Contains(trxNo));
        //            if (dataTransDTL != null && dataTransDTL.Count() > 0)
        //            {
        //                foreach (var index in dataTransDTL)
        //                {
        //                    index.LastUpdate = DateTime.Now;
        //                    index.Status = paidStatus;
        //                    await transDTLLogic.Update(index);
        //                }
        //            }

        //            await UpdateTransaction(tempTransNo);
        //        }
        //        else
        //        {
        //            return Json(new AjaxResponse(new { success = false, message = "Transaction Not Found" }));
        //        }

        //        Response.Redirect(_appConfiguration["App:ClientRootAddress"] + "account/payment-response?orderNo=" + data.fpx_sellerOrderNo);
        //        return Json(new AjaxResponse(new { success = true }));
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(nameof(AccountingController), ex);
        //        return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> FpxDirectGetResponse([FromForm] FPXResponse data)
        //{
        //    try
        //    {
        //        Response.Redirect(_appConfiguration["App:ClientRootAddress"] + "account/response");
        //        return Json(new AjaxResponse(new { success = true }));
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(nameof(AccountingController), ex);
        //        return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
        //    }
        //}
        #endregion

        #region CWMS
        [HttpGet]
        public async Task<JsonResult> GetCreditStatus(string id)
        {
            string result = string.Empty;

            try
            {
                var custList = new List<string>() { id };
                var resCheck = await _sapManager.CheckCustomerCredit(new List<INT.SAP.KA.Model.CheckCustomerCreditRequest>() { new INT.SAP.KA.Model.CheckCustomerCreditRequest() { CustomerCode = id } });
                if (resCheck != null)
                {
                    if (resCheck.CreditResponse != null)
                    {
                        if (resCheck.CreditResponse.Count() > 0)
                        {
                            if (resCheck.CustomerCreditList.Any(x => x.IsBlocked))
                            {
                                result = "Suspended";
                                return Json(new AjaxResponse(result));
                            }

                            //Remarked by Yoga : INVOICEHDR structure using the old one
                            List<string> invoiceList = new List<string>();
                            var openInvoiceList = await _accountingManager.GetOpenInvoice(id);
                            if (openInvoiceList != null)
                            {
                                if (openInvoiceList.Count() > 0)
                                {
                                    //invoiceList = openInvoiceList.Select(x => x.TransNo).ToList();
                                    invoiceList = openInvoiceList.Select(x => x.BatchCode).ToList();
                                }
                            }
                            resCheck.CreditResponse = resCheck.CreditResponse.Where(x => invoiceList.Contains(x.InvoiceNo)).ToList();

                            if (resCheck.CreditResponse.Any(x => Convert.ToInt32(x.Invoiceageing) >= 90))
                            {
                                result = "Overdue";
                                return Json(new AjaxResponse(result));
                            }

                            var nearlyOverdueInvoices = resCheck.CreditResponse.Where(x => Convert.ToInt32(x.Invoiceageing) >= 85 && Convert.ToInt32(x.Invoiceageing) <= 89).ToList();
                            if (nearlyOverdueInvoices.Count() > 0)
                            {
                                nearlyOverdueInvoices = nearlyOverdueInvoices.OrderByDescending(x => Convert.ToInt32(x.Invoiceageing)).ToList();
                                var topNearlyOverdue = nearlyOverdueInvoices.Select(x => x).FirstOrDefault();
                                var overdueDate = DateTime.ParseExact(topNearlyOverdue.InvoiceDueDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                                var interval = (overdueDate.Date - DateTime.Now.Date).TotalDays;

                                result = "Overdue in " + interval + " day(s)";
                                return Json(new AjaxResponse(result));
                            }
                        }
                    }
                }

                var tempBizLocID = id;
                var invoiceItemLogic = new INVOICEITEM(_env, _connectionString);
                var invoiceItemRes = invoiceItemLogic.RetrieveAll(x => x.BizRegID == tempBizLocID);
                if (invoiceItemRes != null)
                {
                    if (invoiceItemRes.Count() > 0)
                    {
                        if (invoiceItemRes.Any(x => x.Status == 4))
                        {
                            result = "Overdue";
                        }
                        //else if (invoiceHdrRes.Any(x => x.Status == 3))
                        //{
                        //    result = "Near Overdue";
                        //}
                        else
                        {
                            result = "Good";
                        }
                    }
                }

                return Json(new AjaxResponse(result));
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                return Json(new AjaxResponse(string.Empty));
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetInvoiceApprover(InvoiceFilter input)
        {
            try
            {
                DateTime startDate = Convert.ToDateTime(input.dateStart);
                DateTime endDate = Convert.ToDateTime(input.dateEnd);
                byte contStatus = input.status;
                int contFlag = input.Flag;
                string contBillType = input.billType;
                string contCustomerCode = input.customerCode;
                string contInvoiceID = input.invoiceId;
                string contCompany = input.company;
                string contFilter = string.IsNullOrEmpty(input.Filter) ? string.Empty : input.Filter;

                var invoiceHdrLogic = new INVOICEHDR(_env, _connectionString);
                var invoiceOverdueLogic = new INVOICEHDR(_env, _connectionString);
                var invoiceSumLogic = new INVOICEHDR(_env, _connectionString);

                dynamic data = null;

                if (input.mode == 0)
                {
                    #region Mode 0
                    if (!string.IsNullOrEmpty(contCustomerCode))
                    {
                        var transHdrLogic = new TRANSHDR(_env, _connectionString);
                        var transHdrRes = transHdrLogic.RetrieveAll(x => x.CustomerID == contCustomerCode && x.Status == 0);
                        if (transHdrRes != null)
                        {
                            if (transHdrRes.Count() > 0)
                            {
                                foreach (var index in transHdrRes)
                                {
                                    var tempTransNo = index.TransNo;
                                    await UpdateTransaction(tempTransNo);
                                }
                            }
                        }

                        //check customer credit status from SAP API
                        var custList = new List<string>() { contCustomerCode };
                        var resCheck = await _sapManager.CheckCustomerCredit(new List<INT.SAP.KA.Model.CheckCustomerCreditRequest>() { new INT.SAP.KA.Model.CheckCustomerCreditRequest() { CustomerCode = contCustomerCode } });
                        if (resCheck != null)
                        {
                            if (resCheck.CustomerCreditList != null)
                            {
                                if (resCheck.CustomerCreditList.Count() > 0)
                                {
                                    Int32 tempInvStatus = 1;
                                    Int32 tempCustStatus = 1;
                                    INVOICEHDR invHdrLogic = new INVOICEHDR(_env, _connectionString);
                                    //INVOICEITEM invItemLogic = new INVOICEITEM(_env, _connectionString);
                                    INVOICEDTL invDtlLogic = new INVOICEDTL(_env, _connectionString);

                                    if (resCheck.CreditResponse != null)
                                    {
                                        if (resCheck.CreditResponse.Count() > 0)
                                        {
                                            foreach (var index in resCheck.CreditResponse)
                                            {
                                                if (Convert.ToInt32(index.Invoiceageing) >= 90)
                                                {
                                                    tempInvStatus = 4; //Invoice overdue status
                                                }
                                                else if (Convert.ToInt32(index.Invoiceageing) >= 85 && Convert.ToInt32(index.Invoiceageing) <= 89)
                                                {
                                                    tempInvStatus = 3; //Invoice nearly overdue status
                                                }

                                                if (index.CreditBlock.ToUpper() == "X")
                                                {
                                                    tempInvStatus = 4;
                                                }

                                                var tempInvoiceNo = index.InvoiceNo;
                                                var invItemRes = invDtlLogic.Retrieve(x => x.TransNo == tempInvoiceNo && x.BizRegID == contCustomerCode);
                                                if (invItemRes != null)
                                                {
                                                    var tempMonthCode = invItemRes.BillNo;
                                                    var tempBatchcode = invItemRes.TransNo;
                                                    var invHdrRes = invHdrLogic.Retrieve(x => x.TransNo == tempBatchcode && x.BillNo == tempMonthCode);
                                                    if (invHdrRes != null)
                                                    {
                                                        if (invHdrRes.Status != 2)
                                                        {
                                                            invItemRes.Status = Convert.ToByte(tempInvStatus);
                                                            invItemRes.LastUpdate = DateTime.Now;
                                                            invItemRes.SyncLastUpd = DateTime.Now;
                                                            invItemRes.Inuse = 0;
                                                            invHdrRes.Status = Convert.ToByte(tempInvStatus);
                                                            invHdrRes.LastUpdate = DateTime.Now;
                                                            invHdrRes.SyncLastUpd = DateTime.Now;
                                                            invHdrRes.Inuse = 0;

                                                            await invDtlLogic.Update(invItemRes);
                                                            await invHdrLogic.Update(invHdrRes);
                                                        }
                                                    }
                                                }
                                            }

                                            if (resCheck.CreditResponse.Any(x => Convert.ToInt32(x.Invoiceageing) >= 90))
                                            {
                                                tempCustStatus = 4;
                                            }
                                            else if (resCheck.CreditResponse.Any(x => Convert.ToInt32(x.Invoiceageing) >= 85 && Convert.ToInt32(x.Invoiceageing) <= 89))
                                            {
                                                tempCustStatus = 3;
                                            }

                                            if (resCheck.CustomerCreditList.Any(x => x.IsBlocked))
                                            {
                                                tempCustStatus = 4;
                                            }

                                            var bizentityLogic = new Core.Entity.Repo.BIZENTITY(_env, _connectionString);
                                            var bizentityRes = bizentityLogic.Retrieve(x => x.BizRegID == contCustomerCode);
                                            if (bizentityRes != null)
                                            {
                                                var contSuspend = new Model.SuspendCustomerRequest
                                                {
                                                    CustomerID = bizentityRes.BizRegID,
                                                    Status = tempCustStatus,
                                                    TransporterID = bizentityRes.RefNo1,
                                                    UserID = bizentityRes.BizRegID
                                                };
                                                var resSuspend = await _accountingManager.SuspendCustomer(contSuspend);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //check if approver
                    User user = await _userManager.GetUserByIdAsync(Convert.ToInt64(AbpSession.UserId));
                    var isApprover = await _userManager.IsInRoleAsync(user, "Approver");
                    if (isApprover == true)
                    {
                        contCustomerCode = string.Empty;
                    }

                    data = invoiceHdrLogic
                        .Select<DTO.Accounting.INVOICEHDR,
                                DTO.Accounting.INVOICEDTL,
                                DTO.Accounting.TRANSHDR,
                                DTO.Accounting.TRANSDTL,
                                DTO.Accounting.TRANSTENDER,
                                DTO.Core.Entity.BIZENTITY,
                                DTO.Core.General.CODEMASTER,
                                DTO.Core.General.CODEMASTER>
                        ((IH, ID, TH, TD, TT, BE, CMN, CMS) => new
                        {
                            InvoiceNo = IH.TransNo,
                            SWTTANo = ID.BizRegID,
                            Company = BE.CompanyName,
                            Date = IH.PostDate,
                            FPXId = TH.BillNo,
                            Amount = IH.TransAmt,
                            Bank = CMN.CodeDesc,
                            Status = string.Format($"CASE WHEN CMS.Code >1 THEN 'PAID' ELSE 'UNPAID' END"),
                            //TotalInvoice = false,
                            //TotalOverdue = false,
                            //TotalOverdueAmount = false,
                            //TotalAmount = false,
                            //OverduePercent = string.Format("CAST(CAST(TotalOverdue AS Decimal) / CAST(TotalInvoice AS DECIMAL) * 100 AS DECIMAL)"),
                            IH.LastUpdate
                        })
                        //.OuterApply
                        //(
                        //    invoiceOverdueLogic
                        //    .Count(i => new { TotalInvoice = i.Status })
                        //    .InnerJoin<DTO.Accounting.INVOICEDTL>(i => i.TransNo, ID => ID.TransNo, (i, ID) => i.BillNo == ID.BillNo)
                        //    .Where<DTO.Accounting.INVOICEHDR,
                        //           DTO.Accounting.INVOICEHDR,
                        //           DTO.Accounting.INVOICEDTL>
                        //           ((IH, i, ID) => i.Flag == contFlag &&
                        //           (contStatus != 0 ? contStatus == 2 ? i.Inuse == 1 : i.Inuse == 0 : i.Inuse == 0) &&
                        //           (!string.IsNullOrEmpty(contCustomerCode) ? i.BizRegID == contCustomerCode || ID.BizRegID == contCustomerCode : false) &&
                        //           i.TransNo.Equals(IH.TransNo)
                        //           )
                        //    //(!string.IsNullOrEmpty(contCompany) ? ID.CompanyName.Contains(contCompany) : false))
                        //    .ToString()
                        //)
                        //.OuterApply
                        //(
                        //    invoiceOverdueLogic
                        //    .Count(i => new { TotalOverdue = i.Status })
                        //    .InnerJoin<DTO.Accounting.INVOICEDTL>(i => i.TransNo, ID => ID.TransNo, (i, ID) => i.BillNo == ID.BillNo)
                        //    .Where<DTO.Accounting.INVOICEHDR,
                        //            DTO.Accounting.INVOICEHDR,
                        //           DTO.Accounting.INVOICEDTL>
                        //           ((IH, i, ID) => i.Flag == contFlag &&
                        //           (contStatus != 0 ? contStatus == 2 ? i.Inuse == 1 : i.Inuse == 0 : i.Inuse == 0) &&
                        //           (i.Status == 4) &&
                        //           (i.TransNo.Equals(IH.TransNo)) &&
                        //           (!string.IsNullOrEmpty(contCustomerCode) ? i.BizRegID == contCustomerCode || ID.BizRegID == contCustomerCode : false))
                        //    //(!string.IsNullOrEmpty(contCompany) ? II.CompanyName.Contains(contCompany) : false))
                        //    .ToString()
                        //)
                        //.OuterApply
                        //(
                        //    invoiceOverdueLogic
                        //    .Sum(i => new { TotalOverdueAmount = i.TransAmt })
                        //    .InnerJoin<DTO.Accounting.INVOICEDTL>(i => i.TransNo, ID => ID.TransNo, (i, ID) => i.BillNo == ID.BillNo)
                        //    .Where<DTO.Accounting.INVOICEHDR,
                        //            DTO.Accounting.INVOICEHDR,
                        //           DTO.Accounting.INVOICEDTL>
                        //           ((IH, i, ID) => i.Flag == contFlag &&
                        //           (contStatus != 0 ? contStatus == 2 ? i.Inuse == 1 : i.Inuse == 0 : i.Inuse == 0) &&
                        //           (i.Status == 4) &&

                        //            (i.TransNo.Equals(IH.TransNo)) &&
                        //           (!string.IsNullOrEmpty(contCustomerCode) ? i.BizRegID == contCustomerCode || ID.BizRegID == contCustomerCode : false))
                        //    //(!string.IsNullOrEmpty(contCompany) ? II.CompanyName.Contains(contCompany) : false))
                        //    .ToString()
                        //)
                        //.OuterApply
                        //(
                        //    invoiceSumLogic
                        //    .Sum(i => new { TotalAmount = i.TransAmt })
                        //    .InnerJoin<DTO.Accounting.INVOICEDTL>(i => i.TransNo, ID => ID.TransNo, (i, ID) => i.BillNo == ID.BillNo)
                        //    .Where<DTO.Accounting.INVOICEHDR,
                        //           DTO.Accounting.INVOICEHDR,
                        //           DTO.Accounting.INVOICEDTL>
                        //           ((IH, i, ID) => i.Flag == contFlag &&
                        //           (contStatus != 0 ? contStatus == 2 ? i.Inuse == 1 : i.Inuse == 0 : i.Inuse == 0) &&
                        //           (contStatus != 0 ? i.Status == contStatus : i.Status != 2) &&
                        //            (i.TransNo.Equals(IH.TransNo)) &&
                        //           (!string.IsNullOrEmpty(contCustomerCode) ? i.BizRegID == contCustomerCode || ID.BizRegID == contCustomerCode : false))
                        //    //(!string.IsNullOrEmpty(contCompany) ? II.CompanyName.Contains(contCompany) : false))
                        //    .ToString()
                        //)
                        .InnerJoin<DTO.Accounting.INVOICEHDR, DTO.Accounting.INVOICEDTL>(IH => IH.TransNo, ID => ID.TransNo, (IH, ID) => IH.BillNo == ID.BillNo)
                        .InnerJoin<DTO.Core.Entity.BIZENTITY>(IH => IH.BizRegID, BE => BE.BizRegID)
                        .LeftJoin<DTO.Accounting.TRANSDTL>(IH => IH.TransNo, TD => TD.SerialNo)
                        .LeftJoin<DTO.Accounting.TRANSDTL, DTO.Accounting.TRANSHDR>(TD => TD.TransNo, TH => TH.TransNo)
                        .LeftJoin<DTO.Accounting.TRANSDTL, DTO.Accounting.TRANSTENDER>(TD => TD.BillNo, TT => TT.BillNo)
                        .LeftJoin<DTO.Core.General.CODEMASTER>(IH => IH.Status, CMS => CMS.Code, CMS => CMS.CodeType == "IVT")
                        .LeftJoin<DTO.Accounting.INVOICEDTL, DTO.Core.General.CODEMASTER>(ID => ID.ItemType, CMN => CMN.Code, CMN => CMN.CodeType == "BNB")
                        .Where<DTO.Accounting.INVOICEHDR,
                               DTO.Accounting.INVOICEDTL,
                               DTO.Core.General.CODEMASTER,
                               DTO.Core.General.CODEMASTER,
                               DTO.Core.Entity.BIZENTITY>
                        ((IH, ID, CMN, CMS, BE) =>
                            IH.Flag == contFlag &&
                              //(contStatus != 0 ? contStatus == 2 ? IH.Inuse == 1 : IH.Inuse == 0 : IH.Inuse == 0) &&
                              (contStatus == 2 ? ((IH.Status == 2) || (IH.Status == 3) || (IH.Status == 4)) : (IH.Status == contStatus)) &&

                            //(!string.IsNullOrEmpty(contBillType) ? II.ItemType == contBillType : false) &&
                            (!string.IsNullOrEmpty(contInvoiceID) ? IH.TransNo == contInvoiceID || ID.TransNo == contInvoiceID : false) &&
                            (!string.IsNullOrEmpty(contCustomerCode) ? IH.BizRegID == contCustomerCode || ID.BizRegID == contCustomerCode : false) &&
                          
                            //(!string.IsNullOrEmpty(contCompany) ? II.CompanyName.Contains(contCompany) : false) &&
                            ((input.dateStart != null && input.dateEnd != null) ? startDate <= IH.PostDate && endDate >= IH.PostDate : false) &&
                              ((!string.IsNullOrEmpty(contFilter) ? BE.CompanyName.Contains(contFilter) : false) ||
                            (!string.IsNullOrEmpty(contFilter) ? BE.BizRegID.Contains(contFilter) : false) ||
                            (!string.IsNullOrEmpty(contFilter) ? IH.TransNo.Contains(contFilter) : false) ||
                            (!string.IsNullOrEmpty(contFilter) ? CMS.CodeDesc == (contFilter) : false) ||
                            (!string.IsNullOrEmpty(contFilter) ? IH.BillNo.Contains(contFilter) : false))
                        )
                        .GroupBy<DTO.Accounting.INVOICEHDR,
                                DTO.Accounting.INVOICEDTL,
                                DTO.Accounting.TRANSHDR,
                                DTO.Accounting.TRANSDTL,
                                DTO.Accounting.TRANSTENDER,
                                DTO.Core.Entity.BIZENTITY,
                                DTO.Core.General.CODEMASTER,
                                DTO.Core.General.CODEMASTER>
                        ((IH, ID, TH, TD, TT, BE, CMN, CMS) => new
                        {
                            InvoiceNo = IH.TransNo,
                            SWTTANo = ID.BizRegID,
                            Company = BE.CompanyName,
                            Date = IH.PostDate,
                            FPXId = TH.BillNo,
                            Amount = IH.TransAmt,
                            Bank = CMN.CodeDesc,
                            Status = string.Format($"CASE WHEN CMS.Code >1 THEN 'PAID' ELSE 'UNPAID' END"),
                            InvStatus = IH.Status,
                            //TotalInvoice = false,
                            //TotalOverdue = false,
                            //TotalOverdueAmount = false,
                            //TotalAmount = false,
                            IH.LastUpdate
                        })
                        .OrderByDesc(IH => IH.LastUpdate)
                        .Fetch(input.SkipCount > 0 ? input.SkipCount : 0, input.MaxResultCount > 0 ? input.MaxResultCount : 0)
                        .ExecuteList<MapListInvoice_A>();
                       // .ToString();
                    #endregion
                }
                    
                return Json(new AjaxResponse(data));
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetInvoicePayDetail(GetListParameter input)
        {
            try
            {
                var paymentLogic = new PAYMENT(_env, _connectionString);
                paymentLogic.Select<MapListInvoicePayDetail>(x =>
                    new
                    {
                        x.DocumentNo,
                        x.DocumentType,
                        x.DocumentDate,
                        x.Amount
                    });

                var data = paymentLogic.StoredProcedureView<GetListParameter, MapListInvoicePayDetail>("dbo.GetInvoiceDetails", input);
                data.TotalCount = data.Items.Count();
                return Json(new AjaxResponse(data));
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetPaymentHistoryDetailsQ(string transNo)
        {
            try
            {
                var invoiceHdrLogic = new TRANSHDR(_env, _connectionString);
                var invoiceDtlLogic = new TRANSDTL(_env, _connectionString);
                var invoiceTenderLogic = new TRANSTENDER(_env, _connectionString);

                var header = invoiceHdrLogic
                    .Select<DTO.Accounting.TRANSHDR,
                            DTO.Accounting.TRANSDTL,
                            DTO.Core.Entity.BIZENTITY,
                            DTO.Core.General.CODEMASTER, 
                            DTO.Core.General.CODEMASTER,
                            DTO.Core.General.CODEMASTER>
                    ((TH, TD, BE, CMS, CMN, CMF) => new
                    {
                        TH.BillNo,
                        Date = TH.TransDate,
                        //Attn = TH.CashierID,
                        Attn = BE.ContactPerson2,
                        CustomerCode = TH.BizRegID,
                        Company = BE.CompanyName,
                        BE.Address1,
                        BE.Address2,
                        BE.Address3,
                        BE.Address4,
                        CashSaleAddr = TH.TransRemark,
                        FpxStatus = CMS.CodeDesc,
                        PaidStatus = CMN.CodeDesc,
                        Bank = CMF.CodeDesc,
                        TH.CustPrivilege,
                        Id = TH.InSvcID,
                        Tax = TH.TransChgAmt,
                        Rounding = TH.TransAmtRnd,
                        Amount = TH.TransAmt,
                        CustomerNo = TH.CustomerID,
                        YourRef = TH.CustPkgID,
                        RefNo = TH.AcctNo,
                        CreditVote = TH.TransDiscRemark,
                        ProjectCode = TH.TransDiscReasonCode,
                        OurRef = TH.ServerID,
                        TH.TransReasonCode,
                        TH.TransChgAmt,
                        TH.TransSubTotal
                    })
                     .InnerJoin<DTO.Accounting.TRANSDTL>(TH => TH.TransNo, TD => TD.TransNo)
                    .LeftJoin<DTO.Accounting.TRANSTENDER>(TH => TH.TransNo, TT => TT.TransNo)
                    .LeftJoin<DTO.Core.Entity.BIZENTITY>(TH => TH.BizRegID, BE => BE.BizRegID)
                    .LeftJoin<DTO.Core.General.CODEMASTER>(TH => TH.TransStatus, CMS => CMS.Code, CMS => CMS.CodeType == "FPX")
                    .LeftJoin<DTO.Core.General.CODEMASTER>(TH => TH.Status, CMN => CMN.Code, CMN => CMN.CodeType == "PAY")
                    .LeftJoin<DTO.Accounting.TRANSTENDER, DTO.Core.General.CODEMASTER>(TT => TT.BankCode, CMF => CMF.Code, CMF => CMF.CodeType == "BNB")
                    .Where<DTO.Accounting.TRANSHDR,
                           DTO.Accounting.TRANSDTL,
                           DTO.Core.Entity.BIZENTITY,
                           DTO.Core.General.CODEMASTER,
                           DTO.Core.General.CODEMASTER>
                    ((TH, TD, BE, CMS, CMN) =>
                           TH.TransNo == transNo)
                    .GroupBy<DTO.Accounting.TRANSHDR,
                            DTO.Accounting.TRANSDTL,
                            DTO.Core.Entity.BIZENTITY,
                            DTO.Core.General.CODEMASTER,
                            DTO.Core.General.CODEMASTER,
                            DTO.Core.General.CODEMASTER>
                    ((TH, TD, BE, CMS, CMN, CMF) => new
                    {
                        TH.BillNo,
                        Date = TH.TransDate,
                        //Attn = TH.CashierID,
                        Attn = BE.ContactPerson2,
                        CustomerCode = TH.BizRegID,
                        Company = BE.CompanyName,
                        BE.Address1,
                        BE.Address2,
                        BE.Address3,
                        BE.Address4,
                        CashSaleAddr = TH.TransRemark,
                        FpxStatus = CMS.CodeDesc,
                        PaidStatus = CMN.CodeDesc,
                        Bank = CMF.CodeDesc,
                        TH.CustPrivilege,
                        Id = TH.InSvcID,
                        Tax = TH.TransChgAmt,
                        Rounding = TH.TransAmtRnd,
                        Amount = TH.TransAmt,
                        CustomerNo = TH.CustomerID,
                        YourRef = TH.CustPkgID,
                        RefNo = TH.AcctNo,
                        CreditVote = TH.TransDiscRemark,
                        ProjectCode = TH.TransDiscReasonCode,
                        OurRef = TH.ServerID,
                        TH.TransReasonCode,
                        TH.TransChgAmt,
                        TH.TransSubTotal
                    })
                    .Execute().FirstOrDefault();

                var details = invoiceDtlLogic
                    .Select<DTO.Accounting.TRANSDTL,
                            DTO.Core.General.CODEMASTER, DTO.Core.Entity.BIZENTITY>
                    ((TD, CMS, BE) => new
                    {
                        ItemCode = TD.ItemCode,
                        ItemDesc = TD.Remark,
                        Company = BE.CompanyName,
                        Qty = TD.Qty,
                        UnitPrice = TD.UnitCost,
                        SubTotal = TD.TolAmt,
                        Type = CMS.CodeDesc,
                        TD.ItemType,
                        TD.StkType
                    })
                    .LeftJoin<DTO.Core.Entity.BIZENTITY>(TD => TD.BizRegID, BE => BE.BizRegID)
                    .LeftJoin<DTO.Core.General.CODEMASTER>(TD => TD.StkType, CMS => CMS.Code, CMS => CMS.CodeType == "INT")
                    .Where<DTO.Accounting.TRANSDTL,
                           DTO.Core.Entity.BIZENTITY>
                    ((TD, CMS) =>
                           TD.TransNo == transNo)
                   .GroupBy<DTO.Accounting.TRANSDTL,
                            DTO.Core.General.CODEMASTER, DTO.Core.Entity.BIZENTITY>
                    ((TD, CMS, BE) => new
                    {
                        ItemCode = TD.ItemCode,
                        ItemDesc = TD.Remark,
                        Qty = TD.Qty,
                        UnitPrice = TD.UnitCost,
                        SubTotal = TD.TolAmt,
                        Type = CMS.CodeDesc,
                        TD.ItemType,
                        TD.StkType,
                        BE.CompanyName
                    })
                    .ExecuteList<dynamic>();

                var tender = invoiceTenderLogic
                    .Select<DTO.Accounting.TRANSTENDER,
                            DTO.Accounting.TENDER>
                    ((TT, TD) => new
                    {
                        TT.TenderID,
                        TT.TransDate,
                        TT.TransTime,
                        TT.TenderAmt,
                        TT.MerchantID,
                        TT.RefToken,
                        TT.BankCode,
                        TT.RespCode,
                        TT.ExternalID,
                        TD.TenderDesc
                    })
                    .LeftJoin<DTO.Accounting.TENDER>(TT => TT.TenderID, TD => TD.TenderID)
                    .Where<DTO.Accounting.TRANSTENDER>
                    ((TT) =>
                        TT.TransNo == transNo)
                    .GroupBy<DTO.Accounting.TRANSTENDER,
                             DTO.Accounting.TENDER>
                    ((TT, TD) => new
                    {
                        TT.TenderID,
                        TT.TransDate,
                        TT.TransTime,
                        TT.TenderAmt,
                        TT.MerchantID,
                        TT.RefToken,
                        TT.BankCode,
                        TT.RespCode,
                        TT.ExternalID,
                        TD.TenderDesc,
                       
                    })
                    .Execute().FirstOrDefault();

                return Json(new AjaxResponse(new { transHdr = header, transDtl = details, transTender = tender }));
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpGet]
        public async Task<JsonResult> CheckAllPendingPaymentStatus(string transNo)
        {
            try
            {
                var data = await _accountingManager.CheckPendingPaymentStatus(transNo);
                if (data == null)
                    return Json(new AjaxResponse(new { success = false, message = "No transaction found" }));

                if (data.TotalCount == 0)
                    return Json(new AjaxResponse(new { success = false, message = "No transaction found" }));

                foreach (var index in data.Items)
                {
                    var res = await UpdateTransactionQ(index.TransNo);
                }
                return Json(new AjaxResponse(data));
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpPost]
        public async Task<JsonResult> UpdateTransactionQ([FromBody] string TransNo)
        {
            try
            {
                var result = await _accountingManager.UpdateTransactionQ(TransNo);
                return Json(result);
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new { success = false }));
            }
        }

        #region check old invoice fpx status
        [HttpPost]
        public async Task<JsonResult> UpdateOldQ(string TransNo)
        {
            try
            {
                var result = await _accountingManager.UpdateOldQ(TransNo);
                return Json(result);
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new { success = false }));
            }
        }
        #endregion


        #region check old invoice fpx status from view
        [HttpPost]
        public async Task<JsonResult> CheckFailedFPXQ()
        {
            try
            {
                var result = await _accountingManager.CheckFailedFPXQ();
                return Json(result);
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new { success = false }));
            }
        }
        #endregion

        #region check old invoice fpx status from view shcedule
        [HttpPost]
        public async Task<JsonResult> CheckFailedFPXQSchedule(string cron = "0 * * * *")
        {
            try
            {
                
                RecurringJob.AddOrUpdate(() => _accountingManager.CheckFailedFPXQ(), cron, TimeZoneInfo.Local);
                return Json(new AjaxResponse(new { success = true,message="Successfully Add The Reccurring Schedule, you will get email when this catch invalid transaction and proccess is done" }));
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new { success = false }));
            }
        }
        #endregion

        #region Invoice List
        [HttpGet]
        //[AbpMvcAuthorize]
        public async Task<JsonResult> GetInvoiceList(Model.GetInvoiceParameter input)
        {
            try
            {
                var result = await _accountingManager.GetInvoiceList(input);
                return Json(new AjaxResponse(result));
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpGet]
        //[AbpMvcAuthorize]
        public async Task<JsonResult> CheckTransaction(string bizregID, string userID)
        {
            try
            {
                string customerID = !string.IsNullOrEmpty(bizregID) ? bizregID : AbpSession.UserData.BizRegID;
                long? id = !string.IsNullOrEmpty(userID) ? Convert.ToInt64(userID) : AbpSession.UserId;                

                BackgroundJob.Enqueue(() => _accountingManager.CheckTransaction(bizregID, id));
                return Json(true);
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new { success = false }));
            }
        }
        #endregion

        #region get total invoice
        [HttpGet]
        public async Task<JsonResult> GetTotalInvoiceQ(string tempBizRegID)
        {
            try
            {
                var result = await _accountingManager.GetTotalInvoiceQ(tempBizRegID);
                return Json(new AjaxResponse(result));
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }
        #endregion

        #region Invoice Detail
        [HttpGet]
        //[AbpMvcAuthorize]
        public async Task<JsonResult> GetInvoiceHeader(string transno)
        {
            try
            {
                var result = await _accountingManager.GetInvoiceHeader(transno);
                return Json(new AjaxResponse(result));
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpGet]
        //[AbpMvcAuthorize]
        public async Task<JsonResult> GetInvoiceDetailList(string bizregid, string transno)
        {
            try
            {
                var result = await _accountingManager.GetInvoiceDetails(bizregid, transno);
                return Json(new AjaxResponse(result));
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }
        #endregion

        #region Get Invoice List CWMS Old
        [HttpGet]
        //[AbpMvcAuthorize]
        public async Task<JsonResult> GetInvoiceQ(Model.InvoiceFilter input)
        {
            try
            {
                //input for now only filtered by customerID/BizRegID
                var tempData = input.customerCode;
                bool result;
                
                //in case when customer code are empty
                if (tempData == null || tempData == "")
                {
                    return Json(new AjaxResponse(result = false));
                }
                //if customer code return any, so data will be processed
                else
                {
                    var data = await _accountingManager.GetInvoiceQ(input);
                    return Json(new AjaxResponse(data));
                }
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }
        #endregion

        #region Get Recent Invoice CWMS
        [HttpGet]
        //[AbpMvcAuthorize]
        public async Task<JsonResult> GetRecentInvoiceQ(Model.InvoiceFilter input)
        {
            try
            {
                var tempData = input.customerCode;
                bool result;

                //in case when customer code are empty
                if (tempData == null || tempData == "")
                {
                    return Json(new AjaxResponse(result = false));
                }
                //if customer code return any, so data will be processed
                else
                {
                    var data = await _accountingManager.GetRecentInvoiceQ(input);
                    return Json(new AjaxResponse(data));
                }
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }
        #endregion
        #endregion

        #region Sample Invoice
        [HttpGet]
        public async Task<JsonResult> GetSampleInvoice(int? num)
        {
            try
            {
                var result = await _accountingManager.GetSampleInvoice(num);
                return Json(result);
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new { success = false }));
            }
        }
        #endregion
    }
}
