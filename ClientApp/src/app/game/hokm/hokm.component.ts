import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { GameService } from '../game.service';
import { AccountService } from 'src/app/account/account.service';
import { take } from 'rxjs';
import * as gs from '../../shared/models/engine/game';

@Component({
  selector: 'app-hokm',
  templateUrl: './hokm.component.html',
  styleUrls: ['./hokm.component.scss']
})
export class HokmComponent implements OnInit, AfterViewInit {
  @ViewChild('divgamebox') divgamebox: ElementRef | undefined;
  gameName: string | undefined;
  GS = (gs as any).GS;

  constructor(public gameService: GameService,
    private accountService: AccountService
  ) { }

  ngOnInit(): void {
    this.gameService.getGameInfo().subscribe({
      next: _ => {
        this.connectToGame();
      }
    })
  }

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.scrollToGameBox()
    }, 500);

  }

  connectToGame() {
    this.accountService.applicationUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user) {
          this.gameService.connectGame(user);
        }
      }
    })
  }

  ngOnDestroy(): void {
    this.gameService.leaveGame();
  }

  sendMessage(message: string) {
    this.gameService.sendMessage(message);
  }

  chooseHokm(suit: string | null) {
    if (suit) {
      this.gameService.hakemChoseHokmSuit(suit);
    }
  }

  playerPlayedTheCard(card: string) {
    this.gameService.playerPlayedTheCard(card);
  }

  private scrollToGameBox(): void {
    if (this.divgamebox) {
      this.divgamebox.nativeElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }
}
