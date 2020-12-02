import { AppComponentBase } from '@shared/common/app-component-base';
import { Component, OnInit, ViewEncapsulation, Injector, ViewChild } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { DatePipe } from '@angular/common';
import { ProxyURL } from '@shared/service-proxies/generic-service-proxies-url';
import { RequestType, GenericServiceProxy } from '@shared/service-proxies/generic-service-proxies';

import * as moment from 'moment';
import { Router } from '@angular/router';
import { PaymentSummaryModalComponent } from './payment-summary-modal.component';
import { InvoiceCont } from '@shared/AppEnums';

@Component({
    selector: 'offlinePaymentModal',
    templateUrl: './offline-payment.component.html',
    styleUrls: ['./offline-payment.component.less'],
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()],
    providers: [DatePipe]
})
export class OfflinePaymentModalComponent extends AppComponentBase implements OnInit {
    @ViewChild('createModal', { static: true }) modal: ModalDirective;
    @ViewChild('paymentSummaryModal', { static: true }) paymentSummaryModal: PaymentSummaryModalComponent;

    active = false;
    paying = false;
    saving = false;

    data: any = [];

    urlGet: string;
    urlMulti: string;
    tender: string;

    InvoiceTitle: string;

    inputHelper: any = {};
    selectedPayment: any;
    paymentCombo: any;
    payModel: any;
    offPayment: any = {};

    selectedBankListType: any;
    bankListCombo: any;
    tempSave: any = {};
    tempData: any = [];
    tempData2: any = [];
    transDtlHdr: any[] = [];

    itemDtlList: any[] = [];

    date: Date;

    dateToday = moment(new Date()).format('YYYYMMDDHHmmss');

    IsCheque = true;
    IsCash = true;
    IsRefNum = true;

    totalAmount: number;

    constructor(
        injector: Injector,
        private _proxy: GenericServiceProxy
    ) {
        super(injector);
    }

    ngOnInit() {
        let urlBankList = ProxyURL.GetCodeMaster;
        let urlPayment = ProxyURL.GetPaymentMethodCombo;
        this._proxy.request(urlPayment, RequestType.Get)
            .pipe().subscribe(result => {
                this.paymentCombo = result;
            });
        this._proxy.request(urlBankList + 'code=' + encodeURIComponent('BNM'), RequestType.Get)
            .pipe().subscribe(result => {
                this.bankListCombo = result;
            });

        this.tempSave.notes = '';
        this.tempSave.paymentMethod = '';
        this.tempSave.clearingDate = '';
        this.tempSave.bankDetails = '';
        this.tempSave.refNum = '';
        this.tempSave.amount = '';
    }

    back(): void {
    }

    show(payment): void {
        console.log('Data: ' + JSON.stringify(payment));
        this.offPayment = payment;
        this.totalAmount = this.offPayment.TransHdr.TransAmt;
        this.tempSave.amount = this.totalAmount;
        this.date = new Date;
        this.modal.show();
        this.active = true;
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }

    onPaymentMethodTypeChange(data: any) {
        this.selectedPayment = data;
        if (data.Code === '000') {
            this.IsCheque = true;
            this.IsCash = false;
            this.IsRefNum = false;
        } else if (data.Code === '180') {
            this.IsCheque = false;
            this.IsCash = true;
            this.IsRefNum = true;
        } else if (data.Code === '300') {
            this.IsCheque = true;
            this.IsCash = true;
            this.IsRefNum = true;
        } else if (data.Code === '995') {
            this.IsCheque = true;
            this.IsCash = true;
            this.IsRefNum = true;
        } else {
            this.IsCheque = true;
            this.IsCash = true;
            this.IsRefNum = true;
        }
    }

    onBankListTypeChange(data: any) {
        this.selectedBankListType = data;
    }

    removeItem(data) {
        let i = this.tempData.indexOf(data);
        this.tempData.splice(i, 1);
        this.totalAmount += data.amount;
        this.tempSave.amount = this.totalAmount;
    }

    saveTemporary(): void {
        let data: any = {};

        if (this.totalAmount <= 0) {
            this.notify.error(this.l('Your total transaction is enough'));
        } else {
            data.paymentMethod = this.selectedPayment.Code;
            data.paymentMethodRemark = this.selectedPayment.Remark.toString();
            data.date = moment(this.date).format('DD/MM/YYYY'); // this.datePipe.transform(this.date, 'dd/MM/yyyy');
            data.bankList = this.selectedBankListType;
            data.refNum = this.tempSave.refNum;
            data.amount = this.tempSave.amount;

            if (this.selectedPayment.Code === '000') {
                // data.date = '';
                data.bankList = '';
                data.refNum = '';
            } else if (this.selectedPayment.Code === '180') {
                data.bankList = this.selectedBankListType.Code;
            } else if (this.selectedPayment.Code === '300') {
                data.bankList = '';
            } else if (this.selectedPayment.Code === '995') {
                data.bankList = '';
            } else {
                data.bankList = '';
            }

            if (data.amount > this.totalAmount) {
                this.notify.error(this.l('Your Amount More Than Total'));
            } else {
                this.totalAmount -= data.amount;
                this.tempData.push(data);
            }
        }

        this.selectedPayment = '';
        this.selectedBankListType = '';
        this.tempSave.notes = '';
        this.tempSave.refNum = '';
        this.tempSave.amount = this.totalAmount;
        this.date = new Date();
    }

    // populateMultiData() {
    //     let itemList: any[] = [];
    //     this.primengTableHelper.records = this._invoiceCont.Invoice;
    //     this.transDtlHdr = this._invoiceCont.Invoice;
    //     this.inputHelper.billno = 'INV' + this.dateToday;
    //     this.urlMulti = ProxyURL.GetInvoiceDetail;

    //     if (this._invoiceCont.Invoice[0].Type === 'Credit Bill') {
    //         // this.urlMulti = ProxyURL.GetCreditBillById;
    //         this.InvoiceTitle = 'TaxInvoice';
    //     } else if (this._invoiceCont.Invoice[0].Type === 'Proforma Bill') {
    //         // this.urlMulti = ProxyURL.GetProformaBillById;
    //         this.InvoiceTitle = 'ProformaInvoice';
    //     }

    //     // let url = this.urlMulti + 'billId=' + encodeURIComponent(this._invoiceCont.Invoice[0].Id) + '&';
    //     this.urlMulti += 'transNo=' + encodeURIComponent(this._invoiceCont.Invoice[0].SiNumber) + '&';
    //     this._proxy.request(this.urlMulti, RequestType.Get)
    //         .subscribe(result => {
    //             this.inputHelper.attn = result.items.attention;
    //             this.inputHelper.address = result.items.customerdto.name + ' ' + result.items.customerdto.address;
    //             this.inputHelper.customerno = result.items.customerdto.code;
    //             this.inputHelper.taxcode = result.items.creditvotedto.taxcategorydto.code;

    //             this.payModel = result.items;
    //         });

    //     for (let i = 0; i < this._invoiceCont.Invoice.length; i++) {
    //         let urlItem = this.urlMulti + 'billId=' + encodeURIComponent(this._invoiceCont.Invoice[i].Id) + '&';

    //         this._proxy.request(urlItem, RequestType.Get)
    //             .subscribe(result => {
    //                 for (let e = 0; e < result.items.items.length; e++) {
    //                     let itemDtl: any = {};
    //                     itemDtl.code = result.items.items[e].code;
    //                     itemDtl.quantity = Number(result.items.items[e].quantity);
    //                     itemDtl.description = result.items.items[e].description;
    //                     itemDtl.unitprice = Number(result.items.items[e].unitprice);
    //                     itemDtl.amount = Number(result.items.items[e].amount);
    //                     itemList.push(itemDtl);
    //                 }
    //             });
    //     }
    //     this.itemDtlList = itemList;

    //     this.inputHelper.creditvote = '-';
    //     this.inputHelper.projectcode = '-';
    //     this.inputHelper.refno = '-';
    //     this.inputHelper.deliveryref = '-';
    //     this.inputHelper.ourref = '-';
    //     this.inputHelper.yourref = '-';

    //     this.inputHelper.subtotal = this._invoiceCont.Invoice.map(x => x.Amount).reduce((a, b) => a + b, 0);
    //     this.inputHelper.sst = (this.inputHelper.subtotal * 6) / 100;
    //     this.inputHelper.total = this.inputHelper.subtotal + this.inputHelper.sst;
    //     this.inputHelper.totalamountdue = this.inputHelper.total;
    // }

    refresh(): void {
    }

    submit(): void {
        let offlinePayment: any = [];

        for (let i = 0; i < this.tempData.length; i++) {
            let offLinePayment: any = {};
            offLinePayment.transNo = this.dateToday;
            offLinePayment.tenderID = this.tempData[i].paymentMethod;
            offLinePayment.clearingDate = this.tempData[i].date;
            offLinePayment.bankDetails = this.tempData[i].bankList;
            offLinePayment.refNumber = this.tempData[i].refNum;
            offLinePayment.amount = this.tempData[i].amount;

            offlinePayment.push(offLinePayment);
        }

        this.offPayment.RequestMessage = 'email';
        this.offPayment.OfflineInfoList = offlinePayment;

        this.modal.hide();
        this.paymentSummaryModal.show(this.offPayment, undefined, 'offPay', this.offPayment.TransHdr.TransAmt);
    }
}
