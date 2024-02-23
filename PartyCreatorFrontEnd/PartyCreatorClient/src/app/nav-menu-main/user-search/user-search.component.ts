import { Component } from '@angular/core';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { FormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { SearchEmailDto } from 'src/app/interfaces/search-email-dto';
import { HttpErrorResponse } from '@angular/common/http';
import { NgToastService } from 'ng-angular-popup';
import { UserContactDto } from 'src/app/interfaces/user-contact-dto';
import { UserService } from 'src/app/services/user.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user-search',
  templateUrl: './user-search.component.html',
  styleUrls: ['./user-search.component.css'],
  standalone: true,
  imports: [
    MatIconModule,
    MatMenuModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    CommonModule,
  ],
})
export class UserSearchComponent {
  userSearch = this.fb.group({
    search: '',
  });
  users: UserContactDto[] = [];

  constructor(
    private fb: FormBuilder,
    private toast: NgToastService,
    private userService: UserService,
    private router: Router
  ) {}

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

  doSomething($event: any) {
    $event.stopPropagation();
    //Another instructions
  }

  goToProfile(userId: number) {
    this.router.navigate([`profil/${userId}`]);
  }
}
