import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-paging-size-entries',
  templateUrl: './paging-size-entries.component.html',
  styleUrls: ['./paging-size-entries.component.scss']
})
export class PagingSizeEntriesComponent {
  @Output() pageSize = new EventEmitter<number>();
  
  entries = [
    { name: '10', value: 10 },
    { name: '25', value: 25 },
    { name: '50', value: 50 },
    { name: '100', value: 100 },
  ];

  onEntriesSelection(event: any) {
    this.pageSize.emit(event.target.value);
  }
}
