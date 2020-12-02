import { Component, Injector, ViewChild, ViewEncapsulation, AfterViewInit, OnInit, NgZone } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AppConsts } from '@shared/AppConsts';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { FileUpload } from 'primeng/fileupload';
import { finalize } from 'rxjs/operators';
import { GenericServiceProxy, RequestType } from '@shared/service-proxies/generic-service-proxies';
import { ProxyURL } from '@shared/service-proxies/generic-service-proxies-url';
import { BaseListComponent } from '@app/shared/form/list/base-list.component';
import { RealTimeService } from '@app/shared/form/real-time/real-time.service';
import { SignalRHelper } from '@shared/helpers/SignalRHelper';
import * as moment from 'moment';;
import { getDate } from 'ngx-bootstrap/chronos/utils/date-getters';
import { CalendarComponent } from '@app/shared/form/calendar/calendar.component';
import { NgSelectConfig } from '@ng-select/ng-select';
import { RealTimeMethod } from '@app/shared/form/real-time/real-time-method';
import { CountdownComponent } from 'ngx-countdown';
import { HttpClient } from '@angular/common/http';

@Component({
    templateUrl: './form-page.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class FormPageComponent extends AppComponentBase implements OnInit, AfterViewInit {
    @ViewChild('ExcelFileUpload', { static: true }) excelFileUpload: FileUpload;
    @ViewChild('baseList', { static: false }) baseList: BaseListComponent;
    @ViewChild('baseList1', { static: false }) baseList1: BaseListComponent;
    @ViewChild('baseListExpanded', { static: false }) baseListExpanded: BaseListComponent;
    @ViewChild('calendar', { static: false }) calendar: CalendarComponent;
    @ViewChild('cd', { static: false }) private countdown: CountdownComponent;
    @ViewChild('otp', { static: false }) otp: any;

    uploadUrl: string;

    //Filters
    advancedFiltersAreShown = false;
    filterText = '';
    flag = true;

    gridUrl = ProxyURL.GetCity;
    gridUrl1 = ProxyURL.GetCountry;
    gridUrl2 = ProxyURL.GetStateList;

    galleryUrl = ProxyURL.GetCompanyDocument;
    permissionCreate = '';
    permissionView = 'Pages.Administration.Users.Edit';
    permissionEdit = 'Pages.Administration.Users.Edit';
    permissionDelete = 'Pages.Administration.Users.Delete';
    selectedData: any = [];
    selectedSingleSelect: any;
    selectedMultiSelect: any = [];
    comboModel: any = [];
    timeLeft: any = 30;

    otpInput: any;

    //#region Calendar
    calendarUrl = ProxyURL.GetVehicleSchedule;
    date = moment(new Date()).format('YYYY-MM-DD');
    eventTitleSize = 2;
    //#endregion

    constructor(
        injector: Injector,
        private _proxy: GenericServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _realTimeService: RealTimeService,
        public _zone: NgZone,
        private _config: NgSelectConfig,
        private _httpClient: HttpClient,
    ) {
        super(injector);
        this.filterText = this._activatedRoute.snapshot.queryParams['filterText'] || '';
        this.uploadUrl = AppConsts.remoteServiceBaseUrl + '/Users/ImportFromExcel';
        this._config.notFoundText = 'No Result';
    }

    ngOnInit(): void {
        this.registerEvents();
        let methods = [RealTimeMethod.app_dashboard_refresh];
        if (this.appSession.application) {
            SignalRHelper.initSignalR(() => { this._realTimeService.init(methods); });
        }
    }

    ngAfterViewInit(): void {
        this._proxy.request(ProxyURL.GetCityCombo, RequestType.Get)
            .pipe().subscribe(result => {
                this.comboModel = result;
            });
        this.initCalendar();
    }

    refresh(mode: number): void {
        if (mode === 0) {
            this.baseList.refresh();
        } else if (mode === 1) {
            this.baseList1.refresh();
        } else if (mode === 2) {
            this.baseListExpanded.refresh();
        }
    }

    delete(data: any, mode: number): void {
        this.message.confirm(
            this.l('DataDeleteWarningMessage', data.CityName),
            this.l('AreYouSure'),
            isConfirmed => {
                if (isConfirmed) {
                    this._proxy.request(ProxyURL.DeleteCity, RequestType.Post, data)
                        .subscribe(result => {
                            if (result.success) {
                                this.refresh(mode);
                                this.notify.success(this.l('SuccessfullyDeleted'));
                            } else {
                                this.notify.error(this.l('FailedToDeleted'));
                            }
                        });
                }
            }
        );
    }

    onLoadFinished() {
    }

    onSelectedChange(data: any[]): void {
        this.selectedData = data;
    }

    onSelectedGallery(data: any[]): void {
        console.log(data);
    }

    rowExpanded(data: any) {
        // //console.log(data);
    }
    rowColapsed(data: any) {
        //console.log(data);
    }

    //#region Calendar
    initCalendar() {
        this.calendarUrl = ProxyURL.GetVehicleSchedule;
        this.calendarUrl += 'Id=10000101&';
        this.calendarUrl += 'ViewDate=' + this.date;
        this.calendar.refresh(this.calendarUrl);
    }

    onCalendarDateClick(data: string): void {
        console.log(data);
    }

    onCalendarViewChange(data: Date) {
        if (this.calendar !== undefined) {
            this.date = moment(data).format('YYYY-MM-DD');
            this.initCalendar();
        }
    }

    onCalendarEventRender(event: any) {
        if (event.event.rendering === 'background') {
            event.el.innerHTML = '<div style="witdh:100%;height:100%;"><span class="fc-title" style="color:' + event.event.textColor + ';font-size:' + this.eventTitleSize + 'vw;font-weight:500;text-align:center;margin-bottom: -15px;">' + event.event.title + '</span><div class="kt-widget15__item" style="margin-top: -15px; "><span class="kt-widget15__stats" style="color: #fff; font-weight: 500;">45%</span><div class="kt-space-10"><div class="progress kt-progress--sm"><div class="progress-bar bg-calendar-warning" role="progressbar" style="width: 55%;" aria-valuenow="50" aria-valuemin="0" aria-valuemax="100"></div></div></div></div></div>';
            event.el.style.opacity = '.7';
        }
    }

    onRowRender(event: any) {
        event.StateDesc === 'JOHOR' ? event.RowColor = 'grid-row-danger' : event.StateDesc === 'MELAKA' ? event.RowColor = 'grid-row-warning' : event.RowColor = null;
    }
    //#endregion

    registerEvents(): void {
        const self = this;
        abp.event.on(RealTimeMethod.app_dashboard_refresh, (arg1: any) => {
            self._zone.run(() => {
                this.message.success(arg1.toString());
            });
        });
    }

    onSingleSelectChange(data: any): void {
        console.log(data);
        console.log(this.selectedSingleSelect);
    }

    onMultiSelectChange(data: any[]): void {
        console.log(data);
        console.log(this.selectedMultiSelect);
    }

    exportToExcel(): void {
        this.baseList.exportToExcel();
    }

    uploadExcel(data: { files: File }): void {
        const formData: FormData = new FormData();
        const file = data.files[0];
        formData.append('file', file, file.name);

        this._httpClient
            .post<any>(this.uploadUrl, formData)
            .pipe(finalize(() => this.excelFileUpload.clear()))
            .subscribe(response => {
                if (response.success) {
                    this.notify.success(this.l('ImportUsersProcessStart'));
                } else if (response.error != null) {
                    this.notify.error(this.l('ImportUsersUploadFailed'));
                }
            });
    }

    onUploadExcelError(): void {
        this.notify.error(this.l('ImportUsersUploadFailed'));
    }

    onOtpChange(input: any) {
        this.otpInput = input;
    }
}
