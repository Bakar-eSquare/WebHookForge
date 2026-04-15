import { Component, OnInit, signal, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule }                        from '@angular/forms';
import { DatePipe }                           from '@angular/common';
import { switchMap }                          from 'rxjs';
import { WorkspaceService }                   from '../../../core/services/workspace.service';
import { EndpointService }                    from '../../../core/services/endpoint.service';
import { AuthService }                        from '../../../core/services/auth.service';
import { Workspace }                          from '../../../core/models/workspace.models';
import { Endpoint }                           from '../../../core/models/endpoint.models';
import { SvgIconDirective }                   from '../../../shared/directives/svg-icon.directive';

@Component({
  selector:    'app-workspace-detail',
  standalone:  true,
  imports:     [RouterLink, FormsModule, DatePipe, SvgIconDirective],
  templateUrl: './workspace-detail.component.html',
  styleUrls:   ['./workspace-detail.component.scss']
})
export class WorkspaceDetailComponent implements OnInit {

  private route  = inject(ActivatedRoute);
  private wsSvc  = inject(WorkspaceService);
  private epSvc  = inject(EndpointService);
  private auth   = inject(AuthService);
  private router = inject(Router);

  workspace    = signal<Workspace | null>(null);
  endpoints    = signal<Endpoint[]>([]);
  loading      = signal(true);

  showNewEp    = signal(false);
  creating     = signal(false);
  newEpName    = '';
  newEpDesc    = '';
  createError  = signal('');

  showMembers  = signal(false);
  addEmail     = '';
  addRole      = 'Viewer';
  addingMember = signal(false);
  memberError  = signal('');

  wsId = '';

  ngOnInit(): void {
    this.route.params.pipe(
      switchMap(p => {
        this.wsId = p['wsId'];
        this.loading.set(true);
        return this.wsSvc.getById(this.wsId);
      })
    ).subscribe({
      next: ws => {
        this.workspace.set(ws);
        this.epSvc.getByWorkspace(this.wsId).subscribe({
          next: eps => { this.endpoints.set(eps); this.loading.set(false); },
          error: ()  => this.loading.set(false)
        });
      },
      error: () => this.loading.set(false)
    });
  }

  createEndpoint(): void {
    if (!this.newEpName.trim()) return;
    this.creating.set(true);
    this.epSvc.create(this.wsId, { name: this.newEpName.trim(), description: this.newEpDesc.trim() || undefined }).subscribe({
      next: ep => {
        this.endpoints.update(list => [ep, ...list]);
        this.showNewEp.set(false);
        this.creating.set(false);
        this.newEpName = '';
        this.newEpDesc = '';
        this.router.navigate(['/workspaces', this.wsId, 'endpoints', ep.id]);
      },
      error: err => {
        this.createError.set(err?.error?.message ?? 'Failed to create endpoint.');
        this.creating.set(false);
      }
    });
  }

  deleteEndpoint(ep: Endpoint, ev: MouseEvent): void {
    ev.stopPropagation();
    ev.preventDefault();
    if (!confirm(`Delete endpoint "${ep.name}"? All captured requests will be lost.`)) return;
    this.epSvc.delete(ep.id).subscribe(() =>
      this.endpoints.update(list => list.filter(e => e.id !== ep.id))
    );
  }

  addMember(): void {
    if (!this.addEmail.trim()) return;
    this.addingMember.set(true);
    this.wsSvc.addMember(this.wsId, { email: this.addEmail.trim(), role: this.addRole }).subscribe({
      next: () => {
        this.wsSvc.getById(this.wsId).subscribe(ws => this.workspace.set(ws));
        this.addEmail = '';
        this.addingMember.set(false);
      },
      error: err => {
        this.memberError.set(err?.error?.message ?? 'Failed to add member.');
        this.addingMember.set(false);
      }
    });
  }

  copyUrl(url: string, ev: MouseEvent): void {
    ev.stopPropagation();
    ev.preventDefault();
    navigator.clipboard.writeText(url);
  }

  get ownerId() { return this.auth.currentUser()?.id; }

  trackById(index: number, ep: Endpoint) { return ep.id; }
}
