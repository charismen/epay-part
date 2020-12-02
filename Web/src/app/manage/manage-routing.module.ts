import { WorkingAccountComponent } from './workingaccount/workingaccount.component';
import { EmployeeComponent } from './employee/employee-list.component';
import { CompanyProfileComponent } from './company-profile/company-profile.component';
import { MaintenanceOfficerComponent } from './maintenance-officer/maintenance-officer.component';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CustomerComponent } from './customer-list/customer-list.component';
import { CreateOrEditCustomerModalComponent } from './customer-list/create-edit-customer-list.component'
import { TenderListComponent } from './tender-list/tender-list.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                children: [
                    { path: 'customerlist', component: CustomerComponent, data: { permission: 'Pages.CustomerList' } },
                    { path: 'employee', component: EmployeeComponent, data: { permission: 'Pages.EmployeeList' } },
                    { path: 'company-profile', component: CompanyProfileComponent, data: { } },
                    { path: 'profilecompany/:id', component: CreateOrEditCustomerModalComponent, data: {} },
                    { path: 'officer-list', component: MaintenanceOfficerComponent, data: { permission: 'Pages.Maintenance' } },
                    { path: 'workingaccount-list', component: WorkingAccountComponent, data: { permission: 'Pages.Maintenance' } },
                    { path: 'tender-list', component: TenderListComponent, data: { permission: 'Pages.Inventory.Tender'} }
                ]
            }
        ])
    ],
    exports: [
        RouterModule
    ]
})
export class ManageRoutingModule { }
