import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

/** Root application shell — just renders the active route. */
@Component({
  selector:    'app-root',
  standalone:  true,
  imports:     [RouterOutlet],
  template:    '<router-outlet />',
})
export class AppComponent {}
