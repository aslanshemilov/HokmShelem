import { Component, ElementRef, EventEmitter, HostListener, Input, OnInit, Output, QueryList, Renderer2, ViewChildren } from '@angular/core';
import { GameService } from '../game.service';
import { GS } from 'src/app/shared/models/engine/game';

@Component({
  selector: 'app-player-card',
  templateUrl: './player-card.component.html',
  styleUrls: ['./player-card.component.scss']
})
export class PlayerCardComponent implements OnInit {
  @Input() cards: string[] | undefined;
  @Output() cardOutPut = new EventEmitter();
  clickCount = 0;
  @ViewChildren('jumpingImg') jumpingImg!: QueryList<ElementRef>;
  currentJumpedCard: string | null = null;
  clickTimeout: any = null;

  constructor(private gameService: GameService, private renderer: Renderer2) { }

  ngOnInit(): void {
  }

  onMouseOver(card: string) {
    if (this.gameService.gameInfo) {
      if (this.gameService.gameInfo.gs != GS.HakemChooseHokm) {
        const imgElement = this.getCardElement(card);
        if (imgElement) {
          this.renderer.addClass(imgElement.nativeElement, 'jump');
        }
      }
    }
  }

  onMouseOut(card: string) {
    if (this.gameService.gameInfo) {
      if (this.gameService.gameInfo.gs != GS.HakemChooseHokm) {
        const imgElement = this.getCardElement(card);
        if (imgElement) {
          this.renderer.removeClass(imgElement.nativeElement, 'jump');
        }
      }
    }
  }

  onDoubleClick(card: string, event: MouseEvent) {
    if (this.gameService.gameInfo) {
      if (this.gameService.gameInfo.gs != GS.HakemChooseHokm) {
        event.stopPropagation();
        if (this.clickTimeout) {
          clearTimeout(this.clickTimeout);
          this.clickTimeout = null;
        }
        console.log(card);
      }
    }
  }

  onClick(card: string, event: MouseEvent) {
   event.stopPropagation();

    if (this.clickTimeout) {
      clearTimeout(this.clickTimeout);
      this.clickTimeout = null;
    }

    this.clickTimeout = setTimeout(() => {
      // If the same card is clicked again, remove the jump class
      if (this.currentJumpedCard === card) {
        const imgElement = this.getCardElement(card);
        if (imgElement) {
          this.renderer.removeClass(imgElement.nativeElement, 'jump');
        }
        this.currentJumpedCard = null; // Reset the current jumped card
      } else {
        // If another card was jumped, remove the jump class from it
        if (this.currentJumpedCard) {
          const previousCardElement = this.getCardElement(this.currentJumpedCard);
          if (previousCardElement) {
            this.renderer.removeClass(previousCardElement.nativeElement, 'jump');
          }
        }
        // Set the new card as the current jumped card
        this.currentJumpedCard = card;
        const imgElement = this.getCardElement(card);
        if (imgElement) {
          this.renderer.addClass(imgElement.nativeElement, 'jump');
        }
      }
      this.clickTimeout = null;
    }, 300);

    // this.clickCount++;

    // setTimeout(() => {
    //   if (this.clickCount === 2) {
    //     console.log(card);
    //     this.cardOutPut.emit(card);
    //   } else  {
       
    //   }
    //   this.clickCount = 0;
    // }, 200)
  }

  @HostListener('document:click', ['$event'])
  handleClickOutside(event: MouseEvent) {
    if (this.currentJumpedCard) {
      const cardElement = this.getCardElement(this.currentJumpedCard);
      if (cardElement) {
        this.renderer.removeClass(cardElement.nativeElement, 'jump');
        this.currentJumpedCard = null;
      }
    }
  }

  private getCardElement(card: string): ElementRef | undefined {
    return this.jumpingImg.find((div) => div.nativeElement.src.includes(card));
  }
}
