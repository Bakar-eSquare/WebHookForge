import { Component, OnInit, signal, inject } from '@angular/core';
import { ActivatedRoute, RouterLink }         from '@angular/router';
import { DatePipe, NgClass }                  from '@angular/common';
import { RequestService }                     from '../../../core/services/request.service';
import { IncomingRequest }                    from '../../../core/models/request.models';
import { SvgIconDirective }                   from '../../../shared/directives/svg-icon.directive';
import { HttpMethodPipe }                     from '../../../shared/pipes/http-method.pipe';
import { FileSizePipe }                       from '../../../shared/pipes/file-size.pipe';
import { PrettyJsonPipe }                     from '../../../shared/pipes/pretty-json.pipe';
import { ObjectEntriesPipe }                  from '../../../shared/pipes/object-entries.pipe';

@Component({
  selector:    'app-request-detail',
  standalone:  true,
  imports:     [RouterLink, DatePipe, NgClass, SvgIconDirective, HttpMethodPipe, FileSizePipe, PrettyJsonPipe, ObjectEntriesPipe],
  templateUrl: './request-detail.component.html',
  styleUrls:   ['./request-detail.component.scss']
})
export class RequestDetailComponent implements OnInit {

  private route = inject(ActivatedRoute);
  private svc   = inject(RequestService);

  request = signal<IncomingRequest | null>(null);
  loading = signal(true);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.svc.getById(id).subscribe({
      next: r  => { this.request.set(r); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  copy(text: string): void { navigator.clipboard.writeText(text); }
}
