import { lazy, Suspense } from "react";
import { Navigate, Route, Routes } from "react-router-dom";
import {
  Bot,
  Settings,
  ShieldCheck,
  Users,
} from "lucide-react";
import AppLoader from "../components/common/AppLoader";
import AuthLayout from "../components/layout/AuthLayout";
import DashboardLayout from "../components/layout/DashboardLayout";
import ProtectedRoute from "../components/navigation/ProtectedRoute";
import RoleRoute from "../components/navigation/RoleRoute";
import { useAuth } from "../hooks/useAuth";
import { getRoleHome, routes } from "../utils/routePaths";

const LoginPage = lazy(() => import("../pages/auth/LoginPage"));
const RegisterPage = lazy(() => import("../pages/auth/RegisterPage"));
const CandidateDashboardPage = lazy(
  () => import("../pages/candidate/CandidateDashboardPage"),
);
const CandidateProfilePage = lazy(
  () => import("../pages/candidate/CandidateProfilePage"),
);
const CandidateJobsPage = lazy(
  () => import("../pages/candidate/CandidateJobsPage"),
);
const JobDetailsPage = lazy(
  () => import("../pages/candidate/JobDetailsPage"),
);
const CandidateApplicationsPage = lazy(
  () => import("../pages/candidate/CandidateApplicationsPage"),
);
const ResumePage = lazy(() => import("../pages/candidate/ResumePage"));
const CandidateInterviewsPage = lazy(
  () => import("../pages/candidate/CandidateInterviewsPage"),
);
const JobMatchesPage = lazy(
  () => import("../pages/candidate/JobMatchesPage"),
);
const RecruiterDashboardPage = lazy(
  () => import("../pages/recruiter/RecruiterDashboardPage"),
);
const RecruiterJobsPage = lazy(
  () => import("../pages/recruiter/RecruiterJobsPage"),
);
const CreateJobPage = lazy(
  () => import("../pages/recruiter/CreateJobPage"),
);
const RecruiterApplicationsPage = lazy(
  () => import("../pages/recruiter/RecruiterApplicationsPage"),
);
const InterviewsPage = lazy(
  () => import("../pages/recruiter/InterviewsPage"),
);
const CandidatesPage = lazy(
  () => import("../pages/recruiter/CandidatesPage"),
);
const CandidateDetailsPage = lazy(
  () => import("../pages/recruiter/CandidateDetailsPage"),
);
const CandidateRankingPage = lazy(
  () => import("../pages/ai/CandidateRankingPage"),
);
const InterviewQuestionsPage = lazy(
  () => import("../pages/ai/InterviewQuestionsPage"),
);
const ReportsPage = lazy(() => import("../pages/shared/ReportsPage"));
const NotificationsPage = lazy(
  () => import("../pages/shared/NotificationsPage"),
);
const AdminDashboardPage = lazy(
  () => import("../pages/admin/AdminDashboardPage"),
);
const CompaniesPage = lazy(() => import("../pages/admin/CompaniesPage"));
const AuditLogsPage = lazy(() => import("../pages/admin/AuditLogsPage"));
const ModulePage = lazy(() => import("../pages/shared/ModulePage"));
const UnauthorizedPage = lazy(
  () => import("../pages/shared/UnauthorizedPage"),
);
const NotFoundPage = lazy(() => import("../pages/shared/NotFoundPage"));

function RouteLoader() {
  return (
    <div className="state-panel">
      <AppLoader label="Opening workspace" />
    </div>
  );
}

function HomeRedirect() {
  const { user, isAuthenticated } = useAuth();
  return (
    <Navigate
      to={isAuthenticated && user ? getRoleHome(user.role) : routes.login}
      replace
    />
  );
}

export default function AppRoutes() {
  return (
    <Suspense fallback={<RouteLoader />}>
      <Routes>
        <Route path="/" element={<HomeRedirect />} />

        <Route element={<AuthLayout />}>
          <Route path={routes.login} element={<LoginPage />} />
          <Route path={routes.register} element={<RegisterPage />} />
        </Route>

        <Route element={<ProtectedRoute />}>
          <Route path={routes.unauthorized} element={<UnauthorizedPage />} />
          <Route element={<DashboardLayout />}>
            <Route
              path="/notifications"
              element={<NotificationsPage />}
            />

            <Route element={<RoleRoute allowedRoles={["Candidate"]} />}>
              <Route
                path={routes.candidate.dashboard}
                element={<CandidateDashboardPage />}
              />
              <Route
                path={routes.candidate.profile}
                element={<CandidateProfilePage />}
              />
              <Route
                path={routes.candidate.jobs}
                element={<CandidateJobsPage />}
              />
              <Route
                path="/candidate/jobs/:jobId"
                element={<JobDetailsPage />}
              />
              <Route
                path={routes.candidate.applications}
                element={<CandidateApplicationsPage />}
              />
              <Route
                path={routes.candidate.resumes}
                element={<ResumePage />}
              />
              <Route
                path={routes.candidate.interviews}
                element={<CandidateInterviewsPage />}
              />
              <Route
                path={routes.candidate.jobMatches}
                element={<JobMatchesPage />}
              />
              <Route
                path={routes.candidate.notifications}
                element={<NotificationsPage />}
              />
              <Route
                path="/candidate/settings"
                element={
                  <ModulePage
                    title="Candidate settings"
                    description="Manage account and workspace preferences."
                    message="Additional candidate preferences will appear when their API settings are available."
                    icon={Settings}
                  />
                }
              />
            </Route>

            <Route element={<RoleRoute allowedRoles={["Recruiter"]} />}>
              <Route
                path={routes.recruiter.dashboard}
                element={<RecruiterDashboardPage />}
              />
              <Route
                path={routes.recruiter.jobs}
                element={<RecruiterJobsPage />}
              />
              <Route
                path={routes.recruiter.createJob}
                element={<CreateJobPage />}
              />
              <Route
                path={routes.recruiter.applications}
                element={<RecruiterApplicationsPage />}
              />
              <Route
                path={routes.recruiter.interviews}
                element={<InterviewsPage />}
              />
              <Route
                path={routes.recruiter.candidates}
                element={<CandidatesPage />}
              />
              <Route
                path="/recruiter/candidates/:candidateId"
                element={<CandidateDetailsPage />}
              />
              <Route
                path={routes.recruiter.ranking}
                element={<CandidateRankingPage />}
              />
              <Route
                path={routes.recruiter.questions}
                element={<InterviewQuestionsPage />}
              />
              <Route
                path={routes.recruiter.reports}
                element={<ReportsPage />}
              />
              <Route
                path="/recruiter/settings"
                element={
                  <ModulePage
                    title="Recruiter settings"
                    description="Manage recruiter account and workflow preferences."
                    message="Additional recruiter preferences will appear when their API settings are available."
                    icon={Settings}
                  />
                }
              />
            </Route>

            <Route element={<RoleRoute allowedRoles={["Admin"]} />}>
              <Route
                path={routes.admin.dashboard}
                element={<AdminDashboardPage />}
              />
              <Route
                path={routes.admin.companies}
                element={<CompaniesPage />}
              />
              <Route
                path={routes.admin.jobs}
                element={<RecruiterJobsPage adminMode />}
              />
              <Route
                path={routes.admin.applications}
                element={<RecruiterApplicationsPage adminMode />}
              />
              <Route
                path={routes.admin.interviews}
                element={<InterviewsPage />}
              />
              <Route
                path={routes.admin.reports}
                element={<ReportsPage />}
              />
              <Route
                path={routes.admin.auditLogs}
                element={<AuditLogsPage />}
              />
              <Route
                path={routes.admin.users}
                element={
                  <ModulePage
                    title="User management"
                    description="Manage platform users and account access."
                    message="User records will appear when the administration user API is available."
                    icon={Users}
                  />
                }
              />
              <Route
                path="/admin/roles"
                element={
                  <ModulePage
                    title="Roles and permissions"
                    description="Review platform roles and access policies."
                    message="Role management will appear when its administration API is available."
                    icon={ShieldCheck}
                  />
                }
              />
              <Route
                path={routes.admin.aiUsage}
                element={
                  <ModulePage
                    title="AI usage"
                    description="Monitor responsible use of AI-assisted recruitment features."
                    message="AI usage metrics will appear when telemetry is exposed by the backend."
                    icon={Bot}
                  />
                }
              />
              <Route
                path="/admin/settings"
                element={
                  <ModulePage
                    title="Platform settings"
                    description="Manage platform-wide configuration and preferences."
                    message="Platform settings will appear when the administration settings API is available."
                    icon={Settings}
                  />
                }
              />
            </Route>
          </Route>
        </Route>

        <Route path="*" element={<NotFoundPage />} />
      </Routes>
    </Suspense>
  );
}
