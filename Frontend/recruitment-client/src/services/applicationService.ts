import type { Application } from "../models/application";
import apiClient from "./apiClient";

export async function applyToJob(jobId: string): Promise<{ id: string }> {
  const { data } = await apiClient.post<{ id: string }>("/applications", {
    jobId,
  });
  return data;
}

export async function getMyApplications(): Promise<Application[]> {
  const { data } = await apiClient.get<Application[]>("/applications/my");
  return data;
}

export async function getApplicationsByJob(
  jobId: string,
): Promise<Application[]> {
  const { data } = await apiClient.get<Application[]>(
    `/applications/job/${jobId}`,
  );
  return data;
}

export async function updateApplicationStatus(
  applicationId: string,
  status: string,
): Promise<void> {
  await apiClient.patch(`/applications/${applicationId}/status`, { status });
}
