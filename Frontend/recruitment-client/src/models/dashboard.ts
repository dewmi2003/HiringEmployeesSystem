export interface DashboardStatistics {
  totalJobs: number;
  openJobs: number;
  closedJobs: number;
  draftJobs: number;
  totalApplications: number;
  shortlistedCandidates: number;
  rejectedCandidates: number;
  hiredCandidates: number;
  pendingInterviews: number;
}

export interface RecentApplication {
  applicationId: string;
  candidateId: string;
  candidateName: string;
  candidateEmail: string;
  jobId: string;
  jobTitle: string;
  status: string;
  appliedDate: string;
}

export interface TopCandidate {
  candidateId: string;
  fullName: string;
  email: string;
  averageEvaluationScore: number;
  recommendation: string;
  applicationCount: number;
}

export interface MonthlyStat {
  year: number;
  month: number;
  monthName: string;
  count: number;
}

export interface DashboardJob {
  id: string;
  title: string;
  status: string;
  department: string;
  location: string;
  applicationCount: number;
  postedDate: string;
}
