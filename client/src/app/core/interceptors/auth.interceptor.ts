import { HttpInterceptorFn, HttpRequest, HttpHandlerFn, HttpErrorResponse } from '@angular/common/http';
import { inject }       from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService }  from '../services/auth.service';

/**
 * Functional HTTP interceptor (Angular 15+ style — no class, no boilerplate).
 *
 * Responsibilities:
 *   1. Attach the in-memory Bearer token to every outgoing request.
 *   2. On 401 — attempt a silent token refresh once.
 *      If refresh succeeds, replay the original request with the new token.
 *      If refresh fails, force logout.
 */
export const authInterceptor: HttpInterceptorFn = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn
) => {
  const authService = inject(AuthService);
  const token       = authService.getAccessToken();

  const authorised = token ? addToken(req, token) : req;

  return next(authorised).pipe(
    catchError((err: HttpErrorResponse) => {
      // Only retry on 401 and only for non-auth endpoints (avoid infinite loop)
      if (err.status === 401 && !req.url.includes('/auth/')) {
        const refresh$ = authService.restoreSession();
        if (refresh$) {
          return refresh$.pipe(
            switchMap(res => next(addToken(req, res.accessToken))),
            catchError(refreshErr => {
              authService.logout();
              return throwError(() => refreshErr);
            })
          );
        }
        authService.logout();
      }
      return throwError(() => err);
    })
  );
};

function addToken(req: HttpRequest<unknown>, token: string): HttpRequest<unknown> {
  return req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
}
