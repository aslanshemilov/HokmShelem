import { Injectable, Optional, SkipSelf } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { GameService } from 'src/app/game/game.service';
import { GS } from '../models/engine/game';

@Injectable({
  providedIn: 'root'
})
export class DroppableService {
  dragStart$: Observable<PointerEvent>;
  dragMove$: Observable<PointerEvent>;
  dragEnd$: Observable<PointerEvent>;

  private dragStartSubject = new Subject<PointerEvent>();
  private dragMoveSubject = new Subject<PointerEvent>();
  private dragEndSubject = new Subject<PointerEvent>();

  constructor(private gameService: GameService,
    @SkipSelf() @Optional() private parent?: DroppableService) {
    this.dragStart$ = this.dragStartSubject.asObservable();
    this.dragMove$ = this.dragMoveSubject.asObservable();
    this.dragEnd$ = this.dragEndSubject.asObservable();
  }

  onDragStart(event: PointerEvent): void {
    const gameInfo = this.gameService.getGameInfoSourceValue();
    if (gameInfo) {
      if (gameInfo.gs == GS.RoundGameStarted && gameInfo.whosTurnIndex == gameInfo.myIndex) {
        this.dragStartSubject.next(event);
        if (this.parent) {
          this.parent.onDragStart(event);
        }
      }
    }
  }

  onDragMove(event: PointerEvent): void {
    const gameInfo = this.gameService.getGameInfoSourceValue();
    if (gameInfo) {
      if (gameInfo.gs == GS.RoundGameStarted && gameInfo.whosTurnIndex == gameInfo.myIndex) {
        this.dragMoveSubject.next(event);
        if (this.parent) {
          this.parent.onDragMove(event);
        }
      }
    }
  }

  onDragEnd(event: PointerEvent): void {
    const gameInfo = this.gameService.getGameInfoSourceValue();
    if (gameInfo) {
      if (gameInfo.gs == GS.RoundGameStarted && gameInfo.whosTurnIndex == gameInfo.myIndex) {
        this.dragEndSubject.next(event);
        if (this.parent) {
          this.parent.onDragEnd(event);
        }
      }
    }
  }
}
