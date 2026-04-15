import { Pipe, PipeTransform } from '@angular/core';

/** Transforms an HTTP method string into a badge CSS class name. */
@Pipe({ name: 'httpMethod', standalone: true, pure: true })
export class HttpMethodPipe implements PipeTransform {
  transform(method: string): string {
    return `method-${method.toLowerCase()}`;
  }
}
