import { ReportComponent } from './report/report.component';
import { CashbillComponent } from './cashbill/cashbill.component';
import { PaymentSummaryModalComponent } from './invoice/payment-summary-modal.component';
import { DirectResponseComponent } from './direct-response/direct-response.component';
import { QuickPaymentComponent } from './invoice/quickpayment.component';
import { NgModule } from '@angular/core';
import { NavigationEnd, Router, RouterModule } from '@angular/router';
import { InvoiceComponent } from './invoice/invoice.component';
import { InvoiceListingComponent } from './invoice-listing/invoice-listing.component';
import { TransactionHistoryComponent } from './transaction-history/transaction-history.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                children: [
                    { path: 'invoice', component: InvoiceComponent, data: { permission: 'Pages.Account.Invoice.Receipt' } },
                    { path: 'invoice-listing', component: InvoiceListingComponent, data: { permission: 'Pages.Account.Invoice.Payment' } },
                    { path: 'payment-summary-response', component: PaymentSummaryModalComponent, data: { }},
                    { path: 'payment-history', component: TransactionHistoryComponent, data: { permission: 'Pages.Account.Payment.History' } },
                    { path: 'cashbill', component: CashbillComponent, data: { permission: 'Pages.Account.CashBill' } },
                    { path: 'quickpayment', component: QuickPaymentComponent, data: { permission: 'Pages.Account.Invoice' } },
                    { path: 'directresponse', component: DirectResponseComponent },
                    { path: 'report', component: ReportComponent }
                ]
            }
        ])
    ],
    exports: [
        RouterModule
    ]
})
export class TransactionRoutingModule {
    constructor(
        private router: Router
    ) {
        router.events.subscribe((event) => {
            if (event instanceof NavigationEnd) {
                window.scroll(0, 0);
            }
        });
    }
}
