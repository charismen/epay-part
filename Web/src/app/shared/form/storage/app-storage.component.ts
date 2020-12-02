import { LocalStorageService } from '@shared/utils/local-storage.service';
import { BizUser, BizEmpLoc } from './bizuser';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import * as localForage from 'localforage';

@Injectable()
export class AppStorageKey {
    static readonly bizUser = 'biz_user';
}

@Injectable()
export class AppStorage {
    private static _biz: BizUser = new BizUser();

    get email(): string {
        return AppStorage._biz.email;
    }
    get bizRegID(): string {
        return AppStorage._biz.bizRegID;
    }
    get bizLocID(): string {
        return AppStorage._biz.bizLocID;
    }
    get companyName(): string {
        return AppStorage._biz.companyName;
    }
    get branchName(): string {
        return AppStorage._biz.branchName;
    }
    get employeeID(): string {
        return AppStorage._biz.employeeID;
    }
    get employeeLoc(): Observable<BizEmpLoc> {
        return AppStorage._biz.employeeLoc;
    }

    constructor() {
        this.init();
    }

    private async init(): Promise<void> {
        if (AppStorage._biz) {
            let value = await localForage.getItem<any>(AppStorageKey.bizUser);
            AppStorage._biz = value ? BizUser.fromJS(JSON.parse(value.biz)) : new BizUser();
        }
    }
}
