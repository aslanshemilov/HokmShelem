import { HttpClient } from '@angular/common/http';
import { Injectable, Renderer2 } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Country } from './models/hokmshelem/country';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { NotificationComponent } from './components/modals/notification/notification.component';
import { BehaviorSubject, Observable, map, of } from 'rxjs';
import { ExpiringSessionCountdownComponent } from './components/modals/expiring-session-countdown/expiring-session-countdown.component';
import { ConfirmBoxComponent } from './components/modals/confirm-box/confirm-box.component';
import { NotificationMessage } from './models/engine/notificationMessage';
import { ToastrService } from 'ngx-toastr';
import { WinnerTeamComponent } from './components/modals/winner-team/winner-team.component';

@Injectable({
  providedIn: 'root'
})
export class SharedService {
  apiUrl = environment.apiUrl;
  countries: Country[] = [];
  bsModalRef: BsModalRef = new BsModalRef;
  displayingExpiringSessionModal = false;

  resetFilterValue = false;
  resetFilterSource = new BehaviorSubject<boolean>(this.resetFilterValue);
  resetFilter$ = this.resetFilterSource.asObservable();

  resetSortByValue = '';
  resetSortBySource = new BehaviorSubject<string>(this.resetSortByValue);
  resetSortBy$ = this.resetSortBySource.asObservable();

  constructor(private http: HttpClient,
    private modalService: BsModalService,
    private toastr: ToastrService) { }

  getCountries() {
    if (this.countries.length > 0) return of(this.countries);

    return this.http.get<Country[]>(this.apiUrl + 'hokmshelem/countries').pipe(
      map((countries: Country[]) => {
        if (countries) {
          this.countries = countries;
          return countries;
        }

        return null;
      }))
  }

  showNotification(isSuccess: boolean, title: string, message: string, isHtmlEnabled: boolean = false) {
    const initalState: ModalOptions = {
      initialState: {
        isSuccess,
        title,
        message,
        isHtmlEnabled
      }
    };

    this.bsModalRef = this.modalService.show(NotificationComponent, initalState);
  }

  openExpiringSessionCountdown = async () => {
    const config: ModalOptions = {
      backdrop: 'static',
      keyboard: false,
      ignoreBackdropClick: true
    }

    this.bsModalRef = this.modalService.show(ExpiringSessionCountdownComponent, config);
  }

  confirmBox(title: string, message: string): Observable<boolean> {
    const config = {
      initialState: {
        title,
        message
      }
    }

    this.bsModalRef = this.modalService.show(ConfirmBoxComponent, config);
    return new Observable<boolean>(this.getResult());
  }

  resetFilter() {
    this.resetFilterValue = !this.resetFilterValue;
    this.resetFilterSource.next(this.resetFilterValue);
  }

  resetSortBy(label: string) {
    this.resetSortByValue = label;
    this.resetSortBySource.next(this.resetSortByValue);
  }

  displayHubNotification(notification: NotificationMessage) {
    if (notification.useToastr) {
      if (notification.isSuccess) {
        this.toastr.success(notification.title);
      } else {
        this.toastr.error(notification.title);
      }
    } else {
      this.showNotification(notification.isSuccess, notification.title, notification.message);
    }
  }

  showWinner(winnerTeam: string, title: string, message: string) {
    const initalState: ModalOptions = {
      initialState: {
        winnerTeam,
        title,
        message
      }
    };

    this.bsModalRef = this.modalService.show(WinnerTeamComponent, initalState);
  }

  private getResult() {
    return (observer: any) => {
      const subscription = this.bsModalRef.onHidden?.subscribe(() => {
        observer.next(this.bsModalRef.content.result);
        observer.complete();
      });

      return {
        unsubscribe() {
          subscription?.unsubscribe();
        }
      }
    }
  }
}
