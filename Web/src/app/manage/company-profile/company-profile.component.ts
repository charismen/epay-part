import { ProxyURL } from './../../../shared/service-proxies/generic-service-proxies-url';
import { BaseListComponent } from '@app/shared/form/list/base-list.component';
import { PermissionCheckerService } from 'abp-ng2-module';
import { Component, OnInit, AfterViewInit, EventEmitter, Injector, Output, ElementRef, ViewChild, ViewEncapsulation, Type } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Router } from '@angular/router';
import * as moment from 'moment';
import { DatePipe } from '@angular/common';
import { GenericServiceProxy, RequestType } from '@shared/service-proxies/generic-service-proxies';
import { RoleServiceProxy } from '@shared/service-proxies/service-proxies';
import { toISOFormat } from '@shared/helpers/DateTimeHelper';
import { finalize, tap, switchMap, concatMap, filter } from 'rxjs/operators';
import { TabsetComponent } from 'ngx-bootstrap/tabs';

@Component({
  //selector: 'app-company-profile',
  templateUrl: './company-profile.component.html',
  styleUrls: ['./company-profile.component.less']
})
export class CompanyProfileComponent extends AppComponentBase implements AfterViewInit, OnInit {
  @ViewChild('mainTabs', { static: false }) mainTabs: TabsetComponent;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  constructor(
    injector: Injector,
    private _proxy: GenericServiceProxy,
    private _permissionChecker: PermissionCheckerService,
    private _roleProxy: RoleServiceProxy,
    private _route: Router
  ) {
    super(injector);
  }

  bizRegCookies: any;
  companyDetail: any = {};
  companyLocation: any = {};
  wasteDetails: any = {};
  designationCombo = [];
  packageCombo = [];
  wasteTypeCombo = [];
  industryKeyCombo = [];
  companyStateCombo = [];
  companyCityCombo = [];
  branchStateCombo = [];
  branchCityCombo = [];
  selectedDesignation: any;
  selectedPackage: any;
  selectedWaste: any;
  selectedIndustryKey: any;
  selectedCompanyState: any;
  selectedCompanyCity: any;
  selectedBranchState: any;
  selectedBranchCity: any;
  companyHelper: any = {};
  inputHelper: any = {};
  wasteHelper: any = {};
  saving = false;
  wasteList: any = [];
  newWasteList: any = [];
  nav: number = 1;
  countryCompany: any;

  BranchStateList: any[] = [];
  BranchCityList: any[] = [];
  stateList: any[] = [];
  cityList: any[] = [];
  industryList: any[] = [];
  designationList: any[] = [];
  wasteDetailList: any[] = [];
  itemCodeList: any[] = [];
  checkWaste: any = 1;
  wasteItemName: any;
  wasteComponent: any;
  isSubmited: any = false;
  companyStateUrl = ProxyURL.GetState;
  branchStateUrl = ProxyURL.GetState;
  companyCityUrl = ProxyURL.GetCityCombo;
  branchCityUrl = ProxyURL.GetCityCombo;


  ngOnInit() {
    this.statePopulate();
    this.industryPopulate();
    this.designationPopulate();
    this.wastePopulate();

    let companyUrl = ProxyURL.GetCompanyDetail;
    let wasteUrl = ''; //ProxyURL.GetWasteDetails;
    let packageComboUrl = ProxyURL.GetItemCombo + 'ItemCode=CWC&';
    this.wasteItemName = "Clinical Waste";
    this.bizRegCookies = this.appStorage;
    //companyUrl += this.bizRegCookies.bizRegID !== null ? 'companyid=' + encodeURIComponent(this.bizRegCookies.bizRegID) + '&' + 'LocationID=' + encodeURIComponent(this.bizRegCookies.bizLocID) + '&' : '';
    companyUrl += this.bizRegCookies.bizRegID !== null ? 'bizRegID=' + encodeURIComponent(this.bizRegCookies.bizRegID) + '&' : '';
    wasteUrl += this.bizRegCookies.bizRegID !== null ? 'bizRegID=' + encodeURIComponent(this.bizRegCookies.bizRegID) + '&' : '';

    this._proxy.request(companyUrl, RequestType.Get)
      .pipe(
        tap((b) => {

        }),
        switchMap((a) => this._proxy.request(wasteUrl, RequestType.Get)),
        tap((b) => {
          if (b.length > 0) {
            this.wasteHelper = b[0];
          }


        }),
        switchMap((c) => this._proxy.request(packageComboUrl, RequestType.Get)),
        tap((d) => {
          this.packageCombo = d;
          // this.selectedPackage = this.wasteDetails.CommID;
        })
      )
      .subscribe(result => { });

    this._proxy.request('', RequestType.Get) //ProxyURL.GetPackageList
      .subscribe(result => {
        this.wasteList = result;

        let output = [];
        this.wasteList.forEach(function (item) {
          let existing = output.filter(function (v, i) {
            return v.ItemCode == item.ItemCode;
          });
          if (existing.length) {
            let existingIndex = output.indexOf(existing[0]);
            output[existingIndex].ConsumableItem = output[existingIndex].ConsumableItem.concat(item.ConsumableItem);
            output[existingIndex].ConsumableDesc = output[existingIndex].ConsumableDesc.concat(item.ConsumableDesc);
          } else {
            if (typeof item.ConsumableItem === 'string') {
              item.ConsumableItem = [item.ConsumableItem];
            }
            if (typeof item.ConsumableDesc === 'string') {
              item.ConsumableDesc = [item.ConsumableDesc];
            }
            output.push(item);
          }
        });
        this.newWasteList = output;
      });

    // this.inputHelper.BizRegID = '';
    // this.inputHelper.TelNo = '';
    // this.inputHelper.FaxNo = '';
    // this.inputHelper.Email = '';
    // this.inputHelper.ContactPerson = '';
    // this.inputHelper.ContactDesignation = '';

    this.populateData(companyUrl);

    this.wasteHelper.ItemName = "Clinical Waste";
    this.wasteHelper.ShortDesc = "Refer to Annexure 1 Section 6";
    this.itemCodeList.unshift({ "Code": "SW404", "CodeDesc": "SW404" });

    // if(this.wasteHelper.ItemName == undefined){

    // }
  }

  ngAfterViewInit() {

  }


  setClass(num: number) {
    this.nav = num;
  }


  getNavClass(num: number) {
    let classes = num == this.nav ? 'nav-link active' : 'nav-link';
    return classes;
  }

  getContentClass(num: number) {
    let classes = num == this.nav ? 'tab-pane fadeIn active show animated kt-margin-t-20 fadeIn' : 'tab-pane fadeIn';
    return classes;
  }

  stateChanged(): void {

    let countryId = this.stateList.filter(x => x.Code === this.selectedCompanyState)[0].CountryCode;
    this.countryCompany = countryId;
    let cityURL = ProxyURL.GetCityCombo + "countryCode=" + countryId + "&stateCode=" + this.selectedCompanyState + "&";

    this._proxy.request(cityURL, RequestType.Get)
      .pipe(finalize(() => {
      }))
      .subscribe(result => {
        this.cityList = result;
      });
  }

  statePopulate(): void {
    let stateURL = ProxyURL.GetState;
    this._proxy.request(stateURL, RequestType.Get)
      .pipe(finalize(() => {
      }))
      .subscribe(result => {
        this.stateList = result;
        this.BranchStateList = result;
      });

  }

  BranchStateChanged(): void {

    let countryId = this.BranchStateList.filter(x => x.Code === this.selectedBranchState)[0].CountryCode;
    let cityURL = ProxyURL.GetCityCombo + "countryCode=" + countryId + "&stateCode=" + this.selectedBranchState + "&";

    this._proxy.request(cityURL, RequestType.Get)
      .pipe(finalize(() => {
      }))
      .subscribe(result => {
        this.BranchCityList = result;
      });
  }

  industryPopulate(): void {
    let industryURL = ProxyURL.GetCodeMaster + "code=CDC";
    this._proxy.request(industryURL, RequestType.Get)
      .pipe(finalize(() => {
      }))
      .subscribe(result => {
        this.industryList = result;
      });
  }

  wastePopulate(): void {
    let codeUrl = ProxyURL.GetCodeMaster + 'code=WTY&';
    this._proxy.request(codeUrl, RequestType.Get)
      .subscribe(result => {

        this.wasteDetailList = result.filter(x => x.SysCode === 1);

      });

  }

  designationPopulate(): void {
    let designURL = ProxyURL.GetCodeMaster + "code=DSN";
    this._proxy.request(designURL, RequestType.Get)
      .pipe(finalize(() => {
      }))
      .subscribe(result => {
        this.designationList = result;
      });
  }

  wasteDetailChanged(): void {
    this.wasteDetails.ItemDesc = this.wasteDetailList.filter(x => x.Code === this.wasteHelper.ItemType)[0].CodeDesc;

  }

  saveCustomer(): void {
    this.spinnerService.show();
    let data: any = {};
    let dataEmployee: any = {};
    let dataEmpbranch: any = {};
    let dataPush: any = {};

    // this.inputHelper.bizlocate=[];

    this.inputHelper.Country = this.countryCompany;
    this.inputHelper.State = this.selectedCompanyState;
    this.inputHelper.City = this.selectedCompanyCity;
    this.inputHelper.ContactDesignation = this.inputHelper.designationCode;
    this.inputHelper.CompanyType = 3;//this.inputHelper.designationCode;
    this.inputHelper.IndustryType = this.selectedIndustryKey;
    this.inputHelper.Active = 0;
    this.inputHelper.Flag = 1;
    this.inputHelper.PostalCode = this.inputHelper.PostalCode;
    // this.inputHelper.CreateBy = this.appSession.user.userName;
    this.inputHelper.Status = 0;
    this.inputHelper.UpdateBy = this.appSession.user.userName;
    dataPush.BizEntity = this.inputHelper;

    data.BizRegID = this.inputHelper.BizRegID;
    data.BizLocID = this.inputHelper.BizLocID;
    data.BranchName = this.inputHelper.BranchName;
    data.BizRegType = 0;
    data.BizRegStatus = 0;
    data.CommID = this.inputHelper.CommID;
    data.IndustryType = this.selectedIndustryKey;
    data.Address1 = this.inputHelper.BranchAddress1;
    data.Address2 = this.inputHelper.BranchAddress2;
    data.Address3 = this.inputHelper.BranchAddress3;
    data.PostalCode = this.inputHelper.BranchPostCode;
    data.ContactPerson = this.inputHelper.ContactPerson;
    data.ContactDesignation = this.inputHelper.designationCode;
    data.ContactEmail = this.inputHelper.Email;
    data.ContactTelNo = this.inputHelper.ContactPersonTelNo;
    data.ContactMobile = this.inputHelper.ContactPersonMobile;
    data.Email = this.inputHelper.Email;
    data.Tel = this.inputHelper.BranchTelNo;
    data.Fax = this.inputHelper.BranchFaxNo;
    data.Country = this.countryCompany;
    data.State = this.selectedBranchState;
    data.City = this.selectedBranchCity;
    data.Active = 0;
    data.Status = 0;
    data.Flag = 1;
    data.UpdateBy = this.appSession.user.userName;
    // data.CreateBy = this.appSession.user.userName;
    // this.inputHelper.bizlocate.push(data);
    dataPush.BizLocate = data;

    // dataEmployee.EmployeeID="xxx";


    // let url = ProxyURL.AddCustomer;
    let url = ProxyURL.UpdateCustomerOne;

    this.saving = true;
    if (url !== undefined) {
      this._proxy.request(url, RequestType.Post, dataPush)
        .pipe(finalize(() => { this.spinnerService.hide(); }))
        .subscribe((result) => {
          if (result.success) {
            this.notify.success(this.l('SavedSuccessfully'));
            // this._route.navigate(['/account/login']);
          } else {
            this.notify.error(this.l('Failed'));
            this.spinnerService.hide();
          }
        });
    }

  }

  populateData(url: any): void {
    this._proxy.request(url, RequestType.Get)
      .subscribe(result => {
        this.inputHelper = result;
        this.selectedDesignation = this.inputHelper.ContactDesignation;
        if (this.inputHelper.BizRegStatus == 4) {
          this.isSubmited = true;
        }
        this.selectedCompanyState = this.inputHelper.CompanyState;
        this.stateChanged();
        this.selectedCompanyCity = this.inputHelper.CompanyCity;
        this.selectedBranchState = this.inputHelper.BranchState;
        this.BranchStateChanged();
        this.selectedBranchCity = this.inputHelper.BranchCity;
        this.selectedIndustryKey = this.inputHelper.IndustryType;
        this.inputHelper.designationCode = this.inputHelper.ContactDesignation;
        this.inputHelper.Address1 = this.inputHelper.CompanyAddress1;
        this.inputHelper.Address2 = this.inputHelper.CompanyAddress2;
        this.inputHelper.Address3 = this.inputHelper.CompanyAddress3;
      });
  }

  nextTabTnc(data: any): void {
    let activeTab = this.mainTabs.tabs.findIndex(x => x.active === true);
    setTimeout(() => {
      this.mainTabs.tabs[activeTab + 1].active = true;
    }, 0);
  }

  saveWaste(): void {
    this.spinnerService.show();
    let dataPush: any = [];
    this.wasteHelper.BizRegID = this.appStorage.bizRegID;
    this.wasteHelper.BizLocID = this.appStorage.bizLocID;
    this.wasteHelper.SeqNo = 1;
    this.wasteHelper.CreateBy = this.appSession.user.userName;
    this.wasteHelper.DefUOM = "kg";

    let url = ''; //ProxyURL.SetWasteDetail;

    this.saving = true;
    if (url !== undefined) {
      this._proxy.request(url, RequestType.Post, this.wasteHelper)
        .pipe(finalize(() => { this.spinnerService.hide(); }))
        .subscribe((result) => {
          if (result.success) {
            this.notify.success(this.l('SavedSuccessfully'));
            // this._route.navigate(['/account/login']);
          } else {
            this.notify.error(this.l('Failed'));
            this.spinnerService.hide();
          }
        });
    }
  }

  savePackage() {
    let data: any = {};
    if (this.checkWaste == 0) {
      this.notify.error(this.l('CheckWasteFirst'));
    }
    else {
      this.spinnerService.show();

      data.checkWaste = this.checkWaste;
      data.selectedPackage = this.selectedPackage;
      data.createdBy = this.appSession.user.userName;

      let url = ProxyURL.AddContractRegistration;

      this.saving = true;
      if (url !== undefined) {
        this._proxy.request(url, RequestType.Post, data)
          .pipe(finalize(() => { this.spinnerService.hide(); }))
          .subscribe((result) => {
            if (result.success) {
              this.notify.success(this.l('SavedSuccessfully'));
              // this._route.navigate(['/account/login']);
              window.location.reload();
            } else {
              this.notify.error(this.l('Failed'));
              this.spinnerService.hide();
            }
          });
      }

    }
  }

}
