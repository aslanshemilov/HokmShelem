import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { MessageAdd } from '../shared/models/hokmshelem/messageAdd';
import { ApiResponse } from '../shared/models/apiResponse';
import { ApplicationUser } from '../shared/models/account/applicationUser';
import { ReplaySubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class HomeService {
  apiUrl = environment.apiUrl;
  visited = false;

  private guestUserSource = new ReplaySubject<ApplicationUser | null>(1);
  guestUser$ = this.guestUserSource.asObservable();

  constructor(private http: HttpClient) { }

  visitor() {
    return this.http.get(this.apiUrl + 'hokmshelem');
  }

  addMessage(model: MessageAdd) {
    return this.http.post<ApiResponse>(this.apiUrl + 'hokmshelem/add-message', model);
  }
}
