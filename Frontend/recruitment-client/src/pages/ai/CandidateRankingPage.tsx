import { useEffect, useState } from "react";
import { Sparkles } from "lucide-react";
import AppBadge from "../../components/common/AppBadge";
import AppButton from "../../components/common/AppButton";
import AppSelect from "../../components/common/AppSelect";
import PageHeader from "../../components/layout/PageHeader";
import DataTable, {
  type TableColumn,
} from "../../components/tables/DataTable";
import type { CandidateRank, CandidateRanking } from "../../models/ai";
import type { Job } from "../../models/job";
import { rankJobApplicants } from "../../services/aiService.ts";
import { getErrorMessage } from "../../services/apiClient";
import { getJobs } from "../../services/jobService.ts";

export default function CandidateRankingPage() {
  const [jobs, setJobs] = useState<Job[]>([]);
  const [jobId, setJobId] = useState("");
  const [ranking, setRanking] = useState<CandidateRanking | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    getJobs()
      .then(setJobs)
      .catch((loadError) => setError(getErrorMessage(loadError)));
  }, []);

  const generateRanking = async () => {
    if (!jobId) return;
    setLoading(true);
    setError("");
    setRanking(null);
    try {
      setRanking(await rankJobApplicants(jobId));
    } catch (rankingError) {
      setError(getErrorMessage(rankingError));
    } finally {
      setLoading(false);
    }
  };

  const columns: TableColumn<CandidateRank>[] = [
    {
      key: "rank",
      header: "Rank",
      render: (candidate) => (
        <span className="table-primary">#{candidate.rank}</span>
      ),
    },
    {
      key: "candidate",
      header: "Candidate",
      render: (candidate) => (
        <div>
          <div className="table-primary">{candidate.candidateName}</div>
          <div className="table-secondary">{candidate.recommendation}</div>
        </div>
      ),
    },
    {
      key: "score",
      header: "Match score",
      render: (candidate) => (
        <AppBadge tone="primary">{`${candidate.score}%`}</AppBadge>
      ),
    },
    {
      key: "matched",
      header: "Matched skills",
      render: (candidate) =>
        candidate.matchedSkills.slice(0, 3).join(", ") || "None identified",
    },
    {
      key: "missing",
      header: "Missing skills",
      render: (candidate) =>
        candidate.missingSkills.slice(0, 3).join(", ") || "None identified",
    },
  ];

  return (
    <div className="animate-in">
      <PageHeader
        title="AI resume ranking"
        description="Compare applicants against a selected vacancy using evidence from their resumes."
        badge={<AppBadge tone="primary">AI-assisted</AppBadge>}
      />

      {error ? (
        <div className="alert alert--error" role="alert">
          {error}
        </div>
      ) : null}

      <div className="filter-bar">
        <AppSelect
          label="Job vacancy"
          value={jobId}
          options={jobs.map((job) => ({
            label: `${job.title} · ${job.companyName}`,
            value: job.jobId,
          }))}
          onChange={(event) => setJobId(event.target.value)}
        />
        <div className="field">
          <span className="field-label">Analysis</span>
          <AppButton
            loading={loading}
            disabled={!jobId}
            icon={<Sparkles size={17} aria-hidden="true" />}
            onClick={() => void generateRanking()}
          >
            Rank candidates
          </AppButton>
        </div>
      </div>

      <section className="content-panel">
        <div className="panel-header">
          <div>
            <h2>{ranking?.jobTitle || "Candidate ranking"}</h2>
            <p>
              {ranking
                ? `${ranking.candidates.length} applicants analyzed`
                : "Select a vacancy to generate an evidence-based comparison."}
            </p>
          </div>
        </div>
        <DataTable
          columns={columns}
          data={ranking?.candidates || []}
          getRowKey={(candidate) =>
            candidate.candidateId || `${candidate.rank}-${candidate.candidateName}`
          }
          emptyTitle="No ranking generated"
          emptyDescription="Choose a job and run the ranking when applications are available."
        />
        {ranking ? (
          <p className="ai-disclaimer">
            AI-generated insight. Review each candidate and supporting evidence
            before making a hiring decision.
          </p>
        ) : null}
      </section>
    </div>
  );
}
