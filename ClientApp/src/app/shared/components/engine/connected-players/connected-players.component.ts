import { Component, Input, OnInit } from '@angular/core';
import { take } from 'rxjs';
import { AccountService } from 'src/app/account/account.service';
import { LobbyService } from 'src/app/lobby/lobby.service';
import { ApplicationUser } from 'src/app/shared/models/account/applicationUser';
import { Player } from 'src/app/shared/models/engine/player';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-connected-players',
  templateUrl: './connected-players.component.html',
  styleUrls: ['./connected-players.component.scss']
})
export class ConnectedPlayersComponent implements OnInit {
  gameImageUrl = environment.azureContainerUrl;
  @Input() players: Player[] | undefined | null;
  myName: string | undefined;
  isPopupVisible = false;

  constructor(public lobbyService: LobbyService, 
    private accountService: AccountService) {}

  ngOnInit(): void {
    this.accountService.applicationUser$.pipe(take(1)).subscribe({
      next: (user: ApplicationUser | null) => {
        if (user) {
          this.myName = user.playerName;
        }
      }
    })
  }

  showPlayerProfile() {
    this.isPopupVisible = true;
  }

  hidePlayerProfile() {
    this.isPopupVisible = false;
  }
}
