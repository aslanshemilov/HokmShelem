import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, catchError } from 'rxjs';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { SharedService } from 'src/app/shared/shared.service';
import { ApiResponse } from 'src/app/shared/models/apiResponse';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router, 
    private toastr: ToastrService,
    private sharedService: SharedService) { }

  intercept(req: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error) {
          let apiResponse: ApiResponse;
          apiResponse = error.error;

          if (error.status === 400) {
            if (error.error.displayByDefault) {
              this.sharedService.showNotification(false, error.error.title, error.error.message, error.error.isHtmlEnabled);
            }
          }

          if (error.status === 401) {
            this.sharedService.showNotification(false, error.error.title, error.error.message, error.error.isHtmlEnabled);
          }

          if (error.status === 404) {
            this.router.navigateByUrl('/not-found');
          }

          if (error.status === 500) {
            this.toastr.error(error.error.message);
          }

          throw apiResponse;
        }

        throw error;
      })
    );
  }
}
