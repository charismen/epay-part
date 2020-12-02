import { UserServiceProxy } from '@shared/service-proxies/service-proxies';
import { GenericServiceProxy, RequestType } from '@shared/service-proxies/generic-service-proxies';
import { ProxyURL } from '@shared/service-proxies/generic-service-proxies-url';
import { UserEditDto, CreateOrUpdateUserInput } from '@shared/service-proxies/service-proxies';
import { BaseListComponent } from '@app/shared/form/list/base-list.component';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { AppComponentBase } from 'shared/common/app-component-base';
import { Component, Injector, ViewChild, OnInit, AfterViewInit, NgZone, Pipe } from '@angular/core';
import { Observable, combineLatest as _observableCombineLatest, of as _observableOf } from 'rxjs';
import { finalize, tap, switchMap, concatMap } from 'rxjs/operators';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { LoginService } from '../login/login.service';
import { AppAuthService } from '../../app/shared/common/auth/app-auth.service';
import { Router } from '@angular/router';
import { AppConsts } from '@shared/AppConsts';

@Component({
    selector: 'registrationModal',
    templateUrl: './lgm-registration-modal.component.html',
    styleUrls: ['./lgm-registration-modal.component.less'],
    animations: [accountModuleAnimation()]
})
export class RegistrationModalComponent extends AppComponentBase implements AfterViewInit {

    @ViewChild('registrationModal', { static: true }) modal: ModalDirective;
    @ViewChild('baseList', { static: false }) baseList: BaseListComponent;

    isUAT = AppConsts.isUAT;

    active = false;
    saving = false;
    loading = false;

    individual = false;

    comboModel: any = [];
    countryCombo: any = [];
    stateCombo: any = [];
    districtCombo: any = [];
    townCombo: any = [];

    selectedCountry: any;
    selectedState: any;
    selectedDistrict: any;
    selectedTown: any;
    EmailRegister: string;
    UserRegister: string;

    inputHelper: any = {};
    registratedUser: any = {};
    selfRegModel: any = {};
    userHelper: UserEditDto = new UserEditDto();

    gridUrlRegister = ProxyURL.SelfRegCompany;
    gridUrlCompany = ProxyURL.GetCompanyDetail;
    // urlEmail = ProxyURL.RegisterUserEmail;
    urlEmail = ProxyURL.RegisterUserEmailNotif;
    checkCustomerUrl = ProxyURL.CheckCustomer;
    getAllOutstanding = ProxyURL.JobGetAllOutstanding;

    checkbox = false;
    role: any;
    filter = '';
    gridUrl = ProxyURL.GetCustomer; //ProxyURL.GetCustomer + 'Flag=0&Sorting=""&SkipCount=0&MaxResultCount=10&Filter=';
    verify = false;
    tableIsShown = false;
    isChoosed = false;
    validated = false;
    showRegistrated = false;
    continue = true;
    regComplete = false;

    AccNotInList: any;

    constructor(
        injector: Injector,
        private _proxy: GenericServiceProxy,
        private _userService: UserServiceProxy,
        private readonly _loginService: LoginService,
        private _route: Router,
        private _appAuthService: AppAuthService

    ) {
        super(injector);
    }

    show(): void {
        this.active = true;

        this.verify = false;
        this.individual = false;

        this.tableIsShown = false;

        this.inputHelper.ROCNumber = '-';
        this.inputHelper.address2 = '';
        this.inputHelper.address3 = '';
        this.inputHelper.address4 = '';
        this.inputHelper.country = '-';
        this.inputHelper.postalCode = '';
        this.inputHelper.state = '-';
        this.inputHelper.area = '-';
        this.inputHelper.city = '-';
        this.inputHelper.TID = 1;
        this.inputHelper.regNo = '';
        this.inputHelper.telNo = '';
        this.inputHelper.createBy = '';
        this.inputHelper.bizRegID = '';

        this.userHelper.surname = '';
        this.userHelper.name = '';
        this.userHelper.userName = '';
        this.userHelper.emailAddress = '';
        this.userHelper.phoneNumber = '';
        this.userHelper.password = '123456';

        this.selectedCountry = null;
        this.selectedState = null;
        this.stateCombo = [];
        this.selectedDistrict = null;
        this.districtCombo = [];
        this.selectedTown = null;
        this.townCombo = [];

        this.getAccNotIn();

        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
        this.verify = false;
    }

    ngAfterViewInit() {
        this._proxy.request(ProxyURL.GetCityCombo, RequestType.Get)
            .subscribe(result => {
                this.comboModel = result;
            });

        this._proxy.request(ProxyURL.GetCountryCombo + 'countryCode=' + encodeURIComponent('MY'), RequestType.Get)
            .subscribe(result => {
                this.countryCombo = result;
            });
    }

    getAccNotIn() {
        let listUrl = ProxyURL.GetListMaintenance + 'branchId=' + encodeURIComponent('06') + '&' + 'prefix=' + encodeURIComponent('ADR') + '&';
        this._proxy.request(listUrl, RequestType.Get)
            .subscribe((result) => {
                this.AccNotInList = result.items.map(x => x.SYSKey);
                console.log('AccNotInList: ' + this.AccNotInList);
            });
    }

    getCustomer(data?: any) {
        this.getAccNotIn();
        this.loading = true;
        if (this.filter.length >= 8) {
            this.tableIsShown = true;
        }

        this.baseList.setURL(ProxyURL.GetCustomer + 'Filter=' + encodeURIComponent(this.filter) + '&' + 'AccountNotIn=' + encodeURIComponent('' + this.AccNotInList) + '&');

        setTimeout(() => {
            this.baseList.refresh();
            this.loading = false;
        }, 50);
    }

    checkValue(): void {
        if (!this.isUAT && (this.userHelper.emailAddress === '' || this.userHelper.emailAddress === null)) {
            this.message.info(this.l('NullEmailAddress')).then(() => {
                this.filter = '';
                $(".stepControl1").click();
                this.resetStepper();
            });
        }
    }

    setCustomer(data?: any) {
        //this.customer = data[0];
        this.checkbox = true;
        this.continue = false;
        this.loading = true;

        this.inputHelper.acctNo = data[0].ADD_CODE.trim();
        this.inputHelper.address1 = data[0].ADDRESS_3.trim();
        this.inputHelper.companyName = data[0].ADDRESS_1.trim();
        this.inputHelper.address2 = data[0].ADDRESS_4.trim();
        this.inputHelper.address3 = data[0].ADDRESS_5.trim();
        this.inputHelper.address4 = data[0].ADDRESS_6.trim();
        this.inputHelper.postalCode = '';
        this.selectedState = '';
        this.selectedDistrict = '';
        this.selectedTown = '';

        this.userHelper.phoneNumber = data[0].TELEPHONE.trim();
        this.userHelper.surname = data[0].CONTACT.trim();
        this.userHelper.name = data[0].ADDRESS_1.trim();

        let emailAddr = data[0].E_MAIL.trim();
        if (emailAddr.indexOf(',') >= 1) {
            emailAddr = emailAddr.substr(0, emailAddr.indexOf(','));
        } else if (emailAddr.indexOf(';') >= 1) {
            emailAddr = emailAddr.substr(0, emailAddr.indexOf(';'));
        }
        // else if (!this.isUAT && (emailAddr === '' || emailAddr === null)) {
        //     this.message.info(this.l('NullEmailAddress')).then(() => {
        //         // this.filter = '';
        //         $(".stepControl1").click();
        //         this.resetStepper();
        //     });
        // }
        this.userHelper.emailAddress = emailAddr.trim();

        this.userHelper.userName = this.inputHelper.acctNo;
        this.isChoosed = true;
        this.tableIsShown = false;
        this.verify = true;

        let checkCustomerUrl = this.checkCustomerUrl + 'accountNo=' + encodeURIComponent(this.inputHelper.acctNo) + '&';

        this._proxy.request(checkCustomerUrl, RequestType.Get)
            .pipe(finalize(() => { this.loading = false; }))
            .subscribe((result) => {
                if (result.success === true || result.returnData === null) {
                    this.showRegistrated = false;
                    this.validated = true;
                    this.continue = false;
                } else if (result.success === false || result.returnData !== null) {
                    this.showRegistrated = true;
                    this.validated = false;
                    this.continue = true;
                    this.registratedUser.name = result.returnData.companyName;
                    this.registratedUser.email = result.returnData.email;
                    this.message.warn(this.l(result.message + ' ' + '<br />' + result.returnData.companyName + ' ' + '<br />' + result.returnData.email)).then(() => {
                        this.reEnter();
                    });
                }
            });
    }

    reEnter() {
        this.verify = false;
        this.tableIsShown = false;
        this.isChoosed = false;
        this.validated = false;
        this.showRegistrated = false;
        this.continue = true;
        this.checkbox = false;
    }

    onSelectedCountry(data: any): void {
        this.selectedState = null;
        this.stateCombo = [];
        this.selectedDistrict = null;
        this.districtCombo = [];
        this.selectedTown = null;
        this.townCombo = [];

        if (this.selectedCountry) {
            let stateUrl = ProxyURL.GetState + 'countryCode=' + encodeURIComponent(this.selectedCountry.Code) + '&';
            this._proxy.request(stateUrl, RequestType.Get)
                .subscribe(result => {
                    this.stateCombo = result;
                });
        }
    }

    onSelectedState(data: any): void {
        this.selectedDistrict = null;
        this.districtCombo = [];
        this.selectedTown = null;
        this.townCombo = [];

        if (this.selectedCountry && this.selectedState) {
            let cityUrl = ProxyURL.GetCityCombo + 'countryCode=' + encodeURIComponent(this.selectedCountry.Code) + '&' + 'stateCode=' + encodeURIComponent(this.selectedState.Code) + '&';
            this._proxy.request(cityUrl, RequestType.Get)
                .subscribe(result => {
                    this.districtCombo = result;
                });
        }
    }

    onSelectedDistrict(data: any): void {
        this.selectedTown = null;
        this.townCombo = [];

        if (this.selectedCountry && this.selectedState && this.selectedDistrict) {
            let areaUrl = ProxyURL.GetArea + 'countryCode=' + encodeURIComponent(this.selectedCountry.Code) + '&' + 'stateCode=' + encodeURIComponent(this.selectedState.Code) + '&' + 'cityCode=' + encodeURIComponent(this.selectedDistrict.Code) + '&';
            this._proxy.request(areaUrl, RequestType.Get)
                .subscribe(result => {
                    if (result.length === 0) {
                        this.townCombo = [{ 'Code': '-', 'Remark': 'Town Unavailable' }];
                    } else {
                        this.townCombo = result;
                    }
                });
        }
    }

    onSelectedTown(data: any): void {
    }

    onRegistType(): void {
        this.individual = false;
    }

    save(data?: any) {
        this.loading = true;
        let send: any = [];
        let userModel = new CreateOrUpdateUserInput();
        let empbranch: any = {};
        let bizEntity: any = {};
        let bizLocate: any = {};
        let employee: any = {};

        this.selfRegModel = {
            bizentity: bizEntity,
            bizlocate: bizLocate,
            empbranch: empbranch,
            employee: employee,
            mode: 1
        };

        this.inputHelper.acctNo === undefined ? this.inputHelper.BizRegID = '' : this.inputHelper.BizRegID = this.inputHelper.acctNo.trim();

        this.inputHelper.companyType = 0;
        this.inputHelper.faxNo = this.inputHelper.telNo;
        this.inputHelper.contactPersonTelNo = this.inputHelper.telNo;
        this.inputHelper.active = 0;
        this.inputHelper.flag = 1;
        this.inputHelper.inuse = 0;
        this.inputHelper.ishost = 0;
        this.inputHelper.Country = this.selectedCountry.Code;
        this.inputHelper.State = this.selectedState.Code;
        this.inputHelper.City = this.selectedDistrict.Code;
        this.selectedTown !== null ? this.inputHelper.Area = this.selectedTown.Code : this.inputHelper.Area = '';

        this.inputHelper.InitName = this.userHelper.userName;
        this.inputHelper.contactPerson = this.userHelper.name;
        this.inputHelper.email = this.userHelper.emailAddress;
        this.inputHelper.isEmailConfirmed = true;

        send.push(this.inputHelper);

        bizEntity.bizregid = 'xxx';
        bizEntity.acctno = this.inputHelper.acctNo;
        bizEntity.companyName = this.inputHelper.companyName !== undefined ? this.inputHelper.companyName : this.userHelper.name;
        bizEntity.country = this.inputHelper.Country;
        bizEntity.state = this.inputHelper.State;
        bizEntity.city = this.inputHelper.City;
        bizEntity.area = this.inputHelper.Area;
        bizEntity.email = this.inputHelper.email;
        bizEntity.address1 = this.inputHelper.address1;
        bizEntity.address2 = this.inputHelper.address2;
        bizEntity.address3 = this.inputHelper.address3;
        bizEntity.address4 = this.inputHelper.address4;
        bizEntity.contactperson = this.userHelper.surname;
        bizEntity.active = 0;
        bizEntity.inuse = 0;
        bizEntity.flag = 1;
        bizEntity.ishost = 0;

        bizLocate.bizregid = bizEntity.bizregid;
        bizLocate.bizlocid = 'xxx';
        bizLocate.branchname = this.inputHelper.companyName !== undefined ? this.inputHelper.companyName : this.userHelper.name;
        bizLocate.country = this.inputHelper.Country;
        bizLocate.state = this.inputHelper.State;
        bizLocate.city = this.inputHelper.City;
        bizLocate.area = this.inputHelper.Area;
        bizEntity.address1 = this.inputHelper.address1;
        bizEntity.address2 = this.inputHelper.address2;
        bizEntity.address3 = this.inputHelper.address3;
        bizEntity.address4 = this.inputHelper.address4;
        bizEntity.contactperson = this.userHelper.surname;
        bizLocate.active = 0;
        bizLocate.inuse = 0;
        bizLocate.flag = 1;
        bizLocate.ishost = 0;

        empbranch.employeeid = 'xxx';
        empbranch.bizlocid = bizLocate.bizlocid;
        empbranch.active = 0;
        empbranch.inuse = 0;
        empbranch.flag = 1;
        empbranch.ishost = 0;

        employee.employeeid = 'xxx';
        employee.bizregid = bizLocate.bizregid;
        employee.bizlocid = bizLocate.bizlocid;
        employee.firstname = this.inputHelper.companyName !== undefined ? this.inputHelper.companyName : this.userHelper.name;
        employee.surname = this.userHelper.surname;
        employee.sex = 'M';
        employee.costate = '';
        employee.cocountry = '';
        employee.pnstate = '';
        employee.pncountry = '';
        employee.nickname = this.userHelper.userName;
        employee.emailaddress = this.userHelper.emailAddress;
        employee.emercontactno = this.userHelper.phoneNumber;
        employee.active = 0;
        employee.inuse = 0;
        employee.flag = 1;
        employee.ishost = 0;

        if (!this.isUAT && (this.inputHelper.email === '' || this.inputHelper.email === null)) {
            this.message.info(this.l('NullEmailAddress')).then(() => {
                this.filter = '';
                $(".stepControl1").click();
                this.resetStepper();
            });
        } else {
            let selfRegUrl = ProxyURL.SelfRegCompany;
            this._proxy.request(selfRegUrl, RequestType.Post, this.selfRegModel)
                .pipe(
                    finalize(() => { this.loading = false; })
                )
                .subscribe((result) => {
                    this.sendEmail(result.items);
                });
        }
    }

    sendEmail(data): void {
        let contEmail: any = {};
        contEmail.emailAddress = this.userHelper.emailAddress;
        contEmail.userID = this.userHelper.userName;
        contEmail.password = data.uniqueString;
        contEmail.accountNumber = this.userHelper.userName;

        this.getAllOutstanding += 'customerCode=' + encodeURIComponent(this.inputHelper.acctNo) + '&';
        this.getAllOutstanding += 'limit=' + encodeURIComponent('' + 100) + '&';
        this.getAllOutstanding += 'offset=' + encodeURIComponent('' + 0) + '&';

        _observableCombineLatest([
            this._proxy.request(this.urlEmail, RequestType.Post, contEmail),
            this._proxy.request(this.getAllOutstanding, RequestType.Get)
        ]).subscribe(result => {
            //this.logout();
            this.message.info(this.l('RegistrationSuccessPopUp')).then(() => {
                this.close();
                this.filter = '';
                $(".stepControl1").click();
                this.resetStepper();
            });
         });

        // this._proxy.request(this.urlEmail, RequestType.Post, contEmail)
        //     .pipe(
        //         switchMap((a) => (
        //             this._proxy.request(this.getAllOutstanding, RequestType.Get)
        //         ))
        //     ).subscribe(result => {
        //     });
    }

    logout() {
        this._appAuthService.logout(false);
        this.loading = false;
        this._loginService.authenticateModel.userNameOrEmailAddress = '';
        this._loginService.authenticateModel.password = '';
    }

    resetStepper(): void {
        let allContent = document.querySelectorAll('.stepContent'),
            allController = document.querySelectorAll('.stepControl'),
            activeContent = document.querySelector('.activeStepContent'),
            activeController = document.querySelector('.activeStepControl'),
            allInputText = document.querySelectorAll('input[type="text"]');

        //clear all text input
        for (let i = 0; i < allInputText.length; i++) {
            let curInput = allInputText[i] as HTMLInputElement;
            curInput.value = '';
        }

        activeContent.classList.remove('activeStepContent');
        activeController.classList.remove('activeStepControl');

        allContent[0].classList.add('activeStepContent');
        allController[0].classList.add('activeStepControl');

        //set the first sbumit button to disabled
        let activeSubmit = allContent[0].querySelector('.btnSubmit') as HTMLButtonElement;
        activeSubmit.disabled = true;
    }
}

jQuery(document).ready(function (event) {

    //--------------------------------- Stepper Plugin Start ------------------------------------------------------
    //------------------------------------- Reset the registration form --------------------------------------------
    function resetStepper() {
        let allContent = document.querySelectorAll('.stepContent'),
            allController = document.querySelectorAll('.stepControl'),
            activeContent = document.querySelector('.activeStepContent'),
            activeController = document.querySelector('.activeStepControl'),
            allInputText = document.querySelectorAll('input[type="text"]');

        //clear all text input
        for (let i = 0; i < allInputText.length; i++) {
            let curInput = allInputText[i] as HTMLInputElement;
            curInput.value = '';
        }

        activeContent.classList.remove('activeStepContent');
        activeController.classList.remove('activeStepControl');

        allContent[0].classList.add('activeStepContent');
        allController[0].classList.add('activeStepControl');

        //set the first sbumit button to disabled
        let activeSubmit = allContent[0].querySelector('.btnSubmit') as HTMLButtonElement;
        activeSubmit.disabled = true;
    }

    //---------------------- Function to jump to Nth Controller n index --------------------------------------------
    function activateNth(elPos) {
        let targetEl = elPos - 1,
            activeIndex = document.querySelectorAll('.stepControl'), activePos,
            allContent = document.querySelectorAll('.stepContent'),
            allController = document.querySelectorAll('.stepControl'),
            activeContent = document.querySelector('.activeStepContent'),
            activeController = document.querySelector('.activeStepControl'),
            allInputText = document.querySelectorAll('input[type="text"]');

        activeContent.classList.remove('activeStepContent');
        activeController.classList.remove('activeStepControl');

        allContent[targetEl].classList.add('activeStepContent');
        allController[targetEl].classList.add('activeStepControl');

        for (let i = 0; i < activeIndex.length; i++) {
            let curEl = activeIndex[i];
            if (curEl.classList.contains('activeStepControl')) {
                activePos = i;
                for (let a = activePos; a <= targetEl; a++) {
                    let activeSubmit = allContent[a].querySelector('.btnSubmit') as HTMLButtonElement;
                    activeSubmit.disabled = false;
                }
            }
        }

    }
    //---------------------- Function to get current index ---------------------------------------------------------
    function returnElementIndex(arrayElement) {
        let elementWrp;
        if (arrayElement.classList.contains('stepMain') || arrayElement.classList.contains('stepTitle')) {
            elementWrp = arrayElement.parentNode.parentNode.parentNode;
            let elementIndex = Array.from(elementWrp.parentNode.children).indexOf(elementWrp);
            return Number(elementIndex + 1); //return the index of cur element
        } else {
            if (arrayElement.classList.contains('stepControlWrp')) {
                elementWrp = arrayElement.parentNode;
                let elementIndex = Array.from(elementWrp.parentNode.children).indexOf(elementWrp);
                return Number(elementIndex + 1); //return the index of cur element
            } else {
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
    function validateForm(event) {
        let elementTriggering = event.srcElement;
        //if controller clicked
        // console.log(elementTriggering.classList);
        if (elementTriggering.classList.contains('stepMain') || elementTriggering.classList.contains('stepControlWrp') || elementTriggering.classList.contains('stepTitle')) {
            let curIndex = returnElementIndex(elementTriggering);
            let curActive = document.querySelector('.activeStepContent');
            let numActive = returnElementIndex(curActive);
            if (curIndex > numActive) {
                let hasDisabled = document.querySelectorAll('.activeStepContent .btnContinue');
                if (hasDisabled.length > 0 && !hasDisabled[0].hasAttribute('disabled')) {
                    return true;
                }
            } else { return true; }
        } else {
            if (elementTriggering.classList.contains('btnContinue')) {
                let curIndex = returnElementIndex(elementTriggering);
                let qtyContent = document.querySelectorAll('.stepContent');
                if (curIndex < qtyContent.length) {
                    return true;
                } else {
                    return false;
                }
            } else {
                let curIndex = returnElementIndex(elementTriggering);
                if (curIndex > 1) {
                    return true;
                } else {
                    return false;
                }
            }
        }
    }
    //--------------------------------------End of validation function -------------------------------------------

    let stepperControl = document.querySelectorAll('.stepControl'),
        stepperContent = document.querySelectorAll('.stepContent'),
        allBtnContinue = document.querySelectorAll('.btnContinue'),
        allBtnBack = document.querySelectorAll('.btnBack'),
        btnClose = document.querySelectorAll('#btnClose'),
        numOfController = 12 / Number(stepperControl.length),
        tempTitle;

    for (let i = 0; i < btnClose.length; i++) {
        btnClose[i].addEventListener('click', function (event) {
            resetStepper();
            resetStepper();

        });
    }



    //attach event for btn back
    for (let i = 0; i < allBtnBack.length; i++) {
        allBtnBack[i].addEventListener('click', function (event) {
            // let curActiveElement = document.querySelector('.activeStepContent');
            let isValid = validateForm(event);
            let indexCurElement = Number(returnElementIndex(event.srcElement) - 1);
            if (isValid) {
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
    for (let i = 0; i < allBtnContinue.length; i++) {
        let buttonContinue = document.querySelectorAll('.btnContinue');
        buttonContinue[i].addEventListener('click', function (event) {
            let activeContent, activeController, nextActiveContent, nextActiveControl;
            let checkActiveContentControl = new Promise(function (resolve, reject) {
                activeContent = document.querySelector('.activeStepContent');
                activeController = document.querySelector('.activeStepControl');
                nextActiveContent = document.querySelector('.activeStepContent + div');
                nextActiveControl = document.querySelector('.activeStepControl + div');
                if (activeContent && activeController && nextActiveContent && nextActiveControl) {
                    resolve(true);
                }
            });
            checkActiveContentControl.then(function (result) {
                let isValid = validateForm(event);
                if (isValid) {
                    activeContent.classList.remove('activeStepContent');
                    activeController.classList.remove('activeStepControl');
                    nextActiveContent.classList.add('activeStepContent');
                    nextActiveControl.classList.add('activeStepControl');
                }
            });
        });
    }

    //attach event for controller button
    for (let i = 0; i < stepperControl.length; i++) {

        tempTitle = stepperControl[i].querySelector('.stepText .stepMain');
        tempTitle.innerHTML = Number(i + 1) + '.' + tempTitle.innerHTML;

        let waitAssignContentShow = new Promise(function (resolve, reject) {
            if (stepperControl[i].classList.contains('activeStepControl')) {
                stepperContent[i].classList.add('activeStepContent');
            }
            resolve(true);
        });
        waitAssignContentShow.then(function (result) {
            /* handle a successful result */
            stepperControl[i].classList.add('col-sm-' + numOfController);
            stepperControl[i].classList.add('col-md-' + numOfController);
            stepperControl[i].addEventListener('click', function (event) {
                let isValid = validateForm(event);
                if (isValid) {
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
    let theHeader = document.querySelector('registrationmodal .linkIndividual');
    theHeader.addEventListener('click', function (e) {
        //resetStepper();
        activateNth(2);
    });
});
