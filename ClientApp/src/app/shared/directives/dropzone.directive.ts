import { Directive, ElementRef, EventEmitter, HostBinding, OnInit, Output, SkipSelf } from '@angular/core';
import { DroppableService } from './droppable.service';

@Directive({
  selector: '[appDropzone]'
})
export class DropzoneDirective implements OnInit {
  @HostBinding('class.dropzone-activated') dropzoneActivated = false;
  @HostBinding('class.dropzone-entered') entered = false;

  @Output() drop = new EventEmitter<PointerEvent>();
  @Output() remove = new EventEmitter<PointerEvent>();

  private clientRect?: ClientRect;

  constructor(@SkipSelf() private allDroppableService: DroppableService,
    private innerDroppableService: DroppableService,
    private element: ElementRef) { }

  ngOnInit(): void {
    this.allDroppableService.dragStart$.subscribe(_ => this.onDragStart());
    this.allDroppableService.dragEnd$.subscribe(event => this.onDragEnd(event));

    this.allDroppableService.dragMove$.subscribe(event => {
      if (this.isEventInside(event)) {
        this.onPointerEnter();
      } else {
        this.onPointerLeave();
      }
    });

    this.innerDroppableService.dragStart$.subscribe(_ => this.onInnerDragStart());
    this.innerDroppableService.dragEnd$.subscribe(event => this.onInnerDragEnd(event));
  }

  private onPointerEnter(): void {
    if (!this.dropzoneActivated) {
      return;
    }

    this.entered = true;
  }

  private onPointerLeave(): void {
    this.dropzoneActivated = true;
    this.entered = false;
  }

  private onDragStart(): void {
    this.clientRect = this.element.nativeElement.getBoundingClientRect();
    this.dropzoneActivated = true;
  }

  private onDragEnd(event: any): void {
    if (!this.dropzoneActivated) {
      return;
    }

    if (this.entered) {
      this.drop.emit(event);
    }

    this.dropzoneActivated = false;
    this.entered = false;
  }

  private onInnerDragStart() {
   this.dropzoneActivated = false;
  }

  private onInnerDragEnd(event: PointerEvent) {
    if (!this.entered) {
      this.remove.emit(event);
    }

    this.dropzoneActivated = false;
    this.entered = false;
  }

  private isEventInside(event: PointerEvent) {
    return event.clientX >= this.clientRect!.left &&
      event.clientX <= this.clientRect!.right &&
      event.clientY >= this.clientRect!.top &&
      event.clientY <= this.clientRect!.bottom;
  }
}
