import { useCallback, useEffect, useMemo, useState } from "react";
import { jwtDecode } from "jwt-decode";
import type { PropsWithChildren } from "react";
import type {
  AuthResponse,
  AuthUser,
  LoginRequest,
  RegisterRequest,
} from "../models/auth";
import { loginUser, registerUser } from "../services/authService.ts";
import {
  AUTH_TOKEN_KEY,
  AUTH_UNAUTHORIZED_EVENT,
  AUTH_USER_KEY,
} from "../services/apiClient";
import { AuthContext, type AuthContextValue } from "./AuthContext.tsx";

interface TokenClaims {
  sub?: string;
  exp?: number;
}

function clearStoredSession(): void {
  localStorage.removeItem(AUTH_TOKEN_KEY);
  localStorage.removeItem(AUTH_USER_KEY);
  localStorage.removeItem("token");
  localStorage.removeItem("user");
}

function toAuthUser(response: AuthResponse): AuthUser {
  let userId: string | undefined;

  try {
    userId = jwtDecode<TokenClaims>(response.token).sub;
  } catch {
    userId = undefined;
  }

  return {
    userId,
    email: response.email,
    fullName: response.fullName,
    role: response.role,
    expiresAt: response.expiresAt,
  };
}

function readStoredSession(): { user: AuthUser | null; token: string | null } {
  try {
    const token =
      localStorage.getItem(AUTH_TOKEN_KEY) || localStorage.getItem("token");
    const rawUser =
      localStorage.getItem(AUTH_USER_KEY) || localStorage.getItem("user");

    if (!token || !rawUser) return { user: null, token: null };

    const user = JSON.parse(rawUser) as AuthUser;
    const decoded = jwtDecode<TokenClaims>(token);

    if (decoded.exp && decoded.exp * 1000 <= Date.now()) {
      clearStoredSession();
      return { user: null, token: null };
    }

    return {
      token,
      user: { ...user, userId: user.userId || decoded.sub },
    };
  } catch {
    clearStoredSession();
    return { user: null, token: null };
  }
}

export default function AuthProvider({ children }: PropsWithChildren) {
  const [initialSession] = useState(readStoredSession);
  const [user, setUser] = useState<AuthUser | null>(initialSession.user);
  const [token, setToken] = useState<string | null>(initialSession.token);

  const storeSession = useCallback((response: AuthResponse): AuthUser => {
    const nextUser = toAuthUser(response);
    localStorage.setItem(AUTH_TOKEN_KEY, response.token);
    localStorage.setItem(AUTH_USER_KEY, JSON.stringify(nextUser));
    setToken(response.token);
    setUser(nextUser);
    return nextUser;
  }, []);

  const signIn = useCallback(
    async (request: LoginRequest) => storeSession(await loginUser(request)),
    [storeSession],
  );

  const signUp = useCallback(
    async (request: RegisterRequest) =>
      storeSession(await registerUser(request)),
    [storeSession],
  );

  const signOut = useCallback(() => {
    clearStoredSession();
    setToken(null);
    setUser(null);
  }, []);

  useEffect(() => {
    window.addEventListener(AUTH_UNAUTHORIZED_EVENT, signOut);
    return () => window.removeEventListener(AUTH_UNAUTHORIZED_EVENT, signOut);
  }, [signOut]);

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      token,
      isAuthenticated: Boolean(user && token),
      signIn,
      signUp,
      signOut,
    }),
    [signIn, signOut, signUp, token, user],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
