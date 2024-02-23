import {
  Component,
  Input,
  OnInit,
  AfterViewChecked,
  ElementRef,
  ViewChild,
  OnDestroy,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChatMessageReceiveDto } from 'src/app/interfaces/chat-message-receive-dto';
import { ChatService } from 'src/app/services/chat.service';
import { HttpErrorResponse } from '@angular/common/http';
import { NgToastService } from 'ng-angular-popup';
import { ChatMessageSendDto } from 'src/app/interfaces/chat-message-send-dto';
import { FormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UserService } from 'src/app/services/user.service';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { SignalRService } from 'src/app/services/signal-r.service';
import * as signalR from '@microsoft/signalr';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    ReactiveFormsModule,
    MatIconModule,
  ],
})
export class ChatComponent implements OnInit, OnDestroy, AfterViewChecked {
  @Input() eventId = '';
  messages: ChatMessageReceiveDto[] = [];
  @ViewChild('myScrollContainer', { static: false })
  myScrollContainer!: ElementRef;
  newMessage: string;
  @Input() userId: number = 0;
  messageData = this.fb.group({
    message: '',
  });
  prevMessagesLength: number = 0;

  constructor(
    private chatService: ChatService,
    private toast: NgToastService,
    private fb: FormBuilder,
    private signalRService: SignalRService
  ) {
    this.newMessage = '';
  }

  ngOnInit(): void {
    this.loadAllMessages();

    this.signalRService.hubConnection.on(
      'ReceiveChatMessage',
      (signalRMessage: ChatMessageReceiveDto) => {
        console.log(signalRMessage);

        if (signalRMessage.userId != this.userId) {
          var responseMessage = signalRMessage;
          responseMessage.dateTime = new Date(signalRMessage.dateTime);
          var stringDate = responseMessage.dateTime.toISOString().slice(0, -1);
          responseMessage.dateTime = new Date(stringDate);

          this.messages.push(signalRMessage);
        }
      }
    );
  }

  ngOnDestroy(): void {
    this.signalRService.hubConnection.off('ReceiveChatMessage');
  }

  ngAfterViewChecked(): void {
    if (this.prevMessagesLength !== this.messages.length) {
      this.prevMessagesLength = this.messages.length;
      this.scrollToBottom();
    }
  }

  scrollToBottom(): void {
    try {
      this.myScrollContainer.nativeElement.scrollTop =
        this.myScrollContainer.nativeElement.scrollHeight -
        this.myScrollContainer.nativeElement.clientHeight;
    } catch (err) {}
  }

  loadAllMessages() {
    this.chatService.getAllMessages(this.eventId).subscribe({
      next: (res) => {
        this.messages = res;
      },
      error: (err: HttpErrorResponse) => {
        this.toast.error({
          detail: 'ERROR',
          summary: err.error,
          duration: 3000,
        });
      },
    });
  }

  sendMessage() {
    if (
      this.messageData.value.message != '' ||
      this.messageData.value.message != null
    ) {
      const messageToSend: ChatMessageSendDto = {
        id: 0,
        userId: this.userId,
        eventId: +this.eventId,
        message: this.messageData.value.message!,
        dateTime: new Date(),
      };

      this.chatService.sendMessage(messageToSend).subscribe(
        (response) => {
          this.newMessage = '';
          var responseMessage = response;

          responseMessage.dateTime = new Date(response.dateTime);
          var stringDate = responseMessage.dateTime.toISOString().slice(0, -1);
          responseMessage.dateTime = new Date(stringDate);

          this.messages.push(responseMessage);
          this.messageData.reset();
        },
        (error: HttpErrorResponse) => {
          console.error('Błąd podczas wysyłania wiadomości:', error);
        }
      );
    }
  }
}
