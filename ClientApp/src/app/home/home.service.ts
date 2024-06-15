import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { MessageAdd } from '../shared/models/hokmshelem/messageAdd';
import { ApiResponse } from '../shared/models/apiResponse';
import { ReplaySubject, delay, map } from 'rxjs';
import { HomePageInfo } from '../shared/models/engine/homePageInfo';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { RegisterAsGuestComponent } from './register-as-guest/register-as-guest.component';

@Injectable({
  providedIn: 'root'
})
export class HomeService {
  apiUrl = environment.apiUrl;
  engineUrl = environment.engineUrl;
  visited = false;
  loadingGameInfo = false;
  bsModalRef?: BsModalRef;

  private homePageInfoSource = new ReplaySubject<HomePageInfo | null>(1);
  homePageInfo$ = this.homePageInfoSource.asObservable();

  constructor(private http: HttpClient,
    private modalService: BsModalService
  ) { }
  
  visitor() {
    return this.http.get(this.apiUrl + 'hokmshelem');
  }

  addMessage(model: MessageAdd) {
    return this.http.post<ApiResponse>(this.apiUrl + 'hokmshelem/add-message', model);
  }

  getHomePageInfo() {
    this.loadingGameInfo = true;
    return this.http.get<HomePageInfo>(this.engineUrl + 'game/homepage-info').pipe(
      delay(500),
      map((homePageInfo: HomePageInfo | null) => {
        if (homePageInfo) {
          this.loadingGameInfo = false;
          this.homePageInfoSource.next(homePageInfo);
        }
      })
    )
  }

  openRegisterGuestUserModal() {
    this.bsModalRef = this.modalService.show(RegisterAsGuestComponent);
    this.bsModalRef.content.closeBtnName = 'Close';
  }
}
