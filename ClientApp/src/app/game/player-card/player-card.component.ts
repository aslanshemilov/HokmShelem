import { Component, ElementRef, EventEmitter, HostListener, Input, OnInit, Output, QueryList, ViewChildren } from '@angular/core';
import { GameService } from '../game.service';
import { GS } from 'src/app/shared/models/engine/game';
import { environment } from 'src/environments/environment';
@Component({
  selector: 'app-player-card',
  templateUrl: './player-card.component.html',
  styleUrls: ['./player-card.component.scss']
})
export class PlayerCardComponent implements OnInit {
  @ViewChildren('jumpingImg') jumpingImg!: QueryList<ElementRef>;
  @Input() cards: string[] | undefined;
  @Output() cardBeingPlayed = new EventEmitter();
  blobImageUrl = environment.azureContainerUrl + 'game';
  clickTimeout: any = null;
  screenWidth?: number;


  @HostListener('window:resize', ['$event'])
  onResize(event: any): void {
    this.getScreenWidth();
  }

  constructor(private gameService: GameService) { }

  ngOnInit(): void {
    this.getScreenWidth();
  }

  onDoubleClick(card: string, event: MouseEvent) {
    const gameInfo = this.gameService.getGameInfoSourceValue();
    if (gameInfo) {
      if (gameInfo.gs == GS.RoundGameStarted &&
        gameInfo.whosTurnIndex == gameInfo.myIndex) {
        event.stopPropagation();
        if (this.clickTimeout) {
          clearTimeout(this.clickTimeout);
          this.clickTimeout = null;
        }

        this.cardBeingPlayed.emit(card);
      }
    }
  }

  getScreenWidth(): void {
    this.screenWidth = window.innerWidth;
  }

}
