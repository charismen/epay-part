import { Component, Injector, Input, OnInit, AfterViewInit, Output, EventEmitter, ElementRef, ViewChild } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ProxyURL } from '@shared/service-proxies/generic-service-proxies-url';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { ViewEncapsulation } from '@angular/core';
import { FileUpload } from 'primeng/fileupload';

@Component({
    selector: 'file-upload',
    templateUrl: './file-upload.component.html',
    styleUrls: ['./file-upload.component.less'],
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})

export class FileUploadComponent extends AppComponentBase implements OnInit, AfterViewInit {
    @ViewChild('fileUploadInput', { static: true }) fileUpload: FileUpload;
    uploadUrl: string;
    @Input() uploadedFiles: any[] = [];
    @Input() Status: boolean;
    selectedFiles: any[] = [];
    @Input() APIUrl = ProxyURL.FileUpload;
    @Input() accept = 'image/*, .csv, .xls, .xlsx, .doc, .docx, .pdf, .zip';
    @Input() auto = false;
    @Input() customUpload = false;
    @Input() multiple = true;
    @Input() maxFileSize = 1000000;
    @Input() showUploaded = true;
    @Input() showUploadButton = true;
    @Input() showCancelButton = true;
    @Input() showChoosenUpload = true;
    @Input() chooseLabel = 'ChoseFileOrDrag';
    labelText = this.l(this.chooseLabel);
    @Output() onUpload: EventEmitter<any> = new EventEmitter<any>();
    @Output() onSelect: EventEmitter<any> = new EventEmitter<any>();
    @Output() onUploadHandler: EventEmitter<any> = new EventEmitter<any>();
    @Output() downloadError: EventEmitter<any> = new EventEmitter<any>();
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
        this.onSelect.emit(this.fileUpload.files);
    }

    remove(event, file) {
        this.fileUpload.remove(event, file);
        this.onSelect.emit(this.fileUpload.files);
    }

    uploadHandler(event){
        this.onUploadHandler.emit(event);
    }

    DownloadError(): void {
        this.downloadError.emit();
    }

    removeAll(event){
        this.onRemove.emit(event);
    }

    upload(){
        this.fileUpload.upload();
    }
}
