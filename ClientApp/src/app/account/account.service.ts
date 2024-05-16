import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { of, map, ReplaySubject, take } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ApplicationUser } from '../shared/models/account/applicationUser';
import { Login } from '../shared/models/account/login';
import { Register } from '../shared/models/account/register';
import { ConfirmEmail } from '../shared/models/account/confirmEmail';
import { ResetPassword } from '../shared/models/account/resetPassword';
import { ApiResponse } from '../shared/models/apiResponse';
import { RegisterWithExternal } from '../shared/models/account/registerWithExternal';
import { LoginWithExternal } from '../shared/models/account/loginWithExternal';
import jwt_decode from 'jwt-decode';
import { SharedService } from '../shared/shared.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  apiUrl = environment.apiUrl;
  engineUrl = environment.engineUrl;

  private applicationUserSource = new ReplaySubject<ApplicationUser | null>(1);
  applicationUser$ = this.applicationUserSource.asObservable();

  refreshTokenTimeout: any;
  timeoutId: any;

  constructor(private http: HttpClient,
    private router: Router,
    private sharedService: SharedService) { }

  refreshToken = async () => {
    this.http.get<ApplicationUser>(this.apiUrl + 'account/refresh-applicationUser')
      .subscribe({
        next: (user: ApplicationUser) => {
          if (user) {
            this.setApplicationUser(user);
          }
        }, error: error => {
          this.sharedService.showNotification(false, 'Error', error.error);
          this.logout();
        }
      })
  }

  refreshApplicationUser(jwt: string | null) {
    if (jwt === null) {
      this.applicationUserSource.next(null);
      return of(undefined);
    }

    let headers = new HttpHeaders();
    headers = headers.set('Authorization', 'Bearer ' + jwt);

    return this.http.get<ApplicationUser>(this.apiUrl + 'account/refresh-applicationUser', { headers }).pipe(
      map((user: ApplicationUser) => {
        if (user) {
          this.setApplicationUser(user);
        }
      })
    )
  }

  login(model: Login) {
    return this.http.post<ApplicationUser>(this.apiUrl + 'account/login', model).pipe(
      map((user: ApplicationUser) => {
        if (user) {
          this.setApplicationUser(user);
        }
      })
    );
  }

  loginWithThirdParty(model: LoginWithExternal) {
    return this.http.post<ApplicationUser>(this.apiUrl + 'account/login-with-third-party', model).pipe(
      map((user: ApplicationUser) => {
        if (user) {
          this.setApplicationUser(user);
        }
      })
    )
  }

  logout() {
    localStorage.removeItem(environment.applicationUserKey);
    this.applicationUserSource.next(null);
    this.router.navigateByUrl('/');
    this.stopRefreshTokenTimer();
  }

  register(model: Register) {
    return this.http.post<ApiResponse>(this.apiUrl + 'account/register', model);
  }

  registerWithThirdParty(model: RegisterWithExternal) {
    return this.http.post<ApplicationUser>(this.apiUrl + 'account/register-with-third-party', model).pipe(
      map((user: ApplicationUser) => {
        if (user) {
          this.setApplicationUser(user);
          this.router.navigateByUrl('/');
        }
      })
    );
  }

  checkPlayerNameTaken(playerName: string) {
    return this.http.get(this.apiUrl + 'account/playername-taken?playerName=' + playerName);
  }

  confirmEmail(model: ConfirmEmail) {
    return this.http.put<ApiResponse>(this.apiUrl + 'account/confirm-email', model);
  }

  resendEmailConfirmationLink(email: string) {
    return this.http.post<ApiResponse>(this.apiUrl + 'account/resend-email-confirmation-link/' + email, {});
  }

  forgotUsernameOrPassword(email: string) {
    return this.http.post<ApiResponse>(this.apiUrl + 'account/forgot-username-or-password/' + email, {});
  }

  resetPassword(model: ResetPassword) {
    return this.http.put<ApiResponse>(this.apiUrl + 'account/reset-password', model);
  }

  getJwt() {
    const key = localStorage.getItem(environment.applicationUserKey);
    if (key) {
      const applicationUser: ApplicationUser = JSON.parse(key);
      return applicationUser.jwt;
    }

    return null;
  }

  checkUserIdleTimout() {
    this.applicationUser$.pipe(take(1)).subscribe({
      next: (user: ApplicationUser | null) => {
        // the user is logged in
        if (user) {
          // if not currently dipsplaying expiring session modal
          if (!this.sharedService.displayingExpiringSessionModal) {
            this.timeoutId = setTimeout(() => {
              this.sharedService.displayingExpiringSessionModal = true;
              this.sharedService.openExpiringSessionCountdown();
              // in 10 minutes of user incativity
            }, environment.allowedInactivityTimeInMinutes * 60 * 1000);
          }
        }
      }
    })
  }

  private setApplicationUser(user: ApplicationUser) {
    this.http.get(this.engineUrl + 'environment/engine-ready', {responseType: 'text'}).subscribe();
    this.stopRefreshTokenTimer();
    this.startRefreshTokenTimer(user.jwt);
    localStorage.setItem(environment.applicationUserKey, JSON.stringify(user));
    this.applicationUserSource.next(user);

    this.sharedService.displayingExpiringSessionModal = false;
    this.checkUserIdleTimout();
  }

  private startRefreshTokenTimer(jwt: string) {
    const decodedToken: any = jwt_decode(jwt);
    // expires in seconds
    const expires = new Date(decodedToken.exp * 1000);
    // 60 seconds before the expiration calls refreshToken
    const timeout = expires.getTime() - Date.now() - (60 * 1000);
    this.refreshTokenTimeout = setTimeout(this.refreshToken, timeout);
  }

  private stopRefreshTokenTimer() {
    clearTimeout(this.refreshTokenTimeout);
  }
}
