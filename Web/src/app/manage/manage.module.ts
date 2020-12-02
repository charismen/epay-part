import { PopoverModule } from 'ngx-bootstrap/popover';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { CustomerDetailComponent } from './customer-list/customer-detail.component';
import { CompanyProfileComponent } from './company-profile/company-profile.component';
import { EmployeeComponent } from './employee/employee-list.component';
import { MaintenanceOfficerComponent } from './maintenance-officer/maintenance-officer.component';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AppCommonModule } from '@app/shared/common/app-common.module';
import { UtilsModule } from '@shared/utils/utils.module';
import { CountoModule } from 'angular2-counto';
import { PaginatorModule } from 'primeng/paginator';
import { ModalModule } from 'ngx-bootstrap/modal';
import { ManageRoutingModule } from './manage-routing.module';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { TableModule } from 'primeng/table';
import { TimepickerModule } from 'ngx-bootstrap/timepicker';
import { BsDatepickerModule, BsDatepickerConfig, BsDaterangepickerConfig, BsLocaleService } from 'ngx-bootstrap/datepicker';
import { NgxBootstrapDatePickerConfigService } from 'assets/ngx-bootstrap/ngx-bootstrap-datepicker-config.service';
import { AppFormModule } from '@app/shared/form/app-form.module';
import { CustomerComponent } from './customer-list/customer-list.component';
import { CreateOrEditCustomerModalComponent } from './customer-list/create-edit-customer-list.component';
import { CreateOrEditEmployeeModalComponent } from './employee/create-edit-employee-list.component';
import { BizRegBizLoc } from '@shared/AppEnums';
import { EditCustomerModalComponent } from './customer-list/edit-customer-modal.component';
import { TenderListComponent } from './tender-list/tender-list.component';
import { CreateEditTenderComponent } from './tender-list/create-edit-tender-list.component';
import { AppBsModalModule } from '@shared/common/appBsModal/app-bs-modal.module';
import { WorkingAccountComponent } from './workingaccount/workingaccount.component';

NgxBootstrapDatePickerConfigService.registerNgxBootstrapDatePickerLocales();

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ModalModule.forRoot(),
        TabsModule,
        TableModule,
        TooltipModule,
        AppCommonModule,
        AppFormModule,
        UtilsModule,
        PaginatorModule,
        ManageRoutingModule,
        CountoModule,
        NgxChartsModule,
        AppBsModalModule,
        TimepickerModule.forRoot(),
        BsDatepickerModule.forRoot(),
        BsDropdownModule.forRoot(),
        PopoverModule.forRoot()
    ],
    declarations: [
        CustomerComponent,
        CreateOrEditCustomerModalComponent,
        CustomerDetailComponent,
        EditCustomerModalComponent,
        CreateEditTenderComponent,
        CompanyProfileComponent,
        TenderListComponent,
        EmployeeComponent,
        CreateOrEditEmployeeModalComponent,
        MaintenanceOfficerComponent,
        WorkingAccountComponent
    ],
    providers: [
        { provide: BsDatepickerConfig, useFactory: NgxBootstrapDatePickerConfigService.getDatepickerConfig },
        { provide: BsDaterangepickerConfig, useFactory: NgxBootstrapDatePickerConfigService.getDaterangepickerConfig },
        { provide: BsLocaleService, useFactory: NgxBootstrapDatePickerConfigService.getDatepickerLocale },
        BizRegBizLoc
    ]
})
export class ManageModule { }
