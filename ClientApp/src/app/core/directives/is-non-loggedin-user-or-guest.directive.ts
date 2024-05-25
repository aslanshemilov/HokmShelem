import { Directive, TemplateRef, ViewContainerRef } from '@angular/core';
import { take } from 'rxjs';
import { AccountService } from 'src/app/account/account.service';

@Directive({
  selector: '[appIsNonLoggedinUserOrGuestDirective]'
})
export class IsNonLoggedinUserOrGuestDirective {

  constructor(private viewContainerRef: ViewContainerRef,
    private templateRef: TemplateRef<any>,
    private accountService: AccountService) { }

  ngOnInit(): void {
    this.accountService.applicationUser$.pipe((take(1))).subscribe({
      next: user => {
        if (user) {
          this.viewContainerRef.clear();
        } else {
          this.viewContainerRef.createEmbeddedView(this.templateRef);
        }
      }
    })
  }
}
