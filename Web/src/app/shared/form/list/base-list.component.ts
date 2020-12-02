import { Component, Injector, ViewChild, ViewEncapsulation, AfterViewInit, OnInit, AfterContentChecked, ChangeDetectorRef, Input, Output, EventEmitter, ContentChildren, TemplateRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AppConsts } from '@shared/AppConsts';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { LazyLoadEvent } from 'primeng/public_api';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { FileUpload } from 'primeng/fileupload';
import { finalize } from 'rxjs/operators';
import { GenericServiceProxy, RequestType } from '@shared/service-proxies/generic-service-proxies';
import { ViewContainerComponent } from '../container/view-container.component';
import { convertAbpLocaleToAngularLocale } from 'root.module';
import { registerLocaleData } from '@angular/common';

@Component({
    templateUrl: './base-list.component.html',
    styleUrls: ['./base-list.component.less'],
    selector: 'base-list',
    encapsulation: ViewEncapsulation.None
})
export class BaseListComponent extends AppComponentBase implements AfterViewInit, AfterContentChecked {
    @Input()
    get model() {
        return this.primengTableHelper;
    }
    set model(val) {
        this.primengTableHelper = val;
    }

    @Input()
    get filterText() {
        return this.filter;
    }
    set filterText(val) {
        this.filter = val;
    }

    @Input() gridUrl = '';
    @Input() rowKey = '';
    @Input() rowExpand = false;
    @Input() actionColumn: actionColumn = actionColumn.none;
    @Input() enableRowClickEdit = false;
    @Input() permissionView = '';
    @Input() permissionCreate = '';
    @Input() permissionEdit = '';
    @Input() permissionDelete = '';
    @Input() permissionHistory = '';
    @Input() historyName = '';
    @Input() enableFilter = true;
    @Input() enablePaginator = true;
    @Input() headerAlignment = '';
    @Input() indexing = false;
    @Input() downloadDocLabel = "Download";
    @Input() downloadDoc = false;
    @Input() downloadDocAlignment: alignment = alignment.right;
    @Input() detailLabel = "Detail";
    @Input() detail = false;
    @Input() detailAlignment: alignment = alignment.left;
    @Input() hideActiveFilter = false;
    @Input() emptyMessage = 'NoData';
    @Input() iconStyle = 'btn btn-sm btn-icon btn-circle';
    @Input() rowExpandMode: expandmode = expandmode.multiple;
    @Input() bodyTemplate: TemplateRef<any>;
    @Input() filterTemplate: TemplateRef<any>;
    @Input() rowExpandTemplate: TemplateRef<any>;
    @Output() onLoadFinished: EventEmitter<any> = new EventEmitter<any>();
    @Output() onViewClick: EventEmitter<any> = new EventEmitter<any>();
    @Output() onEditClick: EventEmitter<any> = new EventEmitter<any>();
    @Output() onDeleteClick: EventEmitter<any> = new EventEmitter<any>();
    @Output() onDownloadDocClick: EventEmitter<any> = new EventEmitter<any>();
    @Output() onSelectedChange: EventEmitter<any> = new EventEmitter<any>();
    @Output() onRowRender: EventEmitter<any> = new EventEmitter<any>();
    @Output() onStyleRender: EventEmitter<any> = new EventEmitter<any>();
    @Output() onCellRender: EventEmitter<any> = new EventEmitter<any>();
    @Output() onDetailClick: EventEmitter<any> = new EventEmitter<any>();
    @Output() onIconClick: EventEmitter<any> = new EventEmitter<any>();
    @Output() onRowExpand: EventEmitter<any> = new EventEmitter<any>();
    @Output() onRowCollapse: EventEmitter<any> = new EventEmitter<any>();

    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;
    @ViewChild('ExcelFileUpload', { static: true }) excelFileUpload: FileUpload;

    selectedData: any = [];
    uploadUrl: string;

    @ContentChildren(ViewContainerComponent)
    views: ViewContainerComponent[];

    //Filters
    advancedFiltersAreShown = false;
    flag = true;
    filter = '';

    constructor(
        injector: Injector,
        private __cdref: ChangeDetectorRef,
        private __proxy: GenericServiceProxy,
        private __fileDownloadService: FileDownloadService,
        private __activatedRoute: ActivatedRoute
    ) {
        super(injector);
        this.filterText = this.__activatedRoute.snapshot.queryParams['filterText'] || '';
        this.uploadUrl = AppConsts.remoteServiceBaseUrl + '/Users/ImportFromExcel';
    }

    ngAfterViewInit(): void {
        this.primengTableHelper.adjustScroll(this.dataTable);
    }

    ngAfterContentChecked(): void {
        this.__cdref.detectChanges();
    }

    refresh(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        let url = this.gridUrl;
        if (url !== undefined) {
            url += 'Flag=' + (this.flag ? 1 : 0) + '&';
            if (this.filterText !== undefined) { url += 'Filter=' + encodeURIComponent('' + this.filterText) + '&'; }
            if (this.primengTableHelper.getSorting(this.dataTable) !== undefined) { url += 'Sorting=' + encodeURIComponent('' + this.primengTableHelper.getSorting(this.dataTable)) + '&'; }
            if (!this.enablePaginator) { url += 'SkipCount=0&'; } else { if (this.primengTableHelper.getSkipCount(this.paginator, event) !== undefined) { url += 'SkipCount=' + encodeURIComponent('' + this.primengTableHelper.getSkipCount(this.paginator, event)) + '&'; }}
            if (!this.enablePaginator) { url += 'MaxResultCount=0&'; } else { if (this.primengTableHelper.getMaxResultCount(this.paginator, event) !== undefined) { url += 'MaxResultCount=' + encodeURIComponent('' + this.primengTableHelper.getMaxResultCount(this.paginator, event)) + '&'; }}

            this.primengTableHelper.showLoadingIndicator();
            this.__proxy.request(url, RequestType.Get)
                .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
                .subscribe(result => {
                    this.primengTableHelper.columns = result.columns;
                    this.primengTableHelper.totalRecordsCount = result.totalCount;
                    this.primengTableHelper.records = result.items;
                    this.primengTableHelper.hideLoadingIndicator();
                    this.onLoadFinished.emit();
                });
        }
    }

    setURL(URL: string) {
        this.gridUrl = URL;
    }

    exportToExcel(event?: LazyLoadEvent): void {
        let url = this.gridUrl;
        if (url !== undefined) {
            url += 'Flag=' + (this.flag ? 1 : 0) + '&';
            if (this.filterText !== undefined) { url += 'Filter=' + encodeURIComponent('' + this.filterText) + '&'; }
            if (this.primengTableHelper.getSorting(this.dataTable) !== undefined) { url += 'Sorting=' + encodeURIComponent('' + this.primengTableHelper.getSorting(this.dataTable)) + '&'; }
            url += 'SkipCount=0&MaxResultCount=0&Function=2';
            this.notify.warn(this.l('PopulatingData'));
            this.__proxy.request(url, RequestType.Get)
                .pipe(
                    finalize(() => this.notify.info(this.l('DownloadingFile')))
                )
                .subscribe(result => {
                    this.__fileDownloadService.downloadTempFile(result);
                });
        }
    }
    view(data: any): void {
        this.onViewClick.emit(data);
    }
    edit(data: any): void {
        this.onEditClick.emit(data);
    }
    delete(data: any): void {
        this.onDeleteClick.emit(data);
    }
    history(data: any): void {
        this.onDeleteClick.emit(data);
    }
    downloadDocument(data: any): void {
        this.onDownloadDocClick.emit(data);
    }
    detailClick(data: any): void {
        this.onDetailClick.emit(data);
    }
    iconClick(data: any, field: string): void {
        let value: any = {};
        value.field = field;
        value.data = data;
        this.onIconClick.emit(value);
    }
    onTableHeaderCheckboxToggle(event: any) {
        if (this.actionColumn === actionColumn.checkbox) {
            if (event.checked === true) {
                this.selectAll();
            } else {
                this.selectedData.length = 0;
                this.onSelectedChange.emit(this.selectedData);
            }
        }
    }
    unselectAll() {
        if (this.actionColumn === actionColumn.checkbox) {
            this.selectedData = undefined;
        }
    }
    selectAll() {
        if (this.actionColumn === actionColumn.checkbox) {
            let url = this.gridUrl;
            if (url !== undefined) {
                url += 'Flag=' + (this.flag ? 1 : 0) + '&';
                if (this.filterText !== undefined) { url += 'Filter=' + encodeURIComponent('' + this.filterText) + '&'; }
                if (this.primengTableHelper.getSorting(this.dataTable) !== undefined) { url += 'Sorting=' + encodeURIComponent('' + this.primengTableHelper.getSorting(this.dataTable)) + '&'; }
                url += 'SkipCount=0&MaxResultCount=0';
                this.spinnerService.show();
                this.__proxy.request(url, RequestType.Get)
                .pipe(finalize(() => {
                    this.spinnerService.hide();
                  })).subscribe(result => {
                        this.selectedData.length = 0;
                        this.selectedData.push(...result.items);
                        this.onSelectedChange.emit(this.selectedData);
                    });
            }
        }
    }
    onRowSelect(event: any) {
        if (this.enableRowClickEdit) {
            this.onEditClick.emit(event.data);
            this.onSelectedChange.emit(this.selectedData);
        } else {
            if (this.actionColumn === actionColumn.none || this.actionColumn === actionColumn.checkbox) {
                this.onSelectedChange.emit(this.selectedData);
            }
        }
    }
    onRowUnselect(event: any) {
        if (this.enableRowClickEdit) {
            this.onEditClick.emit(event.data);
            this.onSelectedChange.emit(this.selectedData);
        } else {
            if (this.actionColumn === actionColumn.none || this.actionColumn === actionColumn.checkbox) {
                this.onSelectedChange.emit(this.selectedData);
            }
        }
    }
    rowExpanded(event: any) {
        this.onRowExpand.emit(event.data);
    }
    rowColapsed(event: any) {
        this.onRowCollapse.emit(event.data);
    }
    getArrayValue(arr, value, index: number) {
        const data = arr.filter(name => name.split('|')[0].toLowerCase() === value.toLowerCase()).toString().split('|');
        if (data.length > index) {
            return data[index];
        }
        return '';
    }
    setIndexing(value: number, event?: LazyLoadEvent) {
        return this.primengTableHelper.getSkipCount(this.paginator, event) !== undefined ?
            this.primengTableHelper.getSkipCount(this.paginator, event) + value : value;
    }
    rowRender(event: any) {
        this.onRowRender.emit(event);
    }
    styleRender(event: any) {
        this.onStyleRender.emit(event);
    }
}

export enum actionColumn {
    none = 'none',
    action = 'action',
    checkbox = 'checkbox'
}

export enum alignment {
    left = 'left',
    right = 'right',
}

export enum expandmode {
    single = 'single',
    multiple = 'multiple',
}