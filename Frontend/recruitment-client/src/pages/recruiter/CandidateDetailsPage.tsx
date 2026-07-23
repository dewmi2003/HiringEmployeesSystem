import { useCallback, useEffect, useState } from "react";
import { FileText } from "lucide-react";
import { useParams } from "react-router-dom";
import AppBadge from "../../components/common/AppBadge";
import AppLoader from "../../components/common/AppLoader";
import EmptyState from "../../components/common/EmptyState";
import ErrorState from "../../components/common/ErrorState";
import PageHeader from "../../components/layout/PageHeader";
import type { CandidateProfile } from "../../models/candidate";
import type { Resume } from "../../models/resume";
import { getErrorMessage } from "../../services/apiClient";
import { getCandidateProfile } from "../../services/candidateService.ts";
import { getResumeHistory } from "../../services/resumeService";
import {
  formatDate,
  formatFileSize,
} from "../../utils/formatters";

export default function CandidateDetailsPage() {
  const { candidateId } = useParams();
  const [profile, setProfile] = useState<CandidateProfile | null>(null);
  const [resumes, setResumes] = useState<Resume[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const loadCandidate = useCallback(async () => {
    if (!candidateId) return;
    setLoading(true);
    setError("");
    try {
      const [profileResult, resumeResult] = await Promise.all([
        getCandidateProfile(candidateId),
        getResumeHistory(candidateId),
      ]);
      setProfile(profileResult);
      setResumes(resumeResult);
    } catch (loadError) {
      setError(getErrorMessage(loadError));
    } finally {
      setLoading(false);
    }
  }, [candidateId]);

  useEffect(() => {
    void loadCandidate();
  }, [loadCandidate]);

  if (loading) {
    return (
      <div className="state-panel">
        <AppLoader label="Loading candidate profile" />
      </div>
    );
  }

  if (!profile) {
    return <ErrorState message={error} onRetry={loadCandidate} />;
  }

  return (
    <div className="animate-in">
      <PageHeader
        title={profile.fullName}
        description={profile.email}
        badge={<AppBadge tone="primary">Candidate</AppBadge>}
      />

      <div className="content-grid">
        <section className="content-panel">
          <div className="panel-header">
            <div>
              <h2>Professional profile</h2>
              <p>Candidate background supplied in their account.</p>
            </div>
          </div>
          <dl className="detail-list">
            {[
              ["Phone", profile.phone || "Not provided"],
              ["Address", profile.address || "Not provided"],
              ["Summary", profile.bio || "Not provided"],
              ["Experience", profile.experience || "Not provided"],
              ["Education", profile.education || "Not provided"],
            ].map(([label, value]) => (
              <div className="detail-row" key={label}>
                <dt>{label}</dt>
                <dd>{value}</dd>
              </div>
            ))}
          </dl>
        </section>

        <aside className="content-panel">
          <div className="panel-header">
            <div>
              <h2>Skills</h2>
              <p>Skills linked to this profile.</p>
            </div>
          </div>
          <div className="inline-list">
            {profile.skills.map((skill) => (
              <AppBadge key={skill} tone="primary">
                {skill}
              </AppBadge>
            ))}
          </div>
        </aside>
      </div>

      <section className="content-panel section-gap">
        <div className="panel-header">
          <div>
            <h2>Resume history</h2>
            <p>Uploaded candidate documents and versions.</p>
          </div>
        </div>
        {resumes.length === 0 ? (
          <EmptyState
            title="No resumes found"
            description="This candidate has not uploaded a resume."
            icon={FileText}
          />
        ) : (
          <div className="stack">
            {resumes.map((resume) => (
              <article className="list-row" key={resume.id}>
                <div>
                  <div className="table-primary">{resume.fileName}</div>
                  <div className="table-secondary">
                    {formatDate(resume.uploadedDate)} ·{" "}
                    {formatFileSize(resume.fileSize)}
                  </div>
                </div>
                <AppBadge tone={resume.isActive ? "success" : "neutral"}>
                  {resume.isActive ? "Active" : `Version ${resume.version}`}
                </AppBadge>
              </article>
            ))}
          </div>
        )}
      </section>
    </div>
  );
}
