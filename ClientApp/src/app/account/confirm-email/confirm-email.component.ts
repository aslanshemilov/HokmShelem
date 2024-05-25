import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { take } from 'rxjs';
import { ApplicationUser } from 'src/app/shared/models/account/applicationUser';
import { ConfirmEmail } from 'src/app/shared/models/account/confirmEmail';
import { AccountService } from '../account.service';
import { ApiResponse } from 'src/app/shared/models/apiResponse';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.scss']
})
export class ConfirmEmailComponent implements OnInit {
  completed = false;
  success = false;
  apiResponse: ApiResponse | undefined;

  constructor(private accountService: AccountService,
    private sharedService: SharedService,
    private router: Router,
    private activatedRoute: ActivatedRoute) {
  }

  ngOnInit(): void {
    this.accountService.applicationUser$.pipe(take(1)).subscribe({
      next: (user: ApplicationUser | null) => {
        if (user && user.roles.length > 0) {
          this.router.navigateByUrl('/');
        } else {
          this.activatedRoute.queryParamMap.subscribe({
            next: (params: any) => {
              const token = params.get('token');
              const email = params.get('email');

              if (!token || !email) {
                this.router.navigateByUrl('/');
              } else {
                const confirmEmail: ConfirmEmail = {
                  token: params.get('token'),
                  email: params.get('email'),
                }
  
                this.accountService.confirmEmail(confirmEmail).subscribe({
                  next: (response: ApiResponse) => {
                    this.apiResponse = response;
                    this.sharedService.showNotification(true, response.title, response.message);
                    this.completed = true;
                    this.success = true;
                  }, error: error => {
                    this.apiResponse = error;
                    this.sharedService.showNotification(false, error.title, error.message);
                    this.completed = true;
                  }
                })
              }
            }
          })
        }
      }
    })
  }

  resendEmailConfirmationLink() {
    this.router.navigateByUrl('/account/send-email/resend-email-confirmation-link');
  }
}
