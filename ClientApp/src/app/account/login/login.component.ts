import { Component, ElementRef, Inject, Renderer2, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { take } from 'rxjs';
import { AccountService } from '../account.service';
import { ApplicationUser } from 'src/app/shared/models/account/applicationUser';
import { environment } from 'src/environments/environment';
import { CredentialResponse } from 'google-one-tap';
import { DOCUMENT } from '@angular/common';
import { LoginWithExternal } from 'src/app/shared/models/account/loginWithExternal';
import { SharedService } from 'src/app/shared/shared.service';
import jwt_decode from 'jwt-decode';
import { ApiResponse } from 'src/app/shared/models/apiResponse';
import { renderGoogleButton } from 'src/app/shared/sharedHelper';
declare const FB: any;

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  @ViewChild('googleButton', { static: true }) googleButton: ElementRef = new ElementRef({});
  loginForm: FormGroup = new FormGroup({});
  submitted = false;
  apiResponse: ApiResponse | undefined;
  returnUrl: string | null = null;

  constructor(private formBuilder: FormBuilder,
    private accountService: AccountService,
    private sharedService: SharedService,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private _renderer2: Renderer2,
    @Inject(DOCUMENT) private _document: Document) {
    
  }

  ngOnInit(): void {
    this.accountService.applicationUser$.pipe(take(1)).subscribe(
      (user: ApplicationUser | null) => {
        if (user && user.roles.length > 0) {
          this.router.navigateByUrl('/');
        } else {
          this.activatedRoute.queryParamMap.subscribe({
            next: (params: any) => {
              if (params) {
                this.returnUrl = params.get('returnUrl');
              }
            }
          });
        }
      }
    )
    
    this.initializeGoogleButton();
    this.initializeForm();
  }

  ngAfterViewInit() {
    renderGoogleButton(this._renderer2, this._document);
  }

  initializeForm() {
    this.loginForm = this.formBuilder.group({
      userName: ['', Validators.required],
      password: ['', Validators.required],
    })
  }

  login() {
    this.submitted = true;
    this.apiResponse = undefined;

    if (this.loginForm.valid) {
      this.accountService.login(this.loginForm.value).subscribe({
        next: _ => {
          if (this.returnUrl) {
            this.router.navigateByUrl(this.returnUrl);
          } else {
            this.router.navigateByUrl('/');
          }
        },
        error: error => {
          this.apiResponse = error;
        }
      })
    }
  }

  loginWithFacebook() {
    FB.login(async (fbResult: any) => {
      if (fbResult.authResponse) {
        const accessToken = fbResult.authResponse.accessToken;
        const userId = fbResult.authResponse.userID;

        this.accountService.loginWithThirdParty(new LoginWithExternal(accessToken, userId, "facebook")).subscribe({
          next: () => {
            if (this.returnUrl) {
              this.router.navigateByUrl(this.returnUrl);
            } else {
              this.router.navigateByUrl('/');
            }
          },
          error: error => {
            this.apiResponse = error;
          }
        })
      } else {
        this.sharedService.showNotification(false, "Failed", "Unable to login with your facebook");
      }
    })
  }

  resendEmailConfirmationLink() {
    this.router.navigateByUrl('/account/send-email/resend-email-confirmation-link');
  }

  private initializeGoogleButton() {
    (window as any).onGoogleLibraryLoad = () => {
      // @ts-ignore
      google.accounts.id.initialize({
        client_id: environment.googleCLientId,
        callback: this.googleCallBack.bind(this),
        auto_select: false,
        cancel_on_tap_outside: true
      });
      // @ts-ignore
      google.accounts.id.renderButton(
        this.googleButton.nativeElement,
        { size: "medium", shape: "rectangular", text: "signin_with", logo_alignment: "center" },
      );
    };
  }

  private async googleCallBack(response: CredentialResponse) {
    const decodedToken: any = jwt_decode(response.credential);
    this.accountService.loginWithThirdParty(new LoginWithExternal(response.credential, decodedToken.sub, "google"))
      .subscribe({
        next: _ => {
          if (this.returnUrl) {
            this.router.navigateByUrl(this.returnUrl);
          } else {
            this.router.navigateByUrl('/');
          }
        },
        error: error => {
          this.apiResponse = error;
        }
      })
  }
}
