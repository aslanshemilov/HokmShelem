import { Component, OnDestroy, OnInit } from '@angular/core';
import { LobbyService } from './lobby.service';
import { AccountService } from '../account/account.service';
import { take } from 'rxjs';
import { Router } from '@angular/router';

@Component({
  selector: 'app-lobby',
  templateUrl: './lobby.component.html',
  styleUrls: ['./lobby.component.scss']
})
export class LobbyComponent implements OnInit, OnDestroy {
  message?: string;

  constructor(public lobbyService: LobbyService,
    private accountService: AccountService,
    private router: Router,) {
    this.accountService.applicationUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user) {
          this.lobbyService.myPlayerName = user.playerName;
        }
      }
    })
  }

  ngOnDestroy(): void {
    this.lobbyService.leaveLobby();
  }

  ngOnInit(): void {
    this.accountService.applicationUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user) {
          this.lobbyService.connectLobby(user);
        }
      }
    })
  }

  sendMessage(message: string) {
    this.lobbyService.sendMessage(message);
  }

  createRoom() {
    this.lobbyService.createRoomModal(this.lobbyService.myPlayerName);
  }

  lobbyButtonToggle(target: number) {
    switch (target) {
      case 1:
        this.lobbyService.mode.showLobbyChat = !this.lobbyService.mode.showLobbyChat;
        this.lobbyService.setRoomChatSize();
        break;
      case 2:
        this.lobbyService.mode.showLobbyPlayers = !this.lobbyService.mode.showLobbyPlayers;
        break;
      case 3:
        this.lobbyService.mode.showLobbyRooms = !this.lobbyService.mode.showLobbyRooms;
        break;
    }
  }

  leaveTheRoom() {
    this.lobbyService.leaveTheRoom().then(_ => {
      this.lobbyService.setLobbyMode();
    });
  }

  selectSit(sitNumber: string) {
    this.lobbyService.selectSit(sitNumber);
  }

  ready() {
    this.lobbyService.readyButtonClicked();
  }

  start() {
    this.lobbyService.startTheGame();
  }
}