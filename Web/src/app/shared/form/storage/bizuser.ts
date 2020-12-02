import { Observable } from 'rxjs';

export interface IBizEmpLoc {
    bizLocID: string | undefined;
    branchName: string | undefined;
}

export class BizEmpLoc {
    bizLocID: string | undefined;
    branchName: string | undefined;

    constructor(data?: IBizUser) {
        if (data) {
            for (let property in data) {
                if (data.hasOwnProperty(property)) {
                    (<any>this)[property] = (<any>data)[property];
                }
            }
        }
    }

    static fromJS(data: any): BizUser {
        data = typeof data === 'object' ? data : {};
        let result = new BizUser();
        result.init(data);
        return result;
    }

    init(data?: any) {
        if (data) {
            this.bizLocID = data['bizLocID'];
            this.branchName = data['branchName'];
        }
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data['bizLocID'] = this.bizLocID;
        data['branchName'] = this.branchName;
        return data;
    }
}

export interface IBizUser {
    email: string | undefined;
    bizRegID: string | undefined;
    bizLocID: string | undefined;
    employeeID: string | undefined;
    employeeLoc: Observable<BizEmpLoc> | undefined;
}

export class BizUser implements IBizUser {
    email: string | undefined;
    bizRegID: string | undefined;
    companyName: string | undefined;
    bizLocID: string | undefined;
    branchName: string | undefined;
    employeeID: string | undefined;
    employeeLoc: Observable<BizEmpLoc> | undefined;

    constructor(data?: IBizUser) {
        if (data) {
            for (let property in data) {
                if (data.hasOwnProperty(property)) {
                    (<any>this)[property] = (<any>data)[property];
                }
            }
        }
    }

    static fromJS(data: any): BizUser {
        data = typeof data === 'object' ? data : {};
        let result = new BizUser();
        result.init(data);
        return result;
    }

    init(data?: any) {
        if (data) {
            this.email = data['email'];
            this.bizRegID = data['bizRegID'];
            this.companyName = data['companyName'];
            this.bizLocID = data['bizLocID'];
            this.branchName = data['branchName'];
            this.employeeID = data['employeeID'];
            this.employeeLoc = data['employeeLoc'];
        }
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data['email'] = this.email;
        data['bizRegID'] = this.bizRegID;
        data['companyName'] = this.companyName;
        data['bizLocID'] = this.bizLocID;
        data['branchName'] = this.branchName;
        data['employeeID'] = this.employeeID;
        data['employeeLoc'] = this.employeeLoc;
        return data;
    }
}
