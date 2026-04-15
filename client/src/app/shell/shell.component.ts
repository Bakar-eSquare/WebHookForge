import { Component, OnInit, signal }    from '@angular/core';
import { RouterOutlet, RouterLink,
         RouterLinkActive, Router }       from '@angular/router';
import { WorkspaceService }              from '../core/services/workspace.service';
import { AuthService }                   from '../core/services/auth.service';
import { Workspace }                     from '../core/models/workspace.models';
import { SvgIconDirective }              from '../shared/directives/svg-icon.directive';

/**
 * Authenticated shell: renders the sidebar (workspace switcher + nav) and the
 * main content area where child routes are projected via <router-outlet>.
 */
@Component({
  selector:    'app-shell',
  standalone:  true,
  imports:     [RouterOutlet, RouterLink, RouterLinkActive, SvgIconDirective],
  templateUrl: './shell.component.html',
  styleUrls:   ['./shell.component.scss']
})
export class ShellComponent implements OnInit {

  workspaces = signal<Workspace[]>([]);
  activeWs   = signal<Workspace | null>(null);
  loading    = signal(true);
  menuOpen   = signal(false);
  readonly user = this.auth.currentUser;  // Signal<CurrentUser | null>

  constructor(
    private ws:     WorkspaceService,
    private auth:   AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.ws.getAll().subscribe({
      next: list => {
        this.workspaces.set(list);
        if (list.length > 0) this.activeWs.set(list[0]);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  selectWorkspace(ws: Workspace): void {
    this.activeWs.set(ws);
    this.menuOpen.set(false);
    this.router.navigate(['/workspaces', ws.id]);
  }

  logout(): void {
    this.auth.logout();
  }
}
