import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { NotificationDto } from '../interfaces/notification-dto';
import { NotificationResponseDto } from '../interfaces/notification-response-dto';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  constructor(private http: HttpClient) {}
  private baseUrl = environment.apiUrl + 'Notification/';

  getAllOfUser() {
    return this.http.get<NotificationDto[]>(`${this.baseUrl}getAllOfUser`);
  }

  toggleRead(id: number) {
    return this.http.put<NotificationDto>(`${this.baseUrl}toggleRead`, id);
  }

  delete(id: string) {
    return this.http.delete<NotificationResponseDto>(
      `${this.baseUrl}deleteNotification/${id}`
    );
  }
}
