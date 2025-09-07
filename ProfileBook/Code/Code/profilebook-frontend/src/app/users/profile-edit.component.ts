// src/app/users/profile-edit.component.ts
import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UsersService } from '../core/services/users.service';
import { AuthService } from '../core/services/auth.service';
import { environment } from '../../environments/environment';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-profile-edit',
  standalone: false,
  template: `
  <main class="container" style="padding-top:24px; padding-bottom:24px; max-width:720px;">
    <h2>Edit Profile</h2>
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
          <img [src]="previewUrl" alt="preview" style="max-width:200px; border-radius:8px; border:1px solid #e5e7eb;" />
        </div>
        <div class="actions">
          <button class="btn" type="submit" [disabled]="!selectedFile || uploading">Upload</button>
          <span *ngIf="uploadError" class="error">{{ uploadError }}</span>
        </div>
      </form>
    </section>

    <section class="card" style="margin-bottom:16px;">
      <form [formGroup]="infoForm" (ngSubmit)="saveInfo()" novalidate>
        <div class="row">
          <label>Username</label>
          <input class="input" type="text" formControlName="userName" placeholder="Your username" />
        </div>
        <div class="row">
          <label>Email</label>
          <input class="input" type="email" formControlName="email" placeholder="you@example.com" />
        </div>
        <div class="row">
          <label>Bio</label>
          <textarea class="input" rows="3" formControlName="bio" placeholder="Tell something about yourself"></textarea>
        </div>
        <div class="actions">
          <button class="btn primary" type="submit" [disabled]="infoForm.invalid || savingInfo">Save</button>
          <span *ngIf="infoSuccess" class="success">Saved.</span>
          <span *ngIf="infoError" class="error">{{ infoError }}</span>
        </div>
      </form>
    </section>

    <section class="card">
      <h3 style="margin-top:0;">Change Password</h3>
      <form [formGroup]="passwordForm" (ngSubmit)="changePassword()" novalidate>
        <div class="row">
          <label>Current password</label>
          <input class="input" type="password" formControlName="currentPassword" />
        </div>
        <div class="row">
          <label>New password</label>
          <input class="input" type="password" formControlName="newPassword" />
        </div>
        <div class="actions">
          <button class="btn" type="submit" [disabled]="passwordForm.invalid || savingPwd">Update Password</button>
          <span *ngIf="pwdSuccess" class="success">Password updated.</span>
          <ul *ngIf="pwdErrors?.length" class="error">
            <li *ngFor="let e of pwdErrors">{{ e }}</li>
          </ul>
        </div>
      </form>
    </section>
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
  `]
})
export class ProfileEditComponent implements OnInit, OnDestroy {
  infoForm!: FormGroup;
  passwordForm!: FormGroup;
  savingInfo = false;
  savingPwd = false;
  infoSuccess = false;
  infoError = '';
  pwdSuccess = false;
  pwdErrors: string[] | null = null;
  selectedFile: File | null = null;
  previewUrl: string | null = null;
  uploading = false;
  uploadError = '';
  mediaBase = '';
  currentImageUrl: string = '';
  private sub?: Subscription;
  private initialized = false;

  constructor(private fb: FormBuilder, private users: UsersService, private auth: AuthService, private router: Router) {}

  ngOnInit(): void {
    this.mediaBase = (environment.mediaBaseUrl || '').replace(/\/+$/,'');
    const current = (this.auth as any).userSub?.value as any;
    this.infoForm = this.fb.group({
      userName: [current?.userName || current?.username || '', [Validators.required]],
      email: [current?.email || '', [Validators.required, Validators.email]],
      bio: [current?.bio || '']
    });
    this.passwordForm = this.fb.group({
      currentPassword: ['', Validators.required],
      newPassword: ['', [Validators.required, Validators.minLength(6)]]
    });

    // Compute current image URL and keep form synced once auth user loads (e.g., after refresh)
    const initialImg = current?.profileImageUrl || current?.profileImage || '';
    this.currentImageUrl = this.buildUrl(initialImg);
    this.sub = this.auth.user$.subscribe((u: any) => {
      if (!u) return;
      const userName = u.userName || u.username || '';
      const email = u.email || '';
      const bio = u.bio || '';
      // Only patch when not yet initialized or when values differ
      if (!this.initialized || this.infoForm.pristine) {
        this.infoForm.patchValue({ userName, email, bio }, { emitEvent: false });
      }
      this.currentImageUrl = this.buildUrl(u.profileImageUrl || u.profileImage || '');
      this.initialized = true;
    });
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
    if (this.previewUrl) URL.revokeObjectURL(this.previewUrl);
  }

  saveInfo() {
    if (this.infoForm.invalid) return;
    this.savingInfo = true; this.infoSuccess = false; this.infoError = '';
    const payload: any = {};
    const { userName, email, bio } = this.infoForm.value;
    if (userName) payload.userName = userName;
    if (email) payload.email = email;
    if (bio) payload.bio = bio;
    this.users.updateProfile(payload).subscribe({
      next: () => { this.savingInfo = false; this.infoSuccess = true; location.reload(); },
      error: (err) => { this.savingInfo = false; this.infoError = (err?.error?.message || 'Failed to save'); }
    });
  }

  changePassword() {
    if (this.passwordForm.invalid) return;
    this.savingPwd = true; this.pwdSuccess = false; this.pwdErrors = null;
    const { currentPassword, newPassword } = this.passwordForm.value;
    const payload: any = { };
    if (currentPassword) payload.currentPassword = currentPassword;
    if (newPassword) payload.newPassword = newPassword;
    this.users.updateProfile(payload).subscribe({
      next: () => { this.savingPwd = false; this.pwdSuccess = true; this.passwordForm.reset(); location.reload(); },
      error: (err) => {
        this.savingPwd = false;
        // API returns ["Incorrect password."] as array per spec
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
    this.users.uploadProfileImage(this.selectedFile).subscribe({
      next: () => { this.uploading = false; this.selectedFile = null; if (this.previewUrl) URL.revokeObjectURL(this.previewUrl); this.previewUrl = null; location.reload(); },
      error: (err) => { this.uploading = false; this.uploadError = err?.error?.message || 'Failed to upload image'; }
    });
  }

  private buildUrl(u: string): string {
    if (!u) return '';
    if (/^https?:\/\//i.test(u)) return u;
    return this.mediaBase + (u.startsWith('/') ? u : '/' + u);
  }
}
