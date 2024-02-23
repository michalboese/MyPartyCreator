import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { UserService } from 'src/app/services/user.service';
import { NavMenuMainComponent } from 'src/app/nav-menu-main/nav-menu-main.component';
import { MatTabsModule } from '@angular/material/tabs';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { NgToastService } from 'ng-angular-popup';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.css'],
  standalone: true,
  imports: [
    CommonModule,
    NavMenuMainComponent,
    MatTabsModule,
    MatIconModule,
    MatButtonModule,
  ],
})
export class UserProfileComponent implements OnInit {
  public userData: any;
  public profilePicture: string = '';
  public userId: string = '';
  public contact: any;

  constructor(
    private route: ActivatedRoute,
    private userService: UserService,
    private toast: NgToastService,
    private router: Router
  ) {
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
  }

  ngOnInit(): void {
    this.loadUser();
  }

  loadUser() {
    this.userId = this.route.snapshot.paramMap.get('id')!;
    this.userService.getUserData(this.userId).subscribe({
      next: (data) => {
        console.log(data);
        this.contact = {
          name: data.firstName + ' ' + data.lastName,
          email: data.email,
        };
        console.log(this.contact);
        this.userData = data;
        this.profilePicture = `assets/profile-avatars/${this.userData.image}`;
      },
      error: (err: HttpErrorResponse) => {
        this.toast.error({
          detail: 'ERROR',
          summary: err.error,
          duration: 3000,
        });
        this.router.navigate([`wydarzenia`]);
      },
    });
  }

  addContact() {
    this.userService.addContact(this.contact).subscribe({
      next: (res) => {
        console.log(res);
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
}
