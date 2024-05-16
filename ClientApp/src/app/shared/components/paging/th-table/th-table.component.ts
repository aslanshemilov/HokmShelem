import { Component, EventEmitter, Input, Output } from '@angular/core';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-th-table',
  templateUrl: './th-table.component.html',
  styleUrls: ['./th-table.component.scss']
})
export class ThTableComponent {
  @Input() label?: string;
  @Input() isSortable = false;
  @Output() sortBy = new EventEmitter<string>();
  arrowUp: boolean | undefined;

  constructor(private sharedService: SharedService) {
    this.sharedService.resetFilter$.subscribe(_ => {
      this.arrowUp = undefined;
    });

    this.sharedService.resetSortBy$.subscribe(result => {
      if (this.label !== result) {
        this.arrowUp = undefined;
      }
    });
  }

  toggleArrow() {
    if (this.isSortable) {
      if (this.label) {
        this.sharedService.resetSortBy(this.label);
      }

      if (this.arrowUp === undefined) {
        this.arrowUp = true;
        this.sortBy.emit(this.label?.replace(/\s+/, "")  + 'a');
      } else if (this.arrowUp === true) {
        this.arrowUp = false;
        this.sortBy.emit(this.label?.replace(/\s+/, "")  + 'd');
      } else {
        this.arrowUp = undefined;
        this.sortBy.emit('');
      }
    }
  }
}
