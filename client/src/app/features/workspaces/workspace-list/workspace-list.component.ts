import { Component, OnInit, signal } from '@angular/core';
import { Router, RouterLink }        from '@angular/router';
import { FormsModule }               from '@angular/forms';
import { DatePipe }                  from '@angular/common';
import { WorkspaceService }          from '../../../core/services/workspace.service';
import { Workspace }                 from '../../../core/models/workspace.models';
import { SvgIconDirective }          from '../../../shared/directives/svg-icon.directive';

@Component({
  selector:    'app-workspace-list',
  standalone:  true,
  imports:     [RouterLink, FormsModule, DatePipe, SvgIconDirective],
  templateUrl: './workspace-list.component.html',
  styleUrls:   ['./workspace-list.component.scss']
})
export class WorkspaceListComponent implements OnInit {

  workspaces  = signal<Workspace[]>([]);
  loading     = signal(true);
  creating    = signal(false);
  showModal   = signal(false);
  newName     = '';
  createError = signal('');

  constructor(private svc: WorkspaceService, private router: Router) {}

  ngOnInit(): void {
    this.svc.getAll().subscribe({
      next: list => { this.workspaces.set(list); this.loading.set(false); },
      error: ()   => this.loading.set(false)
    });
  }

  openCreate(): void {
    this.newName = '';
    this.createError.set('');
    this.showModal.set(true);
  }

  create(): void {
    if (!this.newName.trim()) return;
    this.creating.set(true);
    this.svc.create({ name: this.newName.trim() }).subscribe({
      next: ws => {
        this.workspaces.update(list => [...list, ws]);
        this.showModal.set(false);
        this.creating.set(false);
        this.router.navigate(['/workspaces', ws.id]);
      },
      error: err => {
        this.createError.set(err?.error?.message ?? 'Failed to create workspace.');
        this.creating.set(false);
      }
    });
  }

  trackById(index: number, ws: Workspace) { return ws.id; }
}
