import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { UserHasRoleDirective } from './directives/user-has-role.directive';
import { IsNonLoggedinUserOrGuestDirective } from './directives/is-non-loggedin-user-or-guest.directive';
import { NavbarComponent } from './navbar/navbar.component';
import { FooterComponent } from './footer/footer.component';



@NgModule({
  declarations: [
    NavbarComponent,
    FooterComponent,
    UserHasRoleDirective,
    IsNonLoggedinUserOrGuestDirective
  ],
  imports: [
    CommonModule,
    SharedModule
  ],
  exports: [
    NavbarComponent,
    FooterComponent,
    UserHasRoleDirective,
    IsNonLoggedinUserOrGuestDirective
  ]
})
export class CoreModule { }
