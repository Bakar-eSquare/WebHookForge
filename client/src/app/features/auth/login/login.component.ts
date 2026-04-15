import { Component, signal }    from '@angular/core';
import { Router, RouterLink }   from '@angular/router';
import { FormsModule }          from '@angular/forms';
import { AuthService }          from '../../../core/services/auth.service';
import { SvgIconDirective }     from '../../../shared/directives/svg-icon.directive';

@Component({
  selector:    'app-login',
  standalone:  true,
  imports:     [FormsModule, RouterLink, SvgIconDirective],
  templateUrl: './login.component.html',
  styleUrls:   ['./login.component.scss']
})
export class LoginComponent {

  email    = '';
  password = '';
  loading  = signal(false);
  error    = signal('');

  constructor(private auth: AuthService, private router: Router) {}

  submit(): void {
    if (!this.email || !this.password) return;
    this.loading.set(true);
    this.error.set('');

    this.auth.login({ email: this.email, password: this.password }).subscribe({
      next: () => this.router.navigate(['/workspaces']),
      error: err => {
        this.error.set(err?.error?.message ?? 'Invalid email or password.');
        this.loading.set(false);
      }
    });
  }
}
