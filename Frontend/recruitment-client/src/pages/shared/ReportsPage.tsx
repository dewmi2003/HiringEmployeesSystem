import { useCallback, useEffect, useState } from "react";
import {
  BriefcaseBusiness,
  CalendarCheck,
  ClipboardList,
  UserCheck,
  Users,
} from "lucide-react";
import StatCard from "../../components/cards/StatCard";
import AppLoader from "../../components/common/AppLoader";
import ErrorState from "../../components/common/ErrorState";
import PageHeader from "../../components/layout/PageHeader";
import type { ReportsOverview } from "../../models/report";
import { getErrorMessage } from "../../services/apiClient";
import { getReportsOverview } from "../../services/reportService";

export default function ReportsPage() {
  const [reports, setReports] = useState<ReportsOverview | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const loadReports = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      setReports(await getReportsOverview());
    } catch (loadError) {
      setError(getErrorMessage(loadError));
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    void loadReports();
  }, [loadReports]);

  if (loading) {
    return (
      <div className="state-panel">
        <AppLoader label="Loading recruitment reports" />
      </div>
    );
  }

  if (!reports) {
    return <ErrorState message={error} onRetry={loadReports} />;
  }

  return (
    <div className="animate-in">
      <PageHeader
        title="Reports and analytics"
        description="Review platform-level recruitment activity and hiring outcomes."
      />

      <section className="metrics-grid" aria-label="Report summary">
        <StatCard
          label="Candidates"
          value={reports.recruitment.totalCandidates}
          detail={`${reports.candidates.activeCandidates} active candidates`}
          icon={Users}
        />
        <StatCard
          label="Jobs"
          value={reports.recruitment.totalJobs}
          detail={`${reports.jobs.openJobs} currently open`}
          icon={BriefcaseBusiness}
          tone="blue"
        />
        <StatCard
          label="Applications"
          value={reports.recruitment.totalApplications}
          detail={`${reports.candidates.candidatesApplied} candidates applied`}
          icon={ClipboardList}
          tone="amber"
        />
        <StatCard
          label="Interviews"
          value={reports.recruitment.totalInterviews}
          detail={`${reports.interviews.pending} pending`}
          icon={CalendarCheck}
          tone="green"
        />
      </section>

      <div className="content-grid content-grid--equal">
        <section className="content-panel">
          <div className="panel-header">
            <div>
              <h2>Hiring outcome</h2>
              <p>Selection results across the recruitment pipeline.</p>
            </div>
          </div>
          <div className="list-row">
            <div>
              <p className="stat-label">Total hired</p>
              <p className="stat-value">{reports.recruitment.totalHired}</p>
            </div>
            <UserCheck size={32} aria-hidden="true" />
          </div>
          <progress
            className="score-progress"
            value={Math.max(
              0,
              Math.min(100, reports.recruitment.hiringRate),
            )}
            max={100}
            aria-label="Hiring rate"
          />
          <p className="field-hint">
            Hiring rate: {reports.recruitment.hiringRate.toFixed(1)}%
          </p>
        </section>

        <section className="content-panel">
          <div className="panel-header">
            <div>
              <h2>Interview completion</h2>
              <p>Completed and pending scheduled interviews.</p>
            </div>
          </div>
          <dl className="detail-list">
            <div className="detail-row">
              <dt>Completed</dt>
              <dd>{reports.interviews.completed}</dd>
            </div>
            <div className="detail-row">
              <dt>Pending</dt>
              <dd>{reports.interviews.pending}</dd>
            </div>
            <div className="detail-row">
              <dt>Total</dt>
              <dd>{reports.interviews.totalInterviews}</dd>
            </div>
          </dl>
        </section>
      </div>
    </div>
  );
}
