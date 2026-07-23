import type { UserRole } from "../models/auth";

export const routes = {
  login: "/login",
  register: "/register",
  unauthorized: "/unauthorized",
  candidate: {
    dashboard: "/candidate/dashboard",
    profile: "/candidate/profile",
    jobs: "/candidate/jobs",
    applications: "/candidate/applications",
    resumes: "/candidate/resumes",
    interviews: "/candidate/interviews",
    jobMatches: "/candidate/ai-job-matches",
    notifications: "/candidate/notifications",
  },
  recruiter: {
    dashboard: "/recruiter/dashboard",
    jobs: "/recruiter/jobs",
    createJob: "/recruiter/jobs/new",
    applications: "/recruiter/applications",
    candidates: "/recruiter/candidates",
    interviews: "/recruiter/interviews",
    ranking: "/recruiter/ai-ranking",
    questions: "/recruiter/ai-questions",
    reports: "/recruiter/reports",
  },
  admin: {
    dashboard: "/admin/dashboard",
    users: "/admin/users",
    companies: "/admin/companies",
    jobs: "/admin/jobs",
    applications: "/admin/applications",
    interviews: "/admin/interviews",
    reports: "/admin/reports",
    auditLogs: "/admin/audit-logs",
    aiUsage: "/admin/ai-usage",
  },
} as const;

export function getRoleHome(role: UserRole): string {
  if (role === "Admin") return routes.admin.dashboard;
  if (role === "Recruiter") return routes.recruiter.dashboard;
  return routes.candidate.dashboard;
}
