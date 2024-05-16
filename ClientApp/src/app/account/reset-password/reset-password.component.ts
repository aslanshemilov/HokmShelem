import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ValidatorFn, AbstractControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { take } from 'rxjs';
import { ApplicationUser } from 'src/app/shared/models/account/applicationUser';
import { ResetPassword } from 'src/app/shared/models/account/resetPassword';
import { AccountService } from '../account.service';
import { ApiResponse } from 'src/app/shared/models/apiResponse';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent implements OnInit {
  requestCompleted = false;
  resetPasswordForm: FormGroup = new FormGroup({});
  token: string = '';
  submitted = false;
  apiResponse: ApiResponse | undefined;

  constructor(private accountService: AccountService,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private sharedService: SharedService,
    private formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.accountService.applicationUser$.pipe((take(1))).subscribe({
      next: (applicationUser: ApplicationUser | null) => {
        if (applicationUser) {
          this.router.navigateByUrl('/');
        } else {
          this.activatedRoute.queryParamMap.subscribe({
            next: (params: any) => {
              this.token = params.get('token');
              this.initializeForm(params.get('username'), params.get('email'));
              this.requestCompleted = true;
            }
          })
        }
      }
    })
  }

  initializeForm(username: string, email: string) {
    this.resetPasswordForm = this.formBuilder.group({
      username: [{ value: username, disabled: true }],
      email: [{ value: email, disabled: true }],
      newPassword: ['', [Validators.required, Validators.pattern('^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{6,15}$')]],
      confirmPassword: ['', [Validators.required, this.matchValues('newPassword')]],
    });

    this.resetPasswordForm.controls['newPassword'].valueChanges.subscribe({
      next: () => this.resetPasswordForm.controls['confirmPassword'].updateValueAndValidity()
    })
  }

  resetPassword() {
    this.submitted = true;
    this.apiResponse = undefined;

    if (this.resetPasswordForm.valid) {

      const model: ResetPassword = {
        token: this.token,
        email: this.resetPasswordForm.get('email')?.value,
        newPassword: this.resetPasswordForm.get('newPassword')?.value
      };

      this.accountService.resetPassword(model).subscribe({
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

  cancel() {
    this.router.navigateByUrl('/account/login');
  }

  private  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value ? null : { notMatching: true };
    }
  }
}
