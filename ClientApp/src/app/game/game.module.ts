import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HokmComponent } from './hokm/hokm.component';
import { SharedModule } from '../shared/shared.module';
import { GameRoutingModule } from './game-routing.module';
import { PlayerCardComponent } from './player-card/player-card.component';
import { GameInterruptionButtonsComponent } from './game-interruption-buttons/game-interruption-buttons.component';


@NgModule({
  declarations: [
    HokmComponent,
    PlayerCardComponent,
    GameInterruptionButtonsComponent,
  ],
  imports: [
    CommonModule,
    GameRoutingModule,
    SharedModule
  ]
})
export class GameModule { }
