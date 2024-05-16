import { Injectable } from '@angular/core';
import { PaginatedResult } from '../shared/models/pagination/paginatedResult';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { A_MemberParams } from '../shared/models/admin/a_memberParams';
import { A_Member } from '../shared/models/admin/a_member';
import { A_DropdownFilters } from '../shared/models/admin/a_dropdownFilters';
import { map, of } from 'rxjs';
import { MemberAddEdit } from '../shared/models/admin/MemberAddEdit';
import { ApiResponse } from '../shared/models/apiResponse';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  apiUrl = environment.apiUrl;
  a_memberParams: A_MemberParams | undefined;
  a_dropdownFilters: A_DropdownFilters | undefined;

  constructor(private http: HttpClient) { }

  getPaginatedMembers(model: A_MemberParams) {
    let params = new HttpParams();

    params = params.append('pageNumber', model.pageNumber);
    params = params.append('pageSize', model.pageSize);
    params = params.append('role', model.role);
    params = params.append('status', model.status);
    params = params.append('provider', model.provider);
    params = params.append('search', model.search);
    params = params.append('sortBy', model.sortBy);

    return this.http.get<PaginatedResult<A_Member[]>>(this.apiUrl + 'admin/get-members', { params });
  }

  setAMemberParams(memberParams: A_MemberParams) {
    this.a_memberParams = memberParams;
  }

  getAdminDropdownFilters() {
    if (this.a_dropdownFilters) return of(this.a_dropdownFilters);

    return this.http.get<A_DropdownFilters>(this.apiUrl + 'admin/get-dropdown-filters').pipe(
      map((result: A_DropdownFilters) => {
        if (result) {
          this.a_dropdownFilters = result;
          return result;
        }

        return null;
      }))
  }

  getMember(id: string) {
    return this.http.get<MemberAddEdit>(this.apiUrl + 'admin/get-member/' + id);
  }
  addEditMember(model: MemberAddEdit) {
    console.log("works");
    return this.http.put<ApiResponse>(this.apiUrl + 'admin/update-member/', model);
  }
}
