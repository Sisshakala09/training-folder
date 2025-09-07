import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { filter, take, timeout, catchError, of } from 'rxjs';

@Component({
  selector: 'app-login',
  standalone: false,
  // ...existing code...
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  form: FormGroup;
  loading = false;
  error: string | null = null;

  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router) {
    this.form = this.fb.group({
      userNameOrEmail: ['', [Validators.required]],
      password: ['', [Validators.required]]
    });
  }

  // instrumented submit for debugging
  submit() {
    console.log('[Login] submit called', { valid: this.form.valid, value: this.form.value });
    if (this.form.invalid) {
      this.error = 'Please fill required fields';
      console.warn('[Login] form invalid, aborting');
      return;
    }

    this.loading = true;
    this.error = null;

    // ensure the auth service exists
    if (!this.auth || !this.auth.login) {
      console.error('[Login] AuthService or login method not available', this.auth);
      this.loading = false;
      this.error = 'Internal error (no auth service)';
      return;
    }

    // call login and log results
    this.auth.login(this.form.value as { userNameOrEmail: string; password: string }).subscribe({
      next: (resp) => {
        console.log('[Login] login response', resp);
        // Wait for /Auth/me to populate roles, then route
        this.auth.user$
          .pipe(
            filter((u: any) => !!u),
            take(1),
            timeout({ each: 2000, with: () => of(null) }),
            catchError(() => of(null))
          )
          .subscribe(() => {
            this.loading = false;
            const isAdmin = this.auth.isAdmin();
            this.router.navigate([isAdmin ? '/admin' : '/home']);
          });
      },
      error: (err) => {
        console.error('[Login] login error', err);
        this.loading = false;
        this.error = err?.error?.message || err?.message || 'Login failed';
      }
    });
  }
}
