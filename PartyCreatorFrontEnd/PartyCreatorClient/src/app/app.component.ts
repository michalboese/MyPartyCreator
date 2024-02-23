import { Component, OnInit } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { AuthService } from './services/auth.service';
import { SignalRService } from './services/signal-r.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  title = 'PartyCreatorClient';

  constructor(
    private auth: AuthService,
    private signalRservice: SignalRService,
  ) {}

  ngOnInit(): void {
    //mozna dac if jest zalogowany
    this.signalRservice.startConnection();

    window.addEventListener('storage', (e) => this.onManualStorageChange(e));

    window.addEventListener('signOut', (e) => this.stopConnection());
    window.addEventListener('signIn', (e) => this.startConnection());

    //chyba przed stop trzeba dac te off
  }

  private stopConnection() {
    this.signalRservice.hubConnection.stop();
  }

  private startConnection() {
    this.signalRservice.startConnection();
  }

  private onManualStorageChange(event: Event) {
    if (this.auth.getToken() == null) {
      this.signalRservice.hubConnection.stop();
    } else if (
      this.signalRservice.hubConnection.state ===
      signalR.HubConnectionState.Connected
    ) {
      this.signalRservice.hubConnection.stop().then(() => {
        this.signalRservice.hubConnection.start();
      });
    } else if (
      this.signalRservice.hubConnection.state ===
      signalR.HubConnectionState.Disconnected
    ) {
      this.signalRservice.hubConnection.start();
    }
  }
  
}
