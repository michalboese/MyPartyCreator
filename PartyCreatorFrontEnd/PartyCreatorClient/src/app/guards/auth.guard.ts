import { ActivatedRouteSnapshot, CanActivateFn, Router, RouterStateSnapshot } from '@angular/router';
import { inject } from '@angular/core'
import { AuthService } from '../services/auth.service';
import { NgToastService } from 'ng-angular-popup';

export const authGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
  if(inject(AuthService).isLoggedIn())
  {
    return true;
  }else{
    inject(NgToastService).error({detail:"ERROR",summary:"Musisz się najpierw zalogować"});
    inject(Router).navigate(['logowanie']);
    return false;
  }
};
