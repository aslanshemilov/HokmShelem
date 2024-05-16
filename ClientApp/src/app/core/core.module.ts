import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from './navbar/navbar.component';
import { FooterComponent } from './footer/footer.component';
import { SharedModule } from '../shared/shared.module';
import { UserHasRoleDirective } from './directives/user-has-role.directive';



@NgModule({
  declarations: [
    NavbarComponent,
    FooterComponent,
    UserHasRoleDirective
  ],
  imports: [
    CommonModule,
    SharedModule
  ],
  exports: [
    NavbarComponent,
    FooterComponent,
    UserHasRoleDirective
  ]
})
export class CoreModule { }
