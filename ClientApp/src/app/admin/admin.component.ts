import { Component, OnInit } from '@angular/core';
import { AdminService } from './admin.service';
import { PaginatedResult } from '../shared/models/pagination/paginatedResult';
import { A_MemberParams } from '../shared/models/admin/a_memberParams';
import { A_Member } from '../shared/models/admin/a_member';
import { A_DropdownFilters } from '../shared/models/admin/a_dropdownFilters';
import { SharedService } from '../shared/shared.service';


@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss']
})

export class AdminComponent implements OnInit {
  aMemberParams = new A_MemberParams(10);
  paginatedMember?: PaginatedResult<A_Member[]>;
  a_dropdownFilters: A_DropdownFilters | undefined;
  pageSize = ['10', '25', '50', '100'];
  reset: boolean = false;

  constructor(private adminService: AdminService,
    private sharedService: SharedService) { }

  ngOnInit(): void {
    this.adminService.getAdminDropdownFilters().subscribe({
      next: result => {
        if (result) {
          this.a_dropdownFilters = result;
          this.loadMembers();
        }
      }
    });
  }

  loadMembers() {
    if (this.aMemberParams) {
      this.adminService.getPaginatedMembers(this.aMemberParams).subscribe({
        next: response => {
          if (response) {
            this.paginatedMember = response;
          }
        }
      })
    }
  }

  onPageChanged(event: any) {
    if (this.aMemberParams.pageNumber !== event) {
      this.aMemberParams.pageNumber = event;
      this.loadMembers();
    }
  }

  pageSizeEntry(pageSize: number) {
    this.aMemberParams.pageSize = pageSize;
    this.loadMembers();
  }

  onDropDownFilterEntries(value: any) {
    switch (value.action) {
      case 'role':
        this.aMemberParams.role = value.selectedValue;
        break;
      case 'provider':
        this.aMemberParams.provider = value.selectedValue;
        break;
      case 'status':
        this.aMemberParams.status = value.selectedValue;
        break;
    }

    this.loadMembers();
  }

  receivedSearchInput(value: any) {
    this.aMemberParams.search = value;
    this.aMemberParams.pageNumber = 1;
    this.loadMembers();
  }

  onSortBy(sortBy: any) {
    this.aMemberParams.sortBy = sortBy;
    this.loadMembers();
  }

  onReset() {
    this.sharedService.resetFilter();
    this.aMemberParams = new A_MemberParams(this.aMemberParams.pageSize);
    this.loadMembers();
  }
}
