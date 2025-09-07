import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment.prod';
import { tap } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class AuthService {
  tokenKey = 'pb_token';
  constructor(private http: HttpClient) {}
  register(data:any){ return this.http.post(`${environment.apiUrl}/api/auth/register`, data); }
  login(data:any){ return this.http.post<any>(`${environment.apiUrl}/api/auth/login`, data).pipe(tap(res => { if(res?.token) localStorage.setItem(this.tokenKey, res.token); })); }
  getToken(){ return localStorage.getItem(this.tokenKey); }
  logout(){ localStorage.removeItem(this.tokenKey); }
}
