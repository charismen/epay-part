import { Component, Injector, Input, OnInit, AfterViewInit, Output, EventEmitter, ElementRef, ViewChild } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ProxyURL } from '@shared/service-proxies/generic-service-proxies-url';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { ViewEncapsulation } from '@angular/core';

@Component({
    selector: 'field-upload',
    templateUrl: './field-upload.component.html',
    // styleUrls: ['./field-upload.component.less'],
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})

export class FieldUploadComponent extends AppComponentBase implements OnInit, AfterViewInit {
    uploadUrl: string;
    uploadedFiles: any[] = [];
    selectedFiles: any[] = [];
    @Input() APIUrl = '';
    @Input() accept = 'image/*, .csv, .xls, .xlsx, .doc, .docx, .pdf, .zip';
    @Input() auto = false;
    @Input() customUpload = false;
    @Input() multiple = true;
    @Input() maxFileSize = 1000000;
    @Input() showUploaded = true;
    @Input() completed = false;
    @Input() required = false;
    @Input() placeholder = this.l('ChoseFileOrDrag');
    @Input() title = '';
    @Input() subtitle = '';
    @Output() onUpload: EventEmitter<any> = new EventEmitter<any>();
    @Output() onSelect: EventEmitter<any> = new EventEmitter<any>();
    @Output() onUploadHandler: EventEmitter<any> = new EventEmitter<any>();
    @Output() onRemove: EventEmitter<any> = new EventEmitter<any>();

    constructor(
        injector: Injector,
        private sanitizer: DomSanitizer
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.uploadUrl = AppConsts.remoteServiceBaseUrl + this.APIUrl;
    }

    ngAfterViewInit(): void {
    }

    // upload completed event
    onUploadComplete(event): void {
        let responseBody = event.originalEvent.body.result;
        let status = event.originalEvent.status;
        let files = event.files;

        if (status === 200) {
            for (const file of responseBody) {
                if (file.type.indexOf('image/') > -1) {
                    file.objectURL = files.filter(item => item.name === file.name).map(select => select.objectURL)[0];
                }
                this.uploadedFiles.push(file);
                this.selectedFiles = [];
            }
        }

        this.onUpload.emit(this.uploadedFiles);
    }

    onBeforeSend(event): void {
        event.xhr.setRequestHeader('Authorization', 'Bearer ' + abp.auth.getToken());
    }

    onSelected(): void {
        // this.onSelect.emit(this.FieldUpload.files);
    }

    remove(event, file) {
        // this.FieldUpload.remove(event, file);
        // this.onSelect.emit(this.FieldUpload.files);
    }

    uploadHandler(event){
        this.onUploadHandler.emit(event);
    }

    removeAll(event){
        this.onRemove.emit(event);
    }
}
