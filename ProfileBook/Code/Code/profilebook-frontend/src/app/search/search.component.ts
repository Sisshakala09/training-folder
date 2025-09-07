// src/app/search/search.component.ts
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UsersService, UserProfile } from '../core/services/users.service';
import { PostsService } from '../core/services/posts.service';
import { PostModel } from '../core/models/post.model';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-search',
  standalone: false,
  template: `
  <main class="container" style="padding-top:16px; padding-bottom:24px;">
    <div class="card" style="padding:12px; display:flex; gap:8px; align-items:center; margin-bottom:12px;">
      <input class="input" type="text" [(ngModel)]="q" (keyup.enter)="go()" placeholder="Search users or posts" style="flex:1;" />
      <button class="btn primary" (click)="go()">Search</button>
    </div>

    <section *ngIf="loading" class="card">Searching…</section>
    <section *ngIf="!loading">
      <div class="card" *ngIf="!users.length && !posts.length">No results.</div>

      <div *ngIf="users.length" class="card" style="margin-bottom:12px;">
        <h3 style="margin-top:0;">Users</h3>
        <a *ngFor="let u of users" [routerLink]="['/users', u.id]" style="display:flex; gap:10px; align-items:center; padding:10px; text-decoration:none; color:inherit; border-radius:8px;">
          <ng-container *ngIf="img(u.profileImageUrl || u.profileImage) as uimg; else initial">
            <img [src]="uimg" alt="avatar" style="width:36px; height:36px; border-radius:50%; object-fit:cover; border:1px solid #e5e7eb;"/>
          </ng-container>
          <ng-template #initial>
            <div class="avatar" [attr.data-initial]="(u.userName||u.email||'?')[0].toUpperCase()"></div>
          </ng-template>
          <div style="min-width:0;">
            <div style="font-weight:600;">{{ u.userName || u.email }}</div>
            <div class="small muted" style="white-space:nowrap; overflow:hidden; text-overflow:ellipsis;">{{ u.email }}</div>
          </div>
        </a>
      </div>

      <div *ngIf="posts.length" class="card">
        <h3 style="margin-top:0;">Posts</h3>
        <div *ngFor="let p of posts" style="padding:10px 0; border-top:1px solid #f3f4f6;">
          <a [routerLink]="['/posts', p.id]" style="display:flex; gap:10px; align-items:center; text-decoration:none; color:inherit;">
            <ng-container *ngIf="userImg(p.user) as pimg; else pInitial">
              <img [src]="pimg" alt="avatar" style="width:32px; height:32px; border-radius:50%; object-fit:cover; border:1px solid #e5e7eb;"/>
            </ng-container>
            <ng-template #pInitial>
              <div class="avatar" [attr.data-initial]="(p.user?.userName||'?')[0].toUpperCase()"></div>
            </ng-template>
            <div style="min-width:0;">
              <div style="font-weight:600; white-space:nowrap; overflow:hidden; text-overflow:ellipsis;">{{ p.content }}</div>
              <div class="small muted">by {{ p.user?.userName }}</div>
            </div>
          </a>
        </div>
      </div>
    </section>
  </main>
  `,
  styles: [`
    .avatar { width:32px; height:32px; border-radius:50%; background:#e5e7eb; display:inline-flex; align-items:center; justify-content:center; color:#374151; font-weight:700; border:1px solid #e5e7eb; }
    .avatar::after { content: attr(data-initial); }
  `]
})
export class SearchComponent implements OnInit {
  q = '';
  loading = false;
  users: UserProfile[] = [];
  posts: PostModel[] = [];
  mediaBase = (environment.mediaBaseUrl || '').replace(/\/+$/,'');

  constructor(private route: ActivatedRoute, private router: Router, private usersSvc: UsersService, private postsSvc: PostsService) {}

  ngOnInit(): void {
    this.route.queryParamMap.subscribe(params => {
      this.q = (params.get('q') || '').trim();
      if (this.q) this.search(); else { this.users = []; this.posts = []; }
    });
  }

  go() {
    const t = this.q.trim();
    this.router.navigate(['/search'], { queryParams: { q: t || undefined } });
  }

  private search() {
    const term = this.q.trim();
    if (!term) { this.users = []; this.posts = []; return; }
    this.loading = true;
    this.usersSvc.search(term).subscribe({ next: (u) => { this.users = u || []; this.loading = false; }, error: () => { this.users = []; this.loading = false; } });
    this.postsSvc.search(term).subscribe({ next: (p) => { this.posts = p || []; }, error: () => { this.posts = []; } });
  }

  img(url?: string | null): string | null {
    if (!url) return null;
    if (/^https?:\/\//i.test(url)) return url;
    return this.mediaBase + (url.startsWith('/') ? url : '/' + url);
  }

  userImg(u: any): string | null { return this.img(u?.profileImageUrl || u?.profileImage || null); }
}
