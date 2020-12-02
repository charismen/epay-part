import { GenericServiceProxy, RequestType } from '@shared/service-proxies/generic-service-proxies';
import { ProxyURL } from '@shared/service-proxies/generic-service-proxies-url';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, of as _observableOf } from 'rxjs';
import { HttpClient, HttpResponseBase } from '@angular/common/http';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Component, OnInit, ViewEncapsulation, Injector, AfterViewInit, Injectable } from '@angular/core';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import * as moment from 'moment';

@Component({
  templateUrl: './payment-response.component.html',
  styleUrls: ['./payment-response.component.less'],
  encapsulation: ViewEncapsulation.None,
  animations: [accountModuleAnimation()]
})

@Injectable()
export class PaymentResponseComponent extends AppComponentBase implements OnInit, AfterViewInit {

  isPageLoading: boolean;

  inputHelper: any = {};
  transno: string;
  statClass: string;

  creditCard: boolean;
  offPay: boolean;

  // urlEmail = ProxyURL.InvoiceNotification;
  urlEmail = ProxyURL.InvoiceMailNotif;

  constructor(
    injector: Injector,
    private _activatedRoute: ActivatedRoute,
    private _proxy: GenericServiceProxy,
    private _http: HttpClient,
    private _router: Router,
  ) {
    super(injector);
  }

  ngOnInit() {
    this.inputHelper.transno = '';
    this.inputHelper.fpxid = '';
    this.inputHelper.transdate = '';
    this.inputHelper.bankcode = '';
    this.inputHelper.amount = '';
    this.inputHelper.status = '';
    this.inputHelper.paymentvia = '';

    this._activatedRoute.queryParams
      .subscribe(params => {
        params.orderNo === undefined ? this.transno = undefined : this.transno = params.orderNo;
        params.creditCard === 'true' ? this.creditCard = true : params.creditCard === undefined ? this.creditCard = false : this.creditCard = false;
        params.offPay === 'true' ? this.offPay = true : this.offPay = false;
      });

    if (this.transno === undefined) {
      this.inputHelper.transno = 'No Data';
      this.inputHelper.fpxid = 'No Data';
      this.inputHelper.transdate = 'No Data';
      this.inputHelper.bankcode = 'No Data';
      this.inputHelper.amount = '0.00';
      this.inputHelper.status = 'No Data';
      this.inputHelper.paymentvia = 'No Data';
      this.statClass = 'failResponse';
    } else if (this.creditCard === true) {
      this.getCCDetails(this.transno);
    } else if (this.offPay === true) {
      this.getOffDetails(this.transno);
    } else {
      this.getDetails(this.transno);
    }
  }

  getDetails(transNo) {
    let url = ProxyURL.GetResponseDetails;
    url += 'transNo=' + encodeURIComponent('' + transNo) + '&';

    if (url !== undefined) {
      this._proxy.request(url, RequestType.Get)
        .subscribe((result) => {
          this.inputHelper.transno = result.TransNo;
          this.inputHelper.fpxid = result.ExternalID;
          this.inputHelper.transdate = moment(result.TransDate).format('DD-MM-YYYY') + ' ' + result.TransTime;
          this.inputHelper.bankcode = result.BankName;
          this.inputHelper.amount = result.TenderAmt;
          this.inputHelper.status = result.Status;
          this.inputHelper.paymentvia = this.l('PaymentViaFPX');
          result.Status === 'Approved' ? this.statClass = 'successResponse' : result.Status.substring(0, 7) == 'Pending' ? this.statClass = 'pendingResponse' :  this.statClass = 'failResponse';

          this.inputHelper.AcctNo = result.AcctNo;
          this.inputHelper.CompanyName = result.CompanyName;
          this.inputHelper.Email = result.Email;

          if (result.Status === 'Approved') {
            this.sendEmail();
          }
        });
    }
  }

  getCCDetails(transNo) {
    let url = ProxyURL.GetResponseDetails;
    url += 'transNo=' + encodeURIComponent('' + transNo) + '&';
    url += 'creditCard=true';

    if (url !== undefined) {
      this._proxy.request(url, RequestType.Get)
        .subscribe((result) => {
          this.inputHelper.transno = result.TransNo;
          this.inputHelper.transdate = moment(result.TransDate).format('DD-MM-YYYY') + ' ' + result.TransTime;
          this.inputHelper.bankcode = 'MayBank';
          this.inputHelper.amount = result.TenderAmt;
          this.inputHelper.status = result.RefRemark;
          this.inputHelper.paymentvia = this.l('PaymentViaCC');
          result.RefRemark === 'APPROVED OR COMPLETED' ? this.statClass = 'successResponse' : result.RespCode === '00' ? this.statClass = 'successResponse' : this.statClass = 'failResponse';

          this.inputHelper.AcctNo = result.AcctNo;
          this.inputHelper.CompanyName = result.CompanyName;
          this.inputHelper.Email = result.Email;

          if (result.RefRemark === 'APPROVED OR COMPLETED') {
            this.sendEmail();
          }
        });
    }
  }

  getOffDetails(transNo) {
    let url = ProxyURL.GetResponseDetails;
    url += 'transNo=' + encodeURIComponent('' + transNo) + '&';
    url += 'offPay=true';

    if (url !== undefined) {
      this._proxy.request(url, RequestType.Get)
        .subscribe((result) => {
          this.inputHelper.transno = result.TransNo;
          this.inputHelper.transdate = moment(result.TransDate).format('DD-MM-YYYY') + ' ' + result.TransStartTime;
          this.inputHelper.bankcode = this.l('offlinePayment'); // 'OffLine Payment';
          this.inputHelper.amount = result.TransAmt;
          this.inputHelper.status = 'Paid';
          this.inputHelper.paymentvia = this.l('OfflinePayment');
          this.statClass = 'successResponse';

          this.inputHelper.AcctNo = result.AcctNo;
          this.inputHelper.CompanyName = result.CompanyName;
          this.inputHelper.Email = result.Email;

          this.sendEmail();
        });
    }
  }

  back(): void {
    this._router.navigate(['/app/transaction/payment-history']);
  }

  sendEmail(): void {
    let stAmount = this.inputHelper.amount.toLocaleString('en-US', { minimumFractionDigits: 2 });
    this.urlEmail += 'emailAddress=' + this.inputHelper.Email + '&paymentMethod=' + this.inputHelper.paymentvia + '&totalAmount=' + stAmount; // + '&mode=' + '1';
    this.urlEmail += '&companyName=' + this.inputHelper.CompanyName + '&transno=' + this.inputHelper.transno + '&';

    console.log('Email URL: ' + this.urlEmail);

    this._proxy.request(this.urlEmail, RequestType.Get)
      .pipe().subscribe(result => {
        if (result.success === 'true') {
          this.notify.success(this.l('SendSuccess'));
        }
      });
  }

  async ngAfterViewInit() {
    this.isPageLoading = true;
    await this.startIconStatus();
  }

  async startIconStatus() {
    let elementStatus = document.querySelectorAll('#iconStatus > div');
    for (let i = 0; i < elementStatus.length; i++) {
      setTimeout(function () {
        elementStatus[i].classList.remove('hideThis');
      }, 1500);
    }
  }

}
