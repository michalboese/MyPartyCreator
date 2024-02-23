import { Component, OnInit } from '@angular/core';
//icons
import { faSpotify } from '@fortawesome/free-brands-svg-icons';
import {
  faAnglesDown,
  faCalendarPlus,
  faPhotoFilm,
  faMoneyBillTransfer,
  faUserPlus,
  faSquarePollVertical,
  faPersonCircleQuestion,
  faListCheck,
  faComments,
} from '@fortawesome/free-solid-svg-icons';
import { AuthService } from '../services/auth.service';
//icons
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  title = 'PartyCreatorClient';
  //icons
  faSpotify = faSpotify; //ikonka spotify
  faMoney = faMoneyBillTransfer;
  faCalendar = faCalendarPlus;
  faPhoto = faPhotoFilm;
  faUser = faUserPlus;
  faPoll = faSquarePollVertical;
  faPerson = faPersonCircleQuestion;
  faList = faListCheck;
  faComments = faComments;
  faArrowDown = faAnglesDown;
  //end of icons
  user = '';
  message = '';
  messages: { user: string; text: string }[] = [];
  isLoggedIn = false;

  constructor(private auth: AuthService) {}

  ngOnInit(): void {
    this.auth.changeIsLoggedInValue();
    this.isLoggedIn = this.auth.isLoggedIn();
  }

  scrollToElement($element: HTMLElement): void {
    console.log($element);
    $element.scrollIntoView({
      behavior: 'smooth',
      block: 'start',
      inline: 'nearest',
    });
  }

  changeLoginType() {
    this.auth.changeLoginType('signup');
  }
}
