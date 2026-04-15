import { Component, signal }   from '@angular/core';
import { Router, RouterLink }  from '@angular/router';
import { FormsModule }         from '@angular/forms';
import { AuthService }         from '../../../core/services/auth.service';
import { SvgIconDirective }    from '../../../shared/directives/svg-icon.directive';

@Component({
  selector:    'app-register',
  standalone:  true,
  imports:     [FormsModule, RouterLink, SvgIconDirective],
  templateUrl: './register.component.html',
  styleUrls:   ['./register.component.scss']
})
export class RegisterComponent {

  displayName = '';
  email       = '';
  password    = '';
  confirm     = '';
  loading     = signal(false);
  error       = signal('');

  constructor(private auth: AuthService, private router: Router) {}

  submit(): void {
    if (!this.displayName || !this.email || !this.password) return;
    if (this.password !== this.confirm) {
      this.error.set('Passwords do not match.');
      return;
    }
    this.loading.set(true);
    this.error.set('');

    this.auth.register({ displayName: this.displayName, email: this.email, password: this.password }).subscribe({
      next: () => this.router.navigate(['/workspaces']),
      error: err => {
        this.error.set(err?.error?.message ?? 'Registration failed. Please try again.');
        this.loading.set(false);
      }
    });
  }
}
