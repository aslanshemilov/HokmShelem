import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HomeComponent } from './home.component';
import { SharedModule } from '../shared/shared.module';
import { HomeRoutingModule } from './home-routing.module';
import { DemoComponent } from './demo/demo.component';
import { ContactUSComponent } from './contact-us/contact-us.component';
import { PolicyComponent } from './policy/policy.component';
import { TemsOfServiceComponent } from './tems-of-service/tems-of-service.component';
import { AboutUsComponent } from './about-us/about-us.component';
import { RulesComponent } from './rules/rules.component';
import { CoreModule } from '../core/core.module';
import { RegisterAsGuestComponent } from './register-as-guest/register-as-guest.component';

@NgModule({
  declarations: [
    HomeComponent,
    DemoComponent,
    ContactUSComponent,
    PolicyComponent,
    TemsOfServiceComponent,
    AboutUsComponent,
    RulesComponent,
    RegisterAsGuestComponent,
  ],
  imports: [
    CommonModule,
    SharedModule,
    HomeRoutingModule,
    CoreModule
  ]
})
export class HomeModule { }
