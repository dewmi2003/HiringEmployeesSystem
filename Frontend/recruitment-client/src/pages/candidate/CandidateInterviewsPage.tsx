import { useCallback, useEffect, useState } from "react";
import { CalendarDays } from "lucide-react";
import AppBadge from "../../components/common/AppBadge";
import AppLoader from "../../components/common/AppLoader";
import EmptyState from "../../components/common/EmptyState";
import ErrorState from "../../components/common/ErrorState";
import PageHeader from "../../components/layout/PageHeader";
import type { Application } from "../../models/application";
import { getMyApplications } from "../../services/applicationService.ts";
import { getErrorMessage } from "../../services/apiClient";

export default function CandidateInterviewsPage() {
  const [applications, setApplications] = useState<Application[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const loadInterviewApplications = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      const items = await getMyApplications();
      setApplications(
        items.filter((item) => item.status === "InterviewScheduled"),
      );
    } catch (loadError) {
      setError(getErrorMessage(loadError));
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    void loadInterviewApplications();
  }, [loadInterviewApplications]);

  return (
    <div className="animate-in">
      <PageHeader
        title="Interviews"
        description="Review applications that have progressed to the interview stage."
      />
      <section className="content-panel">
        {loading ? (
          <div className="state-panel">
            <AppLoader label="Checking interview status" />
          </div>
        ) : error ? (
          <ErrorState
            message={error}
            onRetry={loadInterviewApplications}
          />
        ) : applications.length === 0 ? (
          <EmptyState
            title="No interviews scheduled"
            description="Interview updates will appear when a recruiter schedules your application."
            icon={CalendarDays}
          />
        ) : (
          <div className="stack">
            {applications.map((application) => (
              <article className="interview-card" key={application.applicationId}>
                <div className="list-row">
                  <div>
                    <h2>{application.jobTitle}</h2>
                    <p>{application.companyName}</p>
                  </div>
                  <AppBadge>{application.status}</AppBadge>
                </div>
                <p className="field-hint">
                  Calendar details will appear when available in your
                  notification or interview email.
                </p>
              </article>
            ))}
          </div>
        )}
      </section>
    </div>
  );
}
