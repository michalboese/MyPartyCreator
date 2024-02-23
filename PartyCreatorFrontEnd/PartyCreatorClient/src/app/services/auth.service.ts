import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { LoginDto } from '../interfaces/login-dto';
import { RegisterDto } from '../interfaces/register-dto';
import { LoginResponseDto } from '../interfaces/login-response-dto';
import { UserDto } from '../interfaces/user-dto';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { ChangePasswordDto } from '../interfaces/change-password-dto'; // Upewnij się, że masz odpowiednią strukturę DTO.

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private baseUrl = environment.apiUrl + 'Auth/';
  private loginTypeSource = new BehaviorSubject<string>('signin'); //domyślnie signin
  currentLoginType = this.loginTypeSource.asObservable();
  eventSignIn = new Event('signIn');
  eventSignOut = new Event('signOut');
  isLoggedInValue = true;

  constructor(private http: HttpClient, private router: Router) {
    this.isLoggedInValue = this.isLoggedIn();
  }

  signUp(registerObj: RegisterDto) {
    return this.http.post<UserDto>(`${this.baseUrl}register`, registerObj);
  }

  signIn(loginObj: LoginDto) {
    return this.http.post<LoginResponseDto>(`${this.baseUrl}login`, loginObj);
  }

  storeToken(tokenValue: string) {
    localStorage.setItem('token', tokenValue);
    this.changeIsLoggedInValue();

    window.dispatchEvent(this.eventSignIn);
  }

  getToken() {
    return localStorage.getItem('token');
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }

  signOut() {
    localStorage.clear();
    this.changeIsLoggedInValue();

    window.dispatchEvent(this.eventSignOut);

    this.changeLoginType('signin');
    this.router.navigate(['logowanie']);
  }

  changeLoginType(type: string) {
    this.loginTypeSource.next(type);
  }

  changeIsLoggedInValue() {
    if (this.isLoggedIn()) {
      this.isLoggedInValue = true;
    } else {
      this.isLoggedInValue = false;
    }
  }

  changePassword(changePasswordData: ChangePasswordDto) {
    return this.http.post<any>(
      `${this.baseUrl}change-password`,
      changePasswordData
    );
  }

  LoginWithFacebook(credentials: string): Observable<any> {
    const header = new HttpHeaders().set('Content-type', 'application/json');
    return this.http.post(
      `${this.baseUrl}loginWithFacebook`,
      JSON.stringify(credentials),
      { headers: header, withCredentials: true }
    );
  }
}
