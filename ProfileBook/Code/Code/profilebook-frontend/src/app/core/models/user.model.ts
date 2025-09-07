// src/app/core/models/user.model.ts
export interface UserModel {
  id?: string;
  username?: string;
  email?: string;
  roles?: string[];
  profileImage?: string | null;
}
