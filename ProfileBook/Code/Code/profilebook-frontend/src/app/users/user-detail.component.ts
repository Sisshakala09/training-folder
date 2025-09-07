// src/app/users/user-detail.component.ts
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { UsersService, UserProfile } from '../core/services/users.service';
import { AuthService } from '../core/services/auth.service';
import { ReportsService } from '../core/services/reports.service';
import { PostCardComponent } from '../shared/post-card/post-card.component';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-user-detail',
  standalone: false,
  // ...existing code...
  template: `
  <main class="container" style="padding-top:24px; padding-bottom:24px;">
    <section *ngIf="loading" class="center">Loading user…</section>

    <section *ngIf="!loading && user" class="user-page">
      <div class="user-card card" style="display:flex; gap:16px; align-items:center; padding:18px;">
        <ng-container *ngIf="profileUrl; else initialAvatar">
          <img [src]="profileUrl!" alt="avatar" style="width:80px; height:80px; border-radius:50%; object-fit:cover;"/>
        </ng-container>
        <ng-template #initialAvatar>
          <div class="avatar-initial" aria-label="user initial">{{ userInitial }}</div>
        </ng-template>

        <div style="flex:1;">
          <h2 style="margin:0; display:flex; align-items:center; gap:10px;">
            <span>{{ user.userName }}</span>
            <span class="muted" style="font-weight:600; font-size:0.9rem;">• {{ (user.posts?.length || 0) }} posts</span>
          </h2>
          <div class="muted">{{ user.email }}</div>
        </div>

        <div class="actions" style="display:flex; gap:8px;">
          <button *ngIf="isMe; else otherActions" class="btn-ghost small" (click)="onEditProfile()">Edit Profile</button>
          <ng-template #otherActions>
            <button class="btn-ghost small" (click)="onMessageUser()">Message</button>
            <button class="btn-ghost small" (click)="onReportUser()">Report User</button>
          </ng-template>
        </div>
      </div>

      <h3 style="margin-top:20px;">Posts</h3>
      <section *ngIf="(user.posts?.length || 0) === 0" class="card">No posts yet.</section>

      <div *ngFor="let p of user.posts">
        <!-- reuse post-card; posts coming from user endpoint may have slightly different shape -->
        <app-post-card [post]="normalizePost(p)" [mediaBase]="mediaBase" style="margin-bottom:16px;"></app-post-card>
      </div>
    </section>

    <section *ngIf="!loading && !user" class="card">User not found.</section>
  </main>
  `,
  styles: [`
    .avatar-initial { width:80px; height:80px; border-radius:50%; display:flex; align-items:center; justify-content:center; background:#e5e7eb; color:#1f2937; font-weight:800; font-size:1.4rem; border:1px solid rgba(0,0,0,0.06); }
    .muted { color:#6b7280; }
  `]
})
export class UserDetailComponent implements OnInit {
  user?: UserProfile | null;
  loading = false;
  mediaBase = '';
  isMe = false;

  constructor(private route: ActivatedRoute, private users: UsersService, private auth: AuthService, private router: Router, private reports: ReportsService) {
    this.mediaBase = (environment.mediaBaseUrl || '').replace(/\/+$/,'');
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;
    this.loading = true;
    this.users.getById(id).subscribe({
      next: u => { this.user = u; this.computeIsMe(); this.loading = false; },
      error: () => { this.user = null; this.loading = false; }
    });
  }

  // normalize shape of a user's post to the PostModel expected by PostCardComponent
  normalizePost(p: any) {
    return {
      id: p.id,
      content: p.content,
      imagePath: p.imagePath ?? p.imageURL ?? p.image,   // try common alternatives
      createdAt: p.createdAt,
      status: p.status,
      user: {
        userId: this.user?.id || this.user?.id,
        userName: this.user?.userName || this.user?.userName,
        profileImageUrl: this.user?.profileImage || this.user?.profileImageUrl
      },
      likesCount: p.likesCount,
      comments: p.comments || [],
      isMine: p.isMine
    };
  }

  get profileUrl(): string {
    const u = this.user?.profileImageUrl || (this.user as any)?.profileImage;
    if (!u) return '';
    if (/^https?:\/\//i.test(u)) return u;
    return this.mediaBase + (u.startsWith('/') ? u : '/' + u);
  }

  get userInitial(): string {
    const name = this.user?.userName || 'U';
    return (name.trim().charAt(0) || 'U').toUpperCase();
  }

  private computeIsMe() {
    // compare route id to current auth user id if available
    const current = (this.auth as any).userSub?.value as any;
    const currentId = current?.id || current?.userId;
    this.isMe = !!(currentId && this.user?.id && String(currentId) === String(this.user.id));
  }

  onEditProfile() { this.router.navigateByUrl('/profile/edit'); }
  onReportUser() {
    if (!this.user?.id) return;
    const reason = prompt('Please enter a reason for reporting this user:');
    if (reason && reason.trim()) {
      this.reports.reportUser(this.user.id, reason.trim()).subscribe({
        next: (r) => {
          alert('Report submitted. Status: ' + (r?.status || 'Open'));
          // Optionally navigate to reports page
          // this.router.navigate(['/reports']);
        },
        error: (e) => {
          alert(e?.error?.message || 'Failed to submit report');
        }
      });
    }
  }
  onMessageUser() { if (this.user?.id) this.router.navigate(['/chat', this.user.id]); }
}
