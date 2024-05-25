import { Component, OnInit } from '@angular/core';
import { HomeService } from './home.service';
import { ApplicationUser } from '../shared/models/account/applicationUser';
import { SharedService } from '../shared/shared.service';
import { AccountService } from '../account/account.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  constructor(private homeService: HomeService,
    public accountService: AccountService,
    private sharedService: SharedService,
    private router: Router) { }

  ngOnInit(): void {
    if (!this.homeService.visited) {
      this.homeService.visitor().subscribe({
        next: _ => {
          this.homeService.visited = true;
        }
      });
    }
  }

  registerAsGuest() {
    this.accountService.registerAsGuest().subscribe({
      next: (user: ApplicationUser | null) => {
        if (user) {
          //this.sharedService.showNotification(true, 'Guest Player', 'Your name is <strong>' + user.playerName + '</strong>', true);
          this.router.navigateByUrl('/lobby');
        }
      }
    });
  }
}
