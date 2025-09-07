// src/app/admin/admin-dashboard.component.ts
import { Component, OnInit } from '@angular/core';
import { PostsService } from '../core/services/posts.service';
import { PostModel } from '../core/models/post.model';
import { ReportsService, ReportDto } from '../core/services/reports.service';
import { AdminService, AdminUsersPage, AdminUserDto, AdminGroup } from '../core/services/admin.service';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-admin-dashboard',
  standalone: false,
  template: `
  <main class="container" style="padding-top:16px; padding-bottom:24px;">
    <div class="card" style="padding:16px;">
      <h2 style="margin:0 0 12px;">Admin</h2>

      <!-- Tabs nav -->
      <nav class="tabs" aria-label="Admin sections">
  <button class="tab" [class.active]="activeTab==='posts'" (click)="activeTab='posts'" [attr.aria-current]="activeTab==='posts' ? 'page' : null">Posts</button>
  <button class="tab" [class.active]="activeTab==='reports'" (click)="onReportsTab()" [attr.aria-current]="activeTab==='reports' ? 'page' : null">Reports</button>
  <button class="tab" [class.active]="activeTab==='users'" (click)="onUsersTab()" [attr.aria-current]="activeTab==='users' ? 'page' : null">Users</button>
  <button class="tab" [class.active]="activeTab==='groups'" (click)="onGroupsTab()" [attr.aria-current]="activeTab==='groups' ? 'page' : null">Groups</button>
      </nav>

      <!-- Posts tab content -->
  <section *ngIf="activeTab==='posts'" aria-label="Posts moderation" style="margin-top:12px;">
        <div class="toolbar">
          <div class="segmented" role="tablist" aria-label="Filter by status">
            <button class="seg-btn" [class.active]="statusFilter==='Pending'" role="tab" aria-selected="{{statusFilter==='Pending'}}" (click)="setFilter('Pending')">Pending <span class="pill">{{ count('Pending') }}</span></button>
            <button class="seg-btn" [class.active]="statusFilter==='Approved'" role="tab" aria-selected="{{statusFilter==='Approved'}}" (click)="setFilter('Approved')">Approved <span class="pill">{{ count('Approved') }}</span></button>
            <button class="seg-btn" [class.active]="statusFilter==='Rejected'" role="tab" aria-selected="{{statusFilter==='Rejected'}}" (click)="setFilter('Rejected')">Rejected <span class="pill">{{ count('Rejected') }}</span></button>
            <button class="seg-btn" [class.active]="statusFilter==='All'" role="tab" aria-selected="{{statusFilter==='All'}}" (click)="setFilter('All')">All <span class="pill">{{ filtered.length || posts.length }}</span></button>
          </div>
          <div class="right">
            <button (click)="reload()">Refresh</button>
          </div>
        </div>

        <div *ngIf="loading" class="muted">Loading…</div>
        <div *ngIf="error" class="error" style="color:#b00020;">{{ error }}</div>

        <div *ngIf="!loading && filtered.length === 0" class="muted">No posts found.</div>

        <div class="card-grid">
          <article class="post-card" *ngFor="let p of filtered">
            <div class="media" *ngIf="p.imagePath">
              <a [routerLink]="['/posts', p.id]" target="_blank" rel="noopener">
                <img [src]="p.imagePath" alt="post image" />
              </a>
            </div>
            <div class="body">
              <div class="head">
                <span class="id">#{{ p.id }}</span>
                <span class="badge" [ngClass]="p.status?.toLowerCase()">{{ p.status }}</span>
                <span class="muted date">{{ p.createdAt | date:'short' }}</span>
                <span class="spacer"></span>
              </div>
              <div class="content">
                <a [routerLink]="['/posts', p.id]" target="_blank" rel="noopener">{{ p.content }}</a>
              </div>
              <div class="user muted">by {{ p.user?.userName || p.user?.username }}</div>
            </div>
            <div class="actions">
              <button class="btn btn-approve" (click)="approve(p)" [disabled]="approving[p.id]" *ngIf="(p.status||'').toLowerCase() !== 'approved'">Approve</button>
              <button class="btn btn-reject" (click)="reject(p)" [disabled]="rejecting[p.id]" *ngIf="(p.status||'').toLowerCase() !== 'rejected'">Reject</button>
            </div>
          </article>
        </div>
      </section>

      <!-- Groups tab -->
      <section *ngIf="activeTab==='groups'" aria-label="Groups" style="margin-top:12px;">
        <div class="card" style="padding:12px; margin-bottom:12px;">
          <h3 style="margin:0 0 8px;">Create group</h3>
          <form (ngSubmit)="createGroup()" class="user-form">
            <input type="text" name="groupName" [(ngModel)]="groupName" placeholder="Group name" required />
            <input type="text" name="memberIds" [(ngModel)]="memberIdsCsv" placeholder="Member IDs (comma-separated, optional)" />
            <button type="submit" [disabled]="creatingGroup">Create</button>
            <span *ngIf="groupCreateError" class="error" style="color:#b00020;">{{ groupCreateError }}</span>
          </form>
          <div *ngIf="groupCreateSuccess" class="success">Group created with id {{ currentGroup?.id }}.</div>
        </div>

        <div class="card" style="padding:12px;">
          <div class="toolbar">
            <div class="left"><strong>All groups</strong></div>
            <div class="right">
              <button (click)="loadGroups()" [disabled]="groupsLoading">Refresh</button>
            </div>
          </div>
          <div *ngIf="groupsLoading" class="muted">Loading…</div>
          <div *ngIf="groupsError" class="error" style="color:#b00020;">{{ groupsError }}</div>
          <div *ngIf="!groupsLoading && groups.length===0" class="muted">No groups.</div>
          <div class="card-grid" style="margin-top:8px;">
            <article class="post-card" *ngFor="let g of groups">
              <div class="body">
                <div class="head">
                  <span class="id">#{{ g.id }}</span>
                  <span class="spacer"></span>
                </div>
                <div class="content">
                  <a [routerLink]="['/admin/groups', g.id]">{{ g.name }}</a>
                </div>
                <div class="muted">{{ g.members?.length || 0 }} member(s)</div>
              </div>
            </article>
          </div>
        </div>
      </section>

      <!-- Reports tab -->
      <section *ngIf="activeTab==='reports'" aria-label="Reports" style="margin-top:12px;">
        <div class="toolbar">
          <div class="segmented" role="tablist" aria-label="Filter by report status">
            <button class="seg-btn" [class.active]="reportFilter==='Open'" (click)="setReportFilter('Open')">Open <span class="pill">{{ reportCount('Open') }}</span></button>
            <button class="seg-btn" [class.active]="reportFilter==='Reviewing'" (click)="setReportFilter('Reviewing')">Reviewing <span class="pill">{{ reportCount('Reviewing') }}</span></button>
            <button class="seg-btn" [class.active]="reportFilter==='Actioned'" (click)="setReportFilter('Actioned')">Actioned <span class="pill">{{ reportCount('Actioned') }}</span></button>
            <button class="seg-btn" [class.active]="reportFilter==='Dismissed'" (click)="setReportFilter('Dismissed')">Dismissed <span class="pill">{{ reportCount('Dismissed') }}</span></button>
            <button class="seg-btn" [class.active]="reportFilter==='All'" (click)="setReportFilter('All')">All <span class="pill">{{ reports.length }}</span></button>
          </div>
          <div class="right">
            <button (click)="reloadReports()">Refresh</button>
          </div>
        </div>

        <div *ngIf="reportsLoading" class="muted">Loading…</div>
        <div *ngIf="reportsError" class="error" style="color:#b00020;">{{ reportsError }}</div>
        <div *ngIf="!reportsLoading && filteredReports.length===0" class="muted">No reports found.</div>

        <div class="card-grid">
          <article class="report-card" *ngFor="let r of filteredReports">
            <div class="body">
              <div class="head">
                <span class="id">#R{{ r.id }}</span>
                <span class="badge" [ngClass]="(r.status||'').toLowerCase()">{{ r.status }}</span>
                <span class="muted date">{{ r.createdAt | date:'short' }}</span>
              </div>
              <div class="content">
                <div><strong>Reported:</strong> {{ r.reportedUserName || r.reportedUserId }}</div>
                <div><strong>By:</strong> {{ r.reportingUserName || r.reportingUserId }}</div>
                <div><strong>Reason:</strong> {{ r.reason }}</div>
              </div>
            </div>
            <div class="actions">
              <span class="muted actions-label">Set status:</span>
              <button class="btn btn-open" (click)="setReportStatus(r, 'Open')" [disabled]="updatingReport[r.id] || r.status==='Open'">Open</button>
              <button class="btn btn-reviewing" (click)="setReportStatus(r, 'Reviewing')" [disabled]="updatingReport[r.id] || r.status==='Reviewing'">Reviewing</button>
              <button class="btn btn-actioned" (click)="setReportStatus(r, 'Actioned')" [disabled]="updatingReport[r.id] || r.status==='Actioned'">Actioned</button>
              <button class="btn btn-dismissed" (click)="setReportStatus(r, 'Dismissed')" [disabled]="updatingReport[r.id] || r.status==='Dismissed'">Dismiss</button>
            </div>
          </article>
        </div>
      </section>

      <!-- Users tab -->
      <section *ngIf="activeTab==='users'" aria-label="Users" style="margin-top:12px;">
        <div class="card" style="padding:12px; margin-bottom:12px;">
          <h3 style="margin:0 0 8px;">Create user</h3>
          <form (ngSubmit)="createUser()" #createForm="ngForm" class="user-form">
            <input type="text" name="userName" [(ngModel)]="create.userName" placeholder="Username" required />
            <input type="email" name="email" [(ngModel)]="create.email" placeholder="Email" required />
            <input type="password" name="password" [(ngModel)]="create.password" placeholder="Password" required />
            <label class="inline">
              <input type="checkbox" name="emailConfirmed" [(ngModel)]="create.emailConfirmed" /> Email confirmed
            </label>
            <select name="role" [(ngModel)]="create.role">
              <option value="USER">USER</option>
              <option value="ADMIN">ADMIN</option>
            </select>
            <input type="file" (change)="onUserImage($event)" />
            <button type="submit" [disabled]="creating">Create</button>
            <span *ngIf="createError" class="error" style="color:#b00020;">{{ createError }}</span>
          </form>
        </div>

        <div class="card" style="padding:12px;">
          <div class="toolbar">
            <div class="left"><strong>All users</strong></div>
            <div class="right">
              <button (click)="reloadUsers()" [disabled]="usersLoading">Refresh</button>
            </div>
          </div>
          <div *ngIf="usersLoading" class="muted">Loading…</div>
          <div *ngIf="usersError" class="error" style="color:#b00020;">{{ usersError }}</div>
          <div *ngIf="!usersLoading && users.items.length === 0" class="muted">No users.</div>
          <div class="user-card" *ngFor="let u of users.items">
            <div class="avatar" *ngIf="u.profileImage; else noImg2"><img [src]="userImg(u.profileImage)" alt="{{u.userName}} avatar"/></div>
            <ng-template #noImg2><div class="avatar avatar-fallback">{{ initial(u.userName) }}</div></ng-template>
            <div class="info">
              <div class="name">{{ u.userName }}</div>
              <div class="muted">{{ u.email }}</div>
            </div>
            <div class="actions-right">
              <a class="btn" [routerLink]="['/users', u.id]" target="_blank" rel="noopener">View profile</a>
              <a class="btn" [routerLink]="['/admin/users', u.id, 'edit']">Edit</a>
              <button class="btn danger" (click)="confirmDelete(u)" [disabled]="deleting[u.id]">Delete</button>
            </div>
          </div>
          <div class="pager">
            <button (click)="prevPage()" [disabled]="page<=1">Prev</button>
            <span>Page {{ page }} / {{ totalPages() }}</span>
            <button (click)="nextPage()" [disabled]="page>=totalPages()">Next</button>
          </div>
        </div>
      </section>
    </div>
  </main>
  `
  ,
  styles: [`
    .tabs { display:flex; gap:8px; border-bottom:1px solid #eee; }
    .tab { background:none; border:none; padding:8px 12px; cursor:pointer; border-bottom:2px solid transparent; }
    .tab.active { border-bottom-color:#1976d2; color:#1976d2; font-weight:600; }

  .toolbar { display:flex; align-items:center; justify-content:space-between; gap:12px; margin:8px 0 12px; flex-wrap:wrap; }
  .segmented { display:flex; gap:6px; flex-wrap:wrap; }
  .seg-btn { background:#f6f7f8; border:1px solid #e3e6e8; padding:6px 10px; border-radius:999px; cursor:pointer; }
  .seg-btn.active { background:#e8f0fe; border-color:#a0c2ff; color:#1967d2; }
  .pill { display:inline-block; min-width:18px; padding:0 6px; margin-left:6px; border-radius:999px; background:#e9ecef; font-size:12px; text-align:center; }

    .card-grid { display:grid; grid-template-columns: repeat(auto-fill, minmax(260px, 1fr)); gap:12px; }
  .post-card, .report-card { border:1px solid #e6e9ec; border-radius:12px; overflow:hidden; display:flex; flex-direction:column; background:#fff; box-shadow:0 1px 2px rgba(0,0,0,0.04); }
  .post-card .media { width:100%; height:160px; background:#fafafa; overflow:hidden; }
  .post-card .media a { display:block; width:100%; height:100%; }
  .post-card .media img { width:100%; height:100%; object-fit:cover; display:block; }
  .post-card .body, .report-card .body { padding:12px 14px; display:flex; flex-direction:column; gap:8px; }
  .post-card .head { display:flex; align-items:center; gap:8px; }
  .post-card .head .spacer { flex:1 1 auto; }
    .post-card .head .id { font-weight:600; }
    .post-card .head .date { margin-left:auto; }
  .actions { display:flex; gap:10px; padding:12px 14px; border-top:1px solid #edf1f4; align-items:center; flex-wrap:wrap; }
  .actions-label { min-width: 80px; }
  .btn { border:1px solid transparent; background:#f7f9fc; padding:8px 12px; border-radius:8px; cursor:pointer; font-weight:600; }
  .btn:hover { filter: brightness(0.98); }
  .btn:disabled { opacity:0.6; cursor:not-allowed; }
  .btn-approve { color:#0f5132; background:#d1e7dd; border-color:#badbcc; }
  .btn-reject { color:#842029; background:#f8d7da; border-color:#f5c2c7; }
  .btn-open { color:#7a4d00; background:#ffe7ba; border-color:#ffd590; }
  .btn-reviewing { color:#0d47a1; background:#e3f2fd; border-color:#bbdefb; }
  .btn-close { color:#2d3436; background:#eaecef; border-color:#dfe2e6; }
    .badge { padding: 2px 8px; border-radius: 999px; font-size: 12px; background:#eee; }
    .badge.pending { background:#fff4cc; color:#a86b00; }
    .badge.approved { background:#e8f5e9; color:#1b5e20; }
    .badge.rejected { background:#ffebee; color:#b71c1c; }
  .badge.open { background:#fff4cc; color:#a86b00; }
  .badge.reviewing { background:#e3f2fd; color:#0d47a1; }
  .badge.actioned { background:#e8f5e9; color:#1b5e20; }
  .badge.dismissed { background:#ffebee; color:#b71c1c; }
  .user-form { display:grid; grid-template-columns: repeat(auto-fit, minmax(180px,1fr)); gap:8px; align-items:center; }
  .user-form input[type="text"], .user-form input[type="email"], .user-form input[type="password"], .user-form select { padding:8px 10px; border:1px solid #e1e5ea; border-radius:8px; }
  .user-form button { padding:8px 12px; border-radius:8px; border:1px solid #d0d7de; background:#f6f8fa; font-weight:600; cursor:pointer; }
  .user-card { display:flex; gap:12px; align-items:center; padding:12px; border:1px solid #edf1f4; border-radius:12px; background:#fff; }
  .user-card .info { flex:1 1 auto; }
  .actions-right { display:flex; gap:8px; margin-left:auto; }
  .actions-right .btn { padding:6px 10px; border:1px solid #d0d7de; border-radius:8px; background:#f6f8fa; text-decoration:none; font-weight:600; color:#111; }
  .actions-right .btn.primary { background:#111827; border-color:#111827; color:#fff; }
  .actions-right .btn.danger { background:#fef2f2; border-color:#fecaca; color:#991b1b; }
  .row-actions a { color:#1976d2; font-size:12px; text-decoration:none; }
  .row-actions a:hover { text-decoration:underline; }
  .avatar { width:40px; height:40px; border-radius:50%; overflow:hidden; background:#f1f3f4; display:flex; align-items:center; justify-content:center; }
  .avatar img { width:100%; height:100%; object-fit:cover; }
  .avatar-fallback { color:#555; font-weight:600; }
  .pager { margin-top:10px; display:flex; gap:10px; align-items:center; justify-content:center; }
  `]
})
export class AdminDashboardComponent implements OnInit {
  activeTab: 'posts' | 'reports' | 'users' | 'groups' = 'posts';
  posts: PostModel[] = [];
  filtered: PostModel[] = [];
  statusFilter: 'Pending' | 'Approved' | 'Rejected' | 'All' = 'Pending';
  loading = false;
  error: string | null = null;
  approving: Record<number, boolean> = {};
  rejecting: Record<number, boolean> = {};

  // Reports state
  reports: ReportDto[] = [];
  filteredReports: ReportDto[] = [];
  reportFilter: 'Open'|'Reviewing'|'Actioned'|'Dismissed'|'All' = 'All';
  reportsLoading = false;
  reportsError: string | null = null;
  updatingReport: Record<number, boolean> = {};

  // Users state
  users: AdminUsersPage = { total: 0, page: 1, pageSize: 20, items: [] };
  page = 1;
  pageSize = 20;
  usersLoading = false;
  usersError: string | null = null;
  creating = false;
  createError: string | null = null;
  create = { userName: '', email: '', password: '', emailConfirmed: false, role: 'USER' as 'USER'|'ADMIN', imageFile: null as File | null };
  deleting: Record<string, boolean> = {};

  // Groups state
  groupName = '';
  memberIdsCsv = '';
  creatingGroup = false;
  groupCreateError = '';
  groupCreateSuccess = false;
  groupId: number | null = null;
  currentGroup: AdminGroup | null = null;
  groupsLoading = false;
  groupsError = '';
  userIdToModify = '';
  modifying = false;
  modifyMessage = '';
  groups: AdminGroup[] = [];
  groupsLoadedOnce = false;

  constructor(private postsService: PostsService, private reportsService: ReportsService, private adminService: AdminService) {}

  ngOnInit(): void {
    this.reload();
  }

  reload(): void {
    this.loading = true;
    this.error = null;
    this.postsService.getAll().subscribe({
      next: (list) => {
        this.posts = Array.isArray(list) ? list : [];
        this.applyFilter();
        this.loading = false;
      },
      error: (err) => {
        this.loading = false;
        this.error = err?.error?.title || err?.message || 'Failed to load posts';
      }
    });
  }

  // Reports tab handlers
  onReportsTab() {
    this.activeTab = 'reports';
    if (this.reports.length === 0) {
      this.reloadReports();
    } else {
      this.applyReportFilter();
    }
  }

  reloadReports() {
    this.reportsLoading = true;
    this.reportsError = null;
    const useAll = this.reportFilter === 'All';
    const call$ = useAll
      ? this.reportsService.listAllReports()
      : this.reportsService.listAll({ status: this.reportFilter, page: 1, pageSize: 50 });
    call$.subscribe({
      next: (list) => {
        this.reports = Array.isArray(list) ? list : [];
        this.applyReportFilter();
        this.reportsLoading = false;
      },
      error: (err) => {
        this.reportsLoading = false;
        this.reportsError = err?.error?.title || err?.message || 'Failed to load reports';
      }
    });
  }

  setReportFilter(v: 'Open'|'Reviewing'|'Actioned'|'Dismissed'|'All') {
    this.reportFilter = v;
    if (this.reports.length) {
      this.applyReportFilter();
    } else {
      this.reloadReports();
    }
  }

  applyReportFilter() {
    if (this.reportFilter === 'All') {
      this.filteredReports = this.reports.slice();
    } else {
      const f = this.reportFilter.toLowerCase();
      this.filteredReports = this.reports.filter(r => (r.status || '').toLowerCase() === f);
    }
  }

  reportCount(v: 'Open'|'Reviewing'|'Actioned'|'Dismissed'): number {
    const f = v.toLowerCase();
    return this.reports.filter(r => (r.status || '').toLowerCase() === f).length;
  }

  setReportStatus(r: ReportDto, status: 'Open'|'Reviewing'|'Actioned'|'Dismissed') {
    if (!r?.id) return;
    this.updatingReport[r.id] = true;
    this.reportsService.updateStatus(r.id, status).subscribe({
      next: () => {
        r.status = status;
        this.applyReportFilter();
        this.updatingReport[r.id] = false;
      },
      error: (err) => {
        this.updatingReport[r.id] = false;
        this.reportsError = err?.error?.title || err?.message || 'Failed to update status';
      }
    });
  }

  // Users tab
  onUsersTab() {
    this.activeTab = 'users';
    this.reloadUsers();
  }

  onGroupsTab() {
    this.activeTab = 'groups';
    if (!this.groupsLoadedOnce) this.loadGroups();
  }

  reloadUsers() {
    this.usersLoading = true;
    this.usersError = null;
    this.adminService.listUsers(this.page, this.pageSize).subscribe({
      next: (res) => { this.users = res; this.usersLoading = false; },
      error: (err) => { this.usersLoading = false; this.usersError = err?.error?.title || err?.message || 'Failed to load users'; }
    });
  }

  nextPage() { if (this.page < this.totalPages()) { this.page++; this.reloadUsers(); } }
  prevPage() { if (this.page > 1) { this.page--; this.reloadUsers(); } }
  totalPages() { return Math.max(1, Math.ceil(this.users.total / this.pageSize)); }

  onUserImage(ev: Event) {
    const input = ev.target as HTMLInputElement;
    this.create.imageFile = input?.files && input.files.length ? input.files[0] : null;
  }

  createUser() {
    if (!this.create.userName || !this.create.email || !this.create.password) return;
    this.creating = true; this.createError = null;
    this.adminService.createUserWithProfile({
      userName: this.create.userName,
      email: this.create.email,
      password: this.create.password,
      emailConfirmed: this.create.emailConfirmed,
      role: this.create.role,
      imageFile: this.create.imageFile || undefined
    }).subscribe({
      next: () => {
        this.creating = false;
        // reset form
        this.create = { userName: '', email: '', password: '', emailConfirmed: false, role: 'USER', imageFile: null };
        this.reloadUsers();
      },
      error: (err) => {
        this.creating = false;
        this.createError = err?.error?.title || err?.message || 'Failed to create user';
      }
    });
  }

  // Groups logic
  loadGroups() {
    this.groupsLoading = true; this.groupsError='';
    this.adminService.listGroups().subscribe({
      next: (gs) => { this.groupsLoading = false; this.groups = gs || []; this.groupsLoadedOnce = true; },
      error: (err) => { this.groupsLoading = false; this.groupsError = err?.error?.title || err?.message || 'Failed to load groups'; }
    });
  }
  createGroup() {
    if (!this.groupName) return;
    this.creatingGroup = true; this.groupCreateError = ''; this.groupCreateSuccess = false;
    const memberIds = (this.memberIdsCsv || '').split(',').map(s => s.trim()).filter(Boolean);
    this.adminService.createGroup({ name: this.groupName, memberIds: memberIds.length ? memberIds : undefined }).subscribe({
      next: (g) => { this.creatingGroup = false; this.groupCreateSuccess = true; this.currentGroup = g; this.groupId = g.id as any; this.loadGroups(); },
      error: (err) => { this.creatingGroup = false; this.groupCreateError = err?.error?.title || err?.message || 'Failed to create group'; }
    });
  }

  loadGroup() {
    if (!this.groupId && this.groupId !== 0) { this.groupsError = 'Enter Group ID'; return; }
    this.groupsLoading = true; this.groupsError=''; this.currentGroup = null; this.modifyMessage='';
    this.adminService.getGroup(this.groupId!).subscribe({
      next: (g) => { this.groupsLoading = false; this.currentGroup = g; },
      error: (err) => { this.groupsLoading = false; this.groupsError = err?.error?.title || err?.message || 'Failed to load group'; }
    });
  }

  addUserToGroup() {
    if (!this.currentGroup || !this.userIdToModify) return;
    this.modifying = true; this.modifyMessage='';
    this.adminService.addUserToGroup(this.currentGroup.id, this.userIdToModify).subscribe({
      next: (txt) => { this.modifying = false; this.modifyMessage = txt || 'Added'; this.loadGroup(); },
      error: (err) => { this.modifying = false; this.modifyMessage = err?.error || err?.message || 'Failed to add'; }
    });
  }

  removeUserFromGroup() {
    if (!this.currentGroup || !this.userIdToModify) return;
    this.modifying = true; this.modifyMessage='';
    this.adminService.removeUserFromGroup(this.currentGroup.id, this.userIdToModify).subscribe({
      next: (txt) => { this.modifying = false; this.modifyMessage = txt || 'Removed'; this.loadGroup(); },
      error: (err) => { this.modifying = false; this.modifyMessage = err?.error || err?.message || 'Failed to remove'; }
    });
  }

  // Helpers for group member display
  memberName(m: any): string {
    if (!m) return '';
    if (typeof m === 'string') return m;
    return m.userName || m.username || m.id || '';
  }
  memberEmail(m: any): string {
    if (!m || typeof m === 'string') return '';
    return m.email || '';
  }

  confirmDelete(u: AdminUserDto) {
    if (!u?.id) return;
    const ok = confirm(`Delete user "${u.userName}"? This cannot be undone.`);
    if (!ok) return;
    this.deleting[u.id] = true;
    this.adminService.deleteUser(u.id).subscribe({
      next: () => {
        this.deleting[u.id] = false;
        // If current page becomes empty after deletion, go back a page if possible
        if (this.users.items.length === 1 && this.page > 1) {
          this.page -= 1;
        }
        this.reloadUsers();
      },
      error: (err) => {
        this.deleting[u.id] = false;
        const msg = err?.status === 404 ? 'User not found' : (err?.error?.title || err?.message || 'Delete failed');
        alert(msg);
      }
    });
  }

  userImg(path?: string | null) {
    if (!path) return '';
    if (/^https?:\/\//i.test(path)) return path;
    return `${environment.mediaBaseUrl}${path.startsWith('/') ? path : '/' + path}`;
  }
  initial(name?: string) { return (name?.trim()?.charAt(0) || '?').toUpperCase(); }

  applyFilter(): void {
    if (this.statusFilter === 'All') {
      this.filtered = this.posts.slice();
    } else {
      this.filtered = this.posts.filter(p => (p.status || '').toLowerCase() === this.statusFilter.toLowerCase());
    }
  }

  setFilter(v: 'Pending'|'Approved'|'Rejected'|'All') {
    this.statusFilter = v;
    this.applyFilter();
  }

  count(v: 'Pending'|'Approved'|'Rejected'): number {
    return this.posts.filter(p => (p.status || '').toLowerCase() === v.toLowerCase()).length;
  }

  approve(p: PostModel): void {
    if (!p?.id) return;
    this.approving[p.id] = true;
    this.postsService.approvePost(p.id).subscribe({
      next: () => {
        p.status = 'Approved';
        this.applyFilter();
        this.approving[p.id!] = false;
      },
      error: (err) => {
        this.error = err?.error?.title || err?.message || 'Approve failed';
        this.approving[p.id!] = false;
      }
    });
  }

  reject(p: PostModel): void {
    if (!p?.id) return;
    this.rejecting[p.id] = true;
    this.postsService.rejectPost(p.id).subscribe({
      next: () => {
        p.status = 'Rejected';
        this.applyFilter();
        this.rejecting[p.id!] = false;
      },
      error: (err) => {
        this.error = err?.error?.title || err?.message || 'Reject failed';
        this.rejecting[p.id!] = false;
      }
    });
  }
}
