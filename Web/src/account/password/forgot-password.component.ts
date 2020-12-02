import { Component, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AppUrlService } from '@shared/common/nav/app-url.service';
import { AccountServiceProxy, SendPasswordResetCodeInput } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { GenericServiceProxy, RequestType } from '@shared/service-proxies/generic-service-proxies';
import { ProxyURL } from '@shared/service-proxies/generic-service-proxies-url';

@Component({
    templateUrl: './forgot-password.component.html',
    animations: [accountModuleAnimation()]
})
export class ForgotPasswordComponent extends AppComponentBase {

    model: SendPasswordResetCodeInput = new SendPasswordResetCodeInput();

    saving = false;

    inputHelper: any = {};

    constructor (
        injector: Injector,
        private _accountService: AccountServiceProxy,
        private _appUrlService: AppUrlService,
        private _router: Router,
        private _proxy: GenericServiceProxy
        ) {
        super(injector);
    }

    // save(): void {
    //     this.saving = true;
    //     this._accountService.sendPasswordResetCode(this.model)
    //         .pipe(finalize(() => { this.saving = false; }))
    //         .subscribe(() => {
    //             this.message.success(this.l('PasswordResetMailSentMessage'), this.l('MailSent')).then(() => {
    //                 this._router.navigate(['account/login']);
    //             });
    //         });
    // }

    save(): void {
        this.saving = true;
        this.inputHelper.mode = 1;
        let url = ProxyURL.ForgotPassword;

        if (url !== undefined) {
            this._proxy.request(url, RequestType.Post, this.inputHelper)
            .pipe(finalize(() => {this.saving = false;}))
            .subscribe(() => {
                this.message.success(this.l('PasswordResetMailSentMessage'), this.l('MailSent')).then(() => {
                    this._router.navigate(['account/login']);
                });
            });
        }
    }


}
