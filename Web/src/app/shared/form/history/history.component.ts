import { Component, Injector, OnInit, ElementRef, ViewChild, AfterViewInit, Input, Output, EventEmitter } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { GenericServiceProxy, RequestType } from '@shared/service-proxies/generic-service-proxies';
import { ViewEncapsulation } from '@angular/core';
import { BaseListComponent } from '@app/shared/form/list/base-list.component';
import { ProxyURL } from '@shared/service-proxies/generic-service-proxies-url';
import * as _ from 'lodash';
import * as moment from 'moment';;
import { EntityChangeType, EntityChangeListDto } from '@shared/service-proxies/service-proxies';
import { HistoryDetailModalComponent } from './history-detail-modal.component';

@Component({
    templateUrl: './history.component.html',
    selector: 'history',
    styleUrls: ['./history.component.less'],
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})

export class HistoryComponent extends AppComponentBase implements OnInit, AfterViewInit {
    @ViewChild('baseList', { static: false }) baseList: BaseListComponent;
    @ViewChild('historyDetailModal', { static: true }) historyDetailModal: HistoryDetailModalComponent;

    @Input() id = '';
    @Input() name = '';
    @Input() detailUrl = ProxyURL.GetHistoryDTL;

    gridUrl = ProxyURL.GetHistoryHDR;
    gridUrlDtl = ProxyURL.GetHistoryDTL;

    selectedActionType: any;
    comboModelActionType: any = [];
    selectedActionBy: any;
    comboModelActionBy: any = [];

    startDate = undefined; //moment(new Date()).format('YYYY-MM-DD');
    endDate = undefined; //moment(new Date()).format('YYYY-MM-DD');

    constructor(
        injector: Injector,
        private _activatedRoute: ActivatedRoute,
        private _route: Router,
        private __proxy: GenericServiceProxy,
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.gridUrl = ProxyURL.GetHistoryHDR + 'entityID=' + encodeURIComponent(this.id) + '&' + 'entityName=' + encodeURIComponent(this.name) + '&';
    }

    ngAfterViewInit() {
        this.__proxy.request(ProxyURL.GetHistoryActionCombo + 'entityID=' + encodeURIComponent(this.id) + '&' + 'entityName=' + encodeURIComponent(this.name) + '&',
            RequestType.Get)
            .pipe().subscribe(result => {
                this.comboModelActionType = result;
                this.comboModelActionType.unshift({ 'Code': '', 'Remark': 'All' });
                this.selectedActionType = this.comboModelActionType[0].Code;
            });
        this.__proxy.request(ProxyURL.GetHistoryUserCombo + 'entityID=' + encodeURIComponent(this.id) + '&' + 'entityName=' + encodeURIComponent(this.name) + '&',
            RequestType.Get)
            .pipe().subscribe(result => {
                this.comboModelActionBy = result;
                this.comboModelActionBy.unshift({ 'Code': '', 'Remark': 'All' });
                this.selectedActionBy = this.comboModelActionBy[0].Code;
            });
    }

    refresh(event?: any): void {
        if (event !== null && event !== undefined) {
            this.baseList.setURL(this.setUrl());
            this.baseList.refresh();
        }
    }

    setUrl(): string {
        let url = this.gridUrl;
        if (this.selectedActionType !== undefined) { url += 'actionType=' + encodeURIComponent(this.selectedActionType) + '&'; }
        if (this.selectedActionBy !== undefined) { url += 'actionBy=' + encodeURIComponent(this.selectedActionBy) + '&'; }
        if (this.startDate !== undefined) { url += 'startDate=' + encodeURIComponent(moment(new Date(this.startDate)).format('YYYY-MM-DD')) + '&'; }
        if (this.endDate !== undefined) { url += 'startDate=' + encodeURIComponent(moment(new Date(this.endDate)).format('YYYY-MM-DD')) + '&'; }
        return url;
    }

    onDetailClick(data?: any): void {
        let obj: any = {};
        obj.userId = data.UserId;
        obj.userName = data.ActionBy;
        obj.changeTime = data.Time;
        obj.entityTypeFullName = data.EntityTypeFullName.split(',')[0];
        obj.changeType = data.ChangeType;
        (<any>this).changeTypeName = EntityChangeType[data.ChangeType];
        obj.entityChangeSetId = data.ChangeId;

        this.historyDetailModal.show(obj);
    }
}
