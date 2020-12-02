import { UserServiceProxy, CreateOrUpdateUserInput } from '@shared/service-proxies/service-proxies';
import { Component, EventEmitter, Injector, Output, ViewChild, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { GenericServiceProxy, RequestType } from '@shared/service-proxies/generic-service-proxies';
import { ActivatedRoute, Router } from '@angular/router';
import { SaveType, ApprovalStatus } from '@shared/AppEnums';
import * as _ from 'lodash';
import { ModalDirective } from 'ngx-bootstrap';
import { ProxyURL } from '@shared/service-proxies/generic-service-proxies-url';
import { finalize } from 'rxjs/operators';
import { combineLatest as _observableCombineLatest } from 'rxjs';
import { UserEditDto, RoleServiceProxy, UserListDto } from './../../../shared/service-proxies/service-proxies';

@Component({
    selector: 'viewOrEditApprovalModal',
    templateUrl: './view-edit-approval-modal.component.html'
})
export class ViewOrEditApprovalModalComponent extends AppComponentBase implements OnInit {
    @ViewChild('createModal', {static: true}) modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;
    dataObtained = false;

    saveState: number;
    saveType: number;

    viewscript: string;
    item: any = {};

    dataApproveURL = ProxyURL.UpdateCompany_A;

    role: 'any';
    users: UserListDto = new UserListDto();
    userHelper: UserEditDto = new UserEditDto();
    inputHelper: any = {};

    modalTitle = '';

    status: boolean;

    approval = ApprovalStatus;

    constructor(
        injector: Injector,
        private _route: Router,
        private __proxy: GenericServiceProxy,
        private _userService: UserServiceProxy
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.inputHelper.BizRegID = '';
        this.inputHelper.CompanyName = '';
        this.inputHelper.InitName = '';
        this.inputHelper.RegDate = '';
        this.inputHelper.Country = '';
        this.inputHelper.ContactPerson = '';

    }

    onShown(): void {
        document.getElementById('approval').focus();
    }

    getUserData(initName: string): void {
        this._userService.getUsers(initName, [''], 14, false, '', 1, 0)
        .subscribe((result) => {
            // this.userModel = result.items[0];
            this.users = result.items[0];
            this.userHelper.id = this.users.id;
            this.userHelper.name = this.users.name;
            this.userHelper.surname = this.users.surname;
            this.userHelper.userName = this.users.userName;
            this.userHelper.emailAddress = this.users.emailAddress;
            console.log(JSON.stringify(this.users));
            this.dataObtained = true;
        });
    }

    show(data?: any): void {
      this.modal.show();
        if (data[0] !== undefined) {            
            //edit aproval
            this.modalTitle = 'ViewApproval';
            this.active = true;
            this.inputHelper.BizRegID = data[0].BizRegID;
            this.inputHelper.CompanyName = data[0].CompanyName;
            this.inputHelper.InitName = data[0].InitName;
            this.inputHelper.RegDate = data[0].RegDate;
            this.inputHelper.Country = data[0].Country;
            this.inputHelper.ContactPerson = data[0].ContactPerson;

            this.userHelper.name = '';
            this.userHelper.surname = '';
            this.userHelper.userName = '';
            this.userHelper.emailAddress = '';
            this.userHelper.phoneNumber = '';
            this.userHelper.password = '';
            this.userHelper.isActive = true;
            this.getUserData(this.inputHelper.InitName);
            console.log(data[0]);
          }
    }

    save(data: number): void {
        // console.log('dataSave: ' + JSON.stringify(data));
        let url = '';
        let notif = '';
        let dataSave = [];
        let userModel = new CreateOrUpdateUserInput();        

        if (data === ApprovalStatus.Approve) {
            this.inputHelper.active = 1;
            this.inputHelper.flag = 1;
            this.inputHelper.status = 1;
            userModel.user = this.userHelper;
            userModel.setRandomPassword = false;
            userModel.sendActivationEmail = false;
            //userModel.assignedRoleNames = this.role;
            //this.userModel.user.isActive = true;
            url = this.dataApproveURL;
            notif = 'Approved!';
        } else {
            this.inputHelper.Active = 0;
            this.inputHelper.Flag = 1;
            this.inputHelper.status = 0;
            //this.userModel.user.isActive = false;
            url = this.dataApproveURL;
            notif = 'Rejected!';
        }

        dataSave.push(this.inputHelper);

        if (url !== undefined) {
            _observableCombineLatest([
                this.__proxy.request(url, RequestType.Post, dataSave),
                this._userService.createOrUpdateUser(userModel)
            ]).subscribe((result) => {
                if (result != null) {
                    this.notify.success(this.l(notif));
                    this.modalSave.emit();
                    this.close();
                } else {
                    this.notify.error(this.l('Error'));
                }
                this.dataObtained = false;
            });
        }
        console.log('dataSave: ' + JSON.stringify(userModel));
        this.dataObtained = false;
    }

    close(): void {
        this.active = false;
        this.dataObtained = false;
        this.modal.hide();        
    }

    back(): void {
        this.dataObtained = false;
        this._route.navigate(['/app/main/approval']);      
    }

}
