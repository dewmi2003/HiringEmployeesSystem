import {
  useCallback,
  useEffect,
  useRef,
  useState,
  type DragEvent,
} from "react";
import { FileText, Sparkles, Trash2, UploadCloud } from "lucide-react";
import { Link } from "react-router-dom";
import AppBadge from "../../components/common/AppBadge";
import AppButton from "../../components/common/AppButton";
import AppLoader from "../../components/common/AppLoader";
import EmptyState from "../../components/common/EmptyState";
import ErrorState from "../../components/common/ErrorState";
import PageHeader from "../../components/layout/PageHeader";
import DataTable, {
  type TableColumn,
} from "../../components/tables/DataTable";
import type { Resume } from "../../models/resume";
import { getErrorMessage } from "../../services/apiClient";
import { getMyCandidateProfile } from "../../services/candidateService.ts";
import {
  deleteResume,
  getResumeHistory,
  uploadResume,
} from "../../services/resumeService";
import {
  formatDateTime,
  formatFileSize,
} from "../../utils/formatters";
import { routes } from "../../utils/routePaths";

const allowedExtensions = [".pdf", ".doc", ".docx", ".txt"];
const maxFileSize = 5 * 1024 * 1024;

function validateFile(file: File): string {
  const extension = `.${file.name.split(".").pop()?.toLowerCase() || ""}`;
  if (!allowedExtensions.includes(extension)) {
    return "Choose a PDF, DOC, DOCX, or TXT file.";
  }
  if (file.size > maxFileSize) {
    return "The file must be 5 MB or smaller.";
  }
  return "";
}

export default function ResumePage() {
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [candidateId, setCandidateId] = useState("");
  const [resumes, setResumes] = useState<Resume[]>([]);
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [dragActive, setDragActive] = useState(false);
  const [progress, setProgress] = useState(0);
  const [loading, setLoading] = useState(true);
  const [uploading, setUploading] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const loadResumes = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      const profile = await getMyCandidateProfile();
      setCandidateId(profile.candidateId);
      setResumes(await getResumeHistory(profile.candidateId));
    } catch (loadError) {
      setError(getErrorMessage(loadError));
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    void loadResumes();
  }, [loadResumes]);

  const chooseFile = (file?: File) => {
    if (!file) return;
    const validationMessage = validateFile(file);
    setError(validationMessage);
    setSuccess("");
    setSelectedFile(validationMessage ? null : file);
  };

  const handleDrop = (event: DragEvent<HTMLLabelElement>) => {
    event.preventDefault();
    setDragActive(false);
    chooseFile(event.dataTransfer.files[0]);
  };

  const upload = async () => {
    if (!selectedFile) return;
    setUploading(true);
    setProgress(0);
    setError("");
    setSuccess("");
    try {
      await uploadResume(selectedFile, undefined, setProgress);
      setSuccess("Resume uploaded successfully.");
      setSelectedFile(null);
      if (fileInputRef.current) fileInputRef.current.value = "";
      await loadResumes();
    } catch (uploadError) {
      setError(getErrorMessage(uploadError));
    } finally {
      setUploading(false);
    }
  };

  const removeResume = async (resumeId: string) => {
    setError("");
    try {
      await deleteResume(resumeId);
      await loadResumes();
    } catch (deleteError) {
      setError(getErrorMessage(deleteError));
    }
  };

  const columns: TableColumn<Resume>[] = [
    {
      key: "file",
      header: "File",
      render: (resume) => (
        <div>
          <div className="table-primary">{resume.fileName}</div>
          <div className="table-secondary">
            {formatFileSize(resume.fileSize)} · Version {resume.version}
          </div>
        </div>
      ),
    },
    {
      key: "uploaded",
      header: "Uploaded",
      render: (resume) => formatDateTime(resume.uploadedDate),
    },
    {
      key: "status",
      header: "Status",
      render: (resume) => (
        <AppBadge tone={resume.isActive ? "success" : "neutral"}>
          {resume.isActive ? "Active" : "Previous"}
        </AppBadge>
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
            to={`/candidate/ai-job-matches?resumeId=${resume.id}`}
          >
            <Sparkles size={15} aria-hidden="true" />
            Analyze
          </Link>
          <AppButton
            variant="ghost"
            size="small"
            icon={<Trash2 size={15} aria-hidden="true" />}
            onClick={() => void removeResume(resume.id)}
          >
            Delete
          </AppButton>
        </div>
      ),
    },
  ];

  return (
    <div className="animate-in">
      <PageHeader
        title="Resumes"
        description="Upload and manage the documents used for applications and AI analysis."
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
        <section className="content-panel">
          <label
            className={`dropzone${dragActive ? " dropzone--active" : ""}`}
            onDragEnter={(event) => {
              event.preventDefault();
              setDragActive(true);
            }}
            onDragOver={(event) => event.preventDefault()}
            onDragLeave={() => setDragActive(false)}
            onDrop={handleDrop}
          >
            <input
              ref={fileInputRef}
              type="file"
              accept=".pdf,.doc,.docx,.txt"
              onChange={(event) => chooseFile(event.target.files?.[0])}
            />
            <div>
              <span className="dropzone-icon">
                <UploadCloud size={25} aria-hidden="true" />
              </span>
              <strong>
                {selectedFile
                  ? selectedFile.name
                  : "Drop your resume here or browse"}
              </strong>
              <p>PDF, DOC, DOCX, or TXT up to 5 MB</p>
            </div>
          </label>

          {selectedFile ? (
            <div className="stack upload-summary">
              <div className="list-row">
                <span>
                  {selectedFile.name} · {formatFileSize(selectedFile.size)}
                </span>
                <AppButton
                  loading={uploading}
                  icon={<UploadCloud size={17} aria-hidden="true" />}
                  onClick={() => void upload()}
                >
                  Upload resume
                </AppButton>
              </div>
              {uploading ? (
                <progress
                  className="score-progress"
                  value={progress}
                  max={100}
                  aria-label="Resume upload progress"
                />
              ) : null}
            </div>
          ) : null}
        </section>

        <aside className="ai-insight">
          <span className="ai-label">
            <Sparkles size={15} aria-hidden="true" />
            AI-ready resume
          </span>
          <h2>Build an ATS-friendly application</h2>
          <p>
            Use role-specific analysis to review matched skills, missing
            keywords, and resume quality.
          </p>
          <Link
            className="app-button app-button--secondary"
            to={routes.candidate.jobMatches}
          >
            Open AI analysis
          </Link>
          <p className="ai-disclaimer">
            Review generated insights before updating your resume.
          </p>
        </aside>
      </div>

      <section className="content-panel section-gap">
        <div className="panel-header">
          <div>
            <h2>Resume history</h2>
            <p>Your active resume and previous uploaded versions.</p>
          </div>
        </div>
        {loading ? (
          <div className="state-panel">
            <AppLoader label="Loading resume history" />
          </div>
        ) : error && !candidateId ? (
          <ErrorState message={error} onRetry={loadResumes} />
        ) : resumes.length === 0 ? (
          <EmptyState
            title="No resume uploaded"
            description="Your uploaded resume versions will appear here."
            icon={FileText}
          />
        ) : (
          <DataTable
            columns={columns}
            data={resumes}
            getRowKey={(resume) => resume.id}
          />
        )}
      </section>
    </div>
  );
}
