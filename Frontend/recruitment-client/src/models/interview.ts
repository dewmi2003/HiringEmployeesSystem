export interface Interview {
  interviewId: string;
  applicationId: string;
  candidateFullName: string;
  candidateEmail?: string;
  jobTitle: string;
  interviewDate: string;
  interviewEndDate?: string;
  location: string | null;
  interviewStatus: string;
  calendarResult?: string;
  emailResult?: string;
}

export interface ScheduleInterviewRequest {
  applicationId: string;
  interviewDate: string;
  location: string | null;
}
