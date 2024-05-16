import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-chat-input',
  templateUrl: './chat-input.component.html',
  styleUrls: ['./chat-input.component.scss']
})
export class ChatInputComponent implements OnInit {
  messageForm: FormGroup = new FormGroup({});
  @Output() messageToSend = new EventEmitter();

  constructor(private formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.messageForm = this.formBuilder.group({
      message: ['', [Validators.required, Validators.maxLength(1000)]],
    });
  }

  sendMessage() {
    if (this.messageForm.valid) {
      let message = this.messageForm.controls['message'].value.trim();
      if (message) {
        this.messageToSend.emit(message);
      }
    }

    this.messageForm.reset();
  }
}
