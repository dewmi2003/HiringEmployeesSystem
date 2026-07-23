import type {
  DashboardStatistics,
  DashboardJob,
  MonthlyStat,
  RecentApplication,
  TopCandidate,
} from "../models/dashboard";
import type { PagedResult } from "../models/common";
import apiClient from "./apiClient";

export async function getDashboardStatistics(): Promise<DashboardStatistics> {
  const { data } = await apiClient.get<DashboardStatistics>(
    "/recruiter-dashboard/statistics",
  );
  return data;
}

export async function getDashboardJobs(params?: {
  page?: number;
  pageSize?: number;
  search?: string;
  status?: string;
}): Promise<PagedResult<DashboardJob>> {
  const { data } = await apiClient.get<PagedResult<DashboardJob>>(
    "/recruiter-dashboard/jobs",
    { params },
  );
  return data;
}

export async function getRecentApplications(): Promise<RecentApplication[]> {
  const { data } = await apiClient.get<RecentApplication[]>(
    "/recruiter-dashboard/recent-applications",
  );
  return data;
}

export async function getTopCandidates(): Promise<TopCandidate[]> {
  const { data } = await apiClient.get<TopCandidate[]>(
    "/recruiter-dashboard/top-candidates",
  );
  return data;
}

export async function getMonthlyStats(
  type: "jobs" | "applications",
): Promise<MonthlyStat[]> {
  const { data } = await apiClient.get<MonthlyStat[]>(
    "/recruiter-dashboard/monthly-stats",
    { params: { type } },
  );
  return data;
}
