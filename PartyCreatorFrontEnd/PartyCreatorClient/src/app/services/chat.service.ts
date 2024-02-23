import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ChatMessageReceiveDto } from '../interfaces/chat-message-receive-dto';
import { ChatMessageSendDto } from '../interfaces/chat-message-send-dto';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ChatService {
  constructor(private http: HttpClient) {}
  private baseUrl = environment.apiUrl + 'Chat/';

  getAllMessages(id: string) {
    return this.http.get<ChatMessageReceiveDto[]>(
      `${this.baseUrl}getAllMessages/${id}`
    );
  }

  sendMessage(message: ChatMessageSendDto) {
    return this.http.post<ChatMessageReceiveDto>(
      `${this.baseUrl}sendMessage`,
      message
    );
  }
}
