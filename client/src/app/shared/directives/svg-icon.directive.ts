import { Directive, ElementRef, Input, OnChanges, inject } from '@angular/core';
import { IconRegistryService }                              from '../services/icon-registry.service';

/**
 * Renders a registered SVG icon as an inline SVG element.
 * Usage: <svg-icon name="plus" [size]="16" />
 */
@Directive({ selector: 'svg-icon', standalone: true })
export class SvgIconDirective implements OnChanges {
  @Input() name        = '';
  @Input() size        = 16;
  @Input() strokeWidth = 2;

  private el       = inject(ElementRef<HTMLElement>);
  private registry = inject(IconRegistryService);

  ngOnChanges(): void {
    const host = this.el.nativeElement;
    host.innerHTML = '';

    const svg = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
    svg.setAttribute('width',        String(this.size));
    svg.setAttribute('height',       String(this.size));
    svg.setAttribute('viewBox',      '0 0 24 24');
    svg.setAttribute('fill',         'none');
    svg.setAttribute('stroke',       'currentColor');
    svg.setAttribute('stroke-width', String(this.strokeWidth));
    svg.innerHTML = this.registry.get(this.name);

    host.appendChild(svg);
  }
}
