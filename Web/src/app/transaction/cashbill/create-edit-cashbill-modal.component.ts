import { ProxyURL } from '@shared/service-proxies/generic-service-proxies-url';
import { GenericServiceProxy, RequestType } from '@shared/service-proxies/generic-service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { OnInit, ViewChild, Injector, Component, Output, EventEmitter } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'createOrEditCashBillModal',
    templateUrl: './create-edit-cashbill-modal.component.html',
    styleUrls: ['./create-edit-cashbill-modal.component.less']
})
export class CreateEditCashBillModalComponent extends AppComponentBase implements OnInit {

    @ViewChild('createOrEditCashBillModal', { static: true }) modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    loading = false;
    active = false;
    saving = false;
    priceDisabled = false;

    inputHelper: any = {};
    itemCombo: any;
    selectedItem: any;
    minPax = 1;

    itemComboUrl = ProxyURL.GetTenderItemCombo;

    constructor(
        injector: Injector,
        private _proxy: GenericServiceProxy
    ) {
        super(injector);
    }

    ngOnInit() {
        this._proxy.request(this.itemComboUrl, RequestType.Get)
            .subscribe((result) => {
                this.itemCombo = result;
            });
    }

    setnull() {
        this.selectedItem = '';
        this.inputHelper.paymentfor = '';
        this.inputHelper.description = '';
        this.inputHelper.sundb = '';
        this.inputHelper.taxcode = '';
        this.inputHelper.unitprice = 0;
        this.inputHelper.quantity = 1;
        this.inputHelper.taxamt = 0;
        this.inputHelper.subtotal = 0;
        this.inputHelper.total = 0;
    }

    paxChange(evt) {
        let minSet = this.minPax;
        let val = evt.target.value;

        if (val % 1 !== 0) {
            this.inputHelper.quantity = minSet;
        } else if (val < minSet) {
            this.inputHelper.quantity = minSet;
        } else {
            if (val = null) {
                this.inputHelper.quantity = minSet;
            }
        }
    }

    show(): void {
        this.setnull();

        this.active = true;
        this.modal.show();
    }

    onItemChange(data) {
        console.log('Item: ' + JSON.stringify(data));
        if (data.Amount !== 0) {
            this.inputHelper.unitprice = data.Amount;
            this.inputHelper.taxamt = data.TaxAmount;
            this.priceDisabled = true;
        } else {
            this.inputHelper.unitprice = 0;
            this.inputHelper.taxamt = data.TaxAmount;
            this.priceDisabled = false;
        }
    }

    save(): void {
        this.loading = true;

        this.inputHelper.paymentfor = this.selectedItem.RevCode;
        this.inputHelper.remark = this.selectedItem.Description;
        this.inputHelper.sundb = this.selectedItem.SunDB;
        this.inputHelper.taxcode = this.selectedItem.Unit;
        this.inputHelper.unitprice = Number(this.inputHelper.unitprice);
        this.inputHelper.quantity = Number(this.inputHelper.quantity);
        this.inputHelper.taxamt = Number(this.inputHelper.taxamt);
        let total = this.inputHelper.unitprice * this.inputHelper.quantity;
        let sst = (this.inputHelper.taxamt * total) / 100; // this.inputHelper.quantity;
        this.inputHelper.subtotal = total;
        this.inputHelper.totaltax = sst;
        this.inputHelper.total = total + sst;

        console.log('Item Save: ' + 'Total= ' + total + ' SST= ' + sst + ' Subtotal= ' + this.inputHelper.total);

        this.notify.success(this.l('ItemAdded'));
        this.modalSave.emit(this.inputHelper);
        this.close();
        this.loading = false;
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
