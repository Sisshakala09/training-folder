// src/app/core/models/post.model.ts
export interface PostUser {
  // API may return userId or id, username or userName, profileImageUrl or profileImage
  id?: string;
  userId?: string;
  userName?: string;
  username?: string;
  profileImageUrl?: string | null;
  profileImage?: string | null;
}

export interface PostComment {
  id: number;
  userId: string;
  userName: string;
  text: string;
  createdAt: string;
}

export interface PostModel {
  id: number;
  content: string;
  imagePath?: string | null;
  createdAt?: string;
  status?: string;
  user?: PostUser | null;
  likesCount?: number;
  comments?: PostComment[];
  isMine?: boolean;
}
