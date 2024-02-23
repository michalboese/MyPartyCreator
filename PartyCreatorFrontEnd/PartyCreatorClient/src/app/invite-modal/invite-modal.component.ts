import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { AllGuestsListDto } from '../interfaces/all-guests-list-dto';
import { EventService } from '../services/event.service';
import { NgToastService } from 'ng-angular-popup';
import { FormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatTabsModule } from '@angular/material/tabs';
import { CommonModule } from '@angular/common';
import { UserContactDto } from '../interfaces/user-contact-dto';
import { UserService } from '../services/user.service';
import { HttpErrorResponse } from '@angular/common/http';
import { SearchEmailDto } from '../interfaces/search-email-dto';
import { InviteListDto } from '../interfaces/invite-list-dto';
import { ContactDto } from '../interfaces/contact-dto';
import { ContactEventDto } from '../interfaces/contact-event-dto';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-invite-modal',
  templateUrl: './invite-modal.component.html',
  styleUrls: ['./invite-modal.component.css'],
  standalone: true,
  imports: [
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatTabsModule,
    CommonModule,
    MatTooltipModule,
  ],
})
export class InviteModalComponent implements OnInit {
  users: UserContactDto[] = [];
  invitedUsers: UserContactDto[] = [];
  contacts: ContactDto[] = [];
  userSearch = this.fb.group({
    search: '',
  });

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public data: {
      eventId: string;
      guests: AllGuestsListDto[];
      invites: AllGuestsListDto[];
    },
    private eventService: EventService,
    private toast: NgToastService,
    private fb: FormBuilder,
    private userService: UserService
  ) {}

  ngOnInit(): void {
    this.getContacts();
  }

  onSubmit() {
    var searchObject: SearchEmailDto = { email: this.userSearch.value.search! };

    this.userService.getUsersEmailContains(searchObject).subscribe({
      next: (res) => {
        this.users = res;
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

  inviteToEvent(userContact: UserContactDto) {
    var invite: InviteListDto = {
      userId: userContact.userId,
      eventId: Number(this.data.eventId),
      id: 0,
    };
    this.eventService.inviteToEvent(invite).subscribe({
      next: (res) => {
        this.invitedUsers.push(userContact);
        this.toast.success({
          detail: 'SUCCESS',
          summary: 'Użytkownik został zaproszony do wydarzenia',
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

  getContacts() {
    this.userService.getMyContacts().subscribe({
      next: (contacts) => {
        this.contacts = contacts;
        console.log(this.contacts);
      },
      error: (error) => {
        console.log(error);
      },
    });
  }

  inviteToEventEmail(contact: ContactDto) {
    var contactEventDto: ContactEventDto = {
      id: 0,
      userId: 0,
      name: contact.name,
      email: contact.email,
      eventId: Number(this.data.eventId),
    };
    this.eventService.inviteToEventByEmail(contactEventDto).subscribe({
      next: (res) => {
        var newUserContact: UserContactDto = {
          name: contact.name,
          userId: res.userId,
          email: contact.email,
        };
        this.invitedUsers.push(newUserContact);
        this.toast.success({
          detail: 'SUCCESS',
          summary: 'Użytkownik został zaproszony do wydarzenia',
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

  deleteGuest(userId: number) {
    this.eventService
      .deleteGuest(this.data.eventId, userId.toString())
      .subscribe({
        next: (res) => {
          this.data.guests.splice(
            this.data.guests.findIndex((obj) => obj.id === userId),
            1
          );
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
  deleteInvitedOld(userId: number) {
    this.eventService
      .deleteInvited(this.data.eventId, userId.toString())
      .subscribe({
        next: (res) => {
          this.data.invites.splice(
            this.data.invites.findIndex((obj) => obj.id === userId),
            1
          );
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
  deleteInvitedNew(userId: number) {
    this.eventService
      .deleteInvited(this.data.eventId, userId.toString())
      .subscribe({
        next: (res) => {
          this.invitedUsers.splice(
            this.invitedUsers.findIndex((obj) => obj.userId === userId),
            1
          );
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
