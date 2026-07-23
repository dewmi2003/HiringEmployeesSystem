import { useCallback, useEffect, useState } from "react";
import { ExternalLink, MapPin, Send, WalletCards } from "lucide-react";
import { Link, useParams } from "react-router-dom";
import AppBadge from "../../components/common/AppBadge";
import AppButton from "../../components/common/AppButton";
import AppLoader from "../../components/common/AppLoader";
import ErrorState from "../../components/common/ErrorState";
import PageHeader from "../../components/layout/PageHeader";
import type { JobDetail } from "../../models/job";
import { applyToJob } from "../../services/applicationService.ts";
import { getErrorMessage } from "../../services/apiClient";
import { getJob } from "../../services/jobService.ts";
import {
  formatCurrency,
  formatDate,
} from "../../utils/formatters";
import { routes } from "../../utils/routePaths";

export default function JobDetailsPage() {
  const { jobId } = useParams();
  const [job, setJob] = useState<JobDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [applying, setApplying] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const loadJob = useCallback(async () => {
    if (!jobId) return;
    setLoading(true);
    setError("");
    try {
      setJob(await getJob(jobId));
    } catch (loadError) {
      setError(getErrorMessage(loadError));
    } finally {
      setLoading(false);
    }
  }, [jobId]);

  useEffect(() => {
    void loadJob();
  }, [loadJob]);

  const apply = async () => {
    if (!jobId) return;
    setApplying(true);
    setError("");
    setSuccess("");
    try {
      await applyToJob(jobId);
      setSuccess("Your application was submitted successfully.");
    } catch (applyError) {
      setError(getErrorMessage(applyError));
    } finally {
      setApplying(false);
    }
  };

  if (loading) {
    return (
      <div className="state-panel">
        <AppLoader label="Loading job details" />
      </div>
    );
  }

  if (!job) {
    return <ErrorState message={error} onRetry={loadJob} />;
  }

  return (
    <div className="animate-in">
      <PageHeader
        title={job.title}
        description={`${job.companyName} · ${job.location || "Flexible location"}`}
        badge={<AppBadge>{job.status}</AppBadge>}
        actions={
          <>
            <Link
              className="app-button app-button--secondary"
              to={routes.candidate.jobs}
            >
              Back to jobs
            </Link>
            <AppButton
              loading={applying}
              icon={<Send size={17} aria-hidden="true" />}
              onClick={() => void apply()}
            >
              Apply now
            </AppButton>
          </>
        }
      />

      {error ? (
        <div className="alert alert--error" role="alert">
          {error}
        </div>
      ) : null}
      {success ? (
        <div className="alert alert--success" role="status">
          {success}
        </div>
      ) : null}

      <div className="content-grid">
        <div className="stack">
          <section className="content-panel">
            <div className="panel-header">
              <div>
                <h2>Role overview</h2>
                <p>Published {formatDate(job.createdDate)}</p>
              </div>
            </div>
            <p>{job.description}</p>
          </section>
          <section className="content-panel">
            <div className="panel-header">
              <div>
                <h2>Requirements</h2>
                <p>Experience and capabilities expected for this role.</p>
              </div>
            </div>
            <p className="pre-wrap">{job.requirements}</p>
          </section>
        </div>

        <aside className="content-panel">
          <div className="panel-header">
            <div>
              <h2>Job details</h2>
              <p>Key information about this vacancy.</p>
            </div>
          </div>
          <dl className="detail-list">
            <div className="detail-row">
              <dt>Company</dt>
              <dd>{job.companyName}</dd>
            </div>
            <div className="detail-row">
              <dt>Location</dt>
              <dd>
                <span className="inline-list">
                  <MapPin size={15} aria-hidden="true" />
                  {job.location || "Flexible"}
                </span>
              </dd>
            </div>
            <div className="detail-row">
              <dt>Salary</dt>
              <dd>
                <span className="inline-list">
                  <WalletCards size={15} aria-hidden="true" />
                  {formatCurrency(job.salary)}
                </span>
              </dd>
            </div>
            {job.companyWebsite ? (
              <div className="detail-row">
                <dt>Website</dt>
                <dd>
                  <a
                    className="inline-list"
                    href={job.companyWebsite}
                    target="_blank"
                    rel="noreferrer"
                  >
                    Visit company
                    <ExternalLink size={14} aria-hidden="true" />
                  </a>
                </dd>
              </div>
            ) : null}
          </dl>
        </aside>
      </div>
    </div>
  );
}
