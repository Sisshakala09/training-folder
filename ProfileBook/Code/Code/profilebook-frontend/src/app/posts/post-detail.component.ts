// src/app/posts/post-detail.component.ts
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';

import { PostsService } from '../core/services/posts.service';
import { AuthService } from '../core/services/auth.service';
import { PostModel } from '../core/models/post.model';

@Component({
  selector: 'app-post-detail',
  standalone: false,
  // ...existing code...
  template: `
  <main class="container" style="padding-top: 24px; padding-bottom: 24px;">
    <ng-container *ngIf="loading">Loading...</ng-container>

    <ng-container *ngIf="!loading && post">
      <!-- reuse PostCardComponent to render the post -->
  <app-post-card [post]="post" [mediaBase]="mediaBase" [clickable]="false" style="margin-bottom:16px;"></app-post-card>

      <section class="comments card" style="padding:16px;">
        <h3 style="margin-top:0;">Comments ({{ post.comments?.length || 0 }})</h3>

        <div *ngFor="let c of post.comments" class="comment" style="padding:12px 0; border-bottom:1px solid rgba(0,0,0,0.04);">
          <div style="font-weight:600">{{c.userName}}</div>
          <div style="color:#374151; margin-top:6px;">{{c.text}}</div>
          <div style="font-size:0.85rem; color:#6b7280; margin-top:6px;">{{ c.createdAt | date:'d MMMM y, h:mm a' }}</div>
        </div>

        <div *ngIf="auth.isAuthenticated(); else loginPrompt" style="margin-top:12px;">
          <textarea [(ngModel)]="newComment" rows="3" placeholder="Write a comment..." style="width:100%; padding:8px; border-radius:8px; border:1px solid rgba(0,0,0,0.06)"></textarea>
          <div style="margin-top:8px; display:flex; gap:8px; align-items:center;">
            <button class="btn-primary" (click)="submitComment()" [disabled]="addingComment">{{ addingComment ? 'Posting...' : 'Add comment' }}</button>
            <button class="btn-ghost" (click)="newComment = ''">Clear</button>
          </div>
        </div>

        <ng-template #loginPrompt>
          <div style="padding:12px; background:rgba(37,99,235,0.04); border-radius:8px;">
            <a routerLink="/auth/login">Log in</a> to add a comment.
          </div>
        </ng-template>
      </section>
    </ng-container>

    <ng-container *ngIf="!loading && !post">
      <div class="card">Post not found.</div>
    </ng-container>
  </main>
  `
})
export class PostDetailComponent implements OnInit {
  post?: PostModel;
  loading = false;
  addingComment = false;
  newComment = '';
  mediaBase = '';

  constructor(
    private route: ActivatedRoute,
    private posts: PostsService,
    public auth: AuthService
  ) {
    // derive media base from environment if needed; safe default:
    try {
      // if your environment.apiBaseUrl is like http://localhost:5111/api, strip /api
      // import of environment at top is avoided to keep file self-contained; you can set mediaBase explicitly if desired
      this.mediaBase = (window.location.origin);
    } catch {
      this.mediaBase = '';
    }
  }

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!id) {
      return;
    }
    this.loadPost(id);
  }

  loadPost(id: number) {
    this.loading = true;
    this.posts.getById(id).subscribe({
      next: p => { this.post = p; this.loading = false; },
      error: () => { this.loading = false; this.post = undefined; }
    });
  }

  submitComment() {
    if (!this.auth.isAuthenticated()) {
      // keep it simple: redirect to login
      // could store return url to come back after login
      location.href = '/auth/login';
      return;
    }
    if (!this.post || !this.newComment.trim()) return;

    this.addingComment = true;
    this.posts.addComment(this.post.id, this.newComment.trim()).subscribe({
      next: (c) => {
        this.post!.comments = this.post!.comments || [];
        this.post!.comments.push(c);
        this.newComment = '';
        this.addingComment = false;
      },
      error: (e) => {
        console.error('Add comment failed', e);
        this.addingComment = false;
      }
    });
  }
}
