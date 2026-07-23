import { useCallback, useEffect, useState } from "react";
import { CalendarPlus, RefreshCw } from "lucide-react";
import { Link, useSearchParams } from "react-router-dom";
import AppBadge from "../../components/common/AppBadge";
import AppButton from "../../components/common/AppButton";
import AppLoader from "../../components/common/AppLoader";
import AppSelect from "../../components/common/AppSelect";
import EmptyState from "../../components/common/EmptyState";
import ErrorState from "../../components/common/ErrorState";
import PageHeader from "../../components/layout/PageHeader";
import DataTable, {
  type TableColumn,
} from "../../components/tables/DataTable";
import type { Application } from "../../models/application";
import type { Job } from "../../models/job";
import {
  getApplicationsByJob,
  updateApplicationStatus,
} from "../../services/applicationService.ts";
import { getErrorMessage } from "../../services/apiClient";
import { getJobs } from "../../services/jobService.ts";
import { formatDate } from "../../utils/formatters";
import { routes } from "../../utils/routePaths";

const statusOptions = [
  "Submitted",
  "UnderReview",
  "Shortlisted",
  "InterviewScheduled",
  "Rejected",
  "Selected",
  "Hired",
];

interface RecruiterApplicationsPageProps {
  adminMode?: boolean;
}

export default function RecruiterApplicationsPage({
  adminMode = false,
}: RecruiterApplicationsPageProps) {
  const [searchParams, setSearchParams] = useSearchParams();
  const [jobs, setJobs] = useState<Job[]>([]);
  const [selectedJobId, setSelectedJobId] = useState(
    searchParams.get("jobId") || "",
  );
  const [applications, setApplications] = useState<Application[]>([]);
  const [loadingJobs, setLoadingJobs] = useState(true);
  const [loadingApplications, setLoadingApplications] = useState(false);
  const [updatingId, setUpdatingId] = useState("");
  const [error, setError] = useState("");

  useEffect(() => {
    getJobs()
      .then((items) => {
        setJobs(items);
        if (!selectedJobId && items[0]) {
          setSelectedJobId(items[0].jobId);
        }
      })
      .catch((loadError) => setError(getErrorMessage(loadError)))
      .finally(() => setLoadingJobs(false));
  }, [selectedJobId]);

  const loadApplications = useCallback(async () => {
    if (!selectedJobId) {
      setApplications([]);
      return;
    }
    setLoadingApplications(true);
    setError("");
    try {
      setApplications(await getApplicationsByJob(selectedJobId));
      setSearchParams({ jobId: selectedJobId }, { replace: true });
    } catch (loadError) {
      setError(getErrorMessage(loadError));
    } finally {
      setLoadingApplications(false);
    }
  }, [selectedJobId, setSearchParams]);

  useEffect(() => {
    void loadApplications();
  }, [loadApplications]);

  const updateStatus = async (
    applicationId: string,
    status: string,
  ) => {
    setUpdatingId(applicationId);
    setError("");
    try {
      await updateApplicationStatus(applicationId, status);
      setApplications((items) =>
        items.map((item) =>
          item.applicationId === applicationId ? { ...item, status } : item,
        ),
      );
    } catch (updateError) {
      setError(getErrorMessage(updateError));
    } finally {
      setUpdatingId("");
    }
  };

  const columns: TableColumn<Application>[] = [
    {
      key: "candidate",
      header: "Candidate",
      render: (application) => (
        <div>
          <div className="table-primary">
            {application.candidateFullName || "Candidate"}
          </div>
          <div className="table-secondary">
            {application.candidateEmail || "Email not available"}
          </div>
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
      header: "Current status",
      render: (application) => <AppBadge>{application.status}</AppBadge>,
    },
    {
      key: "decision",
      header: "Update status",
      render: (application) => (
        <select
          className="app-select table-select"
          value={application.status}
          disabled={updatingId === application.applicationId}
          aria-label={`Update ${application.candidateFullName || "candidate"} status`}
          onChange={(event) =>
            void updateStatus(application.applicationId, event.target.value)
          }
        >
          {statusOptions.map((status) => (
            <option key={status} value={status}>
              {status}
            </option>
          ))}
        </select>
      ),
    },
    {
      key: "actions",
      header: "Actions",
      align: "right",
      render: (application) => (
        <div className="table-actions">
          <Link
            className="app-button app-button--ghost app-button--small"
            to={`${adminMode ? routes.admin.interviews : routes.recruiter.interviews}?applicationId=${application.applicationId}`}
          >
            <CalendarPlus size={15} aria-hidden="true" />
            Schedule
          </Link>
        </div>
      ),
    },
  ];

  return (
    <div className="animate-in">
      <PageHeader
        title="Applications"
        description="Review applicants, update pipeline status, and schedule interviews."
        actions={
          <AppButton
            variant="secondary"
            icon={<RefreshCw size={16} aria-hidden="true" />}
            onClick={() => void loadApplications()}
          >
            Refresh
          </AppButton>
        }
      />

      {error ? (
        <div className="alert alert--error" role="alert">
          {error}
        </div>
      ) : null}

      <div className="filter-bar">
        <AppSelect
          label="Job vacancy"
          value={selectedJobId}
          placeholder={loadingJobs ? "Loading jobs" : "Select a job"}
          options={jobs.map((job) => ({
            label: `${job.title} · ${job.companyName}`,
            value: job.jobId,
          }))}
          onChange={(event) => setSelectedJobId(event.target.value)}
        />
        <div className="field">
          <span className="field-label">Applicants</span>
          <div className="app-input">{applications.length} records</div>
        </div>
      </div>

      <section className="content-panel">
        {loadingApplications ? (
          <div className="state-panel">
            <AppLoader label="Loading applications" />
          </div>
        ) : !selectedJobId ? (
          <EmptyState
            title="Select a vacancy"
            description="Choose a job to review its candidate applications."
          />
        ) : error && applications.length === 0 ? (
          <ErrorState message={error} onRetry={loadApplications} />
        ) : (
          <DataTable
            columns={columns}
            data={applications}
            getRowKey={(application) => application.applicationId}
            emptyTitle="No applicants for this job"
            emptyDescription="Applications will appear after candidates apply."
          />
        )}
      </section>
    </div>
  );
}
