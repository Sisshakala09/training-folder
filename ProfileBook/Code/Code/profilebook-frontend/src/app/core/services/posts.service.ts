// src/app/core/services/posts.service.ts
import { Injectable } from '@angular/core';
import { ApiBaseService } from './api-base.service';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PostModel, PostComment } from '../models/post.model';

@Injectable({ providedIn: 'root' })
export class PostsService extends ApiBaseService {
  constructor(http: HttpClient) { super(http); }

  getAll(): Observable<PostModel[]> {
    return this.http.get<PostModel[]>(`${this.base}/Posts`);
  }

  getById(postId: number) {
    return this.http.get<PostModel>(`${this.base}/Posts/${postId}`);
 }


  // Like a post — adjust endpoint if your API differs (some APIs use POST /Posts/{id}/like)
  likePost(postId: number): Observable<any> {
    return this.http.post(`${this.base}/Posts/${postId}/like`, {});
  }

  // Add comment — adjust path/body as needed
  addComment(postId: number, text: string): Observable<PostComment> {
    return this.http.post<PostComment>(`${this.base}/Posts/${postId}/comments`, { text });
  }

  // Create a new post with multipart/form-data
  create(content: string, imageFile?: File): Observable<PostModel> {
    const form = new FormData();
    form.append('content', content ?? '');
    if (imageFile) form.append('image', imageFile);
    return this.http.post<PostModel>(`${this.base}/Posts`, form);
  }

  // GET /api/Posts?q=...
  search(q: string): Observable<PostModel[]> {
    return this.http.get<PostModel[]>(`${this.base}/Posts`, { params: { q } });
  }

  // Admin: Approve a post
  approvePost(postId: number): Observable<any> {
    return this.http.put(`${this.base}/Posts/${postId}/approve`, {});
  }

  // Admin: Reject a post
  rejectPost(postId: number): Observable<any> {
    return this.http.put(`${this.base}/Posts/${postId}/reject`, {});
  }
}
