import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { ResetPasswordComponent } from './reset-password/reset-password.component';
import { SendEmailComponent } from './send-email/send-email.component';
import { AccountRoutingModule } from './account-routing.module';
import { ConfirmEmailComponent } from './confirm-email/confirm-email.component';
import { RegisterWithThirdPartyComponent } from './register-with-third-party/register-with-third-party.component';

@NgModule({
  declarations: [
    ConfirmEmailComponent,
    LoginComponent,
    RegisterComponent,
    ResetPasswordComponent,
    SendEmailComponent,
    RegisterWithThirdPartyComponent,
  ],
  imports: [
    CommonModule,
    AccountRoutingModule,
    SharedModule,
  ]
})
export class AccountModule { }
