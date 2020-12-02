using System;
using System.Threading.Tasks;
using System.Text;
using System.Web;
using Abp.Localization;
using Plexform.Authorization.Users;
using Plexform.Authorization.Roles;
using Plexform.MultiTenancy;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using Abp.ObjectMapping;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Abp.Web.Models;
using System.Transactions;
using System.Collections.Specialized;
using Plexform.Base.Core;
using Plexform.Base.Core.Helper;
using Plexform.Base.Accounting.Controller;
using Plexform.Base.Accounting.Payment.Model;
using Plexform.Base.Accounting.Payment.Map;
using Plexform.Base.Accounting.Payment.Repo;
using Plexform.Base.Accounting.Repo;
using Plexform.Base.Accounting.Map;
using Hangfire;

namespace Plexform.Base.Accounting
{
    public class PaymentManager : BaseClass
    {
        #region Declaration
        private JObject appsettingsjson;
        private JObject SUNSystem;
        private JObject AppSet;
        private readonly AccountingManager _accountingManager;
        #endregion

        public PaymentManager(
            IWebHostEnvironment env,
            IAppFolders appFolders,
            IObjectMapper objectMapper,
            ILocalizationManager localizationManager,
            IExcelExporter excelExporter,
            RoleManager roleManager,
            UserManager userManager,
            TenantManager tenantManager,
            AccountingManager accountingManager)
            : base(env, appFolders, objectMapper, roleManager, userManager, tenantManager, excelExporter, localizationManager)
        {
            try
            {
                #region Declaration
                _accountingManager = accountingManager;
                #endregion
            }
            catch (Exception ex)
            {
                Log.Error(nameof(PaymentManager), ex);
            }
        }

        #region FPX Response
        public async Task<AjaxResponse> FPXIndirectResponse(FPXResponse data)
        {
            try
            {
                return new AjaxResponse(true);
            }
            catch (Exception ex)
            {

                Log.Error(nameof(PaymentManager), ex);
                throw ex;
            }
        }

        public async Task<AjaxResponse> FPXDirectResponse(FPXResponse data)
        {
            try
            {
                return new AjaxResponse(true);
            }
            catch (Exception ex)
            {
                Log.Error(nameof(PaymentManager), ex);
                throw ex;
            }
        }
        #endregion

        #region Payment Manager
        public async Task<AjaxResponse> CheckPaymentFormat(AddPaymentModel input)
        {
            try
            {
                bool checkRes = true;
                string checkMsg = String.Empty;

                if (String.IsNullOrEmpty(input.MerchantCode))
                {
                    checkMsg = "Incorrect MerchantCode Format or Empty";
                    return new AjaxResponse(new ErrorInfo(checkMsg));
                }

                if (String.IsNullOrEmpty(input.MerchantTransNo))
                {
                    checkMsg = "Incorrect MerchantTransNo Format or Empty";
                    return new AjaxResponse(new ErrorInfo(checkMsg));
                }

                if (String.IsNullOrEmpty(input.TransCurrency))
                {
                    checkMsg = "Incorrect TransCurrency Format or Empty";
                    return new AjaxResponse(new ErrorInfo(checkMsg));
                }

                if (String.IsNullOrEmpty(input.TransAmt))
                {
                    checkMsg = "Incorrect TransAmt Format or Empty";
                    return new AjaxResponse(new ErrorInfo(checkMsg));
                }

                if (String.IsNullOrEmpty(input.ProductDesc))
                {
                    checkMsg = "Incorrect ProductDesc Format or Empty";
                    return new AjaxResponse(new ErrorInfo(checkMsg));
                }

                if (String.IsNullOrEmpty(input.UserName))
                {
                    checkMsg = "Incorrect UserName Format or Empty";
                    return new AjaxResponse(new ErrorInfo(checkMsg));
                }

                if (String.IsNullOrEmpty(input.UserEmail))
                {
                    checkMsg = "Incorrect UserEmail Format or Empty";
                    return new AjaxResponse(new ErrorInfo(checkMsg));
                }

                if (String.IsNullOrEmpty(input.UserContact))
                {
                    checkMsg = "Incorrect UserContact Format or Empty";
                    return new AjaxResponse(new ErrorInfo(checkMsg));
                }

                if (String.IsNullOrEmpty(input.Remark))
                {
                    checkMsg = "Incorrect Remark Format or Empty";
                    return new AjaxResponse(new ErrorInfo(checkMsg));
                }

                if (String.IsNullOrEmpty(input.Signature))
                {
                    checkMsg = "Incorrect Signature Format or Empty";
                    return new AjaxResponse(new ErrorInfo(checkMsg));
                }

                if (String.IsNullOrEmpty(input.ResponseURL))
                {
                    checkMsg = "Incorrect ResponseURL Format or Empty";
                    return new AjaxResponse(new ErrorInfo(checkMsg));
                }

                if (String.IsNullOrEmpty(input.AdditionalURL))
                {
                    checkMsg = "Incorrect AdditionalURL Format or Empty";
                    return new AjaxResponse(new ErrorInfo(checkMsg));
                }

                return new AjaxResponse(checkRes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<AjaxResponse> AddPayment(AddPaymentModel input)
        {
            try
            {
                bool paymentSuccess = false;
                var contPaymentTransID = input.MerchantTransNo;
                PAYMENTHDR paymentHDRLogic = new PAYMENTHDR(_env, _connectionString);
                PAYMENTDTL paymentDTLLogic = new PAYMENTDTL(_env, _connectionString);
                DTO.Payment.PAYMENTHDR dataPaymentHDR, contPaymentHDR;
                DTO.Payment.PAYMENTDTL dataPaymentDTL, contPaymentDTL;

                contPaymentHDR = paymentHDRLogic.Retrieve(x => x.PaymentRef == contPaymentTransID);
                if (contPaymentHDR == null)
                {
                    using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        try
                        {
                            dataPaymentHDR = new DTO.Payment.PAYMENTHDR
                            {
                                BizRegID = "1",
                                BizLocID = "1",
                                PaymentTransID = DateTime.Now.ToString("yyyyMMddHHmmss") + GeneratorHelper.GenerateRandomString(6),
                                PaymentRef = input.MerchantTransNo,
                                MerchantCode = input.MerchantCode,
                                BaseCurrency = input.TransCurrency,
                                TransCurrency = input.TransCurrency,
                                TransTotalAmt = Convert.ToDecimal(input.TransAmt),
                                ProductDesc = input.ProductDesc,
                                UserName = input.UserName,
                                UserEmail = input.UserEmail,
                                UserContact = input.UserContact,
                                Signature = input.Signature,
                                ResponseURL = input.ResponseURL,
                                AdditionalURL = input.AdditionalURL,
                                Status = 1,
                                Flag = 1,
                                IsHost = 0
                            };
                            await paymentHDRLogic.Create(dataPaymentHDR);

                            paymentSuccess = true;
                            scope.Complete();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
                else
                {
                    return new AjaxResponse(new ErrorInfo("Duplicate Merchant Transaction ID"));
                }

                return new AjaxResponse(new { success = paymentSuccess, ID = dataPaymentHDR.PaymentTransID });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<AjaxResponse> GetPayment(string PaymentTransID)
        {
            try
            {
                var contPaymentTransID = PaymentTransID;
                Payment.Repo.PAYMENTHDR paymentHDRLogic = new Payment.Repo.PAYMENTHDR(_env, _connectionString);
                dynamic data = new { };

                data = paymentHDRLogic
                    .Select<DTO.Payment.PAYMENTHDR>(
                        (PH) => new
                        {
                            PH.BizRegID,
                            PH.BizLocID,
                            PH.PaymentTransID,
                            PH.PaymentRef,
                            PH.MerchantCode,
                            PH.BaseCurrency,
                            PH.TransCurrency,
                            PH.TransTotalAmt,
                            PH.TransPaidAmt,
                            PH.TransDueAmt,
                            PH.ProductDesc,
                            PH.UserEmail,
                            PH.UserContact,
                            PH.ResponseURL,
                            PH.AdditionalURL,
                            PH.Remark
                        })
                    .Where<DTO.Payment.PAYMENTHDR>((PH) =>
                        PH.PaymentTransID == contPaymentTransID &&
                        PH.Status == 1 &&
                        PH.Flag == 1
                    )
                    .GroupBy<DTO.Payment.PAYMENTHDR>(
                        (PH) => new
                        {
                            PH.BizRegID,
                            PH.BizLocID,
                            PH.PaymentTransID,
                            PH.PaymentRef,
                            PH.MerchantCode,
                            PH.BaseCurrency,
                            PH.TransCurrency,
                            PH.TransTotalAmt,
                            PH.TransPaidAmt,
                            PH.TransDueAmt,
                            PH.ProductDesc,
                            PH.UserEmail,
                            PH.UserContact,
                            PH.ResponseURL,
                            PH.AdditionalURL,
                            PH.Remark
                        })
                    .Execute().FirstOrDefault();

                return new AjaxResponse(new { items = data });
            }
            catch (Exception ex)
            {
                string exMsg = ex.Message.ToString();
                return null;
            }
        }

        //public async Task<AjaxResponse> AddPayment(Accounting.Payment.Model.AddPayment input)
        //{
        //    try
        //    {
        //        Payment.Repo.PAYMENTHDR paymentHDRLogic = new Payment.Repo.PAYMENTHDR(_env, _connectionString);
        //        Payment.Repo.PAYMENTDTL paymentDTLLogic = new Payment.Repo.PAYMENTDTL(_env, _connectionString);
        //        Payment.Repo.PAYMENTTENDER paymentTenderLogic = new Payment.Repo.PAYMENTTENDER(_env, _connectionString);
        //        Payment.Repo.PAYMENTLOG paymentLogLogic = new Payment.Repo.PAYMENTLOG(_env, _connectionString);
        //        Payment.Repo.TENDERTYPE tenderTypeLogic = new Payment.Repo.TENDERTYPE(_env, _connectionString);
        //        Core.General.Repo.CODEMASTER codemasterLogic = new Core.General.Repo.CODEMASTER(_env, _connectionString);

        //        DTO.Payment.PAYMENTHDR dataMapPaymentHDR = _objectMapper.Map<DTO.Payment.PAYMENTHDR>(input.PaymentHDR);
        //        IList<DTO.Payment.PAYMENTDTL> dataMapPaymentDTL = _objectMapper.Map<IList<DTO.Payment.PAYMENTDTL>>(input.PaymentDTL);
        //        DTO.Payment.PAYMENTTENDER dataMapPaymentTender = _objectMapper.Map<DTO.Payment.PAYMENTTENDER>(input.PaymentTender);
        //        DTO.Payment.PAYMENTLOG dataMapPaymentLog = _objectMapper.Map<DTO.Payment.PAYMENTLOG>(input.PaymentTender);

        //        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //        {
        //            try
        //            {
        //                // PaymentHDR
        //                DTO.Payment.PAYMENTHDR contPaymentHDR = paymentHDRLogic.Retrieve(x => x.PaymentTransID == dataMapPaymentHDR.PaymentTransID);
        //                if (contPaymentHDR == null)
        //                {
        //                    await paymentHDRLogic.Create(dataMapPaymentHDR);
        //                }
        //                else
        //                {
        //                    await paymentHDRLogic.Update(dataMapPaymentHDR);
        //                }

        //                // PaymentDTL
        //                DTO.Payment.PAYMENTDTL contPaymentDTL;
        //                foreach (var index in dataMapPaymentDTL)
        //                {
        //                    contPaymentDTL = paymentDTLLogic.Retrieve(x => x.PaymentTransID == index.PaymentTransID && x.TransRef == index.TransRef && x.SeqNo == index.SeqNo);
        //                    if (contPaymentDTL == null)
        //                    {
        //                        await paymentDTLLogic.Create(index);
        //                    }
        //                }

        //                using (TransactionScope scope_tender = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
        //                {
        //                    try
        //                    {
        //                        // PaymentTender
        //                        var contPaymentTender = paymentTenderLogic.Retrieve(x => x.PaymentTransID == dataMapPaymentTender.PaymentTransID && x.TenderID == dataMapPaymentTender.TenderID && x.SeqNo == dataMapPaymentTender.SeqNo);
        //                        if (contPaymentHDR == null)
        //                        {   
        //                            await paymentTenderLogic.Create(dataMapPaymentTender);
        //                        }

        //                        // Payment Log
        //                        var contPaymentLog = paymentLogLogic.Retrieve(x => x.PaymentTransID == dataMapPaymentLog.PaymentTransID && x.TenderID == dataMapPaymentLog.TenderID && x.SeqNo == dataMapPaymentLog.SeqNo);
        //                        if (contPaymentLog == null)
        //                        {
        //                            await paymentLogLogic.Create(dataMapPaymentLog);
        //                        }

        //                        scope_tender.Complete();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        throw ex;
        //                    }
        //                }

        //                scope.Complete();
        //            }
        //            catch(Exception ex)
        //            {
        //                throw ex;
        //            }
        //        }

        //        return new AjaxResponse(new { success = true, message = "" });
        //    }
        //    catch(Exception ex)
        //    {
        //        string exMsg = ex.Message.ToString();
        //        return null;
        //    }
        //}

        //public async Task<AjaxResponse> UpdatePayment(string PaymentTransID, string RefID)
        //{
        //    try
        //    {
        //        var contPaymentTransID = PaymentTransID;
        //        var contRefID = RefID;
        //        Payment.Repo.PAYMENTHDR paymentHDRLogic = new Payment.Repo.PAYMENTHDR(_env, _connectionString);
        //        Payment.Repo.PAYMENTDTL paymentDTLLogic = new Payment.Repo.PAYMENTDTL(_env, _connectionString);
        //        Payment.Repo.PAYMENTTENDER paymentTenderLogic = new Payment.Repo.PAYMENTTENDER(_env, _connectionString);
        //        Payment.Repo.PAYMENTLOG paymentLogLogic = new Payment.Repo.PAYMENTLOG(_env, _connectionString);

        //        return new AjaxResponse(new { success = true, message = "" });
        //    }
        //    catch (Exception ex)
        //    {
        //        string exMsg = ex.Message.ToString();
        //        return null;
        //    }
        //}

        //public async Task<AjaxResponse> CheckPayment(string PaymentTransID, string RefID)
        //{
        //    try
        //    {
        //        var contPaymentTransID = PaymentTransID;
        //        var contRefID = RefID;
        //        Payment.Repo.PAYMENTHDR paymentHDRLogic = new Payment.Repo.PAYMENTHDR(_env, _connectionString);
        //        Payment.Repo.PAYMENTTENDER paymentTenderLogic = new Payment.Repo.PAYMENTTENDER(_env, _connectionString);
        //        Payment.Repo.TENDERTYPE tenderTypeLogic = new Payment.Repo.TENDERTYPE(_env, _connectionString);

        //        DTO.Payment.PAYMENTHDR contPaymentHDR = paymentHDRLogic.Retrieve(x => x.PaymentTransID == contPaymentTransID);
        //        IList<DTO.Payment.PAYMENTTENDER> contPaymentTender;
        //        DTO.Payment.TENDERTYPE contTenderType;
        //        if (contPaymentTransID != null)
        //        {
        //            contPaymentTender = paymentTenderLogic.RetrieveAll(x => x.PaymentTransID == contPaymentTransID);
        //            if (contPaymentTender != null)
        //            {
        //                foreach (DTO.Payment.PAYMENTTENDER index in contPaymentTender)
        //                {
        //                    contTenderType = tenderTypeLogic.Retrieve(x => x.TenderCode == index.TenderCode);
        //                    if (contTenderType != null)
        //                    {
        //                        // Check Payment by Each Tender
        //                    }
        //                }
        //            }
        //        }

        //        return new AjaxResponse(new { success = true, message = "" });
        //    }
        //    catch (Exception ex)
        //    {
        //        string exMsg = ex.Message.ToString();
        //        return null;
        //    }
        //}

        //public async Task<AjaxResponse> ReceivedResponsePayment(string PaymentTransID, string PaymentType)
        //{
        //    try
        //    {
        //        return new AjaxResponse(new { success = true, message = "" });
        //    }
        //    catch (Exception ex)
        //    {
        //        string exMsg = ex.Message.ToString();
        //        return null;
        //    }
        //}

        //public async Task<AjaxResponse> GetPayment(string PaymentTransID, string RefID)
        //{
        //    try
        //    {
        //        return new AjaxResponse(new { success = true, message = "" });
        //    }
        //    catch (Exception ex)
        //    {
        //        string exMsg = ex.Message.ToString();
        //        return null;
        //    }
        //}

        public async Task<AjaxResponse> AddTender(AddTenderModel input)
        {
            try
            {
                bool paymentSuccess = false;
                PAYMENTTENDER paymentTenderLogic = new PAYMENTTENDER(_env, _connectionString);
                PAYMENTLOG paymentLogLogic = new PAYMENTLOG(_env, _connectionString);
                DTO.Payment.PAYMENTTENDER contPaymentTender;
                DTO.Payment.PAYMENTLOG contPaymentLog;

                IList<DTO.Payment.PAYMENTTENDER> dataPaymentTender = _objectMapper.Map<IList<DTO.Payment.PAYMENTTENDER>>(input.PaymentTender);
                IList<DTO.Payment.PAYMENTLOG> dataPaymentLog = _objectMapper.Map<IList<DTO.Payment.PAYMENTLOG>>(input.PaymentLog);

                foreach (var idxTender in dataPaymentTender)
                {
                    var idxTenderPaymentTransID = idxTender.PaymentTransID;
                    var idxTenderTenderID = idxTender.TenderID;
                    var idxTenderTenderCode = idxTender.TenderCode;

                    contPaymentTender = paymentTenderLogic.Retrieve(x => x.PaymentTransID == idxTenderPaymentTransID && x.TenderID == idxTenderTenderID && x.TenderCode == idxTenderTenderCode);
                    if (contPaymentTender != null)
                    {
                        return new AjaxResponse(new ErrorInfo("Tender Data Already Exist"));
                    }
                    else
                    {
                        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        {
                            try
                            {
                                await paymentTenderLogic.Create(idxTender);

                                var seqNo = 1;
                                foreach (var idxLog in dataPaymentLog)
                                {
                                    var idxLogPaymentTransID = idxLog.PaymentTransID;
                                    var idxLogTenderID = idxLog.TenderID;
                                    var idxLogTenderCode = idxLog.TenderCode;
                                    var idxLogSeqNo = seqNo;

                                    contPaymentLog = paymentLogLogic.Retrieve(x => x.PaymentTransID == idxLogPaymentTransID && x.TenderID == idxLogTenderID && x.TenderCode == idxLogTenderCode && x.SeqNo == idxLogSeqNo);
                                    if (contPaymentLog != null)
                                    {
                                        return new AjaxResponse(new ErrorInfo("Tender Log Already Exist"));
                                    }
                                    else
                                    {
                                        using (TransactionScope scopeLog = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
                                        {
                                            try
                                            {
                                                await paymentLogLogic.Create(idxLog);
                                                paymentSuccess = true;
                                                scopeLog.Complete();
                                            }
                                            catch (Exception ex)
                                            {
                                                throw ex;
                                            }
                                        }
                                        scope.Complete();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                    }
                }
                return new AjaxResponse(paymentSuccess);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        #region FPX
        public async Task<AjaxResponse> GetFPXBankList(string fpx_msgToken)
        {
            string result = string.Empty;
            string posting_data = string.Empty;
            string fpx_msgtype = "BE";
            string fpx_sellerExId;
            string fpx_version = _appConfiguration["SUNSystem:FPXVersion"];
            string path;
            string fpx_checkSum = "";
            string checkSum_String = "";
            string bankType = string.Empty;
            string URLBank = string.Empty;
            List<FPXBankListModel> resultMap = new List<FPXBankListModel>();

            try
            {
                //B2C : fpx_msgToken = 01
                //B2B : fpx_msgToken = 02

                fpx_sellerExId = _appConfiguration["SUNSystem:SellerExId"];
                path = _appConfiguration["SUNSystem:CertificateDir"] + fpx_sellerExId + ".key";
                URLBank = _appConfiguration["SUNSystem:UrlBankList"];

                if (fpx_msgToken == "01")
                {
                    bankType = "BNC";
                }
                else if (fpx_msgToken == "02")
                {
                    bankType = "BNB";
                }

                fpx_checkSum = fpx_msgToken + "|" + fpx_msgtype + "|" + fpx_sellerExId + "|" + fpx_version;
                checkSum_String = Payment.Repo.RSASign.RSASignValue(fpx_checkSum, path);
                posting_data = "fpxmsgToken=" + fpx_msgToken +
                    "&fpx_msgToken=" + fpx_msgToken +
                    "&fpx_msgType=" + fpx_msgtype +
                    "&fpx_sellerExId=" + fpx_sellerExId +
                    "&fpx_version=" + fpx_version +
                    "&fpx_checkSum=" + checkSum_String;

                byte[] _byteVersion = Encoding.ASCII.GetBytes(string.Concat("content=", posting_data));

                var sunSystem = new Payment.Repo.SUNSystemConnection();
                result = sunSystem.POSTWebRequest(URLBank, posting_data, _byteVersion);
                if (string.IsNullOrEmpty(result))
                {
                    return new AjaxResponse(new ErrorInfo("FPX Response Null or Empty"));
                }

                if (result.ToUpper().Contains("ERROR"))
                {
                    return new AjaxResponse(new ErrorInfo("Connection to FPX was Error"));
                }

                var codemasterLogic = new Core.General.Repo.CODEMASTER(_env, _connectionString);
                var listCodemaster = codemasterLogic.RetrieveAll(x => x.CodeType == bankType);

                NameValueCollection qscoll = HttpUtility.ParseQueryString(result);
                var strbanklist = qscoll["fpx_bankList"].Replace("%7E", "~").Replace("%2C", ", ");
                var splitstrbanklist = strbanklist.Split(',');
                foreach (var index in splitstrbanklist)
                {
                    var splitVal = index.Split("~");
                    var bankCode = splitVal[0];
                    var bankName = listCodemaster != null && listCodemaster.Count() > 0 ? listCodemaster.Where(x => x.Code == bankCode).Select(y => y.CodeDesc).FirstOrDefault() : string.Empty;
                    var bankStatus = splitVal[1];

                    if (bankStatus.ToUpper() == "B")
                    {
                        bankStatus = " (OFFLINE)";
                    }
                    else
                    {
                        bankStatus = string.Empty;
                    }

                    FPXBankListModel cont = new FPXBankListModel
                    {
                        BankCode = bankCode,
                        BankDisplayName = !string.IsNullOrEmpty(bankName) ? bankName : bankCode + bankStatus
                    };
                    resultMap.Add(cont);
                    resultMap = resultMap.OrderBy(x => x.BankDisplayName).ToList();
                }

                return new AjaxResponse(resultMap);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<AjaxResponse> GetFPXCheckSum(FPXBodyModel fpxBody)
        {
            string result = string.Empty;
            string posting_data = string.Empty;
            string fpx_msgtype = "AE";
            string fpx_sellerExId = _appConfiguration["SUNSystem:SellerExId"];
            string fpx_version = _appConfiguration["SUNSystem:FPXVersion"];
            string path = _appConfiguration["SUNSystem:CertificateDir"] + fpx_sellerExId + ".key";
            string fpx_checkSum = "";
            string checkSum_String = "";

            try
            {
                fpx_checkSum = fpxBody.fpx_buyerAccNo + "|" + fpxBody.fpx_buyerBankBranch + "|" + fpxBody.fpx_buyerBankId + "|" + fpxBody.fpx_buyerEmail + "|" + fpxBody.fpx_buyerIban + "|" + fpxBody.fpx_buyerId + "|" + fpxBody.fpx_buyerName + "|";
                fpx_checkSum += fpxBody.fpx_makerName + "|" + fpxBody.fpx_msgToken + "|" + fpxBody.fpx_msgType + "|" + fpxBody.fpx_productDesc + "|" + fpxBody.fpx_sellerBankCode + "|" + fpx_sellerExId + "|";
                fpx_checkSum += fpxBody.fpx_sellerExOrderNo + "|" + fpxBody.fpx_sellerId + "|" + fpxBody.fpx_sellerOrderNo + "|" + fpxBody.fpx_sellerTxnTime + "|" + fpxBody.fpx_txnAmount + "|" + fpxBody.fpx_txnCurrency + "|" + fpx_version;
                fpx_checkSum = fpx_checkSum.Trim();
                checkSum_String = Payment.Repo.RSASign.RSASignValue(fpx_checkSum, path);

                return new AjaxResponse(new { checksum = checkSum_String, checksumData = fpx_checkSum });
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<AjaxResponse> GetFPXValidateStatus(string refInfo)
        {
            ValidateStatusModel result = new ValidateStatusModel();
            string posting_data = "";
            string[] splitInfo;
            string path = string.Empty;
            string url = string.Empty;
            string fpx_debitAuthCode = string.Empty;
            NameValueCollection qscoll;

            try
            {
                if (refInfo.Contains("|AR|"))
                {
                    splitInfo = refInfo.Replace("|AR|", "|AE|").Split('|');
                }
                else
                {
                    splitInfo = refInfo.Split('|');
                }

                if (splitInfo.Length != 20)
                {
                    result.Message = "Invalid Payment Info";
                    return new AjaxResponse(new ErrorInfo(result.Message));
                }

                if (splitInfo[9].ToUpper() != "AE")
                {
                    result.Message = "Invalid FPX Message Type";
                    return new AjaxResponse(new ErrorInfo(result.Message));
                }

                path = _appConfiguration["SUNSystem:CertificateDir"] + _appConfiguration["SUNSystem:SellerExId"] + ".key";
                url = _appConfiguration["SUNSystem:FPXTXNSTAT"];

                string fpx_buyerAccNo = splitInfo[0];
                string fpx_buyerBankBranch = splitInfo[1];
                string fpx_buyerBankId = splitInfo[2];
                string fpx_buyerEmail = splitInfo[3];
                string fpx_buyerIban = splitInfo[4];
                string fpx_buyerId = splitInfo[5];
                string fpx_buyerName = splitInfo[6];
                string fpx_makerName = splitInfo[7];
                string fpx_msgToken = splitInfo[8];
                string fpx_msgType = splitInfo[9];
                string fpx_productDesc = splitInfo[10];
                string fpx_sellerBankCode = splitInfo[11];
                string fpx_sellerExId = splitInfo[12];
                string fpx_sellerExOrderNo = splitInfo[13];
                string fpx_sellerId = splitInfo[14];
                string fpx_sellerOrderNo = splitInfo[15];
                string fpx_sellerTxnTime = splitInfo[16];
                string fpx_txnAmount = splitInfo[17];
                string fpx_txnCurrency = splitInfo[18];
                string fpx_version = splitInfo[19].Contains("&") ? splitInfo[19].Split('&')[0] : splitInfo[19];
                string fpx_checkSum = Payment.Repo.RSASign.RSASignValue(refInfo, path);

                posting_data += "fpx_msgType1=AE&fpx_msgType=AE&" + "fpx_msgToken=" + fpx_msgToken + "&" + "fpx_sellerExId=" + fpx_sellerExId + "&";
                posting_data += "fpx_sellerExOrderNo=" + fpx_sellerExOrderNo + "&" + "fpx_sellerTxnTime=" + fpx_sellerTxnTime + "&";
                posting_data += "fpx_sellerOrderNo=" + fpx_sellerOrderNo + "&" + "fpx_sellerBankCode=" + fpx_sellerBankCode + "&";
                posting_data += "fpx_txnCurrency=" + fpx_txnCurrency + "&" + "fpx_txnAmount=" + fpx_txnAmount + "&";
                posting_data += "fpx_buyerEmail=" + fpx_buyerEmail + "&" + "fpx_checkSum=" + fpx_checkSum + "&";
                posting_data += "fpx_buyerName=" + fpx_buyerName + "&" + "fpx_buyerBankId=" + fpx_buyerBankId + "&";
                posting_data += "fpx_buyerBankBranch=" + fpx_buyerBankBranch + "&" + "fpx_buyerAccNo=" + fpx_buyerAccNo + "&";
                posting_data += "fpx_buyerId=" + fpx_buyerId + "&" + "fpx_makerName=" + fpx_makerName + "&";
                posting_data += "fpx_buyerIban=" + fpx_buyerIban + "&" + "fpx_productDesc=" + fpx_productDesc + "&";
                posting_data += "fpx_version=" + fpx_version + "&" + "fpx_sellerId=" + fpx_sellerId + "&";
                posting_data += "checkSum_String=" + fpx_checkSum;

                byte[] _byteVersion = Encoding.ASCII.GetBytes(string.Concat("content=", posting_data));

                var sunSystem = new Payment.Repo.SUNSystemConnection();
                result.CheckSum = sunSystem.POSTWebRequest(url, posting_data, _byteVersion);
                if (string.IsNullOrEmpty(result.CheckSum))
                {
                    result.Message = "No return from FPX";
                    result.DebitAuthCode = "99";
                    return new AjaxResponse(new ErrorInfo(result.DebitAuthCode, result.Message));
                }

                if (result.CheckSum.ToUpper().Contains("ERROR"))
                {
                    result.Message = result.CheckSum;
                    result.DebitAuthCode = "99";
                    return new AjaxResponse(new ErrorInfo(result.DebitAuthCode, result.Message));
                }

                var clearView = result.CheckSum.Replace("&", "&" + System.Environment.NewLine);

                qscoll = HttpUtility.ParseQueryString(result.CheckSum);
                if (qscoll != null)
                {
                    fpx_debitAuthCode = qscoll["fpx_debitAuthCode"];
                }

                if (!string.IsNullOrEmpty(fpx_debitAuthCode))
                {
                    result.DebitAuthCode = fpx_debitAuthCode;

                    var codemasterLogic = new Core.General.Repo.CODEMASTER(_env, _connectionString);
                    var listCodemaster = codemasterLogic.RetrieveAll(x => x.CodeType == "FPX");
                    if (listCodemaster != null && listCodemaster.Count() > 0)
                    {
                        result.Message = listCodemaster.Where(x => x.Code == result.DebitAuthCode).Select(y => y.CodeDesc).FirstOrDefault();
                    }
                }
                return new AjaxResponse(result);
            }
            catch (Exception ex)
            {
                //result.Message = ex.Message.ToString();
                //result.DebitAuthCode = "99";
                //result.CheckSum = string.Empty;
                //Log.Error(nameof(AccountingController), ex);
                //return new AjaxResponse(result);
                throw ex;
            }
        }

        public async Task<AjaxResponse> IndResponse(FPXResponseModel fpxData)
        {
            try
            {
                bool responseSuccess = false;

                return new AjaxResponse(responseSuccess);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<AjaxResponse> DirResponse(FPXResponseModel fpxData)
        {
            try
            {
                bool responseSuccess = false;

                return new AjaxResponse(responseSuccess);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region MayBank Credit Card
        public async Task<AjaxResponse> GetMayBankCCSignature(string MerchantCode, string Amount)
        {
            string result = string.Empty;
            string MERCHANT_HASHKEY = string.Empty;
            string MERCHANT_ACC_NO = string.Empty;
            string strConcat = string.Empty;
            string mbbCheckSum = string.Empty;
            try
            {
                MERCHANT_HASHKEY = _appConfiguration["SUNSystem:MBBCCHashKey"];
                MERCHANT_ACC_NO = _appConfiguration["SUNSystem:MBBCCAccount"];

                mbbCheckSum = MERCHANT_HASHKEY + "|" + MERCHANT_ACC_NO + "|" + MerchantCode + "|" + Amount;
                mbbCheckSum = mbbCheckSum.Trim();

                strConcat = string.Concat(MERCHANT_HASHKEY, MERCHANT_ACC_NO, MerchantCode, Amount);
                result = Payment.Repo.RSASign.GetSignature(strConcat);

                return new AjaxResponse(new { CheckSum = mbbCheckSum, TXN = strConcat, TXN_SIGNATURE = result });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<AjaxResponse> GetMayBankCCValidateStatus(string refInfo)
        {
            ValidateStatusModel result = new ValidateStatusModel();
            string posting_data = "";
            string[] splitInfo;
            string path = string.Empty;
            string url = string.Empty;
            string TXN_STATUS = string.Empty;
            string RESPONSE_CODE = string.Empty;
            string RESPONSE_DESC = string.Empty;
            string returnUrl = string.Empty;
            NameValueCollection qscoll;

            try
            {
                if (refInfo.Contains("|"))
                {
                    splitInfo = refInfo.Split('|');
                }
                else
                {
                    result.Message = "Invalid Payment Info";
                    return new AjaxResponse(new ErrorInfo(result.Message));
                }

                if (splitInfo.Length != 4)
                {
                    result.Message = "Invalid Payment Info";
                    return new AjaxResponse(new ErrorInfo(result.Message));
                }

                url = _appConfiguration["SUNSystem:MBBQueryURL"];
                returnUrl = _appConfiguration["SUNSystem:MBBReturnURL"];

                string MERCHANT_HASHKEY = splitInfo[0];
                string MERCHANT_ACC_NO = splitInfo[1];
                string MERCHANT_TRANID = splitInfo[2];
                string AMOUNT = splitInfo[3];
                string TRANSACTION_TYPE = "1";
                string TXN_SIGNATURE = Payment.Repo.RSASign.GetSignature(string.Concat(MERCHANT_HASHKEY, MERCHANT_ACC_NO, MERCHANT_TRANID, AMOUNT));
                string RESPONSE_TYPE = "PLAIN";
                string RETURN_URL = returnUrl;

                posting_data += "MERCHANT_ACC_NO=" + MERCHANT_ACC_NO + "&MERCHANT_TRANID=" + MERCHANT_TRANID + "&AMOUNT=" + AMOUNT;
                posting_data += "&TRANSACTION_TYPE=" + TRANSACTION_TYPE + "&TXN_SIGNATURE=" + TXN_SIGNATURE;
                posting_data += "&RESPONSE_TYPE=" + RESPONSE_TYPE + "&RETURN_URL=" + RETURN_URL;
                byte[] _byteVersion = Encoding.ASCII.GetBytes(posting_data);

                var sunSystem = new Payment.Repo.SUNSystemConnection();
                result.CheckSum = sunSystem.POSTWebRequest(url, posting_data, _byteVersion);
                if (string.IsNullOrEmpty(result.CheckSum))
                {
                    result.Message = "No return from MayBank";
                    result.DebitAuthCode = "99";
                    return new AjaxResponse(new ErrorInfo(result.DebitAuthCode, result.Message));
                }

                result.CheckSum = result.CheckSum.Replace("<BR>", "&");

                if (result.CheckSum.ToUpper().Contains("ERROR"))
                {
                    result.Message = result.CheckSum;
                    result.DebitAuthCode = "99";
                    return new AjaxResponse(new ErrorInfo(result.DebitAuthCode, result.Message));
                }

                var clearView = result.CheckSum.Replace("&", "&" + System.Environment.NewLine);

                qscoll = HttpUtility.ParseQueryString(result.CheckSum);
                if (qscoll != null)
                {
                    TXN_STATUS = qscoll["TXN_STATUS"];
                    RESPONSE_CODE = qscoll["RESPONSE_CODE"];
                    RESPONSE_DESC = qscoll["RESPONSE_DESC"];
                }

                if (string.IsNullOrEmpty(TXN_STATUS))
                {
                    result.Message = "No return from MayBank";
                    result.DebitAuthCode = "99";
                    return new AjaxResponse(new ErrorInfo(result.DebitAuthCode, result.Message));
                }

                if (string.IsNullOrEmpty(RESPONSE_CODE))
                {
                    result.Message = "No return from MayBank";
                    result.DebitAuthCode = "99";
                    return new AjaxResponse(new ErrorInfo(result.DebitAuthCode, result.Message));
                }

                result.Message = RESPONSE_DESC;
                result.DebitAuthCode = "99";

                if (TXN_STATUS.ToUpper() == "C")
                {
                    if (RESPONSE_CODE == "0")
                    {
                        result.DebitAuthCode = "00";
                    }
                }
                return new AjaxResponse(new ErrorInfo(result.DebitAuthCode, result.Message));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<AjaxResponse> IndCCResponse(MayBankCCResponseModel ccData)
        {
            try
            {
                bool responseSuccess = false;

                return new AjaxResponse(responseSuccess);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #endregion
    }
}