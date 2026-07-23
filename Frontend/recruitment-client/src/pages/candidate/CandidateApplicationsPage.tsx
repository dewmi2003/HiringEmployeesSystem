import { useCallback, useEffect, useMemo, useState } from "react";
import { ExternalLink, Search } from "lucide-react";
import { Link } from "react-router-dom";
import AppBadge from "../../components/common/AppBadge";
import AppInput from "../../components/common/AppInput";
import AppLoader from "../../components/common/AppLoader";
import ErrorState from "../../components/common/ErrorState";
import PageHeader from "../../components/layout/PageHeader";
import DataTable, {
  type TableColumn,
} from "../../components/tables/DataTable";
import type { Application } from "../../models/application";
import { getMyApplications } from "../../services/applicationService.ts";
import { getErrorMessage } from "../../services/apiClient";
import { formatDate } from "../../utils/formatters";

export default function CandidateApplicationsPage() {
  const [applications, setApplications] = useState<Application[]>([]);
  const [query, setQuery] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const loadApplications = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      setApplications(await getMyApplications());
    } catch (loadError) {
      setError(getErrorMessage(loadError));
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    void loadApplications();
  }, [loadApplications]);

  const filteredApplications = useMemo(() => {
    const normalizedQuery = query.trim().toLowerCase();
    if (!normalizedQuery) return applications;
    return applications.filter((application) =>
      [
        application.jobTitle,
        application.companyName,
        application.status,
      ].some((value) => value.toLowerCase().includes(normalizedQuery)),
    );
  }, [applications, query]);

  const columns: TableColumn<Application>[] = [
    {
      key: "role",
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
    {
      key: "actions",
      header: "Actions",
      align: "right",
      render: (application) => (
        <div className="table-actions">
          <Link
            className="app-button app-button--ghost app-button--small"
            to={`/candidate/jobs/${application.jobId}`}
          >
            <ExternalLink size={15} aria-hidden="true" />
            View job
          </Link>
        </div>
      ),
    },
  ];

  return (
    <div className="animate-in">
      <PageHeader
        title="My applications"
        description="Follow each application from submission through the hiring process."
      />

      <div className="filter-bar">
        <AppInput
          label="Search applications"
          value={query}
          placeholder="Role, company, or status"
          icon={<Search size={18} aria-hidden="true" />}
          onChange={(event) => setQuery(event.target.value)}
        />
        <div className="field">
          <span className="field-label">Total</span>
          <div className="app-input">{filteredApplications.length} records</div>
        </div>
      </div>

      <section className="content-panel">
        {loading ? (
          <div className="state-panel">
            <AppLoader label="Loading applications" />
          </div>
        ) : error ? (
          <ErrorState message={error} onRetry={loadApplications} />
        ) : (
          <DataTable
            columns={columns}
            data={filteredApplications}
            getRowKey={(application) => application.applicationId}
            emptyTitle="No applications yet"
            emptyDescription="Applications you submit will appear here."
          />
        )}
      </section>
    </div>
  );
}
