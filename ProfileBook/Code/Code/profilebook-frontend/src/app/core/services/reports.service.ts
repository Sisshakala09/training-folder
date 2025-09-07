// src/app/core/services/reports.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiBaseService } from './api-base.service';

export interface ReportDto {
  id: number;
  reportedUserId: string;
  reportedUserName?: string;
  reportingUserId?: string;
  reportingUserName?: string;
  reason: string;
  status: 'Open' | 'Reviewing' | 'Actioned' | 'Dismissed' | string;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class ReportsService extends ApiBaseService {
  constructor(http: HttpClient) { super(http); }

  reportUser(reportedUserId: string, reason: string): Observable<ReportDto> {
    return this.http.post<ReportDto>(`${this.base}/Reports`, { reportedUserId, reason });
  }

  listMine(): Observable<ReportDto[]> {
    return this.http.get<ReportDto[]>(`${this.base}/Reports/mine`);
  }

  // Admin: list reports with optional status, page, pageSize
  listAll(params?: { status?: string; page?: number; pageSize?: number }): Observable<ReportDto[]> {
    const query: any = {};
    if (params?.status) query.status = params.status;
    if (params?.page) query.page = String(params.page);
    if (params?.pageSize) query.pageSize = String(params.pageSize);
    return this.http.get<ReportDto[]>(`${this.base}/Reports`, { params: query });
  }

  // Admin: list all reports (unfiltered) via /Reports/all
  listAllReports(): Observable<ReportDto[]> {
    return this.http.get<ReportDto[]>(`${this.base}/Reports/all`);
  }

  // Admin: update report status
  updateStatus(reportId: number, status: string): Observable<any> {
    return this.http.put(`${this.base}/Reports/${reportId}/status`, { status });
  }
}
