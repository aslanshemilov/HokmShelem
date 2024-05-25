import { DOCUMENT } from '@angular/common';
import { Component, Inject, OnInit } from '@angular/core';
import { take } from 'rxjs';
import { AccountService } from 'src/app/account/account.service';
import { HomeService } from 'src/app/home/home.service';
import { ApplicationUser } from 'src/app/shared/models/account/applicationUser';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {
  collapsed = true;
  darkMode: boolean | undefined;

  constructor(public accountService: AccountService,
    public homeService: HomeService,
    @Inject(DOCUMENT) private document: Document) { }

  ngOnInit(): void {
    const mode = localStorage.getItem(environment.modeKey);
    if (!mode || mode === 'light') {
      this.setLightMode();
    } else {
      this.setDarkMode();
    }
  }

  toggleCollapsed() {
    this.collapsed = !this.collapsed;
  }

  toggleTheme() {
    this.darkMode = !this.darkMode;

    if (this.darkMode) {
      this.setDarkMode();
    } else {
      this.setLightMode();
    }
  }

  logout() {
    this.accountService.logout();
  }

  private setLightMode() {
    this.document.getElementsByTagName('html')[0].setAttribute('data-bs-theme', 'light');
    this.darkMode = false;
    localStorage.setItem(environment.modeKey, 'light');
  }

  private setDarkMode() {
    this.document.getElementsByTagName('html')[0].setAttribute('data-bs-theme', 'dark');
    this.darkMode = true;
    localStorage.setItem(environment.modeKey, 'dark');
  }
}
