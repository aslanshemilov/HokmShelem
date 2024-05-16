import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { Subscription, interval } from 'rxjs';

@Component({
  selector: 'app-count-down-to-start',
  templateUrl: './count-down-to-start.component.html',
  styleUrls: ['./count-down-to-start.component.scss']
})
export class CountDownToStartComponent implements OnInit, OnDestroy {
  @Input() countDown: number = 3;
  stop = false;
  private subscription?: Subscription;

  ngOnInit() {
    this.subscription = interval(1000)
      .subscribe(x => {
        if (this.countDown > 1) {
          this.countDown--;
        }
        else {
          this.ngOnDestroy();
        }
      });
  }

  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
