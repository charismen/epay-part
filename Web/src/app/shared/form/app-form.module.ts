import { ModuleWithProviders, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppLocalizationService } from '@app/shared/common/localization/app-localization.service';
import { AppNavigationService } from '@app/shared/layout/nav/app-navigation.service';
import { PlexformCommonModule } from '@shared/common/common.module';
import { UtilsModule } from '@shared/utils/utils.module';
import { ModalModule } from 'ngx-bootstrap/modal';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { PopoverModule } from 'ngx-bootstrap/popover';
import { BsDatepickerModule, BsDatepickerConfig, BsDaterangepickerConfig, BsLocaleService } from 'ngx-bootstrap/datepicker';
import { NgxBootstrapDatePickerConfigService } from 'assets/ngx-bootstrap/ngx-bootstrap-datepicker-config.service';
import { PaginatorModule } from 'primeng/paginator';
import { TableModule } from 'primeng/table';
import { FileUploadModule } from 'ng2-file-upload';
import { AppCommonModule } from '../common/app-common.module';
import { ContextMenuModule } from 'primeng/contextmenu';
import { DragDropModule } from 'primeng/dragdrop';
import { EditorModule } from 'primeng/editor';
import { InputMaskModule } from 'primeng/inputmask';
import { TextMaskModule } from 'angular2-text-mask';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { CountoModule } from 'angular2-counto';
import { ImageCropperModule } from 'ngx-image-cropper';
import { FileUploadModule as PrimeNgFileUploadModule } from 'primeng/fileupload';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { DateTimeService } from '../common/timing/date-time.service';
import { AppRouteGuard } from '../common/auth/auth-route-guard';
import { AppAuthService } from '../common/auth/app-auth.service';
import { AppBsModalModule } from '@shared/common/appBsModal/app-bs-modal.module';
import { BaseListComponent } from './list/base-list.component';
import { GalleryComponent } from './gallery/gallery.component';
import { RealTimeService } from './real-time/real-time.service';
import { CalendarComponent } from './calendar/calendar.component';
import { FullCalendarModule } from 'primeng/fullcalendar';
import { NgSelectModule } from '@ng-select/ng-select';
import { FileUploadComponent } from './file-upload/file-upload.component';
import { AppStorage } from '@app/shared/form/storage/app-storage.component';
import { ViewContainerComponent } from './container/view-container.component';
import { NgxFileDropModule } from 'ngx-file-drop';
import { CountdownModule, CountdownGlobalConfig } from 'ngx-countdown';
import { HistoryComponent } from './history/history.component';
import { HistoryDetailModalComponent } from './history/history-detail-modal.component';
import { FieldUploadComponent } from './file-upload/field-upload.component';
import { QuestionnaireFieldComponent } from './questionnaire/questionnaire-field.component';
import { CardListComponent } from './card/card-list.component';
import { TimelineComponent } from './timeline/timeline.component';
import { NgOtpInputModule } from 'ng-otp-input';

// Metronic
import { PerfectScrollbarModule, PERFECT_SCROLLBAR_CONFIG, PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';
const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
    // suppressScrollX: true
};

export function countdownConfigFactory(): CountdownGlobalConfig {
    return new CountdownGlobalConfig(abp.localization.currentLanguage.name);
}

@NgModule({
    imports: [
        FormsModule,
        ReactiveFormsModule,
        PlexformCommonModule,
        FileUploadModule,
        ModalModule.forRoot(),
        TabsModule.forRoot(),
        TooltipModule.forRoot(),
        PopoverModule.forRoot(),
        BsDropdownModule.forRoot(),
        BsDatepickerModule.forRoot(),
        CountdownModule,
        PerfectScrollbarModule,
        NgOtpInputModule,
        UtilsModule,
        AppCommonModule,
        TableModule,
        DragDropModule,
        ContextMenuModule,
        PaginatorModule,
        PrimeNgFileUploadModule,
        AutoCompleteModule,
        EditorModule,
        InputMaskModule,
        NgxChartsModule,
        CountoModule,
        TextMaskModule,
        ImageCropperModule,
        FullCalendarModule,
        NgSelectModule,
        NgxFileDropModule,
        AppBsModalModule
    ],
    declarations: [
        BaseListComponent,
        CardListComponent,
        TimelineComponent,
        GalleryComponent,
        CalendarComponent,
        HistoryComponent,
        HistoryDetailModalComponent,
        FileUploadComponent,
        FieldUploadComponent,
        QuestionnaireFieldComponent,
        ViewContainerComponent
    ],
    exports: [
        BaseListComponent,
        CardListComponent,
        TimelineComponent,
        GalleryComponent,
        CalendarComponent,
        HistoryComponent,
        HistoryDetailModalComponent,
        NgSelectModule,
        FileUploadComponent,
        FieldUploadComponent,
        QuestionnaireFieldComponent,
        CountdownModule,
        PerfectScrollbarModule,
        NgOtpInputModule,
        ViewContainerComponent
    ],
    providers: [
        DateTimeService,
        AppLocalizationService,
        AppNavigationService,
        RealTimeService,
        AppStorage,
        { provide: BsDatepickerConfig, useFactory: NgxBootstrapDatePickerConfigService.getDatepickerConfig },
        { provide: BsDaterangepickerConfig, useFactory: NgxBootstrapDatePickerConfigService.getDaterangepickerConfig },
        { provide: BsLocaleService, useFactory: NgxBootstrapDatePickerConfigService.getDatepickerLocale },
        { provide: CountdownGlobalConfig, useFactory: countdownConfigFactory },
        { provide: PERFECT_SCROLLBAR_CONFIG, useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG }
    ]
})
export class AppFormModule {
    static forRoot(): ModuleWithProviders {
        return {
            ngModule: AppFormModule,
            providers: [
                AppAuthService,
                AppRouteGuard
            ]
        };
    }
}
