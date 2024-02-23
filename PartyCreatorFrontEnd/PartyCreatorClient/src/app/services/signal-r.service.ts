import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import * as signalR from '@microsoft/signalr';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class SignalRService {
  hubConnection!: signalR.HubConnection;
  baseUrl = environment.apiUrl.replace('/api/', '');
  event = new Event('signalRConnected');

  constructor(private auth: AuthService) {}

  startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${this.baseUrl}/chat`, {
        accessTokenFactory: () => this.auth.getToken() ?? '',
      })
      .build();
    this.hubConnection
      .start()
      .then(() => {
        window.dispatchEvent(this.event);
      })
      .catch((err) => {});
  }
}
