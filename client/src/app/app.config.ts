import { ApplicationConfig, APP_INITIALIZER } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { provideHttpClient, withInterceptors }      from '@angular/common/http';
import { provideAnimations }                        from '@angular/platform-browser/animations';
import { routes }                                   from './app.routes';
import { authInterceptor }                          from './core/interceptors/auth.interceptor';
import { AuthService }                              from './core/services/auth.service';

/**
 * On app startup, attempt to restore the session silently using the stored refresh token.
 * If no refresh token exists or it's expired, the user stays on the login page.
 */
function initAuth(auth: AuthService) {
  return () => {
    const restore$ = auth.restoreSession();
    return restore$ ? restore$.toPromise().catch(() => null) : Promise.resolve(null);
  };
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes, withComponentInputBinding()),
    provideHttpClient(withInterceptors([authInterceptor])),
    provideAnimations(),
    {
      provide:    APP_INITIALIZER,
      useFactory: initAuth,
      deps:       [AuthService],
      multi:      true
    }
  ]
};
