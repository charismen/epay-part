export class ProxyURL {

    //#region Notifications
    static readonly RegisterConfirmationEmail = '/api/Sample/RegisterConfirmEmail?';
    static readonly ActivatedEmail = '/api/Sample/ActivatedEmail?';
    static readonly RegisterUserEmail = '/api/Entity/RegisterUser?';
    static readonly RegisterUserEmailNotif = '/api/Entity/RegisterUserMailNotification?';
    static readonly ForgotPassword = '/api/Entity/ForgotPassword?';
    //#endregion

    //#region Cookies
    static readonly GetCompanyCookies = '/api/Entity/GetCompanyCookies?';
    //#endregion

    //#region AccountManager
    static readonly GetDashboardInvoice = '/api/Accounting/GetDashboardInvoice?';
    static readonly GetInvoice = '/api/Accounting/GetInvoice?';
    static readonly GetInvoiceDetail = '/api/Accounting/GetInvoiceDetails?';
    static readonly GetCostumerCombo = '/api/Entity/GetCustomerCombo?';
    static readonly GetBillTypeCombo = '/api/General/GetBillTypeCombo?';
    // static readonly GetAllOutStandingBill = '/api/Accounting/GetAllOutStandingBillByCustomerCode?';
    static readonly JobGetAllOutstanding = '/api/Accounting/JobGetAllOutstanding?';
    // static readonly GetProformaBillById = '/api/Accounting/GetProformaBillById?';
    // static readonly GetCreditBillById = '/api/Accounting/GetCreditBillById?';
    static readonly GetCustomer = '/api/Accounting/GetCustomer?';
    // static readonly UpdatePayment = '/api/Accounting/UpdatePayment?';
    static readonly UpdateTransaction = '/api/Accounting/UpdateTransaction?';
    static readonly JobUpdateTransaction = '/api/Accounting/JobUpdateTransaction?';
    static readonly ValidateFPXStatus = '/api/Accounting/ValidateFPXStatus?';
    static readonly ValidateMBBStatus = '/api/Accounting/validateMBBStatus?';
    static readonly GetFPXBankList = '/api/Accounting/GetFPXBankList?';
    static readonly SaveBankListToJson = '/api/Accounting/SaveBankListToFile?';
    static readonly GetFPXCheckSum = '/api/Accounting/GetFPXCheckSum?';
    // static readonly AddFPXPayment = '/api/Accounting/AddFPXPayment?';
    static readonly AddTransaction = '/api/Accounting/AddTransaction?';
    static readonly GetTransactionHistory = '/api/Accounting/GetTransactionHistory?';
    static readonly GetPaymentHistoryDetails = '/api/Accounting/GetPaymentHistoryDetails?';
    static readonly GetDetailTransaction = '/api/Accounting/GetDetailTransaction?';
    static readonly GetPaymentMethodCombo = '/api/Accounting/GetPaymentMethodCombo?';
    static readonly GetResponseDetails = '/api/Accounting/GetResponseDetails?';
    static readonly GetMBBSignature = '/api/Accounting/GetMBBSignature?';
    static readonly GetAllEmpBranch = 'api/Accounting/GetAllEmpBranch?';
    static readonly GetTenderList = '/api/Accounting/GetTenderList?';
    static readonly GetTxt = '/api/Accounting/GetTxt?';
    static readonly GetTextFileString = '/api/Accounting/GetTextFileString?';
    static readonly GetTender = '/api/Inventory/GetTender?';
    static readonly GetTransactionReport = '/api/Accounting/GetTransactionReport?';
    static readonly InvoiceMailNotif = '/api/Accounting/PaymentSuccessNotification?';
    static readonly ResolveTransNo = '/api/Accounting/ResolveTransNo?';
    //#endregion

    //#region CompanyManager
    static readonly GetLGMCompany = '/api/Entity/GetLGMCompany?';
    static readonly GetCompany = '/api/Entity/GetCompany?';
    static readonly GetEmployee = '/api/Entity/GetEmployee?';
    static readonly GetCompanyLocation = '/api/Entity/GetCompanyLocation?';
    static readonly AddCompany_A = '/api/Entity/AddCompany_A?';
    static readonly AddEmployee = '/api/Entity/AddEmployee?';
    static readonly AddEmpBranch_A = '/api/Entity/AddEmpBranch_A?';
    static readonly AddCompanyLocation = '/api/Entity/AddCompanyLocation?';
    static readonly UpdateEmployee_A = '/api/Entity/UpdateEmployee_A?';
    static readonly UpdateCompany_A = '/api/Entity/UpdateCompany_A?';
    static readonly UpdateCompany = '/api/Entity/UpdateCompany?';
    static readonly GetAllEmployee = '/api/Entity/GetAllEmployee?';
    static readonly SelfRegCompany = '/api/Entity/SelfRegisterCompany?';
    static readonly GetCompanyDetail = '/api/Entity/GetCompanyDetail?';
    static readonly CheckCustomer = '/api/Entity/CheckRegisteredCustomer?';
    static readonly GetMember = '/api/Entity/GetMember?';
    static readonly GetBizHistory = '/api/Entity/GetBizHistory?';
    static readonly GetUserLogin = '/api/Entity/GetUserLogin?';
    static readonly UpdateCustomerOne = '/api/Entity/UpdateCustomerOne?';
    static readonly GetCompanyDocument = '/api/Entity/GetCompanyDocument?';
    static readonly InvoiceNotification = '/api/Entity/InvoiceNotification?';
    // static readonly InvoiceMailNotif = '/api/Entity/InvoiceMailNotification?';
    static readonly GetCustomerList = '/api/Entity/GetCustomerList?';
    static readonly GetCustomerList_A = '/api/Entity/GetCustomerList_A?';
    static readonly UpdateCustomer = '/api/Entity/UpdateCustomer?';
    static readonly GetUsersCombo = '/api/Entity/GetUsersCombo?';
    static readonly GetUsrGroupCombo = '/api/Entity/GetUsrGroupCombo?';
    static readonly CreateEditEmployee = '/api/Entity/CreateEditEmployee?';
    static readonly CreateToAssignEmployee = '/api/Entity/CreateToAssignEmployee?';
    static readonly ManualActivatingCompany = '/api/Entity/ManualActivatingCompany?';
    static readonly CreateTender = '/api/Inventory/CreateTender?';
    static readonly UpdateTender = '/api/Inventory/UpdateTender?';
    //#endregion

    //#region LocationManager
    static readonly GetCountry = '/api/Location/GetCountry?';
    static readonly GetStateList = '/api/Location/GetStateList?';
    //#region CITY
    static readonly GetCity = '/api/Location/GetCity?';
    static readonly DeleteCity = '/api/Location/DeleteCity?';
    static readonly GetCityCombo = '/api/Location/GetCityCombo?';
    static readonly GetPBT = '/api/Location/GetPBT?';
    //#endregion

    //#region Location
    static readonly GetCountryCombo = '/api/Location/GetCountryCombo?';
    static readonly GetState = '/api/Location/GetState?';
    static readonly GetArea = '/api/Location/GetArea?';
    //#endregion

    //#region General
    static readonly GetCodeMaster = '/api/General/GetCodeMasterCombo?';
    static readonly GetListMaintenance = '/api/General/GetListMaintenance?';
    static readonly AddSysPrefB = '/api/General/AddSYSPREFB?';
    static readonly DeleteSysPrefB = '/api/General/DeleteSYSPREFB?';

    //#endregion

    //#region company
    static readonly SuspendCustomer = '/api/Contract/SuspendCustomer?';
    static readonly GetVehicleSchedule = '/api/Contract/GetVehicleSchedule?';
    static readonly AddContractRegistration = '/api/Contract/AddContractRegistration?';
    //#endregion

    static readonly FileUpload = '';
    static readonly GetItemCombo = '/api/Inventory/GetItemCombo?';
    static readonly GetTenderItemCombo = '/api/Inventory/GetTenderItemCombo?';

    //#region HistoryManager
    static readonly GetHistoryHDR = '/api/History/GetHistoryHDR?';
    static readonly GetHistoryDTL = '/api/History/GetHistoryDTL?';
    static readonly GetHistoryActionCombo = '/api/History/GetHistoryActionCombo?';
    static readonly GetHistoryUserCombo = '/api/History/GetHistoryUserCombo?';
    static readonly GetCompanyHistoryDTL = '/api/Entity/GetCompanyHistoryDTL?';
    //#endregion
}
