import { ActivatedRoute } from '@angular/router';
import { Component, Injector, ViewChild, AfterViewInit, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { finalize, tap, switchMap, switchMapTo } from 'rxjs/operators';
import { GenericServiceProxy, RequestType } from '@shared/service-proxies/generic-service-proxies';
import { ProxyURL } from '@shared/service-proxies/generic-service-proxies-url';
import { BaseListComponent } from '@app/shared/form/list/base-list.component';
import { CreateOrEditEmployeeModalComponent } from './create-edit-employee-list.component';
import { PlatformLocation, Location } from '@angular/common';
import { BizRegBizLoc } from '@shared/AppEnums';

@Component({
    templateUrl: './employee-list.component.html',
    styleUrls: ['./employee-list.component.less'],
    animations: [appModuleAnimation()]
})
export class EmployeeComponent extends AppComponentBase implements OnInit, AfterViewInit {

    @ViewChild('employeeList', { static: false }) employeeList: BaseListComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    @ViewChild('createOrEditEmployeeModal', { static: true }) createOrEditEmployeeModal: CreateOrEditEmployeeModalComponent;

    baseUrl = this._platform.getBaseHrefFromDOM();
    employeeListUrl = ProxyURL.GetEmployee;
    permissionView = 'Pages.EmployeeList.View';
    permissionEdit = 'Pages.EmployeeList.Edit';
    permissionDelete = 'Pages.EmployeeList.Delete';

    strStatus: any;
    isPageLoading: boolean;
    filterText: '';

    statusCombo: any = [];
    packageCombo: any = [];
    selectedStatus: any;
    selectedPackage: any;

    bizRegID: string;

    constructor(
        injector: Injector,
        private _proxy: GenericServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _platform: PlatformLocation,
        private _bizRegBizLoc: BizRegBizLoc,
        private _location: Location
    ) {
        super(injector);
        this._activatedRoute.queryParams.subscribe(params => {
            this.bizRegID = params.id; // params['id'];
        });
    }

    ngOnInit(): void {
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

        this.getEmployees();
    }

    ngAfterViewInit(): void {
    }

    getEmployees(): void {
        if (this.bizRegID !== undefined) {
            this.employeeListUrl += 'bizRegID=' + encodeURIComponent('' + this.bizRegID) + '&' + 'status=' + '1' + '&';
        }
        this.employeeList.refresh();
    }

    refresh(): void {
        this.employeeList.refresh();
    }

    createEmployee(): void {
        if (this.bizRegID !== undefined) {
            this._bizRegBizLoc.BizRegID = this.bizRegID;
        }
        this.createOrEditEmployeeModal.show();
    }

    viewEditEmployee(data?: any, viewEdit?: string): void {
        this.createOrEditEmployeeModal.show(data, viewEdit);
    }

    delete(data): void {
        this.message.confirm(
            this.l('EmployeeDeleteWarningMessage', data.Name !== undefined ? data.Name : data.FullName),
            this.l('AreYouSure'),
            isConfirmed => {
                if (isConfirmed) {
                    data.Active = 0;
                    data.Flag = 0;
                    let dataEmployee: any = [];
                    dataEmployee.push(data);
                    //console.log('dataSave: ' + JSON.stringify(data));
                    let url = ProxyURL.CreateToAssignEmployee;
                    let notif = 'SuccessfullyDeleted';

                    if (url !== undefined) {
                        this._proxy.request(url, RequestType.Post, dataEmployee)
                            .subscribe((result) => {
                                if (result.success) {
                                    this.notify.success(this.l(notif));
                                    this.refresh();
                                } else {
                                    this.notify.error(this.l(result.message));
                                }
                            });
                    }
                }
            }
        );
    }

    goBack(): void {
        this._location.back();
    }
}
