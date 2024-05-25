import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { map } from 'rxjs';
import { AccountService } from 'src/app/account/account.service';
import { ApplicationUser } from 'src/app/shared/models/account/applicationUser';

export const playerOrGuestGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const router = inject(Router);

  return accountService.applicationUser$.pipe(
    map((user: ApplicationUser | null) => {
      if (user) {
        return true;
      } else {
        router.navigate(['account/login'], { queryParams: { returnUrl: state.url } });
        return false;
      }
    }));
};
