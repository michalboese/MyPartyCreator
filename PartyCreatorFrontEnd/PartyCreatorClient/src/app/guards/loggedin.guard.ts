import { ActivatedRouteSnapshot, CanActivateFn, Router, RouterStateSnapshot } from '@angular/router';
import { inject } from '@angular/core'
import { AuthService } from '../services/auth.service';
import { NgToastService } from 'ng-angular-popup';

export const loggedinGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
  if(inject(AuthService).isLoggedIn())
  {
    inject(NgToastService).success({detail:"SUCCES",summary:"Jestes już zalogowany!"});
    inject(Router).navigate(['wydarzenia']);
    return false;
  }else {
    return true;
  }
};
