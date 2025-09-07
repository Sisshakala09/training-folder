// src/app/shared/post-card/post-card.component.ts
import { Component, Input } from '@angular/core';
import { PostModel } from '../../core/models/post.model';
import { PostsService } from '../../core/services/posts.service';
import { AuthService } from '../../core/services/auth.service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-post-card',
  standalone: false,
  // ...existing code...
  template: `
  <article class="post-card card">
    <header class="post-header">
        <a class="profile-link" [routerLink]="['/users', post.user?.userId || post.user?.id]" title="View profile">
            <ng-container *ngIf="profileImageUrl; else avatarInitial">
              <img class="avatar" [src]="profileImageUrl!" alt="avatar" />
            </ng-container>
            <ng-template #avatarInitial>
              <div class="avatar-initial" aria-label="user initial">{{ userInitial }}</div>
            </ng-template>
        </a>

        <div class="meta">
            <div class="row">
            <a class="profile-link username-link" [routerLink]="['/users', post.user?.userId || post.user?.id]">
                <span class="username">{{ post.user?.userName || post.user?.userName || 'Unknown' }}</span>
            </a>
            <span *ngIf="post.isMine" class="mine-badge" title="This is your post">Mine</span>
            <span class="time">{{ post.createdAt ? (post.createdAt | date:'d MMMM y, h:mm a') : '' }}</span>
            </div>
        </div>

        <div class="actions" *ngIf="post.isMine">
            <!-- ... -->
        </div>
    </header>


    <p class="content">
      <a *ngIf="clickable; else plainContent" class="content-link" [routerLink]="['/posts', post.id]">{{ post.content }}</a>
      <ng-template #plainContent>
        <span class="content-text">{{ post.content }}</span>
      </ng-template>
    </p>

    <ng-container *ngIf="postImageUrl">
      <a *ngIf="clickable; else plainImage" class="image-link" [routerLink]="['/posts', post.id]" aria-label="Open post details">
        <img class="post-image" [src]="postImageUrl" alt="post image" />
      </a>
      <ng-template #plainImage>
        <img class="post-image" [src]="postImageUrl" alt="post image" />
      </ng-template>
    </ng-container>

    <footer class="post-footer">
      <div class="counts">
        <button class="icon-btn" (click)="handleLike()" [disabled]="liking" aria-label="Like">
          <svg width="16" height="16" viewBox="0 0 24 24" fill="currentColor" style="margin-right:6px">
            <path d="M12 21.35l-1.45-1.32C5.4 15.36 2 12.28 2 8.5 2 6 4 4 6.5 4 8 4 9.5 5 10.3 6.2 11.1 5 12.6 4 14.1 4 16.6 4 18.6 6 18.6 8.5c0 3.78-3.4 6.86-8.55 11.54L12 21.35z"></path>
          </svg>
          <span class="count">{{ localLikes }}</span>
        </button>

        <a *ngIf="clickable; else commentsStatic" class="icon-btn" [routerLink]="['/posts', post.id]" role="link" aria-label="Comments">
          <svg width="16" height="16" viewBox="0 0 24 24" fill="currentColor" style="margin-right:6px">
            <path d="M21 6h-2v9H7v2c0 .55.45 1 1 1h9l4 4V7c0-.55-.45-1-1-1zM3 6h12v9H5l-2 2V6c0-.55.45-1 1-1z"/>
          </svg>
          <span class="count">{{ post.comments?.length || 0 }}</span>
        </a>
        <ng-template #commentsStatic>
          <span class="icon-btn" aria-label="Comments">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="currentColor" style="margin-right:6px">
              <path d="M21 6h-2v9H7v2c0 .55.45 1 1 1h9l4 4V7c0-.55-.45-1-1-1zM3 6h12v9H5l-2 2V6c0-.55.45-1 1-1z"/>
            </svg>
            <span class="count">{{ post.comments?.length || 0 }}</span>
          </span>
        </ng-template>
      </div>
    </footer>
  </article>
  `,
  styles: [`
    .post-card { padding: 14px; border-radius: 14px; background: #fff; color: #111; box-shadow: 0 8px 24px rgba(2,6,23,0.06); margin-bottom:16px;}
    .post-header { display:flex; align-items:center; gap:12px; margin-bottom:8px; }
  .avatar { width:40px; height:40px; border-radius:50%; object-fit:cover; border:1px solid rgba(0,0,0,0.04); flex-shrink:0; }
  .avatar-initial { width:40px; height:40px; border-radius:50%; display:flex; align-items:center; justify-content:center; background:#e5e7eb; color:#1f2937; font-weight:700; border:1px solid rgba(0,0,0,0.04); flex-shrink:0; }
    .meta { flex:1; }
    .row { display:flex; align-items:center; gap:12px; }
    .username { font-weight:600; font-size:0.95rem; color:#111827; }
  .mine-badge { background:#e0f2fe; color:#075985; font-weight:600; font-size:0.72rem; padding:2px 6px; border-radius:999px; border:1px solid #bae6fd; }
    .time { font-size:0.82rem; color:#6b7280; }
  .content { margin: 8px 0 12px 0; color:#1f2937; }
  .content-link { color: inherit; text-decoration: none; }
  .content-link:hover { text-decoration: underline; }
  .image-link { display:block; text-decoration:none; }
  /* Make image smaller without cropping: scale down and keep aspect ratio */
    .post-image {
      display:block;
      max-width:100%;
      width:auto;
      height:auto;
      max-height:200px;
      border-radius:10px;
      margin-bottom:12px;
    }
    .post-footer { display:flex; align-items:center; }
    .counts { display:flex; gap:12px; align-items:center; }
  .icon-btn { display:inline-flex; align-items:center; gap:6px; background:#fff; border:1px solid #e5e7eb; color:#374151; cursor:pointer; padding:6px 10px; border-radius:10px; text-decoration:none; box-shadow:0 1px 2px rgba(0,0,0,0.02); }
  .icon-btn:hover { background:#f9fafb; border-color:#e5e7eb; }
  .icon-btn:active { background:#f3f4f6; }
    .count { font-weight:600; font-size:0.92rem; }
    .btn-ghost.small { padding:4px 8px; font-size:0.85rem; }
  `]
})
export class PostCardComponent {
  @Input({ required: true }) post!: PostModel;
  @Input() mediaBase = '';
  @Input() clickable = true;

  liking = false;
  localLikes = 0;

  constructor(
    private postsService: PostsService,
    private auth: AuthService
  ) {}

  ngOnInit() {
    this.localLikes = this.post.likesCount || 0;
  }

  get profileImageUrl(): string | null {
    const u = this.post.user?.profileImageUrl || (this.post.user as any)?.profileImage;
    if (!u) return null;
        // If API already returned an absolute URL, use it unchanged
        if (/^https?:\/\//i.test(u)) return u;

        // otherwise build absolute URL using environment.mediaBaseUrl (fallback)
        const base = (this.mediaBase?.replace(/\/+$/,'') || environment.mediaBaseUrl?.replace(/\/+$/,'') || '');
        return base ? `${base}${u.startsWith('/') ? u : '/' + u}` : (u.startsWith('/') ? u : '/' + u);
    }

    get postImageUrl(): string | null {
        const u = this.post.imagePath;
        if (!u) return '/assets/default-avatar.png';
        // If API already returned an absolute URL, use it unchanged
        if (/^https?:\/\//i.test(u)) return u;

        // otherwise build absolute URL using environment.mediaBaseUrl (fallback)
        const base = (this.mediaBase?.replace(/\/+$/,'') || environment.mediaBaseUrl?.replace(/\/+$/,'') || '');
        return base ? `${base}${u.startsWith('/') ? u : '/' + u}` : (u.startsWith('/') ? u : '/' + u);
    }

  handleLike() {
    if (!this.auth.isAuthenticated()) {
      // store intended action and route after login (optional)
      this.routerNavigateToLogin();
      return;
    }

    this.liking = true;
    this.localLikes++;
    this.postsService.likePost(this.post.id).subscribe({
      next: () => { this.liking = false; },
      error: () => { this.localLikes = Math.max(0, this.localLikes - 1); this.liking = false; }
    });
  }

  get userInitial(): string {
    const name = this.post.user?.userName || this.post.user?.username || 'U';
    const first = (name || 'U').trim().charAt(0) || 'U';
    return first.toUpperCase();
  }

  private routerNavigateToLogin() {
    // Use location change to preserve behavior without injecting Router directly
    // You can replace with Router.navigate if you prefer to inject Router
    location.href = '/auth/login';
  }

  onEdit() { console.log('edit', this.post.id); }
  onDelete() { console.log('delete', this.post.id); }
}
