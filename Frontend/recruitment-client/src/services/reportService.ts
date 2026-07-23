import type {
  CandidateReport,
  InterviewReport,
  JobReport,
  RecruitmentReport,
  ReportsOverview,
} from "../models/report";
import apiClient from "./apiClient";

export async function getReportsOverview(): Promise<ReportsOverview> {
  const [recruitment, candidates, jobs, interviews] = await Promise.all([
    apiClient.get<RecruitmentReport>("/reports/recruitment"),
    apiClient.get<CandidateReport>("/reports/candidates"),
    apiClient.get<JobReport>("/reports/jobs"),
    apiClient.get<InterviewReport>("/reports/interviews"),
  ]);

  return {
    recruitment: recruitment.data,
    candidates: candidates.data,
    jobs: jobs.data,
    interviews: interviews.data,
  };
}
