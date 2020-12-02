import { UserServiceProxy, RoleServiceProxy, RegisterInput, AccountServiceProxy, RegisterOutput } from '@shared/service-proxies/service-proxies';
import { ProxyURL } from '@shared/service-proxies/generic-service-proxies-url';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { AppComponentBase } from 'shared/common/app-component-base';
import { Component, Injector, ViewChild, OnInit, AfterViewInit, NgZone, Pipe, Output, EventEmitter } from '@angular/core';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { ThrowStmt } from '@angular/compiler';
import { SaveType } from '@shared/AppEnums';
import { GenericServiceProxy, RequestType } from '@shared/service-proxies/generic-service-proxies';
import { NgForm } from '@angular/forms';
import { format } from 'path';
import * as _ from 'lodash';

@Component({
    selector: 'CreateEditTender',
    templateUrl: './create-edit-tender-list.component.html',
    styleUrls: ['./create-edit-tender-list.component.less'],
    animations: [accountModuleAnimation()]
})

export class CreateEditTenderComponent extends AppComponentBase implements OnInit, AfterViewInit {
    
    @ViewChild('createModal', {static: true}) modal: ModalDirective;
    //@ViewChild('tenantCreateForm', {static: tr}) resetForm: NgForm;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    
    modalTitle = '';

    urlGet: string;
    inputHelper: any = {};
    input: any = {};
    saveState: number;
    saveType: number;
    
    loading = false;
    active = false;
    create = false;
    saving = false;
    view = false;

    comboLoadedCount = 0;
    disable: boolean;

    constructor(
      injector: Injector,
      private _proxy: GenericServiceProxy
    ) {
      super(injector);
    }

    ngOnInit(): void {
      
    }

    ngAfterViewInit(): void {
      
    }

    saveData(): void {
      if(this.modalTitle == 'CreateNewTender') {
        this.saveNew();
      } else {
        this.saveEdit();
      }
    }
    
    show(data?: any, viewEdit?: string): void {

      if (data == undefined) {
        this.modalTitle = 'CreateNewTender';
        this.saveState = SaveType.Insert;
        this.active = true;
        this.create = true;
        this.inputHelper = {};
        this.view = false;
      } else {
        if (viewEdit == 'view') {
          this.view = true;
          this.modalTitle = 'ViewTender';
        } else {
          this.view = false;
          this.modalTitle = 'EditTender';      
        }
        this.inputHelper = _.cloneDeep(data);
        this.saveState = SaveType.Update;
        this.inputHelper.tenderType = 11;
      }
      this.modal.show();
    }

    saveNew(): void{
      let data: any = [];
      
      this.input.itemCode = this.inputHelper.RevCode;
      this.input.classType = this.inputHelper.DatabaseTender;
      this.input.itemDesc = this.inputHelper.DescriptionTender;
      this.input.markNo = this.inputHelper.Unit;
      this.input.sellPrice = this.inputHelper.Amount;
      this.input.coRate1H = this.inputHelper.TaxAmount;

      data.push(this.input);
      //console.log('Hasil New : ' + JSON.stringify(data));

      let url = ProxyURL.CreateTender;

      this._proxy.request(url, RequestType.Post, data)
        .subscribe((result) => {
          if(result.success = 'true') {
            this.notify.success(this.l('CreateTenderSuccess'));
            this.modalSave.emit();
            this.close();
          } else {
            this.notify.error(this.l('CreateTenderFailed'));
          }
        });

    }

    saveEdit(): void{
      let data: any = [];

      this.input.itemCode = this.inputHelper.RevCode;
      this.input.classType = this.inputHelper.DatabaseTender;
      this.input.itemDesc = this.inputHelper.DescriptionTender;
      this.input.markNo = this.inputHelper.Unit;
      this.input.sellPrice = this.inputHelper.Amount;
      this.input.coRate1H = this.inputHelper.TaxAmount;

      data.push(this.input);
      //console.log('Hasil Edit : ' + JSON.stringify(data));

      let url = ProxyURL.UpdateTender;
      let notif = '';

      this._proxy.request(url, RequestType.Post, data)
        .subscribe((result) => {
          if(result.success = 'true') {
            this.notify.success(this.l('EditTenderSuccess'));
            this.modalSave.emit();
            this.close();
          } else {
            this.notify.error(this.l('EditTenderFailed'));
          }
        });
    }

    close(): void {
        this.active = false;
        //this.resetForm.reset();
        this.modal.hide();
    }

    

}
