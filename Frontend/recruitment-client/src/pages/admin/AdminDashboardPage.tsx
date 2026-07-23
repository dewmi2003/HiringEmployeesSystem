import { useCallback, useEffect, useState } from "react";
import { Link } from "react-router-dom";
import {
  BriefcaseBusiness,
  CalendarClock,
  ClipboardList,
  UserCheck,
  Users,
} from "lucide-react";
import StatCard from "../../components/cards/StatCard";
import AppLoader from "../../components/common/AppLoader";
import ErrorState from "../../components/common/ErrorState";
import PageHeader from "../../components/layout/PageHeader";
import type { DashboardStatistics } from "../../models/dashboard";
import type { ReportsOverview } from "../../models/report";
import { getErrorMessage } from "../../services/apiClient";
import { getDashboardStatistics } from "../../services/dashboardService";
import { getReportsOverview } from "../../services/reportService";
import { routes } from "../../utils/routePaths";

export default function AdminDashboardPage() {
  const [statistics, setStatistics] = useState<DashboardStatistics | null>(null);
  const [reports, setReports] = useState<ReportsOverview | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const loadDashboard = useCallback(async () => {
    setLoading(true);
    setError("");
    const [statisticsResult, reportsResult] = await Promise.allSettled([
      getDashboardStatistics(),
      getReportsOverview(),
    ]);

    if (statisticsResult.status === "fulfilled") {
      setStatistics(statisticsResult.value);
    }
    if (reportsResult.status === "fulfilled") {
      setReports(reportsResult.value);
    }
    if (
      statisticsResult.status === "rejected" &&
      reportsResult.status === "rejected"
    ) {
      setError(
        getErrorMessage(
          statisticsResult.reason,
          "Administration dashboard data could not be loaded.",
        ),
      );
    }
    setLoading(false);
  }, []);

  useEffect(() => {
    void loadDashboard();
  }, [loadDashboard]);

  if (loading) {
    return (
      <div className="state-panel">
        <AppLoader label="Loading administration dashboard" />
      </div>
    );
  }

  if (error) {
    return <ErrorState message={error} onRetry={loadDashboard} />;
  }

  return (
    <div className="animate-in">
      <PageHeader
        title="Administration dashboard"
        description="Monitor platform activity, governance, and recruitment performance."
        actions={
          <>
            <Link
              className="app-button app-button--secondary"
              to={routes.admin.companies}
            >
              Manage companies
            </Link>
            <Link
              className="app-button app-button--primary"
              to={routes.admin.reports}
            >
              Open reports
            </Link>
          </>
        }
      />

      <section className="metrics-grid" aria-label="Platform overview">
        <StatCard
          label="Candidates"
          value={reports?.recruitment.totalCandidates ?? "—"}
          detail={`${reports?.candidates.activeCandidates ?? 0} active profiles`}
          icon={Users}
        />
        <StatCard
          label="Jobs"
          value={statistics?.totalJobs ?? reports?.recruitment.totalJobs ?? "—"}
          detail={`${statistics?.openJobs ?? reports?.jobs.openJobs ?? 0} open vacancies`}
          icon={BriefcaseBusiness}
          tone="blue"
        />
        <StatCard
          label="Applications"
          value={
            statistics?.totalApplications ??
            reports?.recruitment.totalApplications ??
            "—"
          }
          detail={`${statistics?.shortlistedCandidates ?? 0} shortlisted`}
          icon={ClipboardList}
          tone="amber"
        />
        <StatCard
          label="Hired"
          value={
            statistics?.hiredCandidates ??
            reports?.recruitment.totalHired ??
            "—"
          }
          detail={`${reports?.recruitment.hiringRate.toFixed(1) ?? 0}% hiring rate`}
          icon={UserCheck}
          tone="green"
        />
      </section>

      <div className="content-grid content-grid--equal">
        <section className="content-panel">
          <div className="panel-header">
            <div>
              <h2>Recruitment health</h2>
              <p>Current workflow volumes requiring attention.</p>
            </div>
          </div>
          <dl className="detail-list">
            <div className="detail-row">
              <dt>Draft jobs</dt>
              <dd>{statistics?.draftJobs ?? 0}</dd>
            </div>
            <div className="detail-row">
              <dt>Closed jobs</dt>
              <dd>{statistics?.closedJobs ?? 0}</dd>
            </div>
            <div className="detail-row">
              <dt>Rejected candidates</dt>
              <dd>{statistics?.rejectedCandidates ?? 0}</dd>
            </div>
          </dl>
        </section>

        <section className="content-panel">
          <div className="panel-header">
            <div>
              <h2>Interview operations</h2>
              <p>Scheduled interview activity across recruiters.</p>
            </div>
          </div>
          <div className="list-row">
            <div>
              <p className="stat-label">Pending interviews</p>
              <p className="stat-value">
                {statistics?.pendingInterviews ??
                  reports?.interviews.pending ??
                  0}
              </p>
            </div>
            <CalendarClock size={34} aria-hidden="true" />
          </div>
        </section>
      </div>
    </div>
  );
}
