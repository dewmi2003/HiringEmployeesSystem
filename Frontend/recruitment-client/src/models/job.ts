export type JobStatus = "Draft" | "Published" | "Closed" | "Archived" | string;

export interface Job {
  jobId: string;
  title: string;
  location: string | null;
  salary: number | null;
  status: JobStatus;
  companyName: string;
  createdDate: string;
}

export interface JobDetail extends Job {
  description: string;
  requirements: string;
  companyWebsite: string | null;
}

export interface JobCreateRequest {
  companyId: string;
  title: string;
  description: string;
  requirements: string;
  salary: number | null;
  location: string | null;
}

export interface JobFilters {
  title?: string;
  location?: string;
}
