import { useCallback, useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import {
  BriefcaseBusiness,
  CalendarClock,
  ClipboardCheck,
  UserRoundCheck,
} from "lucide-react";
import AIInsightCard from "../../components/cards/AIInsightCard";
import JobCard from "../../components/cards/JobCard";
import StatCard from "../../components/cards/StatCard";
import AppBadge from "../../components/common/AppBadge";
import AppLoader from "../../components/common/AppLoader";
import ErrorState from "../../components/common/ErrorState";
import PageHeader from "../../components/layout/PageHeader";
import DataTable, {
  type TableColumn,
} from "../../components/tables/DataTable";
import { useAuth } from "../../hooks/useAuth";
import type { Application } from "../../models/application";
import type { CandidateProfile } from "../../models/candidate";
import type { Job } from "../../models/job";
import { getMyApplications } from "../../services/applicationService.ts";
import { getMyCandidateProfile } from "../../services/candidateService.ts";
import { getJobs } from "../../services/jobService.ts";
import { formatDate } from "../../utils/formatters";
import { routes } from "../../utils/routePaths";

interface DashboardData {
  profile: CandidateProfile | null;
  applications: Application[];
  jobs: Job[];
}

const initialData: DashboardData = {
  profile: null,
  applications: [],
  jobs: [],
};

export default function CandidateDashboardPage() {
  const { user } = useAuth();
  const [data, setData] = useState<DashboardData>(initialData);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const loadDashboard = useCallback(async () => {
    setLoading(true);
    setError("");
    const [profileResult, applicationsResult, jobsResult] =
      await Promise.allSettled([
        getMyCandidateProfile(),
        getMyApplications(),
        getJobs(),
      ]);

    const nextData: DashboardData = {
      profile:
        profileResult.status === "fulfilled" ? profileResult.value : null,
      applications:
        applicationsResult.status === "fulfilled"
          ? applicationsResult.value
          : [],
      jobs: jobsResult.status === "fulfilled" ? jobsResult.value : [],
    };

    setData(nextData);
    if (
      profileResult.status === "rejected" &&
      applicationsResult.status === "rejected" &&
      jobsResult.status === "rejected"
    ) {
      setError("The dashboard data could not be loaded from the API.");
    }
    setLoading(false);
  }, []);

  useEffect(() => {
    void loadDashboard();
  }, [loadDashboard]);

  const profileCompleteness = useMemo(() => {
    if (!data.profile) return 0;
    const values = [
      data.profile.phone,
      data.profile.address,
      data.profile.bio,
      data.profile.experience,
      data.profile.education,
      data.profile.skills.length > 0 ? "skills" : null,
    ];
    return Math.round(
      (values.filter((value) => Boolean(value)).length / values.length) * 100,
    );
  }, [data.profile]);

  const activeApplications = data.applications.filter(
    (application) =>
      !["Rejected", "Hired"].includes(application.status),
  ).length;
  const interviews = data.applications.filter(
    (application) => application.status === "InterviewScheduled",
  ).length;

  const applicationColumns: TableColumn<Application>[] = [
    {
      key: "job",
      header: "Role",
      render: (application) => (
        <div>
          <div className="table-primary">{application.jobTitle}</div>
          <div className="table-secondary">{application.companyName}</div>
        </div>
      ),
    },
    {
      key: "date",
      header: "Applied",
      render: (application) => formatDate(application.appliedDate),
    },
    {
      key: "status",
      header: "Status",
      render: (application) => <AppBadge>{application.status}</AppBadge>,
    },
  ];

  if (loading) {
    return (
      <div className="state-panel">
        <AppLoader label="Loading your dashboard" />
      </div>
    );
  }

  if (error) {
    return <ErrorState message={error} onRetry={loadDashboard} />;
  }

  return (
    <div className="animate-in">
      <PageHeader
        title={`Welcome, ${data.profile?.fullName || user?.fullName || "Candidate"}`}
        description="Track your profile, applications, and relevant opportunities from one place."
        actions={
          <>
            <Link
              className="app-button app-button--secondary"
              to={routes.candidate.profile}
            >
              Edit profile
            </Link>
            <Link
              className="app-button app-button--primary"
              to={routes.candidate.jobs}
            >
              Find jobs
            </Link>
          </>
        }
      />

      <section className="metrics-grid" aria-label="Candidate overview">
        <StatCard
          label="Profile complete"
          value={`${profileCompleteness}%`}
          detail="Complete profiles help recruiters assess your experience."
          icon={UserRoundCheck}
        />
        <StatCard
          label="Applications"
          value={data.applications.length}
          detail={`${activeApplications} currently active`}
          icon={ClipboardCheck}
          tone="blue"
        />
        <StatCard
          label="Interviews"
          value={interviews}
          detail="Applications at interview stage"
          icon={CalendarClock}
          tone="green"
        />
        <StatCard
          label="Open opportunities"
          value={data.jobs.length}
          detail="Published jobs available now"
          icon={BriefcaseBusiness}
          tone="amber"
        />
      </section>

      <div className="content-grid">
        <section className="content-panel">
          <div className="panel-header">
            <div>
              <h2>Recent applications</h2>
              <p>Your latest submitted applications and statuses.</p>
            </div>
            <Link to={routes.candidate.applications}>View all</Link>
          </div>
          <DataTable
            columns={applicationColumns}
            data={data.applications.slice(0, 5)}
            getRowKey={(application) => application.applicationId}
            emptyTitle="No applications yet"
            emptyDescription="Your submitted applications will appear here."
          />
        </section>

        <AIInsightCard
          title="Improve your next match"
          summary={
            profileCompleteness < 80
              ? "Complete your profile and resume before running a job match."
              : "Your profile is ready for a role-specific AI job match."
          }
        >
          <div className="card-actions">
            <Link
              className="app-button app-button--secondary"
              to={routes.candidate.jobMatches}
            >
              Open AI matching
            </Link>
          </div>
        </AIInsightCard>
      </div>

      <section className="content-panel section-gap">
        <div className="panel-header">
          <div>
            <h2>Recommended opportunities</h2>
            <p>Recently published roles available for application.</p>
          </div>
          <Link to={routes.candidate.jobs}>Browse all jobs</Link>
        </div>
        <div className="job-list">
          {data.jobs.slice(0, 3).map((job) => (
            <JobCard
              key={job.jobId}
              job={job}
              detailsPath={`/candidate/jobs/${job.jobId}`}
            />
          ))}
        </div>
      </section>
    </div>
  );
}
