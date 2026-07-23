import { useCallback, useEffect, useState } from "react";
import { Link } from "react-router-dom";
import {
  BriefcaseBusiness,
  CalendarClock,
  ClipboardList,
  UserRoundCheck,
} from "lucide-react";
import StatCard from "../../components/cards/StatCard";
import ApplicationsChart from "../../components/charts/ApplicationsChart";
import AppBadge from "../../components/common/AppBadge";
import AppLoader from "../../components/common/AppLoader";
import ErrorState from "../../components/common/ErrorState";
import PageHeader from "../../components/layout/PageHeader";
import DataTable, {
  type TableColumn,
} from "../../components/tables/DataTable";
import type {
  DashboardStatistics,
  MonthlyStat,
  RecentApplication,
  TopCandidate,
} from "../../models/dashboard";
import {
  getDashboardStatistics,
  getMonthlyStats,
  getRecentApplications,
  getTopCandidates,
} from "../../services/dashboardService";
import { formatDate } from "../../utils/formatters";
import { routes } from "../../utils/routePaths";

interface RecruiterDashboardData {
  statistics: DashboardStatistics | null;
  recentApplications: RecentApplication[];
  topCandidates: TopCandidate[];
  monthlyStats: MonthlyStat[];
}

const initialData: RecruiterDashboardData = {
  statistics: null,
  recentApplications: [],
  topCandidates: [],
  monthlyStats: [],
};

export default function RecruiterDashboardPage() {
  const [data, setData] = useState(initialData);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const loadDashboard = useCallback(async () => {
    setLoading(true);
    setError("");
    const results = await Promise.allSettled([
      getDashboardStatistics(),
      getRecentApplications(),
      getTopCandidates(),
      getMonthlyStats("applications"),
    ]);

    setData({
      statistics: results[0].status === "fulfilled" ? results[0].value : null,
      recentApplications:
        results[1].status === "fulfilled" ? results[1].value : [],
      topCandidates:
        results[2].status === "fulfilled" ? results[2].value : [],
      monthlyStats:
        results[3].status === "fulfilled" ? results[3].value : [],
    });

    if (results.every((result) => result.status === "rejected")) {
      setError("Recruitment dashboard data could not be loaded.");
    }
    setLoading(false);
  }, []);

  useEffect(() => {
    void loadDashboard();
  }, [loadDashboard]);

  const applicationColumns: TableColumn<RecentApplication>[] = [
    {
      key: "candidate",
      header: "Candidate",
      render: (application) => (
        <div>
          <div className="table-primary">{application.candidateName}</div>
          <div className="table-secondary">{application.candidateEmail}</div>
        </div>
      ),
    },
    {
      key: "role",
      header: "Role",
      render: (application) => application.jobTitle,
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
        <AppLoader label="Loading recruitment dashboard" />
      </div>
    );
  }

  if (error) {
    return <ErrorState message={error} onRetry={loadDashboard} />;
  }

  const stats = data.statistics;

  return (
    <div className="animate-in">
      <PageHeader
        title="Recruiter dashboard"
        description="Monitor vacancies, applicants, interviews, and hiring progress."
        actions={
          <>
            <Link
              className="app-button app-button--secondary"
              to={routes.recruiter.applications}
            >
              Review applications
            </Link>
            <Link
              className="app-button app-button--primary"
              to={routes.recruiter.createJob}
            >
              Create job
            </Link>
          </>
        }
      />

      <section className="metrics-grid" aria-label="Recruitment overview">
        <StatCard
          label="Open jobs"
          value={stats?.openJobs ?? "—"}
          detail={`${stats?.totalJobs ?? 0} jobs in total`}
          icon={BriefcaseBusiness}
        />
        <StatCard
          label="Applications"
          value={stats?.totalApplications ?? "—"}
          detail="Across all vacancies"
          icon={ClipboardList}
          tone="blue"
        />
        <StatCard
          label="Shortlisted"
          value={stats?.shortlistedCandidates ?? "—"}
          detail={`${stats?.hiredCandidates ?? 0} candidates hired`}
          icon={UserRoundCheck}
          tone="green"
        />
        <StatCard
          label="Pending interviews"
          value={stats?.pendingInterviews ?? "—"}
          detail="Require coordination or follow-up"
          icon={CalendarClock}
          tone="amber"
        />
      </section>

      <div className="content-grid">
        <section className="content-panel">
          <div className="panel-header">
            <div>
              <h2>Application activity</h2>
              <p>Monthly application volume across the platform.</p>
            </div>
          </div>
          <ApplicationsChart data={data.monthlyStats} />
        </section>

        <section className="content-panel">
          <div className="panel-header">
            <div>
              <h2>Top candidates</h2>
              <p>Ranked by recorded evaluation results.</p>
            </div>
          </div>
          <div className="stack">
            {data.topCandidates.slice(0, 5).map((candidate, index) => (
              <div className="list-row" key={candidate.candidateId}>
                <div>
                  <div className="table-primary">
                    {index + 1}. {candidate.fullName}
                  </div>
                  <div className="table-secondary">
                    {candidate.applicationCount} applications
                  </div>
                </div>
                <AppBadge tone="primary">
                  {candidate.averageEvaluationScore.toFixed(1)}
                </AppBadge>
              </div>
            ))}
          </div>
        </section>
      </div>

      <section className="content-panel section-gap">
        <div className="panel-header">
          <div>
            <h2>Recent applications</h2>
            <p>Latest candidates entering the recruitment pipeline.</p>
          </div>
          <Link to={routes.recruiter.applications}>View all</Link>
        </div>
        <DataTable
          columns={applicationColumns}
          data={data.recentApplications}
          getRowKey={(application) => application.applicationId}
          emptyTitle="No applications yet"
          emptyDescription="New candidate applications will appear here."
        />
      </section>
    </div>
  );
}
