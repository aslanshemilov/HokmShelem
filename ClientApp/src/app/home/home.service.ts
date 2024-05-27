import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { MessageAdd } from '../shared/models/hokmshelem/messageAdd';
import { ApiResponse } from '../shared/models/apiResponse';
import { ReplaySubject, map } from 'rxjs';
import { HomePageInfo } from '../shared/models/engine/homePageInfo';

@Injectable({
  providedIn: 'root'
})
export class HomeService {
  apiUrl = environment.apiUrl;
  engineUrl = environment.engineUrl;
  visited = false;

  private homePageInfoSource = new ReplaySubject<HomePageInfo | null>(1);
  homePageInfo$ = this.homePageInfoSource.asObservable();

  constructor(private http: HttpClient) { }

  visitor() {
    return this.http.get(this.apiUrl + 'hokmshelem');
  }

  addMessage(model: MessageAdd) {
    return this.http.post<ApiResponse>(this.apiUrl + 'hokmshelem/add-message', model);
  }

  getHomePageInfo() {
    return this.http.get<HomePageInfo>(this.engineUrl + 'game/homepage-info').pipe(
      map((homePageInfo: HomePageInfo | null) => {
        if (homePageInfo) {
          this.homePageInfoSource.next(homePageInfo);
        }
      })
    )
  }
}
