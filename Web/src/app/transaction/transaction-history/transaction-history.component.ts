import { ProxyURL } from '../../../shared/service-proxies/generic-service-proxies-url';
import { BaseListComponent } from '@app/shared/form/list/base-list.component';
import { PermissionCheckerService } from 'abp-ng2-module';
import { Component, OnInit, AfterViewInit, Injector, ElementRef, ViewChild, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Router } from '@angular/router';
import * as moment from 'moment';
import { DatePipe } from '@angular/common';
import { GenericServiceProxy, RequestType } from '@shared/service-proxies/generic-service-proxies';
import { finalize, filter, tap, switchMap } from 'rxjs/operators';

// import * as _ from 'lodash';
import { RoleServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
    templateUrl: './transaction-history.component.html',
    styleUrls: ['./transaction-history.component.less'],
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()],
    providers: [DatePipe]
})
export class TransactionHistoryComponent extends AppComponentBase implements AfterViewInit, OnInit {

    @ViewChild('transactionHistoryComponent', { static: false }) transactionHistoryComponent: BaseListComponent;

    //gridUrl = ProxyURL.GetTransactionHistory + 'mode=' + encodeURIComponent('' + 1) + '&';
    gridUrl = undefined;
    compUrl = ProxyURL.GetLGMCompany;

    isPageLoading: boolean;
    loading = false;

    currency = 'RM';
    filterText = '';

    cookie: any;
    company: any;

    __proxy: any;
    inputHelper: any;

    constructor(
        injector: Injector,
        private _proxy: GenericServiceProxy,
        private _permissionChecker: PermissionCheckerService,
        private _roleProxy: RoleServiceProxy,
        private _route: Router,
        private datePipe: DatePipe
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.isPageLoading = true;
        let bizRegCookies = this.appStorage;
        this.getHistoryList(bizRegCookies);

        //this.cookie = this._cookies.getBizCookies();

        // if (this.cookie !== null) {
        //     this.compUrl += 'bizRegID=' + this.cookie.bizRegID + '&';
        //     this._proxy.request(this.compUrl, RequestType.Get)
        //         .pipe(
        //             tap(result1 => {
        //                 this.company = result1.CompanyName;
        //                 if (this.company !== 'LGM') {
        //                     this.gridUrl += 'company=' + encodeURIComponent(this.company) + '&';
        //                 }
        //             }),
        //             switchMap((a) => this._proxy.request(this.gridUrl += 'customerID=' + this.cookie.bizRegID + '&', RequestType.Get)),
        //             tap(result => {
        //                 this.transactionHistoryComponent.refresh();
        //             }),
        //             (finalize(() => {
        //                 this.isPageLoading = false;
        //             })))
        //         .subscribe(result => {
        //             this.transactionHistoryComponent.refresh();
        //         });
        //     }
    }

    ngAfterViewInit(): void {
    }

    getHistoryList(bizCookies) {
        if (bizCookies.bizRegID === null) {
            this.gridUrl = ProxyURL.GetTransactionHistory + 'mode=1' + '&';
            this._proxy.request(this.gridUrl, RequestType.Get)
                .pipe(finalize(() => { this.isPageLoading = false; }))
                .subscribe(result => {
                    this.transactionHistoryComponent.refresh();
                });
        } else if (bizCookies.bizRegID !== null) {
            this.compUrl += 'bizRegID=' + bizCookies.bizRegID + '&';
            this.gridUrl = ProxyURL.GetTransactionHistory;
            this.gridUrl += 'customerID=' + encodeURIComponent(bizCookies.bizRegID) + '&' + 'mode=1' + '&';
            this._proxy.request(this.gridUrl, RequestType.Get)
                .pipe(finalize(() => { this.isPageLoading = false; }))
                .subscribe(result => {
                    this.transactionHistoryComponent.refresh();
                });
        }
    }

    detail(data): void {
        let params = {
            transno: data[0].TransNo
            //status: data[0].Status
        };
        this._route.navigate(['/app/transaction/invoice'], { queryParams: params });
    }

}
