import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home.component';
import { PolicyComponent } from './policy/policy.component';
import { TemsOfServiceComponent } from './tems-of-service/tems-of-service.component';
import { AboutUsComponent } from './about-us/about-us.component';
import { RulesComponent } from './rules/rules.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'policy', component: PolicyComponent },
  { path: 'terms-of-service', component: TemsOfServiceComponent },
  { path: 'rules', component:  RulesComponent},
  { path: 'about-us', component: AboutUsComponent }
]

@NgModule({
  declarations: [],
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ]
})
export class HomeRoutingModule { }
