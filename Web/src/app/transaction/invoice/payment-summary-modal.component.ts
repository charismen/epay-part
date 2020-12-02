import { Decimal } from 'decimal.js';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Component, OnInit, ViewEncapsulation, Injector, ViewChild, Output, EventEmitter } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { AppConsts } from '@shared/AppConsts';
import { ProxyURL } from '@shared/service-proxies/generic-service-proxies-url';
import { RequestType, GenericServiceProxy } from '@shared/service-proxies/generic-service-proxies';

import * as moment from 'moment';

@Component({
  selector: 'paymentSummaryModal',
  templateUrl: './payment-summary-modal.component.html',
  styleUrls: ['./payment-summary-modal.component.less'],
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()]
})
export class PaymentSummaryModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('createModal', { static: true }) modal: ModalDirective;

  @Output() modalClose: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  paying = false;
  saving = false;

  bizregid: string;

  customer: any;
  emailCustomer: any[];

  paymentSubHeader: string;

  bankName: string; // = '/assets/qstl/img/';

  transModel: any;
  totalAmount: any;
  paymentModel: any = {};
  paymentType: string;
  paymentMethod: string;
  companyName: string;

  gridUrlCompany = ProxyURL.GetCompanyDetail;
  urlEmail = ProxyURL.InvoiceNotification;
  urlCompany = ProxyURL.GetCompanyDetail;

  constructor(
    injector: Injector,
    private _router: Router,
    private _proxy: GenericServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {
    this.setNullForm();
  }

  setNullForm() {
    this.paymentModel.fpx_msgType = '';
    this.paymentModel.fpx_msgToken = '';
    this.paymentModel.fpx_sellerExId = '';
    this.paymentModel.fpx_sellerExOrderNo = '';
    this.paymentModel.fpx_sellerOrderNo = '';
    this.paymentModel.fpx_sellerTxnTime = '';
    this.paymentModel.fpx_sellerId = '';
    this.paymentModel.fpx_sellerBankCode = '';
    this.paymentModel.fpx_txnCurrency = '';
    this.paymentModel.fpx_txnAmount = '';
    this.paymentModel.fpx_buyerEmail = '';
    this.paymentModel.fpx_buyerId = '';
    this.paymentModel.fpx_buyerName = '';
    this.paymentModel.fpx_buyerBankId = '';
    this.paymentModel.fpx_buyerBankBranch = '';
    this.paymentModel.fpx_buyerAccNo = '';
    this.paymentModel.fpx_makerName = '';
    this.paymentModel.fpx_buyerIban = '';
    this.paymentModel.fpx_productDesc = '';
    this.paymentModel.fpx_version = '';

    this.paymentModel.mmerchant_acc_no = '';
    this.paymentModel.amount = '';
    this.paymentModel.transaction_type = '';
    this.paymentModel.merchant_tranid = '';
    this.paymentModel.transaction_id = '';
    this.paymentModel.response_type = '';
    this.paymentModel.return_url = '';
    this.paymentModel.txn_desc = '';
    this.paymentModel.customer_id = '';
    this.paymentModel.fr_highrisk_email = '';
    this.paymentModel.fr_highrisk_country = '';
    this.paymentModel.fr_billing_address = '';
    this.paymentModel.fr_shipping_address = '';
    this.paymentModel.fr_shipping_cost = '';
    this.paymentModel.fr_purchase_hour = '';
    this.paymentModel.fr_customer_ip = '';
    this.paymentModel.txn_signature = '';
  }

  show(trxData?: any, payData?: any, type?: string, totalAmt?: any): void {
    this.totalAmount = totalAmt;
    type === 'fpxPay' ? this.paymentSubHeader = 'PaymentViaFpx' : type === 'ccPay' ? this.paymentSubHeader = 'PaymentViaCC' : this.paymentSubHeader = 'OfflinePayment';
    this.bankName = 'assets/qstl/img/';
    type === 'fpxPay' ? this.bankName += payData.fpx_buyerBankId + '.png' : type === 'ccPay' ? this.bankName += 'MB2U0227' + '.png' : this.bankName = 'Offline';

    this.transModel = trxData;
    payData !== undefined ? this.paymentModel = payData : this.setNullForm();
    this.paymentType = type;
    this.paymentType === 'fpxPay' ? this.paymentMethod = 'FPX' : this.paymentType === 'ccPay' ? this.paymentMethod = 'MayBank' : this.paymentMethod = 'Offline Payment';

    console.log('TRX Data: ' + JSON.stringify(trxData));
    console.log('Pay Data: ' + JSON.stringify(payData));
    this.getCompany(trxData.TransHdr.BizRegID);

    this.modal.show();
    this.active = true;
  }

  proceed(): void {
    this.paying = true;
    let urlPay = ProxyURL.AddTransaction;

    this._proxy.request(urlPay, RequestType.Post, this.transModel)
      .subscribe((result) => {
        if (result.success = 'true') {
          //this.sendEmail();
          this.notify.success(this.l('RedirectingWithThreeDots'));
          this.redirect();
          this.close();
          this._router.navigate(['/app/transaction/payment-history']);
          this.paying = false;
        } else {
          this.notify.error(this.l('PaymentFailed'));
          this.paying = false;
        }
      });
  }

  getCompany(bizRegid): void {
    this.urlCompany += 'bizRegID=' + bizRegid + '&';

    this._proxy.request(this.urlCompany, RequestType.Get)
      .subscribe((result) => {
        this.companyName = result.CompanyName;
      });
  }

  redirect(): void {
    if (this.paymentType === 'fpxPay') {
      let form = (document.getElementById('fpxForm') as HTMLFormElement);

      form.setAttribute('target', '_self');
      form.action = AppConsts.FPX.FPXJSP;
      form.submit();
      form.removeAttribute('target');
    } else if (this.paymentType === 'ccPay') {
      let form = (document.getElementById('maybankForm') as HTMLFormElement);

      form.setAttribute('target', '_self');
      form.action = AppConsts.MayBank.MBBEBPGCARDSURL;
      form.submit();
      form.removeAttribute('target');
    } else {
      //this.sendEmail();
      // this._router.navigate(['/account/payment-response?orderNo=' + this.transModel.TransHdr.TransNo + '&offPay=true']);
      let url = this.appRootUrl() + '/account/payment-response?orderNo=' + this.transModel.TransHdr.TransNo + '&offPay=true';
      window.open(url, '_blank');
    }
  }

  close(): void {
    this.transModel = undefined;
    this.paymentModel = undefined;
    this.active = false;
    this.modalClose.emit(null);
    this.modal.hide();
  }

  sendEmail(): void {
    let stAmount = this.totalAmount.toLocaleString('en-US', { minimumFractionDigits: 2 });
    this.urlEmail += 'emailAddress=' + this.appStorage.email + '&paymentMethod=' + this.paymentMethod + '&totalAmount=' + stAmount;
    this.urlEmail += '&mode=' + '1' + '&companyName=' + this.companyName + '&transno=' + this.transModel.TransHdr.TransNo + '&';

    this._proxy.request(this.urlEmail, RequestType.Get)
      .pipe().subscribe(result => {
      });
  }
}
