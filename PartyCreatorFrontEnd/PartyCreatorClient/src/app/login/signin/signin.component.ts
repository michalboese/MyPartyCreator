import { Component, Input, OnInit, NgZone } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { LoginComponent } from '../login.component';
import { LoginDto } from 'src/app/interfaces/login-dto';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { HttpErrorResponse } from '@angular/common/http';
import { NgToastService } from 'ng-angular-popup';
import {
  ProgressSpinnerMode,
  MatProgressSpinnerModule,
} from '@angular/material/progress-spinner';
import { LoginResponseDto } from 'src/app/interfaces/login-response-dto';

declare const FB: any;

@Component({
  selector: 'app-signin',
  templateUrl: './signin.component.html',
  styleUrls: ['./signin.component.css'],
})
export class SigninComponent {
  public signinForm: FormGroup; // formularz logowania
  credentials: LoginDto = { email: '', password: '' };
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private loginComponent: LoginComponent,
    private auth: AuthService,
    private router: Router,
    private toast: NgToastService,
    private _ngZone: NgZone
  ) {
    this.signinForm = this.createSigninForm();
  }

  // tworzenie formularza logowania
  createSigninForm(): FormGroup {
    return this.fb.group({
      email: ['', [Validators.required]], // email jest wymagany
      password: ['', [Validators.required]], // hasło jest wymagane
    });
  }

  toggleForm() {
    this.loginComponent.toggleForm('signup'); // przełączenie formularza na rejestrację
  }

  submit() {
    this.isLoading = true;

    this.credentials.email = this.signinForm.value.email;
    this.credentials.password = this.signinForm.value.password;

    this.auth.signIn(this.credentials).subscribe({
      next: (res) => {
        this.auth.storeToken(res.token);
        this.toast.success({
          detail: 'SUCCESS',
          summary: 'Udało się zalogować!',
          duration: 3000,
        });
        this.signinForm.reset(); // resetowanie formularza po jego złożeniu
        this.router.navigate(['wydarzenia']);
        this.isLoading = false;
      },
      error: (err: HttpErrorResponse) => {
        this.toast.error({
          detail: 'ERROR',
          summary: err.error,
          duration: 3000,
        });
        this.isLoading = false;
      },
    });
  }

  eyeIcon = 'fa-eye-slash'; // ikona ukrywania hasła

  hideShowPass() {
    // pokazywanie/ukrywanie hasła

    var x = document.getElementById('password');
    if (x!.getAttribute('type') == 'password') {
      x!.setAttribute('type', 'text');
      this.eyeIcon = 'fa-eye';
    } else {
      x!.setAttribute('type', 'password');
      this.eyeIcon = 'fa-eye-slash';
    }
  }

  async loginFacebook() {
    FB.login(
      async (result: any) => {
        await this.auth
          .LoginWithFacebook(result.authResponse.accessToken)
          .subscribe({
            next: (res: LoginResponseDto) => {
              this._ngZone.run(() => {
                this.auth.storeToken(res.token);
                this.router.navigate(['/wydarzenia']);
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
      },
      { scope: 'email', auth_type: 'reauthenticate' }
    );
  }
}
