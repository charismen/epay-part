import { Component, Injector, OnInit, ElementRef, ViewChild, AfterViewInit, Input, Output, EventEmitter, AfterContentChecked, ChangeDetectorRef } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as _ from 'lodash';
import { ActivatedRoute, Router } from '@angular/router';
// import { LocationModalComponent } from './location-modal.component';
import { finalize } from 'rxjs/operators';
import { GenericServiceProxy, RequestType } from '@shared/service-proxies/generic-service-proxies';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin from '@fullcalendar/interaction';
import * as moment from 'moment';;
import { FullCalendar } from 'primeng/fullcalendar';
import { FullCalendarComponent } from '@fullcalendar/angular';
import { LazyLoadEvent } from 'primeng/api';
declare var $: any;

@Component({
    templateUrl: './calendar.component.html',
    styleUrls: ['./calendar.component.less'],
    selector: 'calendar',
    animations: [appModuleAnimation()]
})
export class CalendarComponent extends AppComponentBase implements OnInit, AfterViewInit, AfterContentChecked  {
    @ViewChild('calendar', { static: true }) calendar: FullCalendar;
    @ViewChild('calendar', { static: true }) calendarComponent: FullCalendarComponent;

    @Input() defaultView = 'dayGridMonth';
    @Input() defaultDate = moment(new Date()).format('YYYY-MM-DD');
    @Input() events: any = [];
    @Input() editable = true;
    @Input() droppable = true;
    @Input() draggable = true;
    @Input() weekends = true;
    @Input() showNonCurrentDates = false;
    @Input() allDaySlot = true;
    @Input() slotEventOverlap = false;
    @Input() header = {
        left: 'prev,next today',
        center: 'title',
        right: 'dayGridMonth,timeGridWeek,timeGridDay'
    };

    @Output() onEventRender: EventEmitter<any> = new EventEmitter<any>();
    @Output() onViewChange: EventEmitter<any> = new EventEmitter<any>();
    @Output() onDateClick: EventEmitter<string> = new EventEmitter<string>();
    @Output() onEventClick: EventEmitter<any> = new EventEmitter<any>();
    @Output() onEventDrop: EventEmitter<any> = new EventEmitter<any>();
    options: any;
    nextListener: any;
    prevListener: any;

    constructor(
        injector: Injector,
        private __cdref: ChangeDetectorRef,
        private _activatedRoute: ActivatedRoute,
        private _route: Router,
        private __proxy: GenericServiceProxy,
    ) {
        super(injector);
    }

    init() {
        this.options = {
            plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin],
            defaultDate: this.defaultDate,
            defaultView: this.defaultView,
            editable: this.editable,
            draggable: this.draggable,
            droppable: this.droppable,
            weekends: this.weekends,
            showNonCurrentDates: this.showNonCurrentDates,
            allDaySlot: this.allDaySlot,
            slotEventOverlap: this.slotEventOverlap,
            header: this.header,
            eventRender: (e) => {
                this.onEventRender.emit(e);
            },
            datesRender : (e) => {
                this.datesRender(e);
            },
            dateClick: (e) => {
                this.handleDateClick(e);
            },
            eventClick: (e) => {
                this.handleEventClick(e);
            },
            eventDrop: (e) => {
                this.eventDrop(e);
            },
            drop: (e) => {
                this.eventDrop(e);
            },
            viewRender: (e) => {
                this.viewRender(e);
            },
            viewSkeletonRender: (e) => {
                this.viewSkeletonRender(e);
            },
        };
    }

    refresh(url: string) {
        if (url !== undefined) {
            this.__proxy.request(url, RequestType.Get)
                .subscribe(result => {
                    this.events = result;
                });
        }
    }

    ngOnInit(): void {
        this.init();
    }

    ngAfterViewInit() {
    }

    ngAfterContentChecked(): void {
        this.__cdref.detectChanges();
    }

    datesRender(event: any) {
        this.onViewChange.emit(this.calendar.getCalendar().getDate());
    }

    handleDateClick(event: any) {
        this.onDateClick.emit(event.dateStr);
    }

    handleEventClick(event: any) {
        this.onEventClick.emit(event.event.id);
    }

    eventDrop(event: any) {
        let droppedDate = {
            id: event.event.id,
            start: event.event.start,
            end: event.event.end
        };
        this.onEventDrop.emit(droppedDate);
    }

    viewRender(event: any) {
    }

    viewSkeletonRender(event: any) {
    }
}
