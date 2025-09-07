import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { PostDetailComponent } from './posts/post-detail.component';
import { CreatePostComponent } from './posts/create-post.component';
import { NavbarComponent } from './shared/navbar/navbar.component';
import { PostCardComponent } from './shared/post-card/post-card.component';
import { UserDetailComponent } from './users/user-detail.component';
import { ProfileEditComponent } from './users/profile-edit.component';
import { ChatComponent } from './chat/chat.component';
import { SearchComponent } from './search/search.component';
import { ReportsComponent } from './reports/reports.component';
import { AdminDashboardComponent } from './admin/admin-dashboard.component';
import { AdminUserEditComponent } from './admin/admin-user-edit.component';
import { AdminGroupDetailComponent } from './admin/admin-group-detail.component';
// src/app/app.module.ts
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';

import { AuthInterceptor } from './core/interceptors/auth.interceptor';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    PostDetailComponent,
  CreatePostComponent,
    NavbarComponent,
    PostCardComponent,
  UserDetailComponent,
  ProfileEditComponent,
  ChatComponent,
  SearchComponent,
  ReportsComponent,
  AdminDashboardComponent,
  AdminUserEditComponent,
  AdminGroupDetailComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    CommonModule,
    RouterModule,
  FormsModule,
  ReactiveFormsModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
