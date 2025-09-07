// src/app/home/home.component.ts
import { Component, OnInit } from '@angular/core';
import { PostsService } from '../core/services/posts.service';
import { PostModel } from '../core/models/post.model';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-home',
  standalone: false,
  // ...existing code...
  template: `
  <main class="container" style="padding-top: 24px; padding-bottom: 24px;">
    <section *ngIf="loading" class="center"><div>Loading posts…</div></section>
    <section *ngIf="error" role="alert" class="center"><div class="error">{{error}}</div></section>

    <section *ngIf="!loading && !error">
      <div *ngIf="posts.length === 0" class="card">No posts yet.</div>

      <app-post-card *ngFor="let post of posts" [post]="post" [mediaBase]="mediaBase" style="margin-bottom:16px;"></app-post-card>
    </section>
  </main>
  `,
  styles: [`
    .center { display:flex; justify-content:center; align-items:center; padding: 24px; }
    .error { color: #b91c1c; }
  `]
})
export class HomeComponent implements OnInit {
  posts: PostModel[] = [];
  loading = false;
  error: string | null = null;
  mediaBase = ''; // e.g. http://localhost:5111

  constructor(private postsService: PostsService) {
    // derive media base from apiBaseUrl by stripping an ending /api if present
    this.mediaBase = environment.apiBaseUrl.replace(/\/api\/?$/i, '');
  }

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading = true;
    this.error = null;
    this.postsService.getAll().subscribe({
      next: p => { this.posts = p || []; this.loading = false; },
      error: e => { this.error = 'Could not load posts'; this.loading = false; }
    });
  }
}
