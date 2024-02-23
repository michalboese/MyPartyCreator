import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import { UserDto } from '../interfaces/user-dto';
import { ContactDto } from '../interfaces/contact-dto';
import { UserContactDto } from '../interfaces/user-contact-dto';
import { SearchEmailDto } from '../interfaces/search-email-dto';
import { environment } from 'src/environments/environment';
import { LoginResponseDto } from '../interfaces/login-response-dto';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private baseUrl = environment.apiUrl + 'User/';

  profileUpdated = new Subject<void>();
  constructor(private http: HttpClient) {}

  getUserData(userId: string): Observable<any> {
    return this.http.get<UserDto>(`${this.baseUrl}GetUserById/${userId}`);
  }

  getMyProfileData(): Observable<any> {
    return this.http.get<UserDto>(`${this.baseUrl}GetMyProfile`);
  }

  editMyProfileData(userData: UserDto): Observable<any> {
    return this.http.post<UserDto>(`${this.baseUrl}EditMyProfile`, userData);
  }

  getMyContacts(): Observable<ContactDto[]> {
    return this.http.get<ContactDto[]>(`${this.baseUrl}GetMyContacts`);
  }

  getUserType(): Observable<LoginResponseDto> {
    return this.http.get<LoginResponseDto>(`${this.baseUrl}GetUserType`);
  }

  addContact(contactData: ContactDto): Observable<ContactDto[]> {
    return this.http.post<ContactDto[]>(
      `${this.baseUrl}AddContact`,
      contactData
    );
  }

  editContact(contactData: ContactDto): Observable<any> {
    return this.http.put<ContactDto>(
      `${this.baseUrl}EditContact/`,
      contactData
    );
  }

  deleteContact(contactId: string): Observable<any> {
    return this.http.delete<ContactDto>(
      `${this.baseUrl}DeleteContact/${contactId}`
    );
  }

  getUsersEmailContains(request: SearchEmailDto) {
    return this.http.post<UserContactDto[]>(
      `${this.baseUrl}GetUsersEmailContains`,
      request
    );
  }
}
