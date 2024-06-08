import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HokmComponent } from './hokm/hokm.component';
import { SharedModule } from '../shared/shared.module';
import { GameRoutingModule } from './game-routing.module';
import { PlayerCardComponent } from './player-card/player-card.component';
import { GameInterruptionButtonsComponent } from './game-interruption-buttons/game-interruption-buttons.component';
import { CdkDragHandle } from '@angular/cdk/drag-drop';
import { ShelemComponent } from './shelem/shelem.component';
import { ClaimPointInputComponent } from './claim-point-input/claim-point-input.component';
import { ClaimedPointComponent } from './claimed-point/claimed-point.component';


@NgModule({
  declarations: [
    HokmComponent,
    PlayerCardComponent,
    GameInterruptionButtonsComponent,
    ShelemComponent,
    ClaimPointInputComponent,
    ClaimedPointComponent,
  ],
  imports: [
    CommonModule,
    GameRoutingModule,
    SharedModule,
    CdkDragHandle
  ]
})
export class GameModule { }
