import { inject }       from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService }  from '../services/auth.service';

/** Route guard — redirects unauthenticated users to /login. */
export const authGuard: CanActivateFn = () => {
  const auth   = inject(AuthService);
  const router = inject(Router);
  return auth.isLoggedIn() ? true : router.createUrlTree(['/login']);
};

/** Redirect already-logged-in users away from /login and /register. */
export const guestGuard: CanActivateFn = () => {
  const auth   = inject(AuthService);
  const router = inject(Router);
  return auth.isLoggedIn() ? router.createUrlTree(['/workspaces']) : true;
};
