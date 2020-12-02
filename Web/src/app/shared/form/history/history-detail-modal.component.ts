import { Component, Injector, ViewChild, Input } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { EntityChangeType } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';;
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GenericServiceProxy, RequestType } from '@shared/service-proxies/generic-service-proxies';
import { ProxyURL } from '@shared/service-proxies/generic-service-proxies-url';

@Component({
    selector: 'history-detail-modal',
    templateUrl: './history-detail-modal.component.html'
})
export class HistoryDetailModalComponent extends AppComponentBase {

    @ViewChild('historyDetailModal', {static: true}) modal: ModalDirective;

    @Input() detailUrl = ProxyURL.GetHistoryDTL;

    active = false;
    entityPropertyChanges: any = [];
    entityChange: any = {};
    changeTypeName: string;

    constructor(
        injector: Injector,
        private __proxy: GenericServiceProxy,
    ) {
        super(injector);
    }

    getPropertyChangeValue(propertyChangeValue, propertyTypeFullName) {
        if (!propertyChangeValue) {
            return propertyChangeValue;
        }
        propertyChangeValue = propertyChangeValue.replace(/^['"]+/g, '').replace(/['"]+$/g, '');
        if (this.isDate(propertyChangeValue, propertyTypeFullName)) {
            return moment(propertyChangeValue).format('YYYY-MM-DD HH:mm:ss');
        }

        if (propertyChangeValue === 'null') {
            return '';
        }

        return propertyChangeValue;
    }

    isDate(date, propertyTypeFullName): boolean {
        return propertyTypeFullName.includes('DateTime') && !isNaN(Date.parse(date).valueOf());
    }

    show(record: any): void {
        const self = this;
        self.active = true;
        self.entityChange = record;
        self.changeTypeName = EntityChangeType[self.entityChange.changeType].toString();

        this.__proxy.request(this.detailUrl + 'entityChangeID=' + encodeURIComponent('' + record.entityChangeSetId) + '&',
            RequestType.Get)
            .pipe().subscribe(result => {
                self.entityPropertyChanges = result;
            });

        self.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
