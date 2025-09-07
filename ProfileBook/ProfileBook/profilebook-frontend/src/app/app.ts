import { Component } from '@angular/core';
//import { AuthService } from './services/auth.service';

import { AuthService } from './services/auth';

//import { ApiService } from './services/api.service';
import { ApiService } from './services/api';

@Component({
  selector: 'app-root',
  templateUrl: './app.html'
})
export class AppComponent {
  posts: any[] = [];
  file?: File;
  constructor(public auth: AuthService, private api: ApiService) {}

  register(u:string, e:string, p:string){ this.auth.register({username:u,email:e,password:p}).subscribe(r=>console.log(r)); }
  login(u:string, p:string){ this.auth.login({username:u,password:p}).subscribe(()=>console.log('logged in')); }
  onFile(e:any){ this.file = e.target.files[0]; }
  createPost(){ const content = (document.querySelector('textarea') as HTMLTextAreaElement).value; this.api.getUsers().subscribe((r: any) => {
  console.log(r);
});
 }
  loadPosts(){ this.api.getApprovedPosts().subscribe((res:any)=> this.posts = res); }
}
