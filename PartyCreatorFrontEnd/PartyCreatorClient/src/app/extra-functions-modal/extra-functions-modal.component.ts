import { Component, Inject } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { EventService } from '../services/event.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { EventFunctionsDto } from '../interfaces/event-functions-dto';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-extra-functions-modal',
  templateUrl: './extra-functions-modal.component.html',
  styleUrls: ['./extra-functions-modal.component.css'],
  standalone: true,
  imports: [MatSlideToggleModule, ReactiveFormsModule, MatButtonModule],
})
export class ExtraFunctionsModalComponent {
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<ExtraFunctionsModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private router: Router,
    private eventService: EventService,
    private fb: FormBuilder
  ) {
    this.form = this.fb.group({
      playlist: data.hasPlaylist,
      receipt: data.hasReceipt,
      shoppingList: data.hasShoppingList,
    });
  }

  ngOnInit(): void {}

  onNoClick(): void {
    this.dialogRef.close();
  }

  onAddClick(): void {
    const values = this.form.value;
    const functions: EventFunctionsDto = {
      playlistTitle: values.playlist ? 'Title' : '',
      receiptTitle: values.receipt ? 'Title' : '',
      shoppingListTitle: values.shoppingList ? 'Title' : '',
    };
    console.log(this.data.eventId);

    this.eventService
      .addEventFunctions(this.data.eventId, functions)
      .subscribe(() => {
        this.dialogRef.close();
        this.router.navigate(['/wydarzenie/' + this.data.eventId]);
      });
  }
}
