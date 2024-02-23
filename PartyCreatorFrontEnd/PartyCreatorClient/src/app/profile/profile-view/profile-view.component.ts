import { DatePipe } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { UserService } from 'src/app/services/user.service';
import { CommonModule } from '@angular/common';
import { ProfileEditComponent } from '../profile-edit/profile-edit.component';

@Component({
  selector: 'app-profile-view',
  templateUrl: './profile-view.component.html',
  styleUrls: ['./profile-view.component.css'],
  standalone: true,
  imports: [DatePipe, CommonModule],
})
export class ProfileViewComponent implements OnInit {
  @ViewChild(ProfileEditComponent) profileEditComponent!: ProfileEditComponent;
  constructor(private userService: UserService) {}

  public userData: any;
  public profilePicture: string = '';

  ngOnInit(): void {
    this.refreshUserData();

    this.userService.profileUpdated.subscribe(() => {
      this.refreshUserData();
    });
  }

  private refreshUserData(): void {
    this.userService.getMyProfileData().subscribe({
      next: (data) => {
        this.userData = data;
        this.profilePicture = `assets/profile-avatars/${this.userData.image}`;
      },
      error: (error) => {
        console.log(error);
      },
    });
  }
}
