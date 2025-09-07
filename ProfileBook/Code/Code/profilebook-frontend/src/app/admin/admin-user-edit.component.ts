// src/app/admin/admin-user-edit.component.ts
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminService, AdminUserDto } from '../core/services/admin.service';
import { UsersService, UserProfile } from '../core/services/users.service';
import { environment } from '../../environments/environment';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-admin-user-edit',
  standalone: false,
  template: `
  <main class="container" style="padding:24px 0; max-width:760px;">
    <button class="btn" (click)="goBack()" style="margin-bottom:12px;">← Back</button>
    <h2 style="margin:0 0 12px;">Edit User (Admin)</h2>

    <div *ngIf="loading" class="muted">Loading…</div>
    <div *ngIf="loadError" class="error">{{ loadError }}</div>

    <ng-container *ngIf="!loading">
      <div *ngIf="currentImageUrl" style="margin:8px 0 16px; display:flex; align-items:center; gap:12px;">
        <img [src]="currentImageUrl" alt="current avatar" style="width:72px; height:72px; border-radius:50%; object-fit:cover; border:1px solid #e5e7eb;" />
        <span class="muted">Current profile picture</span>
      </div>

      <section class="card" style="margin-bottom:16px;">
        <h3 style="margin-top:0;">Profile Image</h3>
        <form (ngSubmit)="uploadImage()" novalidate>
          <div class="row">
            <label>Upload new image</label>
            <input class="input" type="file" accept="image/*" (change)="onFileChange($event)" />
          </div>
          <div *ngIf="previewUrl" style="margin-bottom:12px;">
            <img [src]="previewUrl" alt="preview" style="max-width:220px; border-radius:8px; border:1px solid #e5e7eb;" />
          </div>
          <div class="actions">
            <button class="btn" type="submit" [disabled]="!selectedFile || uploading">Upload</button>
            <span *ngIf="uploadError" class="error">{{ uploadError }}</span>
          </div>
        </form>
      </section>

      <section class="card" style="margin-bottom:16px;">
        <h3 style="margin-top:0;">User Details</h3>
        <form [formGroup]="infoForm" (ngSubmit)="saveInfo()" novalidate>
          <div class="row">
            <label>Username</label>
            <input class="input" type="text" formControlName="userName" placeholder="Username" />
          </div>
          <div class="row">
            <label>Email</label>
            <input class="input" type="email" formControlName="email" placeholder="you@example.com" />
          </div>
          <div class="row">
            <label>Role</label>
            <select class="input" formControlName="role">
              <option value="USER">USER</option>
              <option value="ADMIN">ADMIN</option>
            </select>
          </div>
          <div class="actions">
            <button class="btn primary" type="submit" [disabled]="infoForm.invalid || savingInfo">Save</button>
            <span *ngIf="infoSuccess" class="success">Saved.</span>
            <span *ngIf="infoError" class="error">{{ infoError }}</span>
          </div>
        </form>
      </section>

      <section class="card">
        <h3 style="margin-top:0;">Set Password</h3>
        <form [formGroup]="passwordForm" (ngSubmit)="changePassword()" novalidate>
          <div class="row">
            <label>New password</label>
            <input class="input" type="password" formControlName="newPassword" />
          </div>
          <div class="actions">
            <button class="btn" type="submit" [disabled]="passwordForm.invalid || savingPwd">Update Password</button>
            <span *ngIf="pwdSuccess" class="success">Password updated.</span>
          </div>
          <ul *ngIf="pwdErrors?.length" class="error" style="margin-top:8px;">
            <li *ngFor="let e of pwdErrors">{{ e }}</li>
          </ul>
        </form>
      </section>
    </ng-container>
  </main>
  `,
  styles: [`
    .row { display:flex; flex-direction:column; gap:6px; margin-bottom:12px; }
    .actions { display:flex; align-items:center; gap:10px; }
    .btn { padding:8px 14px; border:1px solid rgba(0,0,0,0.1); border-radius:8px; background:#fff; cursor:pointer; }
    .btn.primary { background:#111827; color:#fff; border-color:#111827; }
    .input { padding:10px 12px; border:1px solid #e5e7eb; border-radius:8px; background:#fff; }
    .card { background:#fff; border:1px solid #e5e7eb; border-radius:12px; padding:16px; }
    .success { color:#059669; }
    .error { color:#b91c1c; }
    .muted { color:#6b7280; }
  `]
})
export class AdminUserEditComponent implements OnInit, OnDestroy {
  id = '';
  loading = false;
  loadError = '';
  infoForm!: FormGroup;
  passwordForm!: FormGroup;
  savingInfo = false;
  infoSuccess = false;
  infoError = '';
  savingPwd = false;
  pwdSuccess = false;
  pwdErrors: string[] | null = null;
  selectedFile: File | null = null;
  previewUrl: string | null = null;
  uploading = false;
  uploadError = '';
  mediaBase = '';
  currentImageUrl: string = '';
  private sub?: Subscription;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private admin: AdminService,
    private users: UsersService
  ) {}

  ngOnInit(): void {
    this.mediaBase = (environment.mediaBaseUrl || '').replace(/\/+$/, '');
    this.id = this.route.snapshot.paramMap.get('id') || '';
    this.infoForm = this.fb.group({
      userName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      role: ['USER', Validators.required]
    });
    this.passwordForm = this.fb.group({ newPassword: ['', [Validators.required, Validators.minLength(6)]] });
    this.loadUser();
  }

  ngOnDestroy(): void {
    if (this.previewUrl) URL.revokeObjectURL(this.previewUrl);
    this.sub?.unsubscribe?.();
  }

  private loadUser() {
    if (!this.id) { this.loadError = 'Missing user id'; return; }
    this.loading = true; this.loadError = '';
    // Use public UsersService to fetch base info; roles may not be present.
    this.users.getById(this.id).subscribe({
      next: (u: UserProfile) => {
        this.infoForm.patchValue({ userName: u.userName || (u as any).username || '', email: u.email || '' });
        const rawImg = (u as any).profileImageUrl || (u as any).profileImage || '';
        this.currentImageUrl = this.buildUrl(rawImg);
        this.loading = false;
      },
      error: (err) => { this.loading = false; this.loadError = err?.error?.title || err?.message || 'Failed to load user'; }
    });
  }

  saveInfo() {
    if (this.infoForm.invalid) return;
    this.savingInfo = true; this.infoSuccess = false; this.infoError = '';
    const { userName, email, role } = this.infoForm.value;
    this.admin.updateUser(this.id, { userName, email, roles: role }).subscribe({
      next: (res) => { this.savingInfo = false; this.infoSuccess = true; },
      error: (err) => { this.savingInfo = false; this.infoError = err?.error?.message || err?.message || 'Failed to save'; }
    });
  }

  changePassword() {
    if (this.passwordForm.invalid) return;
    this.savingPwd = true; this.pwdSuccess = false; this.pwdErrors = null;
    const { newPassword } = this.passwordForm.value;
    this.admin.updateUser(this.id, { password: newPassword }).subscribe({
      next: () => { this.savingPwd = false; this.pwdSuccess = true; this.passwordForm.reset(); },
      error: (err) => {
        this.savingPwd = false;
        const e = err?.error;
        this.pwdErrors = Array.isArray(e) ? e : (e?.errors || [e?.message || 'Failed to update password']);
      }
    });
  }

  onFileChange(evt: Event) {
    const input = evt.target as HTMLInputElement;
    const file = input.files && input.files[0];
    this.selectedFile = file || null;
    this.uploadError = '';
    if (this.previewUrl) URL.revokeObjectURL(this.previewUrl);
    this.previewUrl = file ? URL.createObjectURL(file) : null;
  }

  uploadImage() {
    if (!this.selectedFile) return;
    this.uploading = true; this.uploadError = '';
    this.admin.updateUser(this.id, { profileImage: this.selectedFile }).subscribe({
      next: () => { this.uploading = false; this.selectedFile = null; if (this.previewUrl) URL.revokeObjectURL(this.previewUrl); this.previewUrl = null; this.loadUser(); },
      error: (err) => { this.uploading = false; this.uploadError = err?.error?.message || err?.message || 'Failed to upload image'; }
    });
  }

  goBack() { history.length > 1 ? history.back() : this.router.navigate(['/admin']); }

  private buildUrl(u: string): string {
    if (!u) return '';
    if (/^https?:\/\//i.test(u)) return u;
    const base = (this.mediaBase || '').replace(/\/+$/, '');
    return base + (u.startsWith('/') ? u : '/' + u);
  }
}
