import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LobbyComponent } from './lobby.component';
import { LobbyRoutingModule } from './lobby-routing.module';
import { SharedModule } from '../shared/shared.module';
import { CreateRoomComponent } from './modals/create-room/create-room.component';

@NgModule({
  declarations: [
    LobbyComponent,
    CreateRoomComponent
  ],
  imports: [
    CommonModule,
    LobbyRoutingModule,
    SharedModule
  ]
})
export class LobbyModule { }
