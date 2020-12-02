import { Component, Injector, Input, OnInit, AfterViewInit, Output, EventEmitter, ElementRef, ViewChild } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ProxyURL } from '@shared/service-proxies/generic-service-proxies-url';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { ViewEncapsulation } from '@angular/core';

@Component({
    selector: 'questionnaire-field',
    templateUrl: './questionnaire-field.component.html',
    // styleUrls: ['./field-upload.component.less'],
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})

export class QuestionnaireFieldComponent extends AppComponentBase implements OnInit, AfterViewInit {
    @Input() APIUrl = '';
    @Input() id = '';
    @Input() mode: questionerMode = questionerMode.survey;
    @Input() completed = false;
    @Input() question = '';
    @Input() options: { code: string, remark: string }[] = [
        { 'code': 'No', 'remark': 'No' },
        { 'code': 'Yes', 'remark': 'Yes' }
    ];
    //@Input() optValue = [];
    @Input() answer = { 'code': '-1', 'remark': '' };
    @Input() answerValue = '-1';
    @Input() remarkValue = '';
    @Output() onSelect: EventEmitter<any> = new EventEmitter<any>();
    @Output() onRemarkChange: EventEmitter<any> = new EventEmitter<any>();
    @Output() onRemarkClear: EventEmitter<any> = new EventEmitter<any>();
    @Output() onRemarkBlur: EventEmitter<any> = new EventEmitter<any>();

    remark = '';
    isRemark = false;
    constructor(
        injector: Injector,
        private sanitizer: DomSanitizer
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.answer = this.options.filter(x => x.code == this.answerValue)[0];
        this.remark = this.remarkValue;
        this.isRemark = ((this.remark != '' && this.remark != null) ? true : false);
    }

    ngAfterViewInit(): void {
    }

    onSelected(data: any): void {
        this.completed = true;
        this.answer = data;
        this.onSelect.emit(data);
    }

    showRemark(): void {
        if (this.isRemark) {
            this.isRemark = false;
            this.remark = '';
            this.onRemarkClear.emit();
        } else {
            this.isRemark = true;
        }
    }

    onRemarkChanged(): void {
        this.onRemarkChange.emit(this.remark);
    }

    onRemarkBlurred(): void {
        this.onRemarkBlur.emit(this.remark);
    }
}

export enum questionerMode {
    survey = 1,
    review = 2,
    comment = 3,
}
