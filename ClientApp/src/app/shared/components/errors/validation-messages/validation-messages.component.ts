import { Component, Input, OnInit } from '@angular/core';
import { ApiResponse } from 'src/app/shared/models/apiResponse';

@Component({
  selector: 'app-validation-messages',
  templateUrl: './validation-messages.component.html',
  styleUrls: ['./validation-messages.component.scss']
})
export class ValidationMessagesComponent implements OnInit {
  @Input() apiResponse: ApiResponse | undefined;

  constructor() { }

  ngOnInit(): void {
    
  }
}
