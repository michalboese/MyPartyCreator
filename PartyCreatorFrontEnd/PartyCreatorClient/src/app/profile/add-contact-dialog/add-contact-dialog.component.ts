import { Component, Inject } from '@angular/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-add-contact-dialog',
  templateUrl: './add-contact-dialog.component.html',
  styleUrls: ['./add-contact-dialog.component.css'],
  standalone: true,
  imports: [
    MatFormFieldModule,
    MatDialogModule,
    MatButtonModule,
    MatInputModule,
    FormsModule,
  ],
})
export class AddContactDialogComponent {
  title: string;
  contact = { name: '', email: '' };

  constructor(@Inject(MAT_DIALOG_DATA) public data: any) {
    this.title = data.title;
    this.contact = data.contact;
  }
}
