import type {
  AuthResponse,
  LoginRequest,
  RegisterRequest,
} from "../models/auth";
import apiClient from "./apiClient";

export async function loginUser(
  request: LoginRequest,
): Promise<AuthResponse> {
  const { data } = await apiClient.post<AuthResponse>("/auth/login", request);
  return data;
}

export async function registerUser(
  request: RegisterRequest,
): Promise<AuthResponse> {
  const { data } = await apiClient.post<AuthResponse>(
    "/auth/register",
    request,
  );
  return data;
}
