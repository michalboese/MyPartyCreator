import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ReceiptItem } from 'src/app/interfaces/receipt-item';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ReceiptItemService {
  private apiUrl = environment.apiUrl + 'ReceiptItem';

  constructor(private http: HttpClient) { }

  getReceiptItems(eventId: number): Observable<ReceiptItem[]> {
    return this.http.get<ReceiptItem[]>(`${this.apiUrl}/GetReceiptItems/${eventId}`);
  }

  addReceiptItem(receiptItem: ReceiptItem): Observable<ReceiptItem> {
    return this.http.post<ReceiptItem>(`${this.apiUrl}/AddReceiptItem`, receiptItem);
  }

/*  updateReceiptItem(receiptItem: ReceiptItem): Observable<ReceiptItem> {
    return this.http.put<ReceiptItem>(`${this.apiUrl}/UpdateReceiptItem`, receiptItem);
  } */

  removeReceiptItem(id: number): Observable<ReceiptItem> {
    return this.http.delete<ReceiptItem>(`${this.apiUrl}/RemoveReceiptItem/${id}`);
  }
}
