import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { map } from 'rxjs';
import { AccountService } from 'src/app/account/account.service';
import { ApplicationUser } from 'src/app/shared/models/account/applicationUser';
import jwt_decode from 'jwt-decode';
import { SharedService } from 'src/app/shared/shared.service';

export const adminGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const sharedService = inject(SharedService);
  const router = inject(Router);

  return accountService.applicationUser$.pipe(
    map((user: ApplicationUser | null) => {
      if (user) {
        const decodedToken: any = jwt_decode(user.jwt);
        if (decodedToken.role.includes('admin')) {
          return true;
        }
      }

      sharedService.showNotification(false, 'Restricted Area', 'You do not have the permission to visit this page.');
      router.navigate(['/']);
      return false;
    })
  )
};
