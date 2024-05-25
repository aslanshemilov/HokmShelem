import { Component, OnInit } from '@angular/core';
import { AccountService } from '../account.service';
import { map, of, switchMap, take, timer } from 'rxjs';
import { Router, ActivatedRoute } from '@angular/router';
import { ApplicationUser } from 'src/app/shared/models/account/applicationUser';
import { AbstractControl, AsyncValidatorFn, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { RegisterWithExternal } from 'src/app/shared/models/account/registerWithExternal';
import { SharedService } from 'src/app/shared/shared.service';
import { Country } from 'src/app/shared/models/hokmshelem/country';
import { ApiResponse } from 'src/app/shared/models/apiResponse';

@Component({
  selector: 'app-register-with-third-party',
  templateUrl: './register-with-third-party.component.html',
  styleUrls: ['./register-with-third-party.component.scss']
})
export class RegisterWithThirdPartyComponent implements OnInit {
  registerForm: FormGroup = new FormGroup({});
  countries!: Country[] | null;
  submitted = false;
  provider: string | null = null;
  access_token: string = '';
  userId: string | null = null;
  apiResponse: ApiResponse | undefined;
  complete = false;

  constructor(private accountService: AccountService,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private formBuilder: FormBuilder,
    public sharedService: SharedService) {}

  ngOnInit(): void {
    this.accountService.applicationUser$.pipe(take(1)).subscribe({
      next: (user: ApplicationUser | null) => {
        if (user && user.roles.length > 0) {
          this.router.navigateByUrl('/');
        } else {
          this.activatedRoute.queryParamMap.subscribe({
            next: (params: any) => {
              this.provider = this.activatedRoute.snapshot.paramMap.get('provider');
              this.access_token = params.get('access_token');
              this.userId = params.get('userId');

              if (this.provider && this.access_token && this.userId &&
                (this.provider === 'facebook' || this.provider === 'google')) {
                this.initializeForm();
                this.sharedService.getCountries().subscribe({
                  next: countries => {
                    this.countries = countries
                    this.complete = true;
                  }
                });

              } else {
                this.router.navigateByUrl('/account/register');
              }
            }
          })
        }
      }
    })
  }

  initializeForm() {
    this.registerForm = this.formBuilder.group({
      playerName: ['', [Validators.required, Validators.pattern('^[a-zA-Z0-9_.-]*$'), Validators.minLength(3), Validators.maxLength(15)], [this.checkPlayerNameNotTaken()]],
      countryId: [0, [Validators.required, this.countrySelected()]]
    });
  }

  register() {
    this.submitted = true;
    this.apiResponse = undefined;

    if (this.registerForm.valid && this.userId && this.access_token && this.provider) {
      const playerName = this.registerForm.get('playerName')?.value;
      const countryId = this.registerForm.get('countryId')?.value;

      const model = new RegisterWithExternal(playerName, this.userId, this.access_token, this.provider, countryId);
      this.accountService.registerWithThirdParty(model).subscribe({
        next: _ => {
          this.router.navigateByUrl('/');
        }, error: error => {
          this.apiResponse = error;
        }
      })
    }
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
