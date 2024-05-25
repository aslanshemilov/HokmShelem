import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { take } from 'rxjs';
import { AccountService } from 'src/app/account/account.service';

@Directive({
  selector: '[appUserHasRoles]'
})
export class UserHasRoleDirective implements OnInit {
  @Input() appUserHasRoles: string | null = null;

  constructor(private viewContainerRef: ViewContainerRef,
    private templateRef: TemplateRef<any>,
    private accountService: AccountService) { }

  ngOnInit(): void {
    this.accountService.applicationUser$.pipe((take(1))).subscribe({
      next: user => {
        if (user && this.appUserHasRoles) {
          const rolesToCheck = this.appUserHasRoles.split(',');
          if (user.roles.some((role: any) => rolesToCheck.includes(role))) {
            this.viewContainerRef.createEmbeddedView(this.templateRef);
          } else {
            this.viewContainerRef.clear();
          }
        } else {
          this.viewContainerRef.clear();
        }
      }
    })
  }
}
