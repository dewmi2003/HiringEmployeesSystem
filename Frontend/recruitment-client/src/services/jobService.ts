import type {
  Job,
  JobCreateRequest,
  JobDetail,
  JobFilters,
} from "../models/job";
import apiClient from "./apiClient";

export async function getJobs(filters: JobFilters = {}): Promise<Job[]> {
  const { data } = await apiClient.get<Job[]>("/jobs", { params: filters });
  return data;
}

export async function getJob(jobId: string): Promise<JobDetail> {
  const { data } = await apiClient.get<JobDetail>(`/jobs/${jobId}`);
  return data;
}

export async function createJob(
  request: JobCreateRequest,
): Promise<{ id: string }> {
  const { data } = await apiClient.post<{ id: string }>("/jobs", request);
  return data;
}

export async function updateJob(
  jobId: string,
  request: Partial<Omit<JobCreateRequest, "companyId">> & { status?: string },
): Promise<void> {
  await apiClient.put(`/jobs/${jobId}`, request);
}

export async function deleteJob(jobId: string): Promise<void> {
  await apiClient.delete(`/jobs/${jobId}`);
}
