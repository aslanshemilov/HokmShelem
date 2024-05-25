import { Component, EventEmitter, Input, Output } from '@angular/core';
import { LobbyService } from 'src/app/lobby/lobby.service';
import { RoomToJoin } from 'src/app/shared/models/engine/room';

@Component({
  selector: 'app-room-to-join',
  templateUrl: './room-to-join.component.html',
  styleUrls: ['./room-to-join.component.scss']
})
export class RoomToJoinComponent {
  @Input() rooms: RoomToJoin[] | undefined | null;
  @Output() playerJoinedRoom = new EventEmitter();

  constructor(public lobbyService: LobbyService) {}

  isPopupVisible = false;

  showPlayerProfile() {
    this.isPopupVisible = true;
  }

  hidePlayerProfile() {
    this.isPopupVisible = false;
  }

  join(roomName: string) {
    this.lobbyService.joinTheRoom(roomName);
    this.playerJoinedRoom.emit(true);
  }
}
