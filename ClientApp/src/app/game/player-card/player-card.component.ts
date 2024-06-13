import { Component, ElementRef, EventEmitter, HostListener, Input, OnInit, Output, QueryList, Renderer2, ViewChildren } from '@angular/core';
import { GameService } from '../game.service';
import { GS } from 'src/app/shared/models/engine/game';
import { environment } from 'src/environments/environment';
import { CdkDragEnd } from '@angular/cdk/drag-drop';
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
  screenWidth: number | undefined;
  playerCardDivHeight: number | undefined;

  @HostListener('window:resize', ['$event'])
  onResize(): void {
    this.getScreenWidth();
  }

  constructor(public gameService: GameService, private renderer: Renderer2) { }

  ngOnInit(): void {
    this.getScreenWidth();
  }

  onSelectCard(enabled: boolean, card: string, event: MouseEvent) {
    if (enabled) {
      event.stopPropagation();
      const found = this.gameService.selectedCards.find(c => c === card);
      if (found) {
        const imgElement = this.getCardElement(card);
        if (imgElement) {
          this.renderer.removeClass(imgElement.nativeElement, 'jump');
        }
        this.gameService.selectedCards = [...this.gameService.selectedCards.filter(c => c !== found)];
      } else {
        if (this.gameService.selectedCards.length < 4) {
          this.gameService.selectedCards = [...this.gameService.selectedCards, card];
          const imgElement = this.getCardElement(card);
          if (imgElement) {
            this.renderer.addClass(imgElement.nativeElement, 'jump');
          }
        }
      }
    }
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

  onDragEnded(event: CdkDragEnd) {
    const draggedElement = event.source.element.nativeElement;
    setTimeout(() => {
      draggedElement.style.transform = 'none';
      event.source._dragRef.reset();
    }, 150);
  }

  private getCardElement(card: string): ElementRef | undefined {
    return this.jumpingImg.find((div) => div.nativeElement.src.includes(card));
  }
}
