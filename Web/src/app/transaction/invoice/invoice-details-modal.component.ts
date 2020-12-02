import { UserServiceProxy, RoleServiceProxy, RegisterInput, AccountServiceProxy, RegisterOutput } from '@shared/service-proxies/service-proxies';
import { ProxyURL } from '@shared/service-proxies/generic-service-proxies-url';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { AppComponentBase } from 'shared/common/app-component-base';
import { Component, Injector, ViewChild, OnInit, AfterViewInit, NgZone, Pipe, EventEmitter, Output } from '@angular/core';
import { Observable, combineLatest as _observableCombineLatest, of as _observableOf } from 'rxjs';
import { finalize, tap, switchMap, concatMap } from 'rxjs/operators';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { Router } from '@angular/router';
import { InvoiceCont } from '@shared/AppEnums';
import { GenericServiceProxy, RequestType } from '@shared/service-proxies/generic-service-proxies';
import { AppAuthService } from '@app/shared/common/auth/app-auth.service';
import { PrintToPdf } from '@shared/AppEnums';
import { Decimal } from 'decimal.js';

import * as jsPDF from 'jspdf';
import * as moment from 'moment';
import 'jspdf-autotable';

@Component({
  selector: 'detailInvoiceModal',
  templateUrl: './invoice-details-modal.component.html',
  styleUrls: ['./invoice-details-modal.component.less'],
  animations: [accountModuleAnimation()]
})
export class DetailInvoiceModalComponent extends AppComponentBase implements OnInit {

  @ViewChild('detailInvoiceModal', { static: true }) modal: ModalDirective;
  @Output() generatePdf: EventEmitter<any> = new EventEmitter<any>();

  isLoading = false;
  isCashBill = false;

  active = false;
  saving = false;
  loading = false;

  PDFformat: string;
  toPdf = PrintToPdf;

  InvoiceTitle: string;
  DetailModal: string;
  fpxPayment: any = {};
  fpxModel: any = {};
  inputHelper: any = {};
  payModel: any;
  tableItems: any;
  tableItems2: any;
  paying = false;
  id: any;
  type: any;
  urlGet: string;
  printInv = PrintToPdf.Invoice;
  itemCode: any = null;

  invoicePrint: boolean;
  paymentHistory: boolean;

  sstAmt = 0;
  taxAmt: number;
  Rounding: number;

  constructor(
    injector: Injector,
    private _proxy: GenericServiceProxy,
    private _userService: UserServiceProxy,
    private _route: Router,
    private _appAuthService: AppAuthService,
    private _invoiceCont: InvoiceCont

  ) {
    super(injector);
  }

  show(record: any, IsReceipt: any, transno: any): void {
    this.active = true;
    this.id = record.Id;
    this.type = record.Type;
    this.modal.show();
    this.itemCode = record.ItemCode;

    if (IsReceipt === 0) {
      this.urlGet = ProxyURL.GetInvoiceDetail;
      this.urlGet += 'transNo=' + encodeURIComponent(record.SiNumber) + '&';
      this.populateData(this.urlGet);
      this.invoicePrint = true;

      this.printInv = PrintToPdf.Invoice;
      this.PDFformat = 'printInvoice';
    } else {
      this.invoicePrint = false;
      let url = ProxyURL.GetInvoiceDetail;
      url += 'transNo=' + encodeURIComponent(record.ItemCode) + '&' + 'status=' + encodeURIComponent(record.Status) + '&' + 'detail=' + '1' + '&';
      this.populateData(url);

      this.inputHelper.receiptno = record.ReceiptNo;
      this.inputHelper.digitalsign = record.DigitalSignature;
      this.inputHelper.deliveryref = record.DeliveryRef;
      this.inputHelper.costcenter = record.CostCenter;
      this.inputHelper.ExternalID = record.ExternalID;

      this.printInv = PrintToPdf.Receipt;
      this.PDFformat = 'printReceipt';
    }

  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }

  ngOnInit() {

  }

  refresh(): void {
    this.paying = false;
  }

  populateData(url: string) {
    // this.invoicePrint = true;
    this.isLoading = true;
    this._proxy.request(url, RequestType.Get)
      .pipe(finalize(() => { this.isLoading = false; }))
      .subscribe(result => {
        if (this.type === 'Credit Bill') {
          this.InvoiceTitle = 'Invoice';
          this.DetailModal = 'DetailInvoice';
          this._invoiceCont = undefined;
          this.type = PrintToPdf.CashBill;
        } else if (this.type === 'Proforma Bill') {
          this.InvoiceTitle = 'Invoice';
          this.DetailModal = 'DetailInvoice';
          this._invoiceCont = undefined;
          this.type = PrintToPdf.ProformaBill;
        }
        this.primengTableHelper.records = result.items.items;
        this.tableItems = result.items.items;
        this.tableItems2 = result.items.items;

        this.inputHelper.billdate = moment(result.data.Date).format('DD/MM/YYYY');
        this.inputHelper.billno = result.data.BillNo;
        this.inputHelper.customer = result.data.Company;
        this.inputHelper.attn = result.data.ContactPerson;
        this.inputHelper.address = result.data.Address1 + '.\n' + result.data.Address2 + '.\n' + result.data.Address3;
        this.inputHelper.customerno = result.data.AcctNo;
        this.inputHelper.creditvote = result.data.CreditVote === '' ? '-' : result.data.CreditVote;
        this.inputHelper.projectcode = result.data.ProjectCode === '' ? '-' : result.data.ProjectCode;
        this.inputHelper.refno = result.data.RefNo === '' ? '-' : result.data.RefNo;
        //this.inputHelper.deliveryref = result.data.DeliveryRef === '' ? '-' : result.data.DeliveryRef;
        this.inputHelper.ourref = result.data.OurRef === '' ? '-' : result.data.OurRef;
        this.inputHelper.yourref = result.data.YourRef === '' ? '-' : result.data.YourRef;
        //this.inputHelper.sst = this._invoiceCont.Invoice.map(x => x.Tax).reduce((a, b) => a + b, 0);
        this.inputHelper.taxcatcode = result.data.TaxCatCode === '' ? '-' : result.data.TaxCatCode;
        this.inputHelper.sst = result.data.TaxAmt;
        this.inputHelper.subtotal = this.tableItems.map(x => x.SubTotal).reduce((a, b) => a + b, 0);
        this.inputHelper.rounding = result.data.Rounding;
        this.inputHelper.total = this.inputHelper.total = new Decimal(this.inputHelper.subtotal + this.inputHelper.sst + this.inputHelper.rounding).toFixed(2);
        this.inputHelper.totalamountdue = result.data.Balance;
        this.inputHelper.billDate = moment(result.data.Date).format('DD/MM/YYYY');
        // this.inputHelper.taxcode = result.data.TaxCode === '' ? '-' : result.data.TaxCode;
        this.payModel = result.items;
      });
  }

  printToPdf() {
    this.generatePdf.emit(this.inputHelper.billno);
  }
}
