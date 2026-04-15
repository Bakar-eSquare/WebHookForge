import { Pipe, PipeTransform } from '@angular/core';

/** Converts a Record to an array of [key, value] pairs for use in @for loops. */
@Pipe({ name: 'objectEntries', standalone: true, pure: true })
export class ObjectEntriesPipe implements PipeTransform {
  transform(obj?: Record<string, string>): [string, string][] {
    return obj ? Object.entries(obj) : [];
  }
}
