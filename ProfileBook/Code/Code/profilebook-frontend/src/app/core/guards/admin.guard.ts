import { Injectable } from '@angular/core';
import { CanActivate, Router, UrlTree } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({ providedIn: 'root' })
export class AdminGuard implements CanActivate {
  constructor(private auth: AuthService, private router: Router) {}
  canActivate(): boolean | UrlTree {
    if (!this.auth.isAuthenticated()) return this.router.parseUrl('/auth/login');
    if (this.auth.isAdmin()) return true;
    return this.router.parseUrl('/home');
  }
}
