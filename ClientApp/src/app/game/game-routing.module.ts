import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HokmComponent } from './hokm/hokm.component';

const routes: Routes = [
  { path: 'hokm', component: HokmComponent }
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
export class GameRoutingModule { }
