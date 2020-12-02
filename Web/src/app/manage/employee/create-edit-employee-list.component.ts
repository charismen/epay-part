import { Component, EventEmitter, Injector, Output, ViewChild, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { GenericServiceProxy, RequestType } from '@shared/service-proxies/generic-service-proxies';
import { ActivatedRoute, Router } from '@angular/router';
import { SaveType, BizRegBizLoc } from '@shared/AppEnums';
import * as _ from 'lodash';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { ProxyURL } from '@shared/service-proxies/generic-service-proxies-url';

@Component({
  selector: 'createOrEditEmployeeModal',
  templateUrl: './create-edit-employee-list.component.html'
})
export class CreateOrEditEmployeeModalComponent extends AppComponentBase implements OnInit {

  @ViewChild('createOrEditEmployeeModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  loading = false;
  active = false;
  create = false;
  saving = false;
  view = false;

  saveState: number;
  saveType: number;

  salutationCombo = [];
  designationCombo = [];
  departmentCombo = [];
  genderCombo = [];
  stateCombo = [];
  usrGroupCombo = [];
  userCombo = [];
  selectedSalutation: any;
  selectedDesignation: any;
  selectedDepartment: any;
  selectedGender: any;
  selectedState: any;
  selectedUsrGroup: any;
  selectedUser: any;

  dataEditUrl = ProxyURL.UpdateCustomer;
  modalTitle = '';
  inputHelper: any = {};
  comboLoadedCount = 0;

  constructor(
    injector: Injector,
    private _proxy: GenericServiceProxy,
    private _bizRegBizLoc: BizRegBizLoc
  ) {
    super(injector);
  }

  ngOnInit(): void {
  }

  show(data?: any, viewEdit?: string): void {
    this.comboLoadedCount = 0;
    let userUrl = ProxyURL.GetUsersCombo;
    this._proxy.request(userUrl, RequestType.Get)
      .subscribe(result => {
        this.userCombo = result;
        this.selectedUser = data != undefined ? data.ReferralID : this.userCombo[0]['Code'];
        this.comboLoadedCount++;
      });

    let codeUrl = ProxyURL.GetCodeMaster + 'code=SAL&';
    this._proxy.request(codeUrl, RequestType.Get)
      .subscribe(result => {
        this.salutationCombo = result;
        this.salutationCombo.unshift({ "CodeType": "SAL", "Code": "", "CodeDesc": "N/A", "CodeRemark": "N/A", "CodeSeq": 0, "CodeRef": "", "CodeVal1": "", "CodeVal2": "", "CodeVal3": "" });
        this.selectedSalutation = data != undefined ? data.Salutation : this.salutationCombo[0]['Code'];
        this.comboLoadedCount++;
      });

    codeUrl = ProxyURL.GetCodeMaster + 'code=DSN&';
    this._proxy.request(codeUrl, RequestType.Get)
      .subscribe(result => {
        this.designationCombo = result;
        this.selectedDesignation = data != undefined ? data.Designation : this.designationCombo[0]['Code'];
        this.comboLoadedCount++;
      });

    codeUrl = ProxyURL.GetCodeMaster + 'code=DEP&';
    this._proxy.request(codeUrl, RequestType.Get)
      .subscribe(result => {
        this.departmentCombo = result;
        this.departmentCombo.unshift({ "CodeType": "DEP", "Code": "", "CodeDesc": "N/A", "CodeRemark": "N/A", "CodeSeq": 0, "CodeRef": "", "CodeVal1": "", "CodeVal2": "", "CodeVal3": "" });
        this.selectedDepartment = data != undefined ? data.Department : this.departmentCombo[0]['Code'];
        this.comboLoadedCount++;
      });

    codeUrl = ProxyURL.GetCodeMaster + 'code=GEN&';
    this._proxy.request(codeUrl, RequestType.Get)
      .subscribe(result => {
        this.genderCombo = result;
        this.genderCombo.unshift({ "CodeType": "GEN", "Code": "", "CodeDesc": "N/A", "CodeRemark": "", "CodeSeq": 0, "CodeRef": "", "CodeVal1": "", "CodeVal2": "", "CodeVal3": "" });
        this.selectedGender = data != undefined ? data.Sex.trim() : this.genderCombo[0]['CodeRemark'].trim();
        this.comboLoadedCount++;
      });

    let stateUrl = ProxyURL.GetState + 'country=MY&';
    this._proxy.request(stateUrl, RequestType.Get)
      .subscribe(result => {
        this.stateCombo = result;
        this.stateCombo.unshift({ 'Code': 'ALL', 'Remark': 'ALL' });
        this.stateCombo.unshift({ 'Code': '', 'Remark': 'N/A' });
        this.selectedState = data != undefined ? data.PnState : this.stateCombo[0]['Code'];
        this.comboLoadedCount++;
      });

    let groupUrl = ProxyURL.GetUsrGroupCombo + 'appid=1&';
    this._proxy.request(groupUrl, RequestType.Get)
      .subscribe(result => {
        this.usrGroupCombo = result;
        this.selectedUsrGroup = data != undefined ? data.AccessCode : this.usrGroupCombo[0]['Code'];
        this.comboLoadedCount++;
      });
    
    this.inputHelper = {};

    if (data == undefined) {
      this.modalTitle = 'CreateNewEmployee';
      this.saveState = SaveType.Insert;
      this.active = true;
      this.create = true;
      this.view = false;
      // this.setNull();      
    } else {
      if (viewEdit == 'view') {
        this.view = true;
        this.modalTitle = 'ViewEmployee';
      } else {
        this.view = false;
        this.modalTitle = 'EditEmployee';
        this.create = false;        
      }
      this.inputHelper = _.cloneDeep(data);
      //this.inputHelper = data;
      this.inputHelper.NickName = data.UserLogin;
      this.saveState = SaveType.Update;
      this.inputHelper.Active = 1;
      this.inputHelper.Flag = 1;
    }
    this.modal.show();
  }

  setNull(): void{
    this.inputHelper.EmployeeID = null;
    this.inputHelper.Salutation = '';
    //this.inputHelper.FullName = '';
    this.inputHelper.NRICNo = '';
    this.inputHelper.Designation = '';
    this.inputHelper.Department = '';
    this.inputHelper.Sex = '';
    this.inputHelper.State = '';
    this.inputHelper.EmailAddress = '';
    this.inputHelper.EmerContactNo = '';
    this.inputHelper.AccessCode = '';
    this.inputHelper.UserID = '';
    this.inputHelper.Password = '';
  }

  userChange(data): void {
    this.selectedUser = data;
  }

  save(): void {
    let data: any = [];
    this.saving  = true;
    if (this._bizRegBizLoc.BizRegID !== undefined) {
      this.inputHelper.BizRegID = this._bizRegBizLoc.BizRegID;
      this.inputHelper.ReferralID = this.selectedUser;
      console.log('BizRegID: ' + this.inputHelper.BizRegID);
      console.log('UserID: ' + this.inputHelper.ReferralID);
    }

    this.inputHelper.Department = this.selectedDepartment;
    this.inputHelper.Designation = this.selectedDesignation;
    this.inputHelper.Sex = this.selectedGender;
    this.inputHelper.Salutation = this.selectedSalutation;
    this.inputHelper.PnState = this.selectedState;
    this.inputHelper.AccessCode = this.selectedUsrGroup;

    data.push(this.inputHelper);
    //console.log('dataSave: ' + JSON.stringify(data));
    let url = ProxyURL.CreateToAssignEmployee; //ProxyURL.CreateEditEmployee;
    let notif = '';

    if (this.saveState === SaveType.Insert) {
      notif = 'SavedSuccessfully';
    } else {
      notif = 'UpdateSuccessfully';
    }
    
    if (url !== undefined) {
      this._proxy.request(url, RequestType.Post, data)
        .subscribe((result) => {
          this.saving = false;
          if (result.success = 'true') {
            this.notify.success(this.l(notif));
            this.modalSave.emit();
            this.close();            
          } else {
            this.notify.error(this.l(result.message));
          }          
        });
    }
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
