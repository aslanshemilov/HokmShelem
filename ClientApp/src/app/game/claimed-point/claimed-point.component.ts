import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-claimed-point',
  templateUrl: './claimed-point.component.html',
  styleUrls: ['./claimed-point.component.scss']
})
export class ClaimedPointComponent {
  @Input() point: number | undefined;
}
