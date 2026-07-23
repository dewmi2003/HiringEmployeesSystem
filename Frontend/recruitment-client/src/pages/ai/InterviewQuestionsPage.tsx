import { useState } from "react";
import { zodResolver } from "@hookform/resolvers/zod";
import { MessageSquareText, Sparkles } from "lucide-react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import AppBadge from "../../components/common/AppBadge";
import AppButton from "../../components/common/AppButton";
import AppInput from "../../components/common/AppInput";
import AppTextarea from "../../components/common/AppTextarea";
import EmptyState from "../../components/common/EmptyState";
import PageHeader from "../../components/layout/PageHeader";
import type { InterviewQuestions } from "../../models/ai";
import { generateInterviewQuestions } from "../../services/aiService.ts";
import { getErrorMessage } from "../../services/apiClient";

const questionsSchema = z.object({
  jobDescription: z
    .string()
    .trim()
    .min(40, "Add the job description and requirements."),
  candidateSummary: z
    .string()
    .trim()
    .min(30, "Add a candidate summary or resume evidence."),
  count: z.number().int().min(1).max(20),
});

type QuestionValues = z.infer<typeof questionsSchema>;

export default function InterviewQuestionsPage() {
  const [result, setResult] = useState<InterviewQuestions | null>(null);
  const [error, setError] = useState("");
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<QuestionValues>({
    resolver: zodResolver(questionsSchema),
    defaultValues: {
      jobDescription: "",
      candidateSummary: "",
      count: 8,
    },
  });

  const onSubmit = handleSubmit(async (values) => {
    setError("");
    setResult(null);
    try {
      setResult(await generateInterviewQuestions(values));
    } catch (questionError) {
      setError(getErrorMessage(questionError));
    }
  });

  return (
    <div className="animate-in">
      <PageHeader
        title="AI interview questions"
        description="Generate structured questions grounded in the vacancy and candidate evidence."
        badge={<AppBadge tone="primary">AI-assisted</AppBadge>}
      />

      {error ? (
        <div className="alert alert--error" role="alert">
          {error}
        </div>
      ) : null}

      <div className="content-grid">
        <form className="content-panel form-grid" noValidate onSubmit={onSubmit}>
          <div className="full-width">
            <AppTextarea
              label="Job description"
              placeholder="Paste the role responsibilities and requirements."
              error={errors.jobDescription?.message}
              {...register("jobDescription")}
            />
          </div>
          <div className="full-width">
            <AppTextarea
              label="Candidate summary"
              placeholder="Paste relevant resume evidence or a candidate summary."
              error={errors.candidateSummary?.message}
              {...register("candidateSummary")}
            />
          </div>
          <AppInput
            label="Number of questions"
            type="number"
            min="1"
            max="20"
            error={errors.count?.message}
            {...register("count", { valueAsNumber: true })}
          />
          <div className="field">
            <span className="field-label">Generate</span>
            <AppButton
              type="submit"
              loading={isSubmitting}
              icon={<Sparkles size={17} aria-hidden="true" />}
            >
              Create questions
            </AppButton>
          </div>
        </form>

        <section className="content-panel">
          <div className="panel-header">
            <div>
              <h2>Question set</h2>
              <p>Use professional judgment when selecting final questions.</p>
            </div>
          </div>
          {result ? (
            <div className="stack">
              <div className="inline-list">
                {result.focusAreas.map((area) => (
                  <AppBadge key={area} tone="primary">
                    {area}
                  </AppBadge>
                ))}
              </div>
              <ol className="question-list">
                {result.questions.map((question) => (
                  <li key={question}>{question}</li>
                ))}
              </ol>
              <p className="ai-disclaimer">
                AI-generated insight. Check relevance, fairness, and legal
                suitability before use.
              </p>
            </div>
          ) : (
            <EmptyState
              title="No questions generated"
              description="Add role and candidate context to create a focused interview set."
              icon={MessageSquareText}
            />
          )}
        </section>
      </div>
    </div>
  );
}
