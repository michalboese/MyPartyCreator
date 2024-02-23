import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';

export type LoginType = 'signin' | 'signup';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})

// wybór między logowaniem a rejestracją
export class LoginComponent implements OnInit {
  //ustawianie domyślnego formularza
  login: string = '';

  constructor(private authService: AuthService) {}

  ngOnInit() {
    this.authService.changeIsLoggedInValue();
    this.authService.currentLoginType.subscribe((type) => {
      this.login = type;
    });
  }

  get showSignInForm() {
    return this.login === 'signin';
  }

  get showSignUpForm() {
    return this.login === 'signup';
  }

  toggleForm(type: LoginType) {
    this.login = type;
  }
}
