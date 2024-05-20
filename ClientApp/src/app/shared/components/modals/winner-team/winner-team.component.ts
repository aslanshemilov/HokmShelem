import { Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-winner-team',
  templateUrl: './winner-team.component.html',
  styleUrls: ['./winner-team.component.scss']
})
export class WinnerTeamComponent {
  winnerTeam: string = '';
  title: string = '';
  message: string = '';

  constructor (public bsModalRef: BsModalRef) {}
}
