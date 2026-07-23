import { useCallback, useEffect, useState } from "react";
import { Eye, Plus, Search } from "lucide-react";
import { Link, useSearchParams } from "react-router-dom";
import AppBadge from "../../components/common/AppBadge";
import AppInput from "../../components/common/AppInput";
import AppLoader from "../../components/common/AppLoader";
import AppSelect from "../../components/common/AppSelect";
import ErrorState from "../../components/common/ErrorState";
import PageHeader from "../../components/layout/PageHeader";
import DataTable, {
  type TableColumn,
} from "../../components/tables/DataTable";
import { useDebounce } from "../../hooks/useDebounce";
import type { DashboardJob } from "../../models/dashboard";
import { getErrorMessage } from "../../services/apiClient";
import { getDashboardJobs } from "../../services/dashboardService";
import { formatDate } from "../../utils/formatters";
import { routes } from "../../utils/routePaths";

interface RecruiterJobsPageProps {
  adminMode?: boolean;
}

export default function RecruiterJobsPage({
  adminMode = false,
}: RecruiterJobsPageProps) {
  const [searchParams] = useSearchParams();
  const [search, setSearch] = useState(searchParams.get("search") || "");
  const [status, setStatus] = useState("");
  const [jobs, setJobs] = useState<DashboardJob[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const debouncedSearch = useDebounce(search);

  const loadJobs = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      const result = await getDashboardJobs({
        page: 1,
        pageSize: 100,
        search: debouncedSearch || undefined,
        status: status || undefined,
      });
      setJobs(result.items || []);
      setTotalCount(result.totalCount || 0);
    } catch (loadError) {
      setError(getErrorMessage(loadError));
    } finally {
      setLoading(false);
    }
  }, [debouncedSearch, status]);

  useEffect(() => {
    void loadJobs();
  }, [loadJobs]);

  const columns: TableColumn<DashboardJob>[] = [
    {
      key: "job",
      header: "Job",
      render: (job) => (
        <div>
          <div className="table-primary">{job.title}</div>
          <div className="table-secondary">
            {job.department || "Department not set"}
          </div>
        </div>
      ),
    },
    {
      key: "location",
      header: "Location",
      render: (job) => job.location || "Flexible",
    },
    {
      key: "applications",
      header: "Applications",
      render: (job) => job.applicationCount,
    },
    {
      key: "created",
      header: "Created",
      render: (job) => formatDate(job.postedDate),
    },
    {
      key: "status",
      header: "Status",
      render: (job) => <AppBadge>{job.status}</AppBadge>,
    },
    {
      key: "actions",
      header: "Actions",
      align: "right",
      render: (job) => (
        <div className="table-actions">
          <Link
            className="app-button app-button--ghost app-button--small"
            to={`${adminMode ? routes.admin.applications : routes.recruiter.applications}?jobId=${job.id}`}
          >
            <Eye size={15} aria-hidden="true" />
            Applicants
          </Link>
        </div>
      ),
    },
  ];

  return (
    <div className="animate-in">
      <PageHeader
        title={adminMode ? "Job oversight" : "Job management"}
        description={
          adminMode
            ? "Monitor vacancies and application activity across the platform."
            : "Create, monitor, and manage company vacancies."
        }
        actions={
          adminMode ? null : (
          <Link
            className="app-button app-button--primary"
            to={routes.recruiter.createJob}
          >
            <Plus size={17} aria-hidden="true" />
            Create job
          </Link>
          )
        }
      />

      <div className="filter-bar">
        <AppInput
          label="Search jobs"
          value={search}
          placeholder="Title or company"
          icon={<Search size={18} aria-hidden="true" />}
          onChange={(event) => setSearch(event.target.value)}
        />
        <AppSelect
          label="Status"
          value={status}
          options={[
            { label: "Draft", value: "Draft" },
            { label: "Published", value: "Published" },
            { label: "Closed", value: "Closed" },
            { label: "Archived", value: "Archived" },
          ]}
          onChange={(event) => setStatus(event.target.value)}
        />
        <div className="field">
          <span className="field-label">Results</span>
          <div className="app-input">{totalCount} jobs</div>
        </div>
      </div>

      <section className="content-panel">
        {loading ? (
          <div className="state-panel">
            <AppLoader label="Loading jobs" />
          </div>
        ) : error ? (
          <ErrorState message={error} onRetry={loadJobs} />
        ) : (
          <DataTable
            columns={columns}
            data={jobs}
            getRowKey={(job) => job.id}
            emptyTitle="No jobs found"
            emptyDescription="Create a vacancy or adjust your filters."
          />
        )}
      </section>
    </div>
  );
}
