import { createContext } from "react";
import type {
  AuthUser,
  LoginRequest,
  RegisterRequest,
} from "../models/auth";

export interface AuthContextValue {
  user: AuthUser | null;
  token: string | null;
  isAuthenticated: boolean;
  signIn: (request: LoginRequest) => Promise<AuthUser>;
  signUp: (request: RegisterRequest) => Promise<AuthUser>;
  signOut: () => void;
}

export const AuthContext = createContext<AuthContextValue | undefined>(
  undefined,
);
