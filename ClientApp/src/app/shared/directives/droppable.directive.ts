import { Directive, ElementRef, EventEmitter, HostListener, Input, Output } from '@angular/core';
import { DroppableService } from './droppable.service';

@Directive({
  selector: '[appDroppable]'
})
export class DroppableDirective {
  dragging = false;

  @Output() dragStart = new EventEmitter<PointerEvent>();
  @Output() dragMove = new EventEmitter<PointerEvent>();
  @Output() dragEnd = new EventEmitter<PointerEvent>();

  @Input('appCardValue') cardValue : string | undefined;

  constructor(public element: ElementRef, 
     
    private droppableService: DroppableService) { }

  @HostListener('pointerdown', ['$event'])
  onPointerDown(event: PointerEvent): void {
    if (event.button !== 0) {
      return;
    }

    this.dragging = true;
    event.stopPropagation();
    this.dragStart.emit(event);
  }

  @HostListener('document:pointermove', ['$event'])
  onPointerMove(event: PointerEvent): void {
    if (!this.dragging) {
      return;
    }

    this.dragMove.emit(event);
  }

  @HostListener('document:pointerup', ['$event'])
  onPointerUp(event: PointerEvent): void {
    if (!this.dragging) {
      return;
    }

    this.dragging = false;
    this.dragEnd.emit(event);
  }

  @HostListener('dragStart', ['$event'])
  onDragStart(event: PointerEvent): void {
    this.droppableService.onDragStart(event);
  }

  @HostListener('dragMove', ['$event'])
  onDragMove(event: PointerEvent): void {
    this.droppableService.onDragMove(event);
  }

  @HostListener('dragEnd', ['$event'])
  onDragEnd(event: PointerEvent): void {
    this.droppableService.onDragEnd(event);
  }
}
