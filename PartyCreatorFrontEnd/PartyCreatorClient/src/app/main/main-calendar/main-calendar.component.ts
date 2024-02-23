import { Component, OnInit, Input, SimpleChanges } from '@angular/core';
import { Subject, forkJoin } from 'rxjs';
import { isSameDay, isSameMonth } from 'date-fns';
import { Router } from '@angular/router';
import {
  CalendarEvent,
  CalendarModule,
  CalendarView,
  DAYS_OF_WEEK,
  CalendarEventTitleFormatter,
} from 'angular-calendar';
import { EventService } from '../../services/event.service';
import { CommonModule } from '@angular/common';
import { CustomEventTitleFormatter } from './custom-event-title-formatter.provider';
import { EventUserDto } from 'src/app/interfaces/event-user-dto';

@Component({
  selector: 'app-main-calendar',
  templateUrl: './main-calendar.component.html',
  styleUrls: ['./main-calendar.component.css'],
  standalone: true,
  imports: [CalendarModule, CommonModule],
  providers: [
    {
      provide: CalendarEventTitleFormatter,
      useClass: CustomEventTitleFormatter,
    },
  ],
})
export class MainCalendarComponent implements OnInit {
  @Input() creatorEvents: EventUserDto[] = [];
  @Input() upcomingEvents: EventUserDto[] = [];
  @Input() finishedEvents: EventUserDto[] = [];
  view: CalendarView = CalendarView.Month;
  viewDate: Date = new Date();
  calendarEvents: CalendarEvent[] = [];
  refresh = new Subject<void>();
  weekStartsOn: number = DAYS_OF_WEEK.MONDAY;
  CalendarView = CalendarView;

  constructor(private event: EventService, private router: Router) {}

  myEvents: EventUserDto[] = [];

  ngOnInit(): void {}

  ngOnChanges(changes: SimpleChanges): void {
    if (
      changes['creatorEvents'] ||
      changes['upcomingEvents'] ||
      changes['finishedEvents']
    ) {
      this.getMyEvents();
    }
  }

  getMyEvents() {
    this.myEvents = [
      ...this.creatorEvents,
      ...this.upcomingEvents,
      ...this.finishedEvents,
    ];
    this.calendarEvents = this.myEvents.map((event) => {
      return {
        start: new Date(event.dateTime),
        title: event.title,
        id: event.id,
      };
    });
  }
  activeDayIsOpen: boolean = false;

  dayClicked({ date, events }: { date: Date; events: CalendarEvent[] }): void {
    if (isSameMonth(date, this.viewDate)) {
      if (
        (isSameDay(this.viewDate, date) && this.activeDayIsOpen === true) ||
        events.length === 0
      ) {
        this.activeDayIsOpen = false;
      } else {
        this.activeDayIsOpen = true;
      }
      this.viewDate = date;
    }
  }

  closeOpenMonthViewDay() {
    this.activeDayIsOpen = false;
  }

  handleEvent(action: string, event: CalendarEvent): void {
    this.router.navigate([`wydarzenie/${event.id}`]);
  }
}
