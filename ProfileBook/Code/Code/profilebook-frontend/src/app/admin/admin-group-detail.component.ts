// src/app/admin/admin-group-detail.component.ts
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminService, AdminGroup, AdminUserDto } from '../core/services/admin.service';
import { UsersService, UserProfile } from '../core/services/users.service';

@Component({
  selector: 'app-admin-group-detail',
  standalone: false,
  template: `
  <main class="container" style="padding:16px 0;">
    <button class="btn" (click)="goBack()">← Back</button>
    <h2 style="margin:8px 0 12px;">Group #{{ group?.id }} — {{ group?.name }}</h2>
    <div *ngIf="loading" class="muted">Loading…</div>
    <div *ngIf="error" class="error" style="color:#b00020;">{{ error }}</div>
    <div *ngIf="group">
      <section class="panel">
        <h3 class="panel-title">Members</h3>
        <div class="card-grid">
          <article class="user-card" *ngFor="let m of group.members">
            <div class="info">
              <div class="name">{{ memberName(m) }}</div>
              <div class="muted">{{ memberEmail(m) }}</div>
            </div>
            <div class="actions">
              <button class="btn danger" (click)="remove(getMemberId(m))" [disabled]="busyUserId === getMemberId(m)">Remove</button>
            </div>
          </article>
          <div *ngIf="!group.members || group.members.length === 0" class="muted">No members yet.</div>
        </div>
      </section>

      <section class="panel" style="margin-top:16px;">
        <h3 class="panel-title">Add users to this group</h3>
        <div class="row" style="align-items:flex-end;">
          <input class="input" type="text" [(ngModel)]="search" placeholder="Search users by name or email" />
          <button class="btn" (click)="onSearch()" [disabled]="usersLoading">Search</button>
          <button class="btn" (click)="clearSearch()" [disabled]="usersLoading || !search">Clear</button>
          <span class="muted">{{ usersMessage }}</span>
        </div>
        <div *ngIf="usersLoading" class="muted" style="margin-top:8px;">Loading users…</div>
        <div *ngIf="usersError" class="error" style="color:#b00020; margin-top:8px;">{{ usersError }}</div>
        <div class="list" *ngIf="!usersLoading">
          <article class="user-row" *ngFor="let u of users">
            <div class="info">
              <div class="name">{{ u.userName }}</div>
              <div class="muted">{{ u.email }}</div>
            </div>
            <div class="actions">
              <ng-container *ngIf="!isMember(u.id); else removeTpl">
                <button class="btn" (click)="add(u.id)" [disabled]="busyUserId === u.id">Add</button>
              </ng-container>
              <ng-template #removeTpl>
                <button class="btn danger" (click)="remove(u.id)" [disabled]="busyUserId === u.id">Remove</button>
              </ng-template>
            </div>
          </article>
          <div *ngIf="users.length === 0" class="muted" style="margin-top:8px;">No users found.</div>
        </div>
      </section>
    </div>
  </main>
  `,
  styles: [`
    .row { display:flex; gap:8px; }
    .input { padding:8px 10px; border:1px solid #e1e5ea; border-radius:8px; flex:1; }
    .btn { padding:6px 10px; border:1px solid #d0d7de; border-radius:8px; background:#f6f8fa; cursor:pointer; }
    .btn.danger { background:#fff5f5; border-color:#ffd6d6; color:#b00020; }
    .panel { border:1px solid #edf1f4; border-radius:10px; padding:12px; background:#fff; }
    .panel-title { margin:0 0 8px; font-size:16px; }
    .card-grid { display:grid; grid-template-columns: repeat(auto-fill, minmax(280px,1fr)); gap:10px; }
    .user-card { border:1px solid #edf1f4; border-radius:10px; padding:10px; background:#fff; display:flex; align-items:center; justify-content:space-between; gap:8px; }
    .list { margin-top:8px; display:flex; flex-direction:column; gap:8px; }
    .user-row { border:1px solid #edf1f4; border-radius:10px; padding:10px; background:#fff; display:flex; align-items:center; justify-content:space-between; gap:8px; }
    .info { min-width:0; }
    .name { font-weight:600; white-space:nowrap; overflow:hidden; text-overflow:ellipsis; }
    .muted { color:#6a737d; font-size:12px; }
  `]
})
export class AdminGroupDetailComponent implements OnInit {
  id!: number;
  group: AdminGroup | null = null;
  loading = false;
  error = '';
  // users browse/search state
  users: AdminUserDto[] = [];
  usersLoading = false;
  usersError = '';
  usersMessage = '';
  search = '';
  busyUserId: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private admin: AdminService,
    private usersService: UsersService
  ) {}

  ngOnInit(): void {
    this.id = Number(this.route.snapshot.paramMap.get('id'));
    this.load();
  }

  load() {
    this.loading = true; this.error='';
    this.admin.getGroup(this.id).subscribe({
      next: (g) => { this.group = g; this.loading = false; },
      error: (err) => { this.loading = false; this.error = err?.error?.title || err?.message || 'Failed to load group'; }
    });
  }

  ngAfterViewInit() {
    // initial load of users list
    this.loadUsers();
  }

  loadUsers() {
    this.usersLoading = true; this.usersError = ''; this.usersMessage = '';
    this.admin.listUsers(1, 20).subscribe({
      next: (page) => { this.users = page.items || []; this.usersLoading = false; },
      error: (err) => { this.usersLoading = false; this.usersError = err?.error?.title || err?.message || 'Failed to load users'; }
    });
  }

  onSearch() {
    if (!this.search || this.search.trim().length < 2) { this.loadUsers(); return; }
    const q = this.search.trim();
    this.usersLoading = true; this.usersError = ''; this.usersMessage = '';
    this.usersService.search(q).subscribe({
      next: (list: UserProfile[]) => {
        this.users = (list || []).map(u => ({ id: u.id, userName: u.userName, email: u.email || '' } as AdminUserDto));
        this.usersLoading = false;
      },
      error: (err) => { this.usersLoading = false; this.usersError = err?.error?.title || err?.message || 'Search failed'; }
    });
  }

  clearSearch() {
    this.search = '';
    this.loadUsers();
  }

  add(userId: string) {
    if (!userId) return;
    this.busyUserId = userId; this.usersMessage='';
    this.admin.addUserToGroup(this.id, userId).subscribe({
      next: (txt) => { this.busyUserId = null; this.usersMessage = txt || 'Added'; this.load(); },
      error: (err) => { this.busyUserId = null; this.usersMessage = err?.error || err?.message || 'Failed to add'; }
    });
  }

  remove(userId: string) {
    if (!userId) return;
    this.busyUserId = userId; this.usersMessage='';
    this.admin.removeUserFromGroup(this.id, userId).subscribe({
      next: (txt) => { this.busyUserId = null; this.usersMessage = txt || 'Removed'; this.load(); },
      error: (err) => { this.busyUserId = null; this.usersMessage = err?.error || err?.message || 'Failed to remove'; }
    });
  }

  memberName(m: any): string { return typeof m === 'string' ? m : (m?.userName || m?.username || m?.id || ''); }
  memberEmail(m: any): string { return typeof m === 'string' ? '' : (m?.email || ''); }
  getMemberId(m: any): string { return typeof m === 'string' ? m : (m?.id || ''); }
  isMember(userId: string): boolean {
    if (!this.group?.members) return false;
    return this.group.members.some(m => (typeof m === 'string' ? m === userId : m?.id === userId));
  }
  goBack() { history.length > 1 ? history.back() : this.router.navigate(['/admin']); }
}
