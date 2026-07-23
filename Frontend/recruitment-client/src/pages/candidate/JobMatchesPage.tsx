import { useState } from "react";
import { zodResolver } from "@hookform/resolvers/zod";
import { Sparkles } from "lucide-react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import AIInsightCard from "../../components/cards/AIInsightCard";
import AppBadge from "../../components/common/AppBadge";
import AppButton from "../../components/common/AppButton";
import AppInput from "../../components/common/AppInput";
import AppTextarea from "../../components/common/AppTextarea";
import PageHeader from "../../components/layout/PageHeader";
import type { JobMatch } from "../../models/ai";
import { matchJob } from "../../services/aiService.ts";
import { getErrorMessage } from "../../services/apiClient";

const matchSchema = z.object({
  jobTitle: z.string().trim().max(200).optional(),
  resumeText: z
    .string()
    .trim()
    .min(50, "Add enough resume content for a useful analysis."),
  jobDescription: z
    .string()
    .trim()
    .min(50, "Add the job description and requirements."),
});

type MatchValues = z.infer<typeof matchSchema>;

export default function JobMatchesPage() {
  const [result, setResult] = useState<JobMatch | null>(null);
  const [error, setError] = useState("");
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<MatchValues>({
    resolver: zodResolver(matchSchema),
    defaultValues: { jobTitle: "", resumeText: "", jobDescription: "" },
  });

  const onSubmit = handleSubmit(async (values) => {
    setError("");
    setResult(null);
    try {
      setResult(await matchJob(values));
    } catch (matchError) {
      setError(getErrorMessage(matchError));
    }
  });

  return (
    <div className="animate-in">
      <PageHeader
        title="AI job match"
        description="Compare resume evidence with a specific job before submitting an application."
        badge={<AppBadge tone="primary">GitHub Models</AppBadge>}
      />

      {error ? (
        <div className="alert alert--error" role="alert">
          {error}
        </div>
      ) : null}

      <div className="content-grid">
        <form className="content-panel form-grid" noValidate onSubmit={onSubmit}>
          <div className="full-width">
            <AppInput
              label="Job title"
              placeholder="Backend developer"
              error={errors.jobTitle?.message}
              {...register("jobTitle")}
            />
          </div>
          <div className="full-width">
            <AppTextarea
              label="Resume text"
              placeholder="Paste the relevant text from your resume."
              error={errors.resumeText?.message}
              {...register("resumeText")}
            />
          </div>
          <div className="full-width">
            <AppTextarea
              label="Job description"
              placeholder="Paste the job description and requirements."
              error={errors.jobDescription?.message}
              {...register("jobDescription")}
            />
          </div>
          <div className="full-width form-actions">
            <AppButton
              type="submit"
              loading={isSubmitting}
              icon={<Sparkles size={17} aria-hidden="true" />}
            >
              Generate match
            </AppButton>
          </div>
        </form>

        {result ? (
          <AIInsightCard
            title={result.jobTitle || "Job match"}
            score={result.matchPercentage}
            summary={result.rationale}
          >
            <div className="stack">
              <div>
                <strong>Matched skills</strong>
                <div className="inline-list">
                  {result.matchedSkills.map((skill) => (
                    <AppBadge key={skill} tone="success">
                      {skill}
                    </AppBadge>
                  ))}
                </div>
              </div>
              <div>
                <strong>Missing skills</strong>
                <div className="inline-list">
                  {result.missingSkills.map((skill) => (
                    <AppBadge key={skill} tone="warning">
                      {skill}
                    </AppBadge>
                  ))}
                </div>
              </div>
              <p>
                <strong>Recommendation:</strong> {result.recommendation}
              </p>
            </div>
          </AIInsightCard>
        ) : (
          <AIInsightCard
            title="Role-specific analysis"
            summary="Your score, matched skills, missing requirements, and recommendation will appear after analysis."
          />
        )}
      </div>
    </div>
  );
}
