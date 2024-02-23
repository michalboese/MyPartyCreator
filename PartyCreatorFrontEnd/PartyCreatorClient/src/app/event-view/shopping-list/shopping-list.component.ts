import { Component,  Input, OnInit } from '@angular/core';
import { ShoppingListService } from '../../services/shopping-list.service';
import { ShoppingListItemDto } from '../../interfaces/shopping-list-item-dto';
import { EventService } from 'src/app/services/event.service';
import { RoleDto } from 'src/app/interfaces/role-dto';

@Component({
  selector: 'app-shopping-list',
  templateUrl: './shopping-list.component.html',
  styleUrls: ['./shopping-list.component.css']
})
export class ShoppingListComponent implements OnInit {
  userRole: RoleDto = { id: 0, role: '' };

  @Input() eventId = '';

  shoppingList: ShoppingListItemDto[] = []; 
  eventDetails: any; 
  isThingsToBringVisible: boolean = false;
  newItemName: string = '';
  newItemQuantity: number = 0;
  guestsUsers: any[] = [];
  
  constructor(
    private shoppingListService: ShoppingListService,
    private eventService: EventService) { }

    ngOnInit(): void {
      this.loadUserRole();
      this.loadEventDetails(); 
    }
    
    loadEventDetails() {
      this.eventService.getEventDetails(this.eventId).subscribe(
        data => {
          this.eventDetails = data;
          this.loadGuestsUsers();
        },
        error => console.error('Error loading event details', error)
      );
    }
    
    loadGuestsUsers() {
      this.eventService
        .getGuestsUsers(this.eventDetails.id.toString())
        .subscribe((users) => {
          this.guestsUsers = users;
          this.guestsUsers.unshift({
            id: this.eventDetails.creatorId,
            firstName: this.eventDetails.creatorFirstName,
            lastName: this.eventDetails.creatorLastName,
          });
          this.loadShoppingList();
        });
    }
  loadUserRole(): void {
    this.eventService.getAccess(this.eventId).subscribe(
      data => this.userRole = data,
      error => console.error('Error loading user role', error)
    );
  }


  loadShoppingList() {
    this.shoppingListService.getShoppingList(Number(this.eventId)).subscribe(
      (shoppingList: ShoppingListItemDto[]) => {
        this.shoppingList = shoppingList.map((item: ShoppingListItemDto) => {
          const user = this.guestsUsers.find(user => user.id === item.userId);
          return {
            ...item,
            firstName: user ? user.firstName : '',
            lastName: user ? user.lastName : '',
          };
        });
      },
      (error) => {
        console.error('Błąd podczas ładowania listy zakupów', error);
      }
    );
  }
  

  addItem() {
    if (this.newItemName && this.newItemQuantity > 0 && this.newItemQuantity < 100) {
      const newItem = {
        id: 0,
        userId: 0,
        eventId: this.eventId,
        name: this.newItemName,
        quantity: this.newItemQuantity,
      };

      this.shoppingListService
        .addNewItem(Number(this.eventId), newItem)
        .subscribe(
          () => {
            this.loadShoppingList();
            this.newItemName = '';
            this.newItemQuantity = 0;
          },
          (error) => {
            console.error(
              'Wystąpił błąd podczas dodawania nowego przedmiotu',
              error
            );
          }
        );
    }
  }

  deleteItem(itemId: number) {
    this.shoppingListService
      .deleteItem(Number(this.eventId), itemId)
      .subscribe(
        (response) => {
          this.shoppingList = this.shoppingList.filter(
            (item) => item.id !== itemId
          );
        },
        (error) => {
          console.error('Wystąpił błąd podczas usuwania przedmiotu', error);
        }
      );
  }

  signUpForItem(itemId: number) {
    this.shoppingListService.signUpForItem(itemId).subscribe(
      () => {
        this.loadShoppingList(); 
      },
      (error) => {
        console.error(
          'Wystąpił błąd podczas zapisywania się na przedmiot',
          error
        );
      }
    );
  }

  signOutFromItem(itemId: number) {
    this.shoppingListService.signOutFromItem(itemId).subscribe(
      () => {
        this.loadShoppingList(); // odśwież listę po wypisaniu się z przedmiotu
      },
      (error) => {
        console.error(
          'Wystąpił błąd podczas wypisywania się z przedmiotu',
          error
        );
      }
    );
  }
}