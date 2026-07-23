export type UserRole = "Admin" | "Recruiter" | "Candidate";

export interface AuthUser {
  userId?: string;
  email: string;
  fullName: string;
  role: UserRole;
  expiresAt: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
  role: Exclude<UserRole, "Admin">;
}

export interface AuthResponse extends AuthUser {
  token: string;
}
