// src/app/app-routing.module.ts
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { PostDetailComponent } from './posts/post-detail.component';
import { UserDetailComponent } from './users/user-detail.component';
import { CreatePostComponent } from './posts/create-post.component';
import { AuthGuard } from './core/guards/auth.guard';
import { NotAdminGuard } from './core/guards/not-admin.guard';
import { ProfileEditComponent } from './users/profile-edit.component';
import { ChatComponent } from './chat/chat.component';
import { SearchComponent } from './search/search.component';
import { ReportsComponent } from './reports/reports.component';
import { AdminDashboardComponent } from './admin/admin-dashboard.component';
import { AdminUserEditComponent } from './admin/admin-user-edit.component';
import { AdminGroupDetailComponent } from './admin/admin-group-detail.component';
import { AdminGuard } from './core/guards/admin.guard';

const routes: Routes = [
  { path: 'users/:id', component: UserDetailComponent },
  { path: 'auth', loadChildren: () => import('./auth/auth.module').then(m => m.AuthModule) },
  { path: 'profile/edit', component: ProfileEditComponent, canActivate: [AuthGuard] },
  { path: 'chat', component: ChatComponent, canActivate: [AuthGuard, NotAdminGuard] },
  { path: 'chat/:userId', component: ChatComponent, canActivate: [AuthGuard, NotAdminGuard] },
  { path: 'search', component: SearchComponent },
  // Place the specific "posts/new" route BEFORE the dynamic ":id" route
  { path: 'posts/new', component: CreatePostComponent, canActivate: [AuthGuard, NotAdminGuard] },
  { path: 'posts/:id', component: PostDetailComponent },
  { path: 'home', component: HomeComponent },
  { path: 'reports', component: ReportsComponent, canActivate: [AuthGuard] },
  { path: 'admin', component: AdminDashboardComponent, canActivate: [AdminGuard] },
  { path: 'admin/users/:id/edit', component: AdminUserEditComponent, canActivate: [AdminGuard] },
  { path: 'admin/groups/:id', component: AdminGroupDetailComponent, canActivate: [AdminGuard] },
  { path: '', pathMatch: 'full', redirectTo: 'home' },
  { path: '**', redirectTo: 'home' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { scrollPositionRestoration: 'enabled' })],
  exports: [RouterModule]
})
export class AppRoutingModule {}
