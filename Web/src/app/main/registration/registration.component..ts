import { Component, OnInit, AfterViewInit, Injector, ElementRef, ViewChild, ViewEncapsulation, NgZone } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ProxyURL } from '@shared/service-proxies/generic-service-proxies-url';
import { GenericServiceProxy, RequestType } from '@shared/service-proxies/generic-service-proxies';
import { finalize, tap, switchMap, concatMap } from 'rxjs/operators';
import { Observable, combineLatest as _observableCombineLatest, of as _observableOf, Subject } from 'rxjs';
import { BaseListComponent } from '@app/shared/form/list/base-list.component';
import { UserEditDto, RoleServiceProxy, UserListDto, UserServiceProxy, CreateOrUpdateUserInput, RefreshTokenResult } from './../../../shared/service-proxies/service-proxies';

import * as moment from 'moment';
import * as _ from 'lodash';
import { arrayToHash } from '@fullcalendar/core/util/object';

@Component({
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.less'],
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()]
})

export class RegistrationComponent extends AppComponentBase implements OnInit, AfterViewInit {
    @ViewChild('baseList', { static: false }) baseList: BaseListComponent;

    comboModel: any = [];
    inputHelper: any = {};
    registratedUser: any = {};
    //customer: any = {};
    userHelper: UserEditDto = new UserEditDto();
    urlSave = '/api/Company/AddCompany_A?';    
    role: any;
    filter = '';
    gridUrl = ProxyURL.GetCustomer + 'Flag=0&Sorting=""&SkipCount=0&MaxResultCount=10&Filter=';
    // gridUrl = '/api/Account/GetCustomer?SkipCount=0&MaxResultCount=100&Filter=';
    verify = false;
    tableIsShown = false;
    isChoosed = false;
    validated = false;
    showRegistrated = false;
    regComplete = false;

    constructor(
    injector: Injector,
    private _proxy: GenericServiceProxy,
    private _userService: UserServiceProxy,
    private _roleService: RoleServiceProxy,
    public _zone: NgZone
    ) {
    super(injector);
    }

    ngOnInit() {
        this.verify = false;
        this.gridUrl += encodeURIComponent(this.filter) + '&';
        this.userRole();

        /*this.inputHelper.ROCNumber = '-';
        this.inputHelper.address1 = 'nah';
        this.inputHelper.address2 = '';
        this.inputHelper.address3 = '';
        this.inputHelper.address4 = '';
        this.inputHelper.companyName = 'com';
        this.inputHelper.country = 'IN';
        this.inputHelper.postalCode = '61xxx';
        this.inputHelper.state = 'EJ';
        this.inputHelper.area = 'CR';
        this.inputHelper.city = 'Sby';       
        this.inputHelper.TID = 1;
        this.inputHelper.regNo = 'xxx';
        this.inputHelper.acctNo = 'xxx';
        this.inputHelper.telNo = '03211234';        
        this.inputHelper.createBy = 'admin';
        this.inputHelper.bizRegID = 'xxx';        

        this.userHelper.name = 'x';        
        this.userHelper.userName = 'Audi';
        this.userHelper.emailAddress = 'AudiTerry@yuhuu.com';
        this.userHelper.phoneNumber = '087789098';
        */
       this.inputHelper.ROCNumber = '-';
        this.inputHelper.address2 = '';
        this.inputHelper.address3 = '';
        this.inputHelper.address4 = '';
        this.inputHelper.country = '';
        this.inputHelper.postalCode = '';
        this.inputHelper.state = '';
        this.inputHelper.area = '';
        this.inputHelper.city = '';       
        this.inputHelper.TID = 1;
        this.inputHelper.regNo = '';
        this.inputHelper.telNo = '';        
        this.inputHelper.createBy = '';
        this.inputHelper.bizRegID = '';        

        this.userHelper.name = '';        
        this.userHelper.userName = '';
        this.userHelper.emailAddress = '';
        this.userHelper.phoneNumber = ''; 
        
    }

    ngAfterViewInit() {
        this._proxy.request(ProxyURL.GetCityCombo, RequestType.Get)
            .subscribe(result => {
                this.comboModel = result;
            });
    }

    getCustomer(data?: any){ 
        
            this.gridUrl = ProxyURL.GetCustomer + 'Flag=0&Sorting=""&SkipCount=0&MaxResultCount=10&Filter=' + encodeURIComponent(this.filter) + '&';
            console.log(this.filter);
        setTimeout(() => {
        this.baseList.refresh();
        }, 100);
    }

    setCustomer(data?: any){        
        //console.log(JSON.stringify(data[0].ADDRESS_1));
        //this.customer = data[0];
        this.inputHelper.acctNo = data[0].ADD_CODE;
        this.inputHelper.address1 = data[0].ADDRESS_3;
        this.inputHelper.companyName = data[0].ADDRESS_1;
        this.isChoosed = true;

        
    }

    reEnter(){
        this.verify = false;
        this.tableIsShown = false;
        this.isChoosed = false;
        this.validated = false;
        this.showRegistrated = false;
    }

    checkCustomer(){
        let url = ProxyURL.GetLGMCompany + 'isApprove=0&acctNum=' + this.inputHelper.acctNo + '&';
        this._proxy.request(url, RequestType.Get)
      .pipe(finalize(() => {}))
      .subscribe(result => {
        console.log(result.items.length);
          if(result.items.length === 0){
              this.validated = true;
          }
          else{
              console.log(JSON.stringify(result.items[0]));
              this.validated = false;
              this.showRegistrated = true;
            this.registratedUser.name = result.items[0].ContactPerson;
            this.registratedUser.email = result.items[0].Email;
            
          }
      });
    }

    save(data?: any){
        let send: any = [];
        let userModel = new CreateOrUpdateUserInput();

        this.inputHelper.companyType = 0;
        this.inputHelper.BizRegID = 'xxx';
        this.inputHelper.faxNo = this.inputHelper.telNo;
        this.inputHelper.contactPersonTelNo = this.inputHelper.telNo;
        this.inputHelper.active = 0;
        this.inputHelper.flag = 0;
        this.userHelper.surname = this.userHelper.name;
        this.inputHelper.InitName = this.userHelper.userName;
        this.inputHelper.contactPerson = this.userHelper.name;
        this.inputHelper.email = this.userHelper.emailAddress;
        this.userHelper.isActive = true;

        send.push(this.inputHelper);
        console.log(JSON.stringify(this.role));

        userModel.user = this.userHelper;
        userModel.setRandomPassword = true;
        userModel.sendActivationEmail = false;
        userModel.assignedRoleNames = this.role;

        console.log(JSON.stringify(this.inputHelper));
        _observableCombineLatest([
            this._proxy.request(this.urlSave, RequestType.Post, send),
            this._userService.createOrUpdateUser(userModel)
            ])
            .pipe(
                tap(([a, b]) => {
                    this.updateCompanyUserID(this.inputHelper.companyName);
                    
                    }),
                finalize(() => {
                        
                    }))
            .subscribe((result) => {
                this.regComplete = true;
                this.registrated();
                if (result) {
                    /*this.notify.success(this.l(notif));
                    this.modalSave.emit();*/
                } else {
                    //his.notify.info(this.l(failNotif));
                }
            });
    }

    registrated(){
        let url = ProxyURL.GetLGMCompany + 'isApprove=0&acctNum=' + this.inputHelper.acctNo + '&';
        console.log(url);
        this.registratedUser.company = '';
        this.registratedUser.userName = '';
        this.registratedUser.password = '';
        this._proxy.request(url, RequestType.Get)
      .pipe(finalize(() => {}))
      .subscribe(result => {
          console.log(JSON.stringify(result));
          console.log(JSON.stringify(result.items[0].InitName));
            this.registratedUser.company = result.CompanyName;
            this.registratedUser.userName = result.InitName;
            this.registratedUser.password = 'pa2f41u1h';
      });
    }

    private updateCompanyUserID(companyname: string) {
        let getUrl = '/api/Company/GetCompany?';//ProxyURL.GetCompany;
        let updUrl = '/api/Company/UpdateCompany_A?';//ProxyURL.UpdateCompany_A;
        let entity: any = {};
        let entityUser: any = [];

        if (getUrl !== undefined) {
            getUrl += 'Filter=' + encodeURIComponent('' + companyname) + '&';
            getUrl += 'Flag=' + 1 + '&';
            /*getUrl += 'SkipCount=' + 0 + '&';
            getUrl += 'MaxResultCount=' + 1 + '&';
            getUrl += 'approvalstatus=' + -1 + '&'; */
            

            this._proxy.request(getUrl, RequestType.Get)
                .pipe(
                    tap(a => {
                        entity = a.items[0];
                        console.log(JSON.stringify(entity));
                    }),
                    switchMap((a) => this._userService.getUsers(a.items[0].InitName, [], 14, false, '', 1, 0)),
                    tap(b => {
                        console.log(JSON.stringify(b));
                        entity.CommID = b.items[0].id;
                        entity.flag = 0;                        
                        entityUser.push(entity);
                        
                    }),
                    concatMap(() => (                        
                        (updUrl !== undefined) ? this._proxy.request(updUrl, RequestType.Post, entityUser) : _observableOf(null)
                    )))
                .subscribe(result => {
                    if (result != null) {
                        if (result.success) {
                            this.notify.success(this.l('UserHasBeenUpdated'));
                        } else {
                            this.notify.info(this.l('UserNotUpdated'));
                        }
                    } else {
                        this.notify.info(this.l('InvalidURL'));
                    }
                });
        }
    }

    private userRole() {
        this._roleService.getRoles(['Pages.Account.Registration.Customer']).subscribe(result => {
            this.role = result.items.filter(x => x.id === 14).map(x => x.name);
            console.log(JSON.stringify(result));
        });
    }
}
jQuery(document).ready(function(event){

//--------------------------------- Stepper Plugin Start ------------------------------------------------------
//---------------------- Function to get current index ---------------------------------------------------------
function returnElementIndex(arrayElement){
    let elementWrp;
    if(arrayElement.classList.contains('stepMain') || arrayElement.classList.contains('stepTitle')){
        elementWrp = arrayElement.parentNode.parentNode.parentNode;
        let elementIndex = Array.from(elementWrp.parentNode.children).indexOf(elementWrp);
        return  Number(elementIndex + 1); //return the index of cur element 
    }
    else
    {
        if(arrayElement.classList.contains('stepControlWrp')){
        elementWrp = arrayElement.parentNode;
        let elementIndex = Array.from(elementWrp.parentNode.children).indexOf(elementWrp);
        return  Number(elementIndex + 1); //return the index of cur element 
        }
        else
        {
            let curActive = document.querySelector('.activeStepContent');
            let elementIndex = Array.from(curActive.parentNode.children).indexOf(curActive);
            elementIndex = Number(elementIndex + 1);
            return elementIndex;
            //if button next or back clicked
        }
    }


}
//-------------------------------------end cur index function -----------------------------------------------------

//------------------- Validation function check for proceeding to next step ---------------------------------
function validateForm(event){
    let elementTriggering = event.srcElement;
    //if controller clicked
    console.log(elementTriggering.classList);
    if(elementTriggering.classList.contains('stepMain') || elementTriggering.classList.contains('stepControlWrp') || elementTriggering.classList.contains('stepTitle')){
        let curIndex = returnElementIndex(elementTriggering);
        let curActive = document.querySelector('.activeStepContent');
        let numActive = returnElementIndex(curActive);
        
        if(curIndex > numActive){
            let hasDisabled = document.querySelectorAll('.activeStepContent .btnContinue');
            if(hasDisabled.length > 0 && !hasDisabled[0].hasAttribute('disabled')){
                return true;
            }
        }
        else{return true;}
    }
    else
    {
        if(elementTriggering.classList.contains('btnContinue'))
        {
            let curIndex = returnElementIndex(elementTriggering);
            let qtyContent = document.querySelectorAll('.stepContent');
            
            if(curIndex < qtyContent.length){
                return true;
            }
            else{
                return false;
            }
        }
        else
        {
            let curIndex = returnElementIndex(elementTriggering);
            if(curIndex > 1){
                return true;
            }
            else{
                return false;
            }
        }
    }
}
//--------------------------------------End of validation function ------------------------------------------- 

    let stepperControl = document.querySelectorAll('.stepControl'),
    stepperContent = document.querySelectorAll('.stepContent'),
    allBtnContinue = document.querySelectorAll('.btnContinue'),
    allBtnBack =  document.querySelectorAll('.btnBack'),
    numOfController = 12 / Number(stepperControl.length),
    tempTitle;


    //attach event for btn back
    for(let i=0;i<allBtnBack.length;i++){
        allBtnBack[i].addEventListener('click',function(event){
            // let curActiveElement = document.querySelector('.activeStepContent');
            let isValid = validateForm(event);
            let indexCurElement = Number(returnElementIndex(event.srcElement) - 1);
            if(isValid){
                let allContent = document.querySelectorAll('.stepContent'),
                allController = document.querySelectorAll('.stepControl'),
                activeContent = document.querySelector('.activeStepContent'),
                activeController = document.querySelector('.activeStepControl');

                activeContent.classList.remove('activeStepContent');
                activeController.classList.remove('activeStepControl');

                allContent[indexCurElement - 1].classList.add('activeStepContent');
                allController[indexCurElement - 1].classList.add('activeStepControl');

            }
        });
    }


    //attach event for btn continue
    for(let i=0;i<allBtnContinue.length;i++){
        let buttonContinue = document.querySelectorAll('.btnContinue');
        buttonContinue[i].addEventListener('click',function(event){
            let activeContent,activeController,nextActiveContent,nextActiveControl;
            let checkActiveContentControl = new Promise(function(resolve,reject){
                activeContent = document.querySelector('.activeStepContent');
                activeController = document.querySelector('.activeStepControl');
                nextActiveContent = document.querySelector('.activeStepContent + div');
                nextActiveControl = document.querySelector('.activeStepControl + div');
                if(activeContent && activeController && nextActiveContent && nextActiveControl){
                    resolve(true);
                }
            });
            checkActiveContentControl.then(function(result){
                let isValid = validateForm(event);
                if(isValid){
                activeContent.classList.remove('activeStepContent');
                activeController.classList.remove('activeStepControl');
                nextActiveContent.classList.add('activeStepContent');
                nextActiveControl.classList.add('activeStepControl');
                }
            });
        });
    }

    //attach event for controller button
    for(let i=0;i<stepperControl.length;i++){

        tempTitle = stepperControl[i].querySelector('.stepText .stepMain');
        tempTitle.innerHTML = Number(i + 1) + '.' +  tempTitle.innerHTML;

        let waitAssignContentShow = new Promise(function(resolve, reject) {
            if(stepperControl[i].classList.contains('activeStepControl'))
            {
                stepperContent[i].classList.add('activeStepContent');
            }
            resolve(true);
        });
        waitAssignContentShow.then(function(result) {
            /* handle a successful result */ 
            stepperControl[i].classList.add('col-sm-' + numOfController);
            stepperControl[i].classList.add('col-md-' + numOfController);     
            stepperControl[i].addEventListener('click', function(event)
                {
                    let isValid = validateForm(event);
                        if(isValid){
                        let activeController = document.querySelectorAll('.activeStepControl');
                        activeController[0].classList.remove('activeStepControl');
                        this.classList.add('activeStepControl');
      
                        let activeContent = document.querySelectorAll('.activeStepContent');
                        activeContent[0].classList.remove('activeStepContent');
                        stepperContent[i].classList.add('activeStepContent');
                        }
                });
        });
    }
//------------------------------- End Stepper --------------------------------------------------------------------
})
