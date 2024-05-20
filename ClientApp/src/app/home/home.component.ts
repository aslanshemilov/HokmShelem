import { Component, OnInit } from '@angular/core';
import { HomeService } from './home.service';
import { AccountService } from '../account/account.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  constructor(private homeService: HomeService, private accountService: AccountService) { }

  ngOnInit(): void {
    if (!this.homeService.visited) {
      this.homeService.visitor().subscribe({
        next: _ => {
          this.homeService.visited = true;
        }
      });
    }
  }
}
