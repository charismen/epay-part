import { GenericServiceProxy, RequestType } from '@shared/service-proxies/generic-service-proxies';
import { ProxyURL } from '@shared/service-proxies/generic-service-proxies-url';
import { UtilsService } from 'abp-ng2-module/dist/src/utils/utils.service';
import { Injectable } from '@angular/core';

export class AppCookiesKey {
    static readonly encrptedBiz = 'enc_biz';
}

export class AppCookies {
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(
        private _proxy: GenericServiceProxy,
        private _utilsService: UtilsService,
    ) {
    }

    setAppCookies(): void {
        this.setBizCookies();
    }

    private setBizCookies(): void {
        if (this.getBizCookies() === undefined) {
            let url = ProxyURL.GetCompanyCookies;
            if (abp.session.userId > 0) { url += 'userid=' + abp.session.userId + '&'; }
            if (abp.session.tenantId > 0) { url += 'tenantid=' + abp.session.tenantId + '&'; }

            this._proxy.request(url, RequestType.Get)
                .pipe().subscribe(result => {
                    if (result !== undefined) {
                        this._utilsService.setCookieValue(
                            AppCookiesKey.encrptedBiz,
                            JSON.stringify(result),
                            undefined,
                            abp.appPath
                        );
                    }
                });
        }
    }

    getBizCookies(): any {
        return this._utilsService.getCookieValue(AppCookiesKey.encrptedBiz).length === 0 ? undefined : JSON.parse(this._utilsService.getCookieValue(AppCookiesKey.encrptedBiz), this.jsonParseReviver);
    }

}
