import { HttpClient } from '@angular/common/http';
import { LocalStorageService } from './../../../shared/utils/local-storage.service';
import { CreateEditCashBillModalComponent } from './create-edit-cashbill-modal.component';
import { Decimal } from 'decimal.js';
import { AppConsts } from '@shared/AppConsts';
import { ProxyURL } from '@shared/service-proxies/generic-service-proxies-url';
import { finalize } from 'rxjs/operators';
import { OfflinePaymentModalComponent } from './../invoice/offline-payment.component';
import { PaymentSummaryModalComponent } from './../invoice/payment-summary-modal.component';
import { InvoiceCont } from '@shared/AppEnums';
import { ActivatedRoute, Router } from '@angular/router';
import { GenericServiceProxy, RequestType } from '@shared/service-proxies/generic-service-proxies';
import { Table } from 'primeng/table';
import { Location } from '@angular/common';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Component, OnInit, ViewEncapsulation, Injector, ViewChild, NgZone, AfterViewInit } from '@angular/core';
import { Observable, combineLatest as _observableCombineLatest, of as _observableOf } from 'rxjs';
//import bankList from '../../../assets/qstl/banklist.json';

import * as moment from 'moment';
// import * as _ from 'lodash';

@Component({
  templateUrl: './cashbill.component.html',
  styleUrls: ['./cashbill.component.less'],
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()]
})
export class CashbillComponent extends AppComponentBase implements OnInit, AfterViewInit {

  @ViewChild('dataTable', { static: false }) dataTable: Table;
  @ViewChild('createOrEditCashBillModal', { static: true }) createOrEditCashBillModal: CreateEditCashBillModalComponent;
  @ViewChild('paymentSummaryModal', { static: true }) paymentSummaryModal: PaymentSummaryModalComponent;
  @ViewChild('offlinePaymentModal', { static: true }) offlinePaymentModal: OfflinePaymentModalComponent;

  isLoading = false;
  bankLoading = false;
  cLoading = false;
  validToPay = 'hideThis';
  isWorkingAcc = false;

  inputHelper: any = {};
  customerCombo: any;
  selectedCustomer: any;
  itemsData: any = [];
  fpxModel: any = {};
  maybankModel: any = {};
  payModel: any;
  payment: any = {};
  offlineInfoList: any[] = [];
  transDtlHdr: any[] = [];

  payButton = false;
  term = false;
  bankListShow = false;
  paying = false;
  selectedPayment: string;

  personalBank: any[] = [];
  corpBank: any[] = [];
  bankListModel: any;

  checkSum: string;
  maxTotal = false;

  x: any;
  sstAmt = 0;
  dateToday = moment(new Date()).format('YYYYMMDDHHmmss');

  companyUrl = ProxyURL.GetCompanyDetail;

  constructor(
    injector: Injector,
    private location: Location,
    private _proxy: GenericServiceProxy,
    private _activatedRoute: ActivatedRoute,
    private _router: Router,
    private _location: Location,
    public _zone: NgZone,
    private _invoiceCont: InvoiceCont,
    private _localStorageService: LocalStorageService,
    private _httpService: HttpClient
  ) {
    super(injector);
  }

  ngOnInit() {
    this.checkFPXBankList();
    this.setNullBankForm();

    this.inputHelper.billno = this.dateToday;
    this.inputHelper.billdate = moment(new Date()).format('DD/MM/YYYY');
    this.inputHelper.customer = '-';
    this.inputHelper.address = '-';
    this.inputHelper.address1 = '';
    this.inputHelper.address2 = '';
    this.inputHelper.address3 = '';
    this.inputHelper.address4 = '';
    this.inputHelper.customerno = '-';
    this.inputHelper.email = '-';
    this.inputHelper.sst = 0;
    this.inputHelper.subtotal = 0;
    this.inputHelper.total = 0;
    this.inputHelper.valueRounding = 0;
    this.inputHelper.totalamountdue = 0;
    this.sstAmt = 6;

    if (this.isGranted('Pages.Account.CashBill.View')) {
      this._proxy.request(ProxyURL.GetCostumerCombo, RequestType.Get)
        .pipe().subscribe(result => {
            this.customerCombo = result;
        });
    } else {
      let url = this.companyUrl + 'bizRegID=' + this.appStorage.bizRegID + '&';

      if (url !== undefined) {
        this._proxy.request(url, RequestType.Get)
          .subscribe((result) => {
            this.inputHelper.bizregid = result.BizRegID;
            this.inputHelper.bizlocid = result.BizLocID;
            this.inputHelper.customer = result.CompanyName;
            this.inputHelper.address = result.CompanyAddress1 + ' ' + result.CompanyAddress2 + ' ' + result.CompanyAddress3 + ' ' + result.CompanyAddress4;
            this.inputHelper.customerno = result.AcctNo;
            this.inputHelper.email = result.Email;
            this.inputHelper.attn = result.ContactPerson;
            this.selectedCustomer = result.AcctNo;
            this.cLoading = false;
          });
      }
    }
  }

  async ngAfterViewInit() {
    this.primengTableHelper.defaultRecordsCountPerPage = 25;
    await this.checkPaymentType();
  }

  checkFPXBankList() {
    this._httpService.get(this.appUrlService.appRootUrl + 'assets/qstl/banklist.json')
      .subscribe((result) => {
        let res = JSON.parse(JSON.stringify(result));
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

  setNullBankForm() {
    this.fpxModel.fpx_msgType = '';
    this.fpxModel.fpx_msgToken = '';
    this.fpxModel.fpx_sellerExId = '';
    this.fpxModel.fpx_sellerExOrderNo = '';
    this.fpxModel.fpx_sellerOrderNo = '';
    this.fpxModel.fpx_sellerTxnTime = '';
    this.fpxModel.fpx_sellerId = '';
    this.fpxModel.fpx_sellerBankCode = '';
    this.fpxModel.fpx_txnCurrency = '';
    this.fpxModel.fpx_txnAmount = '';
    this.fpxModel.fpx_buyerEmail = '';
    this.fpxModel.fpx_buyerId = '';
    this.fpxModel.fpx_buyerName = '';
    this.fpxModel.fpx_buyerBankId = '';
    this.fpxModel.fpx_buyerBankBranch = '';
    this.fpxModel.fpx_buyerAccNo = '';
    this.fpxModel.fpx_makerName = '';
    this.fpxModel.fpx_buyerIban = '';
    this.fpxModel.fpx_productDesc = '';
    this.fpxModel.fpx_version = '';

    this.maybankModel.mmerchant_acc_no = '';
    this.maybankModel.amount = '';
    this.maybankModel.transaction_type = '';
    this.maybankModel.merchant_tranid = '';
    this.maybankModel.transaction_id = '';
    this.maybankModel.response_type = '';
    this.maybankModel.return_url = '';
    this.maybankModel.txn_desc = '';
    this.maybankModel.customer_id = '';
    this.maybankModel.fr_highrisk_email = '';
    this.maybankModel.fr_highrisk_country = '';
    this.maybankModel.fr_billing_address = '';
    this.maybankModel.fr_shipping_address = '';
    this.maybankModel.fr_shipping_cost = '';
    this.maybankModel.fr_purchase_hour = '';
    this.maybankModel.fr_customer_ip = '';
    this.maybankModel.txn_signature = '';
  }

  refresh() {
  }

  onCustomerChange(data?: any) {
    this.cLoading = true;
    let url = this.companyUrl + 'bizRegID=' + data.Code + '&';
    let waUrl = ProxyURL.GetListMaintenance + 'branchId=' + encodeURIComponent('06') + '&' + 'prefix=' + encodeURIComponent('WORKINGACCOUNT') + '&';

    _observableCombineLatest([
      this._proxy.request(url, RequestType.Get),
      this._proxy.request(waUrl, RequestType.Get)
    ]).subscribe(([result, result2]) => {
        this.inputHelper.bizregid = result.BizRegID;
        this.inputHelper.bizlocid = result.BizLocID;
        this.inputHelper.customer = result.CompanyName;
        this.inputHelper.address = result.CompanyAddress1 + ' ' + result.CompanyAddress2 + ' ' + result.CompanyAddress3 + ' ' + result.CompanyAddress4;
        this.inputHelper.customerno = result.AcctNo;
        this.inputHelper.email = result.Email;
        this.cLoading = false;

        if (result2.items.filter(x => x.SYSValue === this.inputHelper.customerno).length !== 0) {
          this.isWorkingAcc = true;
          this.checkValidToPay();
        } else {
          this.isWorkingAcc = false;
          this.checkValidToPay();
        }
      });
  }

  addItem() {
    this.createOrEditCashBillModal.show();
  }

  removeItem(data) {
    let i = this.itemsData.indexOf(data);
    this.itemsData.splice(i, 1);
    this.calcItem();
  }

  rounding() {
    let res = (this.inputHelper.total).toString().slice(-1);
    if (res == 1 || res == 6) {
      this.inputHelper.valueRounding = -0.01;
      this.inputHelper.total = (((this.x.add(this.inputHelper.sst))).sub(0.01)).toFixed(2);
    } else if (res == 2 || res == 7) {
      this.inputHelper.valueRounding = -0.02;
      this.inputHelper.total = (((this.x.add(this.inputHelper.sst))).sub(0.02)).toFixed(2);
    } else if (res == 3 || res == 8) {
      this.inputHelper.valueRounding = 0.02;
      this.inputHelper.total = (((this.x.add(this.inputHelper.sst))).add(0.02)).toFixed(2);
    } else if (res == 4 || res == 9) {
      this.inputHelper.valueRounding = 0.01;
      this.inputHelper.total = (((this.x.add(this.inputHelper.sst))).add(0.01)).toFixed(2);
    } else {
      this.inputHelper.valueRounding = 0.00;
    }
  }

  calcItem() {
    // console.log('Item: ' + JSON.stringify(this.itemsData));
    let subTotal = this.itemsData.map(x => x.subtotal).reduce((a, b) => a + b, 0);
    // let sst = 6 * subTotal / 100;
    let sst = this.itemsData.map(x => x.totaltax).reduce((a, b) => a + b, 0);
    this.x = new Decimal(subTotal);

    this.inputHelper.sst = new Decimal(sst).toFixed(2);
    this.inputHelper.subtotal = new Decimal(subTotal).toFixed(2);
    this.inputHelper.total = this.x.add(this.inputHelper.sst).toFixed(2);
    this.rounding();
    this.inputHelper.totalamountdue = this.inputHelper.total;
  }

  emitItem(data?: any) {
    this.itemsData.push(JSON.parse(JSON.stringify(data)));
    this.calcItem();
    this.checkValidToPay();
  }

  checkValidToPay() {
    if ((this.isWorkingAcc === true && this.inputHelper.address1.length > 0) || (this.isWorkingAcc === false && (this.itemsData.length > 0 && this.selectedCustomer !== undefined))) {
      this.validToPay = '';
      this.payButton = true;
    // } else if (this.itemsData.length > 0 && this.selectedCustomer !== undefined) {
    //   this.validToPay = '';
    //   this.payButton = true;
    } else {
      this.validToPay = 'hideThis';
      this.payButton = false;
    }
  }

  selectPaymentChange(selectedPay) {
    this.selectedPayment = selectedPay;
  }

  selectBank(msgToken: string): void {
    this.isLoading = true;
    this.payButton = true;
    this.term = false;
    this.bankListModel = [];

    if (msgToken === '01' && this.inputHelper.total <= 30000) {
      this.maxTotal = false;
      this.bankListShow = true;
      this.bankListModel = this.personalBank;
      this.fpxModel.fpx_msgToken = '01';
      this.isLoading = false;
    } else if ( msgToken === '02' && this.inputHelper.total <= 30000) {
      this.maxTotal = false;
      this.bankListShow = true;
      this.bankListModel = this.corpBank;
      this.fpxModel.fpx_msgToken = '02';
      this.isLoading = false;
    } else if (msgToken === '02' && this.inputHelper.total >= 30000) {
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

  // getBankList() {
  //   this.isLoading = true;
  //   this.bankLoading = true;
  //   this.payButton = true;
  //   let url = ProxyURL.GetFPXBankList;

  //   this.bankListModel = {
  //     personalBank: this.personalBank,
  //     corporateBank: this.corpBank
  //   };

  //   if (url !== undefined) {
  //     let persBank = url + 'fpx_msgToken=' + encodeURIComponent('01') + '&';
  //     let corpBank = url + 'fpx_msgToken=' + encodeURIComponent('02') + '&';

  //     _observableCombineLatest([
  //       this._proxy.request(persBank, RequestType.Get),
  //       this._proxy.request(corpBank, RequestType.Get)
  //     ]).pipe(finalize(() => {
  //       this.isLoading = false;
  //       this.bankLoading = false;
  //     }))
  //       .subscribe(([resultPers, resultCorp]) => {
  //         if (resultPers != null) {
  //           this.personalBank = resultPers;
  //         }
  //         if (resultCorp != null) {
  //           this.corpBank = resultCorp;
  //         }
  //       });
  //   }
  // }

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

  setFPXDetails(data: any) {
    this.fpxModel.fpx_msgType = 'AE';
    //this.fpxModel.fpx_msgToken = '01';
    this.fpxModel.fpx_sellerExId = AppConsts.FPX.sellerExId;
    this.fpxModel.fpx_sellerExOrderNo = this.dateToday;
    this.fpxModel.fpx_sellerOrderNo = this.dateToday;
    this.fpxModel.fpx_sellerTxnTime = this.dateToday;
    this.fpxModel.fpx_sellerId = AppConsts.FPX.sellerId;
    this.fpxModel.fpx_sellerBankCode = AppConsts.FPX.sellerBankCode;
    this.fpxModel.fpx_txnCurrency = AppConsts.FPX.txnCurrency;
    this.fpxModel.fpx_txnAmount = new Decimal(this.inputHelper.total).toFixed(2);
    this.fpxModel.fpx_buyerEmail = this.inputHelper.email; // this.appStorage.email;
    this.fpxModel.fpx_buyerId = this.inputHelper.customerno;
    this.fpxModel.fpx_buyerName = this.inputHelper.customer;
    this.fpxModel.fpx_buyerAccNo = '';
    this.fpxModel.fpx_makerName = 'Lembaga Getah Malaysia';
    this.fpxModel.fpx_buyerIban = '';
    this.fpxModel.fpx_productDesc = 'LGM Payment ' + this.inputHelper.billno;
    this.fpxModel.fpx_version = AppConsts.FPX.version;
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
    this.maybankModel.fr_highrisk_email = this.inputHelper.email; // this.appStorage.email;
    this.maybankModel.fr_highrisk_country = '';
    this.maybankModel.fr_billing_address = '';
    this.maybankModel.fr_shipping_address = '';
    this.maybankModel.fr_shipping_cost = '';
    this.maybankModel.fr_purchase_hour = '';
    this.maybankModel.fr_customer_ip = '';
    this.maybankModel.txn_signature = '';
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
      OfflineInfoList: this.offlineInfoList,
      Mode: 1
    };

    //# region HDR
    transHdr.BizRegID = this.inputHelper.bizregid;
    transHdr.BizLocID = this.inputHelper.bizlocid;
    transHdr.TermID = 1;
    this.selectedPayment === 'fpxPay' ? transHdr.TransNo = this.fpxModel.fpx_sellerOrderNo : transHdr.TransNo = this.dateToday;
    transHdr.TransType = 1;
    transHdr.BillNo = this.inputHelper.billno;
    transHdr.ShiftCode = '99';
    transHdr.TransDate = moment(new Date()).format('YYYY-MM-DD');
    transHdr.TransAmt = this.inputHelper.total;
    transHdr.TransChgAmt = this.inputHelper.sst;
    transHdr.TransAmtRnd = this.inputHelper.valueRounding;
    transHdr.TransSubTotal = this.inputHelper.subtotal;
    transHdr.CustomerID = this.inputHelper.customerno;
    transHdr.CustPrivilege = ''; //this.fpxModel.fpx_buyerBankId;
    transHdr.CashierID = this.inputHelper.attn;
    //transHdr.bizRegID = this.inputHelper.CostumerCode;
    transHdr.CustPkgID = '';
    transHdr.ServerID = '';
    transHdr.TransDiscReasonCode = '';
    transHdr.TransDiscRemark = '';
    transHdr.AcctNo = '';
    transHdr.TransReasonCode = '';
    transHdr.TransRemark = this.getFullAddress(); // this.inputHelper.address1 + '|' + this.inputHelper.address2 + '|' + this.inputHelper.address3 + '|' + this.inputHelper.address4;
    transHdr.InSvcID = '';
    transHdr.TransStatus = '99';
    transHdr.TblNo = '';
    transHdr.Posted = 1;
    transHdr.PostDate = moment(this.inputHelper.billdate, 'DD/MM/YYYY');
    transHdr.Status = 0;
    transHdr.Flag = 1;

    //# region DTL
    this.transDtlHdr = this.itemsData;
    for (let i = 0; i < this.transDtlHdr.length; i++) {
      let transDtl: any = {};
      transDtl.BizRegID = transHdr.BizRegID;
      transDtl.BizLocID = transHdr.BizLocID;
      transDtl.TermID = 1;
      transDtl.TransNo = transHdr.TransNo;
      transDtl.TransSeq = i + 1;
      transDtl.BillNo = ''; // this.transDtlHdr[i].BillNo;
      transDtl.StkType = 'CSH';
      transDtl.ItemType = 1;
      transDtl.ItemCode = this.transDtlHdr[i].paymentfor;
      transDtl.Qty = this.transDtlHdr[i].quantity;
      transDtl.Remark = this.transDtlHdr[i].remark;
      transDtl.StkCode = this.transDtlHdr[i].sundb;
      transDtl.CustPkgID = this.transDtlHdr[i].taxcode;
      transDtl.UnitCost = this.transDtlHdr[i].unitprice;
      transDtl.TolAmt = this.transDtlHdr[i].subtotal;
      transDtl.Posted = 1;
      transDtl.Flag = 1;
      transDtl.Status = transHdr.Status;
      transDtl.IsHost = 1;

      transDtlList.push(transDtl);
    }
  }

  maybank(): void {
    this.payButton = false;
    this.term = false;
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

  getFullAddress() {
    let fullAddress: string;
    if ((this.inputHelper.address1 !== '' && this.inputHelper.address2 !== '') && (this.inputHelper.address3 !== '' && this.inputHelper.address4 !== '')) {
      fullAddress = this.inputHelper.address1 + '|' + this.inputHelper.address2 + '|' + this.inputHelper.address3 + '|' + this.inputHelper.address4;
    } else if (this.inputHelper.address4 === '') {
      fullAddress = this.inputHelper.address1 + '|' + this.inputHelper.address2 + '|' + this.inputHelper.address3;
    } else if (this.inputHelper.address3 === '' && this.inputHelper.address4 === '') {
      fullAddress = this.inputHelper.address1 + '|' + this.inputHelper.address2;
    } else {
      fullAddress = this.inputHelper.address1;
    }
    return fullAddress;
  }

  goBack() {
    this.location.back();
  }

  //Add the eventListener for payment type
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
