import { ViewOrEditApprovalModalComponent } from './view-edit-approval-modal.component';
import { ProxyURL } from '@shared/service-proxies/generic-service-proxies-url';
import { BaseListComponent } from '@app/shared/form/list/base-list.component';
import { AfterViewInit, Component, Injector, ViewChild, ViewEncapsulation, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { Paginator } from 'primeng/components/paginator/paginator';
import { Table } from 'primeng/components/table/table';
import { Router } from '@angular/router';
import { GenericServiceProxy, RequestType } from '@shared/service-proxies/generic-service-proxies';
import { finalize, tap, switchMap, concatMap } from 'rxjs/operators';
import { ModalDirective } from 'ngx-bootstrap';
import * as _ from 'lodash';

@Component({
    templateUrl: './approval.component.html',
    styleUrls: ['./approval.component.less'],
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class ApprovalComponent extends AppComponentBase implements AfterViewInit, OnInit {
    @ViewChild('approvalListComponent', { static: false }) approvalListComponent: BaseListComponent;
    @ViewChild('viewOrEditApprovalModal', {static: true}) viewOrEditApprovalModal: ViewOrEditApprovalModalComponent;
    @ViewChild('baseList', { static: false }) baseList: BaseListComponent;

    saveState: number;
    saveType: number;

    modalTitle = '';

    inputHelper: any = {};

    gridUrl = ProxyURL.GetLGMCompany;

    constructor(
        injector: Injector,
        private _proxy: GenericServiceProxy,
        private _router: Router
    ) {
        super(injector);
    }

    ngOnInit(): void {
    }

    ngAfterViewInit(): void {
    }

    refresh(): void {
        this.baseList.refresh();
    }

    onSelectedChange(data?: any) {
        console.log('Change: ' + JSON.stringify(data));
    }

    edit(data: any): void {
    }
}
