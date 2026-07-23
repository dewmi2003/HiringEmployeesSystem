import type {
  Interview,
  ScheduleInterviewRequest,
} from "../models/interview";
import apiClient from "./apiClient";

export async function getInterviews(): Promise<Interview[]> {
  const { data } = await apiClient.get<Interview[]>("/interviews");
  return data;
}

export async function scheduleInterview(
  request: ScheduleInterviewRequest,
): Promise<Interview> {
  const { data } = await apiClient.post<Interview>(
    "/interviews/schedule",
    request,
  );
  return data;
}
