import { finalize } from 'rxjs/operators';
import { ProxyURL } from '@shared/service-proxies/generic-service-proxies-url';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Component, OnInit, ViewEncapsulation, Injector, AfterViewInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AppConsts } from '@shared/AppConsts';
import { GenericServiceProxy, RequestType } from '@shared/service-proxies/generic-service-proxies';

declare var Stimulsoft: any;

import * as moment from 'moment';
import { TransactionStatus } from '@shared/AppEnums';

@Component({
  selector: 'report',
  templateUrl: './report.component.html',
  //encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()]
})
export class ReportComponent extends AppComponentBase implements OnInit, AfterViewInit {

  // report: any;
  viewer: any = new Stimulsoft.Viewer.StiViewer(null, 'StiViewer', false);
  url = ProxyURL.GetTransactionReport;
  // reportData: any;
  toggle: any;

  selectedTransStatus: any;
  status: number;
  statusCombo: any[];

  dateFrom: Date; // moment(new Date()).format('YYYY-MM-DD');
  dateTo: Date; // moment(new Date()).format('YYYY-MM-DD');
  errorDate = '';

  filterData: any = {};

  constructor(
    injector: Injector,
    private _http: HttpClient,
    private _proxy: GenericServiceProxy
  ) {
    super(injector);
  }

  ngOnInit() {
    Stimulsoft.Base.StiLicense.key = AppConsts.Stimulsoft.license;
    // this.report = new Stimulsoft.Report.StiReport();
    // this.report.loadFile(this.appUrlService.appRootUrl + '/assets/stimulsoft/TransactionReport.mrt');
    // this.viewer.renderHtml('viewer');

    this.getReport();

    this.statusCombo = ['Paid', 'Cancel', 'Pending'];
  }

  ngAfterViewInit(): void {
    this.hideSidebar();
  }

  hideSidebar(): void {
    this.toggle = new KTToggle('kt_aside_toggler', {
        target: 'body',
        targetState: 'kt-aside--minimize',
        togglerState: 'kt-aside__brand-aside-toggler--active'
    });
    this.toggle.toggleOn();
  }

  getReport() {
    let report = new Stimulsoft.Report.StiReport();
    // let options = new Stimulsoft.Viewer.StiViewerOptions();
    // options.toolbar.showParametersButton = true;

    report.loadFile(this.appUrlService.appRootUrl + '/assets/stimulsoft/TransactionReport.mrt');
    this.viewer.renderHtml('viewer');

    let start = moment();
    let startPost = moment();

    let rptURL = this.url;
    rptURL += 'Start=' + moment(start.subtract(30, 'days')).format('YYYY-MM-DD') + '&';
    rptURL += 'End=' + moment(new Date()).format('YYYY-MM-DD') + '&';
    rptURL += 'Status=0&';
    rptURL += 'Posted=2&';
    rptURL += 'StartPost=' + moment(startPost.subtract(30, 'days')).format('YYYY-MM-DD') + '&';
    rptURL += 'EndPost=' + moment(new Date()).format('YYYY-MM-DD') + '&';
    rptURL += 'Type=2&';

    if (this.url !== undefined) {
      this._proxy.request(rptURL, RequestType.Get)
        .subscribe((result) => {
          //this.reportData = result.items;
          report.regData('TransactionReport', null, result.items);
          report.dictionary.synchronize();
          this.viewer.report = report;
        });
    }
  }

  onTransStatusChange(data: any) {
    data === 'Paid' ? this.status = TransactionStatus.Paid : data === 'Pending' ? this.status = TransactionStatus.Pending : this.status = TransactionStatus.Cancel;
  }

  dateRangeCheck(from: Date, to: Date, msg: string) {
    (to < from) ? (this.errorDate = msg) : (this.errorDate = '');
  }

  checkDateFrom(data?: Date) {
    this.dateRangeCheck(data, this.dateTo, '"Date To" Must Be After "Date From"');
  }

  checkDateTo(data?: Date) {
    this.dateRangeCheck(this.dateFrom, data, '"Date To" Must Be After "Date From"');
  }

  searchFilter() {
    let report = new Stimulsoft.Report.StiReport();

    report.loadFile(this.appUrlService.appRootUrl + '/assets/stimulsoft/TransactionReport.mrt');
    this.viewer.renderHtml('viewer');

    let filterURL = this.url;
    filterURL += 'Start=' + moment(this.dateFrom).format('YYYY-MM-DD') + '&';
    filterURL += 'End=' + moment(this.dateTo).format('YYYY-MM-DD') + '&';
    filterURL += 'Status=' + this.status + '&';
    filterURL += 'Posted=2&';
    filterURL += 'StartPost=' + moment(this.dateFrom).format('YYYY-MM-DD') + '&';
    filterURL += 'EndPost=' + moment(this.dateTo).format('YYYY-MM-DD') + '&';
    filterURL += 'Type=2&';

    if (this.url !== undefined) {
      this._proxy.request(filterURL, RequestType.Get)
        .subscribe((result) => {
          //this.reportData = result.items;
          report.regData('TransactionReport', null, result.items);
          report.dictionary.synchronize();
          this.viewer.report = report;
        });
    }
  }
}
