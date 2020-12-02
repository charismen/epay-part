import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Component, EventEmitter, Injector, Output, ViewChild, OnInit, AfterViewInit, ViewEncapsulation } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { Router } from '@angular/router';
import { GenericServiceProxy, RequestType } from '@shared/service-proxies/generic-service-proxies';
import { ProxyURL } from '@shared/service-proxies/generic-service-proxies-url';
import { SaveType, BizRegBizLoc } from '@shared/AppEnums';
import * as _ from 'lodash';
import { finalize } from 'rxjs/operators';
import { Table } from 'primeng/table';
import { LazyLoadEvent } from 'primeng/public_api';
import { Paginator } from 'primeng/paginator';

@Component({
    selector: 'editCustomerModal',
    templateUrl: './edit-customer-modal.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [accountModuleAnimation()]
})

export class EditCustomerModalComponent extends AppComponentBase implements OnInit, AfterViewInit {

    @ViewChild('createModal', { static: true }) modal: ModalDirective;
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    gridUrl = ProxyURL.GetLGMCompany;

    inputHelper: any = {};
    inputHelper2: any = {};
    statusdesc: any;
    active = false;
    view = true;
    saving = false;
    selectedCountry: any;
    selectedState: any;
    selectedDistrict: any;
    selectedTown: any;
    countryCombo: any = [];
    stateCombo: any = [];
    districtCombo: any = [];
    townCombo: any = [];
    comboModel: any = [];

    constructor(
        injector: Injector,
        private _route: Router,
        private __proxy: GenericServiceProxy,
        private _bizRegBizLoc: BizRegBizLoc
    ) {
        super(injector);
    }

    ngOnInit(): void {

    }

    ngAfterViewInit(): void {
        this.__proxy.request(ProxyURL.GetCityCombo, RequestType.Get)
            .subscribe(result => {
                this.comboModel = result;
            });

        this.__proxy.request(ProxyURL.GetCountryCombo + 'countryCode=' + encodeURIComponent('MY'), RequestType.Get)
            .subscribe(result => {
                this.countryCombo = result;
            });
    }

    getCompany(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }

        let url = this.gridUrl += 'isApprove=' + encodeURIComponent('' + -2) + '&';
        if (url !== undefined) {
            if (this.primengTableHelper.getSorting(this.dataTable) !== undefined) { url += 'Sorting=' + encodeURIComponent('' + this.primengTableHelper.getSorting(this.dataTable)) + '&'; }
            if (this.primengTableHelper.getSkipCount(this.paginator, event) !== undefined) { url += 'SkipCount=' + encodeURIComponent('' + this.primengTableHelper.getSkipCount(this.paginator, event)) + '&'; }
            if (this.primengTableHelper.getMaxResultCount(this.paginator, event) !== undefined) { url += 'MaxResultCount=' + encodeURIComponent('' + this.primengTableHelper.getMaxResultCount(this.paginator, event)) + '&'; }

            this.primengTableHelper.showLoadingIndicator();
            this.__proxy.request(url, RequestType.Get)
                .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
                .subscribe(result => {
                    this.primengTableHelper.columns = result.columns;
                    this.primengTableHelper.totalRecordsCount = result.totalCount;
                    this.primengTableHelper.records = result.items;
                    this.primengTableHelper.hideLoadingIndicator();
                    //this.onLoadFinished.emit();
                });
        }
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    save(): void {

        this.inputHelper2.BizRegID = this.inputHelper.BizRegID;
        this.inputHelper2.CompanyName = this.inputHelper.CompanyName;
        this.inputHelper2.InitName = this.inputHelper.InitName;
        this.inputHelper2.CompanyType = this.inputHelper.CompanyType;
        this.inputHelper2.RegDate = this.inputHelper.RegDate;
        this.inputHelper2.FileNo = this.inputHelper.FileNo;
        this.inputHelper2.RefNo1 = this.inputHelper.RefNo1;
        this.inputHelper2.RefNo2 = this.inputHelper.RefNo2;
        this.inputHelper2.Owner = this.inputHelper.Owner;
        this.inputHelper2.ICNo = this.inputHelper.ICNo;
        this.inputHelper2.SubGrp = this.inputHelper.SubGrp;
        this.inputHelper2.BizGrp = this.inputHelper.BizGrp;
        this.inputHelper2.BizCatgID = this.inputHelper.BizCatgID;
        this.inputHelper2.BizRegType = this.inputHelper.BizRegType;
        this.inputHelper2.BizRegStatus = this.inputHelper.BizRegStatus;
        this.inputHelper2.CommID = this.inputHelper.CommID;
        this.inputHelper2.IndustryType = this.inputHelper.IndustryType;
        this.inputHelper2.BusinessType = this.inputHelper.BusinessType;
        this.inputHelper2.RegNo = this.inputHelper.RegNo;
        this.inputHelper2.StoreStatus = this.inputHelper.StoreStatus;
        this.inputHelper2.PrivilegeCode = this.inputHelper.PrivilegeCode;
        this.inputHelper2.Address1 = this.inputHelper.Address1;
        this.inputHelper2.Address2 = this.inputHelper.Address2;
        this.inputHelper2.Address3 = this.inputHelper.Address3;
        this.inputHelper2.Address4 = this.inputHelper.Address4;
        this.inputHelper2.WCAddress1 = this.inputHelper.WCAddress1;
        this.inputHelper2.WCAddress2 = this.inputHelper.WCAddress2;
        this.inputHelper2.WCAddress3 = this.inputHelper.WCAddress3;
        this.inputHelper2.WCAddress4 = this.inputHelper.WCAddress4;
        this.inputHelper2.PostalCode = this.inputHelper.PostalCode;
        this.inputHelper2.Country = this.selectedCountry.Code !== undefined ? this.selectedCountry.Code : '';
        this.inputHelper2.State = this.selectedState.Code !== undefined ? this.selectedState.Code : '';
        this.inputHelper2.PBT = this.inputHelper.PBT;
        this.inputHelper2.City = this.selectedDistrict.Code !== undefined ? this.selectedDistrict.Code : '';
        //this.inputHelper2.Area = this.selectedTown.Code;
        this.inputHelper2.TelNo = this.inputHelper.TelNo;
        this.inputHelper2.FaxNo = this.inputHelper.FaxNo;
        this.inputHelper2.Email = this.inputHelper.Email;
        this.inputHelper2.CoWebsite = this.inputHelper.CoWebsite;
        this.inputHelper2.ContactPerson = this.inputHelper.ContactPerson;
        this.inputHelper2.ContactRace = this.inputHelper.ContactRace;
        this.inputHelper2.ContactDesignation = this.inputHelper.ContactDesignation;
        this.inputHelper2.ContactPersonEmail = this.inputHelper.ContactPersonEmail;
        this.inputHelper2.ContactPersonTelNo = this.inputHelper.ContactPersonTelNo;
        this.inputHelper2.ContactPersonFaxNo = this.inputHelper.ContactPersonFaxNo;
        this.inputHelper2.ContactPersonMobile = this.inputHelper.ContactPersonMobile;
        this.inputHelper2.ContactPerson2 = this.inputHelper.ContactPerson2;
        this.inputHelper2.ContactDesignation2 = this.inputHelper.ContactDesignation2;
        this.inputHelper2.ContactPersonEmail2 = this.inputHelper.ContactPersonEmail2;
        this.inputHelper2.ContactPersonTelNo2 = this.inputHelper.ContactPersonTelNo2;
        this.inputHelper2.ContactPersonFaxNo2 = this.inputHelper.ContactPersonFaxNo2;
        this.inputHelper2.ContactPersonMobile2 = this.inputHelper.ContactPersonMobile2;
        this.inputHelper2.SMSAlertNo = this.inputHelper.SMSAlertNo;
        this.inputHelper2.AcctNo = this.inputHelper.AcctNo;
        this.inputHelper2.AgrNo = this.inputHelper.AgrNo;
        this.inputHelper2.AgrDate = this.inputHelper.AgrDate;
        this.inputHelper2.AgrAddDate = this.inputHelper.AgrAddDate;
        this.inputHelper2.AgrRenew = this.inputHelper.AgrRenew;
        this.inputHelper2.AgrRenewDate = this.inputHelper.AgrRenewDate;
        this.inputHelper2.AcctNo2 = this.inputHelper.AcctNo2;
        this.inputHelper2.AgrNo2 = this.inputHelper.AgrNo2;
        this.inputHelper2.AgrDate2 = this.inputHelper.AgrDate2;
        this.inputHelper2.AgrAddDate2 = this.inputHelper.AgrAddDate2;
        this.inputHelper2.AgrRenew2 = this.inputHelper.AgrRenew2;
        this.inputHelper2.AgrRenewDate2 = this.inputHelper.AgrRenewDate2;
        this.inputHelper2.BankCode1 = this.inputHelper.BankCode1;
        this.inputHelper2.BankAccount1 = this.inputHelper.BankAccount1;
        this.inputHelper2.BankCode2 = this.inputHelper.BankCode2;
        this.inputHelper2.BankAccount2 = this.inputHelper.BankAccount2;
        this.inputHelper2.Remark = this.inputHelper.Remark;
        this.inputHelper2.Remark2 = this.inputHelper.Remark2;
        this.inputHelper2.ReqSupp = this.inputHelper.ReqSupp;
        this.inputHelper2.ReqNo = this.inputHelper.ReqNo;
        this.inputHelper2.ReqDate = this.inputHelper.ReqDate;
        this.inputHelper2.ReqCode = this.inputHelper.ReqCode;
        this.inputHelper2.RefAmount1 = this.inputHelper.RefAmount1;
        this.inputHelper2.RefAmount2 = this.inputHelper.RefAmount2;
        this.inputHelper2.IsTermAgr = this.inputHelper.IsTermAgr;
        this.inputHelper2.IsRateAgr = this.inputHelper.IsRateAgr;
        this.inputHelper2.ImagePath = this.inputHelper.ImagePath;
        this.inputHelper2.MediaUrl1 = this.inputHelper.MediaUrl1;
        this.inputHelper2.MediaUrl2 = this.inputHelper.MediaUrl2;
        this.inputHelper2.MediaUrl3 = this.inputHelper.MediaUrl3;
        this.inputHelper2.Active = this.inputHelper.Active;
        this.inputHelper2.Inuse = this.inputHelper.Inuse;
        this.inputHelper2.Flag = this.inputHelper.Flag;
        this.inputHelper2.ApprovedDate = this.inputHelper.ApprovedDate;
        this.inputHelper2.ApprovedBy = this.inputHelper.ApprovedBy;
        this.inputHelper2.CreateDate = this.inputHelper.CreateDate;
        this.inputHelper2.CreateBy = this.inputHelper.CreateBy;
        this.inputHelper2.LastUpdate = this.inputHelper.LastUpdate;
        this.inputHelper2.UpdateBy = this.inputHelper.UpdateBy;
        this.inputHelper2.SyncCreate = this.inputHelper.SyncCreate;
        this.inputHelper2.SyncLastUpd = this.inputHelper.SyncLastUpd;
        this.inputHelper2.LastSyncBy = this.inputHelper.LastSyncBy;
        this.inputHelper2.IsHost = this.inputHelper.IsHost;
        this.inputHelper2.IsNew = this.inputHelper.IsNew;
        this.inputHelper2.Status = this.inputHelper.Status;
        this.inputHelper2.BizType = this.inputHelper.BizType;
        this.inputHelper2.TID = this.inputHelper.TID;
        this.inputHelper2.RefID = this.inputHelper.RefID;

        //data.(this.inputHelper2);
        //console.log('data : ' + JSON.stringify(data));
        let url = ProxyURL.UpdateCompany;
        let notif = '';

        this.__proxy.request(url, RequestType.Post, this.inputHelper2)
            .subscribe((result) => {
                if (result.success = 'true') {
                    this.notify.success(this.l('EditCompanySuccess'));
                    this.modalSave.emit();
                    this.close();
                    this.getCompany();
                } else {
                    this.notify.error(this.l('EditCompanyFailed'));
                }
            });

    }

    show(data: any): void {
        this.modal.show();
        this.inputHelper.BizRegID = data.BizRegID;
        this.inputHelper.BizLocID = data.BizLocID;
        this.inputHelper.CompanyName = data.CompanyName;
        this.inputHelper.AcctNo = data.AcctNo;
        this.inputHelper.Address = data.Address1 + ' ' + data.Address2 + ' ' + data.Address3 + ' ' + data.Address4;
        this.inputHelper.Address1 = data.Address1;
        this.inputHelper.Address2 = data.Address2;
        this.inputHelper.Address3 = data.Address3;
        this.inputHelper.Address4 = data.Address4;
        this.inputHelper.Telno = data.TelNo;
        this.inputHelper.Faxno = data.FaxNo;
        this.inputHelper.Email = data.Email;
        this.inputHelper.PostalCode = data.PostalCode;
        this.inputHelper.Country = data.Country;
        this.inputHelper.State = data.State;
        this.inputHelper.PBT = data.PBT;
        this.inputHelper.ContactPerson = data.ContactPerson;
        this.inputHelper.ContactPerson2 = data.ContactPerson2;
        this.inputHelper.ContactDesignation = data.ContactDesignation;
        this.inputHelper.Designation = data.Designation;
        this.inputHelper.RegNo = data.RegNo;
        this.inputHelper.ContactPersonTelNo = data.ContactPersonTelNo;
        this.inputHelper.CountryDesc = data.CountryDesc;
        //this.inputHelper.PostalCode = data.PostalCode;
        this.inputHelper.CityDesc = data.CityDesc;
        this.inputHelper.StateDesc = data.StateDesc;
        this.statusdesc = data.StatusDesc;
        this.inputHelper.RefNo1 = data.RefNo1;
    }

    close(): void {
        this.active = false;
        this.modal.hide();
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
            this.__proxy.request(stateUrl, RequestType.Get)
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
            this.__proxy.request(cityUrl, RequestType.Get)
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
            this.__proxy.request(areaUrl, RequestType.Get)
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
}
