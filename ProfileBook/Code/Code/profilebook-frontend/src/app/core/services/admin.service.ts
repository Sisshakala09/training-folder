// src/app/core/services/admin.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiBaseService } from './api-base.service';

export interface AdminUserDto {
  id: string;
  userName: string;
  email: string;
  roles?: string[];
  profileImage?: string | null;
}

export interface AdminUsersPage {
  total: number;
  page: number;
  pageSize: number;
  items: AdminUserDto[];
}

export interface AdminGroup {
  id: number;
  name: string;
  members: Array<string | AdminUserDto>;
}

@Injectable({ providedIn: 'root' })
export class AdminService extends ApiBaseService {
  constructor(http: HttpClient) { super(http); }

  // POST /Admin/users/create-with-profile (multipart/form-data)
  createUserWithProfile(opts: {
    userName: string;
    password: string;
    email: string;
    emailConfirmed: boolean;
    role: 'USER' | 'ADMIN';
    imageFile?: File | null;
  }): Observable<AdminUserDto> {
    const fd = new FormData();
    fd.append('UserName', opts.userName);
    fd.append('Password', opts.password);
    fd.append('Email', opts.email);
    fd.append('EmailConfirmed', String(opts.emailConfirmed));
    fd.append('Roles', opts.role); // API expects Roles=USER or Roles=ADMIN
    if (opts.imageFile) fd.append('ProfileImage', opts.imageFile);
    return this.http.post<AdminUserDto>(`${this.base}/Admin/users/create-with-profile`, fd);
  }

  // GET /Admin/users?page=1&pageSize=50
  listUsers(page = 1, pageSize = 50): Observable<AdminUsersPage> {
    return this.http.get<AdminUsersPage>(`${this.base}/Admin/users`, {
      params: { page: String(page), pageSize: String(pageSize) }
    });
  }

  // PUT /Admin/users/{id} (multipart/form-data)
  // Accepts any subset of fields: UserName, Email, Password, Roles, ProfileImage
  updateUser(id: string, opts: { userName?: string; email?: string; password?: string; roles?: 'USER'|'ADMIN'|string; profileImage?: File | null; }): Observable<AdminUserDto> {
    const fd = new FormData();
    if (opts.userName !== undefined) fd.append('UserName', opts.userName);
    if (opts.email !== undefined) fd.append('Email', opts.email);
    if (opts.password !== undefined) fd.append('Password', opts.password);
    if (opts.roles !== undefined && opts.roles !== null) fd.append('Roles', String(opts.roles));
    if (opts.profileImage) fd.append('ProfileImage', opts.profileImage);
    return this.http.put<AdminUserDto>(`${this.base}/Admin/users/${id}`, fd);
  }

  // DELETE /Admin/users/{id}
  deleteUser(id: string) {
    return this.http.delete(`${this.base}/Admin/users/${id}`);
  }

  // Groups
  // POST /Admin/groups { name, memberIds? }
  createGroup(payload: { name: string; memberIds?: string[] }): Observable<AdminGroup> {
    return this.http.post<AdminGroup>(`${this.base}/Admin/groups`, payload);
  }

  // GET /Admin/groups
  listGroups(): Observable<AdminGroup[]> {
    return this.http.get<AdminGroup[]>(`${this.base}/Admin/groups`);
  }

  // GET /Admin/groups/{id}
  getGroup(id: number | string): Observable<AdminGroup> {
    return this.http.get<AdminGroup>(`${this.base}/Admin/groups/${id}`);
  }

  // POST /Admin/groups/{id}/add/{userId}
  addUserToGroup(id: number | string, userId: string): Observable<string> {
    return this.http.post(`${this.base}/Admin/groups/${id}/add/${userId}`,
      null,
      { responseType: 'text' as 'json' }
    ) as unknown as Observable<string>;
  }

  // POST /Admin/groups/{id}/remove/{userId}
  removeUserFromGroup(id: number | string, userId: string): Observable<string> {
    return this.http.post(`${this.base}/Admin/groups/${id}/remove/${userId}`,
      null,
      { responseType: 'text' as 'json' }
    ) as unknown as Observable<string>;
  }
}
