import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { MessageAdd } from '../shared/models/hokmshelem/messageAdd';
import { ApiResponse } from '../shared/models/apiResponse';

@Injectable({
  providedIn: 'root'
})
export class HomeService {
  apiUrl = environment.apiUrl;
  visited = false;

  constructor(private http: HttpClient) { }

  visitor() {
    return this.http.get(this.apiUrl + 'hokmshelem');
  }

  addMessage(model: MessageAdd) {
    return this.http.post<ApiResponse>(this.apiUrl + 'hokmshelem/add-message', model);
  }
}
