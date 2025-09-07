// src/app/shared/navbar/navbar.component.ts
import { Component } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { Location } from '@angular/common';
import { UsersService, UserProfile } from '../../core/services/users.service';
import { PostsService } from '../../core/services/posts.service';
import { PostModel } from '../../core/models/post.model';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-navbar',
  standalone: false,
  // ...existing code...
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent {
  showBack = false;
  mediaBase = (environment.mediaBaseUrl || '').replace(/\/+$/,'');
  constructor(
    public auth: AuthService,
    private router: Router,
    private location: Location,
    private usersSvc: UsersService,
    private postsSvc: PostsService,
  ) {
    this.router.events.subscribe(ev => {
      if (ev instanceof NavigationEnd) {
        const url = ev.urlAfterRedirects || ev.url;
        // Hide back on home and auth pages
        this.showBack = !(url === '/home' || url === '/' || url.startsWith('/auth'));
        // Close search on navigation
        this.open = false;
        this.q = '';
      }
    });
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/auth/login']);
  }

  goBack() {
    // If there's history, go back; otherwise fallback to home
    if (window.history.length > 1) {
      this.location.back();
    } else {
      this.router.navigate(['/home']);
    }
  }

  // Search state
  q = '';
  open = false;
  users: UserProfile[] = [];
  posts: PostModel[] = [];
  private search$ = new Subject<string>();

  ngOnInit() {
    this.search$.pipe(
      debounceTime(250),
      distinctUntilChanged(),
    ).subscribe(term => {
      const t = (term || '').trim();
      if (!t) { this.users = []; this.posts = []; this.open = false; return; }
      // Trigger both searches in parallel
      this.usersSvc.search(t).subscribe({
        next: u => { this.users = u || []; this.open = !!(this.users.length || this.posts.length); },
        error: () => { this.users = []; },
      });
      this.postsSvc.search(t).subscribe({
        next: p => { this.posts = p || []; this.open = !!(this.users.length || this.posts.length); },
        error: () => { this.posts = []; },
      });
    });
  }

  onSearch() { this.search$.next(this.q); }
  clearSearch() { this.q = ''; this.users = []; this.posts = []; this.open = false; }
  closeSearch() { this.open = false; this.q = ''; }

  img(url?: string | null): string | null {
    if (!url) return null;
    if (/^https?:\/\//i.test(url)) return url;
    return this.mediaBase + (url.startsWith('/') ? url : '/' + url);
  }

  userImg(u: any): string | null {
    if (!u) return null;
    return this.img(u.profileImageUrl || u.profileImage || null);
  }
}
