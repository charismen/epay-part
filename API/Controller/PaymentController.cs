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
using Plexform.Base.Accounting.Payment.Model;
using Plexform.Base.Accounting.Payment.Map;
using Hangfire;
using System.Transactions;

namespace Plexform.Base.Accounting.Controller
{
    [Route("api/")]
    public class PaymentController : BaseController
    {
        #region Declaration
        public ILogger Log { get; set; }
        private readonly IRealTimeCommunicator _realtimeService;
        private readonly IPlexformNotifier _plexformNotifier;
        private readonly ILocalizationManager _localizationManager;
        private readonly AccountingManager _accountingManager;
        private readonly PaymentManager _paymentManager;
        #endregion Declaration

        public PaymentController(
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
            BizUserManager bizUser,
            UserManager userManager,
            TenantManager tenantManager,
            AccountingManager accountingManager,
            PaymentManager paymentManager
            ) : base(roleRepository, appNotifier, env, appFolders, objectMapper, tenantManager, excelExporter, localizationManager)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                #region Declaration
                Log = NullLogger.Instance;
                _realtimeService = realtimeService;
                _plexformNotifier = plexformNotifier;
                _localizationManager = localizationManager;
                _accountingManager = accountingManager;
                _paymentManager = paymentManager;
                #endregion
            }
            catch (Exception ex)
            {
                Log.Error(nameof(PaymentController), ex);
            }
        }

        #region Account Controller FPX Response
        [HttpPost("account/[action]")]
        public async Task<IActionResult> FpxGetResponse([FromForm] FPXResponse data)
        {
            try
            {
                var result = await _accountingManager.FPXIndirectResponse(data);
                Response.Redirect(_appConfiguration["App:ClientRootAddress"] + "account/payment-response?orderNo=" + data.fpx_sellerOrderNo);
                return Json(result);
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpPost("account/[action]")]
        public async Task<IActionResult> FpxDirectGetResponse([FromForm] FPXResponse data)
        {
            try
            {
                var result = await _accountingManager.FPXDirectResponse(data);
                Response.Redirect(_appConfiguration["App:ClientRootAddress"] + "account/response");
                return Json(result);
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(AccountingController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }
        #endregion

        /*-----------------------------------------------------------Payment--------------------------------------------------------------------*/

        [HttpPost("payment/[action]")]
        public async Task<IActionResult> AddPayment([FromForm] AddPaymentModel data)
        {
            try
            {
                bool returnStat = false;
                string returnMsg = String.Empty;
                var checkFormat = await _paymentManager.CheckPaymentFormat(data);
                if (checkFormat.Success)
                {
                    var result = await _paymentManager.AddPayment(data);
                    if (result.Success == true)
                    {
                        var resMap = (dynamic)result.Result;
                        Response.Redirect(_appConfiguration["App:ClientRootAddress"] + "account/payment?trx=" + resMap.ID);
                        returnStat = true;
                    }
                }
                else
                {
                    returnMsg = checkFormat.Error.Message;
                }

                return Json(new AjaxResponse(new { success = returnStat, message = returnMsg }));
            }
            catch(UserFriendlyException ex)
            {
                Log.Error(nameof(PaymentController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpGet("payment/[action]")]
        public async Task<JsonResult> GetPayment(string PaymentTransID)
        {
            try
            {
                var resData = await _paymentManager.GetPayment(PaymentTransID);

                return Json(new AjaxResponse(resData.Result));
            }
            catch(UserFriendlyException ex)
            {
                Log.Error(nameof(PaymentController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpPost("payment/[action]")]
        public async Task<JsonResult> AddTender([FromBody] AddTenderModel data)
        {
            try
            {
                bool returnStat = false;
                string returnMsg = String.Empty;
                var resData = await _paymentManager.AddTender(data);
                if (resData.Success)
                {
                    returnStat = true;
                }
                else
                {
                    returnMsg = resData.Error.Message;
                }

                return Json(new AjaxResponse(new { success = returnStat, message = returnMsg }));
            }
            catch(UserFriendlyException ex)
            {
                Log.Error(nameof(PaymentController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        #region FPX
        [HttpGet("payment/FPX/[action]")]
        public async Task<JsonResult> GetList(string fpx_msgToken)
        {
            try
            {
                bool returnStat = false;
                var resData = await _paymentManager.GetFPXBankList(fpx_msgToken);
                if (resData.Success)
                {
                    returnStat = true;
                }
                else
                {
                    returnStat = false;
                }

                return Json(new AjaxResponse(new { success = returnStat, resData.Result }));
            }
            catch(UserFriendlyException ex)
            {
                Log.Error(nameof(PaymentController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpPost("payment/FPX/[action]")]
        public async Task<JsonResult> GetCheckSum([FromBody] FPXBodyModel fpxBody)
        {
            try
            {
                bool returnStat = false;
                var resData = await _paymentManager.GetFPXCheckSum(fpxBody);
                if (resData.Success)
                {
                    returnStat = true;
                }
                else
                {
                    returnStat = false;
                }

                return Json(new AjaxResponse(new { success = returnStat, resData.Result }));
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(PaymentController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpGet("payment/FPX/[action]")]
        public async Task<JsonResult> GetFPXValidateStatus(string refInfo)
        {
            try
            {
                bool returnStat = false;
                var resData = await _paymentManager.GetFPXValidateStatus(refInfo);
                if (resData.Success)
                {
                    returnStat = true;
                }
                else
                {
                    returnStat = false;
                }

                return Json(new AjaxResponse(new { success = returnStat, resData.Result }));
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(PaymentController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpPost("payment/FPX/[action]")]
        public async Task<IActionResult> IndResponse([FromForm] FPXResponseModel data)
        {
            try
            {
                bool returnStat = false;
                string returnMsg = String.Empty;
                var checkFormat = await _paymentManager.IndResponse(data);
                if (checkFormat.Success)
                {
                    // Will be await Update Payment
                    Response.Redirect(_appConfiguration["App:ClientRootAddress"] + "account/fpx-indirect?paymentID=" + data.fpx_sellerOrderNo);
                    returnStat = true;
                }
                else
                {
                    var checkRes = checkFormat.Error;
                    if (checkRes != null)
                    {
                        returnStat = false;
                        returnMsg = checkRes.Message;
                    }
                }

                return Json(new AjaxResponse(new { success = returnStat, message = returnMsg }));
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(PaymentController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpPost("payment/FPX/[action]")]
        public async Task<IActionResult> DirResponse([FromForm] FPXResponseModel data)
        {
            try
            {
                bool returnStat = false;
                string returnMsg = String.Empty;
                var checkFormat = await _paymentManager.DirResponse(data);
                if (checkFormat.Success)
                {
                    // Will be await Update Payment
                    Response.Redirect(_appConfiguration["App:ClientRootAddress"] + "account/fpx-direct");
                    returnStat = true;
                }
                else
                {
                    var checkRes = checkFormat.Error;
                    if (checkRes != null)
                    {
                        returnStat = false;
                        returnMsg = checkRes.Message;
                    }
                }

                return Json(new AjaxResponse(new { success = returnStat, message = returnMsg }));
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(PaymentController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }
        #endregion

        #region MayBank Credit Card
        [HttpGet("payment/MayBankCC/[action]")]
        public async Task<JsonResult> GetSignature(string MerchantCode, string Amount)
        {
            try
            {
                bool returnStat = false;
                var resData = await _paymentManager.GetMayBankCCSignature(MerchantCode, Amount);
                if (resData.Success)
                {
                    returnStat = true;
                }
                else
                {
                    returnStat = false;
                }

                return Json(new AjaxResponse(new { success = returnStat, resData.Result }));
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(PaymentController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpGet("payment/MayBankCC/[action]")]
        public async Task<JsonResult> GetValidateStatus(string refInfo)
        {
            try
            {
                bool returnStat = false;
                var resData = await _paymentManager.GetMayBankCCValidateStatus(refInfo);
                if (resData.Success)
                {
                    returnStat = true;
                }
                else
                {
                    returnStat = false;
                }

                return Json(new AjaxResponse(new { success = returnStat, resData.Result }));
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(PaymentController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpPost("payment/[action]")]
        public async Task<IActionResult> IndCCResponse([FromForm] MayBankCCResponseModel data)
        {
            try
            {
                bool returnStat = false;
                string returnMsg = String.Empty;
                var checkFormat = await _paymentManager.IndCCResponse(data);
                if (checkFormat.Success)
                {
                    // Will be await Update Payment
                    Response.Redirect(_appConfiguration["App:ClientRootAddress"] + "account/creditcard-indirect?paymentID=" + data.MERCHANT_TRANID);
                    returnStat = true;
                }
                else
                {
                    var checkRes = checkFormat.Error;
                    if (checkRes != null)
                    {
                        returnStat = false;
                        returnMsg = checkRes.Message;
                    }
                }

                return Json(new AjaxResponse(new { success = returnStat, message = returnMsg }));
            }
            catch (UserFriendlyException ex)
            {
                Log.Error(nameof(PaymentController), ex);
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }
        #endregion
    }
}
