import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AccountService } from 'src/app/account/account.service';
import { ApplicationUser } from 'src/app/shared/models/account/applicationUser';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-register-as-guest',
  templateUrl: './register-as-guest.component.html',
  styleUrls: ['./register-as-guest.component.scss']
})
export class RegisterAsGuestComponent implements OnInit {
  regusterGuestForm: FormGroup = new FormGroup({});
  submitted = false;

  constructor(public bsModalRef: BsModalRef,
    private accountService: AccountService,
    private sharedService: SharedService,
    private router: Router,
    private formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.regusterGuestForm = this.formBuilder.group({
      guestName: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(15)]]
    });
  }

  register() {
    this.submitted = true;

    if (this.regusterGuestForm.valid) {
      this.accountService.registerAsGuest(this.regusterGuestForm.value).subscribe({
        next: (user: ApplicationUser | null) => {
          if (user) {
            this.bsModalRef.hide();
            this.sharedService.showNotification(true, 'Guest Player', 'Your name is <strong>' + user.playerName + '</strong>', true);
            this.router.navigateByUrl('/lobby');
          }
        }
      });
    }
  }
}
