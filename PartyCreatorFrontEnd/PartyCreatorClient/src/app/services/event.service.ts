import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EventDto } from '../interfaces/event-dto';
import { EventCreateDto } from '../interfaces/event-create-dto';
import { EventUserDto } from '../interfaces/event-user-dto';
import { Observable } from 'rxjs';
import { InviteListDto } from '../interfaces/invite-list-dto';
import { NotificationDto } from '../interfaces/notification-dto';
import { GuestListDto } from '../interfaces/guest-list-dto';
import { RoleDto } from '../interfaces/role-dto';
import { AllGuestsListDto } from '../interfaces/all-guests-list-dto';
import { UserContactDto } from '../interfaces/user-contact-dto';
import { ContactDto } from '../interfaces/contact-dto';
import { ContactEventDto } from '../interfaces/contact-event-dto';
import { EventFunctionsDto } from '../interfaces/event-functions-dto';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class EventService {
  private baseUrl = environment.apiUrl + 'Auth/';
  private baseUrl2 = environment.apiUrl + 'Event/';
  constructor(private http: HttpClient) {}

  getMe() {
    return this.http.get<string>(`${this.baseUrl}getme`);
  }

  getEventDetails(id: string): Observable<any> {
    return this.http.get<EventUserDto[]>(`${this.baseUrl2}${id}`);
  }

  getOfCreator() {
    return this.http.get<EventUserDto[]>(`${this.baseUrl2}getOfCreator`);
  }

  createEvent(eventObj: EventCreateDto) {
    return this.http.post<EventDto>(`${this.baseUrl2}create`, eventObj);
  }

  getUpcomingEvents(): Observable<EventUserDto[]> {
    return this.http.get<EventUserDto[]>(`${this.baseUrl2}getUpcoming`);
  }

  getFinishedEvents(): Observable<EventUserDto[]> {
    return this.http.get<EventUserDto[]>(`${this.baseUrl2}getFinished`);
  }

  inviteToEvent(inviteList: InviteListDto) {
    return this.http.post<InviteListDto>(`${this.baseUrl2}invite`, inviteList);
  }

  acceptInvite(notification: NotificationDto) {
    return this.http.post<GuestListDto>(`${this.baseUrl2}accept`, notification);
  }

  declineInvite(notification: NotificationDto) {
    return this.http.post<InviteListDto>(
      `${this.baseUrl2}decline`,
      notification
    );
  }

  getGuestsUsers(id: string): Observable<AllGuestsListDto[]> {
    return this.http.get<AllGuestsListDto[]>(
      `${this.baseUrl2}getGuestsUsers/${id}`
    );
  }

  getInvitesUsers(id: string): Observable<AllGuestsListDto[]> {
    return this.http.get<AllGuestsListDto[]>(
      `${this.baseUrl2}getInvitesUsers/${id}`
    );
  }

  getAccess(eventId: string) {
    return this.http.get<RoleDto>(`${this.baseUrl2}getAccess/${eventId}`);
  }

  inviteToEventByEmail(contactEventDto: ContactEventDto) {
    return this.http.post<InviteListDto>(
      `${this.baseUrl2}inviteEmail`,
      contactEventDto
    );
  }

  updateEventDetails(eventId: string, updatedDetails: any): Observable<any> {
    const url = `${this.baseUrl2}update/${eventId}`;
    return this.http.put(url, updatedDetails);
  }

  addEventFunctions(
    eventId: string,
    functions: EventFunctionsDto
  ): Observable<any> {
    const url = `${this.baseUrl2}${eventId}/addFunctions`;
    return this.http.post(url, functions);
  }
  deleteGuest(eventId: string, userId: string) {
    return this.http.delete<GuestListDto>(
      `${this.baseUrl2}deleteGuest/${eventId}/${userId}`
    );
  }
  deleteInvited(eventId: string, userId: string) {
    return this.http.delete<GuestListDto>(
      `${this.baseUrl2}deleteInvited/${eventId}/${userId}`
    );
  }

  deleteEvent(eventId: string) {
    return this.http.delete<any>(`${this.baseUrl2}deleteEvent/${eventId}`);
  }
}
