import { Component, OnInit, Input } from '@angular/core';
import { ReceiptItemService } from 'src/app/services/receipt-item.service';
import { ReceiptItem } from 'src/app/interfaces/receipt-item';
import { EventService } from 'src/app/services/event.service';
import { RoleDto } from 'src/app/interfaces/role-dto';


@Component({
  selector: 'app-split-bill',
  templateUrl: './split-bill.component.html',
  styleUrls: ['./split-bill.component.css']
})
export class SplitBillComponent implements OnInit {
  userRole: RoleDto = { id: 0, role: '' };

  @Input() eventId = '';
  @Input() numberOfParticipants = 1;
  receiptItems: ReceiptItem[] = [];
  receiptItem: ReceiptItem = {
    id: 0,
    name: '',
    quantity: 1,
    price: 0,
    eventId: 0
  };
  

  constructor(
    private splitBillService: ReceiptItemService, 
    private eventService: EventService) { }
 
  ngOnInit(): void {
    this.receiptItem.eventId = Number(this.eventId);
    this.loadReceiptItems();
    this.loadNumberOfParticipants();
    this.loadUserRole();
  }
  loadUserRole(): void {
    this.eventService.getAccess(this.eventId).subscribe(
      data => this.userRole = data,
      error => console.error('Error loading user role', error)
    );
  }

  loadNumberOfParticipants(): void {
    this.eventService.getGuestsUsers(this.eventId).subscribe(
      data => this.numberOfParticipants = data.length +1 ,
      error => console.error('Error loading number of participants', error)
    );
  }
  loadReceiptItems(): void {
    this.splitBillService.getReceiptItems(Number(this.eventId)).subscribe(
      data => this.receiptItems = data,
      error => console.error('Error loading receipt items', error)
    );
  }

  onSubmit() { 
    this.splitBillService.addReceiptItem(this.receiptItem).subscribe(
      response => {
        console.log('Receipt item added successfully', response);
        this.loadReceiptItems(); // Ponownie załaduj przedmioty po dodaniu nowego
      },
      error => console.error('Error adding receipt item', error)
    );
  }
  getTotalPrice() {
    let total = 0;
    for (let item of this.receiptItems) {
      total += item.price;
    }
    return total;
  }
  removeItem(id: number) {
    this.splitBillService.removeReceiptItem(id).subscribe({
      next: (response) => {
        console.log('Przedmiot został usunięty', response);
        // Aktualizacja lokalnej listy przedmiotów - usuń przedmiot z listy
        this.receiptItems = this.receiptItems.filter(item => item.id !== id);
        //this.loadReceiptItems(); 
      },
      error: err => console.error('Error removing receipt item', err),
    });
  }
  getAmountPerPerson(): string {
    const totalPrice = this.getTotalPrice();
    const amountPerPerson = this.numberOfParticipants ? totalPrice / this.numberOfParticipants : 0;
    return amountPerPerson.toFixed(2);
  }
}
