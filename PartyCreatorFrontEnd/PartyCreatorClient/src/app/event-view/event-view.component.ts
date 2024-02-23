import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { EventService } from '../services/event.service';
import { UserService } from '../services/user.service';
import {
  faLocationDot,
  faCheck,
  faCalendar,
  faClock,
} from '@fortawesome/free-solid-svg-icons';
import {
  trigger,
  state,
  style,
  animate,
  transition,
  keyframes,
} from '@angular/animations';
import { HttpErrorResponse } from '@angular/common/http';
import { NgToastService } from 'ng-angular-popup';
import { Router } from '@angular/router';
import * as leafletGeosearch from 'leaflet-geosearch';
import { RoleDto } from '../interfaces/role-dto';
import { MatDialog } from '@angular/material/dialog';
import { InviteModalComponent } from '../invite-modal/invite-modal.component';
import { AllGuestsListDto } from '../interfaces/all-guests-list-dto';
import { EventUserDto } from '../interfaces/event-user-dto';
import { ShoppingListService } from '../services/shopping-list.service';
import { ExtraFunctionsModalComponent } from '../extra-functions-modal/extra-functions-modal.component';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { ShoppingListItemDto } from '../interfaces/shopping-list-item-dto';
import { ChatService } from '../services/chat.service';
import { ChatMessageReceiveDto } from '../interfaces/chat-message-receive-dto';
import { MapComponent } from '../map/map.component';
import { GalleryService } from '../services/gallery.service';
import { FileUpload } from 'primeng/fileupload';
import { PhotoDto } from '../interfaces/photo-dto';
import { SignalRService } from '../services/signal-r.service';
import * as signalR from '@microsoft/signalr';
import { ChangeGuestInviteDto } from '../interfaces/change-guest-invite-dto';
import { InviteListDto } from '../interfaces/invite-list-dto';
import { ConfirmDialogComponent } from 'src/app/event-view/confirm-dialog/confirm-dialog.component';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'app-event-view',
  templateUrl: './event-view.component.html',
  styleUrls: ['./event-view.component.css'],
  animations: [
    trigger('slideDown', [
      state('void', style({ height: '0', opacity: '0' })),
      state('*', style({ height: '*', opacity: '1' })),
      transition('void <=> *', animate('500ms ease-in-out')),
    ]),
    trigger('pulse', [
      state('rest', style({ transform: 'scale(1)' })),
      state('pulse', style({ transform: 'scale(1.2)' })),
      transition('rest <=> pulse', animate('500ms ease-in-out')),
    ]),
  ],
})
export class EventViewComponent implements OnInit, OnDestroy {
  faArrowRight: any;
  selected: Date | null;
  eventDetails: EventUserDto;

  faClock = faClock;
  faCalendar = faCalendar;
  faLocationDot = faLocationDot;
  faCheck = faCheck;
  isThingsToBringVisible: boolean = false;
  isMapVisible: boolean = false;

  arrowAnimationState: string = 'rest';

  images: PhotoDto[] = [];
  numVisible = 3;
  maxNumVisible = 3;

  guestsUsers: AllGuestsListDto[] = [];
  invitesUsers: AllGuestsListDto[] = [];
  userRole: RoleDto = { id: 0, role: '' };
  eventId = '';
  eventTitle = '';
  editMode = false;
  editField: string | null = null;
  editedTime: string = '';
  editedDate: string = '';
  isSuccess: number = 0;
  shoppingList: {
    userId: number;
    id: number;
    name: string;
    quantity: number;
    firstName: string;
    lastName: string;
  }[] = [];
  newItemName: string = '';
  newItemQuantity: number = 0;

  constructor(
    private route: ActivatedRoute,
    private eventService: EventService,
    private userService: UserService,
    private toast: NgToastService,
    private router: Router,
    private shoppingListService: ShoppingListService,
    public dialog: MatDialog,
    private chatService: ChatService,
    private galleryService: GalleryService,
    private signalRService: SignalRService
  ) {
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
    this.selected = null;
    this.eventDetails = {
      id: 0,
      creatorId: 0,
      creatorFirstName: '',
      creatorLastName: '',
      title: '',
      description: '',
      dateTime: '',
      city: '',
      address: '',
      country: '',
      color: '',
      playlistTitle: '',
      shoppingListTitle: '',
      receiptTitle: '',
    };
  }

  ngOnInit() {
    this.authorization();
    //zaladuj wszystko jesli ma autoryzacje
    //w html tak samo
    this.loadEventDetails();
    this.loadImages();

    this.addToEventGroup();
    window.addEventListener('signalRConnected', (e) => this.addToEventGroup());

    this.signalRService.hubConnection.on('EventJoined', (message: string) => {
      console.log(message);
    });
    this.signalRService.hubConnection.on('EventLeft', (message: string) => {
      console.log(message);
    });

    this.signalRService.hubConnection.on(
      'AcceptedInvite',
      (signalRMessage: ChangeGuestInviteDto) => {
        console.log(signalRMessage);

        // var inviteIndex = this.invitesUsers.findIndex(
        //   (invite) => invite.id === signalRMessage.inviteListId
        // );
        // if (inviteIndex !== -1) {
        //   console.log('TUTAJ');
        //   var user = this.invitesUsers.find(
        //     (u) => u.id == signalRMessage.inviteListId
        //   );
        //   this.guestsUsers.push(user!);

        //   this.invitesUsers.splice(inviteIndex, 1);
        // } nie dziala :)
        this.loadGuestsUsers();
        this.loadInvitedUsers();
      }
    );

    this.signalRService.hubConnection.on(
      'DeclineInvite',
      (signalRMessage: InviteListDto) => {
        console.log(signalRMessage);

        // var inviteIndex = this.invitesUsers.findIndex(
        //   (invite) => invite.id === signalRMessage.id
        // );
        // if (inviteIndex !== -1) {
        //   this.invitesUsers.splice(inviteIndex, 1);
        // } nie dziala :)
        this.loadGuestsUsers();
        this.loadInvitedUsers();
      }
    );
    this.signalRService.hubConnection.on('DeleteEvent', () => {
      this.router.navigate([`wydarzenia`]);
      this.toast.error({
        detail: 'DELETE',
        summary: 'Wydarzenie zostalo usuniete',
        duration: 3000,
      });
    });
  }

  ngOnDestroy(): void {
    this.removeFromEventGroup();
    this.signalRService.hubConnection.off('EventJoined');
    this.signalRService.hubConnection.off('EventLeft');
    this.signalRService.hubConnection.off('AcceptedInvite');
    this.signalRService.hubConnection.off('DeclineInvite');
    this.signalRService.hubConnection.off('DeleteEvent');
    window.removeEventListener('signalRConnected', (e) =>
      this.addToEventGroup()
    ); //niby nie trzeba ale dla pewnosci
  }

  //Event methods
  authorization() {
    //tutaj naprawic te returny
    this.eventId = this.route.snapshot.paramMap.get('id')!;
    this.eventService.getAccess(this.eventId).subscribe({
      next: (res) => {
        this.userRole = res;
        console.log(this.userRole);
      },
      error: (err: HttpErrorResponse) => {
        this.router.navigate([`wydarzenia`]);
        this.toast.error({
          detail: 'ERROR',
          summary: err.error,
          duration: 3000,
        });
      },
    });
  }

  loadEventDetails(): void {
    const eventId: string | null = this.route.snapshot.paramMap.get('id');
    if (eventId !== null) {
      this.eventService.getEventDetails(eventId).subscribe(
        (data: EventUserDto | null) => {
          if (data !== null) {
            this.eventDetails = data;
            console.log(this.eventDetails.dateTime);
            //this.eventDetails.dateTime = this.eventDetails.dateTime + 'Z';
            this.eventTitle = data.title;
            this.loadGuestsUsers();
            this.loadInvitedUsers();
            this.loadShoppingList();
          } else {
            console.error('Otrzymano nullowe dane z serwera');
          }
        },
        (error: any) => {
          console.error(
            'Wystąpił błąd podczas pobierania szczegółów wydarzenia',
            error
          );
        }
      );
    } else {
      console.error('ID wydarzenia jest nullem lub niezdefiniowane');
    }
  }

  goToUserProfile(userId: number): void {
    this.router.navigate(['/profil', userId]);
  }

  loadGuestsUsers() {
    this.eventService
      .getGuestsUsers(this.eventDetails!.id.toString())
      .subscribe((users) => {
        this.guestsUsers = users;
        this.guestsUsers.unshift({
          id: this.eventDetails.creatorId,
          firstName: this.eventDetails.creatorFirstName,
          lastName: this.eventDetails.creatorLastName,
        });
      });
  }

  loadInvitedUsers() {
    this.eventService
      .getInvitesUsers(this.eventDetails!.id.toString())
      .subscribe((users) => {
        this.invitesUsers = users;
      });
  }

  saveChanges() {
    this.editMode = false;
    this.editField = null;

    if (!this.editedDate) {
      this.editedDate = this.eventDetails.dateTime.slice(0, 10);
    }

    if (!this.editedTime) {
      this.editedTime = this.eventDetails.dateTime.slice(11, 16);
    }

    if (
      this.editedDate.match(/^\d{4}-\d{2}-\d{2}$/) &&
      this.editedTime.match(/^\d{2}:\d{2}$/)
    ) {
      const editedDateTime = new Date(
        this.editedDate + 'T' + this.editedTime + ':00'
      );

      // Formatuj datę i czas jako ciąg znaków 'YYYY-MM-DDTHH:MM:SS'
      const year = editedDateTime.getFullYear();
      const month = String(editedDateTime.getMonth() + 1).padStart(2, '0'); // dodaj 1 do miesiąca, bo są indeksowane od 0
      const day = String(editedDateTime.getDate()).padStart(2, '0');
      const hour = String(editedDateTime.getHours()).padStart(2, '0');
      const minute = String(editedDateTime.getMinutes()).padStart(2, '0');
      const second = String(editedDateTime.getSeconds()).padStart(2, '0');

      this.eventDetails.dateTime = `${year}-${month}-${day}T${hour}:${minute}:${second}`;
    }
    this.eventService
      .updateEventDetails(this.eventId, this.eventDetails)
      .subscribe({
        next: (res) => {
          this.isSuccess = 1;
          this.toast.success({
            detail: 'SUCCESS',
            summary: 'Wydarzenie zostało zaktualizowane!',
            duration: 3000,
          });
        },
        error: (error) => {
          this.isSuccess = -1;
          this.toast.error({
            detail: 'ERROR',
            summary: 'Błąd podczas aktualizowania wydarzenia',
            duration: 3000,
          });
        },
      });
  }

  imagesNum: number = 0; //liczba wszystkich zdjęć
  loadImages() {
    this.galleryService.GetImagesFromEvent(Number(this.eventId)).subscribe({
      next: (res) => {
        this.images = res;
        this.numVisible = Math.min(this.maxNumVisible, this.images.length);
        this.imagesNum = this.images.length;
      },
      error: (error) => {
        console.error('Error fetching images', error);
      },
    });
  }

  @ViewChild('fileUploader') fileUploader: FileUpload | undefined;

  uploadFile(event: any) {
    // event.files zawiera wybrane pliki
    const formData: FormData = new FormData();
    formData.append('file', event.files[0], event.files[0].name);
    formData.append('eventId', this.eventId.toString());
    formData.append('userId', this.userRole.id.toString());

    this.galleryService.UploadImage(formData).subscribe({
      next: (res) => {
        this.toast.success({
          detail: 'SUCCESS',
          summary: 'Plik został przesłany!',
          duration: 3000,
        });
        this.images.push(res);
        this.numVisible = Math.min(this.maxNumVisible, this.images.length);
        this.imagesNum = this.images.length;
        if (this.fileUploader) {
          this.fileUploader.clear();
        }
      },
      error: (error) => {
        this.toast.error({
          detail: 'ERROR',
          summary: 'Błąd podczas przesyłania pliku',
          duration: 3000,
        });
      },
    });
  }

  onUpload(event: any) {
    this.fileUploader?.clear();
  }

  //ShoppingList
  loadShoppingList() {
    this.shoppingListService.getShoppingList(this.eventDetails.id).subscribe(
      (shoppingList: ShoppingListItemDto[]) => {
        this.shoppingList = shoppingList.map((item: ShoppingListItemDto) => {
          const user = this.guestsUsers.find((user) => user.id === item.userId);
          return {
            ...item,
            firstName: user ? user.firstName : '',
            lastName: user ? user.lastName : '',
          };
        });
      },
      (error) => {
        console.error('Wystąpił błąd podczas ładowania listy zakupów', error);
      }
    );
  }

  addItem() {
    if (this.newItemName && this.newItemQuantity > 0) {
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
      .deleteItem(Number(this.eventDetails.id), itemId)
      .subscribe(
        (response) => {
          console.log('Przedmiot został usunięty', response);
          // Aktualizacja lokalnej listy przedmiotów - usuń przedmiot z listy
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

  //Dialogs
  openDialog() {
    const dialogRef = this.dialog.open(InviteModalComponent, {
      data: {
        eventId: this.eventId,
        guests: this.guestsUsers.slice(1),
        invites: this.invitesUsers,
      },
      panelClass: 'inviteDialog',
      backdropClass: 'dialogBackgroundClass',
    });

    dialogRef.afterClosed().subscribe((res) => {
      this.loadInvitedUsers();
      this.loadGuestsUsers(); //poprawic to
    });
  }

  openAddContentModal(): void {
    const dialogRef = this.dialog.open(ExtraFunctionsModalComponent, {
      height: '330px',
      width: '600px',
      data: {
        eventId: this.eventDetails.id,
        hasShoppingList: !!this.eventDetails.shoppingListTitle,
        hasPlaylist: !!this.eventDetails.playlistTitle,
        hasReceipt: !!this.eventDetails.receiptTitle,
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      console.log('The dialog was closed with result:', result);
      this.eventService.getEventDetails(this.eventId).subscribe({
        next: (res) => {
          this.eventDetails = res;
        },
      });
    });
  }

  openMapModal() {
    const addressToGeocode =
      this.eventDetails.address + ', ' + this.eventDetails.city;

    if (addressToGeocode) {
      const provider = new leafletGeosearch.OpenStreetMapProvider();
      provider
        .search({ query: addressToGeocode })
        .then((result: any) => {
          if (result.length > 0) {
            const dialogRef = this.dialog.open(MapComponent, {
              width: '80vw', // lub inny rozmiar
              data: {
                eventAddress:
                  this.eventDetails.address + ', ' + this.eventDetails.city,
                coordinates: result[0], // przekazanie koordynatów do komponentu MapComponent
              },
            });
          } else {
            this.toast.error({
              detail: 'Mapa nie potrafiła znaleźć tego adresu.',
              duration: 3000,
            });
            console.error('Mapa nie potrafiła znaleźć tego adresu.');
          }
        })
        .catch((error: any) => {
          console.error('Błąd podczas geokodowania:', error);
          console.error('Mapa nie potrafiła znaleźć tego adresu.');
          this.toast.error({
            detail: 'Mapa nie potrafiła znaleźć tego adresu.',
            duration: 3000,
          });
        });
    }
  }

  //SingalR
  addToEventGroup() {
    if (
      this.signalRService.hubConnection.state ===
      signalR.HubConnectionState.Connected
    ) {
      this.signalRService.hubConnection
        .invoke('AddToEventGroup', this.eventId)
        .catch((err) => console.error(err));
    }
  }

  removeFromEventGroup() {
    if (
      this.signalRService.hubConnection.state ===
      signalR.HubConnectionState.Connected
    ) {
      this.signalRService.hubConnection
        .invoke('RemoveFromEventGroup', this.eventId)
        .catch((err) => console.error(err));
    }
  }
  deleteEvent() {
    const dialogRef = this.dialog.open(ConfirmDialogComponent);

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.eventService.deleteEvent(this.eventId).subscribe(
          (response) => {
            console.log('Wydarzenie zostało usunięte', response);
            this.router.navigate([`wydarzenia`]);
          },
          (error) => {
            console.error('Wystąpił błąd podczas usuwania wydarzenia', error);
          }
        );
      }
    });
  }

  cancelEdit() {
    this.editMode = false;
    this.editField = null;
  }
}
