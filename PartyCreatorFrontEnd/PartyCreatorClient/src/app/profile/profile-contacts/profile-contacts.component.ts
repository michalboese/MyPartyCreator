import { Component } from '@angular/core';
import { UserService } from 'src/app/services/user.service';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { AddContactDialogComponent } from '../add-contact-dialog/add-contact-dialog.component';
import { NgToastService } from 'ng-angular-popup';

@Component({
  selector: 'app-profile-contacts',
  templateUrl: './profile-contacts.component.html',
  styleUrls: ['./profile-contacts.component.css'],
  standalone: true,
  imports: [MatIconModule, CommonModule, MatButtonModule],
})
export class ProfileContactsComponent {
  constructor(
    private userService: UserService,
    private dialog: MatDialog,
    private toast: NgToastService
  ) {}

  public contacts: any;
  ngOnInit(): void {
    this.refreshContacts();
  }

  private refreshContacts(): void {
    this.userService.getMyContacts().subscribe({
      next: (contacts) => {
        this.contacts = contacts;
      },
      error: (error) => {
        console.log(error);
      },
    });
  }

  openAddContactDialog(): void {
    const dialogRef = this.dialog.open(AddContactDialogComponent, {
      data: { contact: { name: '', email: '' }, title: 'Dodaj kontakt' },
    });

    dialogRef.afterClosed().subscribe((result) => {
      console.log(`Dialog result: ${result}`);
      if (result) {
        this.userService.addContact(result).subscribe({
          next: (res) => {
            this.contacts.push(res);
            this.toast.success({
              detail: 'SUCCESS',
              summary: 'Udało się dodać kontakt!',
              duration: 3000,
            });
          },
          error: (error) => {
            console.log(error);
            this.toast.error({
              detail: 'ERROR',
              summary: error.error,
              duration: 3000,
            });
          },
        });
      }
    });
  }

  editContact(contact: any): void {
    const dialogRef = this.dialog.open(AddContactDialogComponent, {
      data: {
        contact: { id: contact.id, name: contact.name, email: contact.email },
        title: 'Edytuj kontakt',
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      console.log(`Dialog result: ${result}`);
      if (result) {
        this.userService.editContact(result).subscribe({
          next: () => {
            this.refreshContacts();
            // this.contacts.push(result);   pomyśleć nad tym
            this.toast.success({
              detail: 'SUCCESS',
              summary: 'Kontakt został zapisany!',
              duration: 3000,
            });
          },
          error: (error) => {
            console.log(error);
            this.toast.error({
              detail: 'ERROR',
              summary: error.error,
              duration: 3000,
            });
          },
        });
      }
    });
  }

  deleteContact(contactId: string): void {
    console.log(contactId);
    this.userService.deleteContact(contactId.toString()).subscribe({
      next: (res) => {
        this.contacts.splice(this.contacts.indexOf(res), 1);
        this.toast.success({
          detail: 'SUCCESS',
          summary: 'Kontakt został usunięty!',
          duration: 3000,
        });
      },
      error: (error) => {
        console.log(error);
        this.toast.error({
          detail: 'ERROR',
          summary: error.error,
          duration: 3000,
        });
      },
    });
  }
}
