import { Component, OnInit, ElementRef, ViewChild, Inject, Renderer2 } from "@angular/core";
import { FormGroup, FormBuilder, Validators, AbstractControl, ValidatorFn, AsyncValidatorFn } from "@angular/forms";
import { Router } from "@angular/router";
import { CredentialResponse } from "google-one-tap";
import { ApiResponse } from "src/app/shared/models/apiResponse";
import { AccountService } from "../account.service";
import { DOCUMENT } from "@angular/common";
import { environment } from "src/environments/environment";
import { SharedService } from "src/app/shared/shared.service";
import jwt_decode from 'jwt-decode';
import { map, of, switchMap, take, timer } from "rxjs";
import { ApplicationUser } from "src/app/shared/models/account/applicationUser";
import { Country } from "src/app/shared/models/hokmshelem/country";
import { renderGoogleButton } from "src/app/shared/sharedHelper";
declare const FB: any;

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {
  @ViewChild('googleButton', { static: true }) googleButton: ElementRef = new ElementRef({});
  registerForm: FormGroup = new FormGroup({});
  countries!: Country[] | null;
  submitted = false;
  apiResponse: ApiResponse | undefined;
  returnUrl: string | null = null;

  constructor(private formBuilder: FormBuilder,
    private accountService: AccountService,
    private router: Router,
    public sharedService: SharedService,
    private _renderer2: Renderer2,
    @Inject(DOCUMENT) private _document: Document) {
    this.accountService.applicationUser$.pipe(take(1)).subscribe({
      next: (user: ApplicationUser | null) => {
        if (user) {
          this.router.navigateByUrl('/');
        }
      }
    })
  }

  ngOnInit(): void {
    this.initializeGoogleButton();
    this.initializeForm();
    this.sharedService.getCountries().subscribe({
      next: countries => {
        this.countries = countries;
      } 
    });
  }

  ngAfterViewInit() {
    renderGoogleButton(this._renderer2, this._document);
  }

  initializeForm() {
    this.registerForm = this.formBuilder.group({
      playerName: ['', [Validators.required, Validators.pattern('^[a-zA-Z0-9_.-]*$'), Validators.minLength(3), Validators.maxLength(15)], [this.checkPlayerNameNotTaken()]],
      email: ['', [Validators.required, Validators.pattern('^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$')]],
      password: ['', [Validators.required, Validators.pattern('^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{6,15}$')]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]],
      countryId: [0, [Validators.required, this.countrySelected()]]
    });

    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () => this.registerForm.controls['confirmPassword'].updateValueAndValidity()
    })
  }

  register() {
    this.submitted = true;
    this.apiResponse = undefined;

    if (this.registerForm.valid) {
      this.accountService.register(this.registerForm.value).subscribe({
        next: (response: ApiResponse) => {
          this.sharedService.showNotification(true, response.title, response.message);
          this.router.navigateByUrl('/account/login');
        },
        error: error => {
          this.apiResponse = error;
        }
      })
    }
  }

  registerWithFacebook() {
    FB.login(async (fbResult: any) => {
      if (fbResult.authResponse) {
        const accessToken = fbResult.authResponse.accessToken;
        const userId = fbResult.authResponse.userID;
        this.router.navigateByUrl(`/account/register/third-party/facebook?access_token=${accessToken}&userId=${userId}`);
      } else {
        this.sharedService.showNotification(false, "Failed", "Unable to register with your facebook account.");
      }
    })
  }

  private  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value ? null : { notMatching: true };
    }
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
        { size: "medium", shape: "rectangular", text: "signup_with", logo_alignment: "center" },
      );
    };
  }

  private async googleCallBack(response: CredentialResponse) {
    const decodedToken: any = jwt_decode(response.credential);
    this.router.navigateByUrl(`/account/register/third-party/google?access_token=${response.credential}&userId=${decodedToken.sub}`);
  }

  private countrySelected(): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value !== 0 ? null : { noCountrySelected: true };
    }
  }

  private checkPlayerNameNotTaken(): AsyncValidatorFn {
    return control => {
      return timer(500).pipe(
        switchMap(_ => {
          if (!control.value) {
            return of(null);
          }
          return this.accountService.checkPlayerNameTaken(control.value).pipe(
            map(res => {
              if (res === true) {
                return { playerNameTaken: true };
              }

              return null;
            })
          );
        })
      )
    }
  }
}
