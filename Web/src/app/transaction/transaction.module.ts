import { PopoverModule } from 'ngx-bootstrap/popover';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { ReportComponent } from './report/report.component';
import { CreateEditCashBillModalComponent } from './cashbill/create-edit-cashbill-modal.component';
import { CashbillComponent } from './cashbill/cashbill.component';
import { DirectResponseComponent } from './direct-response/direct-response.component';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AppCommonModule } from '@app/shared/common/app-common.module';
import { UtilsModule } from '@shared/utils/utils.module';
import { CountoModule } from 'angular2-counto';
import { ModalModule } from 'ngx-bootstrap/modal';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { TableModule } from 'primeng/table';
import { BsDatepickerModule, BsDatepickerConfig, BsDaterangepickerConfig, BsLocaleService } from 'ngx-bootstrap/datepicker';
import { NgxBootstrapDatePickerConfigService } from 'assets/ngx-bootstrap/ngx-bootstrap-datepicker-config.service';
import { AppFormModule } from '@app/shared/form/app-form.module';
import { TransactionRoutingModule } from './transaction-routing.module';
import { InvoiceComponent } from './invoice/invoice.component';
import { InvoiceListingComponent } from './invoice-listing/invoice-listing.component';
import { QuickPaymentComponent } from './invoice/quickpayment.component';
import { TransactionHistoryComponent } from './transaction-history/transaction-history.component';
import { InvoiceCont, InvoiceFilter } from '@shared/AppEnums';
import { OfflinePaymentModalComponent } from './invoice/offline-payment.component';
import { DetailInvoiceModalComponent } from './invoice/invoice-details-modal.component';
import { PaymentSummaryModalComponent } from './invoice/payment-summary-modal.component';
import { AppBsModalModule } from '@shared/common/appBsModal/app-bs-modal.module';

NgxBootstrapDatePickerConfigService.registerNgxBootstrapDatePickerLocales();

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ModalModule.forRoot(),
        AppBsModalModule,
        TabsModule,
        TooltipModule,
        AppCommonModule,
        UtilsModule,
        TransactionRoutingModule ,
        CountoModule,
        AppFormModule,
        TableModule,
        NgxChartsModule,
        BsDatepickerModule.forRoot(),
        BsDropdownModule.forRoot(),
        PopoverModule.forRoot()
    ],
    declarations: [
        InvoiceComponent,
        InvoiceListingComponent,
        QuickPaymentComponent,
        PaymentSummaryModalComponent,
        TransactionHistoryComponent,
        DirectResponseComponent,
        OfflinePaymentModalComponent,
        DetailInvoiceModalComponent,
        CashbillComponent,
        CreateEditCashBillModalComponent,
        ReportComponent
    ],
    providers: [
        { provide: BsDatepickerConfig, useFactory: NgxBootstrapDatePickerConfigService.getDatepickerConfig },
        { provide: BsDaterangepickerConfig, useFactory: NgxBootstrapDatePickerConfigService.getDaterangepickerConfig },
        { provide: BsLocaleService, useFactory: NgxBootstrapDatePickerConfigService.getDatepickerLocale },
        InvoiceCont,
        InvoiceFilter
    ]
})
export class TransactionModule { }
