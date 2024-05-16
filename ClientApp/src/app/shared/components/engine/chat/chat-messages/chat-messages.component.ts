import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs';
import { MessageThread } from 'src/app/shared/models/engine/messageThread';

@Component({
  selector: 'app-chat-messages',
  templateUrl: './chat-messages.component.html',
  styleUrls: ['./chat-messages.component.scss']
})
export class ChatMessagesComponent implements OnInit {
  @Input() messageThreads: MessageThread[] | null | undefined;
  @ViewChild('scrollMe') private myScrollContainer: ElementRef | undefined;

  ngOnInit() {
    this.scrollToBottom();
  }

  ngOnChanges() {
    setTimeout(() => {
      this.scrollToBottom();
    }, 50);
  }

  scrollToBottom(): void {
    try {
      if (this.myScrollContainer) {
        this.myScrollContainer.nativeElement.scrollTop = this.myScrollContainer.nativeElement.scrollHeight;
      }
    } catch (err) { }
  }
}
