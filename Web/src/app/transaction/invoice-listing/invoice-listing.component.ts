import { ProxyURL } from './../../../shared/service-proxies/generic-service-proxies-url';
import { BaseListComponent } from '@app/shared/form/list/base-list.component';
import { PermissionCheckerService } from 'abp-ng2-module';
import { Component, OnInit, AfterViewInit, Injector, ElementRef, ViewChild, ViewEncapsulation, Type } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Router } from '@angular/router';
import * as moment from 'moment';
import { DatePipe } from '@angular/common';
import { GenericServiceProxy, RequestType } from '@shared/service-proxies/generic-service-proxies';
import { formatCurrency, formatNumber } from '@angular/common';
import { RoleServiceProxy } from '@shared/service-proxies/service-proxies';
import { toISOFormat } from '@shared/helpers/DateTimeHelper';
import { InvoiceCont, InvoiceFilter } from '@shared/AppEnums';
import { finalize, tap, switchMap, concatMap, filter } from 'rxjs/operators';

// import * as _ from 'lodash';

@Component({
    templateUrl: './invoice-listing.component.html',
    styleUrls: ['./invoice-listing.component.less'],
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()],
    providers: [DatePipe]
})
export class InvoiceListingComponent extends AppComponentBase implements AfterViewInit, OnInit {

    @ViewChild('invoiceListComponent', { static: false }) invoiceListComponent: BaseListComponent;

    //gridUrl = ProxyURL.GetInvoice + 'mode=' + encodeURIComponent('' + 1) + '&';
    gridUrl = undefined;
    compUrl = ProxyURL.GetLGMCompany;
    // gridUrl = '';
    // compUrl = ProxyURL.GetCompanyDetail;

    isPageLoading: boolean;
    loading = false;
    filterAdmin = true;

    currency = 'RM';
    filterText = '';

    selectedInvoiceType: any;
    typeCombo: any[];

    selectedInvoice: any;

    selectedCustomer: any;
    customerCombo: any;

    company: any;

    //Filter
    processFilter: string;
    processFilterStart: Date;
    processFilterEnd: Date;

    dateFrom: Date; // moment(new Date()).format('YYYY-MM-DD');
    dateTo: Date; // moment(new Date()).format('YYYY-MM-DD');

    dateNow = new Date;

    //creditStatusUrl = '';
    totalCreditBill: number;
    totalCreditBillAmount: number;
    totalProforma: number;
    totalProformaAmount: number;
    totalInvoice: number;
    totalInvoiceAmount: number;
    totalSelected: number;
    totalSelectedAmount: number;
    cookie: any;
    bizCookies: any;
    continue = false;

    errorDate = '';
    inputHelper: any;

    constructor(
        injector: Injector,
        private _proxy: GenericServiceProxy,
        private _permissionChecker: PermissionCheckerService,
        private _roleProxy: RoleServiceProxy,
        private _route: Router,
        private datePipe: DatePipe,
        private _invoiceCont: InvoiceCont,
        private _invoiceFilter: InvoiceFilter
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.typeCombo = [{
            Seq: 0,
            Code: '',
            Remark: 'All Invoice'
        }, {
            Seq: 1,
            Code: 'CDT',
            Remark: 'Credit Bill'
        }, {
            Seq: 2,
            Code: 'PRF',
            Remark: 'Proforma'
        }];

        let bizRegCookies: any;

        bizRegCookies = this.appStorage;
        this.getInvoiceList(bizRegCookies);

        this._proxy.request(ProxyURL.GetCostumerCombo, RequestType.Get)
            .pipe().subscribe(result => {
                this.customerCombo = result;
            });

        this.totalInvoice = 0;
        this.totalSelected = 0;
        this.totalSelectedAmount = 0;
    }

    async ngAfterViewInit() {
        this.invoiceListComponent.primengTableHelper.defaultRecordsCountPerPage = 25;
    }

    reset() {
        this.dateFrom = undefined;
        this.dateTo = undefined;
    }

    getInvoiceList(bizCookies) {
        if (bizCookies === undefined || (bizCookies.bizRegID === undefined || bizCookies.bizRegID === null)) {
            //Filter Dashboard
            if (this._invoiceFilter.InvoiceType != null && this._invoiceFilter.InvoiceType !== '' || this._invoiceFilter.Customer != null && this._invoiceFilter.Customer !== '' || this._invoiceFilter.BillingDateFrom != null || this._invoiceFilter.BillingDateTo != null) {
                if (this._invoiceFilter.InvoiceType != null && this._invoiceFilter.InvoiceType !== '') {
                    this.selectedInvoiceType = this._invoiceFilter.InvoiceType;
                }
                if (this._invoiceFilter.Customer != null && this._invoiceFilter.Customer !== '') {
                    this.selectedCustomer = this._invoiceFilter.Customer;
                }
                if (this._invoiceFilter.BillingDateFrom != null) {
                    this.dateFrom = this._invoiceFilter.BillingDateFrom;
                }
                if (this._invoiceFilter.BillingDateTo != null) {
                    this.dateTo = this._invoiceFilter.BillingDateTo;
                }
                this.gridUrl = ProxyURL.GetInvoice + 'mode=1' + '&';
                this.isGranted('Pages.Account.Payment.Offline') === true ? '' : this.gridUrl += 'Filter=' + encodeURIComponent('' + this.appStorage.bizRegID) + '&';
                this.selectedInvoiceType != null ? this.gridUrl += 'billType=' + encodeURIComponent('' + this.selectedInvoiceType.Code) + '&' : '';
                this.selectedCustomer != null ? this.gridUrl += 'Filter=' + encodeURIComponent('' + this.selectedCustomer.Code) + '&' : '';
                this.dateFrom != null && this.dateTo != null ? this.gridUrl += 'dateStart=' + moment(this.dateFrom).format('YYYY-MM-DD') + '&dateEnd=' + moment(this.dateTo).format('YYYY-MM-DD') + '&' : '';

                //console.log('hai : '+ this._invoiceFilter);
            } else {
                this.gridUrl = ProxyURL.GetInvoice;
                this.gridUrl += 'mode=1' + '&';
            }

            this._proxy.request(this.gridUrl, RequestType.Get)
                .pipe(finalize(() => { this.isPageLoading = false; }))
                .subscribe(result => {
                    this.totalCreditBill = result.items[0].CreditBillCount;
                    this.totalCreditBillAmount = result.items[0].CreditBillAmount !== null ? result.items[0].CreditBillAmount : 0;

                    this.totalProforma = result.items[0].ProformaCount;
                    this.totalProformaAmount = result.items[0].ProformaAmount !== null ? result.items[0].ProformaAmount : 0;

                    this.totalInvoiceAmount = result.items[0].TotalAmount != null ? result.items[0].TotalAmount : 0;
                    this.totalInvoice = result.items[0].CreditBillCount + result.items[0].ProformaCount;

                    this.invoiceListComponent.refresh();
                });
        } else if (bizCookies !== null || bizCookies.bizRegID !== null) {
            //Filter Dashboard
            if (this._invoiceFilter.InvoiceType != null && this._invoiceFilter.InvoiceType !== '' || this._invoiceFilter.Customer != null && this._invoiceFilter.Customer !== '' || this._invoiceFilter.BillingDateFrom != null || this._invoiceFilter.BillingDateTo != null) {
                if (this._invoiceFilter.InvoiceType != null && this._invoiceFilter.InvoiceType !== '') {
                    this.selectedInvoiceType = this._invoiceFilter.InvoiceType;
                }
                if (this._invoiceFilter.Customer != null && this._invoiceFilter.Customer !== '') {
                    this.selectedCustomer = this._invoiceFilter.Customer;
                }
                if (this._invoiceFilter.BillingDateFrom != null) {
                    this.dateFrom = this._invoiceFilter.BillingDateFrom;
                }
                if (this._invoiceFilter.BillingDateTo != null) {
                    this.dateTo = this._invoiceFilter.BillingDateTo;
                }
                this.gridUrl = ProxyURL.GetInvoice + 'mode=1' + '&';
                this.gridUrl += 'customerCode=' + encodeURIComponent(bizCookies.bizRegID) + '&' + 'mode=1' + '&';
                this.isGranted('Pages.Account.Payment.Offline') === true ? '' : this.gridUrl += 'Filter=' + encodeURIComponent('' + this.appStorage.bizRegID) + '&';
                this.selectedInvoiceType != null ? this.gridUrl += 'billType=' + encodeURIComponent('' + this.selectedInvoiceType.Code) + '&' : '';
                this.selectedCustomer != null ? this.gridUrl += 'Filter=' + encodeURIComponent('' + this.selectedCustomer.Code) + '&' : '';
                this.dateFrom != null && this.dateTo != null ? this.gridUrl += 'dateStart=' + moment(this.dateFrom).format('YYYY-MM-DD') + '&dateEnd=' + moment(this.dateTo).format('YYYY-MM-DD') + '&' : '';

                //console.log('hai : '+ this._invoiceFilter);
            } else {
                this.gridUrl = ProxyURL.GetInvoice;
                this.gridUrl += 'customerCode=' + encodeURIComponent(bizCookies.bizRegID) + '&' + 'mode=1' + '&';
            }

            this._proxy.request(this.gridUrl, RequestType.Get)
                .pipe(finalize(() => { this.isPageLoading = false; }))
                .subscribe(result => {
                    this.totalCreditBill = result.items[0].CreditBillCount;
                    this.totalCreditBillAmount = result.items[0].CreditBillAmount !== null ? result.items[0].CreditBillAmount : 0;

                    this.totalProforma = result.items[0].ProformaCount;
                    this.totalProformaAmount = result.items[0].ProformaAmount !== null ? result.items[0].ProformaAmount : 0;

                    this.totalInvoiceAmount = result.items[0].TotalAmount != null ? result.items[0].TotalAmount : 0;
                    this.totalInvoice = result.items[0].CreditBillCount + result.items[0].ProformaCount;

                    this.invoiceListComponent.refresh();
                });
        }
    }

    onSelectedChange(data?: any) {
        this.selectedInvoice = data;
        this.totalSelected = data.length;
        // this.totalSelectedAmount = data.map(x => parseFloat(x.Amount)).reduce((a, b) => a + b, 0);
        this.totalSelectedAmount = data.map(x => parseFloat(x.Balance)).reduce((a, b) => a + b, 0);
    }

    typeSelectChange(data: any): void {
        this.isPageLoading = true;
        let url = this.gridUrl;
        url += 'billType=' + encodeURIComponent('' + data) + '&';

        setTimeout(() => {
            if (url !== undefined) {
                this.invoiceListComponent.gridUrl = url;
                this.invoiceListComponent.refresh();
                this.isPageLoading = false;
            }
        }, 100);
    }

    checkDateFrom(data?: Date) {
        this.dateRangeCheck(data, this.dateTo, '"Date To" Must Be After "Date From"');
    }

    checkDateTo(data?: Date) {
        this.dateRangeCheck(this.dateFrom, data, '"Date To" Must Be After "Date From"');
    }

    dateRangeCheck(from: Date, to: Date, msg: string) {
        (to < from) ? (this.errorDate = msg) : (this.errorDate = '');
    }

    onInvoiceTypeChange(data: string) {
    }

    onCustomerChange(data?: any) {
    }

    getDate() {
        if (this.dateTo < this.dateFrom) {
            this.message.error('"Date To" Must Be After "Date From"!');
        } else {
            this.message.info(this.datePipe.transform(this.dateFrom, 'dd/MM/yyyy') + ' - ' + this.datePipe.transform(this.dateTo, 'dd/MM/yyyy'));
        }
    }

    searchFilter() {
        this.gridUrl = ProxyURL.GetInvoice + 'mode=1' + '&';
        this.isGranted('Pages.Account.Payment.Offline') === true ? '' : this.gridUrl += 'Filter=' + encodeURIComponent('' + this.appStorage.bizRegID) + '&';
        this.selectedInvoiceType != null ? this.gridUrl += 'billType=' + encodeURIComponent('' + this.selectedInvoiceType.Code) + '&' : '';
        this.selectedCustomer != null ? this.gridUrl += 'Filter=' + encodeURIComponent('' + this.selectedCustomer.Code) + '&' : '';
        this.dateFrom != null && this.dateTo != null ? this.gridUrl += 'dateStart=' + moment(this.dateFrom).format('YYYY-MM-DD') + '&dateEnd=' + moment(this.dateTo).format('YYYY-MM-DD') + '&' : '';

        this._invoiceFilter.InvoiceType = this.selectedInvoiceType;
        this._invoiceFilter.Customer = this.selectedCustomer;
        this._invoiceFilter.BillingDateFrom = this.dateFrom;
        this._invoiceFilter.BillingDateTo = this.dateTo;

        this._proxy.request(this.gridUrl, RequestType.Get)
            .pipe(finalize(() => { this.isPageLoading = false; }))
            .subscribe(result => {
                this.totalCreditBill = result.items[0].CreditBillCount;
                this.totalCreditBillAmount = result.items[0].CreditBillAmount !== null ? result.items[0].CreditBillAmount : 0;
                this.totalProforma = result.items[0].ProformaCount;
                this.totalProformaAmount = result.items[0].ProformaAmount !== null ? result.items[0].ProformaAmount : 0;
                this.totalInvoiceAmount = result.items[0].TotalAmount != null ? result.items[0].TotalAmount : 0;
                this.totalInvoice = result.items[0].CreditBillCount + result.items[0].ProformaCount;
                this.invoiceListComponent.refresh();
            });

        setTimeout(() => {
            this.invoiceListComponent.refresh();
        }, 100);
    }

    payMultiInvoice(): void {
        if (this.totalSelected === 0) {
            this.continue = false;
        } else if (this.totalSelected === 1) {
            let params = {
                id: this.selectedInvoice[0].SiNumber
            };
            this._route.navigate(['/app/transaction/invoice'], { queryParams: params });
            this.continue = true;
        } else {
            this._invoiceCont.Invoice = this.selectedInvoice;
            this._route.navigate(['/app/transaction/invoice']);
            this.continue = true;
        }
    }

    // async assignModalColumn(colName) {
    //     let insertionListener = function (event) {
    //         //Making sure that this is the animation we want.
    //         if (event.animationName === 'tableReady') {

    //             let totalColumn = jQuery('.ui-table-scrollable-body .ui-selectable-row:first-child td'), indexStatus;
    //             totalColumn.each(function (event) {
    //                 let curElement = jQuery(this), colStatus;
    //                 colStatus = curElement.find('.ui-column-title').text();
    //                 colStatus = colStatus.toLowerCase().trim();
    //                 colName = colName.toLowerCase();
    //                 if (colStatus === colName) {
    //                     //find the index of coloumn that has colName
    //                     indexStatus = curElement.index();
    //                     indexStatus++;
    //                     //call function checker here
    //                     let allRowToCheck = jQuery('.ui-table-scrollable-body .ui-selectable-row td:nth-child(' + indexStatus + ') > span:nth-child(2)');
    //                     allRowToCheck.each(function () {
    //                         let curObj = jQuery(this);

    //                         let textStatus = curObj.text().toLowerCase().trim();
    //                         curObj.addClass('modalColumn');
    //                         switch (textStatus) {
    //                             case 'officer approval': curObj.addClass('officer1'); break;
    //                             case 'acceptance by uka': curObj.addClass('uka1'); break;
    //                             case 'accepted': curObj.addClass('accept1'); break;
    //                             case 'paid': curObj.addClass('paid1'); break;
    //                         }
    //                     });
    //                     return false;
    //                 }
    //             });
    //         }
    //     };
    //     document.addEventListener('animationstart', insertionListener, false); //standard + firefox
    //     document.addEventListener('MSAnimationStart', insertionListener, false); //IE
    //     document.addEventListener('webkitAnimationStart', insertionListener, false); //Chrome + Safari
    // }

    showSearchBox() {
        let controllerSearch = document.querySelector('#controlSearchBox + div'),
            triggeringElement = document.querySelector('#controlSearchBox'),
            searchBoxInvoice = document.querySelector('#searchBoxInvoice');

        if (controllerSearch.classList.contains('hideThis')) {
            controllerSearch.classList.remove('hideThis');
            controllerSearch.classList.add('fadeInDown');
            controllerSearch.classList.remove('fadeOutUp');
            triggeringElement.querySelector('i').classList.remove('fa-angle-down');
            triggeringElement.querySelector('i').classList.add('fa-angle-up');
            searchBoxInvoice.classList.add('spacerSearch');
        } else {
            controllerSearch.classList.remove('fadeInDown');
            controllerSearch.classList.add('fadeOutUp');
            triggeringElement.querySelector('i').classList.remove('fa-angle-up');
            triggeringElement.querySelector('i').classList.add('fa-angle-down');
            searchBoxInvoice.classList.remove('spacerSearch');
            setTimeout(function () { controllerSearch.classList.add('hideThis'); }, 500);
        }
    }

    viewDetails(data): void {
        let params = {
            id: data[0].Id,
            type: data[0].Type
        };
        this._route.navigate(['/app/transaction/invoice'], { queryParams: params });
    }

    displayTargetBox(event?: any): void {
        let triggerElement = event.target,
            targetElementID = triggerElement.getAttribute('openTarget');
        let targetElement = document.querySelector('#' + targetElementID);
        if (targetElement.classList.contains('hideThis')) {
            targetElement.classList.remove('hideThis');
            triggerElement.classList.add('btnSelected');
        } else {
            targetElement.classList.add('hideThis');
            triggerElement.classList.remove('btnSelected');
        }
    }
}
