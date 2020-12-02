import { Component, Injector, OnInit, ElementRef, ViewChild, AfterViewInit, Input, Output, EventEmitter } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as _ from 'lodash';
import { ActivatedRoute, Router } from '@angular/router';
// import { LocationModalComponent } from './location-modal.component';
import { finalize } from 'rxjs/operators';
import { GenericServiceProxy, RequestType } from '@shared/service-proxies/generic-service-proxies';
import { ViewEncapsulation } from '@angular/core';
declare var $: any;

@Component({
    templateUrl: './gallery.component.html',
    selector: 'gallery',
    styleUrls: ['./gallery.component.less'],
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class GalleryComponent extends AppComponentBase implements OnInit, AfterViewInit {
    @Input()
    get model() {
        return this.galleryList;
    }
    set model(val) {
        this.galleryList = val;
    }

    @Input() galleryUrl = '';
    @Input() selectMode: selectMode = selectMode.none;
    @Output() onSelectClick: EventEmitter<any> = new EventEmitter<any>();

    //@ViewChild('locationModal', { static: true }) locationModal: LocationModalComponent;
    loading = false;
    galleryFilter: any[] = [];
    galleryList: any[] = [];
    

    constructor(
        injector: Injector,
        private _activatedRoute: ActivatedRoute,
        private _route: Router,
        private __proxy: GenericServiceProxy,
    ) {
        super(injector);
    }

    cubeInit(): void {
        let _type = this.galleryList.map(item => item.CodeRef).filter((value, index, self) => self.indexOf(value) === index);
        let _dimension = this.galleryList.map(item => item.CodeRemark).filter((value, index, self) => self.indexOf(value) === index);

        _.forEach(_type, item => {
            let any = { filter: '.' + item.replace(/\s/g, '').toLowerCase(), title: item.toUpperCase() };
            this.galleryFilter.push(any);
        });
        _.forEach(_dimension, item => {
            let any = { filter: '.' + item.replace(/\s/g, '').toLowerCase(), title: item.toUpperCase() };
            this.galleryFilter.push(any);
        });

        let me = jQuery(this);
        
        jQuery(document).ready(function () {
            this.gridContainer = <any>$('#grid-container');
            //gridContainer.cubeportfolio('destroy');
            // init cubeportfolio
            this.gridContainer.cubeportfolio({
                filters: '#filters-container',
                loadMore: '#grid-loadMore',
                loadMoreAction: 'click',
                layoutMode: 'mosaic',
                sortToPreventGaps: true,
                defaultFilter: '*',
                animationType: 'quicksand',
                gapHorizontal: 0,
                gapVertical: 0,
                gridAdjustment: 'responsive',
                mediaQueries: [{
                    width: 480,
                    cols: 4
                }, {
                    width: 360,
                    cols: 3
                }, {
                    width: 240,
                    cols: 2
                }, {
                    width: 120,
                    cols: 1
                }],
                caption: 'zoom',
                displayType: 'sequentially',
                displayTypeSpeed: 80,
                rewindNav: true,
                scrollByPage: false,

                // lightbox
                lightboxDelegate: '.cbp-lightbox',
                lightboxGallery: true,
                lightboxTitleSrc: 'data-title',
                lightboxCounter: '<div class="cbp-popup-lightbox-counter">{{current}} of {{total}}</div>',

                // singlePage popup
                singlePageDelegate: '.cbp-singlePage',
                singlePageDeeplinking: true,
                singlePageStickyNavigation: true,
                singlePageCounter: '<div class="cbp-popup-singlePage-counter">{{current}} of {{total}}</div>',
                singlePageCallback: function (url, element) {
                    // to update singlePage content use the following method: this.updateSinglePage(yourContent)
                    let t = this;
                    // Update with the service
                    $.ajax({
                        url: url,
                        type: 'GET',
                        dataType: 'html',
                        timeout: 10000
                    })
                        .done(function (result) {
                            t.updateSinglePage(result);
                        })
                        .fail(function () {
                            t.updateSinglePage('Please refresh the page!');
                        });
                },

                // singlePageInline
                singlePageInlineDelegate: '.cbp-singlePageInline',
                singlePageInlinePosition: 'below',
                singlePageInlineInFocus: true,
                singlePageInlineCallback: function (url, element) {
                    // to update singlePageInline content use the following method: this.updateSinglePageInline(yourContent)
                    let t = this;
                    // Update with the service
                    $.ajax({
                        url: url,
                        type: 'GET',
                        dataType: 'html',
                        timeout: 10000
                    })
                        .done(function (result) {
                            t.updateSinglePageInline(result);
                        })
                        .fail(function () {
                            t.updateSinglePageInline('Please refresh the page!');
                        });
                }
            });
        });
    }

    select(data: any): void {
        if (this.selectMode === selectMode.single) {
            this.onSelectClick.emit(data);
        } else if (this.selectMode === selectMode.multiple) {
            if (data.Flag !== 1) {
                data.Flag = 1;
            } else {
                data.Flag = 0;
            }
            let selected = _.filter(this.galleryList, gl => gl.Flag === 1);
            this.onSelectClick.emit(selected);
        }
    }

    ngOnInit(): void {
    }

    ngAfterViewInit() {
        // setTimeout(() => {
        //     this.refresh();
        // }, 100);
    }

    refresh(url?: string): void {
        if (this.galleryUrl) {
            url = this.galleryUrl;
        }
        this.__proxy.request(url, RequestType.Get)
            .pipe(finalize(() => {
                //this.gridContainer.cubeportfolio('destroy');
                this.cubeInit();
            }))
            .subscribe((result) => {
                this.galleryList = result.items;
            });
    }
}

export enum selectMode {
    none = 'none',
    single = 'single',
    multiple = 'multiple',
}
