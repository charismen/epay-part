import { RequestType } from './../../../shared/service-proxies/generic-service-proxies';
import { ProxyURL } from '@shared/service-proxies/generic-service-proxies-url';
import { BaseListComponent } from './../../shared/form/list/base-list.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Component, OnInit, Injector, ViewEncapsulation, ViewChild } from '@angular/core';
import { GenericServiceProxy } from '@shared/service-proxies/generic-service-proxies';

@Component({
  templateUrl: './workingaccount.component.html',
  styleUrls: ['./workingaccount.component.less'],
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()]
})
export class WorkingAccountComponent extends AppComponentBase implements OnInit {

  @ViewChild('baseList', { static: false }) baseList: BaseListComponent;

  cLoading = false;
  hideList = true;
  gridUrl = ProxyURL.GetListMaintenance + 'branchId=' + encodeURIComponent('06') + '&' + 'prefix=' + encodeURIComponent('WORKINGACCOUNT') + '&';
  officerAcc = '';
  permissionDelete = 'Pages.Maintenance.Delete';

  constructor(
    injector: Injector,
    private _proxy: GenericServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {
  }

  delete(event) {
    let delUrl = ProxyURL.DeleteSysPrefB;

    if (delUrl !== undefined) {
      this.message.confirm(
        this.l('DeleteWorkingAccountConfirmation', event.SYSValue),
        this.l('AreYouSure'),
        isConfirmed => {
          if (isConfirmed) {
            this._proxy.request(delUrl, RequestType.Post, event)
              .subscribe((result) => {
                if (result = 'true') {
                  this.notify.success(this.l('DeleteWorkingAccountSuccess'));
                  this.baseList.refresh();
                }
              });
          }
        }
      );
    }
  }
}
