import { Component, Input } from '@angular/core';
import { LobbyService } from 'src/app/lobby/lobby.service';
import { Player } from 'src/app/shared/models/engine/player';

@Component({
  selector: 'app-connected-players',
  templateUrl: './connected-players.component.html',
  styleUrls: ['./connected-players.component.scss']
})
export class ConnectedPlayersComponent {
  @Input() players: Player[] | undefined | null;
  isPopupVisible = false;

  constructor(public lobbyService: LobbyService) {}

  showPlayerProfile() {
    this.isPopupVisible = true;
  }

  hidePlayerProfile() {
    this.isPopupVisible = false;
  }
}
