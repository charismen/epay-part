import { ProxyURL } from '@shared/service-proxies/generic-service-proxies-url';
import { GenericServiceProxy, RequestType } from '@shared/service-proxies/generic-service-proxies';
import { AfterViewInit, Component, Injector, ViewChild, ViewEncapsulation, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { LazyLoadEvent } from 'primeng/public_api';
import { Router } from '@angular/router';
import * as _ from 'lodash';
import { BaseListComponent } from '@app/shared/form/list/base-list.component';
import { InvoiceCont } from '@shared/AppEnums';
import { Decimal } from 'decimal.js';

import * as moment from 'moment';
import { finalize } from 'rxjs/internal/operators/finalize';
import { PaymentSummaryModalComponent } from './payment-summary-modal.component';
import { AppConsts } from '@shared/AppConsts';
import { OfflinePaymentModalComponent } from './offline-payment.component';
import { HttpClient } from '@angular/common/http';

class QuickPaymentInterface {
  docno: string;
  duedate: string;
  status: string;
  doctype: string;
  amount: string;
}

@Component({
  templateUrl: './quickpayment.component.html',
  styleUrls: ['./quickpayment.component.less'],
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()]
})
export class QuickPaymentComponent extends AppComponentBase implements OnInit, AfterViewInit {
  @ViewChild('invoiceListComponent', { static: false }) invoiceListComponent: BaseListComponent;
  @ViewChild('paymentSummaryModal', { static: true }) paymentSummaryModal: PaymentSummaryModalComponent;
  @ViewChild('offlinePaymentModal', { static: true }) offlinePaymentModal: OfflinePaymentModalComponent;

  gridUrl = ProxyURL.GetDashboardInvoice;
  quickPaymentInterface: QuickPaymentInterface[] = [];
  isPageLoading: boolean;
  selectedInvoice: any;
  totalSelected: number;
  totalSelectedAmount: number;

  inputHelper: any = {};
  transDtlHdr: any[] = [];
  itemDtlList: any[] = [];

  urlGet: string;
  urlMulti: string;

  InvoiceTitle: string;
  billNo: string;
  id: string;

  payButton = true;

  isLoading = false;
  bankListShow = false;
  personalBank: any[] = [];
  corpBank: any[] = [];
  bankListModel: any;
  bankLoading: boolean;

  payModel: any;  fpxModel: any = {};
  maybankModel: any = {};
  invoiceModel: any;
  selectedPayment: string;
  offlineInfoList: any[] = [];

  payment: any = {};
  checkSum: string;
  type: string;
  invalidInvoice = false;

  Rounding: number;
  sstAmt = 0;
  x: any;
  y: any;

  paying = false;
  term = false;
  maxTotal = false;

  dateToday = moment(new Date()).format('YYYYMMDDHHmmss');

  constructor(
    injector: Injector,
    private _proxy: GenericServiceProxy,
    private _router: Router,
    private _invoiceCont: InvoiceCont,
    private _httpService: HttpClient
  ) {
    super(injector);
  }

  ngOnInit() {
    this.checkFPXBankList();

    this.gridUrl += 'MaxResultCount=' + encodeURIComponent('' + 100) + '&';
    this.gridUrl += 'customerCode=' + encodeURIComponent(this.appStorage.bizRegID) + '&';
    this._proxy.request(this.gridUrl, RequestType.Get)
      .subscribe(data => {
        //console.log('First 4 Data: ' + JSON.stringify(data.items));
        this.billNo = 'INV' + moment(new Date()).format('YYMMDDHHmm');
        this.selectedInvoice = data.items;
        this.totalSelected = data.items.length;
        this.totalSelectedAmount = data.items.map(x => parseFloat(x.Balance)).reduce((a, b) => a + b, 0);

        this.urlGet += 'billId=' + encodeURIComponent(this.id) + '&';
        this.populateMultiData();
      });
  }

  async ngAfterViewInit() {
    this.invoiceListComponent.primengTableHelper.defaultRecordsCountPerPage = 100;
    await this.checkPaymentType();
  }

  refresh(): void {
  }

  checkFPXBankList() {
    this._httpService.get(this.appUrlService.appRootUrl + 'assets/qstl/banklist.json')
      .subscribe((result) => {
        let res = JSON.parse(JSON.stringify(result));
        console.log('FPX Bank List: ' + res);
        let lastCheck = moment(res.LastCheck);
        let hours = moment().diff(lastCheck, 'hours');
        if ((res.PersonalBank.length === 0 || res.CorporateBank.length === 0) || (res.LastCheck === null || hours >= 24)) {
          this.getBankList();
        } else {
          this.personalBank = res.PersonalBank;
          this.corpBank = res.CorporateBank;
        }
      });
  }

  getBankList() {
    this.isLoading = true;
    this.bankLoading = true;

    let url = ProxyURL.SaveBankListToJson;

    if (url !== undefined) {
      this._proxy.request(url, RequestType.Post, '')
        .pipe(finalize(() => {
          this.isLoading = false;
          this.bankLoading = false;
        }))
        .subscribe(result => {
          if (result.success === true) {
            this.notify.success(this.l('BankListUpdated'));
            this.checkFPXBankList();
            this.payButton = true;
          } else {
            this.notify.success(this.l('BankListUpdateFailed'));
            this.payButton = true;
          }
        });
    }
  }

  bankListChange(bank?: any): void {
    this.setFPXDetails(this.payModel);
    this.fpxModel.fpx_buyerBankId = bank.bankcode;
    this.fpxModel.fpx_buyerBankBranch = '';

    let url = ProxyURL.GetFPXCheckSum;
    if (url !== undefined) {
      this._proxy.request(url, RequestType.Post, this.fpxModel)
        .subscribe((result) => {
          this.fpxModel.fpx_checkSum = result.checksum;
          this.checkSum = result.checksumData;
          this.payButton = false;
          this.term = true;
        });
    }
  }

  selectBank(msgToken: string): void {
    this.isLoading = true;
    this.payButton = true;
    this.term = false;
    this.bankListModel = [];

    if (msgToken === '01' && this.totalSelectedAmount <= 30000) {
      this.maxTotal = false;
      this.bankListShow = true;
      this.bankListModel = this.personalBank;
      this.fpxModel.fpx_msgToken = '01';
      this.isLoading = false;
    } else if ( msgToken === '02' && this.totalSelectedAmount <= 30000) {
      this.maxTotal = false;
      this.bankListShow = true;
      this.bankListModel = this.corpBank;
      this.fpxModel.fpx_msgToken = '02';
      this.isLoading = false;
    } else if (msgToken === '02' && this.totalSelectedAmount >= 30000) {
      this.maxTotal = false;
      this.bankListShow = true;
      this.bankListModel = this.corpBank;
      this.fpxModel.fpx_msgToken = '02';
      this.isLoading = false;
    } else {
      this.term = false;
      this.bankListShow = false;
      this.maxTotal = true;
      this.isLoading = false;
    }
  }

  populateMultiData() {
    let itemList: any[] = [];
    //this.primengTableHelper.records = this._invoiceCont.Invoice;
    this.transDtlHdr = this.selectedInvoice;
    this.inputHelper.billno = 'INV' + moment(new Date()).format('YYMMDDHHmm');
    this.urlMulti = ProxyURL.GetInvoiceDetail;

    if (this.selectedInvoice[0].Type === 'Credit Bill') {
      this.InvoiceTitle = 'Invoice';
    } else if (this.selectedInvoice[0].Type === 'Proforma Bill') {
      this.InvoiceTitle = 'Invoice';
    }

    this.selectedInvoice.forEach(res => {
      //this.checkInvalidInvoice(res.SiNumber);
      res.Rounding = this.roundingDtl(new Decimal(res.Amount).toFixed(2));
    });

    this.urlMulti += 'transNo=' + encodeURIComponent(this.selectedInvoice[0].SiNumber) + '&';
    this._proxy.request(this.urlMulti, RequestType.Get)
      .subscribe(result => {
        this.inputHelper.attn = result.data.Attention !== '' ? result.data.Attention : result.data.ContactPerson;
        this.inputHelper.address = result.data.Address1 + ' ' + result.data.Address2 + ' ' + result.data.Address3 + ' ' + result.data.Address4;
        this.inputHelper.customer = result.data.Company;
        this.inputHelper.customerno = result.data.AcctNo;
        this.inputHelper.taxcode = result.items.items[0].TaxCode.substr(-3, 2);
        this.inputHelper.billdate = moment(new Date()).format('DD/MM/YYYY');

        this.inputHelper.creditvote = result.data.CreditVote === '' ? '-' : result.data.CreditVote;
        this.inputHelper.projectcode = result.data.ProjectCode === '' ? '-' : result.data.ProjectCode;
        this.inputHelper.refno = result.data.RefNo === '' ? '-' : result.data.RefNo;
        this.inputHelper.costcenter = result.data.CostCenter === '' ? '-' : result.data.CostCenter;
        this.inputHelper.deliveryref = result.data.DeliveryRef === '' ? '-' : result.data.DeliveryRef;
        this.inputHelper.ourref = result.data.OurRef === '' ? '-' : result.data.OurRef;
        this.inputHelper.yourref = result.data.YourRef === '' ? '-' : result.data.YourRef;

        this.payModel = result;
      });

    // this.inputHelper.creditvote = '-';
    // this.inputHelper.projectcode = '-';
    // this.inputHelper.refno = '-';
    // this.inputHelper.deliveryref = '-';
    // this.inputHelper.ourref = '-';
    // this.inputHelper.yourref = '-';

    let subTot = this.selectedInvoice.map(x => x.Amount).reduce((a, b) => a + b, 0);
    let sst = this.selectedInvoice.map(x => x.Tax).reduce((a, b) => a + b, 0);
    //console.log('subTot: ' + subTot + ' & sst: ' + sst);
    this.x = new Decimal(subTot);
    this.inputHelper.sst = new Decimal(sst).toFixed(2);
    // this.inputHelper.subtotal = (this.x.sub(this.inputHelper.sst).toFixed(2));
    this.inputHelper.subtotal = this.x.toFixed(2);
    this.sstAmt = 6;
    this.inputHelper.total = (this.x.add(this.inputHelper.sst)).toFixed(2);
    this.rounding(this.inputHelper.total);
    this.inputHelper.totalamountdue = this.inputHelper.total;
  }

  rounding(total: any) {
    let res = (this.inputHelper.total).toString().slice(-1);
    if (res == 1 || res == 6) {
      this.inputHelper.valueRounding = -0.01;
      // this.inputHelper.total = ((((this.x.sub(this.inputHelper.sst)).add(this.inputHelper.sst))).sub(0.01)).toFixed(2);
      this.inputHelper.total = ((this.x.add(this.inputHelper.sst)).sub(0.01)).toFixed(2);
    } else if (res == 2 || res == 7) {
      this.inputHelper.valueRounding = -0.02;
      // this.inputHelper.total = ((((this.x.sub(this.inputHelper.sst)).add(this.inputHelper.sst))).sub(0.02)).toFixed(2);
      this.inputHelper.total = ((this.x.add(this.inputHelper.sst)).sub(0.02)).toFixed(2);
    } else if (res == 3 || res == 8) {
      this.inputHelper.valueRounding = 0.02;
      // this.inputHelper.total = ((((this.x.sub(this.inputHelper.sst)).add(this.inputHelper.sst))).add(0.02)).toFixed(2);
      this.inputHelper.total = ((this.x.add(this.inputHelper.sst)).add(0.02)).toFixed(2);
    } else if (res == 4 || res == 9) {
      this.inputHelper.valueRounding = 0.01;
      // this.inputHelper.total = ((((this.x.sub(this.inputHelper.sst)).add(this.inputHelper.sst))).add(0.01)).toFixed(2);
      this.inputHelper.total = ((this.x.add(this.inputHelper.sst)).add(0.01)).toFixed(2);
    } else {
      this.inputHelper.valueRounding = 0.00;
    }
  }

  roundingDtl(total: any) {
    let res = (total).toString().slice(-1);
    let x = 0.00;
    if (res == 1 || res == 6) {
      x = -0.01;
    } else if (res == 2 || res == 7) {
      x = -0.02;
    } else if (res == 3 || res == 8) {
      x = 0.02;
    } else if (res == 4 || res == 9) {
      x = 0.01;
    } else {
      x = 0.00;
    }
    return x;
  }

  checkInvalidInvoice(siNumber?: string) {
    let result = false;
    if (siNumber == null) {
      if (this.inputHelper.totalamountdue !== this.inputHelper.total) {
        result = true;
      }
    } else {
      let url = ProxyURL.GetInvoiceDetail + 'transNo=' + encodeURIComponent(siNumber) + '&';
      if (url !== undefined) {
        this._proxy.request(url, RequestType.Get)
          .subscribe((res) => {
            let subtotal = res.items.items.map(x => x.SubTotal).reduce((a, b) => a + b, 0);
            let sst = res.data.TaxAmt;
            let total = subtotal + sst;
            let totalAmt = res.data.Amount;

            if (total !== totalAmt) {
              result = true;
              this.message.warn(this.l('InvalidInvoiceMessage')).then(() => {
                this.invalidInvoice = true;
              });
            }
          });
      }
    }
    return result;
  }

  quickpayment(event?: LazyLoadEvent): void {
    this.primengTableHelper.records = this.quickPaymentInterface;
    //this.statuscolor = this.paymentInterface[status] === 'overdue' ? 'kt-badge kt-badge--inline kt-badge--danger' : 'kt-badge kt-badge--inline kt-badge--warning' ;
    this.isPageLoading = false;
  }

  proceed(): void {
    this.paying = false;
    this.saveTransaction();
    this.paymentSummaryModal.show(this.payment, this.fpxModel, this.selectedPayment, this.inputHelper.totalamountdue);
  }

  offlinePayment(): void {
    this.payButton = false;
  }

  saveTransaction(): void {
    let transHdr: any = {};
    let transDtlList: any[] = [];
    let transTender: any = {};

    this.payment = {
      TransHdr: transHdr,
      TransDtl: transDtlList,
      SiNumber: '',
      TenderID: this.selectedPayment === 'fpxPay' ? transTender.TransId = 997 : this.selectedPayment === 'ccPay' ? transTender.TransId = 996 : transTender.TenderId = 993,
      BankCode: this.fpxModel.fpx_buyerBankId,
      RequestMessage: this.selectedPayment === 'fpxPay' ? this.checkSum : this.selectedPayment === 'ccPay' ? this.checkSum : '',
      //TransDtlHdr: this.transDtlHdr,
      OfflineInfoList: this.offlineInfoList,
      Mode: 1
    };

    //# region HDR
    transHdr.BizRegID = this.payModel.data.BizRegID;
    transHdr.BizLocID = this.payModel.data.BizLocID;
    transHdr.TermID = 1;
    this.selectedPayment === 'fpxPay' ? transHdr.TransNo = this.fpxModel.fpx_sellerOrderNo : transHdr.TransNo = this.dateToday;
    transHdr.TransType = 1;
    transHdr.BillNo = this.payModel.data.BillNo;
    transHdr.ShiftCode = '99'; // should be FPX status
    transHdr.TransDate = moment(new Date()).format('YYYY-MM-DD');
    transHdr.TransAmtRnd = this.inputHelper.valueRounding;
    transHdr.TransChgAmt = this.inputHelper.sst;
    transHdr.TransSubTotal = this.inputHelper.subtotal;
    transHdr.TransAmt = this.inputHelper.total;
    transHdr.CustomerID = this.inputHelper.customerno;
    transHdr.CustPrivilege = ''; // this.fpxModel.fpx_buyerBankId;
    transHdr.CashierID = this.inputHelper.attn;
    transHdr.bizRegID = this.inputHelper.CostumerCode;
    transHdr.CustPkgID = this.payModel.data.YourRef;
    transHdr.ServerID = this.payModel.data.OurRef;
    transHdr.TransDiscReasonCode = this.payModel.data.ProjectCode;
    transHdr.TransDiscRemark = this.payModel.data.CreditVote;
    transHdr.AcctNo = this.payModel.data.RefNo;
    transHdr.TransReasonCode = this.payModel.data.DeliveryRef;
    transHdr.SpDiscReasonCode = this.payModel.data.TaxCatCode;
    transHdr.InSvcID = this.payModel.data.Id;
    transHdr.TransStatus = '99'; // should be FPX status
    transHdr.TblNo = this.payModel.data.SunDB;
    transHdr.Posted = 0;
    transHdr.PostDate = moment(this.inputHelper.billdate, 'DD/MM/YYYY');
    transHdr.Status = 0; // should be FPX status
    transHdr.Flag = 1;

    //# region DTL
    for (let i = 0; i < this.transDtlHdr.length; i++) {
      let transDtl: any = {};
      transDtl.BizRegID = transHdr.BizRegID;
      transDtl.BizLocID = transHdr.BizLocID;
      transDtl.TermID = 1;
      transDtl.TransNo = transHdr.TransNo;
      transDtl.TransSeq = i + 1;
      transDtl.BillNo = this.transDtlHdr[i].BillNo; //transHdr.BillNo;
      this.inputHelper.type === 'Credit Bill' ? transDtl.StkType = 'CDT' : transDtl.StkType = 'PRF';
      transDtl.ItemType = 1;
      transDtl.ItemCode = this.transDtlHdr[i].SiNumber; //this.itemDtlList[i].code;
      transDtl.Qty = 1; //Number(this.itemDtlList[i].quantity);
      transDtl.Remark = this.transDtlHdr[i].Detail; //this.itemDtlList[i].description;
      transDtl.UnitCost = this.transDtlHdr[i].Amount; //Number(this.itemDtlList[i].unitprice);
      transDtl.TolAmt = this.transDtlHdr[i].Amount; //Number(this.itemDtlList[i].amount);
      transDtl.Posted = 0;
      transDtl.Flag = 1;
      transDtl.Status = transHdr.Status;
      transDtl.IsHost = 1;
      let cdVtIdx = (this.inputHelper.type === 'Proforma Bill' && transHdr.TransDiscRemark !== undefined) ? transHdr.TransDiscRemark.indexOf('-') : 0;
      this.inputHelper.type === 'Proforma Bill' ? transDtl.ExCode1 = transHdr.TransDiscRemark.substr(0, cdVtIdx) : transDtl.ExCode1 = ''; //Credit Vote for PRF
      transDtl.BaseRate = this.transDtlHdr[i].Tax; //Should be Tax
      transDtl.CoRate1H = this.transDtlHdr[i].Rounding; //Should be Rounding
      transDtl.SerialNo = this.transDtlHdr[i].Id;
      let cdPjIdx = this.transDtlHdr[i].ProjectCode !== undefined ? this.transDtlHdr[i].ProjectCode.indexOf('-') : 0;
      transDtl.ExCode2 = this.transDtlHdr[i].ProjectCode.substr(0, cdPjIdx);

      transDtlList.push(transDtl);
    }
  }

  selectPaymentChange(selectedPay) {
    this.selectedPayment = selectedPay;
  }

  maybank(): void {
    this.payButton = false;
    this.setMayBankDetails(this.payModel);

    let url = ProxyURL.GetMBBSignature;
    url += 'merchantID=' + encodeURIComponent('' + this.maybankModel.merchant_tranid) + '&';
    url += 'amount=' + encodeURIComponent('' + this.maybankModel.amount) + '&';

    if (url !== undefined) {
      this._proxy.request(url, RequestType.Get)
        .subscribe((result) => {
          this.maybankModel.txn_signature = result.txN_SIGNATURE;
          this.checkSum = result.checkSum;
        });
    }
  }

  setFPXDetails(data: any) {
    this.fpxModel.fpx_msgType = 'AE';
    //this.fpxModel.fpx_msgToken = '01';
    this.fpxModel.fpx_sellerExId = AppConsts.FPX.sellerExId; // 'EX00003925';
    this.fpxModel.fpx_sellerExOrderNo = this.dateToday;
    this.fpxModel.fpx_sellerOrderNo = this.dateToday;
    this.fpxModel.fpx_sellerTxnTime = this.dateToday;
    this.fpxModel.fpx_sellerId = AppConsts.FPX.sellerId; // 'SE00004392';
    this.fpxModel.fpx_sellerBankCode = AppConsts.FPX.sellerBankCode; // '01';
    this.fpxModel.fpx_txnCurrency = AppConsts.FPX.txnCurrency; // 'MYR';
    this.fpxModel.fpx_txnAmount = new Decimal(this.inputHelper.total).toFixed(2);
    this.fpxModel.fpx_buyerEmail = this.appStorage.email; // 'charis@collexe.asia';
    this.fpxModel.fpx_buyerId = this.inputHelper.customerno;
    this.fpxModel.fpx_buyerName = this.inputHelper.customer; //'Charis'; //
    this.fpxModel.fpx_buyerAccNo = '';
    this.fpxModel.fpx_makerName = 'Lembaga Getah Malaysia';
    this.fpxModel.fpx_buyerIban = '';
    this.fpxModel.fpx_productDesc = 'LGM Payment ' + this.inputHelper.billno;
    this.fpxModel.fpx_version = AppConsts.FPX.version; // '6.0';
  }

  setMayBankDetails(data: any) {
    this.maybankModel.mmerchant_acc_no = AppConsts.MayBank.MBBCCMerchantAccount_eL;
    this.maybankModel.amount = new Decimal(this.inputHelper.total).toFixed(2);
    this.maybankModel.transaction_type = '3';
    this.maybankModel.merchant_tranid = this.dateToday;
    this.maybankModel.transaction_id = this.dateToday;
    this.maybankModel.response_type = 'HTTP';
    this.maybankModel.return_url = AppConsts.MayBank.MBBReturnURL;
    this.maybankModel.txn_desc = 'LGM Payment ' + this.inputHelper.billno;
    this.maybankModel.customer_id = this.inputHelper.customerno;
    this.maybankModel.fr_highrisk_email = this.appStorage.email;
    this.maybankModel.fr_highrisk_country = '';
    this.maybankModel.fr_billing_address = '';
    this.maybankModel.fr_shipping_address = '';
    this.maybankModel.fr_shipping_cost = '';
    this.maybankModel.fr_purchase_hour = '';
    this.maybankModel.fr_customer_ip = '';
    this.maybankModel.txn_signature = '';
  }

  paynow(payType?: string): void {
    this.paying = false;
    payType = this.selectedPayment;
    this.saveTransaction();
    if (payType === 'ccPay') {
      this.paymentSummaryModal.show(this.payment, this.maybankModel, this.selectedPayment, this.inputHelper.totalamountdue);
    } else if (payType === 'offPay') {
      this.offlinePaymentModal.show(this.payment);
    }
  }

  // populateMultiData() {
  //   let itemList: any[] = [];
  //   this.primengTableHelper.records = this.selectedInvoice;
  //   this.transDtlHdr = this.selectedInvoice;
  //   this.inputHelper.billno = 'INV' + this.dateToday;

  //   if (this.selectedInvoice[0].Type === 'Credit Bill') {
  //       this.urlMulti = ProxyURL.GetCreditBillById;
  //       this.InvoiceTitle = 'TaxInvoice';
  //   } else if (this.selectedInvoice[0].Type === 'Proforma Bill') {
  //       this.urlMulti = ProxyURL.GetProformaBillById;
  //       this.InvoiceTitle = 'ProformaInvoice';
  //   }

  //   let url = this.urlMulti + 'billId=' + encodeURIComponent(this.selectedInvoice[0].Id) + '&';
  //   this._proxy.request(url, RequestType.Get)
  //       .subscribe(result => {
  //           this.inputHelper.attn = result.items.attention;
  //           this.inputHelper.address = result.items.customerdto.name + ' ' + result.items.customerdto.address;
  //           this.inputHelper.customerno = result.items.customerdto.code;
  //           this.inputHelper.taxcode = result.items.creditvotedto.taxcategorydto.code;

  //           this.payModel = result.items;
  //       });

  //   for (let i = 0; i < this.selectedInvoice.length; i++) {
  //       let urlItem = this.urlMulti + 'billId=' + encodeURIComponent(this.selectedInvoice[i].Id) + '&';

  //       this._proxy.request(urlItem, RequestType.Get)
  //           .subscribe(result => {
  //               for (let e = 0; e < result.items.items.length; e++) {
  //                   let itemDtl: any = {};
  //                   itemDtl.code = result.items.items[e].code;
  //                   itemDtl.quantity = Number(result.items.items[e].quantity);
  //                   itemDtl.description = result.items.items[e].description;
  //                   itemDtl.unitprice = Number(result.items.items[e].unitprice);
  //                   itemDtl.amount = Number(result.items.items[e].amount);
  //                   itemList.push(itemDtl);
  //               }
  //           });
  //   }
  //   this.itemDtlList = itemList;

  //   this.inputHelper.creditvote = '-';
  //   this.inputHelper.projectcode = '-';
  //   this.inputHelper.refno = '-';
  //   this.inputHelper.deliveryref = '-';
  //   this.inputHelper.ourref = '-';
  //   this.inputHelper.yourref = '-';

  //   this.inputHelper.subtotal = this._invoiceCont.Invoice.map(x => x.Amount).reduce((a, b) => a + b, 0);
  //   this.inputHelper.sst = (this.inputHelper.subtotal * 6) / 100;
  //   this.inputHelper.total = this.inputHelper.subtotal + this.inputHelper.sst;
  //   this.inputHelper.totalamountdue = this.inputHelper.total;
  // }

  async checkPaymentType() {
    let payNowController = document.querySelectorAll('.activatePayment');
    for (let a = 0; a < payNowController.length; a++) {
      let curController = payNowController[a],
        targetElement = curController.getAttribute('targetShow'),
        isChecked = curController.hasAttribute('checked'),
        targetPaymentBox = document.querySelector('#' + targetElement);

      if (isChecked) {
        //targetPaymentBox.classList.remove('hideThis');
      }

      curController.addEventListener('click', function () {
        let parentChildToHide = targetPaymentBox.parentNode,
          childrenOfParentToHide = parentChildToHide.querySelectorAll('.boxPayWrp');

        for (let i = 0; i < childrenOfParentToHide.length; i++) {
          if (!childrenOfParentToHide[i].classList.contains('hideThis')) {
            childrenOfParentToHide[i].classList.add('hideThis');
          }
        }
        targetPaymentBox.classList.remove('hideThis');
        if (targetPaymentBox.id.indexOf('fpx') !== -1) { //if fpx payment
          let bankTypes = targetPaymentBox.querySelector('.fpxContentWraper > div:first-child'),
            payBtn = targetPaymentBox.querySelector('button') as HTMLButtonElement,
            radioFPX = targetPaymentBox.querySelectorAll('[type="radio"]');

          //reset radio button
          for (let i = 0; i < radioFPX.length; i++) {
            let curRadio = radioFPX[i] as HTMLInputElement;
            curRadio.addEventListener('click', function () {
              let bankTypeList = document.querySelector('#bankTypeList');

              if (bankTypeList) {
                if (bankTypeList.classList.contains('hideThis')) {
                  bankTypeList.classList.remove('hideThis');
                }
              }

            });
          }

          bankTypes.classList.remove('hideThis');

          payBtn.disabled = true;
        } else { //if non fpx is clicked
          let parentTargetBox = targetPaymentBox.parentElement,
            fpxToReset = parentTargetBox.querySelector('[id^="fpxBoxOption"]'),
            paymentTypeRadio = fpxToReset.querySelectorAll('[type="radio"]');

          //reset radio button
          for (let i = 0; i < paymentTypeRadio.length; i++) {
            let curRadio = paymentTypeRadio[i] as HTMLInputElement;
            curRadio.checked = false;
          }

          //reset display of select to hide
          parentTargetBox.querySelector('#bankTypeList').classList.add('hideThis');
        }
      });

    }
  }
}
