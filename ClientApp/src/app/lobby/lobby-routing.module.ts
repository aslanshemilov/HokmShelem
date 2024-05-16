import { NgModule } from '@angular/core';
import { LobbyComponent } from './lobby.component';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  { path: '', component: LobbyComponent },
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
export class LobbyRoutingModule { }
