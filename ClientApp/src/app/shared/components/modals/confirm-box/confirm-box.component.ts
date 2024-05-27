import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-confirm-box',
  templateUrl: './confirm-box.component.html',
  styleUrls: ['./confirm-box.component.scss']
})
export class ConfirmBoxComponent implements OnInit {
  type?: string;
  title?: string;
  message?: string;
  result?: boolean;

  constructor(public bsModalRef: BsModalRef) {}
  ngOnInit(): void {
    
  }

  yes() {
    this.result = true;
    this.bsModalRef.hide();
  }

  no() {
    this.result = false;
    this.bsModalRef.hide();
  }
}
