export type ApplicationStatus =
  | "Submitted"
  | "UnderReview"
  | "Shortlisted"
  | "InterviewScheduled"
  | "Rejected"
  | "Selected"
  | "Hired"
  | string;

export interface Application {
  applicationId: string;
  jobId: string;
  jobTitle: string;
  companyName: string;
  status: ApplicationStatus;
  appliedDate: string;
  candidateId?: string;
  candidateFullName?: string;
  candidateEmail?: string;
}
