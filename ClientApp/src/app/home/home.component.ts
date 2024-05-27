import { Component, OnDestroy, OnInit } from '@angular/core';
import { HomeService } from './home.service';
import { ApplicationUser } from '../shared/models/account/applicationUser';
import { AccountService } from '../account/account.service';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { SharedService } from '../shared/shared.service';


@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit, OnDestroy {
  private intervalId: any;
  private subscription: Subscription | undefined;

  constructor(public homeService: HomeService,
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

    this.startHomePageInfoApiCall();
  }

  ngOnDestroy(): void {
    this.stopHomePageInfoApiCall();
  }

  registerAsGuest() {
    this.accountService.registerAsGuest().subscribe({
      next: (user: ApplicationUser | null) => {
        if (user) {
          this.sharedService.showNotification(true, 'Guest Player', 'Your name is <strong>' + user.playerName + '</strong>', true);
          this.router.navigateByUrl('/lobby');
        }
      }
    });
  }

  private startHomePageInfoApiCall(): void {
    this.homeService.getHomePageInfo().subscribe({
      next: _ => {
        this.intervalId = setInterval(() => {
          this.homeService.getHomePageInfo().subscribe({
            error: _ => {
              this.stopHomePageInfoApiCall();
            }
          })
        }, 30000); 
      }
    });    
  }

  private stopHomePageInfoApiCall(): void {
    if (this.intervalId) {
      clearInterval(this.intervalId);
    }
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
