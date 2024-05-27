import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { GameService } from '../game.service';
import { AccountService } from 'src/app/account/account.service';
import { Observable, take } from 'rxjs';
import { GameInfo } from '../../shared/models/engine/game';
import { environment } from 'src/environments/environment';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-hokm',
  templateUrl: './hokm.component.html',
  styleUrls: ['./hokm.component.scss']
})
export class HokmComponent implements OnInit, AfterViewInit {
  blobImageUrl = environment.azureContainerUrl + 'game';
  game$?: Observable<GameInfo | null>;
  @ViewChild('divgamebox') divgamebox: ElementRef | undefined;
  gameName: string | undefined;

  constructor(public gameService: GameService,
    private accountService: AccountService,
    private sharedService: SharedService
  ) { }

  ngOnInit(): void {
    this.gameService.getGameInfo().subscribe({
      next: _ => {
        this.game$ = this.gameService.gameInfo$;
        this.gameService.canExit = false;
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
    this.gameService.leaveTheGame();
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
