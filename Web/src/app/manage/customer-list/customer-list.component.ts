import { Component, Injector, ViewChild, OnInit, EventEmitter, Output } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { EntityDtoOfInt64, UserListDto, UserServiceProxy, PermissionServiceProxy, FlatPermissionDto } from '@shared/service-proxies/service-proxies';
import { GenericServiceProxy, RequestType } from '@shared/service-proxies/generic-service-proxies';
import { ActivatedRoute, Router } from '@angular/router';
import { ProxyURL } from '@shared/service-proxies/generic-service-proxies-url';
import { BaseListComponent } from '@app/shared/form/list/base-list.component';
import { CreateOrEditCustomerModalComponent } from './create-edit-customer-list.component'
import { PlatformLocation } from '@angular/common';
import { CustomerDetailComponent } from './customer-detail.component';
import { LazyLoadEvent } from 'primeng/public_api';

import * as _ from 'lodash';
import { finalize } from 'rxjs/operators';
import { EditCustomerModalComponent } from './edit-customer-modal.component';

@Component({
    templateUrl: './customer-list.component.html',
    animations: [appModuleAnimation()]
})
export class CustomerComponent extends AppComponentBase implements OnInit {

    @ViewChild('customerList', { static: false }) customerList: BaseListComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    @ViewChild('createOrEditCustomerModal', { static: true }) createOrEditCustomerModal: CreateOrEditCustomerModalComponent;
    @ViewChild('customerDetailModal', { static: true }) customerDetailModal:  CustomerDetailComponent;
    @ViewChild('editCustomerModal', { static: true}) editCustomerModal: EditCustomerModalComponent;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @Output() onLoadFinished: EventEmitter<any> = new EventEmitter<any>();

    baseUrl = this._platform.getBaseHrefFromDOM();
    gridUrl = ProxyURL.GetLGMCompany; //ProxyURL.GetCustomerList;
    customerListAUrl = ProxyURL.GetCustomerList_A;
    permissionView = 'Pages.CustomerList.View';
    permissionEdit = 'Pages.CustomerList.Edit';

    saveState: number;
    saveType: number;
    filter: any;
    //acctNum: any;

    url: any;
    strStatus: any;
    strDate: any;
    isPageLoading: boolean;
    calendarUrl: string;
    bizRegCookies: any;
    filterText: '';
    selectedData: any = [];
    status = false;
    role = '';
    onlyLockedUsers = false;

    accInList: any;
    statusCombo: any = [];
    packageCombo: any = [];
    selectedStatus: any;
    selectedPackage: any;

    constructor(
        injector: Injector,
        private _proxy: GenericServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _userServiceProxy: UserServiceProxy,
        private _route: Router,
        private _platform: PlatformLocation
    ) {
        super(injector);
        this.filterText = this._activatedRoute.snapshot.queryParams['filterText'] || '';
    }

    ngOnInit(): void {
        this.getWorkingAccount();

        let packageComboUrl = ProxyURL.GetItemCombo + 'ItemCode=CWC&';
        this._proxy.request(packageComboUrl, RequestType.Get)
            .subscribe(result => {
                this.packageCombo = result;
                this.packageCombo.unshift({ "Code": "All", "Remark": "All" });
                this.selectedPackage = "All";
            });

        let codeUrl = ProxyURL.GetCodeMaster + 'code=BZS&';
        this._proxy.request(codeUrl, RequestType.Get)
            .subscribe(result => {
                this.statusCombo = result;
                this.statusCombo.unshift({ "CodeType": "BZS", "Code": "0", "CodeDesc": "All", "CodeRemark": "All", "CodeSeq": 0, "CodeRef": "", "CodeVal1": "", "CodeVal2": "", "CodeVal3": "" });
                this.selectedStatus = "0";
            });

        //this.getStatus();
        //this.getCustomers();
    }

    getStatus() {
        if (this._activatedRoute.snapshot.queryParams['status'] != null) {
            this.strStatus = this._activatedRoute.snapshot.queryParams['status'];
            this.strDate = this._activatedRoute.snapshot.queryParams['date'];
        } else {
            this.strStatus = '';
            this.strDate = '';
        }
    }

    getWorkingAccount() {
        let waUrl = ProxyURL.GetListMaintenance + 'branchId=' + encodeURIComponent('06') + '&' + 'prefix=' + encodeURIComponent('WORKINGACCOUNT') + '&';
        this._proxy.request(waUrl, RequestType.Get)
            .subscribe(result => {
                this.accInList = result.items;
            });
    }

    getAccInList(acctNo) {
        return this.accInList.filter(x => x.SYSValue === acctNo).length === 0;
    }

    getCustomers(): void {
        this.gridUrl += 'isApprove=' + encodeURIComponent('' + -2) + '&';
        // this._proxy.request(this.gridUrl, RequestType.Get)
        //     .subscribe(result => {
        //         this.status = result.items[0].status;
        //         console.log('hasil : ' + this.status);
        //     })
        this.customerList.setURL(this.gridUrl);
    }

    // filterList(){
    //     this.customerListAUrl = ProxyURL.GetCustomerList_A;
    //     this.customerListAUrl += this.selectedStatus != "0"? 'status=' + encodeURIComponent(this.selectedStatus) + '&' : '';
    //     this.customerListAUrl += this.selectedPackage != "All"? 'package=' + encodeURIComponent(this.selectedPackage) + '&' : '';
    //     this.customerListAUrl += this.filterText != undefined? 'filter=' + encodeURIComponent(this.filterText) + '&' : '';

    //     setTimeout(() => {
    //          this.customerList.refresh();
    //     }, 100);
    // }

    getCompany(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        let url = this.gridUrl += 'isApprove=' + encodeURIComponent('' + -2) + '&';
        if (url !== undefined) {
            if (this.primengTableHelper.getSorting(this.dataTable) !== undefined) { url += 'Sorting=' + encodeURIComponent('' + this.primengTableHelper.getSorting(this.dataTable)) + '&'; }
            if (this.primengTableHelper.getSkipCount(this.paginator, event) !== undefined) { url += 'SkipCount=' + encodeURIComponent('' + this.primengTableHelper.getSkipCount(this.paginator, event)) + '&'; }
            if (this.primengTableHelper.getMaxResultCount(this.paginator, event) !== undefined) { url += 'MaxResultCount=' + encodeURIComponent('' + this.primengTableHelper.getMaxResultCount(this.paginator, event)) + '&'; }

        this.primengTableHelper.showLoadingIndicator();
        this._proxy.request(url, RequestType.Get)
        .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
        .subscribe(result => {
            this.primengTableHelper.columns = result.columns;
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
            this.onLoadFinished.emit();
        });
        }
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    updateList() {
        this.customerListAUrl = ProxyURL.GetCustomerList_A;
        this.customerListAUrl += this.selectedStatus != "0" ? 'status=' + encodeURIComponent(this.selectedStatus) + '&' : '';
        this.customerListAUrl += this.selectedPackage != "All" ? 'package=' + encodeURIComponent(this.selectedPackage) + '&' : '';

        this.customerList.setURL(this.customerListAUrl);
    }

    refresh(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        let url = this.gridUrl;
        if (url !== undefined) {
            if (this.filterText !== undefined) { url += 'Filter=' + encodeURIComponent('' + this.filterText) + '&'; }
            if (this.primengTableHelper.getSorting(this.dataTable) !== undefined) { url += 'Sorting=' + encodeURIComponent('' + this.primengTableHelper.getSorting(this.dataTable)) + '&'; }
            if (this.primengTableHelper.getSkipCount(this.paginator, event) !== undefined) { url += 'SkipCount=' + encodeURIComponent('' + this.primengTableHelper.getSkipCount(this.paginator, event)) + '&'; }
            if (this.primengTableHelper.getMaxResultCount(this.paginator, event) !== undefined) { url += 'MaxResultCount=' + encodeURIComponent('' + this.primengTableHelper.getMaxResultCount(this.paginator, event)) + '&'; }

            this.primengTableHelper.showLoadingIndicator();
            this._proxy.request(url, RequestType.Get)
                .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
                .subscribe(result => {
                    this.primengTableHelper.columns = result.columns;
                    this.primengTableHelper.totalRecordsCount = result.totalCount;
                    this.primengTableHelper.records = result.items;
                    this.primengTableHelper.hideLoadingIndicator();
                    this.onLoadFinished.emit();
                });
        }
    }

    createCustomer(): void {
        this.createOrEditCustomerModal.show();
    }

    viewEdit(data?: any, viewEdit?: string): void {
        this.createOrEditCustomerModal.show(data, viewEdit);
        //this.customerDetailModal.show(data, viewEdit);
        // console.log(data);
        // this._router.navigate(['/app/workorder/profilecompany/' + data.RefNo1]);
    }

    viewCompany(data: any): void {
        //console.log(JSON.stringify(data));
        this.customerDetailModal.show(data);
    }

    editCompany(data: any): void {
        this.editCustomerModal.show(data);
    }

    //delete(data): void {
    //     this.message.confirm(
    //         this.l('EditionDeleteWarningMessage', edition.displayName),
    //         this.l('AreYouSure'),
    //         isConfirmed => {
    //             if (isConfirmed) {
    //                 this._editionService.deleteEdition(edition.id).subscribe(() => {
    //                     this.getEditions();
    //                     this.notify.success(this.l('SuccessfullyDeleted'));
    //                 });
    //             }
    //         }
    //     );
    //}

    back(): void {
        this._route.navigate(['/app/workorder/weeklyplanner/' + this.strDate]);
    }

    onCompanyNameChange(data: string) {
    }

    searchFilter() {
        this.gridUrl = ProxyURL.GetLGMCompany + 'isApprove=' + encodeURIComponent('' + -2) + '&';
        this.filter != null ? this.gridUrl += 'Filter=' + encodeURIComponent('' + this.filter) + '&' : '';
        //console.log('Filter : ' + this.gridUrl);

        this.primengTableHelper.showLoadingIndicator();
        this._proxy.request(this.gridUrl, RequestType.Get)
            .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
            .subscribe(result => {
                this.primengTableHelper.columns = result.columns;
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.records = result.items;
                this.primengTableHelper.hideLoadingIndicator();
                this.onLoadFinished.emit();
            });
    }

    addToList(data): void {
        let addUrl = ProxyURL.AddSysPrefB;
        let contSysPrefB: any = {};
        contSysPrefB.BranchID = '06';
        contSysPrefB.SYSKey = 'WORKINGACCOUNT ' + data.AcctNo;
        contSysPrefB.SYSValue = data.AcctNo;
        contSysPrefB.SYSSet = 1;
        contSysPrefB.Flag = 1;

        if (addUrl !== undefined) {
            this.message.confirm(
            this.l('AddWorkingAccountConfirmation', data.AcctNo),
            this.l('AreYouSure'),
            isConfirmed => {
                if (isConfirmed) {
                this._proxy.request(addUrl, RequestType.Post, contSysPrefB)
                    .subscribe((result) => {
                    if (result = 'true') {
                        this.notify.success(this.l('AddWorkingAccountSuccess'));
                        this.getWorkingAccount();
                        this.getCompany();
                        // this.hideList = true;
                    }
                    });
                }
            }
            );
        }
    }

    activateCompany(data): void {
        let url = ProxyURL.ManualActivatingCompany;

        url += 'accountNumber=' + encodeURIComponent('' + data) + '&';
        console.log('hasil : ' + url);
        this._proxy.request(url, RequestType.Get)
          .subscribe((result) => {
            if (result.success = 'true') {
                this.message.success(this.l('ActivationSuccess'));
                this.getCompany();
            } else {
                this.message.error(this.l('ActivationFailed'));
            }
          });
    }

    onSelectedChange(data): void {
    this.selectedData = data;
        let params = {
            id: data
        };
        this._route.navigate(['/app/manage/employee'], { queryParams: params});
    }
}
