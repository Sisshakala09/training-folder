// src/app/core/services/users.service.ts
import { Injectable } from '@angular/core';
import { ApiBaseService } from './api-base.service';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface UserProfile {
  id: string;
  userName: string;
  email?: string;
  profileImage?: string;    // if API uses different name adjust
  profileImageUrl?: string; // keep both possibilities supported
  posts?: any[];            // API returns posts array; type as needed
}

export interface UpdateProfileRequest {
  userName?: string;
  email?: string;
  bio?: string;
  currentPassword?: string;
  newPassword?: string;
}

export interface UpdateProfileResponse {
  id: string;
  username: string;
  email: string;
  profileImage: string | null;
}

@Injectable({ providedIn: 'root' })
export class UsersService extends ApiBaseService {
  constructor(http: HttpClient) { super(http); }

  // GET /api/Users/{id}
  getById(id: string): Observable<UserProfile> {
    return this.http.get<UserProfile>(`${this.base}/Users/${id}`);
  }

  // PUT /api/Users/profile (send only fields to update)
  updateProfile(payload: UpdateProfileRequest): Observable<UpdateProfileResponse> {
    return this.http.put<UpdateProfileResponse>(`${this.base}/Users/profile`, payload);
  }

  // POST /api/Users/profile/upload-image (multipart/form-data)
  uploadProfileImage(file: File): Observable<UpdateProfileResponse> {
    const form = new FormData();
    form.append('file', file);
    return this.http.post<UpdateProfileResponse>(`${this.base}/Users/profile/upload-image`, form);
  }

  // GET /api/Users/search?query=...
  search(query: string): Observable<UserProfile[]> {
    return this.http.get<UserProfile[]>(`${this.base}/Users/search`, { params: { query } });
  }
}
