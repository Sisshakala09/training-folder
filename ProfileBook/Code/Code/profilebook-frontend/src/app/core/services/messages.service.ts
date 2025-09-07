// src/app/core/services/messages.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiBaseService } from './api-base.service';

export interface MessageDto {
  id: number;
  senderId: string;
  receiverId: string;
  body: string;
  sentAt: string;
  sender?: { id: string; userName?: string; profileImageUrl?: string | null };
  receiver?: { id: string; userName?: string; profileImageUrl?: string | null };
}

export interface ThreadSummaryDto {
  userId: string;
  userName: string;
  email: string;
  lastMessageFromUserToMe?: MessageDto | null;
  lastMessageOverall?: MessageDto | null;
  unreadCount?: number;
}

@Injectable({ providedIn: 'root' })
export class MessagesService extends ApiBaseService {
  constructor(http: HttpClient) { super(http); }

  // GET /api/Messages -> list of conversations with last message
  listThreads(): Observable<ThreadSummaryDto[]> {
    return this.http.get<ThreadSummaryDto[]>(`${this.base}/Messages`);
  }

  // GET /api/Messages/{userId} -> full conversation with specific user
  getConversation(otherUserId: string): Observable<MessageDto[]> {
    return this.http.get<MessageDto[]>(`${this.base}/Messages/${otherUserId}`);
  }

  // POST /api/Messages -> send a message
  sendMessage(receiverId: string, body: string): Observable<MessageDto> {
    return this.http.post<MessageDto>(`${this.base}/Messages`, { receiverId, body });
  }
}
