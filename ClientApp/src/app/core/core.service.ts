import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { SharedService } from '../shared/shared.service';
import { timer$ } from '../shared/sharedHelper';

@Injectable({
  providedIn: 'root'
})
export class CoreService {
  private loadingRequestCount = 0;
  interupt: boolean | undefined;

  constructor(private spinnerService: NgxSpinnerService) { }

  loading() {
    this.interupt = false;

    timer$(3).subscribe(
      () => {
        if (!this.interupt) {
          this.loadingRequestCount++;
          this.spinnerService.show(undefined, {
            type: 'ball-fussion',
            bdColor: 'rgba(0, 0, 0, 0.8)',
            color: '#fff'
          });
        }
      }
    );
  }

  idle() {
    this.loadingRequestCount--;
    if (this.loadingRequestCount <= 0) {
      this.loadingRequestCount = 0;
      this.spinnerService.hide();
      this.interupt = true;
    }
  }
}
