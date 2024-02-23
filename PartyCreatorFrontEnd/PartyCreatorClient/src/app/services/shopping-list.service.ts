import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ShoppingListService {
  private Url = environment.apiUrl + 'ShoppingList/';

  constructor(private http: HttpClient) {}
  getShoppingList(eventId: number): Observable<any> {
    return this.http.get(`${this.Url}GetShoppingList/${eventId}`);
  }

  addNewItem(eventId: number, item: any): Observable<any> {
    return this.http.post(`${this.Url}NewShoppingListItem`, item);
  }

  deleteItem(eventId: number, itemId: number): Observable<any> {
    return this.http.delete(
      `${this.Url}RemoveShoppingListItem/${eventId}/${itemId}`
    );
  }

  signUpForItem(itemId: number): Observable<any> {
    return this.http.put(`${this.Url}SignUpForItem/${itemId}`, null);
  }

  signOutFromItem(itemId: number): Observable<any> {
    return this.http.put(`${this.Url}SignOutFromItem/${itemId}`, null);
  }
}
