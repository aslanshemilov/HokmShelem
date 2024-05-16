import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HomeComponent } from './home.component';
import { SharedModule } from '../shared/shared.module';
import { HomeRoutingModule } from './home-routing.module';
import { DemoComponent } from './demo/demo.component';
import { VisitorMessageComponent } from './visitor-message/visitor-message.component';
import { PolicyComponent } from './policy/policy.component';
import { TemsOfServiceComponent } from './tems-of-service/tems-of-service.component';
import { AboutUsComponent } from './about-us/about-us.component';
import { RulesComponent } from './rules/rules.component';

@NgModule({
  declarations: [
    HomeComponent,
    DemoComponent,
    VisitorMessageComponent,
    PolicyComponent,
    TemsOfServiceComponent,
    AboutUsComponent,
    RulesComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    HomeRoutingModule
  ]
})
export class HomeModule { }
