using System;
using System.Threading.Tasks;
using System.Text;
using System.Web;
using System.IO;
using Abp.Localization;
using Abp.Runtime.Security;
using Plexform.Authorization.Users;
using Plexform.Authorization.Roles;
using Plexform.MultiTenancy;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using Abp.ObjectMapping;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plexform.Base.Core;
using Plexform.Base.Core.Helper;
using Plexform.Base.Core.Entity.Repo;
using Plexform.Base.Core.Entity.Model;
using Plexform.Base.Core.General;
using Plexform.Base.Core.Entity;
using Plexform.Base.Accounting.Repo;
using Plexform.Base.Accounting.Map;
using Plexform.Base.Accounting.Model;
using Plexform.Base.Accounting.Filter;
using Plexform.Base.Accounting.XML;
using System.Collections.Generic;
using System.Collections.Specialized;
using Abp.Web.Models;
using System.Transactions;
using LOGIC.Shared.Collection;
using Microsoft.AspNetCore.Mvc;
using Hangfire;
using Plexform.Base.Core.General.Repo;
using Microsoft.Extensions.Configuration;
using Plexform.Notifications;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Plexform.DTO.Questil.Payment;
using Twilio.Http;
using System.Threading;

namespace Plexform.Base.Accounting
{
    public class AccountingManager : BaseClass
    {
        private JObject appsettingsjson;
        private JObject InvoiceConfig;
        private string _serverExtAddress;
        private readonly ITxtExporter _txtExporter;
        private readonly IRealTimeCommunicator _realtimeService;
        private ILocalizationManager _localizationManager;
        private readonly string _OverdueInterval;
        private readonly GeneralManager _generalManager;
        private readonly EntityManager _entityManager;
        private readonly AccountingEmailer _accountingEmailer;
        
        private readonly Plexform.INT.SAP.KA.IntegrationManager _sapManager;
        private readonly Repo<DTO.Questil.Payment.INVOICEHDR_CWMS> _invoiceHDRRepo;

        public AccountingManager(
            IWebHostEnvironment env,
            IAppFolders appFolders,
            IObjectMapper objectMapper,
            ILocalizationManager localizationManager,
            IExcelExporter excelExporter,
            ITxtExporter txtExporter,
            IRealTimeCommunicator realtimeService,
            RoleManager roleManager,
            UserManager userManager,
            TenantManager tenantManager,
            GeneralManager generalManager,
            EntityManager entityManager,
            AccountingEmailer accountingEmailer,
            Repo<DTO.Questil.Payment.INVOICEHDR_CWMS> invoiceHDRRepo,
            Plexform.INT.SAP.KA.IntegrationManager sapManager)
            : base(env, appFolders, objectMapper, roleManager, userManager, tenantManager, excelExporter, localizationManager)
        {
            _localizationManager = localizationManager;
            _generalManager = generalManager;
            _entityManager = entityManager;
            _realtimeService = realtimeService;
            _OverdueInterval = _appConfiguration["InvoiceConfig:OverdueInterval"];
            _sapManager = sapManager;
            _txtExporter = txtExporter;
            _invoiceHDRRepo = invoiceHDRRepo;
            _accountingEmailer = accountingEmailer;
        }

        #region GetAllOutstanding
        public async Task<AjaxResponse> JobGetAllOutstanding(string customerCode, Int32 limit, Int32 offset)
        {
            try
            {
                BackgroundJob.Enqueue(() => GetAllOutstanding(customerCode, limit, offset));
                return new AjaxResponse(new { message = "Success Getting All Outstanding" });
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }

        public async Task<AjaxResponse> ScheduleJobGetAllOutstanding()
        {
            try
            {
                var CRONValue = _appConfiguration["CRONConfig:CRONValue"];
                RecurringJob.AddOrUpdate(() => DailyGetAllOutstanding(), CRONValue, TimeZoneInfo.Local);
                return new AjaxResponse(new { message = "Success Scheduling Job" });
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }

        public async Task<AjaxResponse> GetProformaBillById(string billId)
        {
            Map.Envelope requestEnvelope = new Map.Envelope();
            Map.Envelope responseEnvelope = new Map.Envelope();
            Model.Envelope data = new Model.Envelope();

            try
            {
                requestEnvelope = new Map.Envelope
                {
                    Body = new Map.Body
                    {
                        GetProformaBillById = new Map.GetProformaBillById
                        {
                            AuthenticationDTO = new Map.AuthenticationDTO
                            {
                                Username = _appConfiguration["SUNSystem:Username"],
                                Password = _appConfiguration["SUNSystem:Password"]
                            },
                            BillId = billId
                        }
                    }
                };

                var sunSystem = new SUNSystemConnection();
                responseEnvelope = await sunSystem.XMLSender("getProformaBillById", requestEnvelope, "getProformaBillById");
                if (responseEnvelope != null)
                {
                    var strJson = JsonConvert.SerializeObject(responseEnvelope);
                    if (!string.IsNullOrEmpty(strJson))
                    {
                        data = JsonConvert.DeserializeObject<Model.Envelope>(strJson);
                    }
                }

                var result = data.Body.GetProformaBillByIdResponse.Return;
                return new AjaxResponse(new { items = result });
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }

        public async Task<AjaxResponse> GetCreditBillById(string billId)
        {
            Map.Envelope requestEnvelope = new Map.Envelope();
            Map.Envelope responseEnvelope = new Map.Envelope();
            Model.Envelope data = new Model.Envelope();

            try
            {
                requestEnvelope = new Map.Envelope
                {
                    Body = new Map.Body
                    {
                        GetCreditBillById = new Map.GetCreditBillById
                        {
                            AuthenticationDTO = new Map.AuthenticationDTO
                            {
                                Username = _appConfiguration["SUNSystem:Username"],
                                Password = _appConfiguration["SUNSystem:Password"]
                            },
                            BillId = billId
                        }
                    }
                };

                var sunSystem = new SUNSystemConnection();
                responseEnvelope = await sunSystem.XMLSender("getCreditBillById", requestEnvelope, "getCreditBillById", "getCreditBillById");
                if (responseEnvelope != null)
                {
                    var strJson = JsonConvert.SerializeObject(responseEnvelope);
                    if (!string.IsNullOrEmpty(strJson))
                    {
                        data = JsonConvert.DeserializeObject<Model.Envelope>(strJson);
                    }
                }

                var result = data.Body.GetCreditBillByIdResponse.Return;
                return new AjaxResponse(new { items = result });
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }

        public async Task<AjaxResponse> GetCreditBills(Int32 isView, GetCreditBills input)
        {
            try
            {
                var _connStr = _appConfiguration.GetConnectionString("Conn1") + ";Enlist=false";
                var scheduleLogic = new PAYMENT(_env, _connStr);
                scheduleLogic.Select<MapListCreditBills>(x =>
                    new
                    {
                        x.Description,
                        x.BillNo,
                        x.TReference,
                        x.D_C,
                        x.Add_Code,
                        x.Rec_Type,
                        x.Trans_Date,
                        x.Amount,
                        x.Accnt_Code,
                        x.Jrnal_No,
                        x.Trans_Ref,
                        x.Sun_Db,
                        x.Allocation,
                        x.Inv_Date,
                        x.Status,
                        x.Comments,
                        x.Cust_Code,
                        x.Trans_Val,
                        x.Del_Date,
                        x.Anal_T1,
                        x.Cust_Ref,
                        x.Address_1,
                        x.Address_2,
                        x.Address_3,
                        x.Address_4,
                        x.Address_5,
                        x.Address_6,
                        x.E_Mail
                    });

                if (isView == 1)
                {
                    var data = scheduleLogic.StoredProcedureView<GetCreditBills, MapListCreditBills>("dbo.GetCreditBills", input);
                    data.TotalCount = data.Items.Count();
                    return new AjaxResponse(data);
                }
                else
                {
                    var dataMap = scheduleLogic.StoredProcedureViewMap<MapListCreditBills, GetCreditBills>("dbo.GetCreditBills", input).ToList();
                    return new AjaxResponse(dataMap);
                }
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }

        public async Task<AjaxResponse> GetCreditBillsDetail(Int32 detail, GetCreditBillDetails input)
        {
            try
            {
                var _connStr = _appConfiguration.GetConnectionString("Conn1") + ";Enlist=false";
                var scheduleLogic = new PAYMENT(_env, _connStr);
                scheduleLogic.Select<MapListCreditBillsDetail>(x =>
                    new
                    {
                        x.REC_TYPE,
                        x.TRANS_REF,
                        x.INV_NO,
                        x.ACCNT_CODE,
                        x.DESCRIPTN,
                        x.VALUE_1,
                        x.VALUE_2,
                        x.VALUE_10,
                        x.INV_DATE,
                        x.ID_ENTERED,
                        x.ID_INVOICED,
                        x.ANAL_M1,
                        x.ANAL_M3,
                        x.ANAL_M6,
                        x.ANAL_M9
                    });
                if (detail == 1)
                {
                    var data = scheduleLogic.StoredProcedureView<GetCreditBillDetails, MapListCreditBillsDetail>("dbo.GetCreditBillsDetail", input);
                    data.TotalCount = data.Items.Count();
                    return new AjaxResponse(data);
                }
                else
                {
                    var dataMap = scheduleLogic.StoredProcedureViewMap<MapListCreditBillsDetail, GetCreditBillDetails>("dbo.GetCreditBillsDetail", input).ToList();
                    return new AjaxResponse(dataMap);
                }
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }

        public async Task<AjaxResponse> GetCreditBillsTax(Int32 isView, GetCreditBillDetails input)
        {
            try
            {
                var _connStr = _appConfiguration.GetConnectionString("Conn1") + ";Enlist=false";
                var scheduleLogic = new PAYMENT(_env, _connStr);
                scheduleLogic.Select<MapCreditBillTax>(x =>
                    new
                    {
                        x.AMOUNT
                    });
                if (isView == 1)
                {
                    var data = scheduleLogic.StoredProcedureView<GetCreditBillDetails, MapCreditBillTax>("dbo.GetCreditBillsTax", input);
                    data.TotalCount = data.Items.Count();
                    return new AjaxResponse(data);
                }
                else
                {
                    var dataMap = scheduleLogic.StoredProcedureViewMap<MapCreditBillTax, GetCreditBillDetails>("dbo.GetCreditBillsTax", input).ToList();
                    return new AjaxResponse(dataMap);
                }
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }

        public async Task<AjaxResponse> DailyGetAllOutstanding()
        {
            try
            {
                BIZENTITY bizEntityLogic = new BIZENTITY(_env, _connectionString);
                List<DTO.Core.Entity.BIZENTITY> acctNoList = new List<DTO.Core.Entity.BIZENTITY>();
                string[] exAccNo = { };

                var exAccList = await _generalManager.GetListMaintenance("06", "WORKINGACCOUNT");
                if (exAccList.Success)
                {
                    var accList = (dynamic)exAccList.Result;
                    List<string> list = new List<string>();
                    list.Add("000000");
                    foreach (var idx in accList.Items)
                    {
                        list.Add(idx["SYSValue"]);
                    }
                    exAccNo = list.ToArray();
                }

                acctNoList = bizEntityLogic.RetrieveAll(x => x.Status == 1 && !exAccNo.Equals(x.AcctNo));
                if (acctNoList != null)
                {
                    foreach (var index in acctNoList)
                    {
                        await GetAllOutstanding(index.AcctNo, 1000, 0);
                    }
                    return new AjaxResponse(new { message = "Success Daily Sync GetAllOutstanding" });
                }
                else
                {
                    return new AjaxResponse(new ErrorInfo("There is no active Account Number"));
                }
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }

        public async Task<AjaxResponse> GetAllOutstanding(string customerCode, Int32 limit, Int32 offset)
        {
            Map.Envelope requestEnvelope = new Map.Envelope();
            Map.Envelope responseEnvelope = new Map.Envelope();
            SUNSystemConnection sunSystem = new SUNSystemConnection();
            BIZENTITY bizEntityLogic;
            BIZLOCATE bizLocateLogic;
            INVOICEHDR invoiceHDRLogic;
            INVOICEDTL invoiceDTLLogic;
            INVOICETAX invoiceTaxLogic;
            string tempBizRegID;
            string tempBizLocID;
            string contCustomerCode = customerCode.ToUpper();
            List<string> siNumberInList = new List<string>();
            List<string> siNumberNotInList = new List<string>();
            List<string> itemNumberInList = new List<string>();
            List<Map.Data> contResDataAPI;
            List<dynamic> contResDataSunDB = new List<dynamic>();

            try
            {
                requestEnvelope = new Map.Envelope
                {
                    Body = new Map.Body
                    {
                        GetAllOutStandingBillByCustomerCode = new Map.GetAllOutStandingBillByCustomerCode
                        {
                            AuthenticationDTO = new Map.AuthenticationDTO
                            {
                                Username = _appConfiguration["SUNSystem:Username"],
                                Password = _appConfiguration["SUNSystem:Password"]
                            },
                            Config = new Map.Config
                            {
                                CustomerCode = contCustomerCode,
                                Limit = limit.ToString(),
                                Offset = offset.ToString()
                            }
                        }
                    }
                };

                responseEnvelope = await sunSystem.XMLSender("getAllOutStandingBillByCustomerCode", requestEnvelope, "getAllOutStandingBillByCustomerCode", "getAllOutStandingBillByCustomerCode");

                contResDataAPI = responseEnvelope.Body.GetAllOutStandingBillByCustomerCodeResponse.ReturnGetAllOutStandingBillByCustomerCodeResponse.Data;

                #region Pull from SUNDB
                invoiceHDRLogic = new INVOICEHDR(_env, _connectionString);
                var creditBillSunDBList = invoiceHDRLogic
                    .Select<DTO.Accounting.INVOICEHDR, DTO.Accounting.TRANSHDR>((IH, TH) => new
                    {
                        IH.TransNo
                    })
                    .InnerJoin<DTO.Accounting.TRANSHDR>(IH => IH.TransNo, TH => TH.InSvcID)
                    .Where<DTO.Accounting.INVOICEHDR, DTO.Accounting.TRANSHDR>((IH, TH) =>
                        TH.Status != 1 &&
                        TH.AcctNo == contCustomerCode
                    )
                    .GroupBy<DTO.Accounting.INVOICEHDR, DTO.Accounting.TRANSHDR>((IH, TH) => new
                    {
                        IH.TransNo,
                        IH.BillNo
                    })
                    .OrderBy(IH => IH.BillNo)
                    .ExecuteList<MapINVOICEDTL>();

                if (creditBillSunDBList != null && creditBillSunDBList.TotalCount > 0)
                {
                    if (creditBillSunDBList.Items != null && creditBillSunDBList.Items.Count() > 0)
                    {
                        siNumberNotInList = creditBillSunDBList.Items.Select(x => (string)x.TransNo).ToList();
                    }
                }

                if (contResDataAPI != null && contResDataAPI.Count > 0)
                {
                    foreach (var index in contResDataAPI)
                    {
                        if (index.Status == "Accepted")
                        {
                            siNumberInList.Add(index.SiNumber);
                        }
                    }
                }

                var codemasterLogic = new Core.General.Repo.CODEMASTER(_env, _connectionString);
                var departmentList = codemasterLogic.RetrieveAll(x => x.CodeType == "DPT");
                if (departmentList != null && departmentList.Count() > 0)
                {
                    foreach (var index in departmentList)
                    {
                        var contCreditBills = new GetCreditBills
                        {
                            CustomerCode = contCustomerCode,
                            DateFrom = string.Empty, // DateTime.Today.AddYears(-1).ToString("yyyyMMdd"),
                            DateTo = string.Empty, // DateTime.Today.ToString("yyyyMMdd"),
                            //SiNumberIn = string.Join(",", contResDataAPI.Select(x => x.SiNumber).ToList()),
                            SiNumberIn = siNumberInList != null && siNumberInList.Count() > 0 ? string.Join("','", siNumberInList) : string.Empty,
                            SiNumberNotIn = siNumberNotInList != null && siNumberNotInList.Count() > 0 ? string.Join("','", siNumberNotInList) : string.Empty,
                            Type = index.Code
                        };

                        var resCreditBills = await GetCreditBills(0, contCreditBills);
                        if (resCreditBills != null)
                        {
                            if (resCreditBills.Success)
                            {
                                var resMap = (List<MapListCreditBills>)resCreditBills.Result;
                                if (resMap != null)
                                {
                                    if (resMap.Count() > 0)
                                    {
                                        foreach (var indexMap in resMap)
                                        {
                                            contResDataSunDB.Add(new
                                            {
                                                Amount = indexMap.Amount,
                                                Balance = indexMap.Trans_Val,
                                                BillNo = indexMap.BillNo,
                                                CompanyID = indexMap.Add_Code,
                                                Company = indexMap.Address_1,
                                                Date = DateTime.ParseExact(indexMap.Inv_Date, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"),
                                                Department = indexMap.Sun_Db,
                                                Detail = indexMap.Sun_Db + "/" + indexMap.Description,
                                                Gst = 0,
                                                Id = indexMap.Jrnal_No,
                                                Rounding = 0,
                                                SiNumber = indexMap.Trans_Ref, // indexMap.TReference,
                                                Status = "Accepted",
                                                Type = "Credit BIll",
                                                CustomerRef = indexMap.Cust_Ref,
                                                CostCenter = indexMap.Anal_T1,
                                                SunDB = contCreditBills.Type
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion Pull from SUNDB

                bizEntityLogic = new BIZENTITY(_env, _connectionString);
                bizLocateLogic = new BIZLOCATE(_env, _connectionString);
                invoiceHDRLogic = new INVOICEHDR(_env, _connectionString);
                invoiceDTLLogic = new INVOICEDTL(_env, _connectionString);
                invoiceTaxLogic = new INVOICETAX(_env, _connectionString);

                var dataBizEntity = bizEntityLogic.Retrieve(x => x.AcctNo == contCustomerCode);
                if (dataBizEntity != null)
                {
                    tempBizRegID = dataBizEntity.BizRegID;
                }
                else
                {
                    return new AjaxResponse(new { success = false, message = "Company Not Found" });
                }
                var dataBizLocate = bizLocateLogic.Retrieve(x => x.BizRegID == tempBizRegID);
                if (dataBizLocate != null)
                {
                    tempBizLocID = dataBizLocate.BizLocID;
                }
                else
                {
                    return new AjaxResponse(new { success = false, message = "Company Branch Not Found" });
                }

                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    #region e-Billing
                    foreach (var index in contResDataAPI)
                    {
                        if (index.Status.Contains("Accepted"))
                        {
                            var contInvHdr = new DTO.Accounting.INVOICEHDR();
                            DTO.Accounting.INVOICEDTL contInvDtl = new DTO.Accounting.INVOICEDTL();
                            DTO.Accounting.INVOICETAX contInvTax = new DTO.Accounting.INVOICETAX();
                            string contType = string.Empty;
                            AjaxResponse getDetails = null;
                            //AjaxResponse resDetails;
                            var resMap = (dynamic)getDetails;
                            byte contStatus = 0;
                            string contDepartmentType = string.Empty;

                            contStatus = 1;

                            //if (index.Type == "Credit Bill")
                            //{
                            //    contType = "CDT";
                            //    getDetails = await GetCreditBillById(index.Id);
                            //}
                            if (index.Type == "Proforma Bill")
                            {
                                contType = "PRF";
                                getDetails = await GetProformaBillById(index.Id);
                            }

                            if (!string.IsNullOrEmpty(index.Detail))
                            {
                                var splitDetail = index.Detail.Split("/");
                                if (splitDetail != null && splitDetail.Count() > 0)
                                {
                                    contDepartmentType = splitDetail[0];
                                }
                            }

                            if (contType == "PRF")
                            {
                                index.SiNumber = index.SiNumber.Length < 3 ? index.BillNo : index.SiNumber;

                                contInvHdr = new DTO.Accounting.INVOICEHDR
                                {
                                    BizRegID = tempBizRegID,
                                    BizLocID = tempBizLocID,
                                    CustomerID = contCustomerCode,
                                    Active = 1,
                                    BillNo = index.BillNo,
                                    TransNo = index.SiNumber,
                                    CreateBy = "SYSTEM",
                                    CreateDate = DateTime.Now,
                                    Flag = 1,
                                    Inuse = 0,
                                    IsHost = 0,
                                    ShiftCode = contType,
                                    //ShiftCode = DateTime.ParseExact(index.Date, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyyMM"),
                                    PostDate = DateTime.ParseExact(index.Date, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                                    InSvcID = index.Id,
                                    Status = contStatus,
                                    TransChgAmt = Convert.ToDecimal(index.Balance),
                                    TransAmt = Convert.ToDecimal(index.Amount),
                                    TransAmtRnd = Convert.ToDecimal(index.Rounding),
                                    TransAmtOrg = Convert.ToDecimal(index.Gst),
                                    TransRemark = index.Detail,
                                    SyncCreate = DateTime.Now,
                                    LastUpdate = null,
                                    SyncLastUpd = null,
                                    LastSyncBy = null,
                                    UpdateBy = string.Empty
                                };

                                var siNumber = index.SiNumber;
                                var dataInvHdr = invoiceHDRLogic.Retrieve(x => x.TransNo == siNumber && x.BizRegID == tempBizRegID && x.BizLocID == tempBizLocID);
                                if (dataInvHdr == null)
                                {
                                    await invoiceHDRLogic.Create(contInvHdr);
                                }
                                else if (dataInvHdr.Status != 2)
                                {
                                    contInvHdr.CreateBy = dataInvHdr.CreateBy;
                                    contInvHdr.CreateDate = dataInvHdr.CreateDate;
                                    contInvHdr.SyncCreate = dataInvHdr.SyncCreate;
                                    contInvHdr.LastUpdate = DateTime.Now;
                                    contInvHdr.SyncLastUpd = DateTime.Now;
                                    contInvHdr.LastSyncBy = 1;
                                    contInvHdr.UpdateBy = "SYSTEM";
                                    await invoiceHDRLogic.Update(contInvHdr);
                                }

                                if (getDetails.Success)
                                {
                                    resMap = getDetails.Result;

                                    contInvHdr = invoiceHDRLogic.Retrieve(x => x.TransNo == siNumber && x.BizRegID == tempBizRegID && x.BizLocID == tempBizLocID);
                                    if (contInvHdr != null)
                                    {
                                        contInvHdr.CustPkgID = resMap.items.CustomerReference;
                                        contInvHdr.CashierID = resMap.items.Attention;
                                        contInvHdr.ServerID = resMap.items.DivisionReference;
                                        contInvHdr.TransDiscReasonCode = resMap.items.ProjectDTO.Code;
                                        contInvHdr.TransDiscRemark = resMap.items.CreditVoteDTO.Code;
                                        contInvHdr.AcctNo = resMap.items.LabReference;
                                        //contInvHdr.TransReasonCode = resMap.items.Deliveryreference.Nil;
                                        contInvHdr.TransReasonCode = "SF";
                                        contInvHdr.SpDiscReasonCode = resMap.items.CreditVoteDTO.TaxCategoryDTO.Code;
                                        contInvHdr.TblNo = resMap.items.DatabaseDTO.Name;
                                        await invoiceHDRLogic.Update(contInvHdr);
                                    }

                                    var transSeqNo = 1;
                                    foreach (var details in resMap.items.Items)
                                    {
                                        contInvDtl = new DTO.Accounting.INVOICEDTL
                                        {
                                            BizRegID = tempBizRegID,
                                            BizLocID = tempBizLocID,
                                            TransNo = index.SiNumber,
                                            BillNo = index.BillNo,
                                            TransSeq = transSeqNo,
                                            ItemCode = details.Code,
                                            Remark = details.Description,
                                            Qty = Convert.ToInt32(details.Quantity),
                                            NettPrice = Convert.ToDecimal(details.UnitPrice),
                                            SerialNo = details.Id,
                                            CreateBy = "SYSTEM",
                                            CreateDate = DateTime.Now,
                                            Active = 1,
                                            Inuse = 0,
                                            IsHost = 0,
                                            Flag = 1,
                                            SyncCreate = DateTime.Now,
                                            LastUpdate = null,
                                            SyncLastUpd = null,
                                            LastSyncBy = null,
                                            UpdateBy = string.Empty
                                        };

                                        itemNumberInList.Add(contInvDtl.ItemCode);

                                        var tempItemCode = contInvDtl.ItemCode;
                                        var dataInvDtl = invoiceDTLLogic.Retrieve(x => x.TransNo == siNumber && x.ItemCode == tempItemCode && x.TransSeq == transSeqNo && x.BizRegID == tempBizRegID && x.BizLocID == tempBizLocID);
                                        if (dataInvDtl == null)
                                        {
                                            await invoiceDTLLogic.Create(contInvDtl);
                                            transSeqNo++;
                                        }
                                        else if (dataInvDtl.Status != 2)
                                        {
                                            contInvDtl.BizRegID = dataInvDtl.BizRegID;
                                            contInvDtl.CreateBy = dataInvDtl.CreateBy;
                                            contInvDtl.CreateDate = dataInvDtl.CreateDate;
                                            contInvDtl.SyncCreate = dataInvDtl.CreateDate;
                                            contInvDtl.UpdateBy = "SYSTEM";
                                            contInvDtl.SyncLastUpd = DateTime.Now;
                                            contInvDtl.LastUpdate = DateTime.Now;
                                            contInvDtl.LastSyncBy = "SYSTEM";
                                            await invoiceDTLLogic.Update(contInvDtl);
                                            transSeqNo++;
                                        }
                                    }

                                    #region CreditBill Tax
                                    //GetCreditBillDetails dtlInput = new GetCreditBillDetails();
                                    //List<string> dept = new List<string>() { "CSA", "WAC" };

                                    //foreach (var indx in dept)
                                    //{
                                    //    dtlInput.Type = indx;
                                    //    dtlInput.InvoiceNo = index.SiNumber;

                                    //    var resCreditBillsTax = await GetCreditBillsTax(0, dtlInput);

                                    //    if (resCreditBillsTax.Success)
                                    //    {
                                    //        var res = (List<MapCreditBillTax>)resCreditBillsTax.Result;
                                    //        if (res != null && res[0].AMOUNT != null)
                                    //        {
                                    //            foreach (var indexMap in resMap)
                                    //            {
                                    //                var tmpType = dtlInput.Type;
                                    //                var tmpTransNo = dtlInput.InvoiceNo;
                                    //                contInvTax = new DTO.Accounting.INVOICETAX
                                    //                {
                                    //                    BizRegID = tempBizRegID,
                                    //                    BizLocID = tempBizLocID,
                                    //                    TransNo = tmpTransNo,
                                    //                    TaxCode = 6,
                                    //                    TaxAmt = Convert.ToDecimal(indexMap.AMOUNT),
                                    //                    TaxRemark = "SST",
                                    //                    CreateBy = "SYSTEM",
                                    //                    CreateDate = DateTime.Now,
                                    //                    Active = 1,
                                    //                    Inuse = 0,
                                    //                    IsHost = 0,
                                    //                    SyncCreate = DateTime.Now,
                                    //                    SyncLastUpd = null,
                                    //                    LastSyncBy = null
                                    //                };

                                    //                var dataInvTax = invoiceTaxLogic.Retrieve(x => x.TransNo == tmpTransNo && x.BizRegID == tempBizRegID && x.BizLocID == tempBizLocID);
                                    //                if (dataInvTax == null)
                                    //                {
                                    //                    await invoiceTaxLogic.Create(contInvTax);
                                    //                }
                                    //                else
                                    //                {
                                    //                    contInvTax.BizRegID = dataInvTax.BizRegID;
                                    //                    contInvTax.CreateBy = dataInvTax.CreateBy;
                                    //                    contInvTax.CreateDate = dataInvTax.CreateDate;
                                    //                    contInvTax.SyncCreate = dataInvTax.CreateDate;
                                    //                    contInvTax.SyncLastUpd = DateTime.Now;
                                    //                    await invoiceTaxLogic.Update(contInvTax);
                                    //                }
                                    //            }
                                    //        }
                                    //        else
                                    //        {
                                    //            var tmpType = dtlInput.Type;
                                    //            var tmpTransNo = dtlInput.InvoiceNo;
                                    //            contInvTax = new DTO.Accounting.INVOICETAX
                                    //            {
                                    //                BizRegID = tempBizRegID,
                                    //                BizLocID = tempBizLocID,
                                    //                TransNo = tmpTransNo,
                                    //                TaxCode = 0,
                                    //                TaxRate = 0,
                                    //                TaxAmt = 0,
                                    //                TaxRemark = "SS0",
                                    //                CreateBy = "SYSTEM",
                                    //                CreateDate = DateTime.Now,
                                    //                Active = 1,
                                    //                Inuse = 0,
                                    //                IsHost = 0,
                                    //                SyncCreate = DateTime.Now,
                                    //                SyncLastUpd = null,
                                    //                LastSyncBy = null
                                    //            };

                                    //            var dataInvTax = invoiceTaxLogic.Retrieve(x => x.TransNo == tmpTransNo && x.BizRegID == tempBizRegID && x.BizLocID == tempBizLocID);
                                    //            if (dataInvTax == null)
                                    //            {
                                    //                await invoiceTaxLogic.Create(contInvTax);
                                    //            }
                                    //            else
                                    //            {
                                    //                contInvTax.BizRegID = dataInvTax.BizRegID;
                                    //                contInvTax.CreateBy = dataInvTax.CreateBy;
                                    //                contInvTax.CreateDate = dataInvTax.CreateDate;
                                    //                contInvTax.SyncCreate = dataInvTax.CreateDate;
                                    //                contInvTax.SyncLastUpd = DateTime.Now;
                                    //                await invoiceTaxLogic.Update(contInvTax);
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                    #endregion

                                    #region Proforma Tax
                                    contInvTax = new DTO.Accounting.INVOICETAX
                                    {
                                        BizRegID = tempBizRegID,
                                        BizLocID = tempBizLocID,
                                        TransNo = siNumber,
                                        TaxCode = 6,
                                        TaxAmt = Convert.ToDecimal(index.Gst),
                                        TaxRemark = "SST",
                                        CreateBy = "SYSTEM",
                                        CreateDate = DateTime.Now,
                                        Active = 1,
                                        Inuse = 0,
                                        IsHost = 0,
                                        SyncCreate = DateTime.Now,
                                        SyncLastUpd = null,
                                        LastSyncBy = null
                                    };

                                    var dataInvTax = invoiceTaxLogic.Retrieve(x => x.TransNo == siNumber && x.BizRegID == tempBizRegID && x.BizLocID == tempBizLocID);
                                    if (dataInvTax == null)
                                    {
                                        await invoiceTaxLogic.Create(contInvTax);
                                    }
                                    else if (dataInvTax.Inuse == 0)
                                    {
                                        contInvTax.BizRegID = dataInvTax.BizRegID;
                                        contInvTax.CreateBy = dataInvTax.CreateBy;
                                        contInvTax.CreateDate = dataInvTax.CreateDate;
                                        contInvTax.SyncCreate = dataInvTax.CreateDate;
                                        contInvTax.SyncLastUpd = DateTime.Now;
                                        await invoiceTaxLogic.Update(contInvTax);
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                    #endregion

                    #region SUNDB43
                    //siNumberInList = contResDataAPI.Select(x => x.SiNumber).ToList(); -- Remark for testing incorrect siNumber from e-Billing
                    foreach (var index in contResDataSunDB)
                    {
                        var contInvHdr = new DTO.Accounting.INVOICEHDR();
                        DTO.Accounting.INVOICEDTL contInvDtl = new DTO.Accounting.INVOICEDTL();
                        DTO.Accounting.INVOICETAX contInvTax = new DTO.Accounting.INVOICETAX();
                        string contType = string.Empty;
                        byte contStatus = 1;
                        string contDepartmentType = string.Empty;
                        AjaxResponse geteBillingDetails = null;

                        contType = "CDT";
                        contDepartmentType = index.Department;

                        string tempTransNo = index.SiNumber;
                        string tempBillID = string.Empty;

                        if (siNumberInList.Contains(tempTransNo))
                        {
                            var resBill = contResDataAPI.FindIndex(x => x.SiNumber == tempTransNo);
                            if (resBill >= 0)
                            {
                                tempBillID = contResDataAPI[resBill].Id;
                                geteBillingDetails = await GetCreditBillById(contResDataAPI[resBill].Id);
                            }
                        }

                        contInvHdr = new DTO.Accounting.INVOICEHDR
                        {
                            BizRegID = tempBizRegID,
                            BizLocID = tempBizLocID,
                            CustomerID = contCustomerCode,
                            Active = 1,
                            BillNo = index.BillNo,
                            TransNo = index.SiNumber,
                            CreateBy = "SYSTEM",
                            CreateDate = DateTime.Now,
                            Flag = 1,
                            Inuse = 0,
                            IsHost = 0,
                            ShiftCode = contType,
                            PostDate = DateTime.ParseExact(index.Date, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                            InSvcID = !string.IsNullOrEmpty(tempBillID) ? tempBillID : index.Id,
                            Status = contStatus,
                            TransChgAmt = Convert.ToDecimal(index.Balance),
                            TransAmt = Convert.ToDecimal(index.Amount),
                            TransAmtRnd = Convert.ToDecimal(index.Rounding),
                            TransAmtOrg = Convert.ToDecimal(index.Gst),
                            TransRemark = index.Detail,
                            TransReasonCode = index.CostCenter,
                            TblNo = index.SunDB,
                            SyncCreate = DateTime.Now,
                            LastUpdate = null,
                            SyncLastUpd = null,
                            LastSyncBy = null,
                            UpdateBy = string.Empty
                        };

                        //string siNumber = index.SiNumber;
                        var dataInvHdr = invoiceHDRLogic.Retrieve(x => x.TransNo == tempTransNo && x.BizRegID == tempBizRegID && x.BizLocID == tempBizLocID);
                        if (dataInvHdr == null)
                        {
                            await invoiceHDRLogic.Create(contInvHdr);
                        }
                        else if (dataInvHdr.Status != 2)
                        {
                            contInvHdr.CreateBy = dataInvHdr.CreateBy;
                            contInvHdr.CreateDate = dataInvHdr.CreateDate;
                            contInvHdr.SyncCreate = dataInvHdr.SyncCreate;
                            contInvHdr.LastUpdate = DateTime.Now;
                            contInvHdr.SyncLastUpd = DateTime.Now;
                            contInvHdr.LastSyncBy = 1;
                            contInvHdr.UpdateBy = "SYSTEM";
                            await invoiceHDRLogic.Update(contInvHdr);
                        }

                        GetCreditBillDetails dtlInput = new GetCreditBillDetails();
                        dtlInput.Type = index.Department;
                        dtlInput.InvoiceNo = index.SiNumber;

                        #region SUNDB43 Details
                        var resCreditBillsDtl = await GetCreditBillsDetail(0, dtlInput);
                        if (resCreditBillsDtl.Success)
                        {
                            var resMap = (List<MapListCreditBillsDetail>)resCreditBillsDtl.Result;
                            if (resMap != null)
                            {
                                if (resMap.Count() > 0)
                                {
                                    var transSeqNo = 1;
                                    var taxCatCode = string.Empty;
                                    foreach (var indexMap in resMap)
                                    {
                                        if (dtlInput.InvoiceNo == indexMap.INV_NO.Trim() && itemNumberInList.Contains(indexMap.ITEM_CODE.Trim()))
                                        {
                                            string tempNo = dtlInput.InvoiceNo;
                                            string tempCode = indexMap.ITEM_CODE.Trim();
                                            contInvDtl = invoiceDTLLogic.Retrieve(x => x.TransNo == tempNo && x.ItemCode == tempCode && x.BizRegID == tempBizRegID && x.BizLocID == tempBizLocID);
                                            if (contInvDtl != null)
                                            {
                                                contInvDtl.StkCode = indexMap.ANAL_M1.Trim();
                                                await invoiceDTLLogic.Update(contInvDtl);
                                                transSeqNo++;
                                            }
                                        }
                                        else
                                        {
                                            string tempItemCd = string.Empty;
                                            if (indexMap.ITEM_CODE.Trim() == "-" || indexMap.ITEM_CODE.Trim() == string.Empty)
                                            {
                                                tempItemCd = indexMap.ID_ENTERED.Trim() + indexMap.ANAL_M3.Trim() + transSeqNo.ToString();
                                            }
                                            else
                                            {
                                                tempItemCd = indexMap.ITEM_CODE.Trim();
                                            }

                                            var tmpTransNo = indexMap.INV_NO.Trim();
                                            var tmpRemark = indexMap.DESCRIPTN.Trim();
                                            int iQQ = indexMap.VALUE_1.IndexOf(".");
                                            var tmpQty = Convert.ToInt32(indexMap.VALUE_1.Substring(0, iQQ));
                                            var tmpUnitCost = Convert.ToDecimal(indexMap.VALUE_2);
                                            var tmpTotal = Convert.ToDecimal(indexMap.VALUE_10);
                                            taxCatCode = indexMap.ANAL_M9.Trim();

                                            contInvDtl = new DTO.Accounting.INVOICEDTL
                                            {
                                                BizRegID = tempBizRegID,
                                                BizLocID = tempBizLocID,
                                                TransNo = tmpTransNo,
                                                //BillNo = index.BillNo,
                                                TransSeq = transSeqNo,
                                                ItemCode = tempItemCd,
                                                Remark = tmpRemark,
                                                Qty = tmpQty,
                                                NettPrice = tmpUnitCost,
                                                UnitCost = tmpUnitCost,
                                                TolAmt = tmpTotal,
                                                //SerialNo = index.Id,
                                                CreateBy = "SYSTEM",
                                                CreateDate = DateTime.Now,
                                                Active = 1,
                                                Inuse = 0,
                                                IsHost = 0,
                                                Flag = 1,
                                                SyncCreate = DateTime.Now,
                                                LastUpdate = null,
                                                SyncLastUpd = null,
                                                LastSyncBy = null,
                                                UpdateBy = string.Empty
                                            };

                                            string tempNo = index.SiNumber;
                                            var tempItemCode = contInvDtl.ItemCode.Trim();
                                            var dataInvDtl = invoiceDTLLogic.Retrieve(x => x.TransNo == tempNo && x.ItemCode == tempItemCode && x.TransSeq == transSeqNo && x.BizRegID == tempBizRegID && x.BizLocID == tempBizLocID);
                                            if (dataInvDtl == null)
                                            {
                                                await invoiceDTLLogic.Create(contInvDtl);
                                                transSeqNo++;
                                            }
                                            else if (dataInvDtl.Status != 2)
                                            {
                                                contInvDtl.BizRegID = dataInvDtl.BizRegID;
                                                contInvDtl.CreateBy = dataInvDtl.CreateBy;
                                                contInvDtl.CreateDate = dataInvDtl.CreateDate;
                                                contInvDtl.SyncCreate = dataInvDtl.CreateDate;
                                                contInvDtl.UpdateBy = "SYSTEM";
                                                contInvDtl.SyncLastUpd = DateTime.Now;
                                                contInvDtl.LastUpdate = DateTime.Now;
                                                contInvDtl.LastSyncBy = "SYSTEM";
                                                await invoiceDTLLogic.Update(contInvDtl);
                                                transSeqNo++;
                                            }
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(tempBillID) && geteBillingDetails.Success)
                                    {
                                        var res = (dynamic)geteBillingDetails.Result;
                                        taxCatCode = res.items.CreditVoteDTO.TaxCategoryDTO.Code;
                                    }

                                    contInvHdr.SpDiscReasonCode = taxCatCode;
                                    await invoiceHDRLogic.Update(contInvHdr);
                                }
                            }
                        }
                        #endregion

                        #region SUNDB43 Tax
                        var resCreditBillsTax = await GetCreditBillsTax(0, dtlInput);

                        if (resCreditBillsTax.Success)
                        {
                            var resMap = (List<MapCreditBillTax>)resCreditBillsTax.Result;
                            if (resMap != null && resMap[0].AMOUNT != null)
                            {
                                var transSeqNo = 1;
                                foreach (var indexMap in resMap)
                                {
                                    var tmpType = dtlInput.Type;
                                    var tmpTransNo = dtlInput.InvoiceNo;
                                    contInvTax = new DTO.Accounting.INVOICETAX
                                    {
                                        BizRegID = tempBizRegID,
                                        BizLocID = tempBizLocID,
                                        TransNo = tmpTransNo,
                                        TaxCode = 6,
                                        TaxAmt = Convert.ToDecimal(indexMap.AMOUNT),
                                        TaxRemark = "SST",
                                        CreateBy = "SYSTEM",
                                        CreateDate = DateTime.Now,
                                        Active = 1,
                                        Inuse = 0,
                                        IsHost = 0,
                                        SyncCreate = DateTime.Now,
                                        SyncLastUpd = null,
                                        LastSyncBy = null
                                    };

                                    var dataInvTax = invoiceTaxLogic.Retrieve(x => x.TransNo == tmpTransNo && x.BizRegID == tempBizRegID && x.BizLocID == tempBizLocID);
                                    if (dataInvTax == null)
                                    {
                                        await invoiceTaxLogic.Create(contInvTax);
                                        transSeqNo++;
                                    }
                                    else
                                    {
                                        contInvTax.BizRegID = dataInvTax.BizRegID;
                                        contInvTax.CreateBy = dataInvTax.CreateBy;
                                        contInvTax.CreateDate = dataInvTax.CreateDate;
                                        contInvTax.SyncCreate = dataInvTax.CreateDate;
                                        contInvTax.SyncLastUpd = DateTime.Now;
                                        await invoiceTaxLogic.Update(contInvTax);
                                        transSeqNo++;
                                    }
                                }
                            }
                            else
                            {
                                var tmpType = dtlInput.Type;
                                var tmpTransNo = dtlInput.InvoiceNo;
                                contInvTax = new DTO.Accounting.INVOICETAX
                                {
                                    BizRegID = tempBizRegID,
                                    BizLocID = tempBizLocID,
                                    TransNo = tmpTransNo,
                                    TaxCode = 0,
                                    TaxRate = 0,
                                    TaxAmt = 0,
                                    TaxRemark = "SS0",
                                    CreateBy = "SYSTEM",
                                    CreateDate = DateTime.Now,
                                    Active = 1,
                                    Inuse = 0,
                                    IsHost = 0,
                                    SyncCreate = DateTime.Now,
                                    SyncLastUpd = null,
                                    LastSyncBy = null
                                };

                                var dataInvTax = invoiceTaxLogic.Retrieve(x => x.TransNo == tmpTransNo && x.BizRegID == tempBizRegID && x.BizLocID == tempBizLocID);
                                if (dataInvTax == null)
                                {
                                    await invoiceTaxLogic.Create(contInvTax);
                                }
                                else
                                {
                                    contInvTax.BizRegID = dataInvTax.BizRegID;
                                    contInvTax.CreateBy = dataInvTax.CreateBy;
                                    contInvTax.CreateDate = dataInvTax.CreateDate;
                                    contInvTax.SyncCreate = dataInvTax.CreateDate;
                                    contInvTax.SyncLastUpd = DateTime.Now;
                                    await invoiceTaxLogic.Update(contInvTax);
                                }
                            }
                        }
                        #endregion
                    }
                    #endregion

                    scope.Complete();
                }

                return new AjaxResponse(new { success = true, message = "Successed Getting All Outstanding" });
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }
        #endregion

        #region Invoice
        public async Task<AjaxResponse> GetDashboardInvoice(GetListParameter input, string customerCode)
        {
            try
            {
                User user = await _userManager.GetUserByIdAsync(Convert.ToInt64(AbpSession.UserId));
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                var isLgmAdmin = await _userManager.IsInRoleAsync(user, "LGM Admin");
                if (isAdmin == true || isLgmAdmin == true)
                {
                    customerCode = string.Empty;
                }

                var invoiceHdrLogic = new INVOICEHDR(_env, _connectionString);
                var invoiceSumLogic = new INVOICEHDR(_env, _connectionString);

                DateTime now = DateTime.Now.AddDays(-90);
                var data = invoiceHdrLogic
                    .Select<DTO.Accounting.INVOICEHDR,
                            DTO.Accounting.INVOICEDTL,
                            DTO.Accounting.INVOICETAX,
                            DTO.Core.Entity.BIZENTITY,
                            DTO.Core.General.CODEMASTER,
                            DTO.Core.General.CODEMASTER>((IH, ID, IT, BE, CMN, CMS) => new
                            {
                                IH.BillNo,
                                Company = BE.CompanyName,
                                Date = IH.PostDate,
                                Id = IH.InSvcID,
                                Detail = IH.TransRemark,
                                SiNumber = IH.TransNo,
                                Gst = IH.TransAmtOrg,
                                Tax = IT.TaxAmt,
                                Amount = IH.TransAmt,
                                Balance = IH.TransChgAmt,
                                TotalAmounts = IH.TransChgAmt,
                                Type = CMN.CodeDesc,
                                Status = CMS.CodeDesc,
                                ProjectCode = IH.TransDiscReasonCode,
                                CreditBillCount = false,
                                CreditBillAmount = false,
                                ProformaCount = false,
                                ProformaAmount = false,
                                TotalAmount = false,
                                TotalOverdue = false,
                                OverdueCount = false
                            })
                            .OuterApply
                            (
                            invoiceSumLogic
                            .Count(i => new { OverdueCount = i.TransNo })
                            //.LeftJoin<DTO.Accounting.INVOICEDTL>(i => i.TransNo, ID => ID.TransNo, (i, ID) => i.BillNo == ID.BillNo)
                            .Where<DTO.Accounting.INVOICEHDR,
                                   DTO.Accounting.INVOICEDTL>
                                   ((i, ID) => i.Flag == 1 && i.ShiftCode == "CDT" &&
                                   i.Inuse == 0 &&
                                   i.Status != 2 &&
                                   i.PostDate <= now &&
                                   (!string.IsNullOrEmpty(customerCode) ? i.BizRegID == customerCode : false))
                            .ToString()
                            )
                            .OuterApply
                            (
                            invoiceSumLogic
                            .Sum(i => new { TotalOverdue = i.TransChgAmt })
                            //.LeftJoin<DTO.Accounting.INVOICEDTL>(i => i.TransNo, ID => ID.TransNo, (i, ID) => i.BillNo == ID.BillNo)
                            .Where<DTO.Accounting.INVOICEHDR,
                                     DTO.Accounting.INVOICEDTL>
                                ((i, ID) => i.Flag == 1 && i.ShiftCode == "CDT" &&
                                i.Inuse == 0 &&
                                i.Status != 2 &&
                                i.PostDate <= now &&
                                (!string.IsNullOrEmpty(customerCode) ? i.BizRegID == customerCode : false))
                            .ToString()
                            )
                            .OuterApply
                            (
                            invoiceSumLogic
                            .Count(i => new { CreditBillCount = i.TransNo })
                            //.LeftJoin<DTO.Accounting.INVOICEDTL>(i => i.TransNo, ID => ID.TransNo, (i, ID) => i.BillNo == ID.BillNo)
                            .Where<DTO.Accounting.INVOICEHDR,
                                   DTO.Accounting.INVOICEDTL>
                                   ((i, ID) => i.Flag == 1 && i.ShiftCode == "CDT" &&
                                   i.Inuse == 0 &&
                                   i.Status != 2 &&
                                   (!string.IsNullOrEmpty(customerCode) ? i.BizRegID == customerCode : false))
                            .ToString()
                            )
                            .OuterApply
                            (
                            invoiceSumLogic
                            .Sum(i => new { CreditBillAmount = i.TransChgAmt })
                            //.LeftJoin<DTO.Accounting.INVOICEDTL>(i => i.TransNo, ID => ID.TransNo, (i, ID) => i.BillNo == ID.BillNo)
                            .Where<DTO.Accounting.INVOICEHDR,
                                   DTO.Accounting.INVOICEDTL>
                                   ((i, ID) => i.Flag == 1 && i.ShiftCode == "CDT" &&
                                   i.Inuse == 0 &&
                                   i.Status != 2 &&
                                   (!string.IsNullOrEmpty(customerCode) ? i.BizRegID == customerCode : false))
                            .ToString()
                            )
                            .OuterApply
                            (
                            invoiceSumLogic
                            .Count(i => new { ProformaCount = i.TransNo })
                            //.LeftJoin<DTO.Accounting.INVOICEDTL>(i => i.TransNo, ID => ID.TransNo, (i, ID) => i.BillNo == ID.BillNo)
                            .Where<DTO.Accounting.INVOICEHDR,
                                   DTO.Accounting.INVOICEDTL>
                                   ((i, ID) => i.Flag == 1 && i.ShiftCode == "PRF" &&
                                   i.Inuse == 0 &&
                                   i.Status != 2 &&
                                   (!string.IsNullOrEmpty(customerCode) ? i.BizRegID == customerCode : false))
                            .ToString()
                            )
                            .OuterApply
                            (
                            invoiceSumLogic
                            .Sum(i => new { ProformaAmount = i.TransChgAmt })
                            //.LeftJoin<DTO.Accounting.INVOICEDTL>(i => i.TransNo, ID => ID.TransNo, (i, ID) => i.BillNo == ID.BillNo)
                            .Where<DTO.Accounting.INVOICEHDR,
                                   DTO.Accounting.INVOICEDTL>
                                   ((i, ID) => i.Flag == 1 && i.ShiftCode == "PRF" &&
                                   i.Inuse == 0 &&
                                   i.Status != 2 &&
                                   (!string.IsNullOrEmpty(customerCode) ? i.BizRegID == customerCode : false))
                            .ToString()
                            )
                            .OuterApply
                            (
                            invoiceSumLogic
                            .Sum(i => new { TotalAmount = i.TransChgAmt })
                            //.LeftJoin<DTO.Accounting.INVOICEDTL>(i => i.TransNo, ID => ID.TransNo, (i, ID) => i.BillNo == ID.BillNo)
                            .Where<DTO.Accounting.INVOICEHDR,
                                   DTO.Accounting.INVOICEDTL>
                                   ((i, ID) => i.Flag == 1 &&
                                   i.Inuse == 0 &&
                                   i.Status != 2 &&
                                   (!string.IsNullOrEmpty(customerCode) ? i.BizRegID == customerCode : false))
                            .ToString()
                            )
                    .LeftJoin<DTO.Accounting.INVOICEHDR, DTO.Accounting.INVOICEDTL>(IH => IH.TransNo, ID => ID.TransNo, (IH, ID) => IH.BillNo == ID.BillNo)
                    .LeftJoin<DTO.Accounting.INVOICEHDR, DTO.Accounting.INVOICETAX>(IH => IH.TransNo, IT => IT.TransNo)
                    .LeftJoin<DTO.Core.Entity.BIZENTITY>(IH => IH.BizRegID, BE => BE.BizRegID)
                    .LeftJoin<DTO.Core.General.CODEMASTER>(IH => IH.Status, CMS => CMS.Code, CMS => CMS.CodeType == "IST")
                    .LeftJoin<DTO.Core.General.CODEMASTER>(IH => IH.ShiftCode, CMN => CMN.Code, CMN => CMN.CodeType == "INT")
                    .Where<DTO.Accounting.INVOICEHDR, DTO.Accounting.INVOICEDTL, DTO.Core.General.CODEMASTER, DTO.Core.General.CODEMASTER>((IH, II, CMN, CMS) =>
                        IH.Flag == 1 &&
                        IH.Inuse == 0 &&
                        IH.Status != 2 &&
                        (!string.IsNullOrEmpty(customerCode) ? IH.BizRegID == customerCode : false)
                    )
                    .GroupBy<DTO.Accounting.INVOICEHDR,
                             DTO.Accounting.INVOICEDTL,
                             DTO.Accounting.INVOICETAX,
                             DTO.Core.Entity.BIZENTITY,
                             DTO.Core.General.CODEMASTER,
                             DTO.Core.General.CODEMASTER>((IH, II, IT, BE, CMN, CMS) => new
                             {
                                 BillNo = IH.BillNo,
                                 Company = BE.CompanyName,
                                 Date = IH.PostDate,
                                 Id = IH.InSvcID,
                                 Detail = IH.TransRemark,
                                 SiNumber = IH.TransNo,
                                 Gst = IH.TransAmtOrg,
                                 Tax = IT.TaxAmt,
                                 Amount = IH.TransAmt,
                                 Balance = IH.TransChgAmt,
                                 TotalAmounts = IH.TransChgAmt,
                                 Type = CMN.CodeDesc,
                                 Status = CMS.CodeDesc,
                                 ProjectCode = IH.TransDiscReasonCode,
                                 CreditBillCount = false,
                                 CreditBillAmount = false,
                                 ProformaCount = false,
                                 ProformaAmount = false,
                                 TotalAmount = false,
                                 TotalOverdue = false,
                                 OverdueCount = false,
                                 IH.PostDate
                             })
                    .OrderByDesc<DTO.Accounting.INVOICEHDR>(IH => IH.PostDate)
                    .Fetch(input.SkipCount > 0 ? input.SkipCount : 0, input.MaxResultCount > 0 ? input.MaxResultCount : 0)
                    .ExecuteList<Map.MapListDashboardInvoice>();
                //.ToString();

                return new AjaxResponse(data);
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }

        public async Task<AjaxResponse> GetInvoice(Filter.InvoiceFilter input)
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
                string contFilter = input.Filter;

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
                                                var resSuspend = await SuspendCustomer(contSuspend);
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
                            Status = CMS.CodeDesc,
                            TotalInvoice = false,
                            TotalOverdue = false,
                            TotalOverdueAmount = false,
                            TotalAmount = false,
                            OverduePercent = string.Format("CAST(CAST(TotalOverdue AS Decimal) / CAST(TotalInvoice AS DECIMAL) * 100 AS DECIMAL)"),
                            IH.CreateDate
                        })
                        .OuterApply
                        (
                            invoiceOverdueLogic
                            .Count(i => new { TotalInvoice = i.Status })
                            .InnerJoin<DTO.Accounting.INVOICEDTL>(i => i.TransNo, ID => ID.TransNo, (i, ID) => i.BillNo == ID.BillNo)
                            .Where<DTO.Accounting.INVOICEHDR,
                                   DTO.Accounting.INVOICEDTL>
                                   ((i, ID) => i.Flag == contFlag &&
                                   (contStatus != 0 ? contStatus == 2 ? i.Inuse == 1 : i.Inuse == 0 : i.Inuse == 0) &&
                                   (!string.IsNullOrEmpty(contCustomerCode) ? i.BizRegID == contCustomerCode || ID.BizRegID == contCustomerCode : false))
                            //(!string.IsNullOrEmpty(contCompany) ? ID.CompanyName.Contains(contCompany) : false))
                            .ToString()
                        )
                        .OuterApply
                        (
                            invoiceOverdueLogic
                            .Count(i => new { TotalOverdue = i.Status })
                            .InnerJoin<DTO.Accounting.INVOICEDTL>(i => i.TransNo, ID => ID.TransNo, (i, ID) => i.BillNo == ID.BillNo)
                            .Where<DTO.Accounting.INVOICEHDR,
                                   DTO.Accounting.INVOICEDTL>
                                   ((i, ID) => i.Flag == contFlag &&
                                   (contStatus != 0 ? contStatus == 2 ? i.Inuse == 1 : i.Inuse == 0 : i.Inuse == 0) &&
                                   (i.Status == 4) &&
                                   (!string.IsNullOrEmpty(contCustomerCode) ? i.BizRegID == contCustomerCode || ID.BizRegID == contCustomerCode : false))
                            //(!string.IsNullOrEmpty(contCompany) ? II.CompanyName.Contains(contCompany) : false))
                            .ToString()
                        )
                        .OuterApply
                        (
                            invoiceOverdueLogic
                            .Sum(i => new { TotalOverdueAmount = i.TransAmt })
                            .InnerJoin<DTO.Accounting.INVOICEDTL>(i => i.TransNo, ID => ID.TransNo, (i, ID) => i.BillNo == ID.BillNo)
                            .Where<DTO.Accounting.INVOICEHDR,
                                   DTO.Accounting.INVOICEDTL>
                                   ((i, ID) => i.Flag == contFlag &&
                                   (contStatus != 0 ? contStatus == 2 ? i.Inuse == 1 : i.Inuse == 0 : i.Inuse == 0) &&
                                   (i.Status == 4) &&
                                   (!string.IsNullOrEmpty(contCustomerCode) ? i.BizRegID == contCustomerCode || ID.BizRegID == contCustomerCode : false))
                            //(!string.IsNullOrEmpty(contCompany) ? II.CompanyName.Contains(contCompany) : false))
                            .ToString()
                        )
                        .OuterApply
                        (
                            invoiceSumLogic
                            .Sum(i => new { TotalAmount = i.TransAmt })
                            .InnerJoin<DTO.Accounting.INVOICEDTL>(i => i.TransNo, ID => ID.TransNo, (i, ID) => i.BillNo == ID.BillNo)
                            .Where<DTO.Accounting.INVOICEHDR,
                                   DTO.Accounting.INVOICEDTL>
                                   ((i, ID) => i.Flag == contFlag &&
                                   (contStatus != 0 ? contStatus == 2 ? i.Inuse == 1 : i.Inuse == 0 : i.Inuse == 0) &&
                                   (contStatus != 0 ? i.Status == contStatus : i.Status != 2) &&
                                   (!string.IsNullOrEmpty(contCustomerCode) ? i.BizRegID == contCustomerCode || ID.BizRegID == contCustomerCode : false))
                            //(!string.IsNullOrEmpty(contCompany) ? II.CompanyName.Contains(contCompany) : false))
                            .ToString()
                        )
                        .InnerJoin<DTO.Accounting.INVOICEHDR, DTO.Accounting.INVOICEDTL>(IH => IH.TransNo, ID => ID.TransNo, (IH, ID) => IH.BillNo == ID.BillNo)
                        .LeftJoin<DTO.Core.Entity.BIZENTITY>(IH => IH.BizRegID, BE => BE.BizRegID)
                        .LeftJoin<DTO.Accounting.TRANSDTL>(IH => IH.TransNo, TD => TD.SerialNo)
                        .LeftJoin<DTO.Accounting.TRANSDTL, DTO.Accounting.TRANSHDR>(TD => TD.TransNo, TH => TH.TransNo)
                        .LeftJoin<DTO.Accounting.TRANSDTL, DTO.Accounting.TRANSTENDER>(TD => TD.BillNo, TT => TT.BillNo)
                        .LeftJoin<DTO.Core.General.CODEMASTER>(IH => IH.Status, CMS => CMS.Code, CMS => CMS.CodeType == "IST")
                        .LeftJoin<DTO.Accounting.INVOICEDTL, DTO.Core.General.CODEMASTER>(ID => ID.ItemType, CMN => CMN.Code, CMN => CMN.CodeType == "BNB")
                        .Where<DTO.Accounting.INVOICEHDR,
                               DTO.Accounting.INVOICEDTL,
                               DTO.Core.General.CODEMASTER,
                               DTO.Core.General.CODEMASTER>
                        ((IH, ID, CMN, CMS) =>
                            IH.Flag == contFlag &&
                            (contStatus != 0 ? contStatus == 2 ? IH.Inuse == 1 : IH.Inuse == 0 : IH.Inuse == 0) &&
                            (contStatus != 0 ? IH.Status == contStatus : IH.Status != 2) &&
                            //(!string.IsNullOrEmpty(contBillType) ? II.ItemType == contBillType : false) &&
                            (!string.IsNullOrEmpty(contInvoiceID) ? IH.TransNo == contInvoiceID || ID.TransNo == contInvoiceID : false) &&
                            (!string.IsNullOrEmpty(contCustomerCode) ? IH.BizRegID == contCustomerCode || ID.BizRegID == contCustomerCode : false) &&
                            //(!string.IsNullOrEmpty(contCompany) ? II.CompanyName.Contains(contCompany) : false) &&
                            ((input.dateStart != null && input.dateEnd != null) ? startDate <= IH.PostDate && endDate >= IH.PostDate : false)
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
                            Status = CMS.CodeDesc,
                            InvStatus = IH.Status,
                            TotalInvoice = false,
                            TotalOverdue = false,
                            TotalOverdueAmount = false,
                            TotalAmount = false,
                            IH.CreateDate
                        })
                        .OrderByDesc(IH => IH.PostDate)
                        .Fetch(input.SkipCount > 0 ? input.SkipCount : 0, input.MaxResultCount > 0 ? input.MaxResultCount : 0)
                        .ExecuteList<MapListInvoice_A>();
                    #endregion
                }
                else if (input.mode == 1)
                {
                    User user = await _userManager.GetUserByIdAsync(Convert.ToInt64(AbpSession.UserId));
                    var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                    var isLgmAdmin = await _userManager.IsInRoleAsync(user, "LGM Admin");
                    if (isAdmin == true || isLgmAdmin == true)
                    {
                        contCustomerCode = string.Empty;
                        contCompany = string.Empty;
                    }

                    #region Mode 1
                    data = invoiceHdrLogic
                        .Select<DTO.Accounting.INVOICEHDR,
                                DTO.Accounting.INVOICEDTL,
                                DTO.Accounting.INVOICETAX,
                                DTO.Core.Entity.BIZENTITY,
                                DTO.Core.General.CODEMASTER,
                                DTO.Core.General.CODEMASTER>(
                        (IH, ID, IT, BE, CMN, CMS) => new
                        {
                            BillNo = IH.BillNo,
                            Company = BE.CompanyName,
                            Date = IH.PostDate,
                            Id = IH.InSvcID,
                            Detail = IH.TransRemark,
                            SiNumber = IH.TransNo,
                            Gst = IH.TransAmtOrg,
                            Tax = IT.TaxAmt,
                            Rounding = IH.TransAmtRnd,
                            Amount = IH.TransAmt,
                            Balance = IH.TransChgAmt,
                            TotalAmounts = IH.TransChgAmt,
                            Type = CMN.CodeDesc,
                            Status = CMS.CodeDesc,
                            ProjectCode = IH.TransDiscReasonCode,
                            CreditBillCount = false,
                            CreditBillAmount = false,
                            ProformaCount = false,
                            ProformaAmount = false,
                            TotalAmount = false
                        })
                        .OuterApply
                        (
                            invoiceSumLogic
                            .Count(i => new { CreditBillCount = i.TransNo })
                            //.LeftJoin<DTO.Accounting.INVOICEDTL>(i => i.TransNo, ID => ID.TransNo, (i, ID) => i.BillNo == ID.BillNo)
                            .Where<DTO.Accounting.INVOICEHDR,
                                   DTO.Accounting.INVOICEDTL>
                                   ((i, ID) => i.Flag == contFlag && i.ShiftCode == "CDT" &&
                                   (contStatus != 0 ? contStatus == 2 ? i.Inuse == 1 : i.Inuse == 0 : i.Inuse == 0) &&
                                   (contStatus != 0 ? i.Status == contStatus : i.Status != 2) &&
                                   (!string.IsNullOrEmpty(contCustomerCode) ? i.BizRegID == contCustomerCode : false) &&
                                   (!string.IsNullOrEmpty(contInvoiceID) ? i.TransNo == contInvoiceID || i.TransNo == contInvoiceID : false) &&
                                   ((!string.IsNullOrEmpty(contCustomerCode) ? i.BizRegID == contCustomerCode || i.BizRegID == contCustomerCode : false) ||
                                   (!string.IsNullOrEmpty(contFilter) ? i.BizRegID == contFilter || i.BizRegID == contFilter : false)) &&
                                   ((input.dateStart != null && input.dateEnd != null) ? startDate <= i.PostDate && endDate >= i.PostDate : false))
                            .ToString()
                        )
                        .OuterApply
                        (
                            invoiceSumLogic
                            .Sum(i => new { CreditBillAmount = i.TransChgAmt })
                            //.LeftJoin<DTO.Accounting.INVOICEDTL>(i => i.TransNo, ID => ID.TransNo, (i, ID) => i.BillNo == ID.BillNo)
                            .Where<DTO.Accounting.INVOICEHDR,
                                   DTO.Accounting.INVOICEDTL>
                                   ((i, ID) => i.Flag == contFlag && i.ShiftCode == "CDT" &&
                                   (contStatus != 0 ? contStatus == 2 ? i.Inuse == 1 : i.Inuse == 0 : i.Inuse == 0) &&
                                   (contStatus != 0 ? i.Status == contStatus : i.Status != 2) &&
                                   (!string.IsNullOrEmpty(contCustomerCode) ? i.BizRegID == contCustomerCode : false) &&
                                   (!string.IsNullOrEmpty(contInvoiceID) ? i.TransNo == contInvoiceID || i.TransNo == contInvoiceID : false) &&
                                   ((!string.IsNullOrEmpty(contCustomerCode) ? i.BizRegID == contCustomerCode || i.BizRegID == contCustomerCode : false) ||
                                   (!string.IsNullOrEmpty(contFilter) ? i.BizRegID == contFilter || i.BizRegID == contFilter : false)) &&
                                   ((input.dateStart != null && input.dateEnd != null) ? startDate <= i.PostDate && endDate >= i.PostDate : false))
                            .ToString()
                        )
                        .OuterApply
                        (
                            invoiceSumLogic
                            .Count(i => new { ProformaCount = i.TransNo })
                            //.InnerJoin<DTO.Accounting.INVOICEDTL>(i => i.TransNo, ID => ID.TransNo, (i, ID) => i.BillNo == ID.BillNo)
                            .Where<DTO.Accounting.INVOICEHDR,
                                   DTO.Accounting.INVOICEDTL>
                                   ((i, ID) => i.Flag == contFlag && i.ShiftCode == "PRF" && // ID.TransSeq == 1 &&
                                   (contStatus != 0 ? contStatus == 2 ? i.Inuse == 1 : i.Inuse == 0 : i.Inuse == 0) &&
                                   (contStatus != 0 ? i.Status == contStatus : i.Status != 2) &&
                                   (!string.IsNullOrEmpty(contCustomerCode) ? i.BizRegID == contCustomerCode : false) &&
                                   (!string.IsNullOrEmpty(contInvoiceID) ? i.TransNo == contInvoiceID || i.TransNo == contInvoiceID : false) &&
                                   ((!string.IsNullOrEmpty(contCustomerCode) ? i.BizRegID == contCustomerCode || i.BizRegID == contCustomerCode : false) ||
                                   (!string.IsNullOrEmpty(contFilter) ? i.BizRegID == contFilter || i.BizRegID == contFilter : false)) &&
                                   ((input.dateStart != null && input.dateEnd != null) ? startDate <= i.PostDate && endDate >= i.PostDate : false))
                            .ToString()
                        )
                        .OuterApply
                        (
                            invoiceSumLogic
                            .Sum(i => new { ProformaAmount = i.TransChgAmt })
                            //.InnerJoin<DTO.Accounting.INVOICEDTL>(i => i.TransNo, ID => ID.TransNo, (i, ID) => i.BillNo == ID.BillNo)
                            .Where<DTO.Accounting.INVOICEHDR,
                                   DTO.Accounting.INVOICEDTL>
                                   ((i, ID) => i.Flag == contFlag && i.ShiftCode == "PRF" && // ID.TransSeq == 1 &&
                                   (contStatus != 0 ? contStatus == 2 ? i.Inuse == 1 : i.Inuse == 0 : i.Inuse == 0) &&
                                   (contStatus != 0 ? i.Status == contStatus : i.Status != 2) &&
                                   (!string.IsNullOrEmpty(contCustomerCode) ? i.BizRegID == contCustomerCode : false) &&
                                   (!string.IsNullOrEmpty(contInvoiceID) ? i.TransNo == contInvoiceID || i.TransNo == contInvoiceID : false) &&
                                   ((!string.IsNullOrEmpty(contCustomerCode) ? i.BizRegID == contCustomerCode || i.BizRegID == contCustomerCode : false) ||
                                   (!string.IsNullOrEmpty(contFilter) ? i.BizRegID == contFilter || i.BizRegID == contFilter : false)) &&
                                   ((input.dateStart != null && input.dateEnd != null) ? startDate <= i.PostDate && endDate >= i.PostDate : false))
                            .ToString()
                        )
                        .OuterApply
                        (
                            invoiceSumLogic
                            .Sum(i => new { TotalAmount = i.TransChgAmt })
                            //.LeftJoin<DTO.Accounting.INVOICEDTL>(i => i.TransNo, ID => ID.TransNo, (i, ID) => i.BillNo == ID.BillNo)
                            .Where<DTO.Accounting.INVOICEHDR,
                                   DTO.Accounting.INVOICEDTL>
                                   ((i, ID) => i.Flag == contFlag &&
                                   (contStatus != 0 ? contStatus == 2 ? i.Inuse == 1 : i.Inuse == 0 : i.Inuse == 0) &&
                                   (contStatus != 0 ? i.Status == contStatus : i.Status != 2) &&
                                   (!string.IsNullOrEmpty(contCustomerCode) ? i.BizRegID == contCustomerCode : false) &&
                                   (!string.IsNullOrEmpty(contInvoiceID) ? i.TransNo == contInvoiceID || i.TransNo == contInvoiceID : false) &&
                                   ((!string.IsNullOrEmpty(contCustomerCode) ? i.BizRegID == contCustomerCode || i.BizRegID == contCustomerCode : false) ||
                                   (!string.IsNullOrEmpty(contFilter) ? i.BizRegID == contFilter || i.BizRegID == contFilter : false)) &&
                                   ((input.dateStart != null && input.dateEnd != null) ? startDate <= i.PostDate && endDate >= i.PostDate : false))
                            .ToString()
                        )
                        .LeftJoin<DTO.Accounting.INVOICEHDR, DTO.Accounting.INVOICEDTL>(IH => IH.TransNo, ID => ID.TransNo, (IH, ID) => IH.BillNo == ID.BillNo)
                        .LeftJoin<DTO.Accounting.INVOICEHDR, DTO.Accounting.INVOICETAX>(IH => IH.TransNo, IT => IT.TransNo)
                        .LeftJoin<DTO.Core.Entity.BIZENTITY>(IH => IH.BizRegID, BE => BE.BizRegID)
                        .LeftJoin<DTO.Core.General.CODEMASTER>(IH => IH.Status, CMS => CMS.Code, CMS => CMS.CodeType == "IST")
                        .LeftJoin<DTO.Core.General.CODEMASTER>(IH => IH.ShiftCode, CMN => CMN.Code, CMN => CMN.CodeType == "INT")
                        .Where<DTO.Accounting.INVOICEHDR, DTO.Accounting.INVOICEDTL, DTO.Core.Entity.BIZENTITY, DTO.Core.General.CODEMASTER, DTO.Core.General.CODEMASTER>((IH, ID, BE, CMN, CMS) =>
                            IH.Flag == contFlag &&
                            (contStatus != 0 ? contStatus == 2 ? IH.Inuse == 1 : IH.Inuse == 0 : IH.Inuse == 0) &&
                            (contStatus != 0 ? IH.Status == contStatus : IH.Status != 2) &&
                            (!string.IsNullOrEmpty(contBillType) ? IH.ShiftCode == contBillType : false) &&
                            (!string.IsNullOrEmpty(contInvoiceID) ? ID.TransNo == contInvoiceID || IH.TransNo == contInvoiceID : false) &&
                            ((!string.IsNullOrEmpty(contCustomerCode) ? ID.BizRegID == contCustomerCode || IH.BizRegID == contCustomerCode : false) ||
                            (!string.IsNullOrEmpty(contFilter) ? ID.BizRegID == contFilter || IH.BizRegID == contFilter : false)) &&
                            (!string.IsNullOrEmpty(contCompany) ? BE.CompanyName.Contains(contCompany) : false) &&
                            ((input.dateStart != null && input.dateEnd != null) ? startDate <= IH.PostDate && endDate >= IH.PostDate : false)
                        )
                        .GroupBy<DTO.Accounting.INVOICEHDR,
                                 DTO.Accounting.INVOICEDTL,
                                 DTO.Accounting.INVOICETAX,
                                 DTO.Core.Entity.BIZENTITY,
                                 DTO.Core.General.CODEMASTER,
                                 DTO.Core.General.CODEMASTER>
                        ((IH, II, IT, BE, CMN, CMS) => new
                        {
                            BillNo = IH.BillNo,
                            Company = BE.CompanyName,
                            Date = IH.PostDate,
                            Id = IH.InSvcID,
                            Detail = IH.TransRemark,
                            SiNumber = IH.TransNo,
                            Gst = IH.TransAmtOrg,
                            Tax = IT.TaxAmt,
                            Rounding = IH.TransAmtRnd,
                            Amount = IH.TransAmt,
                            Balance = IH.TransChgAmt,
                            TotalAmounts = IH.TransChgAmt,
                            Type = CMN.CodeDesc,
                            Status = CMS.CodeDesc,
                            ProjectCode = IH.TransDiscReasonCode,
                            IH.CreateDate,
                            CreditBillCount = false,
                            CreditBillAmount = false,
                            ProformaCount = false,
                            ProformaAmount = false,
                            TotalAmount = false
                        })
                        .OrderByDesc<DTO.Accounting.INVOICEHDR>(IH => IH.PostDate)
                        .Fetch(input.SkipCount > 0 ? input.SkipCount : 0, input.MaxResultCount > 0 ? input.MaxResultCount : 0)
                        .ExecuteList<MapListInvoice>();
                    //.ToString();
                    #endregion
                }

                return new AjaxResponse(data);
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }

        public async Task<AjaxResponse> GetInvoiceDetails(string transNo, byte status, byte detail)
        {
            try
            {
                var invoiceHDRLogic = new INVOICEHDR(_env, _connectionString);
                var invoiceDTLLogic = new INVOICEDTL(_env, _connectionString);
                var invoiceTAXLogic = new INVOICETAX(_env, _connectionString);
                var conTransNo = transNo;

                var data = invoiceHDRLogic
                    .Select<DTO.Accounting.INVOICEHDR,
                            DTO.Accounting.INVOICEDTL,
                            DTO.Accounting.INVOICETAX,
                            DTO.Core.Entity.BIZENTITY,
                            DTO.Core.General.CODEMASTER,
                            DTO.Core.General.CODEMASTER>(
                    (IH, ID, IT, BE, CMN, CMS) => new
                    {
                        IH.BillNo,
                        SiNumber = IH.TransNo,
                        Id = IH.InSvcID,
                        Date = IH.PostDate,
                        BE.BizRegID,
                        IH.BizLocID,
                        BE.AcctNo,
                        Company = BE.CompanyName,
                        BE.ContactPerson,
                        BE.Address1,
                        BE.Address2,
                        BE.Address3,
                        BE.Address4,
                        BE.Email,
                        Detail = IH.TransRemark,
                        Attention = IH.CashierID,
                        CreditVote = IH.TransDiscRemark,
                        ProjectCode = IH.TransDiscReasonCode,
                        RefNo = IH.AcctNo,
                        CostCenter = IH.TransReasonCode,
                        OurRef = IH.ServerID,
                        YourRef = IH.CustPkgID,
                        TaxCatCode = IH.SpDiscReasonCode,
                        IT.TaxRate,
                        IT.TaxAmt,
                        Amount = IH.TransAmt,
                        Gst = IH.TransAmtOrg,
                        Rounding = IH.TransAmtRnd,
                        Balance = IH.TransChgAmt,
                        //TotalAmount = IH.TransSubTotal,
                        //TotalAmount = string.Format("{0}+{1}", IH.TransAmt, IT.TaxAmt),
                        SunDB = IH.TblNo,
                        Type = CMN.CodeDesc,
                        Status = CMS.CodeDesc
                    })
                    .InnerJoin<DTO.Accounting.INVOICEDTL>(IH => IH.TransNo, ID => ID.TransNo)
                    .LeftJoin<DTO.Accounting.INVOICETAX>(IH => IH.TransNo, IT => IT.TransNo)
                    .LeftJoin<DTO.Core.Entity.BIZENTITY>(IH => IH.BizRegID, BE => BE.BizRegID)
                    .LeftJoin<DTO.Core.General.CODEMASTER>(IH => IH.Status, CMS => CMS.Code, CMS => CMS.CodeType == "IST")
                    .LeftJoin<DTO.Core.General.CODEMASTER>(IH => IH.ShiftCode, CMN => CMN.Code, CMN => CMN.CodeType == "INT")
                    .Where<DTO.Accounting.INVOICEHDR, DTO.Accounting.INVOICEDTL, DTO.Core.Entity.BIZENTITY, DTO.Core.General.CODEMASTER, DTO.Core.General.CODEMASTER>((IH, ID, BE, CMN, CMS) =>
                        //IH.Flag == 1 &&
                        //IH.Inuse == 0 &&
                        //IH.Status != 2 &&
                        //IH.TransNo == conTransNo
                        IH.Flag == ((status == 0) ? 1 : 1) &&
                        IH.Inuse == ((status == 0) ? 0 : 1) &&
                        IH.Status != ((status == 0) ? 2 : 1) &&
                        ((detail == 0) ? ((status == 0) ? IH.TransNo == conTransNo : IH.BillNo == conTransNo) : IH.TransNo == conTransNo)
                    )
                    .GroupBy<DTO.Accounting.INVOICEHDR,
                            DTO.Accounting.INVOICEDTL,
                            DTO.Accounting.INVOICETAX,
                            DTO.Core.Entity.BIZENTITY,
                            DTO.Core.General.CODEMASTER,
                            DTO.Core.General.CODEMASTER>(
                    (IH, ID, IT, BE, CMN, CMS) => new
                    {
                        IH.BillNo,
                        SiNumber = IH.TransNo,
                        Id = IH.InSvcID,
                        Date = IH.PostDate,
                        BE.BizRegID,
                        IH.BizLocID,
                        BE.AcctNo,
                        Company = BE.CompanyName,
                        BE.ContactPerson,
                        BE.Address1,
                        BE.Address2,
                        BE.Address3,
                        BE.Address4,
                        BE.Email,
                        Detail = IH.TransRemark,
                        Attention = IH.CashierID,
                        CreditVote = IH.TransDiscRemark,
                        ProjectCode = IH.TransDiscReasonCode,
                        RefNo = IH.AcctNo,
                        CostCenter = IH.TransReasonCode,
                        OurRef = IH.ServerID,
                        YourRef = IH.CustPkgID,
                        TaxCatCode = IH.SpDiscReasonCode,
                        IT.TaxRate,
                        IT.TaxAmt,
                        Amount = IH.TransAmt,
                        Gst = IH.TransAmtOrg,
                        Rounding = IH.TransAmtRnd,
                        Balance = IH.TransChgAmt,
                        SunDB = IH.TblNo,
                        //TotalAmount = IH.TransSubTotal,
                        Type = CMN.CodeDesc,
                        Status = CMS.CodeDesc
                    })
                    .Execute().FirstOrDefault();

                var items = invoiceDTLLogic
                    .Select<DTO.Accounting.INVOICEDTL,
                            DTO.Accounting.INVOICETAX>((ID, IT) => new
                            {
                                ID.TransSeq,
                                ItemCode = ID.ItemCode,
                                Description = ID.Remark,
                                TaxCode = IT.TaxRemark,
                                Quantity = ID.Qty,
                                UnitPrice = ID.NettPrice,
                                SubTotal = string.Format("{0}*{1}", ID.Qty, ID.NettPrice)
                            })
                    .LeftJoin<DTO.Accounting.INVOICETAX>(ID => ID.TransNo, IT => IT.TransNo)
                    .Where<DTO.Accounting.INVOICEDTL>((ID) =>
                        //ID.Flag == 1 &&
                        //ID.Inuse == 0 &&
                        //ID.Status != 2 &&
                        //ID.TransNo == conTransNo
                        ID.Flag == ((status == 0) ? 1 : 1) &&
                        ID.Inuse == ((status == 0) ? 0 : 1) &&
                        ID.Status != ((status == 0) ? 2 : 1) &&
                        ((detail == 0) ? ((status == 0) ? ID.TransNo == conTransNo : ID.BillNo == conTransNo) : ID.TransNo == conTransNo)
                    )
                    .GroupBy<DTO.Accounting.INVOICEDTL,
                             DTO.Accounting.INVOICETAX>((ID, IT) => new
                             {
                                 ID.TransSeq,
                                 ItemCode = ID.ItemCode,
                                 Description = ID.Remark,
                                 TaxCode = IT.TaxRemark,
                                 Quantity = ID.Qty,
                                 UnitPrice = ID.NettPrice
                             })
                    .ExecuteList<MapListInvoiceDetail>();

                return new AjaxResponse(new { data, items });
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }
        #endregion

        #region Transaction
        public async Task<AjaxResponse> UpdateTransaction(string TransNo)
        {
            try
            {
                TRANSHDR transHdrLogic = new TRANSHDR(_env, _connectionString);
                TRANSDTL transDtlLogic = new TRANSDTL(_env, _connectionString);
                TRANSTENDER transTenderLogic = new TRANSTENDER(_env, _connectionString);
                INVOICEDTL invoiceDtlLogic;
                INVOICEHDR invoiceHdrLogic;
                DTO.Accounting.TRANSHDR transHdrRes = new DTO.Accounting.TRANSHDR();
                List<DTO.Accounting.TRANSDTL> transDtlListRes = new List<DTO.Accounting.TRANSDTL>();
                DTO.Accounting.TRANSTENDER transTenderRes = new DTO.Accounting.TRANSTENDER();
                List<DTO.Accounting.INVOICEDTL> invoiceDtlRes = new List<DTO.Accounting.INVOICEDTL>();
                DTO.Accounting.INVOICEHDR invoiceHdrRes;
                AjaxResponse resValidate = null;
                AjaxResponse resAjax;
                MapValidateStatus resMap;
                NameValueCollection qscoll;
                string FPXID = string.Empty;
                string refInfo = string.Empty;
                string bankCode = string.Empty;
                string billNo = string.Empty;
                string invoiceID = string.Empty;
                byte Inuse = 0;
                byte TrxStatus = 0;
                string tempUserID = AbpSession.UserId.ToString() != "" ? AbpSession.UserId.ToString() : "SYSTEM";

                transHdrRes = transHdrLogic.Retrieve(x => x.TransNo == TransNo);
                if (transHdrRes == null)
                    //return new AjaxResponse(new { success = false });
                    return new AjaxResponse(new ErrorInfo("Incorrect Trans No"));

                transDtlListRes = transDtlLogic.RetrieveAll(x => x.TransNo == TransNo);
                if (transDtlListRes == null)
                    //return new AjaxResponse(new { success = false });
                    return new AjaxResponse(new ErrorInfo("Incorrect Trans No"));

                if (transDtlListRes.Count() == 0)
                    //return new AjaxResponse(new { success = false });
                    return new AjaxResponse(new ErrorInfo("Trans Detail Cannot Found"));

                transTenderRes = transTenderLogic.Retrieve(x => x.TransNo == TransNo);
                if (transTenderRes == null)
                    //return new AjaxResponse(new { success = false });
                    return new AjaxResponse(new ErrorInfo("Incorrect Trans No"));

                if (transTenderRes.TenderID == "996" || transTenderRes.TenderID == "997")
                {
                    refInfo = transTenderRes.RefToken;
                    if (string.IsNullOrEmpty(refInfo))
                        //return new AjaxResponse(new { success = false });
                        return new AjaxResponse(new ErrorInfo("Incorrect Reference"));

                    if (transTenderRes.TenderID == "997")
                    {
                        resValidate = await ValidateFPXStatus(refInfo);
                    }
                    else if (transTenderRes.TenderID == "996")
                    {
                        resValidate = await ValidateMBBStatus(refInfo);
                    }

                    if (resValidate.Success)
                    {
                        resMap = (MapValidateStatus)resValidate.Result;
                        if (resMap == null)
                            //return new AjaxResponse(new { success = false });
                            return new AjaxResponse(new ErrorInfo("Error Validating Payment Status"));

                        if (string.IsNullOrEmpty(resMap.CheckSum))
                            //return new AjaxResponse(new { success = false });
                            return new AjaxResponse(new ErrorInfo("Error Validating Payment Status"));

                        if (resMap.CheckSum.Contains("ERROR"))
                            //return new AjaxResponse(new { success = false });
                            return new AjaxResponse(new ErrorInfo("Error Validating Payment Status"));

                        qscoll = HttpUtility.ParseQueryString(resMap.CheckSum);
                        if (qscoll != null)
                        {
                            if (transTenderRes.TenderID == "997")
                            {
                                FPXID = qscoll["fpx_fpxTxnId"];
                                bankCode = qscoll["fpx_buyerBankId"];
                            }
                            else if (transTenderRes.TenderID == "996")
                            {
                                FPXID = qscoll["TRANSACTION_ID"];
                                bankCode = qscoll["fpx_buyerBankId"];
                            }
                        }

                        transHdrRes.Status = 0; // default status if payment not success
                        TrxStatus = 1;

                        //string[] cancelStr = { "1A", "1B", "1C", "1D", "1E", "1F", "1G", "1H", "1I", "1K", "1L", "51", "80", "89", "93", "96", "BC", "OE" };
                        string[] cancelStr = { };

                        var cancelList = await _generalManager.GetCodeMasterbyCodeType("FPX");
                        if (cancelList.Success)
                        {
                            var res = (dynamic)cancelList.Result;
                            List<string> list = new List<string>();
                            foreach (var idx in res.Items)
                            {
                                list.Add(idx["Code"]);
                            }
                            list.Remove("00");
                            list.Remove("09");
                            cancelStr = list.ToArray();
                        }

                        if (resMap.DebitAuthCode == "00" && (transTenderRes.TenderID == "997" || transTenderRes.TenderID == "996"))
                        {
                            transHdrRes.Status = 2;
                            TrxStatus = 2;
                            Inuse = 1;

                            // Set Running Receipt No (LGM)
                            if (transHdrRes.CustPrivilege == null || transHdrRes.CustPrivilege == string.Empty)
                            //if (transHdrRes.TransRemark == null || transHdrRes.TransRemark == string.Empty)
                            {
                                var resReceiptNo = await GenerateDocCode("06", "RC");
                                if (resReceiptNo.Success)
                                {
                                    var res = (dynamic)resReceiptNo.Result;
                                    if (res != null)
                                    {
                                        transHdrRes.CustPrivilege = res;
                                    }
                                }
                            }

                            // Set Running Digital Signature No (LGM)
                            if (transHdrRes.SpDiscRemark == null || transHdrRes.SpDiscRemark == string.Empty)
                            {
                                var resDigSignature = await GenerateDocCode("06", "PT");
                                if (resDigSignature.Success)
                                {
                                    var res = (dynamic)resDigSignature.Result;
                                    if (res != null)
                                    {
                                        transHdrRes.SpDiscRemark = res;
                                    }
                                }
                            }
                        }
                        else if ((resMap.DebitAuthCode == "09" || resMap.DebitAuthCode == "99") && transTenderRes.TenderID == "997")
                        {
                            transHdrRes.Status = 1;
                            TrxStatus = 1;
                            Inuse = 1; //Inuse = 0; -- Amend for Pending Authorization
                        }
                        #region Amended Status for Rejected
                        else if (cancelStr.Contains(resMap.DebitAuthCode) && transTenderRes.TenderID == "997")
                        {
                            transHdrRes.Status = 4;
                            TrxStatus = 1;
                            Inuse = 0;
                        }
                        else if (resMap.DebitAuthCode == "99" && transTenderRes.TenderID == "996")
                        {
                            transHdrRes.Status = 4;
                            TrxStatus = 1;
                            Inuse = 0;
                        }
                        //else
                        //{
                        //    transHdrRes.Status = 3;
                        //    TrxStatus = 3;
                        //    Inuse = 0;
                        //}
                        #endregion
                        //else
                        //{
                        //    transHdrRes.Status = 1;
                        //    TrxStatus = 1;
                        //    Inuse = 0;
                        //}

                        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        {
                            transTenderRes.IsApproved = 1;
                            transTenderRes.TransStatus = 1;
                            transTenderRes.ApprovedDate = DateTime.Now;
                            transTenderRes.RespCode = resMap.DebitAuthCode;
                            transTenderRes.ExternalID = FPXID;
                            transTenderRes.AuthCode = resMap.CheckSum;
                            transTenderRes.LastUpdate = DateTime.Now;
                            transTenderRes.SyncLastUpd = DateTime.Now;
                            await transTenderLogic.Update(transTenderRes);

                            invoiceDtlLogic = new INVOICEDTL(_env, _connectionString);
                            invoiceHdrLogic = new INVOICEHDR(_env, _connectionString);
                            var tempBizRegID = transHdrRes.BizRegID;
                            using (TransactionScope scope_detail = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
                            {
                                var trx = 1;
                                foreach (var index in transDtlListRes)
                                {
                                    invoiceID = index.BillNo;
                                    invoiceHdrRes = invoiceHdrLogic.Retrieve(x => x.BillNo == invoiceID && x.BizRegID == tempBizRegID);
                                    if (invoiceHdrRes != null)
                                    {
                                        invoiceHdrRes.Status = TrxStatus;
                                        invoiceHdrRes.Inuse = Inuse;
                                        invoiceHdrRes.LastUpdate = DateTime.Now;
                                        invoiceHdrRes.SyncLastUpd = DateTime.Now;
                                        invoiceHdrRes.UpdateBy = tempUserID;
                                        await invoiceHdrLogic.Update(invoiceHdrRes);

                                        var trxID = invoiceHdrRes.TransNo;
                                        invoiceDtlRes = invoiceDtlLogic.RetrieveAll(x => x.TransNo == trxID && x.BizRegID == tempBizRegID);
                                        if (invoiceDtlRes != null)
                                        {
                                            foreach (var invDtl in invoiceDtlRes)
                                            {
                                                invDtl.Status = TrxStatus;
                                                invDtl.Inuse = Inuse;
                                                invDtl.LastUpdate = DateTime.Now;
                                                invDtl.SyncLastUpd = DateTime.Now;
                                                invDtl.UpdateBy = tempUserID;
                                                await invoiceDtlLogic.Update(invDtl);
                                            }
                                        }

                                        index.Status = transHdrRes.Status; //TrxStatus;
                                        index.LastUpdate = transHdrRes.LastUpdate;
                                        index.SyncLastUpd = transHdrRes.SyncLastUpd;
                                        index.UpdateBy = tempUserID;

                                        // e-Billing Update Payment
                                        if (_appConfiguration["SUNSystem:isUAT"] == "true")
                                        {
                                            bool singleInv = false;
                                            if (index.ItemType == 0 && trx > 1)
                                                singleInv = true;
                                            if (!singleInv && (transHdrRes.Status == 2 && TrxStatus == 2) && (transHdrRes.Posted == 0 || index.Posted == 0))
                                            {
                                                Map.Envelope responseEnvelope = new Map.Envelope();

                                                var updCustNo = string.Empty;
                                                var updEmail = _appConfiguration["SUNSystem:ServiceEmail"];
                                                var updBillID = index.ItemType == 1 ? index.SerialNo : invoiceHdrRes.InSvcID;
                                                var updAmount = invoiceHdrRes.TransChgAmt.ToString();

                                                var companyDtl = await _entityManager.GetCompanyDetail(invoiceHdrRes.BizRegID, invoiceHdrRes.BizLocID);
                                                if (companyDtl.Success)
                                                {
                                                    var updMap = (dynamic)companyDtl.Result;
                                                    updCustNo = updMap.AcctNo; // Check again
                                                }

                                                var updatePaymentData = new POSTUpdatePayment
                                                {
                                                    Email = updEmail,
                                                    BillID = updBillID,
                                                    Amount = updAmount,
                                                    Reference = string.Empty
                                                };

                                                var updProcess = await UpdatePayment(updatePaymentData);
                                                if (updProcess.Success)
                                                {
                                                    // Add Code for e-Billing Update Payment
                                                    index.Posted = 1;
                                                    transHdrRes.Posted = 1;
                                                }
                                            }
                                        }

                                        await transDtlLogic.Update(index);
                                    }
                                }

                                scope_detail.Complete();
                            }

                            transHdrRes.TransStatus = resMap.DebitAuthCode;
                            //transHdrRes.CustPrivilege = bankCode;
                            transHdrRes.ShiftCode = resMap.DebitAuthCode;
                            transHdrRes.LastUpdate = DateTime.Now;
                            transHdrRes.SyncLastUpd = DateTime.Now;
                            transHdrRes.UpdateBy = tempUserID;
                            await transHdrLogic.Update(transHdrRes);

                            scope.Complete();
                        }

                        return new AjaxResponse(new { success = true });
                    }
                    else
                    {
                        return new AjaxResponse(new ErrorInfo("Error Validating Payment Status"));
                    }
                }
                else if (!string.IsNullOrEmpty(transTenderRes.TenderID) && transTenderRes.RespCode == "00")
                {
                    transHdrRes.Status = 2;
                    TrxStatus = 2;
                    Inuse = 1;

                    // Set Running Receipt No (LGM)
                    if (transHdrRes.CustPrivilege == null || transHdrRes.CustPrivilege == string.Empty)
                    {
                        try
                        {
                            var resReceiptNo = await GenerateDocCode("06", "RC");
                            if (resReceiptNo.Success)
                            {
                                var res = (dynamic)resReceiptNo.Result;
                                if (res != null)
                                {
                                    transHdrRes.CustPrivilege = res;
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    // Set Running Digital Signature No (LGM)
                    if (transHdrRes.SpDiscRemark == null || transHdrRes.SpDiscRemark == string.Empty)
                    {
                        try
                        {
                            var resDigSignature = await GenerateDocCode("06", "PT");
                            if (resDigSignature.Success)
                            {
                                var res = (dynamic)resDigSignature.Result;
                                if (res != null)
                                {
                                    transHdrRes.SpDiscRemark = res;
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        transTenderRes.IsApproved = 1;
                        transTenderRes.TransStatus = 1;
                        transTenderRes.ApprovedDate = DateTime.Now;
                        transTenderRes.RespCode = "00"; // resMap.DebitAuthCode;
                        transTenderRes.ExternalID = FPXID;
                        //transTenderRes.AuthCode = resMap.CheckSum;
                        transTenderRes.LastUpdate = DateTime.Now;
                        transTenderRes.SyncLastUpd = DateTime.Now;
                        await transTenderLogic.Update(transTenderRes);

                        invoiceDtlLogic = new INVOICEDTL(_env, _connectionString);
                        invoiceHdrLogic = new INVOICEHDR(_env, _connectionString);
                        var tempBizRegID = transHdrRes.BizRegID;
                        using (TransactionScope scope_detail = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
                        {
                            var trx = 1;
                            foreach (var index in transDtlListRes)
                            {
                                // InvoiceHDR Update
                                invoiceID = index.BillNo;
                                invoiceHdrRes = invoiceHdrLogic.Retrieve(x => x.BillNo == invoiceID && x.BizRegID == tempBizRegID);
                                if (invoiceHdrRes != null)
                                {
                                    invoiceHdrRes.Status = TrxStatus;
                                    invoiceHdrRes.Inuse = Inuse;
                                    invoiceHdrRes.LastUpdate = DateTime.Now;
                                    invoiceHdrRes.SyncLastUpd = DateTime.Now;
                                    invoiceHdrRes.UpdateBy = tempUserID;
                                    await invoiceHdrLogic.Update(invoiceHdrRes);

                                    // InvoiceDTL Update
                                    var trxID = invoiceHdrRes.TransNo;
                                    invoiceDtlRes = invoiceDtlLogic.RetrieveAll(x => x.TransNo == trxID && x.BizRegID == tempBizRegID);
                                    if (invoiceDtlRes != null)
                                    {
                                        foreach (var invDtl in invoiceDtlRes)
                                        {
                                            invDtl.Status = TrxStatus;
                                            invDtl.Inuse = Inuse;
                                            invDtl.LastUpdate = DateTime.Now;
                                            invDtl.SyncLastUpd = DateTime.Now;
                                            invDtl.UpdateBy = tempUserID;
                                            await invoiceDtlLogic.Update(invDtl);
                                        }
                                    }

                                    // TransDTL Update
                                    index.Status = transHdrRes.Status;
                                    index.LastUpdate = transHdrRes.LastUpdate;
                                    index.SyncLastUpd = transHdrRes.SyncLastUpd;
                                    index.UpdateBy = tempUserID;

                                    // e-Billing Update Payment
                                    if (_appConfiguration["SUNSystem:isUAT"] == "true")
                                    {
                                        bool singleInv = false;
                                        if (index.ItemType == 0 && trx > 1)
                                            singleInv = true;
                                        if (!singleInv && (transHdrRes.Status == 2 && TrxStatus == 2) && (transHdrRes.Posted == 0 || index.Posted == 0))
                                        {
                                            var updCustNo = string.Empty;
                                            var updEmail = _appConfiguration["SUNSystem:ServiceEmail"];
                                            var updBillID = index.ItemType == 1 ? index.SerialNo : invoiceHdrRes.InSvcID;
                                            var updAmount = invoiceHdrRes.TransChgAmt.ToString();

                                            var companyDtl = await _entityManager.GetCompanyDetail(invoiceHdrRes.BizRegID, invoiceHdrRes.BizLocID);
                                            if (companyDtl.Success)
                                            {
                                                var updMap = (dynamic)companyDtl.Result;
                                                updCustNo = updMap.AcctNo; // Check again
                                            }

                                            var updatePaymentData = new POSTUpdatePayment
                                            {
                                                Email = updEmail,
                                                BillID = updBillID,
                                                Amount = updAmount,
                                                Reference = string.Empty
                                            };

                                            var updProcess = await UpdatePayment(updatePaymentData);
                                            if (updProcess.Success)
                                            {
                                                // Add Code for e-Billing Update Payment
                                                index.Posted = 1;
                                                transHdrRes.Posted = 1;
                                            }
                                        }
                                    }

                                    await transDtlLogic.Update(index);
                                    trx++;
                                }
                            }

                            scope_detail.Complete();
                        }

                        transHdrRes.TransStatus = "00";
                        //transHdrRes.CustPrivilege = bankCode;
                        transHdrRes.ShiftCode = "00";
                        transHdrRes.LastUpdate = DateTime.Now;
                        transHdrRes.SyncLastUpd = DateTime.Now;
                        transHdrRes.UpdateBy = tempUserID;
                        await transHdrLogic.Update(transHdrRes);

                        scope.Complete();
                    }

                    return new AjaxResponse(new { success = true });
                }
                return new AjaxResponse(new ErrorInfo("Incorrect Tender ID"));
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }

        public async Task<dynamic> UpdateTransactionQ(string TransNo)
        {
            try
            {
                TRANSHDR transHdrLogic = new TRANSHDR(_env, _connectionString);
                TRANSDTL transDtlLogic = new TRANSDTL(_env, _connectionString);
                TRANSTENDER transTenderLogic = new TRANSTENDER(_env, _connectionString);
                INVOICEITEM invoiceDtlLogic;
                //INVOICEHDR_OLD invoiceHdrLogic;
                DTO.Accounting.TRANSHDR transHdrRes = new DTO.Accounting.TRANSHDR();
                List<DTO.Accounting.TRANSDTL> transDtlListRes = new List<DTO.Accounting.TRANSDTL>();
                DTO.Accounting.TRANSTENDER transTenderRes = new DTO.Accounting.TRANSTENDER();
                DTO.Accounting.INVOICEITEM invoiceDtlRes = new DTO.Accounting.INVOICEITEM();
                DTO.Accounting.INVOICEITEM transCdItemRes = new DTO.Accounting.INVOICEITEM();
                //DTO.Accounting.INVOICEHDR_OLD invoiceHdrRes;
                AjaxResponse resValidate = null;
                AjaxResponse resAjax;
                MapValidateStatus resMap;
                NameValueCollection qscoll;
                string FPXID = string.Empty;
                string refInfo = string.Empty;
                string bankCode = string.Empty;
                string billNo = string.Empty;
                string invoiceID = string.Empty;
                byte Inuse = 0;
                byte TrxStatus = 0;
                byte TrxHDRStatus = 0;

                transHdrRes = transHdrLogic.Retrieve(x => x.TransNo == TransNo);
                if (transHdrRes == null)
                    //return new AjaxResponse(new { success = false });
                    return new AjaxResponse(new ErrorInfo("Incorrect Trans No"));

                transDtlListRes = transDtlLogic.RetrieveAll(x => x.TransNo == TransNo);
                if (transDtlListRes == null)
                    //return new AjaxResponse(new { success = false });
                    return new AjaxResponse(new ErrorInfo("Incorrect Trans No"));

                if (transDtlListRes.Count() == 0)
                    //return new AjaxResponse(new { success = false });
                    return new AjaxResponse(new ErrorInfo("Trans Detail Cannot Found"));

                transTenderRes = transTenderLogic.Retrieve(x => x.TransNo == TransNo);
                if (transTenderRes == null)
                    //return new AjaxResponse(new { success = false });
                    return new AjaxResponse(new ErrorInfo("Incorrect Trans No"));


                if (transTenderRes.TenderID == "996" || transTenderRes.TenderID == "997")
                {
                    refInfo = transTenderRes.RefToken;
                    if (string.IsNullOrEmpty(refInfo))
                        //return (new AjaxResponse(new { success = false }));
                        return new AjaxResponse(new ErrorInfo("Incorrect Reference"));


                    if (transTenderRes.TenderID == "997")
                    {
                        resValidate = await ValidateFPXStatus(refInfo);
                    }
                    else if (transTenderRes.TenderID == "996")
                    {
                        resValidate = await this.ValidateMBBStatus(refInfo);
                    }

                    if (resValidate.Success)
                    {
                        resMap = (MapValidateStatus)resValidate.Result;
                        if (resMap == null)
                            //return new AjaxResponse(new { success = false });
                            return new AjaxResponse(new ErrorInfo("Error Validating Payment Status"));

                        if (string.IsNullOrEmpty(resMap.CheckSum))
                            //return new AjaxResponse(new { success = false });
                            return new AjaxResponse(new ErrorInfo("Error Validating Payment Status"));

                        if (resMap.CheckSum.Contains("ERROR"))
                            //return new AjaxResponse(new { success = false });
                            return new AjaxResponse(new ErrorInfo("Error Validating Payment Status"));


                        qscoll = HttpUtility.ParseQueryString(resMap.CheckSum);
                        if (qscoll != null)
                        {
                            if (transTenderRes.TenderID == "997")
                            {
                                FPXID = qscoll["fpx_fpxTxnId"];
                                bankCode = qscoll["fpx_buyerBankId"];
                            }
                            else if (transTenderRes.TenderID == "996")
                            {
                                FPXID = qscoll["TRANSACTION_ID"];
                                bankCode = qscoll["fpx_buyerBankId"];
                            }
                        }

                        transHdrRes.Status = 0; // default status if payment not success
                        TrxStatus = 1;
                        //string[] cancelStr = { "1C", "1F", "1I", "1L", "80", "BC" };
                        //string[] cancelStr = { "1A", "1B", "1C", "1D", "1E", "1F", "1G", "1H", "1I", "1K", "1L", "80", "89", "93", "96", "BC", "OE" };
                        string[] cancelStr = { };

                        var cancelList = await _generalManager.GetCodeMasterbyCodeType("FPX");
                        if (cancelList.Success)
                        {
                            var res = (dynamic)cancelList.Result;
                            List<string> list = new List<string>();
                            foreach (var idx in res.Items)
                            {
                                list.Add(idx.Code);
                            }
                            list.Remove("00");
                            list.Remove("09");
                            cancelStr = list.ToArray();
                        }
                        //00 is response code from fpx means approved
                        //997 is code for fpx
                        //996 is code for maybank
                        if (resMap.DebitAuthCode == "00" && (transTenderRes.TenderID == "997" || transTenderRes.TenderID == "996"))
                        {
                            transHdrRes.Status = 2;
                            TrxHDRStatus = 2;
                            TrxStatus = 2;
                            Inuse = 1;

                            // Set Running Receipt No (LGM)
                            if (transHdrRes.TransRemark == null || transHdrRes.TransRemark == string.Empty)
                            {
                                try
                                {
                                    var resReceiptNo = await GenerateDocCode("06", "RC");
                                    if (resReceiptNo.Success)
                                    {
                                        var res = (dynamic)resReceiptNo.Result;
                                        if (res != null)
                                        {
                                            transHdrRes.TransRemark = res;

                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(nameof(AccountingManager), ex);
                                }
                            }

                            // Set Running Digital Signature No (LGM)
                            if (transHdrRes.SpDiscRemark == null || transHdrRes.SpDiscRemark == string.Empty)
                            {
                                try
                                {
                                    var resDigSignature = await GenerateDocCode("06", "PT");
                                    if (resDigSignature.Success)
                                    {
                                        var res = (dynamic)resDigSignature.Result;
                                        if (res != null)
                                        {
                                            transHdrRes.SpDiscRemark = res;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(nameof(AccountingManager), ex);
                                }
                            }
                        }
                        else if ((resMap.DebitAuthCode == "09" || resMap.DebitAuthCode == "99") && transTenderRes.TenderID == "997")
                        {
                            transHdrRes.Status = 1;
                            TrxHDRStatus = 1;
                            TrxStatus = 1;
                            Inuse = 1; //Inuse = 0; -- Amend for Pending Authorization
                        }
                        else if (cancelStr.Contains(resMap.DebitAuthCode) && transTenderRes.TenderID == "997")//997 is fpx
                        {
                            transHdrRes.Status = 4;
                            TrxHDRStatus = 4;
                            TrxStatus = 1;
                            Inuse = 0;
                        }
                        else if (resMap.DebitAuthCode == "99" && transTenderRes.TenderID == "996")// may bank
                        {
                            transHdrRes.Status = 4;
                            TrxHDRStatus = 4;
                            TrxStatus = 1;
                            Inuse = 0;
                        }
                        //else
                        //{
                        //    transHdrRes.Status = 3;
                        //    TrxStatus = 3;
                        //    Inuse = 0;
                        //}

                        transTenderRes.IsApproved = 1;
                        transTenderRes.TransStatus = 1;
                        transTenderRes.ApprovedDate = DateTime.Now;
                        transTenderRes.RespCode = resMap.DebitAuthCode;
                        transTenderRes.ExternalID = FPXID;
                        transTenderRes.AuthCode = resMap.CheckSum;
                        transTenderRes.LastUpdate = DateTime.Now;
                        transTenderRes.SyncLastUpd = DateTime.Now;


                        invoiceDtlLogic = new INVOICEITEM(_env, _connectionString);
                        //invoiceHdrLogic = new INVOICEHDR_OLD(_env, _connectionString);

                        await transTenderLogic.Update(transTenderRes);
                        var tempBizRegID = transHdrRes.BizRegID;
                        foreach (var index in transDtlListRes)
                        {
                            index.Status = TrxHDRStatus;//TrxStatus;
                            index.LastUpdate = transHdrRes.LastUpdate;
                            index.SyncLastUpd = transHdrRes.SyncLastUpd;
                            index.UpdateBy = transHdrRes.UpdateBy;
                            await transDtlLogic.Update(index);

                            invoiceID = index.ItemCode;
                            invoiceDtlRes = invoiceDtlLogic.Retrieve(x => x.BatchCode == invoiceID && x.BizRegID == tempBizRegID); //iki yo mas yap tulung sampean cek
                            if (invoiceDtlRes != null)
                            {
                                invoiceID = invoiceDtlRes.BatchCode;
                                var tempMonthCode = invoiceDtlRes.MonthCode;
                                var invoiceHdrRes = _invoiceHDRRepo.Retrieve(x => x.BatchCode == invoiceID && x.MonthCode == tempMonthCode && x.Status != 2); //edit by YAP, 2020-03-23 => Prevent PAID invoice change back to OPEN 
                                if (invoiceHdrRes != null)
                                {
                                    invoiceDtlRes.Status = TrxStatus; // resMap.DebitAuthCode == "00" ? 2 : invoiceDtlRes.Status;
                                    invoiceDtlRes.Inuse = Inuse; // 1;
                                    invoiceDtlRes.LastUpdate = DateTime.Now;
                                    invoiceDtlRes.SyncLastUpd = DateTime.Now;
                                    invoiceDtlRes.UpdateBy = "SYSTEM";
                                    await invoiceDtlLogic.Update(invoiceDtlRes);

                                    invoiceHdrRes.Status = TrxStatus; // resMap.DebitAuthCode == "00" ? 2 : invoiceDtlRes.Status;
                                    invoiceHdrRes.Inuse = Inuse; // 1;
                                    invoiceHdrRes.LastUpdate = DateTime.Now;
                                    invoiceHdrRes.SyncLastUpd = DateTime.Now;
                                    invoiceHdrRes.UpdateBy = "SYSTEM";
                                    await _invoiceHDRRepo.Update(invoiceHdrRes);
                                }
                            }

                        }

                        transHdrRes.TransStatus = resMap.DebitAuthCode;
                        transHdrRes.Status = TrxHDRStatus;//TrxStatus;
                        transHdrRes.CustPrivilege = bankCode;
                        transHdrRes.ShiftCode = resMap.DebitAuthCode;
                        transHdrRes.LastUpdate = DateTime.Now;
                        transHdrRes.SyncLastUpd = DateTime.Now;
                        transHdrRes.UpdateBy = "SYSTEM";
                        await transHdrLogic.Update(transHdrRes);

                        //Remark for CWMS Enhancement
                        if (resMap.DebitAuthCode == "00")
                        {
                            var contBizRegID = invoiceDtlRes.BizRegID;
                            var bizentityLogic = new Core.Entity.Repo.BIZENTITY(_env, _connectionString);
                            var bizentityRes = bizentityLogic.Retrieve(x => x.BizRegID == contBizRegID);
                            if (bizentityRes != null)
                            {
                                #region change customer to active is payment made
                                //var contSuspend = new Model.SuspendCustomerRequest
                                //{
                                //    CustomerID = bizentityRes.BizRegID,
                                //    Status = 1,
                                //    TransporterID = bizentityRes.RefNo1,
                                //    UserID = bizentityRes.BizRegID
                                //};

                                //var resSuspend = await _accountingManager.SuspendCustomer(contSuspend);
                                #endregion


                            }
                            var resPostSAP = await _sapManager.PostToSAP(transHdrRes.TransNo);
                        }
                    }
                    return (new AjaxResponse(new { success = true }));
                }
                return new AjaxResponse(new { success = false });
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }

        #region old service
        public async Task<dynamic> UpdateOldQ(string TransNo)//input transno
        {
            try
            {
                TRANSHDR transHdrLogic = new TRANSHDR(_env, _connectionString);
                TRANSDTL transDtlLogic = new TRANSDTL(_env, _connectionString);
                TRANSTENDER transTenderLogic = new TRANSTENDER(_env, _connectionString);
                TENDER TenderLogic = new TENDER(_env, _connectionString);
                TRANSFPX transFpxLogic = new TRANSFPX(_env, _connectionString);
                TRANSCDITEM transCdItemLogic = new TRANSCDITEM(_env, _connectionString);
                

                var invoiceHdrLogic = new BaseRepository<DTO.Questil.Payment.INVOICEHDR_CWMS>(_env, _connectionString);

                INVOICEITEM invoiceDtlLogic;
                //INVOICEHDR_OLD invoiceHdrLogic;
                DTO.Accounting.TRANSHDR transHdrRes = new DTO.Accounting.TRANSHDR();
                List<DTO.Accounting.TRANSDTL> transDtlListRes = new List<DTO.Accounting.TRANSDTL>();
                DTO.Accounting.TRANSTENDER transTenderRes = new DTO.Accounting.TRANSTENDER();
                DTO.Accounting.TRANSCDITEM TransCdItemRes = new DTO.Accounting.TRANSCDITEM();
                DTO.Accounting.INVOICEITEM invoiceDtlRes = new DTO.Accounting.INVOICEITEM();
                //DTO.Accounting.INVOICEHDR_OLD invoiceHdrRes;
                AjaxResponse resValidate = null;
                AjaxResponse resAjax;
                MapValidateStatus resMap;
                NameValueCollection qscoll;
                string FPXID = string.Empty;
                string refInfo = string.Empty;
                string bankCode = string.Empty;
                string billNo = string.Empty;
                string invoiceID = string.Empty;
                byte Inuse = 0;
                byte TrxStatus = 0;
                byte TrxHDRStatus = 0;

                transHdrRes = transHdrLogic.Retrieve(x => x.TransNo == TransNo);//get required data
                if (transHdrRes == null)
                {
                    Log.Error("Error - ValidateFPX  - Incorrect Trans No");
                    return false;
                }
                //return new AjaxResponse(new { success = false });


                transDtlListRes = transDtlLogic.RetrieveAll(x => x.TransNo == TransNo);
                if (transDtlListRes == null)
                {
                    Log.Error("Error - ValidateFPX  - Incorrect Trans No");
                    return false;
                }

                if (transDtlListRes.Count() == 0)
                {
                    Log.Error("Error - ValidateFPX  - Trans Detail Cannot Found, TransNo : " + transHdrRes.TransNo + " at " + DateTime.Now);
                    return false;
                }

                transTenderRes = transTenderLogic.Retrieve(x => x.TransNo == TransNo);
                if (transTenderRes == null)
                {
                    transTenderRes = new DTO.Accounting.TRANSTENDER();
                    transTenderRes.TenderID = "997";
                }
                //return new AjaxResponse(new { success = false });
                //return new AjaxResponse(new ErrorInfo("Incorrect Trans No"));
                DTO.Accounting.TRANSFPX transFpxTemp = transFpxLogic.Retrieve(x => x.TransNo == TransNo);
                var sysPrefbLogic = new Base.Core.General.Repo.SYSPREFB(_env, _connectionString);
                var mode = 1;
                var checkMode = sysPrefbLogic.Retrieve(x => x.SYSKey == "CHKFAILFPXUSENEWSVC" && x.BranchID == "PLX");
                if (checkMode != null)
                {
                    mode = Convert.ToInt32(checkMode.SYSValue);
                }
                if (mode == 1)
                {
                    var resUpdate = await this.updatePaymentTable(transTenderRes,transHdrRes,transFpxTemp,transDtlListRes,mode);
                    return resUpdate; 
                }
                else
                {
                    var resValidateFPX = await _sapManager.ValidateFPXOldSync(transHdrRes.TransNo);//post to sap
                    var resUpdate = await this.updatePaymentTable(transTenderRes, transHdrRes, transFpxTemp, transDtlListRes,mode);//new 
                    return resUpdate;

                }

            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                return false;

            }
        }

        public async Task<dynamic> updatePaymentTable(DTO.Accounting.TRANSTENDER transTenderRes, DTO.Accounting.TRANSHDR transHdrRes, DTO.Accounting.TRANSFPX transFpxTemp, List<DTO.Accounting.TRANSDTL> transDtlListRes,int mode)
        {
            try
            {
                TRANSHDR transHdrLogic = new TRANSHDR(_env, _connectionString);
                TRANSDTL transDtlLogic = new TRANSDTL(_env, _connectionString);
                TRANSTENDER transTenderLogic = new TRANSTENDER(_env, _connectionString);
                TENDER TenderLogic = new TENDER(_env, _connectionString);
                TRANSFPX transFpxLogic = new TRANSFPX(_env, _connectionString);
                TRANSCDITEM transCdItemLogic = new TRANSCDITEM(_env, _connectionString);
                var sysPrefbLogic = new Base.Core.General.Repo.SYSPREFB(_env, _connectionString);
                INVOICEITEM invoiceDtlLogic;
                var invoiceHdrLogic = new BaseRepository<DTO.Questil.Payment.INVOICEHDR_CWMS>(_env, _connectionString);
                DTO.Accounting.INVOICEITEM invoiceDtlRes = new DTO.Accounting.INVOICEITEM();
                DTO.Accounting.TRANSCDITEM TransCdItemRes = new DTO.Accounting.TRANSCDITEM();


                AjaxResponse resValidate = null;
                AjaxResponse resAjax;
                MapValidateStatus resMap;
                NameValueCollection qscoll;
                string FPXID = string.Empty;
                string refInfo = string.Empty;
                string bankCode = string.Empty;
                string billNo = string.Empty;
                string invoiceID = string.Empty;
                byte Inuse = 0;
                byte TrxStatus = 0;
                byte TrxHDRStatus = 0;

                if (transTenderRes.TenderID == "996" || transTenderRes.TenderID == "997")
                {
                    refInfo = transFpxTemp.RefInfo;//transTenderRes.RefToken;
                    if (string.IsNullOrEmpty(refInfo))
                    {
                        Log.Error("Error - ValidateFPX  - Incorrect Reference, Empty Token, TransNo : " + transHdrRes.TransNo + " at " + DateTime.Now);
                        return false;
                    }


                    if (transTenderRes.TenderID == "997")//get fpx response
                    {
                        resValidate = await ValidateFPXStatusOLDQ(refInfo);
                    }



                    if (resValidate.Success)//if success. able get response
                    {
                        resMap = (MapValidateStatus)resValidate.Result;
                        if (resMap == null)//some condition to test the result of fpx response
                        {
                            Log.Error("Error - ValidateFPX  - Error Validating Payment Status, TransNo : " + transHdrRes.TransNo + " at " + DateTime.Now);
                            return false;
                        }

                        if (string.IsNullOrEmpty(resMap.CheckSum))
                        {
                            Log.Error("Error - ValidateFPX  - Error Validating Payment Status, TransNo : " + transHdrRes.TransNo + " at " + DateTime.Now);
                            return false;
                        }

                        if (resMap.CheckSum == ("ERROR"))
                        {
                            Log.Error("Error - ValidateFPX  - Cannot Reach FPX Server, or FPX Server Blocked due expired certificate");
                            return false;
                        }


                        qscoll = HttpUtility.ParseQueryString(resMap.CheckSum);//parse to array and insert to variable2
                        if (qscoll != null)
                        {
                            if (transTenderRes.TenderID == "997")
                            {
                                FPXID = qscoll["fpx_fpxTxnId"];
                                bankCode = qscoll["fpx_buyerBankId"];
                                transHdrRes.ShiftCode = qscoll["fpx_debitAuthCode"];
                                transTenderRes.BillNo = FPXID;


                            }

                        }

                        //UpdateCCRefInfoFPX save the response ccrefinfo to transfpx
                        var chkRefinfo = "";
                        if (resMap.CheckSum.Length > 255)
                        {
                            chkRefinfo = resMap.CheckSum.Substring(0, 254);
                        }
                        transFpxTemp.CCrefInfo = chkRefinfo;
                        transFpxTemp.SyncLastUpd = DateTime.Now;
                        transFpxTemp.LastSyncBy = 3;
                        await transFpxLogic.Update(transFpxTemp);
                        //end here

                        string[] cancelStr = { };

                        var cancelList = await _generalManager.GetCodeMasterbyCodeType("FPX");
                        if (cancelList.Success)
                        {
                            var res = (dynamic)cancelList.Result;
                            List<string> list = new List<string>();
                            foreach (var idx in res.Items)
                            {
                                list.Add(idx["Code"]);
                            }
                            list.Remove("00");
                            list.Remove("09");
                            cancelStr = list.ToArray();
                        }

                        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        {

                            transHdrRes.Status = 0; // default status if payment not success
                            TrxStatus = 1;


                            //set the status for payment table2
                            if (resMap.DebitAuthCode == "00")
                            {
                                Inuse = 1;
                                TrxHDRStatus = 2;
                                TrxStatus = 2;
                                transHdrRes.Status = 2;
                                transTenderRes.TransStatus = 2;
                            }
                            else if (resMap.DebitAuthCode == "51")
                            {
                                Inuse = 0;
                                TrxHDRStatus = 3;
                                TrxStatus = 3;
                                transHdrRes.Status = 3;
                            }
                            else if (resMap.DebitAuthCode == "57")
                            {
                                //'Transaction not permitted
                                Inuse = 0;
                                TrxHDRStatus = 3;
                                TrxStatus = 3;
                                transHdrRes.Status = 3;
                            }
                            else if (resMap.DebitAuthCode == "80")
                            {
                                //buyer cancel transaction
                                Inuse = 0;
                                TrxHDRStatus = 4;
                                TrxStatus = 1;
                                transHdrRes.Status = 4;
                                //paidbatchcodelist = objInvoiceHdr.GetPaidBatchCode(String.Join("','", listTransDtl.Select(Function(x) x.SerialNo).ToList()), statuscheck)
                            }
                            else if ((resMap.DebitAuthCode == "99") || (resMap.DebitAuthCode == "09"))
                            {
                                //'Pending Authorization
                                Inuse = 1;
                                TrxHDRStatus = 1;
                                TrxStatus = 1;
                                transHdrRes.Status = 1;
                            }
                            else if (cancelStr.Contains(resMap.DebitAuthCode))//(resMap.DebitAuthCode == "1C") 
                            {
                                //'Buyer Choose Cancel At Login Page
                                Inuse = 0;
                                TrxHDRStatus = 4;
                                TrxStatus = 1;
                                transHdrRes.Status = 4;
                            }
                            else//else means this transaction status is not yet paid and able to do payment again (inuse=0)
                            {
                                TrxStatus = 1;
                                TrxHDRStatus = 1;
                                transHdrRes.Status = 1;
                                Inuse = 0;
                            }


                            var tenderTemp = TenderLogic.Retrieve(x => x.TenderID == "997");

                            transTenderRes.BillNo = FPXID;


                            transTenderRes.IsApproved = 1;

                            transTenderRes.ApprovedDate = DateTime.Now;
                            transTenderRes.RespCode = resMap.DebitAuthCode;
                            transTenderRes.ExternalID = FPXID;
                            transTenderRes.AuthCode = resMap.CheckSum;

                            transTenderRes.TenderAmt = transHdrRes.TransAmt;
                            transTenderRes.TermID = 1;
                            transTenderRes.TransSeq = 1;
                            transTenderRes.ExchgRate = 0;
                            transTenderRes.TenderDue = 0;
                            transTenderRes.ChangeAmt = 0;

                            if (transTenderRes.rowguid == Guid.Empty)//if not found transtender data, and the response is 00 from fpx, create new.
                            {
                                transTenderRes.BizRegID = transHdrRes.BizRegID;
                                transTenderRes.BizLocID = transHdrRes.BizLocID;
                                transTenderRes.TermID = 1;
                                transTenderRes.TransNo = transHdrRes.TransNo;
                                transTenderRes.TransSeq = 1;

                                transTenderRes.TransDate = DateTime.Now.Date;
                                transTenderRes.TransTime = DateTime.Now.ToString("HH:mm");
                                transTenderRes.TenderType = tenderTemp.TenderType.ToString();
                                transTenderRes.TenderID = tenderTemp.TenderID;
                                transTenderRes.TenderAmt = transHdrRes.TransAmt;
                                transTenderRes.ExchgRate = 0;
                                transTenderRes.TenderDue = 0;
                                transTenderRes.ChangeAmt = 0;
                                transTenderRes.Currency = tenderTemp.CurrencyCode;
                                transTenderRes.IsApproved = 1;
                                transTenderRes.ApprovedDate = DateTime.Now;

                                transTenderRes.Posted = 1;
                                transTenderRes.CreateDate = DateTime.Now;
                                transTenderRes.CreateBy = String.Empty;
                                transTenderRes.LastUpdate = DateTime.Now;
                                transTenderRes.SyncCreate = DateTime.Now;
                                transTenderRes.SyncLastUpd = DateTime.Now;
                                transTenderRes.IsHost = 0;
                                transTenderRes.LastSyncBy = String.Empty;

                                if (resMap.DebitAuthCode == "00")
                                {
                                    await transTenderLogic.Create(transTenderRes);

                                }
                            }
                            else
                            {
                                transTenderRes.LastUpdate = DateTime.Now;
                                transTenderRes.SyncLastUpd = DateTime.Now;
                                await transTenderLogic.Update(transTenderRes);

                            }

                            invoiceDtlLogic = new INVOICEITEM(_env, _connectionString);
                            //invoiceHdrLogic = new INVOICEHDR_OLD(_env, _connectionString);


                            //this process to update each invoice table status
                            var tempBizRegID = transHdrRes.BizRegID;
                            foreach (var index in transDtlListRes)
                            {
                                index.Status = TrxHDRStatus;//TrxStatus;
                                index.LastUpdate = transHdrRes.LastUpdate;
                                index.SyncLastUpd = transHdrRes.SyncLastUpd;
                                index.UpdateBy = transHdrRes.UpdateBy;
                                index.ItemCode = index.SerialNo;
                                await transDtlLogic.Update(index);

                                invoiceID = index.SerialNo;
                                invoiceDtlRes = invoiceDtlLogic.Retrieve(x => x.BatchCode == invoiceID && x.BizRegID == tempBizRegID);
                                if (invoiceDtlRes != null)
                                {
                                    //var invoiceID = invoiceDtlRes.BatchCode;
                                    var tempMonthCode = invoiceDtlRes.MonthCode;
                                    var invoiceHdrRes = invoiceHdrLogic.Retrieve(x => x.BatchCode == invoiceID && x.MonthCode == tempMonthCode && ((x.Status != 2) || (x.Status != 4)));
                                    if (invoiceHdrRes != null)
                                    {
                                        invoiceDtlRes.Status = TrxStatus; // resMap.DebitAuthCode == "00" ? 2 : invoiceDtlRes.Status;
                                        invoiceDtlRes.Inuse = Inuse; // 1;
                                        invoiceDtlRes.LastUpdate = DateTime.Now;
                                        invoiceDtlRes.SyncLastUpd = DateTime.Now;
                                        invoiceDtlRes.UpdateBy = "SYSTEM";
                                        await invoiceDtlLogic.Update(invoiceDtlRes);

                                        invoiceHdrRes.Status = TrxStatus; // resMap.DebitAuthCode == "00" ? 2 : invoiceDtlRes.Status;
                                        invoiceHdrRes.Inuse = Inuse; // 1;
                                        invoiceHdrRes.LastUpdate = DateTime.Now;
                                        invoiceHdrRes.SyncLastUpd = DateTime.Now;
                                        invoiceHdrRes.UpdateBy = "SYSTEM";
                                        await invoiceHdrLogic.Update(invoiceHdrRes);


                                    }
                                    TransCdItemRes = transCdItemLogic.Retrieve(x => x.BatchCode == invoiceID);//if exist , update also the transcditem status
                                    if (TransCdItemRes != null)
                                    {
                                        TransCdItemRes.BatchCode = index.ItemCode;
                                        TransCdItemRes.ItemCode = index.ItemCode;
                                        TransCdItemRes.Status = TrxStatus;
                                        TransCdItemRes.SyncLastUpd = DateTime.Now;
                                        await transCdItemLogic.Update(TransCdItemRes);
                                    }
                                }

                            }
                            //this process to update the transhdr
                            transHdrRes.TransStatus = TrxHDRStatus.ToString();//resMap.DebitAuthCode;
                            transHdrRes.Status = TrxHDRStatus;//TrxStatus;
                            transHdrRes.CustPrivilege = bankCode;
                            transHdrRes.ShiftCode = resMap.DebitAuthCode;
                            transHdrRes.BillNo = FPXID;
                            transHdrRes.LastUpdate = DateTime.Now;
                            transHdrRes.SyncLastUpd = DateTime.Now;
                            transHdrRes.UpdateBy = "SYSTEM";
                            await transHdrLogic.Update(transHdrRes);


                            //save all changes
                            scope.Complete();
                            //if 00 send to sap
                            if (resMap.DebitAuthCode == "00" && mode == 1)
                            {

                                var resPostSAP = await _sapManager.PostToSAP(transHdrRes.TransNo);
                                if (resPostSAP == false)//if fail send, remark for testing, 
                                {
                                    Log.Error("Error - ValidateFPX  - Post To SAP Failed , TransNo : " + transHdrRes.TransNo + " at " + DateTime.Now);
                                    return false;
                                }
                            }
                        }

                    }
                    else//if not success, not able to get validate from fpx
                    {
                        Log.Error("Error - ValidateFPX  - Cant Validate The Transaction, TransNo : " + transHdrRes.TransNo + " at " + DateTime.Now);
                        return false;
                    }


                }

                return true;
            }
            catch(Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                return false;
            }
        }

        public async Task<dynamic> CheckFailedFPXQ()
        {
            try
            {
                var dateStart = DateTime.Now;
                List<string> successTransno = new List<string>();
                List<string> failTransno = new List<string>();
                var param = new CheckFailedFPX()
                {

                };
                var sysPrefbLogic = new Base.Core.General.Repo.SYSPREFB(_env, _connectionString);
                var invoiceHDRLogic = new INVOICEHDR(_env, _connectionString);
                var delay = 0;
                var delayCheck = sysPrefbLogic.Retrieve(x => x.SYSKey == "CHECKFAILFPXDELAY" && x.BranchID == "PLX");
                if (delayCheck != null)
                {
                    delay = Convert.ToInt32(delayCheck.SYSValue);
                }
                var successCount = 0;
                var failCount = 0;
                var total = 0;
                var dataSP = invoiceHDRLogic.StoredProcedureViewMap<Map.MapCheckFpxList, CheckFailedFPX>("dbo.GetFPXUnsynced", param);
                total = dataSP.Count();
                if (total > 0)
                {
                    foreach (var i in dataSP)
                    {
                        Thread.Sleep(delay);
                        var hasil = await this.UpdateOldQ(i.FpxTransNo);
                        if (hasil == true)
                        {
                            successCount++;
                            successTransno.Add(i.FpxTransNo);
                        }
                        else
                        {
                            failCount++;
                            failTransno.Add(i.FpxTransNo);
                        }

                    }

                    var ts = DateTime.Now - dateStart;

                    var fail = "";
                    if (failTransno.Count > 0)
                    {
                        fail = string.Join(",", failTransno);
                    }

                    var success = "";
                    if (successTransno.Count > 0)
                    {
                        success = string.Join(",", successTransno);
                    }

                    var time = Math.Round(ts.TotalMinutes, 2).ToString() + " minutes (" + Math.Round(ts.TotalSeconds, 2).ToString() + " seconds)";
                    var res = new CheckFpxFailedResponse
                    {
                        duration = time,
                        SuccessfulySync = successCount,
                        FailedSync = failCount,
                        totalSync = total,
                        SuccessTransNo = success,
                        failTransno = fail

                    };
                    string email = _appConfiguration["CRONConfig:EmailAddress"];

                    var emailCheck = sysPrefbLogic.Retrieve(x => x.SYSKey == "CHECKFAILFPXMAIL" && x.BranchID == "PLX");
                    if (emailCheck != null)
                    {
                        email = emailCheck.SYSValue;
                    }
                    if (total > 0)
                    {
                        await _accountingEmailer.CheckFPXNotification(res, email);
                    }

                    return res;
                }
                else
                {
                    return "No Data Available To Process";
                }
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }

        public async Task<AjaxResponse> ValidateFPXStatusOLDQ(System.String refInfo)
        {
            MapValidateStatus result = new MapValidateStatus();
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
                string fpx_checkSum = RSASign.RSASignValue(refInfo, path);

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

                var sunSystem = new SUNSystemConnection();
                result.CheckSum = sunSystem.POSTWebRequest(url, posting_data, _byteVersion);
                if (string.IsNullOrEmpty(result.CheckSum))
                {
                    result.Message = "No return from FPX";
                    result.DebitAuthCode = "99";
                    return new AjaxResponse(new ErrorInfo(result.Message));
                }

                //if (result.CheckSum.ToUpper().Contains("ERROR"))
                //{
                //    result.Message = result.CheckSum;
                //    result.DebitAuthCode = "99";
                //    return new AjaxResponse(new ErrorInfo(result.Message));
                //}

                if (result.CheckSum == "ERROR")
                {
                    result.Message = result.CheckSum;
                    result.DebitAuthCode = "99";
                    return new AjaxResponse(new ErrorInfo(result.Message));
                }

                var clearView = result.CheckSum.Replace("&", "&" + System.Environment.NewLine);

                qscoll = HttpUtility.ParseQueryString(result.CheckSum);
                if (qscoll != null)
                {
                    if (qscoll["fpx_debitAuthCode"] == "ERROR")
                    {
                        result.DebitAuthCode = "99";
                        result.Message = result.CheckSum;
                        return new AjaxResponse(result);
                    }
                    else
                    {
                        fpx_debitAuthCode = qscoll["fpx_debitAuthCode"];
                    }

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
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }
        #endregion
        public async Task<AjaxResponse> AddTransaction(Accounting.Map.Payment json)
        {
            try
            {
                if (json.Mode == 0)
                {
                    #region Mode 0
                    TRANSHDR transHDRLogic = new TRANSHDR(_env, _connectionString);
                    TRANSDTL transDTLLogic = new TRANSDTL(_env, _connectionString);

                    TRANSTENDER transTenderLogic = new TRANSTENDER(_env, _connectionString);
                    //INVOICEHDR_OLD invoiceHDRLogic = new INVOICEHDR_OLD(_env, _connectionString);
                    INVOICEITEM invoiceDTLLogic = new INVOICEITEM(_env, _connectionString);
                    //INVOICEDTL invoiceDTLLogic = new INVOICEDTL(_env, _connectionString);
                    Core.General.Repo.CODEMASTER codemasterLogic = new Core.General.Repo.CODEMASTER(_env, _connectionString);

                    TENDER tenderLogic = new TENDER(_env, _connectionString);

                    DTO.Accounting.TRANSHDR dataTransHDR = _objectMapper.Map<DTO.Accounting.TRANSHDR>(json.TransHdr);
                    IList<DTO.Accounting.TRANSDTL> dataTransDTL = _objectMapper.Map<IList<DTO.Accounting.TRANSDTL>>(json.TransDtl);
                    DTO.Accounting.TRANSTENDER dataTransTender;

                    DateTime dateNow = DateTime.Now;
                    string contBankCode = json.BankCode;
                    string contTenderID = json.TenderID;
                    string contMerchantID = string.Empty;
                    List<string> joinedOnlinePayment = new List<string>() { "996", "997" };
                    //string InvNo = json.SiNumber;
                    bool isOnlinePayment = false;
                    //string invoiceID = json.SiNumber;

                    var tenderDetail = tenderLogic.Retrieve(x => x.TenderID == contTenderID);
                    if (tenderDetail == null)
                        return new AjaxResponse(new { success = false, message = "Invalid payment method" });

                    if (joinedOnlinePayment.Contains(json.TenderID))
                        isOnlinePayment = true;

                    //var invoiceHDR = invoiceHDRLogic.Retrieve(x => x.BatchCode == InvNo); //need mode for CWMS / LGM
                    //if (invoiceHDR != null)
                    //{
                    //    if (json.TransHdr.TransAmt < invoiceHDR.TotalAmt1)
                    //        return Json(new AjaxResponse(new { success = false, message = "Total amount pay cannot less than invoice amount" }));
                    //}
                    //else
                    //{
                    //    return Json(new AjaxResponse(new { success = false, message = "Total amount pay cannot less than invoice amount" }));
                    //}

                    //For FPX and Maybank
                    if (isOnlinePayment)
                    {
                        var bankDetail = codemasterLogic.Retrieve(x => x.Code == contBankCode);
                        if (bankDetail == null)
                            return new AjaxResponse(new { success = false, message = "Invalid bank code" });

                        if (!string.IsNullOrEmpty(json.RequestMessage))
                        {
                            if (json.RequestMessage.Contains("|"))
                            {
                                var reqMsgSplit = json.RequestMessage.Split("|");
                                if (reqMsgSplit != null)
                                {
                                    if (tenderDetail.TenderID == "997")
                                    {
                                        contMerchantID = reqMsgSplit[14];
                                    }
                                    else if (tenderDetail.TenderID == "996")
                                    {
                                        contMerchantID = reqMsgSplit[1];
                                    }
                                }
                            }
                        }

                        dataTransTender = new DTO.Accounting.TRANSTENDER
                        {
                            BizRegID = dataTransHDR.BizRegID,
                            BizLocID = dataTransHDR.BizLocID,
                            TransNo = dataTransHDR.TransNo,
                            TransSeq = 1,
                            BillNo = dataTransHDR.BillNo,
                            TransDate = dateNow,
                            TransTime = dateNow.ToString("HH:mm"),
                            TenderType = tenderDetail.TenderType.ToString(),
                            TenderID = json.TenderID,
                            TenderAmt = dataTransHDR.TransAmt,
                            Currency = tenderDetail.CurrencyCode,
                            ExternalID = "",
                            MerchantID = contMerchantID,
                            RefToken = json.RequestMessage,
                            BankCode = bankDetail.Code,
                            BankAccNo = "",
                            RespCode = "99", //default for pending authorization
                            AuthCode = "",
                            ApprovedDate = null,
                            TransStatus = 0,
                            Posted = 1,
                            CreateDate = dateNow,
                            CreateBy = dataTransHDR.BizRegID,
                            LastUpdate = null,
                            SyncCreate = null,
                            SyncLastUpd = null,
                            IsHost = 0
                        };
                        await transTenderLogic.Create(dataTransTender);
                    }
                    else
                    {
                        if (json.OfflineInfoList != null && json.OfflineInfoList.Count() > 0)
                        {
                            var transSeqNo = 1;
                            foreach (var index in json.OfflineInfoList)
                            {
                                DateTime clrDate = DateTime.ParseExact(index.clearingDate.ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                tenderDetail = tenderLogic.Retrieve(x => x.TenderID == contTenderID);
                                dataTransTender = new DTO.Accounting.TRANSTENDER
                                {
                                    BizRegID = dataTransHDR.BizRegID,
                                    BizLocID = dataTransHDR.BizLocID,
                                    TransNo = dataTransHDR.TransNo,
                                    TransSeq = transSeqNo,
                                    BillNo = dataTransHDR.BillNo,
                                    TransDate = dateNow,
                                    TransTime = dateNow.ToString("HH:mm"),
                                    ClearingDate = clrDate,
                                    TenderType = tenderDetail.TenderType.ToString(),
                                    TenderID = index.tenderID,
                                    TenderAmt = index.amount,
                                    Currency = tenderDetail.CurrencyCode,
                                    RefToken = index.refNumber,
                                    BankCode = index.bankDetails,
                                    BankAccNo = "",
                                    RespCode = "00",
                                    AuthCode = "",
                                    IsApproved = 1,
                                    ApprovedDate = dateNow,
                                    TransStatus = 1,
                                    Posted = 1,
                                    CreateDate = dateNow,
                                    CreateBy = dataTransHDR.BizRegID,
                                    LastUpdate = null,
                                    SyncCreate = null,
                                    SyncLastUpd = null,
                                    IsHost = 0
                                };
                                await transTenderLogic.Create(dataTransTender);
                                transSeqNo++;
                            }
                        }
                    }

                    //foreach (var row in dataTransDTL)
                    //{
                    //    row.Status = isOnlinePayment ? Convert.ToByte(0) : Convert.ToByte(1);
                    //    transDTLLogic.Create(row);
                    //}

                    int seqNo = 1;
                    foreach (var row in json.TransDtl)
                    {
                        DTO.Accounting.TRANSDTL contDtl = new DTO.Accounting.TRANSDTL
                        {
                            BizRegID = row.BizRegID,
                            BizLocID = row.BizLocID,
                            TermID = 1,
                            TransNo = dataTransHDR.TransNo,
                            TransSeq = seqNo,
                            BillNo = dataTransHDR.BillNo,
                            EntryTime = DateTime.Now.ToString("HH:mm"),
                            StkType = "INV",
                            Qty = 1,
                            Remark = row.Remark,
                            TolAmt = row.TolAmt,
                            Posted = 1,
                            Flag = 1,
                            Status = 1,
                            IsHost = 0,
                            ItemCode = row.ItemCode,
                            SerialNo = row.TransNo
                        };
                        await transDTLLogic.Create(contDtl);
                        seqNo++;
                    }

                    dataTransHDR.TransDate = dateNow;
                    dataTransHDR.TransStartTime = dateNow.ToString("HH:mm");
                    dataTransHDR.TransEndTime = dateNow.ToString("HH:mm");
                    dataTransHDR.ShiftCode = isOnlinePayment ? "99" : "00";
                    dataTransHDR.TransStatus = isOnlinePayment ? "99" : "00";
                    dataTransHDR.Status = isOnlinePayment ? Convert.ToByte(0) : Convert.ToByte(1);
                    await transHDRLogic.Create(dataTransHDR);

                    //Set Inuse for Multi Invoice
                    if (json.TransDtl != null && json.TransDtl.Count() > 0)
                    {
                        //var trxID = json.TransDtlHdr.Select(x => x.SiNumber).ToList();
                        //var invoiceHdr = invoiceHDRLogic.RetrieveAll(x => trxID.Equals(x.BatchCode));
                        //var invoiceDtl = invoiceITEMLogic.RetrieveAll(x => trxID.Equals(x.BatchCode));

                        //foreach (var indexHdr in invoiceHdr)
                        //{
                        //    indexHdr.LastUpdate = DateTime.Now;
                        //    indexHdr.Inuse = 1;
                        //    invoiceHDRLogic.Update(indexHdr);
                        //}

                        //foreach (var indexDtl in invoiceDtl)
                        //{
                        //    indexDtl.LastUpdate = DateTime.Now;
                        //    indexDtl.Inuse = 1;
                        //    invoiceITEMLogic.Update(indexDtl);
                        //}

                        var tempBizRegID = json.TransHdr.BizRegID;
                        foreach (var index in json.TransDtl)
                        {
                            var tempInvoiceNo = (string)index.ItemCode;
                            var invoiceDtl = invoiceDTLLogic.Retrieve(x => x.BatchCode == tempInvoiceNo && x.BizRegID == tempBizRegID);
                            if (invoiceDtl != null)
                            {
                                var tempMonthCode = invoiceDtl.MonthCode;
                                var invoicehdr = _invoiceHDRRepo.Retrieve(x => x.BatchCode == tempInvoiceNo && x.MonthCode == tempMonthCode);
                                if (invoicehdr != null)
                                {
                                    invoicehdr.LastUpdate = DateTime.Now;
                                    invoicehdr.Inuse = 1;
                                    await _invoiceHDRRepo.Update(invoicehdr);

                                    invoiceDtl.LastUpdate = DateTime.Now;
                                    invoiceDtl.Inuse = 1;
                                    await invoiceDTLLogic.Update(invoiceDtl);
                                }
                            }
                        }
                    }

                    //Set Inuse for Single Invoice
                    //invoiceHDR = invoiceHDRLogic.Retrieve(x => x.PBTCode == InvNo);
                    //if (invoiceHDR != null)
                    //{
                    //    invoiceHDR.LastUpdate = DateTime.Now;
                    //    invoiceHDR.Inuse = 1;
                    //    invoiceHDRLogic.Update(invoiceHDR);
                    //}

                    //var invoiceItem = invoiceITEMLogic.Retrieve(x => x.PBTCode == InvNo);
                    //if (invoiceItem != null)
                    //{
                    //    invoiceItem.LastUpdate = DateTime.Now;
                    //    invoiceItem.Inuse = 1;
                    //    invoiceITEMLogic.Update(invoiceItem);
                    //}

                    //return Json(new AjaxResponse(new { success = true, message = string.Empty }));
                    #endregion
                }
                else if (json.Mode == 1)
                {
                    #region Mode 1
                    TRANSHDR transHDRLogic = new TRANSHDR(_env, _connectionString);
                    TRANSDTL transDTLLogic = new TRANSDTL(_env, _connectionString);

                    TRANSTENDER transTenderLogic = new TRANSTENDER(_env, _connectionString);
                    INVOICEHDR invoiceHDRLogic = new INVOICEHDR(_env, _connectionString);
                    //INVOICEITEM invoiceITEMLogic = new INVOICEITEM(_env, _connectionString);
                    INVOICEDTL invoiceDTLLogic = new INVOICEDTL(_env, _connectionString);
                    INVOICETAX invoiceTAXLogic = new INVOICETAX(_env, _connectionString);
                    Core.General.Repo.CODEMASTER codemasterLogic = new Core.General.Repo.CODEMASTER(_env, _connectionString);

                    TENDER tenderLogic = new TENDER(_env, _connectionString);

                    DTO.Accounting.TRANSHDR dataTransHDR = _objectMapper.Map<DTO.Accounting.TRANSHDR>(json.TransHdr);
                    IList<DTO.Accounting.TRANSDTL> dataTransDTL = _objectMapper.Map<IList<DTO.Accounting.TRANSDTL>>(json.TransDtl);
                    DTO.Accounting.TRANSTENDER dataTransTender;

                    DateTime dateNow = DateTime.Now;
                    string contBankCode = json.BankCode;
                    string contTenderID = json.TenderID;
                    string contMerchantID = string.Empty;
                    List<string> joinedOnlinePayment = new List<string>() { "996", "997" };
                    string siNumber = json.SiNumber;
                    bool isOnlinePayment = false;
                    //string invoiceID = json.InvoiceID;

                    var tenderDetail = tenderLogic.Retrieve(x => x.TenderID == contTenderID);
                    if (tenderDetail == null)
                        return new AjaxResponse(new { success = false, message = "Invalid payment method" });

                    if (joinedOnlinePayment.Contains(json.TenderID))
                        isOnlinePayment = true;

                    var invoiceHDR = invoiceHDRLogic.Retrieve(x => x.TransNo == siNumber);
                    if (invoiceHDR != null)
                    {
                        if (json.TransHdr.TransAmt < invoiceHDR.TransAmt)
                            return new AjaxResponse(new { success = false, message = "Total amount pay cannot less than invoice amount" });
                    }

                    //For FPX and Maybank
                    using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        if (isOnlinePayment)
                        {
                            var bankDetail = codemasterLogic.Retrieve(x => x.Code == contBankCode);
                            if (bankDetail == null)
                                return new AjaxResponse(new { success = false, message = "Invalid bank code" });

                            if (!string.IsNullOrEmpty(json.RequestMessage))
                            {
                                if (json.RequestMessage.Contains("|"))
                                {
                                    var reqMsgSplit = json.RequestMessage.Split("|");
                                    if (reqMsgSplit != null)
                                    {
                                        if (tenderDetail.TenderID == "997")
                                        {
                                            contMerchantID = reqMsgSplit[14];
                                        }
                                        else if (tenderDetail.TenderID == "996")
                                        {
                                            contMerchantID = reqMsgSplit[1];
                                        }
                                    }
                                }
                            }

                            dataTransTender = new DTO.Accounting.TRANSTENDER
                            {
                                BizRegID = dataTransHDR.BizRegID,
                                BizLocID = dataTransHDR.BizLocID,
                                TransNo = dataTransHDR.TransNo,
                                TransSeq = 1,
                                BillNo = dataTransHDR.BillNo,
                                TransDate = dateNow,
                                TransTime = dateNow.ToString("HH:mm"),
                                TenderType = tenderDetail.TenderType.ToString(),
                                TenderID = json.TenderID,
                                TenderAmt = dataTransHDR.TransAmt,
                                Currency = tenderDetail.CurrencyCode,
                                ExternalID = "",
                                MerchantID = contMerchantID,
                                RefToken = json.RequestMessage,
                                BankCode = bankDetail.Code,
                                BankAccNo = "",
                                RespCode = "99", //default for pending authorization
                                AuthCode = "",
                                ApprovedDate = null,
                                TransStatus = 0,
                                Posted = 1,
                                CreateDate = dateNow,
                                //CreateBy = dataTransHDR.BizRegID,
                                CreateBy = AbpSession.UserData.BizRegID,
                                LastUpdate = null,
                                SyncCreate = null,
                                SyncLastUpd = null,
                                IsHost = 0
                            };
                            await transTenderLogic.Create(dataTransTender);
                        }
                        else
                        {
                            if (json.OfflineInfoList != null && json.OfflineInfoList.Count() > 0)
                            {
                                var transSeqNo = 1;
                                foreach (var index in json.OfflineInfoList)
                                {
                                    DateTime clrDate = DateTime.ParseExact(index.clearingDate.ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                    tenderDetail = tenderLogic.Retrieve(x => x.TenderID == contTenderID);
                                    dataTransTender = new DTO.Accounting.TRANSTENDER
                                    {
                                        BizRegID = dataTransHDR.BizRegID,
                                        BizLocID = dataTransHDR.BizLocID,
                                        TransNo = dataTransHDR.TransNo,
                                        TransSeq = transSeqNo,
                                        BillNo = dataTransHDR.BillNo,
                                        TransDate = dateNow,
                                        TransTime = dateNow.ToString("HH:mm"),
                                        ClearingDate = clrDate,
                                        TenderType = tenderDetail.TenderType.ToString(),
                                        TenderID = index.tenderID,
                                        TenderAmt = index.amount,
                                        Currency = tenderDetail.CurrencyCode,
                                        RefToken = index.refNumber,
                                        BankCode = index.bankDetails,
                                        BankAccNo = "",
                                        RespCode = "00",
                                        AuthCode = "",
                                        IsApproved = 1,
                                        ApprovedDate = dateNow,
                                        TransStatus = 2,
                                        Posted = 1,
                                        CreateDate = dateNow,
                                        CreateBy = dataTransHDR.BizRegID,
                                        LastUpdate = null,
                                        SyncCreate = null,
                                        SyncLastUpd = null,
                                        IsHost = 0
                                    };
                                    await transTenderLogic.Create(dataTransTender);
                                    transSeqNo++;
                                }
                            }
                        }

                        using (TransactionScope scope_details = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
                        {
                            foreach (var row in dataTransDTL)
                            {
                                row.Status = isOnlinePayment ? Convert.ToByte(0) : Convert.ToByte(2);
                                await transDTLLogic.Create(row);
                            }

                            dataTransHDR.TransDate = dateNow;
                            dataTransHDR.TransStartTime = dateNow.ToString("HH:mm");
                            dataTransHDR.TransEndTime = dateNow.ToString("HH:mm");
                            dataTransHDR.ShiftCode = isOnlinePayment ? "99" : "00";
                            dataTransHDR.TransStatus = isOnlinePayment ? "99" : "00";
                            dataTransHDR.Status = isOnlinePayment ? Convert.ToByte(0) : Convert.ToByte(2);
                            dataTransHDR.CreateBy = AbpSession.UserData.BizRegID;
                            await transHDRLogic.Create(dataTransHDR);

                            //using (TransactionScope scope_inuse = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
                            //{
                            //    //Set Inuse for Multi Invoice
                            //    if (json.TransDtl != null && json.TransDtl.Count() > 0 && json.TransDtl[0].ItemType == 1)
                            //    {
                            //        var trxID = json.TransDtl.Select(x => x.ItemCode).ToList();
                            //        var invoiceHdr = invoiceHDRLogic.RetrieveAll(x => trxID.Equals(x.TransNo));
                            //        var invoiceDtl = invoiceDTLLogic.RetrieveAll(x => trxID.Equals(x.TransNo));
                            //        var invoiceTax = invoiceTAXLogic.RetrieveAll(x => trxID.Equals(x.TransNo));

                            //        foreach (var indexHdr in invoiceHdr)
                            //        {
                            //            indexHdr.LastUpdate = DateTime.Now;
                            //            indexHdr.UpdateBy = AbpSession.UserData.BizRegID;
                            //            indexHdr.Inuse = 1;
                            //            await invoiceHDRLogic.Update(indexHdr);
                            //        }

                            //        foreach (var indexDtl in invoiceDtl)
                            //        {
                            //            indexDtl.LastUpdate = DateTime.Now;
                            //            indexDtl.UpdateBy = AbpSession.UserData.BizRegID;
                            //            indexDtl.Inuse = 1;
                            //            await invoiceDTLLogic.Update(indexDtl);
                            //        }

                            //        foreach (var indexTax in invoiceTax)
                            //        {
                            //            indexTax.Inuse = 1;
                            //            await invoiceTAXLogic.Update(indexTax);
                            //        }
                            //    }

                            //    //Set Inuse for Single Invoice
                            //    invoiceHDR = invoiceHDRLogic.Retrieve(x => x.TransNo == siNumber);
                            //    if (invoiceHDR != null)
                            //    {
                            //        invoiceHDR.LastUpdate = DateTime.Now;
                            //        invoiceHDR.UpdateBy = AbpSession.UserData.BizRegID;
                            //        invoiceHDR.Inuse = 1;
                            //        await invoiceHDRLogic.Update(invoiceHDR);
                            //    }

                            //    var invoiceDTL = invoiceDTLLogic.Retrieve(x => x.TransNo == siNumber);
                            //    if (invoiceDTL != null)
                            //    {
                            //        invoiceDTL.LastUpdate = DateTime.Now;
                            //        invoiceDTL.UpdateBy = AbpSession.UserData.BizRegID;
                            //        invoiceDTL.Inuse = 1;
                            //        await invoiceDTLLogic.Update(invoiceDTL);
                            //    }

                            //    var invoiceTAX = invoiceTAXLogic.Retrieve(x => x.TransNo == siNumber);
                            //    if (invoiceTAX != null)
                            //    {
                            //        invoiceTAX.Inuse = 1;
                            //        await invoiceTAXLogic.Update(invoiceTAX);
                            //    }

                            //    scope_inuse.Complete();
                            //}

                            scope_details.Complete();
                        }

                        scope.Complete();
                    }
                    #endregion
                }

                return new AjaxResponse(new { success = true, message = string.Empty });
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }
        #endregion

        #region Transaction History
        public async Task<AjaxResponse> GetTransactionHistory(GetListParameter input, string customerID, byte mode)
        {
            string contBizRegID = customerID;
            string contUserID = string.Empty;
            var invoiceHdrLogic = new TRANSHDR(_env, _connectionString);
            dynamic data = null;

            try
            {
                if (mode == 0)
                {
                    var refresh = false;
                    User user = await _userManager.GetUserByIdAsync(Convert.ToInt64(AbpSession.UserId));
                    #region Mode 0 & Dashboard
                    //var transHdrLogic = new TRANSHDR(_env, _connectionString);
                    //var transHdrRes = transHdrLogic.RetrieveAll(x => x.CustomerID == contBizRegID && x.Status != 2);
                    //if (transHdrRes != null)
                    //{
                    //    if (transHdrRes.Count() > 0)
                    //    {
                    //        foreach (var index in transHdrRes)
                    //        {
                    //            var tempTransNo = index.TransNo;
                    //            await UpdateTransactionQ(tempTransNo);
                    //        }
                    //    }
                    //}


                    var isApprover = await _userManager.IsInRoleAsync(user, "Approver");

                    if (isApprover == true)
                    {
                        contBizRegID = string.Empty;
                    }

                    //if (mode == 0)
                    // {
                    data = invoiceHdrLogic
                         .Select<DTO.Accounting.TRANSHDR,
                                 DTO.Accounting.TRANSDTL,
                                 DTO.Accounting.TRANSTENDER,
                                 DTO.Core.Entity.BIZENTITY,
                                 DTO.Core.General.CODEMASTER,
                                 DTO.Core.General.CODEMASTER,
                                 DTO.Core.General.CODEMASTER>
                         ((TH, TD, TT, BE, CMS, CMN, CMF) => new
                         {
                             TH.TransNo,
                             TH.TransDate,
                             FPXID = TT.ExternalID,
                             TH.Status,
                             TH.CreateDate,
                             Id = TH.InSvcID,
                             TH.ShiftCode,
                             TH.TransStatus,
                             TD.StkCode,
                             Company = BE.CompanyName,
                             Bank = CMN.CodeDesc,
                             Amount = TH.TransAmt,
                             FpxStatus = CMS.CodeDesc,
                             //PaidStatusQ = CMF.CodeDesc
                             PaidStatusQ = string.Format($"CASE WHEN CMF.CodeDesc = 'Fail Payment' THEN 'Fail' WHEN CMF.CodeDesc = 'Cancelled Payment' THEN 'Cancelled' WHEN CMF.CodeDesc = 'Paid' THEN 'Paid' Else CMF.CodeDesc END")
                         })
                         .InnerJoin<DTO.Accounting.TRANSDTL>(TH => TH.TransNo, TD => TD.TransNo)
                         .LeftJoin<DTO.Accounting.TRANSTENDER>(TH => TH.TransNo, TT => TT.TransNo)
                         //.LeftJoin<DTO.Core.Entity.BIZENTITY>(TH => TH.AcctNo, BE => BE.AcctNo)
                         .LeftJoin<DTO.Core.Entity.BIZENTITY>(TH => TH.BizRegID, BE => BE.BizRegID)
                         .LeftJoin<DTO.Core.General.CODEMASTER>(TH => TH.TransStatus, CMS => CMS.Code, CMS => CMS.CodeType == "FPX")
                         .LeftJoin<DTO.Core.General.CODEMASTER>(TH => TH.Status, CMF => CMF.Code, CMF => CMF.CodeType == "PAY")
                         .LeftJoin<DTO.Accounting.TRANSTENDER, DTO.Core.General.CODEMASTER>(TT => TT.BankCode, CMN => CMN.Code, CMN => CMN.CodeType == "BNB")
                         .Where<DTO.Accounting.TRANSHDR,
                                DTO.Accounting.TRANSDTL,
                                DTO.Core.Entity.BIZENTITY,
                                DTO.Core.General.CODEMASTER,
                                DTO.Core.General.CODEMASTER,
                                DTO.Core.General.CODEMASTER>
                         ((TH, TD, BE, CMS, CMN, CMF) =>
                         TH.Flag == 1 &&
                         (!string.IsNullOrEmpty(contBizRegID) ? TH.BizRegID == contBizRegID : false) &&
                         ((!string.IsNullOrEmpty(input.Filter) ? TH.TransNo.Contains(input.Filter) : false) ||
                          (!string.IsNullOrEmpty(input.Filter) ? TH.BillNo.Contains(input.Filter) : false) ||
                          (!string.IsNullOrEmpty(input.Filter) ? CMN.CodeDesc.Contains(input.Filter) : false) ||
                          (!string.IsNullOrEmpty(input.Filter) ? CMS.CodeDesc.Contains(input.Filter) : false) ||
                          (!string.IsNullOrEmpty(input.Filter) ? CMF.CodeDesc.Contains(input.Filter) : false)))
                         .GroupBy<DTO.Accounting.TRANSHDR,
                                 DTO.Accounting.TRANSDTL,
                                 DTO.Accounting.TRANSTENDER,
                                 DTO.Core.Entity.BIZENTITY,
                                 DTO.Core.General.CODEMASTER,
                                 DTO.Core.General.CODEMASTER,
                                 DTO.Core.General.CODEMASTER>
                         ((TH, TD, TT, BE, CMS, CMN, CMF) => new
                         {
                             TransNo = TH.TransNo,
                             TransDate = TH.TransDate,
                             FPXID = TT.ExternalID,
                             TH.Status,
                             TH.CreateDate,
                             Id = TH.InSvcID,
                             TH.ShiftCode,
                             TH.TransStatus,
                             TD.StkCode,
                             TT.BankCode,
                             Company = BE.CompanyName,
                             Bank = CMN.CodeDesc,
                             Amount = TH.TransAmt,
                             FpxStatus = CMS.CodeDesc,
                             CMF.CodeDesc
                         })
                         .OrderByDesc<DTO.Accounting.TRANSHDR>(TH => TH.CreateDate)
                         .Fetch(input.SkipCount > 0 ? input.SkipCount : 0, input.MaxResultCount > 0 ? input.MaxResultCount : 0)
                      .ExecuteList<Map.MapListTransactionHistory_A>();

                    //.ToString();
                    //}
                    //else
                    //{
                    //data = invoiceHdrLogic.Top(10)
                    //    .Select<DTO.Accounting.TRANSHDR,
                    //            DTO.Accounting.TRANSDTL,
                    //            DTO.Accounting.TRANSTENDER,
                    //            DTO.Core.Entity.BIZENTITY,
                    //            DTO.Core.General.CODEMASTER,
                    //            DTO.Core.General.CODEMASTER,
                    //            DTO.Core.General.CODEMASTER>
                    //    ((TH, TD, TT, BE, CMS, CMN, CMF) => new
                    //    {
                    //        TransNo = TH.TransNo,
                    //        TransDate = TH.TransDate,
                    //        FPXID = TH.BillNo,
                    //        TH.Status,
                    //        TH.CreateDate,
                    //        Id = TH.InSvcID,
                    //        TH.ShiftCode,
                    //        TH.TransStatus,
                    //        TD.StkCode,
                    //        Company = BE.CompanyName,
                    //        Bank = CMN.CodeDesc,
                    //        Amount = TH.TransAmt,
                    //        FpxStatus = CMS.CodeDesc,
                    //        PaidStatus = CMF.CodeDesc
                    //    })
                    //    .InnerJoin<DTO.Accounting.TRANSDTL>(TH => TH.TransNo, TD => TD.TransNo)
                    //    .LeftJoin<DTO.Accounting.TRANSTENDER>(TH => TH.TransNo, TT => TT.TransNo)
                    //    .LeftJoin<DTO.Core.Entity.BIZENTITY>(TH => TH.AcctNo, BE => BE.AcctNo)
                    //    .LeftJoin<DTO.Core.General.CODEMASTER>(TH => TH.TransStatus, CMS => CMS.Code, CMS => CMS.CodeType == "FPX")
                    //    .LeftJoin<DTO.Core.General.CODEMASTER>(TH => TH.Status, CMF => CMF.Code, CMF => CMF.CodeType == "PAY")
                    //    .LeftJoin<DTO.Accounting.TRANSTENDER, DTO.Core.General.CODEMASTER>(TT => TT.BankCode, CMN => CMN.Code, CMN => CMN.CodeType == "BNB")
                    //    .Where<DTO.Accounting.TRANSHDR,
                    //           DTO.Accounting.TRANSDTL,
                    //           DTO.Core.Entity.BIZENTITY,
                    //           DTO.Core.General.CODEMASTER,
                    //           DTO.Core.General.CODEMASTER,
                    //           DTO.Core.General.CODEMASTER>
                    //    ((TH, TD, BE, CMS, CMN, CMF) =>
                    //    TH.Flag == 1 && TH.Status == 1 &&
                    //    (!string.IsNullOrEmpty(contBizRegID) ? TH.BizRegID == contBizRegID : false) &&
                    //    ((!string.IsNullOrEmpty(input.Filter) ? TH.TransNo.Contains(input.Filter) : false) ||
                    //     (!string.IsNullOrEmpty(input.Filter) ? TH.BillNo.Contains(input.Filter) : false) ||
                    //     (!string.IsNullOrEmpty(input.Filter) ? CMN.CodeDesc.Contains(input.Filter) : false) ||
                    //     (!string.IsNullOrEmpty(input.Filter) ? CMS.CodeDesc.Contains(input.Filter) : false) ||
                    //     (!string.IsNullOrEmpty(input.Filter) ? CMF.CodeDesc.Contains(input.Filter) : false)))
                    //    .GroupBy<DTO.Accounting.TRANSHDR,
                    //            DTO.Accounting.TRANSDTL,
                    //            DTO.Accounting.TRANSTENDER,
                    //            DTO.Core.Entity.BIZENTITY,
                    //            DTO.Core.General.CODEMASTER,
                    //            DTO.Core.General.CODEMASTER,
                    //            DTO.Core.General.CODEMASTER>
                    //    ((TH, TD, TT, BE, CMS, CMN, CMF) => new
                    //    {
                    //        TransNo = TH.TransNo,
                    //        TransDate = TH.TransDate,
                    //        FPXID = TH.BillNo,
                    //        TH.Status,
                    //        TH.CreateDate,
                    //        Id = TH.InSvcID,
                    //        TH.ShiftCode,
                    //        TH.TransStatus,
                    //        TD.StkCode,
                    //        TT.BankCode,
                    //        Company = BE.CompanyName,
                    //        Bank = CMN.CodeDesc,
                    //        Amount = TH.TransAmt,
                    //        FpxStatus = CMS.CodeDesc,
                    //        PaidStatus = CMF.CodeDesc
                    //    })
                    //    .OrderByDesc<DTO.Accounting.TRANSHDR>(TH => TH.CreateDate)
                    //    //.Fetch(input.SkipCount, input.MaxResultCount)
                    //    .ExecuteList<Map.MapListTransactionHistory_A>();
                    ////.ToString();
                    //}
                    #endregion
                }
                else if (mode == 1)
                {
                    User user = await _userManager.GetUserByIdAsync(Convert.ToInt64(AbpSession.UserId));
                    var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                    var isLgmAdmin = await _userManager.IsInRoleAsync(user, "LGM Admin");

                    if (isAdmin == true || isLgmAdmin == true)
                    {
                        contBizRegID = string.Empty;
                    }

                    #region Mode 1
                    data = invoiceHdrLogic
                        .Select<DTO.Accounting.TRANSHDR,
                                DTO.Accounting.TRANSDTL,
                                DTO.Core.Entity.BIZENTITY,
                                DTO.Core.General.CODEMASTER,
                                DTO.Core.General.CODEMASTER,
                                DTO.Core.General.CODEMASTER>
                        ((TH, TD, BE, CMS, CMN, CMF) => new
                        {
                            BillNo = TH.BillNo,
                            TransDate = TH.TransDate,
                            TH.Status,
                            TH.CreateDate,
                            Id = TH.InSvcID,
                            TH.ShiftCode,
                            TH.TransStatus,
                            TD.StkCode,
                            Company = BE.CompanyName,
                            TransNo = TH.TransNo,
                            Type = CMN.CodeDesc,
                            Amount = TH.TransAmt,
                            FpxStatus = CMS.CodeDesc,
                            PaidStatus = CMF.CodeDesc
                        })
                        .InnerJoin<DTO.Accounting.TRANSDTL>(TH => TH.TransNo, TD => TD.TransNo)
                        .LeftJoin<DTO.Core.Entity.BIZENTITY>(TH => TH.BizRegID, BE => BE.BizRegID)
                        .LeftJoin<DTO.Core.General.CODEMASTER>(TH => TH.TransStatus, CMS => CMS.Code, CMS => CMS.CodeType == "FPX")
                        .LeftJoin<DTO.Core.General.CODEMASTER>(TH => TH.Status, CMF => CMF.Code, CMF => CMF.CodeType == "PAY")
                        .LeftJoin<DTO.Accounting.TRANSDTL, DTO.Core.General.CODEMASTER>(TD => TD.StkType, CMN => CMN.Code, CMN => CMN.CodeType == "INT")
                        .Where<DTO.Accounting.TRANSHDR,
                               DTO.Accounting.TRANSDTL,
                               DTO.Core.Entity.BIZENTITY,
                               DTO.Core.General.CODEMASTER,
                               DTO.Core.General.CODEMASTER>
                        ((TH, TD, BE, CMS, CMN) =>
                        TH.Flag == 1 &&
                        (!string.IsNullOrEmpty(contBizRegID) ? TH.BizRegID == contBizRegID : false) ||
                        (!string.IsNullOrEmpty(contBizRegID) ? TH.CreateBy == contBizRegID : false))
                        .GroupBy<DTO.Accounting.TRANSHDR,
                                 DTO.Accounting.TRANSDTL,
                                 DTO.Core.Entity.BIZENTITY,
                                 DTO.Core.General.CODEMASTER,
                                 DTO.Core.General.CODEMASTER,
                                 DTO.Core.General.CODEMASTER>
                        ((TH, TD, BE, CMS, CMN, CMF) => new
                        {
                            BillNo = TH.BillNo,
                            TransDate = TH.TransDate,
                            TH.Status,
                            TH.CreateDate,
                            Id = TH.InSvcID,
                            TH.ShiftCode,
                            TH.TransStatus,
                            TD.StkCode,
                            Company = BE.CompanyName,
                            TransNo = TH.TransNo,
                            Type = CMN.CodeDesc,
                            Amount = TH.TransAmt,
                            FpxStatus = CMS.CodeDesc,
                            PaidStatus = CMF.CodeDesc
                        })
                        .OrderByDesc<DTO.Accounting.TRANSHDR>(TH => TH.CreateDate)
                        .Fetch(input.SkipCount > 0 ? input.SkipCount : 0, input.MaxResultCount > 0 ? input.MaxResultCount : 0)
                        .ExecuteList<Map.MapListTransactionHistory>();
                    #endregion
                }

                return new AjaxResponse(data);
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }

        public async Task<AjaxResponse> GetPaymentHistoryDetails(string transNo)
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
                            DTO.Core.General.CODEMASTER>
                    ((TH, TD, BE, CMS, CMN) => new
                    {
                        TH.BillNo,
                        Date = TH.TransDate,
                        Attn = TH.CashierID,
                        CustomerCode = TH.BizRegID,
                        Company = BE.CompanyName,
                        BE.Address1,
                        BE.Address2,
                        BE.Address3,
                        BE.Address4,
                        CashSaleAddr = TH.TransRemark,
                        FpxStatus = CMS.CodeDesc,
                        PaidStatus = CMN.CodeDesc,
                        TH.CustPrivilege,
                        TH.SpDiscRemark,
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
                        TaxCatCode = TH.SpDiscReasonCode,
                        TH.TransReasonCode,
                        TH.TransChgAmt,
                        TH.TransSubTotal
                    })
                    .InnerJoin<DTO.Accounting.TRANSDTL>(TH => TH.TransNo, TD => TD.TransNo)
                    .LeftJoin<DTO.Core.Entity.BIZENTITY>(TH => TH.BizRegID, BE => BE.BizRegID)
                    .LeftJoin<DTO.Core.General.CODEMASTER>(TH => TH.TransStatus, CMS => CMS.Code, CMS => CMS.CodeType == "FPX")
                    .LeftJoin<DTO.Core.General.CODEMASTER>(TH => TH.Status, CMN => CMN.Code, CMN => CMN.CodeType == "PAY")
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
                            DTO.Core.General.CODEMASTER>
                    ((TH, TD, BE, CMS, CMN) => new
                    {
                        TH.BillNo,
                        Date = TH.TransDate,
                        Attn = TH.CashierID,
                        CustomerCode = TH.BizRegID,
                        Company = BE.CompanyName,
                        BE.Address1,
                        BE.Address2,
                        BE.Address3,
                        BE.Address4,
                        CashSaleAddr = TH.TransRemark,
                        FpxStatus = CMS.CodeDesc,
                        PaidStatus = CMN.CodeDesc,
                        TH.CustPrivilege,
                        TH.SpDiscRemark,
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
                        TaxCatCode = TH.SpDiscReasonCode,
                        TH.TransReasonCode,
                        TH.TransChgAmt,
                        TH.TransSubTotal
                    })
                    .Execute().FirstOrDefault();

                var details = invoiceDtlLogic
                    .Select<DTO.Accounting.TRANSDTL,
                            DTO.Core.General.CODEMASTER>
                    ((TD, CMS) => new
                    {
                        TD.TransSeq,
                        ItemCode = TD.ItemCode,
                        ItemDesc = TD.Remark,
                        Qty = TD.Qty,
                        Tax = TD.BaseRate,
                        Rounding = TD.CoRate1H,
                        UnitPrice = TD.UnitCost,
                        SubTotal = TD.TolAmt,
                        Type = CMS.CodeDesc,
                        TD.ItemType,
                        TD.StkType,
                        CreditVote = TD.ExCode1,
                        ProjectCode = TD.ExCode2
                    })
                    .LeftJoin<DTO.Core.General.CODEMASTER>(TD => TD.StkType, CMS => CMS.Code, CMS => CMS.CodeType == "INT")
                    .Where<DTO.Accounting.TRANSDTL,
                           DTO.Core.Entity.BIZENTITY>
                    ((TD, CMS) =>
                           TD.TransNo == transNo)
                   .GroupBy<DTO.Accounting.TRANSDTL,
                            DTO.Core.General.CODEMASTER>
                    ((TD, CMS) => new
                    {
                        TD.TransSeq,
                        ItemCode = TD.ItemCode,
                        ItemDesc = TD.Remark,
                        Qty = TD.Qty,
                        Tax = TD.BaseRate,
                        Rounding = TD.CoRate1H,
                        UnitPrice = TD.UnitCost,
                        SubTotal = TD.TolAmt,
                        Type = CMS.CodeDesc,
                        TD.ItemType,
                        TD.StkType,
                        CreditVote = TD.ExCode1,
                        ProjectCode = TD.ExCode2
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
                        TD.TenderDesc
                    })
                    .Execute().FirstOrDefault();

                return new AjaxResponse(new { transHdr = header, transDtl = details, transTender = tender });
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }

        public async Task<dynamic> CheckTransaction(string bizregID, long? userID)
        {
            var transHdrLogic = new TRANSHDR(_env, _connectionString);

            try
            {
                var transHdrRes = transHdrLogic.RetrieveAll(x => x.CustomerID == bizregID && (x.Status == 0 || x.Status == 1) && x.Flag == 1);
                bool refresh = false;

                if (transHdrRes != null)
                {
                    if (transHdrRes.Count() > 0)
                    {
                        foreach (var index in transHdrRes)
                        {
                            var tempTransNo = index.TransNo;
                            await UpdateTransactionQ(tempTransNo);

                            if (refresh == false)
                            {
                                var transNo = index.TransNo;
                                var isiSatu = transHdrLogic.Retrieve(x => x.TransNo == transNo && x.Flag == 1);
                                if (isiSatu.Status != index.Status)
                                {
                                    refresh = true;
                                }
                            }
                        }
                    }

                    if (refresh)
                    {
                        await _realtimeService.Run(RealtimeType.All, RealTimeMethod.app_fpx_payment_refresh, userIds: new List<long>() { userID.Value });
                    }
                }

                return refresh;
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }
        #endregion

        #region FPX
        public async Task<AjaxResponse> GetFPXBankList(System.String fpx_msgToken)
        {
            string result = string.Empty;
            string posting_data = string.Empty;
            string fpx_msgtype = "BE";
            string fpx_sellerExId = string.Empty;
            string fpx_version = _appConfiguration["SUNSystem:FPXVersion"];
            string path = string.Empty;
            string fpx_checkSum = string.Empty;
            string checkSum_String = string.Empty;
            string bankType = string.Empty;
            string URLBank = string.Empty;
            List<MapFPXBankList> resultMap = new List<MapFPXBankList>();

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
                checkSum_String = RSASign.RSASignValue(fpx_checkSum, path);
                posting_data = "fpxmsgToken=" + fpx_msgToken +
                    "&fpx_msgToken=" + fpx_msgToken +
                    "&fpx_msgType=" + fpx_msgtype +
                    "&fpx_sellerExId=" + fpx_sellerExId +
                    "&fpx_version=" + fpx_version +
                    "&fpx_checkSum=" + checkSum_String;

                byte[] _byteVersion = Encoding.ASCII.GetBytes(string.Concat("content=", posting_data));

                var sunSystem = new SUNSystemConnection();
                result = sunSystem.POSTWebRequest(URLBank, posting_data, _byteVersion);
                if (string.IsNullOrEmpty(result))
                {
                    //return new AjaxResponse(resultMap);
                    return new AjaxResponse(new ErrorInfo(result));
                }

                if (result.ToUpper().Contains("ERROR"))
                {
                    //return new AjaxResponse(resultMap);
                    return new AjaxResponse(new ErrorInfo(result));
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

                    MapFPXBankList cont = new MapFPXBankList
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
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }

        public async Task<AjaxResponse> SaveBankListToFile()
        {
            try
            {
                var getPersonalBank = await GetFPXBankList("01");
                var getCorporateBank = await GetFPXBankList("02");
                var lastCheckDate = DateTime.ParseExact(DateTime.Now.ToString("yyyyMMddHHmmss"), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);

                var personalBankList = (dynamic)getPersonalBank.Result; // resPersonalBank;
                var corporateBankList = (dynamic)getCorporateBank.Result; // resCorporateBank;

                var json = JsonConvert.SerializeObject(new
                {
                    PersonalBank = personalBankList,
                    CorporateBank = corporateBankList,
                    LastCheck = lastCheckDate
                });
                string path = @_appConfiguration["SUNSystem:AssetsPath"] + "banklist.json";

                using (TextWriter tw = new StreamWriter(path))
                {
                    tw.WriteLine(json);
                    return new AjaxResponse(new { success = true });
                }
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }

        public async Task<AjaxResponse> ValidateFPXStatus(System.String refInfo)
        {
            MapValidateStatus result = new MapValidateStatus();
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
                string fpx_checkSum = RSASign.RSASignValue(refInfo, path);

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

                var sunSystem = new SUNSystemConnection();
                result.CheckSum = sunSystem.POSTWebRequest(url, posting_data, _byteVersion);
                if (string.IsNullOrEmpty(result.CheckSum))
                {
                    result.Message = "No return from FPX";
                    result.DebitAuthCode = "99";
                    return new AjaxResponse(new ErrorInfo(result.Message));
                }

                if (result.CheckSum.ToUpper().Contains("ERROR"))
                {
                    result.Message = result.CheckSum;
                    result.DebitAuthCode = "99";
                    return new AjaxResponse(new ErrorInfo(result.Message));
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
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }

        public async Task<AjaxResponse> GetFPXCheckSum(MapFPXBody fpxBody)
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
                checkSum_String = RSASign.RSASignValue(fpx_checkSum, path);

                return new AjaxResponse(new { checksum = checkSum_String, checksumData = fpx_checkSum });
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }

        public async Task<AjaxResponse> ScheduledCheckFailedFPX()
        {
            try
            {
                var CRONValue = _appConfiguration["SUNSystem:FPXCRONValue"];
                RecurringJob.AddOrUpdate(() => CheckFailedFPX(), CRONValue, TimeZoneInfo.Local);
                return new AjaxResponse(new { message = "Scheduling Check Failed FPX" });
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }

        public async Task<AjaxResponse> CheckFailedFPX()
        {
            try
            {
                TRANSTENDER transTenderLogic = new TRANSTENDER(_env, _connectionString);
                List<DTO.Accounting.TRANSTENDER> failedTransList = new List<DTO.Accounting.TRANSTENDER>();
                AjaxResponse returnMessage = new AjaxResponse();

                failedTransList = transTenderLogic.RetrieveAll(x => x.TenderID == "997" && x.RespCode == "99" && x.TransStatus == 0);
                if (failedTransList != null)
                {
                    foreach (var index in failedTransList)
                    {
                        var resValidate = await ValidateFPXStatus(index.RefToken);
                        if (resValidate.Success)
                        {
                            var resMap = (MapValidateStatus)resValidate.Result;
                            if (resMap != null)
                            {
                                if (resMap.DebitAuthCode != "99")
                                {
                                    if (_appConfiguration["InvoiceConfig:PaymentType"] == "CWMS")
                                    {
                                        BackgroundJob.Enqueue(() => UpdateTransactionQ(index.TransNo));
                                    }
                                    else
                                    {
                                        BackgroundJob.Enqueue(() => UpdateTransaction(index.TransNo));
                                    }
                                    returnMessage = new AjaxResponse(new { message = "Scheduling Update Transaction" });
                                }
                                else if (resMap.DebitAuthCode == "99")
                                {
                                    BackgroundJob.Enqueue(() => ValidateFPXStatus(index.RefToken));
                                    returnMessage = new AjaxResponse(new { message = "Revalidate FPX Status" });
                                }
                            }
                        }
                    }
                }
                return returnMessage;
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }
        #endregion

        #region eBilling
        //public async Task<AjaxResponse> eBillingUpdatePayment(string billNo, string customerNo, string email, string amount, string bizRegID, string bizLocID)
        //{
        //    try
        //    {
        //        bool res = false;

        //        Map.Envelope requestEnvelope = new Map.Envelope();
        //        Map.Envelope responseEnvelope = new Map.Envelope();
        //        SUNSystemConnection sunSystem = new SUNSystemConnection();
        //        List<Map.Data> contResDataAPI;

        //        var updatePaymentData = new POSTUpdatePayment
        //        {
        //            Email = email,
        //            BillID = billNo,
        //            Amount = amount,
        //            Reference = string.Empty
        //        };

        //        var update = await UpdatePayment(updatePaymentData);
        //        if (update != null)
        //        {
        //            if (update.Success)
        //            {
        //                try
        //                {
        //                    requestEnvelope = new Map.Envelope
        //                    {
        //                        Body = new Map.Body
        //                        {
        //                            GetAllOutStandingBillByCustomerCode = new Map.GetAllOutStandingBillByCustomerCode
        //                            {
        //                                AuthenticationDTO = new Map.AuthenticationDTO
        //                                {
        //                                    Username = _appConfiguration["SUNSystem:Username"],
        //                                    Password = _appConfiguration["SUNSystem:Password"]
        //                                },
        //                                Config = new Map.Config
        //                                {
        //                                    CustomerCode = customerNo,
        //                                    Limit = "1000",
        //                                    Offset = "0"
        //                                }
        //                            }
        //                        }
        //                    };

        //                    responseEnvelope = await sunSystem.XMLSender("getAllOutStandingBillByCustomerCode", requestEnvelope, "getAllOutStandingBillByCustomerCode", "getAllOutStandingBillByCustomerCode");

        //                    contResDataAPI = responseEnvelope.Body.GetAllOutStandingBillByCustomerCodeResponse.ReturnGetAllOutStandingBillByCustomerCodeResponse.Data;

        //                    if (contResDataAPI != null && contResDataAPI.Count > 0)
        //                    {
        //                        var resList = contResDataAPI.Select(x => (x.SiNumber == billNo && x.Type.Contains("Credit Bill") || x.BillNo == billNo && x.Type.Contains("Proforma Bill")) && x.Status != "Accepted").ToList();
        //                        if (resList != null)
        //                        {
        //                            foreach (var idx in resList)
        //                            {
        //                                if (idx != false)
        //                                {
        //                                    res = true;
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            return new AjaxResponse(new ErrorInfo("SiNumber Not Found"));
        //                        }
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    throw ex;
        //                }
        //            }
        //        }

        //        return new AjaxResponse(res);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(nameof(AccountingManager), ex);
        //        throw ex;
        //    }
        //}

        public async Task<AjaxResponse> UpdatePayment(POSTUpdatePayment data)
        {
            Map.Envelope requestEnvelope = new Map.Envelope();
            Map.Envelope responseEnvelope = new Map.Envelope();

            try
            {
                requestEnvelope = new Map.Envelope
                {
                    Body = new Map.Body
                    {
                        UpdatePayment = new UpdatePayment
                        {
                            AuthenticationDTO = new Map.AuthenticationDTO
                            {
                                Username = _appConfiguration["SUNSystem:Username"],
                                Password = _appConfiguration["SUNSystem:Password"]
                            },
                            Email = data.Email,
                            PaymentDTO = new PaymentDTO
                            {
                                Amount = data.Amount,
                                BillId = data.BillID,
                                Reference = data.Reference
                            }
                        }
                    }
                };

                var sunSystem = new SUNSystemConnection();
                responseEnvelope = await sunSystem.XMLSender("updatePayment", requestEnvelope, "updatePayment", "updatePayment");

                bool result = false;
                if (responseEnvelope.Body.UpdatePaymentResponse != null && responseEnvelope.Body.UpdatePaymentResponse.ReturnString != null && responseEnvelope.Body.UpdatePaymentResponse.ReturnString.Contains("Success"))
                {
                    result = true;
                }
                else if (responseEnvelope.Body.Fault != null && responseEnvelope.Body.Fault != null && responseEnvelope.Body.Fault.FaultString.Contains("Balance is paid"))
                {
                    result = true;
                }

                return new AjaxResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }
        #endregion

        public async Task<AjaxResponse> GetResponseDetails(string transNo, bool creditCard, bool offPay)
        {
            try
            {
                var transTenderLogic = new TRANSTENDER(_env, _connectionString);
                var transFpxLogic = new TRANSFPX(_env, _connectionString);
                var transHdrLogic = new TRANSHDR(_env, _connectionString);
                dynamic data;

                if (creditCard == true)
                {
                    data = transTenderLogic
                        .Select<DTO.Accounting.TRANSTENDER,
                                DTO.Core.Entity.BIZENTITY>
                        ((TT, BE) => new
                        {
                            TT.TransNo,
                            TT.BillNo,
                            TT.TenderID,
                            TT.TransDate,
                            TT.TransTime,
                            TT.TenderAmt,
                            TT.MerchantID,
                            TT.RefToken,
                            TT.RefRemark,
                            TT.BankCode,
                            TT.RespCode,
                            BE.AcctNo,
                            BE.CompanyName,
                            BE.Email,
                        })
                        .LeftJoin<DTO.Core.Entity.BIZENTITY>(TT => TT.BizRegID, BE => BE.BizRegID)
                        .Where<DTO.Accounting.TRANSTENDER>
                        ((TT) =>
                            TT.TransNo == transNo)
                        .GroupBy<DTO.Accounting.TRANSTENDER,
                                 DTO.Core.Entity.BIZENTITY>
                        ((TT, BE) => new
                        {
                            TT.TransNo,
                            TT.BillNo,
                            TT.TenderID,
                            TT.TransDate,
                            TT.TransTime,
                            TT.TenderAmt,
                            TT.MerchantID,
                            TT.RefToken,
                            TT.RefRemark,
                            TT.BankCode,
                            TT.RespCode,
                            BE.AcctNo,
                            BE.CompanyName,
                            BE.Email,
                        })
                        .Execute().FirstOrDefault();
                }
                else if (offPay == true)
                {
                    data = transHdrLogic
                        .Select<DTO.Accounting.TRANSHDR,
                                DTO.Core.Entity.BIZENTITY>
                        ((TH, BE) => new
                        {
                            TH.TransNo,
                            TH.BillNo,
                            TH.TransDate,
                            TH.TransStartTime,
                            TH.TransAmt,
                            BE.AcctNo,
                            BE.CompanyName,
                            BE.Email,
                        })
                        .LeftJoin<DTO.Core.Entity.BIZENTITY>(TH => TH.BizRegID, BE => BE.BizRegID)
                        .Where<DTO.Accounting.TRANSHDR>
                        ((TH) =>
                            TH.TransNo == transNo)
                        .GroupBy<DTO.Accounting.TRANSHDR,
                                 DTO.Core.Entity.BIZENTITY>
                        ((TH, BE) => new
                        {
                            TH.TransNo,
                            TH.BillNo,
                            TH.TransDate,
                            TH.TransStartTime,
                            TH.TransAmt,
                            BE.AcctNo,
                            BE.CompanyName,
                            BE.Email,
                        })
                        .Execute().FirstOrDefault();
                }
                else
                {
                    data = transFpxLogic
                        .Select<DTO.Accounting.TRANSFPX,
                                DTO.Accounting.TRANSTENDER,
                                DTO.Core.Entity.BIZENTITY,
                                DTO.Core.General.CODEMASTER,
                                DTO.Core.General.CODEMASTER>((TF, TT, BE, CM, CB) => new
                                {
                                    TF.TransNo,
                                    TF.TransDate,
                                    TT.TransTime,
                                    TF.TenderID,
                                    TT.ExternalID,
                                    TT.BankCode,
                                    BE.AcctNo,
                                    BE.CompanyName,
                                    BE.Email,
                                    BankName = CB.CodeDesc,
                                    TT.RespCode,
                                    Status = CM.CodeDesc,
                                    TF.TenderAmt
                                })
                        .InnerJoin<DTO.Accounting.TRANSTENDER>(TF => TF.TransNo, TT => TT.TransNo)
                        .LeftJoin<DTO.Core.Entity.BIZENTITY>(TF => TF.BizRegID, BE => BE.BizRegID)
                        .LeftJoin<DTO.Core.General.CODEMASTER>(TF => TF.TenderID, CM => CM.Code, CM => CM.CodeType == "FPX")
                        .LeftJoin<DTO.Accounting.TRANSTENDER, DTO.Core.General.CODEMASTER>(TT => TT.BankCode, CB => CB.Code, CB => CB.CodeType == "BNB" || CB.CodeType == "BNC")
                        .Where<DTO.Accounting.TRANSFPX>((TF) =>
                            TF.TransNo == transNo
                        )
                        .GroupBy<DTO.Accounting.TRANSFPX,
                                DTO.Accounting.TRANSTENDER,
                                DTO.Core.Entity.BIZENTITY,
                                DTO.Core.General.CODEMASTER,
                                DTO.Core.General.CODEMASTER>((TF, TT, BE, CM, CB) => new
                                {
                                    TF.TransNo,
                                    TF.TransDate,
                                    TT.TransTime,
                                    TF.TenderID,
                                    TT.ExternalID,
                                    TT.BankCode,
                                    BE.AcctNo,
                                    BE.CompanyName,
                                    BE.Email,
                                    BankName = CB.CodeDesc,
                                    TT.RespCode,
                                    Status = CM.CodeDesc,
                                    TF.TenderAmt
                                })
                        .OrderByDesc<DTO.Accounting.TRANSFPX>(TF => TF.TransDate)
                        .Fetch(0, 1)
                        .Execute().FirstOrDefault();
                }

                return new AjaxResponse(data);
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }

        public async Task<AjaxResponse> FPXIndirectResponse(FPXResponse data)
        {
            try
            {
                string tempBizRegID;
                string tempBizLocID;

                var transFPXLogic = new TRANSFPX(_env, _connectionString);
                var transHDRLogic = new TRANSHDR(_env, _connectionString);
                var transDTLLogic = new TRANSDTL(_env, _connectionString);
                var transTenderLogic = new TRANSTENDER(_env, _connectionString);

                data.fpx_checkSumString = data.fpx_buyerBankBranch + "|" + data.fpx_buyerBankId + "|" + data.fpx_buyerIban + "|" + data.fpx_buyerId + "|" + data.fpx_buyerName + "|";
                data.fpx_checkSumString += data.fpx_creditAuthCode + "|" + data.fpx_creditAuthNo + "|" + data.fpx_debitAuthCode + "|" + data.fpx_debitAuthNo + "|" + data.fpx_fpxTxnId + "|";
                data.fpx_checkSumString += data.fpx_fpxTxnTime + "|" + data.fpx_makerName + "|" + data.fpx_msgToken + "|" + data.fpx_msgType + "|" + data.fpx_sellerExId + "|" + data.fpx_sellerExOrderNo + "|";
                data.fpx_checkSumString += data.fpx_sellerId + "|" + data.fpx_sellerOrderNo + "|" + data.fpx_sellerTxnTime + "|" + data.fpx_txnAmount + "|" + data.fpx_txnCurrency;
                data.fpx_checkSumString = data.fpx_checkSumString.Trim();

                DateTime trxDate = data.fpx_fpxTxnTime != null ? DateTime.ParseExact(data.fpx_fpxTxnTime.Substring(0, 8), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture) : DateTime.ParseExact(DateTime.Now.ToString("yyyyMMdd"), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                String trxTime = data.fpx_fpxTxnTime != null ? data.fpx_fpxTxnTime.Substring(8, 4) : DateTime.Now.ToString("HHmm");

                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var tempTransNo = data.fpx_sellerOrderNo;
                    var transData = transHDRLogic.Retrieve(x => x.TransNo == tempTransNo);
                    if (transData != null)
                    {
                        tempBizRegID = transData.BizRegID;
                        tempBizLocID = transData.BizLocID;

                        var checkTransFPX = transFPXLogic.Retrieve(x => x.TransNo == tempTransNo);
                        if (checkTransFPX == null)
                        {
                            var dataTransFPX = new DTO.Accounting.TRANSFPX
                            {
                                BizRegID = tempBizRegID,
                                BizLocID = tempBizLocID,
                                TermID = 1,
                                TransNo = data.fpx_sellerOrderNo,
                                TransSeq = 1,
                                TransDate = trxDate,
                                TransTime = trxTime, //data.fpx_fpxTxnTime.Substring(8, 4),
                                TenderID = data.fpx_debitAuthCode,
                                TenderAmt = Convert.ToDecimal(data.fpx_txnAmount),
                                RefInfo = data.fpx_checkSum,
                                CCrefInfo = data.fpx_checkSumString,
                                CustName = data.fpx_buyerName,
                                CreateDate = DateTime.Now,
                                LastUpdate = DateTime.Now,
                                SyncLastUpd = DateTime.Now,
                                Active = 1,
                                Posted = 1,
                                Flag = 1,
                                Inuse = 0,
                                IsHost = 0,
                            };

                            await transFPXLogic.Create(dataTransFPX);
                        }
                        else
                        {
                            if (_appConfiguration["InvoiceConfig:PaymentType"] != "CWMS")
                            {
                                return new AjaxResponse(new ErrorInfo("FPX Data Already Exist"));
                            }
                            
                        }

                        Byte? paidStatus;
                        Byte? isApproved;
                        if (data.fpx_debitAuthCode == "00")
                        {
                            paidStatus = 2;
                            isApproved = 1;
                        }
                        else if (data.fpx_debitAuthCode == "09" || data.fpx_debitAuthCode == "99")
                        {
                            paidStatus = 1;
                            isApproved = 0;
                        }
                        else
                        {
                            paidStatus = 3;
                            isApproved = 0;
                        }

                        var trxNo = data.fpx_sellerOrderNo;

                        var dataTransHDR = transHDRLogic.Retrieve(x => x.TransNo == trxNo);
                        if (dataTransHDR != null)
                        {
                            //dataTransHDR.TransReasonCode = data.fpx_fpxTxnId;
                            dataTransHDR.LastUpdate = DateTime.Now;
                            dataTransHDR.Status = paidStatus;
                            dataTransHDR.ShiftCode = data.fpx_debitAuthCode;
                            dataTransHDR.TransStatus = data.fpx_debitAuthCode;
                            await transHDRLogic.Update(dataTransHDR);
                        }

                        var dataTransTender = transTenderLogic.Retrieve(x => x.TransNo == trxNo);
                        if (dataTransTender != null)
                        {
                            dataTransTender.IsApproved = isApproved;
                            dataTransTender.TransStatus = paidStatus;
                            dataTransTender.ExternalID = data.fpx_fpxTxnId;
                            dataTransTender.AuthCode = data.fpx_checkSum;
                            dataTransTender.RespCode = data.fpx_debitAuthCode;
                            dataTransTender.ApprovedDate = DateTime.Now;
                            dataTransTender.LastUpdate = DateTime.Now;
                            dataTransTender.SyncLastUpd = DateTime.Now;
                            await transTenderLogic.Update(dataTransTender);
                        }

                        var dataTransDTL = transDTLLogic.RetrieveAll(x => x.TransNo.Contains(trxNo));
                        if (dataTransDTL != null && dataTransDTL.Count() > 0)
                        {
                            foreach (var index in dataTransDTL)
                            {
                                index.LastUpdate = DateTime.Now;
                                index.Status = paidStatus;
                                await transDTLLogic.Update(index);
                            }
                        }
                    }
                    else
                    {
                        return new AjaxResponse(new ErrorInfo("Transaction Not Found"));
                    }

                    if (_appConfiguration["InvoiceConfig:PaymentType"] == "CWMS")
                    {

                        var sysPrefbLogic = new Base.Core.General.Repo.SYSPREFB(_env, _connectionString);
                        var mode = 1;
                        var checkMode = sysPrefbLogic.Retrieve(x => x.SYSKey == "CHKFAILFPXUSENEWSVC" && x.BranchID == "PLX");
                        if (checkMode != null)
                        {
                            mode = Convert.ToInt32(checkMode.SYSValue);
                        }
                        if (mode == 1)
                        {
                            BackgroundJob.Enqueue(() => UpdateTransactionQ(tempTransNo));
                        }
                        else
                        {
                            BackgroundJob.Enqueue(() => UpdateOldQ(tempTransNo));

                        }

                        
                    }
                    else
                    {
                        BackgroundJob.Enqueue(() => UpdateTransaction(tempTransNo));
                    }

                    scope.Complete();
                }

                return new AjaxResponse(new { success = true });
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }

        public async Task<AjaxResponse> FPXDirectResponse(FPXResponse data)
        {
            try
            {
                string tempBizRegID;
                string tempBizLocID;

                var transFPXLogic = new TRANSFPX(_env, _connectionString);
                var transHDRLogic = new TRANSHDR(_env, _connectionString);
                var transDTLLogic = new TRANSDTL(_env, _connectionString);
                var transTenderLogic = new TRANSTENDER(_env, _connectionString);
                var mode = 1;
                data.fpx_checkSumString = data.fpx_buyerBankBranch + "|" + data.fpx_buyerBankId + "|" + data.fpx_buyerIban + "|" + data.fpx_buyerId + "|" + data.fpx_buyerName + "|";
                data.fpx_checkSumString += data.fpx_creditAuthCode + "|" + data.fpx_creditAuthNo + "|" + data.fpx_debitAuthCode + "|" + data.fpx_debitAuthNo + "|" + data.fpx_fpxTxnId + "|";
                data.fpx_checkSumString += data.fpx_fpxTxnTime + "|" + data.fpx_makerName + "|" + data.fpx_msgToken + "|" + data.fpx_msgType + "|" + data.fpx_sellerExId + "|" + data.fpx_sellerExOrderNo + "|";
                data.fpx_checkSumString += data.fpx_sellerId + "|" + data.fpx_sellerOrderNo + "|" + data.fpx_sellerTxnTime + "|" + data.fpx_txnAmount + "|" + data.fpx_txnCurrency;
                data.fpx_checkSumString = data.fpx_checkSumString.Trim();

                Log.Info("FPX Direct Response: " + data.fpx_checkSumString);

                DateTime trxDate = data.fpx_fpxTxnTime != null ? DateTime.ParseExact(data.fpx_fpxTxnTime.Substring(0, 8), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture) : DateTime.ParseExact(DateTime.Now.ToString("yyyyMMdd"), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                String trxTime = data.fpx_fpxTxnTime != null ? data.fpx_fpxTxnTime.Substring(8, 4) : DateTime.Now.ToString("HHmm");

                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var tempTransNo = data.fpx_sellerOrderNo;
                    var transData = transHDRLogic.Retrieve(x => x.TransNo == tempTransNo);
                    if (transData != null)
                    {
                        tempBizRegID = transData.BizRegID;
                        tempBizLocID = transData.BizLocID;

                        Byte? paidStatus;
                        Byte? isApproved;
                        if (data.fpx_debitAuthCode == "00")
                        {
                            paidStatus = 2;
                            isApproved = 1;
                        }
                        else if (data.fpx_debitAuthCode == "09" || data.fpx_debitAuthCode == "99")
                        {
                            paidStatus = 1;
                            isApproved = 0;
                        }
                        else
                        {
                            paidStatus = 3;
                            isApproved = 0;
                        }

                        if (_appConfiguration["InvoiceConfig:PaymentType"] == "CWMS")
                        {

                            var sysPrefbLogic = new Base.Core.General.Repo.SYSPREFB(_env, _connectionString);
                            
                            var checkMode = sysPrefbLogic.Retrieve(x => x.SYSKey == "CHKFAILFPXUSENEWSVC" && x.BranchID == "PLX");
                            if (checkMode != null)
                            {
                                mode = Convert.ToInt32(checkMode.SYSValue);
                            }
                        }

                        var trxNo = data.fpx_sellerOrderNo;

                        var dataTransFPX = transFPXLogic.Retrieve(x => x.TransNo == trxNo);
                        if (dataTransFPX != null)
                        {
                            if (_appConfiguration["InvoiceConfig:PaymentType"] == "CWMS")
                            {
                                if (mode == 1)
                                {
                                    dataTransFPX.LastUpdate = DateTime.Now;
                                    dataTransFPX.TenderID = data.fpx_debitAuthCode;
                                    dataTransFPX.RefInfo = data.fpx_checkSum;
                                    dataTransFPX.CCrefInfo = data.fpx_checkSumString;
                                    await transFPXLogic.Update(dataTransFPX);
                                }
                            }
                            else
                            {
                                dataTransFPX.LastUpdate = DateTime.Now;
                                dataTransFPX.TenderID = data.fpx_debitAuthCode;
                                dataTransFPX.RefInfo = data.fpx_checkSum;
                                dataTransFPX.CCrefInfo = data.fpx_checkSumString;
                                await transFPXLogic.Update(dataTransFPX);
                            }
                            
                        }

                        var dataTransHDR = transHDRLogic.Retrieve(x => x.TransNo == trxNo);
                        if (dataTransHDR != null)
                        {
                            //dataTransHDR.TransReasonCode = data.fpx_fpxTxnId;
                            dataTransHDR.LastUpdate = DateTime.Now;
                            dataTransHDR.Status = paidStatus;
                            dataTransHDR.ShiftCode = data.fpx_debitAuthCode;
                            dataTransHDR.TransStatus = data.fpx_debitAuthCode;
                            await transHDRLogic.Update(dataTransHDR);
                        }

                        var dataTransTender = transTenderLogic.Retrieve(x => x.TransNo == trxNo);
                        if (dataTransTender != null)
                        {
                            dataTransTender.IsApproved = isApproved;
                            dataTransTender.TransStatus = paidStatus;
                            dataTransTender.ExternalID = data.fpx_fpxTxnId;
                            dataTransTender.AuthCode = data.fpx_checkSum;
                            dataTransTender.RespCode = data.fpx_debitAuthCode;
                            dataTransTender.ApprovedDate = DateTime.Now;
                            dataTransTender.LastUpdate = DateTime.Now;
                            dataTransTender.SyncLastUpd = DateTime.Now;
                            await transTenderLogic.Update(dataTransTender);
                        }

                        var dataTransDTL = transDTLLogic.RetrieveAll(x => x.TransNo.Contains(trxNo));
                        if (dataTransDTL != null && dataTransDTL.Count() > 0)
                        {
                            foreach (var index in dataTransDTL)
                            {
                                index.LastUpdate = DateTime.Now;
                                index.Status = paidStatus;
                                await transDTLLogic.Update(index);
                            }
                        }
                    }

                    if (_appConfiguration["InvoiceConfig:PaymentType"] == "CWMS")
                    {

                        if (mode == 1)
                        {
                            BackgroundJob.Enqueue(() => UpdateTransactionQ(tempTransNo));
                        }
                        else
                        {
                            BackgroundJob.Enqueue(() => UpdateOldQ(tempTransNo));

                        }
                    }
                    else
                    {
                        BackgroundJob.Enqueue(() => UpdateTransaction(tempTransNo));
                    }

                    scope.Complete();
                }

                return new AjaxResponse(new { success = true });
            }
            catch(Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }

        #region MayBank
        public async Task<AjaxResponse> ValidateMBBStatus(System.String refInfo)
        {
            MapValidateStatus result = new MapValidateStatus();
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
                string TXN_SIGNATURE = RSASign.GetSignature(string.Concat(MERCHANT_HASHKEY, MERCHANT_ACC_NO, MERCHANT_TRANID, AMOUNT));
                string RESPONSE_TYPE = "PLAIN";
                string RETURN_URL = returnUrl;

                posting_data += "MERCHANT_ACC_NO=" + MERCHANT_ACC_NO + "&MERCHANT_TRANID=" + MERCHANT_TRANID + "&AMOUNT=" + AMOUNT;
                posting_data += "&TRANSACTION_TYPE=" + TRANSACTION_TYPE + "&TXN_SIGNATURE=" + TXN_SIGNATURE;
                posting_data += "&RESPONSE_TYPE=" + RESPONSE_TYPE + "&RETURN_URL=" + RETURN_URL;
                byte[] _byteVersion = Encoding.ASCII.GetBytes(posting_data);

                var sunSystem = new SUNSystemConnection();
                result.CheckSum = sunSystem.POSTWebRequest(url, posting_data, _byteVersion);
                if (string.IsNullOrEmpty(result.CheckSum))
                {
                    result.Message = "No return from MayBank";
                    result.DebitAuthCode = "99";
                    return new AjaxResponse(new ErrorInfo(result.Message));
                }

                result.CheckSum = result.CheckSum.Replace("<BR>", "&");

                if (result.CheckSum.ToUpper().Contains("ERROR"))
                {
                    result.Message = result.CheckSum;
                    result.DebitAuthCode = "99";
                    return new AjaxResponse(new ErrorInfo(result.Message));
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
                    return new AjaxResponse(new ErrorInfo(result.Message));
                }

                if (string.IsNullOrEmpty(RESPONSE_CODE))
                {
                    result.Message = "No return from MayBank";
                    result.DebitAuthCode = "99";
                    return new AjaxResponse(new ErrorInfo(result.Message));
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
                return new AjaxResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }

        public async Task<AjaxResponse> GetMBBSignature(string merchantID, string amount)
        {
            string result = string.Empty;
            string MERCHANT_HASHKEY = string.Empty;
            string MERCHANT_ACC_NO = string.Empty;
            string strConcat = string.Empty;
            string mbbCheckSum = string.Empty;

            try
            {
                MERCHANT_HASHKEY = _appConfiguration["SUNSystem:MBBCCMerchantHashKey_eL"];
                MERCHANT_ACC_NO = _appConfiguration["SUNSystem:MBBCCMerchantAccount_eL"];

                mbbCheckSum = MERCHANT_HASHKEY + "|" + MERCHANT_ACC_NO + "|" + merchantID + "|" + amount;
                mbbCheckSum = mbbCheckSum.Trim();

                strConcat = string.Concat(MERCHANT_HASHKEY, MERCHANT_ACC_NO, merchantID, amount);
                result = RSASign.GetSignature(strConcat);

                return new AjaxResponse(new { CheckSum = mbbCheckSum, TXN = strConcat, TXN_SIGNATURE = result });
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }

        public async Task<AjaxResponse> MBCCGetResponse([FromForm] MBCCResponse data)
        {
            try
            {
                var transHdrLogic = new TRANSHDR(_env, _connectionString);
                var transDTLLogic = new TRANSDTL(_env, _connectionString);
                var transTenderLogic = new TRANSTENDER(_env, _connectionString);

                Byte? paidStatus;
                if (data.HOST_RESPONSE_CODE == "00")
                {
                    paidStatus = 2;
                }
                else if (data.HOST_RESPONSE_CODE == "99")
                {
                    paidStatus = 1;
                }
                else
                {
                    paidStatus = 3;
                }

                var transNo = data.MERCHANT_TRANID;
                var dataTransTender = transTenderLogic.Retrieve(x => x.TransNo == transNo);
                if (dataTransTender != null)
                {
                    var mbbCheckSum = _appConfiguration["SUNSystem:MBBCCMerchantHashKey_eL"] + "|" + _appConfiguration["SUNSystem:MBBCCMerchantAccount_eL"] + "|" + data.MERCHANT_TRANID + "|" + dataTransTender.TenderAmt;
                    mbbCheckSum = mbbCheckSum.Trim();

                    dataTransTender.ExternalID = data.TRANSACTION_ID;
                    dataTransTender.RespCode = data.HOST_RESPONSE_CODE;
                    dataTransTender.RefToken = mbbCheckSum;
                    dataTransTender.RefRemark = data.RESPONSE_DESC;
                    dataTransTender.AuthCode = data.TXN_SIGNATURE;
                    dataTransTender.LastUpdate = DateTime.Now;
                    dataTransTender.IsApproved = 1;
                    await transTenderLogic.Update(dataTransTender);
                }

                var dataTransHDR = transHdrLogic.Retrieve(x => x.TransNo == transNo);
                if (dataTransHDR != null)
                {
                    dataTransHDR.LastUpdate = DateTime.Now;
                    dataTransHDR.Status = paidStatus;
                    dataTransHDR.ShiftCode = data.HOST_RESPONSE_CODE;
                    dataTransHDR.TransStatus = data.HOST_RESPONSE_CODE;
                    await transHdrLogic.Update(dataTransHDR);

                }

                var dataTransDTL = transDTLLogic.RetrieveAll(x => x.TransNo.Contains(transNo));
                if (dataTransDTL != null && dataTransDTL.Count() > 0)
                {
                    foreach (var index in dataTransDTL)
                    {
                        index.LastUpdate = DateTime.Now;
                        index.Status = paidStatus;
                        await transDTLLogic.Update(index);
                    }
                }

                await UpdateTransaction(transNo);

                //Response.Redirect(_appConfiguration["App:ClientRootAddress"] + "account/payment-response?orderNo=" + data.MERCHANT_TRANID + "&creditCard=true");
                return new AjaxResponse(new { success = true });
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }
        #endregion

        #region Tender --Should be Not here, move to Inventory
        public async Task<AjaxResponse> GetTenderList(GetListParameter input)
        {
            try
            {
                var tenderLogic = new TENDER(_env, _connectionString);
                var data = tenderLogic.Select<DTO.Accounting.TENDER>(x =>
                    new
                    {
                        x.TenderID,
                        x.TenderDesc,
                        x.TenderPrompt,
                        x.RefPrompt
                    })
                    .Where<DTO.Accounting.TENDER>(x => x.TenderType == 11 && x.Active == 1)
                    .OrderBy<DTO.Accounting.TENDER>(x => x.TenderDesc)
                    .Fetch(input.SkipCount > 0 ? input.SkipCount : 0, input.MaxResultCount > 0 ? input.MaxResultCount : 0)
                    .ExecuteList<MapTENDER>();

                return new AjaxResponse(data);
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }

        public async Task<AjaxResponse> GetPaymentMethodCombo()
        {
            try
            {
                var paymentLogic = new TENDER(_env, _connectionString);
                var combo = paymentLogic.Select(x =>
                    new
                    {
                        Code = x.TenderID,
                        Remark = x.TenderDesc
                    })
                    .Where(x => x.TenderType != 11 && x.Flag == 1)
                    .Execute();

                return new AjaxResponse(combo);
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }
        #endregion

        #region Customer
        public async Task<AjaxResponse> GetCustomer(GetCustomerSunDB input)
        {
            try
            {
                var scheduleLogic = new PAYMENT(_env, _connectionString);
                scheduleLogic.Select<MapListCustomer>(x =>
                    new
                    {
                        x.ADD_CODE,
                        x.ADDRESS_1,
                        x.ADDRESS_3
                    });

                if (String.IsNullOrEmpty(input.AccountNotIn))
                {
                    input.AccountNotIn = "''";
                }
                else
                {
                    input.AccountNotIn = input.AccountNotIn.Replace(",", "','");
                    input.AccountNotIn = "'" + input.AccountNotIn + "'";
                }
                //input.offset = list.SkipCount;
                //input.limit = list.MaxResultCount;
                input.Filter = String.IsNullOrEmpty(input.Filter) ? string.Empty : input.Filter;
                input.Sorting = String.IsNullOrEmpty(input.Sorting) ? string.Empty : input.Sorting;
                var data = scheduleLogic.StoredProcedureView<GetCustomerSunDB, MapListCustomer>("dbo.GetCustomerSunDB", input);
                data.TotalCount = data.Items.Count();

                return new AjaxResponse(data);
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }
        #endregion

        public async Task<AjaxResponse> GenerateDocCode(string Branch, string TypeCode)
        {
            try
            {
                var lst = new List<string>();
                var codeReceipt = new SYSCODEB(_env, _connectionString);

                var data = codeReceipt
                    .Select<DTO.Accounting.SYSCODEB>((sy) => new { sy.Prefix, sy.SpCode, Runno = sy.RunNo, sy.Postfix, sy.NoPos, Length = sy.NoLength })
                    .Where<DTO.Accounting.SYSCODEB>((sy) =>
                        sy.Status == 1 &&
                        (!string.IsNullOrEmpty(Branch) ? sy.BranchID == Branch : false) &&
                        (!string.IsNullOrEmpty(TypeCode) ? sy.SysCode == TypeCode : false)
                    )
                    .GroupBy<DTO.Accounting.SYSCODEB>((sy) => new { sy.Prefix, sy.SpCode, Runno = sy.RunNo, sy.Postfix, sy.NoPos, Length = sy.NoLength })
                     .Execute();

                foreach (var item in data)
                {
                    lst.Add(!string.IsNullOrEmpty((string)item.Prefix) ? (string)item.Prefix : string.Empty);
                    lst.Add(!string.IsNullOrEmpty((string)item.SpCOde) ? (string)item.SpCOde : string.Empty);
                    lst.Add(!string.IsNullOrEmpty((string)item.Postfix) ? (string)item.Postfix : string.Empty);
                    lst.Add(((Int32)item.Runno).ToString());
                    lst.Add(((Int16)item.NoPos).ToString());
                    lst.Add(((Int32)item.Length).ToString());
                }

                var sunSystem = new SUNSystemConnection();
                var a = new Accounting.Map.StructTrans
                {
                    Prefix = lst[0],
                    SpCode = lst[1],
                    PostFix = lst[2],
                    Runno = Convert.ToInt32(lst[3]),
                    NoPos = Convert.ToInt16(lst[4]),
                    Length = Convert.ToInt32(lst[5])
                };
                var result = sunSystem.GenerateFormattedNo(a.Prefix, a.SpCode, a.PostFix, a.Runno, a.NoPos, a.Length, DateTime.Now, Branch);

                var ReceiptBranchID = Branch;
                var ReceiptData = codeReceipt.Retrieve(aa => aa.BranchID == ReceiptBranchID && aa.SysCode == TypeCode);
                byte add = (byte)(ReceiptData.RunNo + 1);
                ReceiptData.RunNo = add;
                await codeReceipt.Update(ReceiptData);

                return new AjaxResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }

        public async Task<AjaxResponse> GetTxt(string transNo, float TaxAmt, float Rounding)
        {
            var CreateDate = DateTime.Now.ToString("yyyyMMdd");

            dynamic list = null;
            List<dynamic> dataResPaymentHistory = new List<dynamic>();
            int i = 1;
            try
            {
                #region GetPaymentHistoryDetails            
                var resPaymentHistoryDetail = await GetPaymentHistoryDetails(transNo);
                if(resPaymentHistoryDetail.Success == true)
                {
                    var res = (dynamic)resPaymentHistoryDetail.Result;
                    if (res != null)
                    {
                        list = resPaymentHistoryDetail.Result;
                    }
                }

                //list.transDtl.Items[0].StkType = "PRF" ? string InvoiceTitle = "ProformaBill" : 
                //string InvoiceTitle = list.transDtl.Items[0].StkType == "PRF" ? "ProformaBill" : "CreditBill";
                string InvoiceTitle = null;
                string accoCodeD = null;
                string accoCodeC = null;
                string tax = _appConfiguration["SUNSystem:TaxCode"];//"G60901";
                string rounding = _appConfiguration["SUNSystem:RoundingCode"];//"78999";
                if (list.transDtl.Items[0].StkType == "CDT")
                {
                    //Credit Bill
                    string InvoiceTitles = "CREDIT";
                    //string accoCodes = _appConfiguration["SUNSystem:CreditC"];//"G20101";
                    //string accoCodes_r = _appConfiguration["SUNSystem:CreditD"]; //"DM0064";
                    InvoiceTitle = InvoiceTitles;
                    accoCodeD = list.transTender.TenderDesc == "FPX" ? "G20101" : list.transTender.Items[0].TenderDesc == "MAYBANK CREDIT CARD" ? "G20102" : "G00000"; //accoCodes;
                    accoCodeC = "DE0020"; //accoCodes_r;
                }
                else if (list.transDtl.Items[0].StkType == "PRF")
                {
                    //Proforma Bill
                    string InvoiceTitles = "PROFORMA";
                    //string accoCodes = _appConfiguration["SUNSystem:ProformaC"];//"G20202";
                    //string accoCodes_r = _appConfiguration["SUNSystem:ProformaD"];//"72499";
                    InvoiceTitle = InvoiceTitles;
                    accoCodeD = list.transTender.TenderDesc == "FPX" ? "G20101" : list.transTender.Items[0].TenderDesc == "MAYBANK CREDIT CARD" ? "G20102" : "G00000"; //accoCodes;
                    accoCodeC = "73999"; //accoCodes_r;
                }
                else if (list.transDtl.Items[0].StkType == "CSH")
                {
                    //Cash Bill
                    string InvoiceTitles = "CASH";
                    //string accoCodes = _appConfiguration["SUNSystem:CashC"];//"G20202";
                    //string accoCodes_r = _appConfiguration["SUNSystem:CashD"];//list.transDtl.ItemCode;// _CASHD;//"72499";
                    InvoiceTitle = InvoiceTitles;
                    accoCodeD = list.transTender.TenderDesc == "FPX" ? "G20101" : list.transTender.Items[0].TenderDesc == "MAYBANK CREDIT CARD" ? "G20102" : "G00000"; //accoCodes;
                    accoCodeC = ""; //accoCodes_r;
                }

                string _PaymentMethod = null;
                string _ReferencePayment = null;
                if (list.transTender.TenderID == "997") //FPX
                {
                    string PaymentMethod = list.transTender.TenderDesc;
                    string ReferencePayment = list.transTender.ExternalID;
                    _PaymentMethod = PaymentMethod;
                    _ReferencePayment = ReferencePayment;
                }
                else if (list.transTender.TenderID == "996") // mayBank
                {
                    string PaymentMethod = list.transTender.TenderDesc;
                    string ReferencePayment = list.transTender.ExternalID;
                    _PaymentMethod = PaymentMethod;
                    _ReferencePayment = ReferencePayment;
                }
                else // offline payment
                {
                    _PaymentMethod = "Offline Payment";
                    _ReferencePayment = list.transTender.TenderDesc + list.transTender.RefToken;
                }
                #endregion

                var envelope = ConverterHelper.XMLToClass<Document>(_appConfiguration["Document:format"]);
                StringBuilder template = new StringBuilder();
                string format = "000000000000000000.##";
                string count = "0000000.##";
                template.AppendLine("VERSION                         42601");

                var Outputvalue = list.transHdr.Amount * 100;
                string outputValue = Outputvalue.ToString(format);
                var item = list.transDtl.Items;

                string _FileName = InvoiceTitle + "_" + list.transHdr.Date.ToString("Hmmss");

                #region body
                foreach (var items in item)
                {
                    var subtotal = items.SubTotal * 100;
                    string SubTotal = subtotal.ToString(format);

                    template.Append((list.transDtl.Items[0].StkType == "CSH" ? items.ItemCode.PadRight(envelope.Body.Section1.Items.Item[0].Length) : accoCodeD.PadRight(envelope.Body.Section1.Items.Item[0].Length)));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[1].Length));
                    template.Append(list.transHdr.Date.ToString("yyyy0MM").PadRight(envelope.Body.Section1.Items.Item[2].Length));
                    template.Append(list.transHdr.Date.ToString("yyyyMMdd").PadRight(envelope.Body.Section1.Items.Item[3].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[4].Length));
                    template.Append("L".PadRight(envelope.Body.Section1.Items.Item[5].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[6].Length));
                    template.Append((i.ToString(count)).PadRight(envelope.Body.Section1.Items.Item[7].Length));
                    template.Append(SubTotal.PadRight(envelope.Body.Section1.Items.Item[8].Length));
                    template.Append("D".PadRight(envelope.Body.Section1.Items.Item[9].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[10].Length));
                    template.Append("JRC".PadRight(envelope.Body.Section1.Items.Item[11].Length));
                    template.Append("ABI".PadRight(envelope.Body.Section1.Items.Item[12].Length));
                    template.Append((_FileName.Length < 15 ? (_FileName).PadRight(envelope.Body.Section1.Items.Item[13].Length) : (_FileName).Substring(0, 15).PadRight(envelope.Body.Section1.Items.Item[13].Length)));
                    template.Append(transNo.PadRight(envelope.Body.Section1.Items.Item[14].Length));
                    template.Append(list.transHdr.Date.ToString("yyyyMMdd").PadRight(envelope.Body.Section1.Items.Item[15].Length));
                    template.Append(list.transHdr.Date.ToString("yyyy0MM").PadRight(envelope.Body.Section1.Items.Item[16].Length));
                    template.Append(list.transHdr.Date.ToString("yyyyMMdd").PadRight(envelope.Body.Section1.Items.Item[17].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[18].Length));
                    template.Append((i == 1 ? "000000000".PadRight(envelope.Body.Section1.Items.Item[19].Length) : "".PadRight(envelope.Body.Section1.Items.Item[19].Length)));
                    template.Append((i == 1 ? "00000000".PadRight(envelope.Body.Section1.Items.Item[20].Length) : "".PadRight(envelope.Body.Section1.Items.Item[20].Length)));
                    template.Append((i == 1 ? "0000000".PadRight(envelope.Body.Section1.Items.Item[21].Length) : "".PadRight(envelope.Body.Section1.Items.Item[21].Length)));
                    //template.Append((i == 1 ? "0".PadRight(envelope.Body.Section1.Items.Item[22].Length) : "".PadRight(envelope.Body.Section1.Items.Item[22].Length)));
                    //template.Append((i == 1 ? "0000000000".PadRight(envelope.Body.Section1.Items.Item[23].Length) : "".PadRight(envelope.Body.Section1.Items.Item[23].Length)));
                    //template.Append((i == 1 ? "0".PadRight(envelope.Body.Section1.Items.Item[24].Length) : "".PadRight(envelope.Body.Section1.Items.Item[24].Length)));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[22].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[23].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[24].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[25].Length));
                    template.Append((i == 1 ? "000000000000000000".PadRight(envelope.Body.Section1.Items.Item[26].Length) : "".PadRight(envelope.Body.Section1.Items.Item[26].Length)));
                    template.Append((i == 1 ? "000000000000000000".PadRight(envelope.Body.Section1.Items.Item[27].Length) : "".PadRight(envelope.Body.Section1.Items.Item[27].Length)));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[28].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[29].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[30].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[31].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[32].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[33].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[34].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[35].Length));
                    template.Append(list.transHdr.TransReasonCode.PadRight(envelope.Body.Section1.Items.Item[36].Length));
                    template.Append("00040".PadRight(envelope.Body.Section1.Items.Item[37].Length));
                    template.Append((_PaymentMethod.Length < 15 ? _PaymentMethod.PadRight(envelope.Body.Section1.Items.Item[38].Length) : (_PaymentMethod).Substring(0, 15).PadRight(envelope.Body.Section1.Items.Item[38].Length)));
                    template.Append(list.transHdr.CustomerNo.PadRight(envelope.Body.Section1.Items.Item[39].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[40].Length));
                    template.Append((_ReferencePayment.Length < 15 ? _ReferencePayment.PadRight(envelope.Body.Section1.Items.Item[41].Length) : (_ReferencePayment).Substring(0, 15).PadRight(envelope.Body.Section1.Items.Item[41].Length)));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[42].Length));
                    template.Append("XD".PadRight(envelope.Body.Section1.Items.Item[43].Length));
                    template.Append("OS".PadRight(envelope.Body.Section1.Items.Item[44].Length));
                    template.Append(list.transHdr.Date.ToString("yyyyMMdd").PadRight(envelope.Body.Section1.Items.Item[45].Length));
                    template.Append("\n");
                    i++;
                }

                if (TaxAmt != 0)
                {
                    var Tax = TaxAmt * 100;
                    string outputValueTax = Tax.ToString(format);

                    template.Append(tax.PadRight(envelope.Body.Section1.Items.Item[0].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[1].Length));
                    template.Append(list.transHdr.Date.ToString("yyyy0MM").PadRight(envelope.Body.Section1.Items.Item[2].Length));
                    template.Append(list.transHdr.Date.ToString("yyyyMMdd").PadRight(envelope.Body.Section1.Items.Item[3].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[4].Length));
                    template.Append("L".PadRight(envelope.Body.Section1.Items.Item[5].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[6].Length));
                    template.Append((i.ToString(count)).PadRight(envelope.Body.Section1.Items.Item[7].Length));
                    template.Append(outputValueTax.PadRight(envelope.Body.Section1.Items.Item[8].Length));
                    template.Append("D".PadRight(envelope.Body.Section1.Items.Item[9].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[10].Length));
                    template.Append("JRC".PadRight(envelope.Body.Section1.Items.Item[11].Length));
                    template.Append("ABI".PadRight(envelope.Body.Section1.Items.Item[12].Length));
                    template.Append((_FileName.Length < 15 ? (_FileName).PadRight(envelope.Body.Section1.Items.Item[13].Length) : (_FileName).Substring(0, 15).PadRight(envelope.Body.Section1.Items.Item[13].Length)));
                    template.Append(transNo.PadRight(envelope.Body.Section1.Items.Item[14].Length));
                    template.Append(list.transHdr.Date.ToString("yyyyMMdd").PadRight(envelope.Body.Section1.Items.Item[15].Length));
                    template.Append(list.transHdr.Date.ToString("yyyy0MM").PadRight(envelope.Body.Section1.Items.Item[16].Length));
                    template.Append(list.transHdr.Date.ToString("yyyyMMdd").PadRight(envelope.Body.Section1.Items.Item[17].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[18].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[19].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[20].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[21].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[22].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[23].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[24].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[25].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[26].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[27].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[28].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[29].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[30].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[31].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[32].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[33].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[34].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[35].Length));
                    template.Append(list.transHdr.TransReasonCode.PadRight(envelope.Body.Section1.Items.Item[36].Length));
                    template.Append("00040".PadRight(envelope.Body.Section1.Items.Item[37].Length));
                    template.Append((_PaymentMethod.Length < 15 ? _PaymentMethod.PadRight(envelope.Body.Section1.Items.Item[38].Length) : (_PaymentMethod).Substring(0, 15).PadRight(envelope.Body.Section1.Items.Item[38].Length)));
                    template.Append(list.transHdr.CustomerNo.PadRight(envelope.Body.Section1.Items.Item[39].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[40].Length));
                    template.Append((_ReferencePayment.Length < 15 ? _ReferencePayment.PadRight(envelope.Body.Section1.Items.Item[41].Length) : (_ReferencePayment).Substring(0, 15).PadRight(envelope.Body.Section1.Items.Item[41].Length)));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[42].Length));
                    template.Append("XD".PadRight(envelope.Body.Section1.Items.Item[43].Length));
                    template.Append("OS".PadRight(envelope.Body.Section1.Items.Item[44].Length));
                    template.Append(list.transHdr.Date.ToString("yyyyMMdd").PadRight(envelope.Body.Section1.Items.Item[45].Length));
                    template.Append("\n");
                    i++;
                }

                if (Rounding != 0)
                {
                    string Status = null;
                    float Round = 0;

                    if (Rounding < 0)
                    {
                        string Stat = "C";
                        var Rounds = Rounding * 100 * -1;
                        Status = Stat;
                        Round = Rounds;
                    }
                    if (Rounding > 0)
                    {
                        string Stat = "D";
                        var Rounds = Rounding * 100;
                        Status = Stat;
                        Round = Rounds;
                    }

                    string outputValueRound = Round.ToString(format);

                    template.Append(rounding.PadRight(envelope.Body.Section1.Items.Item[0].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[1].Length));
                    template.Append(list.transHdr.Date.ToString("yyyy0MM").PadRight(envelope.Body.Section1.Items.Item[2].Length));
                    template.Append(list.transHdr.Date.ToString("yyyyMMdd").PadRight(envelope.Body.Section1.Items.Item[3].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[4].Length));
                    template.Append("L".PadRight(envelope.Body.Section1.Items.Item[5].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[6].Length));
                    template.Append((i.ToString(count)).PadRight(envelope.Body.Section1.Items.Item[7].Length));
                    template.Append(outputValueRound.PadRight(envelope.Body.Section1.Items.Item[8].Length));
                    template.Append(Status.PadRight(envelope.Body.Section1.Items.Item[9].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[10].Length));
                    template.Append("JRC".PadRight(envelope.Body.Section1.Items.Item[11].Length));
                    template.Append("ABI".PadRight(envelope.Body.Section1.Items.Item[12].Length));
                    template.Append((_FileName.Length < 15 ? (_FileName).PadRight(envelope.Body.Section1.Items.Item[13].Length) : (_FileName).Substring(0, 15).PadRight(envelope.Body.Section1.Items.Item[13].Length)));
                    template.Append(transNo.PadRight(envelope.Body.Section1.Items.Item[14].Length));
                    template.Append(list.transHdr.Date.ToString("yyyyMMdd").PadRight(envelope.Body.Section1.Items.Item[15].Length));
                    template.Append(list.transHdr.Date.ToString("yyyy0MM").PadRight(envelope.Body.Section1.Items.Item[16].Length));
                    template.Append(list.transHdr.Date.ToString("yyyyMMdd").PadRight(envelope.Body.Section1.Items.Item[17].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[18].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[19].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[20].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[21].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[22].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[23].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[24].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[25].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[26].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[27].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[28].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[29].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[30].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[31].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[32].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[33].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[34].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[35].Length));
                    template.Append(list.transHdr.TransReasonCode.PadRight(envelope.Body.Section1.Items.Item[36].Length));
                    template.Append("00040".PadRight(envelope.Body.Section1.Items.Item[37].Length));
                    template.Append((_PaymentMethod.Length < 15 ? _PaymentMethod.PadRight(envelope.Body.Section1.Items.Item[38].Length) : (_PaymentMethod).Substring(0, 15).PadRight(envelope.Body.Section1.Items.Item[38].Length)));
                    template.Append(list.transHdr.CustomerNo.PadRight(envelope.Body.Section1.Items.Item[39].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[40].Length));
                    template.Append((_ReferencePayment.Length < 15 ? _ReferencePayment.PadRight(envelope.Body.Section1.Items.Item[41].Length) : (_ReferencePayment).Substring(0, 15).PadRight(envelope.Body.Section1.Items.Item[41].Length)));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[42].Length));
                    template.Append("XD".PadRight(envelope.Body.Section1.Items.Item[43].Length));
                    template.Append("OS".PadRight(envelope.Body.Section1.Items.Item[44].Length));
                    template.Append(list.transHdr.Date.ToString("yyyyMMdd").PadRight(envelope.Body.Section1.Items.Item[45].Length));
                    template.Append("\n");
                    i++;
                }

                template.Append(accoCodeC.PadRight(envelope.Body.Section1.Items.Item[0].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[1].Length));
                template.Append(list.transHdr.Date.ToString("yyyy0MM").PadRight(envelope.Body.Section1.Items.Item[2].Length));
                template.Append(list.transHdr.Date.ToString("yyyyMMdd").PadRight(envelope.Body.Section1.Items.Item[3].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[4].Length));
                template.Append("L".PadRight(envelope.Body.Section1.Items.Item[5].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[6].Length));
                template.Append((i.ToString(count)).PadRight(envelope.Body.Section1.Items.Item[7].Length));
                template.Append(outputValue.PadRight(envelope.Body.Section1.Items.Item[8].Length));
                template.Append("C".PadRight(envelope.Body.Section1.Items.Item[9].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[10].Length));
                template.Append("JRC".PadRight(envelope.Body.Section1.Items.Item[11].Length));
                template.Append("ABI".PadRight(envelope.Body.Section1.Items.Item[12].Length));
                template.Append((_FileName.Length < 15 ? (_FileName).PadRight(envelope.Body.Section1.Items.Item[13].Length) : (_FileName).Substring(0, 15).PadRight(envelope.Body.Section1.Items.Item[13].Length)));
                template.Append(transNo.PadRight(envelope.Body.Section1.Items.Item[14].Length));
                template.Append(list.transHdr.Date.ToString("yyyyMMdd").PadRight(envelope.Body.Section1.Items.Item[15].Length));
                template.Append(list.transHdr.Date.ToString("yyyy0MM").PadRight(envelope.Body.Section1.Items.Item[16].Length));
                template.Append(list.transHdr.Date.ToString("yyyyMMdd").PadRight(envelope.Body.Section1.Items.Item[17].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[18].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[19].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[20].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[21].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[22].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[23].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[24].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[25].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[26].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[27].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[28].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[29].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[30].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[31].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[32].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[33].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[34].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[35].Length));
                template.Append(list.transHdr.TransReasonCode.PadRight(envelope.Body.Section1.Items.Item[36].Length));
                template.Append("00040".PadRight(envelope.Body.Section1.Items.Item[37].Length));
                template.Append((_PaymentMethod.Length < 15 ? _PaymentMethod.PadRight(envelope.Body.Section1.Items.Item[38].Length) : (_PaymentMethod).Substring(0, 15).PadRight(envelope.Body.Section1.Items.Item[38].Length)));
                template.Append(list.transHdr.CustomerNo.PadRight(envelope.Body.Section1.Items.Item[39].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[40].Length));
                template.Append((_ReferencePayment.Length < 15 ? _ReferencePayment.PadRight(envelope.Body.Section1.Items.Item[41].Length) : (_ReferencePayment).Substring(0, 15).PadRight(envelope.Body.Section1.Items.Item[41].Length)));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[42].Length));
                template.Append("XD".PadRight(envelope.Body.Section1.Items.Item[43].Length));
                template.Append("OS".PadRight(envelope.Body.Section1.Items.Item[44].Length));
                template.Append(list.transHdr.Date.ToString("yyyyMMdd").PadRight(envelope.Body.Section1.Items.Item[45].Length));
                template.Append("\n");
                i++;
                #endregion

                var data = template.ToString();

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var exporter = _txtExporter.ExportToFileTxt(data, CreateDate + "_CSA_03_" + CreateDate + InvoiceTitle, Encoding.GetEncoding(1252));
                return new AjaxResponse(exporter);
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }

        public async Task<AjaxResponse> GetTextFileString(string transNo)
        {
            var CreateDate = DateTime.Now.ToString("yyyyMMdd");

            dynamic list = null;
            List<dynamic> dataResPaymentHistory = new List<dynamic>();
            int i = 1;
            try
            {
                #region Getting Detail Data
                var resPaymentHistoryDetail = await GetPaymentHistoryDetails(transNo);
                if (resPaymentHistoryDetail.Success == true)
                {
                    var res = (dynamic)resPaymentHistoryDetail.Result;
                    if (res != null)
                    {
                        list = resPaymentHistoryDetail.Result;
                    }
                }

                string InvoiceTitle = null;
                string accoCodeD = null;
                string accoCodeC = null;
                string tax = _appConfiguration["SUNSystem:TaxCode"];
                string rounding = _appConfiguration["SUNSystem:RoundingCode"];

                if (list.transDtl.Items[0].StkType == "CDT")
                {
                    //Credit Bill
                    string InvoiceTitles = "CREDIT";
                    InvoiceTitle = InvoiceTitles;
                    accoCodeD = "G00000";
                    accoCodeC = list.transHdr.CustomerNo;
                }
                else if (list.transDtl.Items[0].StkType == "PRF")
                {
                    //Proforma Bill
                    string InvoiceTitles = "PROFORMA";
                    InvoiceTitle = InvoiceTitles;
                    accoCodeD = "G00000";
                    //var pstn = list.transHdr.CreditVote.IndexOf("-");
                    //accoCodeC = list.transHdr.CreditVote.Substring(0, pstn); // "73999";
                    accoCodeC = "";
                }
                else if (list.transDtl.Items[0].StkType == "CSH")
                {
                    //Cash Bill
                    string InvoiceTitles = "CASH";
                    InvoiceTitle = InvoiceTitles;
                    accoCodeD = "G00000";
                    accoCodeC = "";
                }

                string _PaymentMethod = null;
                string _ReferencePayment = null;
                if (list.transTender.TenderID == "997") //FPX
                {
                    string PaymentMethod = list.transTender.TenderDesc;
                    string ReferencePayment = list.transTender.ExternalID;

                    accoCodeD = _appConfiguration["SUNSystem:FPXCode"];
                    _PaymentMethod = PaymentMethod;
                    _ReferencePayment = ReferencePayment;
                }
                else if (list.transTender.TenderID == "996") // mayBank
                {
                    string PaymentMethod = list.transTender.TenderDesc;
                    string ReferencePayment = list.transTender.ExternalID;

                    accoCodeD = _appConfiguration["SUNSystem:CreditCardCode"];
                    _PaymentMethod = "CARD"; // PaymentMethod;
                    _ReferencePayment = ReferencePayment;
                }
                else // offline payment
                {
                    accoCodeD = _appConfiguration["SUNSystem:CashBillCode"];
                    _PaymentMethod = "OFFLINE";
                    _ReferencePayment = string.Empty; // list.transTender.TenderDesc + list.transTender.RefToken;
                }
                #endregion

                var envelope = ConverterHelper.XMLToClass<Document>(_appConfiguration["Document:format"]);
                StringBuilder template = new StringBuilder();
                string format = "000000000000000000.##";
                string count = "0000000.##";
                template.AppendLine("VERSION                         42601");

                var Outputvalue = list.transHdr.Amount * 1000;
                string outputValue = Outputvalue.ToString(format);
                var item = list.transDtl.Items;

                string _FileName = InvoiceTitle + "_" + list.transHdr.Date.ToString("Hmmss");

                string taxCatCode = list.transHdr.TaxCatCode;

                var cdPjIdx = 0;
                string projectCode = string.Empty;
                if (!string.IsNullOrEmpty(list.transHdr.ProjectCode) && list.transDtl.Items[0].ItemType == 0)
                {
                    cdPjIdx = list.transHdr.ProjectCode.IndexOf("-");
                    projectCode = list.transHdr.ProjectCode.Substring(0, cdPjIdx);
                }

                #region String Builder Body

                #region line header
                //template.Append((list.transDtl.Items[0].StkType == "CSH" ? items.ItemCode.PadRight(envelope.Body.Section1.Items.Item[0].Length) : accoCodeD.PadRight(envelope.Body.Section1.Items.Item[0].Length))); -- Cash Bill
                template.Append(accoCodeD.PadRight(envelope.Body.Section1.Items.Item[0].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[1].Length));
                template.Append(list.transHdr.Date.ToString("yyyy0MM").PadRight(envelope.Body.Section1.Items.Item[2].Length));
                template.Append(list.transHdr.Date.ToString("yyyyMMdd").PadRight(envelope.Body.Section1.Items.Item[3].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[4].Length));
                template.Append("L".PadRight(envelope.Body.Section1.Items.Item[5].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[6].Length));
                template.Append((i.ToString(count)).PadRight(envelope.Body.Section1.Items.Item[7].Length));
                template.Append(outputValue.PadRight(envelope.Body.Section1.Items.Item[8].Length));
                template.Append("D".PadRight(envelope.Body.Section1.Items.Item[9].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[10].Length));
                template.Append("JRC".PadRight(envelope.Body.Section1.Items.Item[11].Length));
                template.Append("ABI".PadRight(envelope.Body.Section1.Items.Item[12].Length));
                //template.Append((_FileName.Length < 15 ? (_FileName).PadRight(envelope.Body.Section1.Items.Item[13].Length) : (_FileName).Substring(0, 15).PadRight(envelope.Body.Section1.Items.Item[13].Length)));
                template.Append(transNo.PadRight(envelope.Body.Section1.Items.Item[13].Length));
                template.Append(transNo.PadRight(envelope.Body.Section1.Items.Item[14].Length));
                template.Append(list.transHdr.Date.ToString("yyyyMMdd").PadRight(envelope.Body.Section1.Items.Item[15].Length));
                template.Append(list.transHdr.Date.ToString("yyyy0MM").PadRight(envelope.Body.Section1.Items.Item[16].Length));
                template.Append(list.transHdr.Date.ToString("yyyyMMdd").PadRight(envelope.Body.Section1.Items.Item[17].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[18].Length));
                template.Append("000000000".PadRight(envelope.Body.Section1.Items.Item[19].Length));
                template.Append("00000000".PadRight(envelope.Body.Section1.Items.Item[20].Length));
                template.Append("0000000".PadRight(envelope.Body.Section1.Items.Item[21].Length));
                //template.Append((i == 1 ? "000000000".PadRight(envelope.Body.Section1.Items.Item[19].Length) : "".PadRight(envelope.Body.Section1.Items.Item[19].Length)));
                //template.Append((i == 1 ? "00000000".PadRight(envelope.Body.Section1.Items.Item[20].Length) : "".PadRight(envelope.Body.Section1.Items.Item[20].Length)));
                //template.Append((i == 1 ? "0000000".PadRight(envelope.Body.Section1.Items.Item[21].Length) : "".PadRight(envelope.Body.Section1.Items.Item[21].Length)));
                //template.Append((i == 1 ? "0".PadRight(envelope.Body.Section1.Items.Item[22].Length) : "".PadRight(envelope.Body.Section1.Items.Item[22].Length)));
                //template.Append((i == 1 ? "0000000000".PadRight(envelope.Body.Section1.Items.Item[23].Length) : "".PadRight(envelope.Body.Section1.Items.Item[23].Length)));
                //template.Append((i == 1 ? "0".PadRight(envelope.Body.Section1.Items.Item[24].Length) : "".PadRight(envelope.Body.Section1.Items.Item[24].Length)));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[22].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[23].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[24].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[25].Length));
                template.Append((i == 1 ? "000000000000000000".PadRight(envelope.Body.Section1.Items.Item[26].Length) : "".PadRight(envelope.Body.Section1.Items.Item[26].Length)));
                template.Append((i == 1 ? "000000000000000000".PadRight(envelope.Body.Section1.Items.Item[27].Length) : "".PadRight(envelope.Body.Section1.Items.Item[27].Length)));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[28].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[29].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[30].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[31].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[32].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[33].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[34].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[35].Length));
                //template.Append("".PadRight(envelope.Body.Section1.Items.Item[36].Length));
                template.Append(list.transHdr.TransReasonCode.PadRight(envelope.Body.Section1.Items.Item[36].Length));
                template.Append("00040".PadRight(envelope.Body.Section1.Items.Item[37].Length));
                template.Append((_PaymentMethod.Length < 15 ? _PaymentMethod.PadRight(envelope.Body.Section1.Items.Item[38].Length) : (_PaymentMethod).Substring(0, 15).PadRight(envelope.Body.Section1.Items.Item[38].Length)));
                template.Append(list.transHdr.CustomerNo.PadRight(envelope.Body.Section1.Items.Item[39].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[40].Length));
                //template.Append((_ReferencePayment.Length < 15 ? _ReferencePayment.PadRight(envelope.Body.Section1.Items.Item[41].Length) : (_ReferencePayment).Substring(0, 15).PadRight(envelope.Body.Section1.Items.Item[41].Length)));
                template.Append(projectCode.PadRight(envelope.Body.Section1.Items.Item[41].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[42].Length));
                template.Append("".PadRight(envelope.Body.Section1.Items.Item[43].Length));
                //template.Append("XD".PadRight(envelope.Body.Section1.Items.Item[43].Length));
                //template.Append("OS".PadRight(envelope.Body.Section1.Items.Item[44].Length));
                template.Append((string.IsNullOrEmpty(taxCatCode) ? "OS" : taxCatCode).PadRight(envelope.Body.Section1.Items.Item[44].Length));
                template.Append(list.transHdr.Date.ToString("yyyyMMdd").PadRight(envelope.Body.Section1.Items.Item[45].Length));
                template.Append("\n");
                i++;
                #endregion

                #region line tax
                var itx = 1;
                foreach (var items in item)
                {
                    //if ((items.ItemType == 0 || items.StkType == "CSH") && list.transHdr.Tax != 0 && itx > 1)
                    if (list.transHdr.Tax != 0 && itx > 1)
                        break;
                    //if ((items.ItemType == 0 && list.transHdr.Tax != 0) || (items.ItemType == 1 && items.Tax != 0) || (items.StkType == "CSH" && list.transHdr.Tax != 0))
                    if (list.transHdr.Tax != 0 || items.Tax != 0)
                    {
                        //var Tax = items.ItemType == 0 ? list.transHdr.Tax * 1000 : items.StkType == "CSH" ? list.transHdr.Tax * 1000 : items.Tax * 1000;
                        var Tax = (items.ItemType == 0 || items.ItemType == 1) && list.transHdr.Tax != 0 ? list.transHdr.Tax * 1000 : items.Tax * 1000;
                        string outputValueTax = Tax.ToString(format);

                        template.Append(tax.PadRight(envelope.Body.Section1.Items.Item[0].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[1].Length));
                        template.Append(list.transHdr.Date.ToString("yyyy0MM").PadRight(envelope.Body.Section1.Items.Item[2].Length));
                        template.Append(list.transHdr.Date.ToString("yyyyMMdd").PadRight(envelope.Body.Section1.Items.Item[3].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[4].Length));
                        template.Append("L".PadRight(envelope.Body.Section1.Items.Item[5].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[6].Length));
                        template.Append((i.ToString(count)).PadRight(envelope.Body.Section1.Items.Item[7].Length));
                        template.Append(outputValueTax.PadRight(envelope.Body.Section1.Items.Item[8].Length));
                        template.Append("C".PadRight(envelope.Body.Section1.Items.Item[9].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[10].Length));
                        template.Append("JRC".PadRight(envelope.Body.Section1.Items.Item[11].Length));
                        template.Append("ABI".PadRight(envelope.Body.Section1.Items.Item[12].Length));
                        //template.Append((_FileName.Length < 15 ? (_FileName).PadRight(envelope.Body.Section1.Items.Item[13].Length) : (_FileName).Substring(0, 15).PadRight(envelope.Body.Section1.Items.Item[13].Length)));
                        template.Append(transNo.PadRight(envelope.Body.Section1.Items.Item[13].Length));
                        template.Append(transNo.PadRight(envelope.Body.Section1.Items.Item[14].Length));
                        template.Append(list.transHdr.Date.ToString("yyyyMMdd").PadRight(envelope.Body.Section1.Items.Item[15].Length));
                        template.Append(list.transHdr.Date.ToString("yyyy0MM").PadRight(envelope.Body.Section1.Items.Item[16].Length));
                        template.Append(list.transHdr.Date.ToString("yyyyMMdd").PadRight(envelope.Body.Section1.Items.Item[17].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[18].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[19].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[20].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[21].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[22].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[23].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[24].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[25].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[26].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[27].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[28].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[29].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[30].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[31].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[32].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[33].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[34].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[35].Length));
                        //template.Append("".PadRight(envelope.Body.Section1.Items.Item[36].Length));
                        template.Append(list.transHdr.TransReasonCode.PadRight(envelope.Body.Section1.Items.Item[36].Length));
                        template.Append("00040".PadRight(envelope.Body.Section1.Items.Item[37].Length));
                        template.Append((_PaymentMethod.Length < 15 ? _PaymentMethod.PadRight(envelope.Body.Section1.Items.Item[38].Length) : (_PaymentMethod).Substring(0, 15).PadRight(envelope.Body.Section1.Items.Item[38].Length)));
                        template.Append(list.transHdr.CustomerNo.PadRight(envelope.Body.Section1.Items.Item[39].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[40].Length));
                        //template.Append((_ReferencePayment.Length < 15 ? _ReferencePayment.PadRight(envelope.Body.Section1.Items.Item[41].Length) : (_ReferencePayment).Substring(0, 15).PadRight(envelope.Body.Section1.Items.Item[41].Length)));
                        template.Append(projectCode.PadRight(envelope.Body.Section1.Items.Item[41].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[42].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[43].Length));
                        //template.Append("XD".PadRight(envelope.Body.Section1.Items.Item[43].Length));
                        //template.Append("OS".PadRight(envelope.Body.Section1.Items.Item[44].Length));
                        template.Append((string.IsNullOrEmpty(taxCatCode) ? "OS" : taxCatCode).PadRight(envelope.Body.Section1.Items.Item[44].Length));
                        template.Append(list.transHdr.Date.ToString("yyyyMMdd").PadRight(envelope.Body.Section1.Items.Item[45].Length));
                        template.Append("\n");
                        itx++;
                        i++;
                    }
                }
                #endregion

                #region line rouding
                var itr = 1;
                foreach (var items in item)
                {
                    //if ((items.ItemType == 0 || items.StkType == "CSH") && list.transHdr.Rounding != 0 && itr > 1)
                    if (list.transHdr.Rounding != 0 && itr > 1)
                        break;
                    //if ((items.ItemType == 0 && list.transHdr.Rounding != 0) || (items.ItemType == 1 && (items.Rounding != 0 || list.transHdr.Rounding != 0)) || (items.StkType == "CSH" && list.transHdr.Rounding != 0))
                    if (list.transHdr.Rounding != 0 || items.Rounding != 0)
                    {
                        var Rounding = (items.ItemType == 0 || items.ItemType == 1) && list.transHdr.Rounding != 0 ? list.transHdr.Rounding : items.Rouding;
                        string Status = null;
                        float Round = 0;

                        if (Rounding < 0)
                        {
                            string Stat = "D";
                            var Rounds = (float)Rounding * 1000 * -1;
                            Status = Stat;
                            Round = Rounds;
                        }
                        if (Rounding > 0)
                        {
                            string Stat = "C";
                            var Rounds = (float)Rounding * 1000;
                            Status = Stat;
                            Round = Rounds;
                        }

                        string outputValueRound = Round.ToString(format);

                        template.Append(rounding.PadRight(envelope.Body.Section1.Items.Item[0].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[1].Length));
                        template.Append(list.transHdr.Date.ToString("yyyy0MM").PadRight(envelope.Body.Section1.Items.Item[2].Length));
                        template.Append(list.transHdr.Date.ToString("yyyyMMdd").PadRight(envelope.Body.Section1.Items.Item[3].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[4].Length));
                        template.Append("L".PadRight(envelope.Body.Section1.Items.Item[5].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[6].Length));
                        template.Append((i.ToString(count)).PadRight(envelope.Body.Section1.Items.Item[7].Length));
                        template.Append(outputValueRound.PadRight(envelope.Body.Section1.Items.Item[8].Length));
                        template.Append(Status.PadRight(envelope.Body.Section1.Items.Item[9].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[10].Length));
                        template.Append("JRC".PadRight(envelope.Body.Section1.Items.Item[11].Length));
                        template.Append("ABI".PadRight(envelope.Body.Section1.Items.Item[12].Length));
                        //template.Append((_FileName.Length < 15 ? (_FileName).PadRight(envelope.Body.Section1.Items.Item[13].Length) : (_FileName).Substring(0, 15).PadRight(envelope.Body.Section1.Items.Item[13].Length)));
                        template.Append(transNo.PadRight(envelope.Body.Section1.Items.Item[13].Length));
                        template.Append(transNo.PadRight(envelope.Body.Section1.Items.Item[14].Length));
                        template.Append(list.transHdr.Date.ToString("yyyyMMdd").PadRight(envelope.Body.Section1.Items.Item[15].Length));
                        template.Append(list.transHdr.Date.ToString("yyyy0MM").PadRight(envelope.Body.Section1.Items.Item[16].Length));
                        template.Append(list.transHdr.Date.ToString("yyyyMMdd").PadRight(envelope.Body.Section1.Items.Item[17].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[18].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[19].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[20].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[21].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[22].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[23].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[24].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[25].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[26].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[27].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[28].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[29].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[30].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[31].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[32].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[33].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[34].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[35].Length));
                        //template.Append("".PadRight(envelope.Body.Section1.Items.Item[36].Length));
                        template.Append(list.transHdr.TransReasonCode.PadRight(envelope.Body.Section1.Items.Item[36].Length));
                        template.Append("00040".PadRight(envelope.Body.Section1.Items.Item[37].Length));
                        template.Append((_PaymentMethod.Length < 15 ? _PaymentMethod.PadRight(envelope.Body.Section1.Items.Item[38].Length) : (_PaymentMethod).Substring(0, 15).PadRight(envelope.Body.Section1.Items.Item[38].Length)));
                        template.Append(list.transHdr.CustomerNo.PadRight(envelope.Body.Section1.Items.Item[39].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[40].Length));
                        //template.Append((_ReferencePayment.Length < 15 ? _ReferencePayment.PadRight(envelope.Body.Section1.Items.Item[41].Length) : (_ReferencePayment).Substring(0, 15).PadRight(envelope.Body.Section1.Items.Item[41].Length)));
                        template.Append(projectCode.PadRight(envelope.Body.Section1.Items.Item[41].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[42].Length));
                        template.Append("".PadRight(envelope.Body.Section1.Items.Item[43].Length));
                        //template.Append("XD".PadRight(envelope.Body.Section1.Items.Item[43].Length));
                        //template.Append("OS".PadRight(envelope.Body.Section1.Items.Item[44].Length));
                        template.Append((string.IsNullOrEmpty(taxCatCode) ? "OS" : taxCatCode).PadRight(envelope.Body.Section1.Items.Item[44].Length));
                        template.Append(list.transHdr.Date.ToString("yyyyMMdd").PadRight(envelope.Body.Section1.Items.Item[45].Length));
                        template.Append("\n");
                        itr++;
                        i++;
                    }
                }
                #endregion

                #region line item
                var it = 1;
                foreach (var items in item)
                {
                    int icount = item.Count;
                    var subtotal = items.SubTotal * 1000;
                    string SubTotal = subtotal.ToString(format);

                    var cdVtIdx = 0;
                    var PRFcode = string.Empty;
                    if (list.transDtl.Items[0].StkType == "PRF")
                    {
                        cdVtIdx = list.transHdr.CreditVote.IndexOf("-");
                        PRFcode = items.ItemType == 0 ? list.transHdr.CreditVote.Substring(0, cdVtIdx) : items.CreditVote;
                    }

                    bool singleInv = false;
                    if ((list.transDtl.Items[0].StkType != "CSH" && items.ItemType == 0) && list.transHdr.TransSubTotal != 0)
                    {
                        subtotal = list.transHdr.TransSubTotal * 1000;
                        SubTotal = subtotal.ToString(format);
                        singleInv = true;

                        if (it > 1)
                            break;
                    }

                    if (items.ItemType == 1)
                        projectCode = items.ProjectCode;

                    //template.Append(accoCodeC.PadRight(envelope.Body.Section1.Items.Item[0].Length));
                    template.Append((list.transDtl.Items[0].StkType == "CSH" ? items.ItemCode.PadRight(envelope.Body.Section1.Items.Item[0].Length) :
                        list.transDtl.Items[0].StkType == "PRF" ? PRFcode.PadRight(envelope.Body.Section1.Items.Item[0].Length) : accoCodeC.PadRight(envelope.Body.Section1.Items.Item[0].Length)));
                    //template.Append(items.ItemCode.PadRight(envelope.Body.Section1.Items.Item[0].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[1].Length));
                    template.Append(list.transHdr.Date.ToString("yyyy0MM").PadRight(envelope.Body.Section1.Items.Item[2].Length));
                    template.Append(list.transHdr.Date.ToString("yyyyMMdd").PadRight(envelope.Body.Section1.Items.Item[3].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[4].Length));
                    template.Append("L".PadRight(envelope.Body.Section1.Items.Item[5].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[6].Length));
                    template.Append((i.ToString(count)).PadRight(envelope.Body.Section1.Items.Item[7].Length));
                    template.Append(SubTotal.PadRight(envelope.Body.Section1.Items.Item[8].Length));
                    template.Append("C".PadRight(envelope.Body.Section1.Items.Item[9].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[10].Length));
                    template.Append("JRC".PadRight(envelope.Body.Section1.Items.Item[11].Length));
                    template.Append("ABI".PadRight(envelope.Body.Section1.Items.Item[12].Length));
                    //template.Append((_FileName.Length < 15 ? (_FileName).PadRight(envelope.Body.Section1.Items.Item[13].Length) : (_FileName).Substring(0, 15).PadRight(envelope.Body.Section1.Items.Item[13].Length)));
                    template.Append(transNo.PadRight(envelope.Body.Section1.Items.Item[13].Length));
                    //template.Append(transNo.PadRight(envelope.Body.Section1.Items.Item[14].Length));
                    template.Append((singleInv ? transNo : items.ItemCode).PadRight(envelope.Body.Section1.Items.Item[14].Length));
                    template.Append(list.transHdr.Date.ToString("yyyyMMdd").PadRight(envelope.Body.Section1.Items.Item[15].Length));
                    template.Append(list.transHdr.Date.ToString("yyyy0MM").PadRight(envelope.Body.Section1.Items.Item[16].Length));
                    template.Append(list.transHdr.Date.ToString("yyyyMMdd").PadRight(envelope.Body.Section1.Items.Item[17].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[18].Length));
                    //template.Append("000000000".PadRight(envelope.Body.Section1.Items.Item[19].Length));
                    //template.Append("00000000".PadRight(envelope.Body.Section1.Items.Item[20].Length));
                    //template.Append("0000000".PadRight(envelope.Body.Section1.Items.Item[21].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[19].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[20].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[21].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[22].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[23].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[24].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[25].Length));
                    //template.Append("000000000000000000".PadRight(envelope.Body.Section1.Items.Item[26].Length));
                    //template.Append("000000000000000000".PadRight(envelope.Body.Section1.Items.Item[27].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[26].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[27].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[28].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[29].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[30].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[31].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[32].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[33].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[34].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[35].Length));
                    //template.Append("".PadRight(envelope.Body.Section1.Items.Item[36].Length));
                    template.Append(list.transHdr.TransReasonCode.PadRight(envelope.Body.Section1.Items.Item[36].Length));
                    template.Append("00040".PadRight(envelope.Body.Section1.Items.Item[37].Length));
                    template.Append((_PaymentMethod.Length < 15 ? _PaymentMethod.PadRight(envelope.Body.Section1.Items.Item[38].Length) : (_PaymentMethod).Substring(0, 15).PadRight(envelope.Body.Section1.Items.Item[38].Length)));
                    template.Append(list.transHdr.CustomerNo.PadRight(envelope.Body.Section1.Items.Item[39].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[40].Length));
                    //template.Append((_ReferencePayment.Length < 15 ? _ReferencePayment.PadRight(envelope.Body.Section1.Items.Item[41].Length) : (_ReferencePayment).Substring(0, 15).PadRight(envelope.Body.Section1.Items.Item[41].Length)));
                    template.Append(projectCode.PadRight(envelope.Body.Section1.Items.Item[41].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[42].Length));
                    template.Append("".PadRight(envelope.Body.Section1.Items.Item[43].Length));
                    //template.Append("XD".PadRight(envelope.Body.Section1.Items.Item[43].Length));
                    //template.Append("OS".PadRight(envelope.Body.Section1.Items.Item[44].Length));
                    template.Append((string.IsNullOrEmpty(taxCatCode) ? "OS" : taxCatCode).PadRight(envelope.Body.Section1.Items.Item[44].Length));
                    template.Append(list.transHdr.Date.ToString("yyyyMMdd").PadRight(envelope.Body.Section1.Items.Item[45].Length));
                    template.Append(it == icount ? ("") : singleInv ? ("") : ("\n"));
                    //template.Append("\n");
                    it++;
                    i++;
                }
                #endregion

                #endregion

                var data = template.ToString();

                return new AjaxResponse(data);
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }

        #region Report
        public async Task<AjaxResponse> GetTransactionReport(TransReport input)
        {
            try
            {
                var transReportLogic = new PAYMENT(_env, _connectionString);
                transReportLogic.Select<MapTransactionReport>(x =>
                    new
                    {
                        x.AccountCode,
                        x.Exported,
                        x.TransStatus,
                        x.CompanyName,
                        x.TransDate,
                        x.TransNo,
                        x.InvoiceNo,
                        x.PaymentMethod,
                        x.Amount
                    });

                //input.Start = DateTime.ParseExact(input.Start.Substring(0, 8), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture).ToString();
                //input.End = DateTime.ParseExact(input.End.Substring(0, 8), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture).ToString();
                input.TransNo = string.IsNullOrEmpty(input.TransNo) ? string.Empty : input.TransNo;
                input.CustID = string.IsNullOrEmpty(input.CustID) ? string.Empty : input.CustID;
                input.Amount = string.IsNullOrEmpty(input.Amount) ? string.Empty : input.Amount;

                //TransReport filter = new TransReport();
                //filter.TransNo = string.Empty;
                //filter.Start = "2019-01-01";
                //filter.End = "2020-12-31";
                //filter.Status = 0;
                //filter.Posted = 2;
                //filter.Amount = "0";
                //filter.StartPost = "2019-01-01";
                //filter.EndPost = "2020-12-31";
                //filter.CustID = string.Empty;
                //filter.Type = 2;

                var data = transReportLogic.StoredProcedureView<TransReport, MapTransactionReport>("dbo.rpt_TransactionReport", input);
                data.TotalCount = data.Items.Count();

                return new AjaxResponse(data);
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AccountingManager), ex);
                throw ex;
            }
        }
        #endregion
        //public async Task<AjaxResponse> SuspendCustomer(Accounting.Model.SuspendCustomerRequest input)
        //{
        //    #region suspend
        //    string contCustomerID = input.CustomerID;
        //    int contStatus = input.Status;
        //    int contScheduleStatus = 2200;
        //    var dateNow = DateTime.Now;
        //    var randomStr = dateNow.ToString("yyyyMMdd") + GeneratorHelper.GenerateRandomString(8);
        //    var contSubType = "ACT";
        //    var contResult = "Active";

        //    //status = 2 | 2016 : Suspend
        //    //status = 4 | 2017 : Credit Suspend

        //    try
        //    {
        //        #region proses
        //        var bizLocateLogic = new Core.Entity.Repo.BIZLOCATE(_env, _connectionString);
        //        var bizLocateRes = bizLocateLogic.Retrieve(x => x.BizLocID == contCustomerID);
        //        if (bizLocateRes == null)
        //            return new AjaxResponse(new { success = false, message = "Customer " + contCustomerID + " is not exist" });

        //        if (bizLocateRes.Active != 1)
        //            return new AjaxResponse(new { success = false, message = "Customer " + contCustomerID + " is not active" });

        //        var bizEntityLogic = new Core.Entity.Repo.BIZENTITY(_env, _connectionString);
        //        var bizEntityRes = bizEntityLogic.Retrieve(x => x.BizRegID == contCustomerID);
        //        if (bizEntityRes == null)
        //            return new AjaxResponse(new { success = false, message = "Customer " + contCustomerID + " is not exist" });

        //        bizEntityRes.Status = Convert.ToByte(contStatus);
        //        bizEntityRes.LastUpdate = DateTime.Now;
        //        bizEntityRes.SyncLastUpd = DateTime.Now;

        //        bizLocateRes.Status = Convert.ToByte(contStatus);
        //        bizLocateRes.LastUpdate = DateTime.Now;
        //        bizLocateRes.SyncLastUpd = DateTime.Now;

        //        var contStatusStr = contStatus.ToString();
        //        var codemasterLogic = new Core.General.Repo.CODEMASTER(_env, _connectionString);
        //        var codemasterRes = codemasterLogic.Retrieve(x => x.CodeType == "BZS" && x.Code == contStatusStr);
        //        if (codemasterRes == null)
        //            return new AjaxResponse(new { success = false, message = "Invalid status" });

        //        contSubType = codemasterRes.CodeRef;
        //        contResult = codemasterRes.CodeDesc;

        //        if (contStatus == 2)
        //        {
        //            contScheduleStatus = 2016;
        //        }
        //        else if (contStatus == 4)
        //        {
        //            contScheduleStatus = 2017;
        //        }

        //        //Remark sek ben gak deadlock
        //        //var procTaskLogic = new Accounting.Repo.PROCTASK(_env, _connectionString);
        //        //var procTaskData = new DTO.Accounting.PROCTASK
        //        //{
        //        //    ProSegID = randomStr,
        //        //    TaskID = Convert.ToInt64(contScheduleStatus),
        //        //    AgentID = input.TransporterID,
        //        //    TaskType = Convert.ToByte(contStatus),
        //        //    SubType = contSubType,
        //        //    TaskStartDate = dateNow.Date,
        //        //    TaskEndDate = dateNow.Date,
        //        //    TaskValue1 = bizEntityRes.BizRegID,
        //        //    TaskValue2 = "",
        //        //    Status = Convert.ToByte(contStatus),
        //        //    Result = contResult,
        //        //    BatchNo = "",
        //        //    TransID = "",
        //        //    RecordLocator = "",
        //        //    QueueCode = "",
        //        //    CreatedDate = dateNow,
        //        //    EmailType = 0,
        //        //    EmailAddress = bizEntityRes.Email,
        //        //    ExpiryDate = dateNow.AddDays(30).Date,
        //        //    Currency = "",
        //        //    BalanceDue = 0,
        //        //    PaymentAmt = 0,
        //        //    TransTotalAmt = 0,
        //        //    AttemptCountSender = 0,
        //        //    IsSuccess = 1,
        //        //    FinishedDate = dateNow.AddDays(30).Date,
        //        //    FailedRemark = "",
        //        //    ApprovedBy = input.UserID,
        //        //    ApprovedDate = dateNow,
        //        //    UpdatedBy = "",
        //        //    UpdatedDate = dateNow,
        //        //    IsDeleted = 0,
        //        //    SyncCreate = dateNow,
        //        //    SyncLastUpd = dateNow,
        //        //    Flag = 1,
        //        //    AttemptCountSenderDate = dateNow,
        //        //    MsgID = "",
        //        //};

        //        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //        {
        //            try
        //            {
        //                //await procTaskLogic.Create(procTaskData); //Remark sek ben gak deadlock
        //                await bizEntityLogic.Update(bizEntityRes);
        //                await bizLocateLogic.Update(bizLocateRes);
        //                scope.Complete();
        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //        }
        //        #endregion

        //        //Remark sek ben gak deadlock
        //        //var scheduleHdrLogic = new Repo.AccountingSCHEDULEHDR(_env, _connectionString);
        //        //var scheduleHdrRes = new List<DTO.Questil.Contract.SCHEDULEHDR>();
        //        //if (contStatus != 1)
        //        //{
        //        //    dateNow = dateNow.Date;
        //        //    scheduleHdrRes = scheduleHdrLogic.RetrieveAll(x => x.CustomerID == contCustomerID && x.PlannedStartDate >= dateNow && x.Status == 2200 && x.Flag == 1);
        //        //}
        //        //else
        //        //{
        //        //    scheduleHdrRes = scheduleHdrLogic.RetrieveAll(x => x.CustomerID == contCustomerID && (x.Status == 2016 || x.Status == 2017) && x.Flag == 1);
        //        //}
        //        //if (scheduleHdrRes != null && scheduleHdrRes.Count() > 0)
        //        //{
        //        //    using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //        //    {
        //        //        try
        //        //        {
        //        //            foreach (var index in scheduleHdrRes)
        //        //            {
        //        //                index.Status = contScheduleStatus;
        //        //                index.LastUpdate = DateTime.Now;
        //        //                index.SyncLastUpd = DateTime.Now;
        //        //                await scheduleHdrLogic.Update(index);
        //        //            }
        //        //            scope.Complete();
        //        //        }
        //        //        catch (Exception ex)
        //        //        {
        //        //            throw ex;
        //        //        }
        //        //    }
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    #endregion
        //    return new AjaxResponse(new { success = true, message = "" });
        //}
        #endregion
        
    }
}