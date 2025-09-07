// src/app/core/services/api-base.service.ts
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ApiBaseService {
  protected base = environment.apiBaseUrl;
  constructor(protected http: HttpClient) {}
}
