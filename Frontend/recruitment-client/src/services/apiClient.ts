import axios, {
  AxiosError,
  type AxiosResponse,
  type InternalAxiosRequestConfig,
} from "axios";

export const AUTH_TOKEN_KEY = "talentai_token";
export const AUTH_USER_KEY = "talentai_user";
export const AUTH_UNAUTHORIZED_EVENT = "talentai:unauthorized";

const configuredBaseUrl = import.meta.env.VITE_API_BASE_URL?.trim();

function normalizeApiBaseUrl(value?: string): string {
  if (!value) return "/api";

  const normalized = value.replace(/\/+$/, "");

  if (/^https?:\/\//i.test(normalized) || normalized.startsWith("/")) {
    return normalized;
  }

  return `https://${normalized}`;
}

export const apiBaseUrl = normalizeApiBaseUrl(configuredBaseUrl);

export class ApiError extends Error {
  status?: number;
  details?: unknown;

  constructor(message: string, status?: number, details?: unknown) {
    super(message);
    this.name = "ApiError";
    this.status = status;
    this.details = details;
  }
}

const apiClient = axios.create({
  baseURL: apiBaseUrl,
  timeout: 30000,
  headers: {
    Accept: "application/json",
  },
});

apiClient.interceptors.request.use((config: InternalAxiosRequestConfig) => {
  const token = localStorage.getItem(AUTH_TOKEN_KEY);

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  if (!(config.data instanceof FormData)) {
    config.headers["Content-Type"] = "application/json";
  }

  return config;
});

apiClient.interceptors.response.use(
  (response: AxiosResponse) => response,
  (error: AxiosError) => {
    const status = error.response?.status;
    const data = error.response?.data as
      | {
          message?: string;
          title?: string;
          error?: string;
          errors?: unknown;
        }
      | string
      | undefined;

    if (
      status === 401 &&
      !error.config?.url?.toLowerCase().includes("/auth/")
    ) {
      window.dispatchEvent(new CustomEvent(AUTH_UNAUTHORIZED_EVENT));
    }

    let message = "Unable to complete the request.";

    if (!error.response) {
      message =
        "The server could not be reached. Check your connection and try again.";
    } else if (typeof data === "string" && data.trim()) {
      message = data;
    } else if (data && typeof data === "object") {
      const validationErrors = Array.isArray(data.errors)
        ? data.errors.filter((item): item is string => typeof item === "string")
        : [];

      message =
        data.message ||
        data.error ||
        data.title ||
        validationErrors.join(" ") ||
        message;
    }

    return Promise.reject(new ApiError(message, status, data));
  },
);

export function getErrorMessage(
  error: unknown,
  fallback = "Something went wrong. Please try again.",
): string {
  return error instanceof Error && error.message ? error.message : fallback;
}

export default apiClient;
