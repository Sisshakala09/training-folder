// src/app/chat/chat.component.ts
import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MessagesService, ThreadSummaryDto, MessageDto } from '../core/services/messages.service';
import { AuthService } from '../core/services/auth.service';
import { Subscription, switchMap } from 'rxjs';

@Component({
  selector: 'app-chat',
  standalone: false,
  template: `
  <main class="container" style="padding-top:16px; padding-bottom:16px;">
    <div class="chat-wrap card" style="display:grid; grid-template-columns: 280px 1fr; gap:12px; min-height:60vh;">
      <aside class="threads" style="border-right:1px solid #e5e7eb; padding-right:8px; overflow:auto;">
        <div style="display:flex; align-items:center; justify-content:space-between; margin-bottom:8px;">
          <h3 style="margin:0;">Messages</h3>
          <button class="btn-ghost small" (click)="refreshThreads()">Refresh</button>
        </div>
        <div *ngIf="threadsLoading" class="small muted">Loading…</div>
        <div *ngFor="let t of threads" (click)="openThread(t.userId)" class="thread-item" [class.active]="t.userId===activeUserId" style="padding:10px; border-radius:8px; cursor:pointer; display:flex; align-items:center; gap:10px;">
          <ng-container *ngIf="getThreadImage(t) as img; else initialAvatar">
            <img [src]="img" alt="avatar" style="width:34px; height:34px; border-radius:50%; object-fit:cover; border:1px solid #e5e7eb;"/>
          </ng-container>
          <ng-template #initialAvatar>
            <div class="avatar" [attr.data-initial]="getThreadInitial(t)"></div>
          </ng-template>
          <div style="flex:1; min-width:0;">
            <div><strong>{{ t.userName || t.email }}</strong></div>
            <div class="small muted" *ngIf="t.lastMessageOverall as lm">{{ lm.sentAt | date:'short' }}</div>
            <div class="small muted" *ngIf="t.lastMessageOverall as lm" style="white-space:nowrap; overflow:hidden; text-overflow:ellipsis;">{{ lm.body }}</div>
          </div>
          <span *ngIf="t.unreadCount" class="badge">{{ t.unreadCount }}</span>
        </div>
      </aside>
      <section class="conversation" style="display:flex; flex-direction:column;">
        <div *ngIf="activeUserId" class="conv-header" style="display:flex; align-items:center; gap:10px; padding:6px 2px; border-bottom:1px solid #e5e7eb; margin-bottom:8px;">
          <ng-container *ngIf="otherUserImage; else initial">
            <img [src]="otherUserImage!" alt="avatar" style="width:34px; height:34px; border-radius:50%; object-fit:cover; border:1px solid #e5e7eb;"/>
          </ng-container>
          <ng-template #initial>
            <div class="avatar" [attr.data-initial]="otherUserInitial"></div>
          </ng-template>
          <strong>{{ otherUserName }}</strong>
        </div>
        <div *ngIf="!activeUserId" class="center muted" style="flex:1;">Select a conversation</div>
        <div *ngIf="activeUserId" class="messages" style="flex:1; overflow:auto; display:flex; flex-direction:column; gap:8px; padding-right:4px;">
          <div *ngIf="convLoading" class="small muted">Loading…</div>
          <div *ngFor="let m of conversation" class="msg" [class.me]="m.senderId===currentUserId">
            <div class="small muted">{{ m.sentAt | date:'short' }}</div>
            <div>{{ m.body }}</div>
          </div>
        </div>
        <form *ngIf="activeUserId" (ngSubmit)="send()" style="display:flex; gap:8px; margin-top:8px;">
          <input class="input" type="text" [(ngModel)]="draft" name="draft" placeholder="Type a message…" required />
          <button class="btn primary" type="submit" [disabled]="sending">Send</button>
        </form>
      </section>
    </div>
  </main>
  `,
  styles: [`
    .thread-item:hover { background:#f9fafb; }
    .thread-item.active { background:#eef2ff; }
    .avatar { width:34px; height:34px; border-radius:50%; background:#e5e7eb; display:flex; align-items:center; justify-content:center; font-weight:700; color:#374151; position:relative; }
    .avatar::after { content: attr(data-initial); }
    .badge { background:#111827; color:#fff; border-radius:999px; padding:2px 6px; font-size:12px; }
  .msg { color:#111827; background:#f3f4f6; border:1px solid #e5e7eb; max-width:70%; padding:8px 10px; border-radius:12px; align-self:flex-start; }
  .msg.me { align-self:flex-end; background:#dbeafe; border-color:#93c5fd; text-align:right; }
    .muted { color:#6b7280; }
    .card { background:#fff; border:1px solid #e5e7eb; border-radius:12px; padding:12px; }
    .btn { padding:8px 14px; border:1px solid rgba(0,0,0,0.1); border-radius:8px; background:#fff; cursor:pointer; }
    .btn.primary { background:#111827; color:#fff; border-color:#111827; }
    .input { padding:10px 12px; border:1px solid #e5e7eb; border-radius:8px; background:#fff; flex:1; }
  `]
})
export class ChatComponent implements OnInit, OnDestroy {
  threads: ThreadSummaryDto[] = [];
  threadsLoading = false;
  conversation: MessageDto[] = [];
  convLoading = false;
  activeUserId: string | null = null;
  draft = '';
  sending = false;
  currentUserId: string | null = null;
  private sub?: Subscription;
  private subAuth?: Subscription;
  otherUserName = '';
  otherUserImage: string | null = null;
  otherUserInitial = '';

  constructor(private route: ActivatedRoute, private router: Router, private messages: MessagesService, private auth: AuthService) {}

  ngOnInit(): void {
    const current = (this.auth as any).userSub?.value as any;
    this.currentUserId = current?.id || current?.userId || null;
  this.subAuth = this.auth.user$.subscribe((u: any) => { this.currentUserId = u?.id || u?.userId || null; });
    this.loadThreads();
    this.sub = this.route.paramMap.pipe(
      switchMap(params => {
        const uid = params.get('userId');
        this.activeUserId = uid;
        this.deriveOtherUserFromThreads();
        if (!uid) { this.conversation = []; return [] as any; }
        this.convLoading = true;
        return this.messages.getConversation(uid);
      })
    ).subscribe((msgs: any) => {
      if (Array.isArray(msgs)) this.conversation = msgs;
      this.convLoading = false;
      this.deriveOtherUserFromConversation();
      // scroll to bottom on load
      setTimeout(() => { const el = document.querySelector('.messages'); if (el) el.scrollTop = (el as any).scrollHeight; }, 0);
    });
  }

  ngOnDestroy(): void { this.sub?.unsubscribe(); this.subAuth?.unsubscribe(); }

  loadThreads() {
    this.threadsLoading = true;
    this.messages.listThreads().subscribe({
    next: t => { this.threads = t; this.threadsLoading = false; this.deriveOtherUserFromThreads(); },
      error: () => { this.threadsLoading = false; }
    });
  }

  refreshThreads() { this.loadThreads(); if (this.activeUserId) this.reloadConversation(); }

  openThread(userId: string) { this.router.navigate(['/chat', userId]); }

  send() {
    if (!this.activeUserId || !this.draft.trim()) return;
    const text = this.draft.trim();
    this.sending = true;
    this.messages.sendMessage(this.activeUserId, text).subscribe({
      next: m => {
        this.draft = '';
        this.sending = false;
        this.conversation = [...this.conversation, m];
        this.refreshThreads();
        setTimeout(() => { const el = document.querySelector('.messages'); if (el) el.scrollTop = (el as any).scrollHeight; }, 0);
      },
      error: () => { this.sending = false; }
    });
  }

  private reloadConversation() {
    if (!this.activeUserId) return;
    this.convLoading = true;
    this.messages.getConversation(this.activeUserId).subscribe({
      next: msgs => { this.conversation = msgs; this.convLoading = false; this.deriveOtherUserFromConversation(); setTimeout(() => { const el = document.querySelector('.messages'); if (el) el.scrollTop = (el as any).scrollHeight; }, 0); },
      error: () => { this.convLoading = false; }
    });
  }

  private deriveOtherUserFromThreads() {
    if (!this.activeUserId) { this.otherUserName = ''; this.otherUserImage = null; this.otherUserInitial = ''; return; }
    const found = this.threads.find(x => x.userId === this.activeUserId);
    const name = found?.userName || found?.email || '';
    if (name) { this.otherUserName = name; this.otherUserInitial = (name || '?').trim().charAt(0).toUpperCase(); }
    const lm = found?.lastMessageOverall || found?.lastMessageFromUserToMe || null;
    const img = lm?.sender?.id === this.activeUserId ? (lm?.sender?.profileImageUrl || null)
             : lm?.receiver?.id === this.activeUserId ? (lm?.receiver?.profileImageUrl || null)
             : null;
    if (img) this.otherUserImage = img;
  }

  private deriveOtherUserFromConversation() {
    if (!this.activeUserId) return;
    if (!this.otherUserName || !this.otherUserImage) {
      const anyMsg = this.conversation.find(m => m.sender?.id === this.activeUserId || m.receiver?.id === this.activeUserId);
      const u = anyMsg?.sender?.id === this.activeUserId ? anyMsg?.sender : (anyMsg?.receiver?.id === this.activeUserId ? anyMsg?.receiver : undefined);
      if (u) {
        this.otherUserName = u.userName || this.otherUserName;
        this.otherUserImage = u.profileImageUrl || this.otherUserImage;
        this.otherUserInitial = (this.otherUserName || '?').trim().charAt(0).toUpperCase();
      }
    }
  }

  getThreadImage(t: ThreadSummaryDto): string | null {
    const lm = t.lastMessageOverall || t.lastMessageFromUserToMe || null;
    if (!lm) return null;
    if (lm.sender?.id === t.userId) return lm.sender?.profileImageUrl || null;
    if (lm.receiver?.id === t.userId) return lm.receiver?.profileImageUrl || null;
    return null;
  }

  getThreadInitial(t: ThreadSummaryDto): string {
    const name = t.userName || t.email || '?';
    return (name.trim().charAt(0) || '?').toUpperCase();
  }
}
