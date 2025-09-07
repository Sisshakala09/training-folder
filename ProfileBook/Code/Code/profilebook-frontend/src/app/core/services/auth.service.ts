// src/app/core/services/auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ApiBaseService } from './api-base.service';
import { BehaviorSubject, Observable, switchMap, tap, of } from 'rxjs';
import { UserModel } from '../models/user.model';

interface AuthResp { token: string; }

@Injectable({ providedIn: 'root' })
export class AuthService extends ApiBaseService {
  private tokenKey = 'pb_token';
  private userSub = new BehaviorSubject<UserModel | null>(null);
  user$ = this.userSub.asObservable();

  constructor(http: HttpClient) { super(http); this.load(); }

  private load() {
    const t = this.getToken();
    if (!t) {
      this.userSub.next(null);
      return;
    }
    // fetch profile
    this.http.get<UserModel>(`${this.base}/Auth/me`).subscribe({
      next: u => this.userSub.next(u),
      error: (err) => {
        if (err?.status === 401) {
          this.clearToken();
        } else {
          this.userSub.next(null);
        }
      }
    });
  }

  setToken(t: string) {
    localStorage.setItem(this.tokenKey, t);
    // after storing token, fetch profile
    this.http.get<UserModel>(`${this.base}/Auth/me`).subscribe({
      next: u => this.userSub.next(u),
      error: (err) => {
        if (err?.status === 401) {
          this.clearToken();
        } else {
          this.userSub.next(null);
        }
      }
    });
  }

  getToken(): string | null { return localStorage.getItem(this.tokenKey); }
  clearToken() { localStorage.removeItem(this.tokenKey); this.userSub.next(null); }

  register(payload: { username: string, email: string, password: string }): Observable<AuthResp> {
    return this.http.post<AuthResp>(`${this.base}/Auth/register`, payload).pipe(
      tap(r => { if (r?.token) this.setToken(r.token); })
    );
  }

  login(payload: { userNameOrEmail: string, password: string }): Observable<AuthResp> {
    return this.http.post<AuthResp>(`${this.base}/Auth/login`, payload).pipe(
      tap(r => { if (r?.token) this.setToken(r.token); })
    );
  }

  logout() { this.clearToken(); }
  isAuthenticated(): boolean { return !!this.getToken(); }

  // helpers for role-aware UI/guards
  get currentUser(): UserModel | null { return this.userSub.value; }
  isAdmin(): boolean {
    const u = this.userSub.value;
    const roles = (u?.roles || []).map(r => r?.toLowerCase?.() || r);
    return roles.includes('admin');
  }
}
