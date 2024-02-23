import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-profile-edit-avatar',
  templateUrl: './profile-edit-avatar.component.html',
  styleUrls: ['./profile-edit-avatar.component.css'],
  standalone: true,
  imports: [CommonModule],
})
export class ProfileEditAvatarComponent {
  avatars = [
    'assets/profile-avatars/avatar1.png',
    'assets/profile-avatars/avatar2.png',
    'assets/profile-avatars/avatar3.png',
    'assets/profile-avatars/avatar4.png',
    'assets/profile-avatars/avatar5.png',
    'assets/profile-avatars/avatar6.png',
    'assets/profile-avatars/avatar7.png',
    'assets/profile-avatars/avatar8.png',
    'assets/profile-avatars/avatar9.png',
  ];

  constructor(private dialogRef: MatDialogRef<ProfileEditAvatarComponent>) {}

  selectAvatar(avatar: string): void {
    const filename = avatar.split('/').pop();
    this.dialogRef.close(filename);
  }
}
