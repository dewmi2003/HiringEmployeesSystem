import type {
  CandidateProfile,
  CandidateProfileUpdate,
} from "../models/candidate";
import apiClient from "./apiClient";

export async function getMyCandidateProfile(): Promise<CandidateProfile> {
  const { data } = await apiClient.get<CandidateProfile>("/candidates/me");
  return data;
}

export async function getCandidateProfile(
  candidateId: string,
): Promise<CandidateProfile> {
  const { data } = await apiClient.get<CandidateProfile>(
    `/candidates/${candidateId}`,
  );
  return data;
}

export async function updateCandidateProfile(
  candidateId: string,
  request: CandidateProfileUpdate,
): Promise<void> {
  await apiClient.put(`/candidates/${candidateId}`, request);
}
