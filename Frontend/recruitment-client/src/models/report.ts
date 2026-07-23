export interface RecruitmentReport {
  totalCandidates: number;
  totalJobs: number;
  totalApplications: number;
  totalInterviews: number;
  totalHired: number;
  hiringRate: number;
}

export interface CandidateReport {
  totalCandidates: number;
  activeCandidates: number;
  candidatesApplied: number;
}

export interface JobReport {
  totalJobs: number;
  openJobs: number;
  closedJobs: number;
}

export interface InterviewReport {
  totalInterviews: number;
  completed: number;
  pending: number;
}

export interface ReportsOverview {
  recruitment: RecruitmentReport;
  candidates: CandidateReport;
  jobs: JobReport;
  interviews: InterviewReport;
}
