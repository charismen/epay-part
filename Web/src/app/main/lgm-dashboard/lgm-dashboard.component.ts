import { DashboardFilter } from '@shared/AppEnums';
import { ProxyURL } from '@shared/service-proxies/generic-service-proxies-url';
import { BaseListComponent } from '@app/shared/form/list/base-list.component';
import { AfterViewInit, Component, Injector, ViewChild, ViewEncapsulation, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Router } from '@angular/router';
import * as moment from 'moment';
import { GenericServiceProxy, RequestType } from '@shared/service-proxies/generic-service-proxies';
import { finalize, tap, switchMap, concatMap } from 'rxjs/operators';
import { Observable, combineLatest as _observableCombineLatest, of as _observableOf } from 'rxjs';

// import * as _ from 'lodash';

@Component({
    templateUrl: './lgm-dashboard.component.html',
    styleUrls: ['./lgm-dashboard.component.less'],
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class LGMDashboardComponent extends AppComponentBase implements AfterViewInit, OnInit {

    @ViewChild('baseList', { static: false }) baseList: BaseListComponent;

    gridUrl: string; //ProxyURL.GetDashboardInvoice;
    chartUrl = ProxyURL.GetDashboardInvoice;

    bizCookies = null;

    totalAmountDue = 0;
    totalCreditBill = 0;
    totalProforma = 0;
    totalOverdue = 0;

    totalInvoice = 0;
    isPageLoading: boolean;
    chartData: any;
    chartOptions: {};
    selectedInvoiceType: any;
    selectedCustomer: any;
    dateFrom: Date;
    dateTo: Date;
    typeCombo: any[];
    customerCombo: any;

    errorDate = '';
    single: any[];
    view: any[] = [700, 400];

    colorScheme = {
        domain: ['#5AA454', '#A10A28', '#C7B42C', '#AAAAAA']
    };

    totalProformaCount: any;
    totalCreditBillCount: any;
    totalOverdueCount: any;

    constructor(
        injector: Injector,
        private _proxy: GenericServiceProxy,
        private _router: Router,
        private _dashboardFilter: DashboardFilter
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

        this._proxy.request(ProxyURL.GetCostumerCombo, RequestType.Get)
            .pipe().subscribe(result => {
                this.customerCombo = result;
            });

        this.loadChartData(35, 30, 35);

        this.bizCookies = this.appStorage;

        //Filter Dashboard
        if (this._dashboardFilter.InvoiceType != null && this._dashboardFilter.InvoiceType !== '' || this._dashboardFilter.Customer != null && this._dashboardFilter.Customer !== '' || this._dashboardFilter.BillingDateFrom != null || this._dashboardFilter.BillingDateTo != null) {
            if (this._dashboardFilter.InvoiceType != null && this._dashboardFilter.InvoiceType !== '') {
                this.selectedInvoiceType = this._dashboardFilter.InvoiceType;
            }
            if (this._dashboardFilter.Customer != null && this._dashboardFilter.Customer !== '') {
                this.selectedCustomer = this._dashboardFilter.Customer;
            }
            if (this._dashboardFilter.BillingDateFrom != null) {
                this.dateFrom = this._dashboardFilter.BillingDateFrom;
            }
            if (this._dashboardFilter.BillingDateTo != null) {
                this.dateTo = this._dashboardFilter.BillingDateTo;
            }
            this.gridUrl = ProxyURL.GetInvoice + 'mode=1' + '&';
            this.isGranted('Pages.Account.Payment.Offline') === true ? '' : this.gridUrl += 'Filter=' + encodeURIComponent('' + this.appStorage.bizRegID) + '&';
            this.selectedInvoiceType != null ? this.gridUrl += 'billType=' + encodeURIComponent('' + this.selectedInvoiceType.Code) + '&' : '';
            this.selectedCustomer != null ? this.gridUrl += 'Filter=' + encodeURIComponent('' + this.selectedCustomer.Code) + '&' : '';
            this.dateFrom != null && this.dateTo != null ? this.gridUrl += 'dateStart=' + moment(this.dateFrom).format('YYYY-MM-DD') + '&dateEnd=' + moment(this.dateTo).format('YYYY-MM-DD') + '&' : '';

            //console.log('hai : '+ this._dashboardFilter);
        } else {
            this.gridUrl = this.bizCookies.bizRegID === null ? ProxyURL.GetDashboardInvoice : ProxyURL.GetDashboardInvoice + 'customerCode=' + encodeURIComponent(this.bizCookies.bizRegID) + '&';
        }
        this.populateChart();
    }

    populateChart(filteredUrl?: string) {
        if (filteredUrl !== undefined) {
            console.log('Filter Chart: ' + filteredUrl);
            this.gridUrl = filteredUrl;
        }

        this._proxy.request(this.gridUrl, RequestType.Get)
            .pipe(finalize(() => {
                this.isPageLoading = false;
            }))
            .subscribe((result) => {
                this.baseList.refresh();
                this.totalInvoice = result.totalCount;
                this.totalAmountDue = result.items[0].TotalAmount != null ? result.items[0].TotalAmount : 0;
                this.totalOverdue = result.items[0].TotalOverdue != null ? result.items[0].TotalOverdue : 0;
                this.totalCreditBill = result.items[0].CreditBillAmount !== null ? result.items[0].CreditBillAmount : 0;
                this.totalProforma = result.items[0].ProformaAmount !== null ? result.items[0].ProformaAmount : 0;

                let creditBill = result.items[0].CreditBillCount;
                let proforma = result.items[0].ProformaCount;
                let overdue = result.items[0].OverdueCount;
                this.totalProformaCount = proforma;
                this.totalCreditBillCount = creditBill;
                this.totalOverdueCount = overdue;
                this.loadChartData(creditBill, proforma, overdue);
                // console.log(JSON.stringify(this.selectedCustomer));
            });
    }

    async cardAllEqualHeight() {
        let allCards = document.querySelectorAll('.cardFullHeight');
        let heightBenchmark = allCards[0].clientHeight;

        for (let i = 0; i < allCards.length; i++) {
            if (allCards[i].clientHeight < heightBenchmark) {
                allCards[i].setAttribute('style', 'min-height:' + heightBenchmark + 'px');
            }
        }
    }

    // async assignModalColumn(colName) {
    //     let insertionListener = function (event) {
    //         //Making sure that this is the animation we want.
    //         if (event.animationName === "tableReady") {

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
    //     }
    //     document.addEventListener("animationstart", insertionListener, false); //standard + firefox
    //     document.addEventListener("MSAnimationStart", insertionListener, false); //IE
    //     document.addEventListener("webkitAnimationStart", insertionListener, false); //Chrome + Safari
    // }

    async ngAfterViewInit() {
        this.isPageLoading = true;
    }

    loadChartData(creditbill: number, proforma: number, overdue: number): void {
        //console.log('credit : ' + creditbill);
        //console.log('porforma : ' + proforma);
        this.chartData = {
            labels: [],
            datasets: [
                {
                    data: [creditbill, proforma, overdue],
                    backgroundColor: [
                        '#028d96',
                        '#00709e',
                        '#ef9c00'
                    ],
                    hoverBackgroundColor: [
                        '#028d96',
                        '#00709e',
                        '#ef9c00'
                    ]
                }
            ],
        };

        this.chartOptions = {
            responsive: true,
            labels: 'none',
            aspectRatio: 1,
            cutoutPercentage: 76,
        };
    }

    dateRangeCheck(from: Date, to: Date, msg: string) {
        (to < from) ? (this.errorDate = msg) : (this.errorDate = '');
    }

    checkDateFrom(data?: Date) {
        this.dateRangeCheck(data, this.dateTo, '"Date To" Must Be After "Date From"');
    }

    checkDateTo(data?: Date) {
        this.dateRangeCheck(this.dateFrom, data, '"Date To" Must Be After "Date From"');
    }

    onInvoiceTypeChange(data: string) {
    }

    onCustomerChange(data?: any) {
    }

    searchFilter() {
        this.gridUrl = ProxyURL.GetInvoice + 'mode=1' + '&';
        this.isGranted('Pages.Account.Payment.Offline') === true ? '' : this.gridUrl += 'Filter=' + encodeURIComponent('' + this.appStorage.bizRegID) + '&';
        this.selectedInvoiceType != null ? this.gridUrl += 'billType=' + encodeURIComponent('' + this.selectedInvoiceType.Code) + '&' : '';
        this.selectedCustomer != null ? this.gridUrl += 'Filter=' + encodeURIComponent('' + this.selectedCustomer.Code) + '&' : '';
        this.dateFrom != null && this.dateTo != null ? this.gridUrl += 'dateStart=' + moment(this.dateFrom).format('YYYY-MM-DD') + '&dateEnd=' + moment(this.dateTo).format('YYYY-MM-DD') + '&' : '';

        this._dashboardFilter.InvoiceType = this.selectedInvoiceType;
        this._dashboardFilter.Customer = this.selectedCustomer;
        this._dashboardFilter.BillingDateFrom = this.dateFrom;
        this._dashboardFilter.BillingDateTo = this.dateTo;
        //console.log( this._dashboardFilter);

        setTimeout(() => {
            this.populateChart(this.gridUrl);
            this.baseList.refresh();
        }, 100);
    }


    quickpay(data?: any): void {
        this._router.navigate(['/app/transaction/quickpayment']);
    }

    clickPay(data): void {
        let params = {
            id: data[0].SiNumber
        };
        this._router.navigate(['/app/transaction/invoice'], { queryParams: params });
    }

    viewAllRecord(): void {
        this._router.navigate(['/app/transaction/invoice-listing']);
    }
}
