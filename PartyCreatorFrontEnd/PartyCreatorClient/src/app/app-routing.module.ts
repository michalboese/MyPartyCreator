import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { MainComponent } from './main/main.component';
import { EventViewComponent } from './event-view/event-view.component';
import { authGuard } from './guards/auth.guard';
import { loggedinGuard } from './guards/loggedin.guard';
import { ProfileComponent } from './profile/profile.component';
import { UserProfileComponent } from './profile/user-profile/user-profile.component';
import { RegulationsComponent } from './login/signup/regulations/regulations.component';
import { RedirectComponent } from './redirect/redirect.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'redirect', component: RedirectComponent },
  {
    path: 'logowanie',
    component: LoginComponent,
    canActivate: [loggedinGuard],
  },
  { path: 'wydarzenia', component: MainComponent, canActivate: [authGuard] },
  { path: 'profil', component: ProfileComponent, canActivate: [authGuard] },
  {
    path: 'profil/:id',
    component: UserProfileComponent,
    canActivate: [authGuard],
  },
  {
    path: 'wydarzenie/:id',
    component: EventViewComponent,
    canActivate: [authGuard],
  },
  {
    path: 'regulamin',
    component: RegulationsComponent,
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { onSameUrlNavigation: 'reload' })],
  exports: [RouterModule],
})
export class AppRoutingModule {}
