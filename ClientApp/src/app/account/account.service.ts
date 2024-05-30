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
import { GameService } from '../game/game.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  apiUrl = environment.apiUrl;
  engineUrl = environment.engineUrl;
  isGuestUser = false;

  private applicationUserSource = new ReplaySubject<ApplicationUser | null>(1);
  applicationUser$ = this.applicationUserSource.asObservable();

  playerName: string | undefined;

  refreshTokenTimeout: any;
  timeoutId: any;

  constructor(private http: HttpClient,
    private router: Router,
    private sharedService: SharedService,
    private gameService: GameService) { }

  refreshToken = async () => {
    this.http.get<ApplicationUser>(this.apiUrl + (this.isGuestUser === false ? 'account/refresh-applicationUser' : 'guest'))
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

    const decodedToken: any = jwt_decode(jwt);
    const roles = decodedToken.role;

    if (!roles) {
      this.isGuestUser = true;
    }

    let headers = new HttpHeaders();
    headers = headers.set('Authorization', 'Bearer ' + jwt);

    return this.http.get<ApplicationUser>(this.apiUrl + (this.isGuestUser === false ? 'account/refresh-applicationUser' : 'guest')
      , { headers }).pipe(
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
    this.isGuestUser = false;
    this.applicationUserSource.next(null);
    this.playerName = undefined;
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

  registerAsGuest() {
    return this.http.post<ApplicationUser | null>(this.apiUrl + 'guest', {}).pipe(
      map((user: ApplicationUser | null) => {
        if (user) {
          this.isGuestUser = true;
          this.setApplicationUser(user);
          return user;
        }

        return null;
      })
    );
  }

  checkUserIdleTimout() {
    this.applicationUser$.pipe(take(1)).subscribe({
      next: (user: ApplicationUser | null) => {
        if (user) {
          // if not currently dipsplaying expiring session modal
          if (!this.sharedService.displayingExpiringSessionModal) {
            this.timeoutId = setTimeout(() => {
              this.sharedService.displayingExpiringSessionModal = true;
              this.sharedService.openExpiringSessionCountdown();
              // in some minutes of user incativity
            }, environment.allowedInactivityTimeInMinutes * 60 * 1000);
          }
        }
      }
    })
  }

  private setApplicationUser(user: ApplicationUser) {
    this.http.get(this.engineUrl + 'environment/engine-ready', { responseType: 'text' }).subscribe();
    const decodedToken: any = jwt_decode(user.jwt);
    const roles = decodedToken.role;
    let userToAdd = new ApplicationUser(user.playerName, user.photoUrl, roles, user.jwt);
    localStorage.setItem(environment.applicationUserKey, JSON.stringify(userToAdd));
    this.applicationUserSource.next(userToAdd)
    this.playerName = user.playerName;
    this.stopRefreshTokenTimer();
    this.startRefreshTokenTimer(decodedToken.exp);
    this.sharedService.displayingExpiringSessionModal = false;
    this.checkUserIdleTimout();
    this.handleGameLostConnectionIfAny();
  }

  private startRefreshTokenTimer(exp: number) {
    const expiresInSeconds = new Date(exp * 1000).getTime();
    // 60 seconds before the expiration calls refreshToken
    const timeout = expiresInSeconds - Date.now() - (60 * 1000);
    this.refreshTokenTimeout = setTimeout(this.refreshToken, timeout);
  }

  private stopRefreshTokenTimer() {
    clearTimeout(this.refreshTokenTimeout);
  }

  private handleGameLostConnectionIfAny() {
    this.gameService.getCurrentGame().subscribe({
      next: gameName => {
        if (gameName) {
          if (this.router.url != '/game/hokm') {
            this.router.navigateByUrl('/');
            var result = this.sharedService.confirmBox('warning', 'Lost Connection', 'You have an ongoing game. Would you like to reconnect?');
            result.subscribe({
              next: (answer) => {
                if (answer) {
                  this.router.navigateByUrl('/game/hokm');
                } else {
                   this.gameService.leaveTheGameApi(gameName).subscribe({
                    next: _ => {

                    },
                    error: error => {
                    }
                   })
                }
              }
            })
          }
        }
      }
    })
  }
}
