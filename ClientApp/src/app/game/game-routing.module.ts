import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HokmComponent } from './hokm/hokm.component';
import { preventLeaveGuard } from '../core/guards/prevent-leave.guard';

const routes: Routes = [
  { path: 'hokm', component: HokmComponent, canDeactivate: [preventLeaveGuard] }
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
