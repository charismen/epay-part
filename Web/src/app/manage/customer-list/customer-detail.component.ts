import { ProxyURL } from './../../../shared/service-proxies/generic-service-proxies-url';
import { BaseListComponent } from '@app/shared/form/list/base-list.component';
import { PermissionCheckerService } from 'abp-ng2-module';
import { Component, OnInit, AfterViewInit, EventEmitter, Injector, Output, ElementRef, ViewChild, ViewEncapsulation, Type } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Router, ActivatedRoute } from '@angular/router';
import * as moment from 'moment';
import { DatePipe, PlatformLocation } from '@angular/common';
import { GenericServiceProxy, RequestType } from '@shared/service-proxies/generic-service-proxies';
import { RoleServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize, tap, switchMap, concatMap, filter } from 'rxjs/operators';
import { toISOFormat } from '@shared/helpers/DateTimeHelper';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { SaveType } from '@shared/AppEnums';

@Component({
  selector: 'customerDetailModal',
  templateUrl: './customer-detail.component.html',
  styleUrls: ['./customer-detail.component.less'],
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()]
})
export class CustomerDetailComponent extends AppComponentBase implements OnInit, AfterViewInit {

  @ViewChild('createModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  loading = false;
  view = false;
  saving = false;
  bizRegID: string;
  bizLocID: string;
  statusIcon = '';
  modalTitle = '';
  saveState: number;
  saveType: number;
  statusdesc: any;
  statusicon: any;
  active = false;
  inputHelper: any = {};
  transporterUrl = ''; // ProxyURL.GetTransporterList;
  selectedTransporterUrl = ''; //ProxyURL.GetSelectedTransporter;
  baseUrl = this._platform.getBaseHrefFromDOM();

  constructor(
    injector: Injector,
    private _activatedRoute: ActivatedRoute,
    private _platform: PlatformLocation
  ) {
    super(injector);
    this._activatedRoute.params.subscribe(params => {
      this.bizRegID = params['BizRegID'];
      this.bizLocID = params['BizLocID'];
    });
  }

  ngOnInit(): void {

  }

  ngAfterViewInit(): void {

  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }

  show(data: any): void {

    this.modal.show();
    this.inputHelper.BizRegID = data.BizRegID;
    this.inputHelper.BizLocID = data.BizLocID;
    this.inputHelper.CompanyName = data.CompanyName;
    this.inputHelper.AcctNo = data.AcctNo;
    this.inputHelper.Address = data.Address1 + ' ' + data.Address2 + ' ' + data.Address3 + ' ' + data.Address4;
    this.inputHelper.Telno = data.TelNo;
    this.inputHelper.Faxno = data.FaxNo;
    this.inputHelper.Email = data.Email;
    this.inputHelper.PostalCode = data.PostalCode;
    this.inputHelper.Country = data.Country;
    this.inputHelper.State = data.State;
    this.inputHelper.PBT = data.PBT;
    this.inputHelper.ContactPerson = data.ContactPerson;
    this.inputHelper.ContactPerson2 = data.ContactPerson2;
    this.inputHelper.ContactDesignation = data.ContactDesignation;
    this.inputHelper.Designation = data.Designation;
    this.inputHelper.RegNo = data.RegNo;
    this.inputHelper.ContactPersonTelNo = data.ContactPersonTelNo;
    this.inputHelper.CountryDesc = data.CountryDesc;
    this.inputHelper.PostalCode = data.PostalCode;
    this.inputHelper.CityDesc = data.CityDesc;
    this.inputHelper.StateDesc = data.StateDesc;
    this.statusdesc = data.StatusDesc;
    this.inputHelper.RefNo1 = data.RefNo1;
  }
}
