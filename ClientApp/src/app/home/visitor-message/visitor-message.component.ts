import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HomeService } from '../home.service';
import { SharedService } from 'src/app/shared/shared.service';
import { ApiResponse } from 'src/app/shared/models/apiResponse';

@Component({
  selector: 'app-visitor-message',
  templateUrl: './visitor-message.component.html',
  styleUrls: ['./visitor-message.component.scss']
})
export class VisitorMessageComponent implements OnInit {
  messageForm: FormGroup = new FormGroup({});
  submitted = false;
  apiResponse: ApiResponse | undefined;

  constructor(private formBuilder: FormBuilder,
    private homeService: HomeService,
    private sharedService: SharedService) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.messageForm = this.formBuilder.group({
      name: ['', [Validators.required], Validators.maxLength(100)],
      email: ['', [Validators.required, Validators.maxLength(100), Validators.pattern('^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$')]],
      message: ['', [Validators.required, Validators.maxLength(5000)]],
    });
  }

  sendMessage() {
    this.submitted = true;
    this.apiResponse = undefined;

    if (this.messageForm.valid) {
      this.homeService.addMessage(this.messageForm.value).subscribe({
        next: (response: ApiResponse) => {
          this.sharedService.showNotification(true, response.title, response.message);
          this.messageForm.reset();
          this.submitted = false;
        },
        error: error => {
          this.apiResponse = error;
        }
      })
    }
  }
}
