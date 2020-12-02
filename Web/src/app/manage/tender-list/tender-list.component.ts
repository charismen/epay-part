import { ProxyURL } from '../../../shared/service-proxies/generic-service-proxies-url';
import { BaseListComponent } from '@app/shared/form/list/base-list.component';
import { PermissionCheckerService } from 'abp-ng2-module';
import { Component, OnInit, AfterViewInit, Injector, ElementRef, ViewChild, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Router, ActivatedRoute } from '@angular/router';
import * as moment from 'moment';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { DatePipe, PlatformLocation } from '@angular/common';
import { GenericServiceProxy, RequestType } from '@shared/service-proxies/generic-service-proxies';
import { finalize, filter, tap, switchMap } from 'rxjs/operators';
import { CreateEditTenderComponent } from './create-edit-tender-list.component';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { BizRegBizLoc } from '@shared/AppEnums';


@Component({
    templateUrl: './tender-list.component.html',
    styleUrls: ['./tender-list.component.less'],
    animations: [appModuleAnimation()]
})

export class TenderListComponent extends AppComponentBase implements AfterViewInit, OnInit {

    @ViewChild('TenderList', { static: false }) TenderList: BaseListComponent;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    @ViewChild('createEditTender', { static: true }) createEditTender: CreateEditTenderComponent;

    permissionView = 'Pages.Inventory.Tender.View';
    permissionEdit = 'Pages.Inventory.Tender.Edit';
    permissionDelete = 'Pages.Inventory.Tender.Delete';

    gridUrl = '';
    record: any;
    isPageLoading: boolean;
    inputHelper: any;
    tenderitem: any;
    bizRegID: string;
    inputFilter: any;
    //gridUrl = ProxyURL.GetTender;

    constructor(
        injector: Injector,
        private _proxy: GenericServiceProxy
    ) {
        super(injector);
    }

    ngOnInit(): void {

        this.populateData();

    }

    populateData() {
        this.gridUrl = ProxyURL.GetTender;
    }

    ngAfterViewInit(): void {

    }

    refresh(): void {
        this.TenderList.refresh();
    }

    viewEditTender(data?: any, viewEdit?: string): void {
        this.createEditTender.show(data, viewEdit);
    }

    createTender(): void {
        this.createEditTender.show();
    }

    searchTender() {
        this.gridUrl = ProxyURL.GetTender;
        this.inputFilter != null ? this.gridUrl += 'Filter=' + encodeURIComponent('' + this.inputFilter) + '&' : '';

        setTimeout(() => {
            this.TenderList.refresh();
        }, 50);
    }

    delete(data): void {
        this.message.confirm(
            this.l('TenderItemDeleteWarningMessage', data.DescriptionTender),
            this.l('AreYouSure'),
            isConfirmed => {
                if (isConfirmed) {
                    data.ItemCode = data.RevCode;
                    data.Active = 0;
                    data.Flag = 0;
                    let dataTender: any = [];
                    dataTender.push(data);

                    let url = ProxyURL.UpdateTender;
                    let notif = 'SuccessfullyDeleted';

                    if (url !== undefined) {
                        this._proxy.request(url, RequestType.Post, dataTender)
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
}
