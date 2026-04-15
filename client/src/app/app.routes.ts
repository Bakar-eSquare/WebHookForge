import { Routes } from '@angular/router';
import { authGuard, guestGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/workspaces', pathMatch: 'full' },

  /* ── Public / guest routes ─────────────────────────────────────────────── */
  {
    path: 'login',
    canActivate: [guestGuard],
    loadComponent: () => import('./features/auth/login/login.component')
      .then(m => m.LoginComponent)
  },
  {
    path: 'register',
    canActivate: [guestGuard],
    loadComponent: () => import('./features/auth/register/register.component')
      .then(m => m.RegisterComponent)
  },

  /* ── Authenticated shell (sidebar + main content) ──────────────────────── */
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () => import('./shell/shell.component').then(m => m.ShellComponent),
    children: [
      {
        path: 'workspaces',
        loadComponent: () => import('./features/workspaces/workspace-list/workspace-list.component')
          .then(m => m.WorkspaceListComponent)
      },
      {
        path: 'workspaces/:wsId',
        loadComponent: () => import('./features/workspaces/workspace-detail/workspace-detail.component')
          .then(m => m.WorkspaceDetailComponent)
      },
      {
        path: 'workspaces/:wsId/endpoints/:id',
        loadComponent: () => import('./features/endpoints/endpoint-detail/endpoint-detail.component')
          .then(m => m.EndpointDetailComponent)
      },
      {
        path: 'requests/:id',
        loadComponent: () => import('./features/requests/request-detail/request-detail.component')
          .then(m => m.RequestDetailComponent)
      }
    ]
  },

  { path: '**', redirectTo: '/workspaces' }
];
