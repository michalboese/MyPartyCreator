import { LOCALE_ID, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { FormsModule } from '@angular/forms';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { SignupComponent } from './login/signup/signup.component';
import { SigninComponent } from './login/signin/signin.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { MainComponent } from './main/main.component';
import { NavMenuMainComponent } from './nav-menu-main/nav-menu-main.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatCardModule } from '@angular/material/card';
import { MatNativeDateModule } from '@angular/material/core';
import { MAT_DATE_LOCALE } from '@angular/material/core';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgToastModule } from 'ng-angular-popup';
import { TokenInterceptor } from './interceptors/token.interceptor';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { EventViewComponent } from './event-view/event-view.component';
import { ProfileComponent } from './profile/profile.component';
import { MatTabsModule } from '@angular/material/tabs';
import { MatIconModule } from '@angular/material/icon';
import { ProfileViewComponent } from './profile/profile-view/profile-view.component';
import { ProfileEditComponent } from './profile/profile-edit/profile-edit.component';
import { registerLocaleData } from '@angular/common';
import localePl from '@angular/common/locales/pl';
import { ProfileContactsComponent } from './profile/profile-contacts/profile-contacts.component';
import { AddContactDialogComponent } from './profile/add-contact-dialog/add-contact-dialog.component';
import { DatePipe } from '@angular/common';
import { MapComponent } from './map/map.component';
import { MatMenuModule } from '@angular/material/menu';
import { ProfileEditAvatarComponent } from './profile/profile-edit/profile-edit-avatar/profile-edit-avatar.component';
import { CalendarModule, DateAdapter } from 'angular-calendar';
import { adapterFactory } from 'angular-calendar/date-adapters/date-fns';
import { MainCalendarComponent } from './main/main-calendar/main-calendar.component';
import { MatTooltipModule } from '@angular/material/tooltip';
import { UserProfileComponent } from './profile/user-profile/user-profile.component';
import { MatInputModule } from '@angular/material/input';
import { ExtraFunctionsModalComponent } from './extra-functions-modal/extra-functions-modal.component';
import { ChatComponent } from './event-view/chat/chat.component';
import { SplitBillComponent } from './event-view/split-bill/split-bill.component';
import { EventGalleryComponent } from './event-view/event-gallery/event-gallery.component';
import { FileUploadModule } from 'primeng/fileupload';
import { RegulationsComponent } from './login/signup/regulations/regulations.component';
import { RedirectComponent } from './redirect/redirect.component';
import { SpotifyComponent } from './event-view/spotify/spotify/spotify.component';
import { ShoppingListComponent } from './event-view/shopping-list/shopping-list.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { SurveyComponent } from './event-view/survey/survey.component';
import {MatExpansionModule} from '@angular/material/expansion';
import {MatFormFieldModule} from '@angular/material/form-field';
import { MatRadioModule } from '@angular/material/radio';
import { ConfirmDialogComponent } from './event-view/confirm-dialog/confirm-dialog.component';
import { ConfirmRegistrationDialogComponent } from './login/signup/confirm-registration-dialog/confirm-registration-dialog.component';

registerLocaleData(localePl);

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    LoginComponent,
    SignupComponent,
    SigninComponent,
    EventViewComponent,
    ProfileComponent,
    MapComponent,
    SplitBillComponent,
    RegulationsComponent,
    ShoppingListComponent,
    SurveyComponent,
    ConfirmDialogComponent,
    ConfirmRegistrationDialogComponent,
  ],

  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    FontAwesomeModule,
    BrowserAnimationsModule,
    MatDatepickerModule,
    MatCardModule,
    MatNativeDateModule,
    HttpClientModule,
    NgToastModule,
    MatButtonModule,
    MatDialogModule,
    MatTabsModule,
    MatIconModule,
    ProfileEditComponent,
    DatePipe,
    ProfileViewComponent,
    ProfileContactsComponent,
    AddContactDialogComponent,
    MatMenuModule,
    ProfileEditAvatarComponent,
    MainComponent,
    NavMenuMainComponent,
    FileUploadModule,
    CalendarModule.forRoot({
      provide: DateAdapter,
      useFactory: adapterFactory,
    }),
    MainCalendarComponent,
    MatTooltipModule,
    UserProfileComponent,
    MatInputModule,
    ChatComponent,
    EventGalleryComponent,
    MatExpansionModule,
    MatIconModule,
    MatFormFieldModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatRadioModule,
    RedirectComponent,
    SpotifyComponent,
    MatProgressSpinnerModule,
  ],
  providers: [
    { provide: LOCALE_ID, useValue: 'pl-PL' },
    { provide: MAT_DATE_LOCALE, useValue: 'pl-PL' },
    { provide: HTTP_INTERCEPTORS, useClass: TokenInterceptor, multi: true },
    [DatePipe],
  ],

  bootstrap: [AppComponent],
})
export class AppModule {}
