import { Pipe, PipeTransform } from '@angular/core';

/** Pretty-prints a JSON string; returns the original string on parse failure. */
@Pipe({ name: 'prettyJson', standalone: true, pure: true })
export class PrettyJsonPipe implements PipeTransform {
  transform(raw?: string): string {
    if (!raw) return '';
    try { return JSON.stringify(JSON.parse(raw), null, 2); }
    catch { return raw; }
  }
}
