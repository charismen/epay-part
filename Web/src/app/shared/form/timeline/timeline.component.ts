import { Component, Injector, ViewChild, ViewEncapsulation, AfterViewInit, OnInit, AfterContentChecked, ChangeDetectorRef, Input, Output, EventEmitter, ContentChildren, TemplateRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AppConsts } from '@shared/AppConsts';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { LazyLoadEvent } from 'primeng/public_api';
import { Paginator } from 'primeng/paginator';
import { finalize } from 'rxjs/operators';
import { GenericServiceProxy, RequestType } from '@shared/service-proxies/generic-service-proxies';
import { ViewContainerComponent } from '../container/view-container.component';
import { convertAbpLocaleToAngularLocale } from 'root.module';
import { registerLocaleData } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
    templateUrl: './timeline.component.html',
    selector: 'timeline',
    styleUrls: ['./timeline.component.less'],
    encapsulation: ViewEncapsulation.None
})

export class TimelineComponent extends AppComponentBase implements AfterViewInit, AfterContentChecked {
    @ContentChildren(ViewContainerComponent)
    views: ViewContainerComponent[];

    @Input()
    get filterText() {
        return this.filter;
    }
    set filterText(val) {
        this.filter = val;
    }

    @Input() gridUrl = '';
    @Output() onLoadFinished: EventEmitter<any> = new EventEmitter<any>();

    data: any;
    flag = true;
    filter = '';

    constructor(
        injector: Injector,
        private __cdref: ChangeDetectorRef,
        private __proxy: GenericServiceProxy
    ) {
        super(injector);
    }

    ngAfterViewInit() {
    }

    ngAfterContentChecked(): void {
        this.__cdref.detectChanges();
    }

    refresh(event?: LazyLoadEvent) {
        // if (this.primengTableHelper.shouldResetPaging(event)) {
        //     this.paginator.changePage(0);
        //     return;
        // }

        let url = this.gridUrl;
        if (url !== undefined) {
            url += 'Flag=' + (this.flag ? 1 : 0) + '&';
            if (this.filterText !== undefined) { url += 'Filter=' + encodeURIComponent('' + this.filterText) + '&'; }
            // if (this.primengTableHelper.getSorting(this.dataTable) !== undefined) { url += 'Sorting=' + encodeURIComponent('' + this.primengTableHelper.getSorting(this.dataTable)) + '&'; }
            // if (this.primengTableHelper.getSkipCount(this.paginator, event) !== undefined) { url += 'SkipCount=' + encodeURIComponent('' + this.primengTableHelper.getSkipCount(this.paginator, event)) + '&'; }
            // if (this.primengTableHelper.getMaxResultCount(this.paginator, event) !== undefined) { url += 'MaxResultCount=' + encodeURIComponent('' + this.primengTableHelper.getMaxResultCount(this.paginator, event)) + '&'; }

            this.primengTableHelper.showLoadingIndicator();
            this.__proxy.request(url, RequestType.Get)
                .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
                .subscribe(result => {
                    this.primengTableHelper.columns = result.columns;
                    this.primengTableHelper.totalRecordsCount = result.totalCount;
                    this.primengTableHelper.records = result.items;
                    this.onLoadFinished.emit();
                });
        }
    }

    setURL(URL: string) {
        this.gridUrl = URL;
    }
}
