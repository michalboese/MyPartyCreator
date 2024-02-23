import { Component, OnInit } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { NgToastService } from 'ng-angular-popup';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatBadgeModule } from '@angular/material/badge';
import { UserSearchComponent } from './user-search/user-search.component';
import { MatTooltipModule } from '@angular/material/tooltip';

//icons
import { faBell, faX } from '@fortawesome/free-solid-svg-icons';
import { AuthService } from '../services/auth.service';
import { NotificationService } from '../services/notification.service';
import { NotificationDto } from '../interfaces/notification-dto';
import { EventService } from '../services/event.service';
import { SignalRService } from '../services/signal-r.service';

@Component({
  selector: 'app-nav-menu-main',
  templateUrl: './nav-menu-main.component.html',
  styleUrls: ['./nav-menu-main.component.scss'],
  standalone: true,
  imports: [
    RouterModule,
    CommonModule,
    MatMenuModule,
    MatIconModule,
    MatButtonModule,
    MatBadgeModule,
    UserSearchComponent,
    MatTooltipModule,
  ],
})
export class NavMenuMainComponent implements OnInit {
  faBell = faBell;
  faX = faX;
  isExpanded = false;
  isNotificationVisible: boolean = false;
  notifications: NotificationDto[] = [];
  counter = 0;
  public isLightTheme = false;

  constructor(
    public auth: AuthService,
    private notificationService: NotificationService,
    private eventService: EventService,
    private toast: NgToastService,
    private router: Router,
    private signalRService: SignalRService
  ) {}

  ngOnInit(): void {
    if (this.auth.isLoggedIn()) {
      this.getNotifications();

      this.signalRService.hubConnection.on(
        'ReceiveNotification',
        (notification: NotificationDto) => {
          console.log(notification);
          this.notifications.unshift(notification);
        }
      );
    }

    window.addEventListener('signIn', (e) => this.ngOnInit());
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  logOut() {
    this.auth.signOut();
  }

  changeLoginType() {
    this.auth.changeLoginType('signin');
  }

  toggleNotifications() {
    this.isNotificationVisible = !this.isNotificationVisible;
  }

  doSomething($event: any) {
    $event.stopPropagation();
    //Another instructions
  }

  getNotifications() {
    this.notificationService.getAllOfUser().subscribe({
      next: (res) => {
        this.notifications = res.reverse();
      },
      error: (err: HttpErrorResponse) => {
        this.toast.error({
          detail: 'ERROR',
          summary: err.error,
          duration: 3000,
        });
      },
    });
  }

  acceptInvite(invite: NotificationDto) {
    this.eventService.acceptInvite(invite).subscribe({
      next: (res) => {
        this.notifications.splice(this.notifications.indexOf(invite), 1);
        this.router.navigate([`wydarzenie/${invite.eventId}`]);
        this.toast.success({
          detail: 'SUCCESS',
          summary: 'Udało się dołączyć do wydarzenia',
          duration: 3000,
        });
      },
      error: (err: HttpErrorResponse) => {
        this.toast.error({
          detail: 'ERROR',
          summary: err.error,
          duration: 3000,
        });
      },
    });
  }

  declineInvite(invite: NotificationDto) {
    this.eventService.declineInvite(invite).subscribe({
      next: (res) => {
        this.notifications.splice(this.notifications.indexOf(invite), 1);
        this.toast.success({
          detail: 'SUCCESS',
          summary: 'Odrzuciłeś zaproszenie do wydarzenia',
          duration: 3000,
        });
      },
      error: (err: HttpErrorResponse) => {
        this.toast.error({
          detail: 'ERROR',
          summary: err.error,
          duration: 3000,
        });
      },
    });
  }
  toggleRead(notification: NotificationDto) {
    if (!notification.isRead) {
      this.notificationService.toggleRead(notification.id).subscribe({
        next: () => {
          this.notifications[this.notifications.indexOf(notification)].isRead =
            true;
          this.countRead();
        },
        error: (err: HttpErrorResponse) => {
          this.toast.error({
            detail: 'ERROR',
            summary: err.error,
            duration: 3000,
          });
        },
      });
    }
  }
  countRead() {
    this.counter = 0;
    this.notifications.forEach((item) => {
      if (!item.isRead) {
        this.counter++;
      }
    });
    return this.counter;
  }

  deleteNotification(id: number) {
    this.notificationService.delete(id.toString()).subscribe({
      next: (res) => {
        this.notifications.splice(
          this.notifications.findIndex((obj) => obj.id === id),
          1
        );
        console.log(res);
        this.toast.success({
          detail: 'SUCCESS',
          summary: 'Usunąłeś powiadomienie',
          duration: 3000,
        });
      },
      error: (err: HttpErrorResponse) => {
        this.toast.error({
          detail: 'ERROR',
          summary: err.error,
          duration: 3000,
        });
      },
    });
  }

  onThemeSwitchChange() {
    this.isLightTheme = !this.isLightTheme;

    document.body.setAttribute(
      'data-theme',
      this.isLightTheme ? 'light' : 'dark'
    );
  }
}
