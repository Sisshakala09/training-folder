// src/app/reports/reports.component.ts
import { Component, OnInit } from '@angular/core';
import { ReportsService, ReportDto } from '../core/services/reports.service';

@Component({
  selector: 'app-reports',
  standalone: false,
  template: `
  <main class="container" style="padding-top:16px; padding-bottom:24px;">
    <div class="card" style="padding:12px; margin-bottom:12px; display:flex; align-items:center; justify-content:space-between;">
      <h2 style="margin:0;">My Reports</h2>
      <button class="btn-ghost" (click)="reload()">Refresh</button>
    </div>

    <div *ngIf="loading" class="card">Loading…</div>
    <div *ngIf="!loading && reports.length === 0" class="card">No reports yet.</div>

    <div *ngFor="let r of reports" class="card" style="margin-bottom:10px;">
      <div style="display:flex; align-items:center; justify-content:space-between;">
        <div>
          <div style="font-weight:700;">Reported: {{ r.reportedUserName || r.reportedUserId }}</div>
          <div class="small muted">Status: {{ r.status }} • {{ r.createdAt | date:'short' }}</div>
        </div>
      </div>
      <div style="margin-top:8px; white-space:pre-wrap;">{{ r.reason }}</div>
    </div>
  </main>
  `
})
export class ReportsComponent implements OnInit {
  reports: ReportDto[] = [];
  loading = false;

  constructor(private reportsSvc: ReportsService) {}

  ngOnInit(): void { this.reload(); }

  reload() {
    this.loading = true;
    this.reportsSvc.listMine().subscribe({
      next: r => { this.reports = r || []; this.loading = false; },
      error: () => { this.reports = []; this.loading = false; }
    });
  }
}
