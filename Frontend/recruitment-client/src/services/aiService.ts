import type {
  CandidateRanking,
  InterviewQuestions,
  JobMatch,
  ResumeAnalysis,
} from "../models/ai";
import apiClient from "./apiClient";

export async function analyzeResume(request: {
  resumeText: string;
  jobDescription?: string;
  candidateName?: string;
}): Promise<ResumeAnalysis> {
  const { data } = await apiClient.post<ResumeAnalysis>(
    "/ai/resume-analysis",
    request,
  );
  return data;
}

export async function analyzeStoredResume(
  resumeId: string,
  jobId?: string,
): Promise<ResumeAnalysis> {
  const { data } = await apiClient.get<ResumeAnalysis>(
    `/ai/resumes/${resumeId}/analysis`,
    { params: jobId ? { jobId } : undefined },
  );
  return data;
}

export async function matchJob(request: {
  resumeText: string;
  jobDescription: string;
  jobTitle?: string;
}): Promise<JobMatch> {
  const { data } = await apiClient.post<JobMatch>("/ai/job-match", request);
  return data;
}

export async function rankJobApplicants(
  jobId: string,
): Promise<CandidateRanking> {
  const { data } = await apiClient.get<CandidateRanking>(
    `/ai/jobs/${jobId}/candidate-ranking`,
  );
  return data;
}

export async function generateInterviewQuestions(request: {
  jobDescription: string;
  candidateSummary: string;
  count: number;
}): Promise<InterviewQuestions> {
  const { data } = await apiClient.post<InterviewQuestions>(
    "/ai/interview-questions",
    request,
  );
  return data;
}
