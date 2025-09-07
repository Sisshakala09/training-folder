// src/app/posts/create-post.component.ts
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { PostsService } from '../core/services/posts.service';
import { AuthGuard } from '../core/guards/auth.guard';

@Component({
  selector: 'app-create-post',
  standalone: false,
  template: `
  <main class="container" style="padding-top:24px; padding-bottom:24px;">
    <form class="card create-card" [formGroup]="form" (ngSubmit)="submit()" novalidate aria-labelledby="create-heading">
      <h2 id="create-heading" class="title">New Post</h2>

      <label class="label" for="content">Content</label>
      <textarea id="content" class="input" formControlName="content" rows="5" placeholder="What's on your mind?"></textarea>
      <div *ngIf="form.controls['content'].invalid && (form.controls['content'].dirty || form.controls['content'].touched)" class="hint error">Content is required</div>

      <label class="label" for="image">Image (optional)</label>
      <input id="image" class="input" type="file" (change)="onFile($event)" accept="image/*" />

      <img *ngIf="previewUrl" [src]="previewUrl" alt="preview" class="preview mt-1" />

      <div class="actions mt-2">
        <button class="btn-primary" type="submit" [disabled]="form.invalid || loading">{{ loading ? 'Posting…' : 'Post' }}</button>
        <button class="btn-ghost" type="button" (click)="reset()">Reset</button>
      </div>

      <div *ngIf="error" class="hint error" style="margin-top:8px;">{{ error }}</div>
    </form>
  </main>
  `,
  styles: [`
    .create-card { max-width: 640px; margin: 0 auto; }
    .title { margin: 0 0 8px 0; }
    .label { display:block; margin: 12px 0 6px; font-weight:600; }
    .hint { font-size: 0.88rem; color:#6b7280; }
    .hint.error { color:#b91c1c; }
    .actions { display:flex; gap:8px; justify-content:flex-end; }
    .preview { display:block; max-width:100%; height:auto; max-height:220px; border-radius:10px; border:1px solid rgba(0,0,0,0.06); }
  `]
})
export class CreatePostComponent {
  form: FormGroup;
  file?: File;
  loading = false;
  error: string | null = null;
  previewUrl?: string;

  constructor(private fb: FormBuilder, private posts: PostsService, private router: Router) {
    this.form = this.fb.group({ content: ['', Validators.required] });
  }

  onFile(ev: Event) {
    const input = ev.target as HTMLInputElement;
    const f = input.files && input.files.length ? input.files[0] : undefined;
    // cleanup previous preview url if any
    if (this.previewUrl) URL.revokeObjectURL(this.previewUrl);
    this.file = f;
    this.previewUrl = f ? URL.createObjectURL(f) : undefined;
  }

  reset() { this.form.reset(); if (this.previewUrl) URL.revokeObjectURL(this.previewUrl); this.previewUrl = undefined; this.file = undefined; this.error = null; }

  submit() {
    if (this.form.invalid) return;
    this.loading = true; this.error = null;
    const content = this.form.value.content as string;
    this.posts.create(content, this.file).subscribe({
      next: () => { this.loading = false; this.router.navigate(['/home']); },
      error: (e) => { this.loading = false; this.error = e?.error?.message || 'Failed to create post'; }
    });
  }
}
