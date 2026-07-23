import { useCallback, useEffect, useState } from "react";
import { Search, SlidersHorizontal } from "lucide-react";
import { useSearchParams } from "react-router-dom";
import JobCard from "../../components/cards/JobCard";
import AppButton from "../../components/common/AppButton";
import AppInput from "../../components/common/AppInput";
import AppLoader from "../../components/common/AppLoader";
import EmptyState from "../../components/common/EmptyState";
import ErrorState from "../../components/common/ErrorState";
import PageHeader from "../../components/layout/PageHeader";
import { useDebounce } from "../../hooks/useDebounce";
import type { Job } from "../../models/job";
import { getErrorMessage } from "../../services/apiClient";
import { getJobs } from "../../services/jobService.ts";

export default function CandidateJobsPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [title, setTitle] = useState(searchParams.get("title") || "");
  const [location, setLocation] = useState(searchParams.get("location") || "");
  const [jobs, setJobs] = useState<Job[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const debouncedTitle = useDebounce(title);
  const debouncedLocation = useDebounce(location);

  const loadJobs = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      setJobs(
        await getJobs({
          title: debouncedTitle || undefined,
          location: debouncedLocation || undefined,
        }),
      );
    } catch (loadError) {
      setError(getErrorMessage(loadError));
    } finally {
      setLoading(false);
    }
  }, [debouncedLocation, debouncedTitle]);

  useEffect(() => {
    setSearchParams(
      {
        ...(debouncedTitle ? { title: debouncedTitle } : {}),
        ...(debouncedLocation ? { location: debouncedLocation } : {}),
      },
      { replace: true },
    );
    void loadJobs();
  }, [debouncedLocation, debouncedTitle, loadJobs, setSearchParams]);

  const clearFilters = () => {
    setTitle("");
    setLocation("");
  };

  return (
    <div className="animate-in">
      <PageHeader
        title="Find jobs"
        description="Explore published vacancies and review role requirements before applying."
      />

      <div className="filter-bar">
        <AppInput
          label="Search roles"
          value={title}
          placeholder="Job title"
          icon={<Search size={18} aria-hidden="true" />}
          onChange={(event) => setTitle(event.target.value)}
        />
        <AppInput
          label="Location"
          value={location}
          placeholder="City or remote"
          onChange={(event) => setLocation(event.target.value)}
        />
        <div className="field">
          <span className="field-label">Results</span>
          <div className="app-input">{jobs.length} jobs</div>
        </div>
        <div className="field">
          <span className="field-label">Filters</span>
          <AppButton
            variant="ghost"
            icon={<SlidersHorizontal size={16} aria-hidden="true" />}
            onClick={clearFilters}
          >
            Clear
          </AppButton>
        </div>
      </div>

      {loading ? (
        <div className="state-panel">
          <AppLoader label="Searching jobs" />
        </div>
      ) : error ? (
        <ErrorState message={error} onRetry={loadJobs} />
      ) : jobs.length === 0 ? (
        <section className="content-panel">
          <EmptyState
            title="No matching jobs"
            description="Try a broader title or remove the location filter."
            icon={Search}
            actionLabel="Clear filters"
            onAction={clearFilters}
          />
        </section>
      ) : (
        <div className="job-list">
          {jobs.map((job) => (
            <JobCard
              key={job.jobId}
              job={job}
              detailsPath={`/candidate/jobs/${job.jobId}`}
            />
          ))}
        </div>
      )}
    </div>
  );
}
