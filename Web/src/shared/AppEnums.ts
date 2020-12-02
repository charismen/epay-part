import { Injectable } from '@angular/core';

export class AppEditionExpireAction {
    static DeactiveTenant = 'DeactiveTenant';
    static AssignToAnotherEdition = 'AssignToAnotherEdition';
}

export enum SaveType {
    Void = 0,
    Delete = -1,
    Insert = 1,
    Update = 2
}

export enum InvoiceType {
    CreditBill = 1,
    ProformaBill = 2,
    CashBill = 3
}

export enum GalleryType {
    Channel = 1
}

export enum ApprovalStatus {
    Pending = 0,
    Approve = 1,
    Reject = 2,
}

export enum PrintToPdf {
    Invoice = 1,
    Receipt = 2,
    CreditBill = 3,
    ProformaBill = 4,
    CashBill = 5,
    SingleReceipt = 6
}

export enum TransactionStatus {
    Void = 0,
    Pending = 1,
    Paid = 2,
    Cancel = 3
}

export enum AffiliateType {
    AEONGroup = 0,
    Others = 1
}

@Injectable()
export class InvoiceCont {
    Invoice: any[] = [];
}

@Injectable()
export class BizRegBizLoc {
    BizRegID: string;
    BizLocID: string;
}

@Injectable()
export class DashboardFilter {
    InvoiceType: string;
    Customer: string;
    BillingDateFrom: Date;
    BillingDateTo: Date;
}

@Injectable()
export class InvoiceFilter {
    InvoiceType: string;
    Customer: string;
    BillingDateFrom: Date;
    BillingDateTo: Date;
}
