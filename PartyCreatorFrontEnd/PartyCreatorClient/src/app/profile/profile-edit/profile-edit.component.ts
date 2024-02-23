import { Component, OnInit } from '@angular/core';
import { UserService } from 'src/app/services/user.service';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import {
  FormGroup,
  FormControl,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatButtonModule } from '@angular/material/button';
import { DatePipe } from '@angular/common';
import { NgToastService } from 'ng-angular-popup';
import { ProfileEditAvatarComponent } from './profile-edit-avatar/profile-edit-avatar.component';
import { MatDialog } from '@angular/material/dialog';
import { AuthService } from 'src/app/services/auth.service';
import { ChangePasswordDto } from 'src/app/interfaces/change-password-dto';
import { CustomValidators } from 'src/app/login/custom-validators';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { AbstractControl } from '@angular/forms';

@Component({
  selector: 'app-profile-edit',
  templateUrl: './profile-edit.component.html',
  styleUrls: ['./profile-edit.component.css'],
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatIconModule
  ],
  providers: [DatePipe],
})
export class ProfileEditComponent implements OnInit {
  public userData: any;
  public profileForm!: FormGroup;
  public passwordForm!: FormGroup;
  public profilePicture: string = '';
  showChangePasswordForm = false;
  hideOldPassword = true;
  hideNewPassword = true;
  public accountType: string = '';

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private toast: NgToastService,
    private dialog: MatDialog
  ) {
    this.profileForm = new FormGroup({
      firstName: new FormControl(''),
      lastName: new FormControl(''),
      email: new FormControl(''),
      birthday: new FormControl(''),
      description: new FormControl(''),
    });
  }
  maxDate = new Date(
    new Date().getFullYear(),
    new Date().getMonth(),
    new Date().getDate()
  );

  ngOnInit(): void {
    this.userService.getMyProfileData().subscribe((data) => {
      if (data) {
        this.userData = data;
        this.profilePicture = `assets/profile-avatars/${this.userData.image}`;
        this.profileForm = new FormGroup({
          firstName: new FormControl(
            this.userData ? this.userData.firstName : ''
          ),
          lastName: new FormControl(
            this.userData ? this.userData.lastName : ''
          ),
          email: new FormControl(this.userData ? this.userData.email : ''),
          birthday: new FormControl(
            this.userData ? this.userData.birthday : ''
          ),
          description: new FormControl(
            this.userData ? this.userData.description : ''
          ),
        });
      }
    });
    this.passwordForm = new FormGroup({
      oldPassword: new FormControl('', [Validators.required, Validators.minLength(8)]),
      newPassword: new FormControl('', [Validators.required, Validators.minLength(8), CustomValidator]),
    }, { validators: CustomValidators.passwordMatchValidator });
    
    this.userService.getUserType().subscribe((res) => {
      this.accountType = res.token;
      console.log(this.accountType);
    });
  }

  public savePassword(): void {
    if (this.passwordForm.valid) {
      const changePasswordData: ChangePasswordDto = {
        oldPassword: this.passwordForm.value.oldPassword,
        newPassword: this.passwordForm.value.newPassword,
      };

      this.authService.changePassword(changePasswordData).subscribe({
        next: () => {
          this.toast.success({
            detail: 'SUCCESS',
            summary: 'Hasło zostało zmienione',
            duration: 3000,
          });
          this.passwordForm.reset();
          this.showChangePasswordForm = false;
        },
        error: (err) => {
          this.toast.error({
            detail: 'ERROR',
            summary: err.error,
            duration: 3000,
          });
        },
      });
    }
  }

  changeProfilePicture(): void {
    const dialogRef = this.dialog.open(ProfileEditAvatarComponent);

    dialogRef.afterClosed().subscribe((selectedAvatar) => {
      if (selectedAvatar) {
        console.log(selectedAvatar);
        this.profilePicture = `assets/profile-avatars/${selectedAvatar}`;
        this.profileForm.value.image = selectedAvatar;
      }
    });
  }

  public editProfile(): void {
    this.userService.editMyProfileData(this.profileForm.value).subscribe({
      next: (res) => {
        console.log(res);
        this.userService.profileUpdated.next();
        this.toast.success({
          detail: 'SUCCESS',
          summary: 'Udało się edytować profil!',
          duration: 3000,
        });
      },
      error: (err) => {
        this.toast.error({
          detail: 'ERROR',
          summary: err.error,
          duration: 3000,
        });
      },
    });
  }
  toggleChangePasswordForm(): void {
    this.showChangePasswordForm = !this.showChangePasswordForm;
    console.log('showChangePasswordForm:', this.showChangePasswordForm);

  }

  toggleOldPasswordVisibility(): void {
    this.hideOldPassword = !this.hideOldPassword;
  }

  toggleNewPasswordVisibility(): void {
    this.hideNewPassword = !this.hideNewPassword;
  }
}

export function CustomValidator(control: AbstractControl) {
  let value: string = control.value || '';
  let upperCaseCharacters = /[A-Z]+/g;
  let numberCharacters = /[0-9]+/g;
  if (value.length < 8) {
    return { customError: 'Hasło musi zawierać przynajmniej 8 znaków' };
  }
  if (!upperCaseCharacters.test(value)) {
    return { customError: 'Hasło musi zawierać przynajmniej jedną dużą literę' };
  }
  if (!numberCharacters.test(value)) {
    return { customError: 'Hasło musi zawierać przynajmniej jedną cyfrę' };
  }
  return null;
}