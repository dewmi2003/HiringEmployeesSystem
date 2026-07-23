import { useCallback, useEffect, useState } from "react";
import { FileSearch, Search, UserRoundSearch } from "lucide-react";
import { Link } from "react-router-dom";
import AppBadge from "../../components/common/AppBadge";
import AppInput from "../../components/common/AppInput";
import AppLoader from "../../components/common/AppLoader";
import ErrorState from "../../components/common/ErrorState";
import PageHeader from "../../components/layout/PageHeader";
import DataTable, {
  type TableColumn,
} from "../../components/tables/DataTable";
import { useDebounce } from "../../hooks/useDebounce";
import type { Resume } from "../../models/resume";
import { getErrorMessage } from "../../services/apiClient";
import { searchResumes } from "../../services/resumeService";
import {
  formatDate,
  formatFileSize,
} from "../../utils/formatters";

export default function CandidatesPage() {
  const [query, setQuery] = useState("");
  const [resumes, setResumes] = useState<Resume[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const debouncedQuery = useDebounce(query);

  const loadCandidates = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      setResumes(await searchResumes(debouncedQuery));
    } catch (loadError) {
      setError(getErrorMessage(loadError));
    } finally {
      setLoading(false);
    }
  }, [debouncedQuery]);

  useEffect(() => {
    void loadCandidates();
  }, [loadCandidates]);

  const columns: TableColumn<Resume>[] = [
    {
      key: "candidate",
      header: "Candidate record",
      render: (resume) => (
        <div>
          <div className="table-primary">{resume.fileName}</div>
          <div className="table-secondary">{resume.candidateId}</div>
        </div>
      ),
    },
    {
      key: "uploaded",
      header: "Resume uploaded",
      render: (resume) => formatDate(resume.uploadedDate),
    },
    {
      key: "file",
      header: "File size",
      render: (resume) => formatFileSize(resume.fileSize),
    },
    {
      key: "score",
      header: "AI score",
      render: (resume) =>
        typeof resume.aiScore === "number" ? (
          <AppBadge tone="primary">{`${resume.aiScore}%`}</AppBadge>
        ) : (
          "Not analyzed"
        ),
    },
    {
      key: "actions",
      header: "Actions",
      align: "right",
      render: (resume) => (
        <div className="table-actions">
          <Link
            className="app-button app-button--ghost app-button--small"
            to={`/recruiter/candidates/${resume.candidateId}`}
          >
            <UserRoundSearch size={15} aria-hidden="true" />
            Profile
          </Link>
          <Link
            className="app-button app-button--ghost app-button--small"
            to={`/recruiter/ai-ranking?resumeId=${resume.id}`}
          >
            <FileSearch size={15} aria-hidden="true" />
            Analyze
          </Link>
        </div>
      ),
    },
  ];

  return (
    <div className="animate-in">
      <PageHeader
        title="Candidate search"
        description="Search candidate resume records and open professional profiles."
      />
      <div className="filter-bar">
        <AppInput
          label="Search resumes"
          value={query}
          placeholder="File content or name"
          icon={<Search size={18} aria-hidden="true" />}
          onChange={(event) => setQuery(event.target.value)}
        />
        <div className="field">
          <span className="field-label">Results</span>
          <div className="app-input">{resumes.length} resumes</div>
        </div>
      </div>
      <section className="content-panel">
        {loading ? (
          <div className="state-panel">
            <AppLoader label="Searching candidates" />
          </div>
        ) : error ? (
          <ErrorState message={error} onRetry={loadCandidates} />
        ) : (
          <DataTable
            columns={columns}
            data={resumes}
            getRowKey={(resume) => resume.id}
            emptyTitle="No candidate resumes found"
            emptyDescription="Try a different search or wait for candidates to upload resumes."
          />
        )}
      </section>
    </div>
  );
}
