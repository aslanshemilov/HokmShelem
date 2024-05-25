import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { take } from 'rxjs';
import { ApplicationUser } from 'src/app/shared/models/account/applicationUser';
import { AccountService } from '../account.service';
import { ApiResponse } from 'src/app/shared/models/apiResponse';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-send-email',
  templateUrl: './send-email.component.html',
  styleUrls: ['./send-email.component.scss']
})
export class SendEmailComponent implements OnInit {
  @ViewChild('myDiv', { static: true }) myDiv?: ElementRef;
  requestCompleted = false;
  emailForm: FormGroup = new FormGroup({});
  submitted = false;
  mode: string = '';
  apiResponse: ApiResponse | undefined;

  constructor(private formBuilder: FormBuilder,
    private accountService: AccountService,
    private sharedService: SharedService,
    private router: Router,
    private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.accountService.applicationUser$.pipe((take(1))).subscribe({
      next: (user: ApplicationUser | null) => {
        if (user && user.roles.length > 0) {
          this.router.navigateByUrl('/');
        } else {
          const mode = this.activatedRoute.snapshot.paramMap.get('mode');
          if (mode) {
            this.mode = mode;
            this.requestCompleted = true;
          }
        }
      }
    })
    this.initializeForm();
  }

  initializeForm() {
    this.emailForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.pattern('^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$')]],
    });
  }

  sendEmail() {
    this.submitted = true;
    this.apiResponse = undefined;

    if (this.emailForm.valid) {
      if (this.mode.includes('resend-email-confirmation-link')) {
        this.accountService.resendEmailConfirmationLink(this.emailForm.get('email')?.value).subscribe({
          next: (response: ApiResponse) => {
            this.sharedService.showNotification(true, response.title, response.message);
            this.router.navigateByUrl('/account/login');
          },
          error: error => {
            this.apiResponse = error;
          }
        })
      } else if (this.mode.includes('forgot-username-or-password')) {
        this.accountService.forgotUsernameOrPassword(this.emailForm.get('email')?.value).subscribe({
          next: (response: ApiResponse) => {
            this.sharedService.showNotification(true, response.title, response.message);
            this.router.navigateByUrl('/account/login');
          },
          error: error => {
            if (error.message.includes('You have an account with us.')) {
              this.sharedService.showNotification(false, error.title, error.message);
            }
            this.apiResponse = error;
          }
        })
      }
    }
  }

  cancel() {
    this.router.navigateByUrl('/account/login');
  }

  resendEmailConfirmationLink() {
    this.router.navigateByUrl('/account/send-email/resend-email-confirmation-link');
  }
}
