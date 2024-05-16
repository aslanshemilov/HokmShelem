import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-paging-numbers',
  templateUrl: './paging-numbers.component.html',
  styleUrls: ['./paging-numbers.component.scss']
})
export class PagingNumbersComponent {
  @Input() totalCount?: number;
  @Input() pageSize?: number;
  @Output() pageChanged = new EventEmitter<number>();

  onPagerChanged(event: any) {
    this.pageChanged.emit(event.page);
  }
}
