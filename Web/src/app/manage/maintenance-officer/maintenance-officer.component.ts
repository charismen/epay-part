import { ProxyURL } from './../../../shared/service-proxies/generic-service-proxies-url';
import { BaseListComponent } from './../../shared/form/list/base-list.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Component, OnInit, ViewEncapsulation, Injector, ViewChild } from '@angular/core';
import { GenericServiceProxy, RequestType } from '@shared/service-proxies/generic-service-proxies';

@Component({
  templateUrl: './maintenance-officer.component.html',
  styleUrls: ['./maintenance-officer.component.less'],
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()]
})
export class MaintenanceOfficerComponent extends AppComponentBase implements OnInit {

  @ViewChild('baseList', { static: false }) baseList: BaseListComponent;
  @ViewChild('searchList', { static: false }) searchList: BaseListComponent;

  accNotIn: any;
  cLoading = false;
  hideList = true;
  gridUrl = ProxyURL.GetListMaintenance + 'branchId=' + encodeURIComponent('06') + '&' + 'prefix=' + encodeURIComponent('ADR') + '&';
  officerAcc = '';
  permissionDelete = 'Pages.Maintenance.Delete';

  companyUrl = ProxyURL.GetCompanyDetail;
  getCompUrl = '';

  constructor(
    injector: Injector,
    private _proxy: GenericServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {
    this._proxy.request(this.gridUrl, RequestType.Get)
      .subscribe((result) => {
        this.accNotIn = result.items.map(x => x.SYSKey);
      });
  }

  searchCompany(data) {
    this.getCompUrl = ProxyURL.GetCustomer + 'Filter=' + encodeURIComponent('' + data) + '&';
    this.getCompUrl += 'AccountNotIn=' + encodeURIComponent('' + this.accNotIn) + '&';

    setTimeout(() => {
      this.searchList.refresh();
      this.hideList = false;
    }, 50);
  }

  addOfficerToList(data) {
    let addUrl = ProxyURL.AddSysPrefB;
    let contSysPrefB: any = {};
    contSysPrefB.BranchID = '06';
    contSysPrefB.SYSKey = data[0].ADD_CODE.trim();
    contSysPrefB.SYSValue = data[0].ADDRESS_1.trim();
    contSysPrefB.SYSSet = 1;
    contSysPrefB.Flag = 1;

    if (data[0].ADD_CODE.trim().substr(0, 3) !== 'ADR') {
      this.message.warn(this.l('OfficerPrefixError'));
    } else {
      if (addUrl !== undefined) {
        this.message.confirm(
          this.l('AddOfficerConfirmation', data[0].ADD_CODE.trim()),
          this.l('AreYouSure'),
          isConfirmed => {
            if (isConfirmed) {
              this._proxy.request(addUrl, RequestType.Post, contSysPrefB)
                .subscribe((result) => {
                  if (result = 'true') {
                    this.notify.success(this.l('AddOfficerSuccess'));
                    this.baseList.refresh();
                    this.hideList = true;
                  }
                });
            }
          }
        );
      }
    }
  }

  delete(event) {
    let delUrl = ProxyURL.DeleteSysPrefB;

    if (delUrl !== undefined) {
      this.message.confirm(
        this.l('DeleteOfficerConfirmation', event.SYSKey),
        this.l('AreYouSure'),
        isConfirmed => {
          if (isConfirmed) {
            this._proxy.request(delUrl, RequestType.Post, event)
              .subscribe((result) => {
                if (result = 'true') {
                  this.notify.success(this.l('DeleteOfficerSuccess'));
                  this.baseList.refresh();
                }
              });
          }
        }
      );
    }
  }
}
