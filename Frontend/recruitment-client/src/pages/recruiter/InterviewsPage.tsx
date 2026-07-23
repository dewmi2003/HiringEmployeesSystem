import { useCallback, useEffect, useState } from "react";
import { CalendarPlus, ExternalLink } from "lucide-react";
import { useSearchParams } from "react-router-dom";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { z } from "zod";
import AppBadge from "../../components/common/AppBadge";
import AppButton from "../../components/common/AppButton";
import AppInput from "../../components/common/AppInput";
import AppLoader from "../../components/common/AppLoader";
import AppModal from "../../components/common/AppModal";
import ErrorState from "../../components/common/ErrorState";
import PageHeader from "../../components/layout/PageHeader";
import DataTable, {
  type TableColumn,
} from "../../components/tables/DataTable";
import type { Interview } from "../../models/interview";
import { getErrorMessage } from "../../services/apiClient";
import {
  getInterviews,
  scheduleInterview,
} from "../../services/interviewService";
import { formatDateTime } from "../../utils/formatters";

const interviewSchema = z.object({
  applicationId: z.string().uuid("Enter a valid application ID."),
  interviewDate: z.string().min(1, "Choose an interview date and time."),
  location: z.string().trim().max(300).nullable(),
});

type InterviewValues = z.infer<typeof interviewSchema>;

export default function InterviewsPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const initialApplicationId = searchParams.get("applicationId") || "";
  const [interviews, setInterviews] = useState<Interview[]>([]);
  const [scheduledResult, setScheduledResult] = useState<Interview | null>(null);
  const [modalOpen, setModalOpen] = useState(Boolean(initialApplicationId));
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const {
    register,
    reset,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<InterviewValues>({
    resolver: zodResolver(interviewSchema),
    defaultValues: {
      applicationId: initialApplicationId,
      interviewDate: "",
      location: "",
    },
  });

  const loadInterviews = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      setInterviews(await getInterviews());
    } catch (loadError) {
      setError(getErrorMessage(loadError));
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    void loadInterviews();
  }, [loadInterviews]);

  const closeModal = () => {
    setModalOpen(false);
    setSearchParams({}, { replace: true });
  };

  const onSubmit = handleSubmit(async (values) => {
    setError("");
    try {
      const result = await scheduleInterview({
        applicationId: values.applicationId,
        interviewDate: new Date(values.interviewDate).toISOString(),
        location: values.location || null,
      });
      setScheduledResult(result);
      reset({ applicationId: "", interviewDate: "", location: "" });
      closeModal();
      await loadInterviews();
    } catch (scheduleError) {
      setError(getErrorMessage(scheduleError));
    }
  });

  const columns: TableColumn<Interview>[] = [
    {
      key: "candidate",
      header: "Candidate",
      render: (interview) => (
        <div>
          <div className="table-primary">{interview.candidateFullName}</div>
          <div className="table-secondary">{interview.jobTitle}</div>
        </div>
      ),
    },
    {
      key: "date",
      header: "Date and time",
      render: (interview) => formatDateTime(interview.interviewDate),
    },
    {
      key: "location",
      header: "Location",
      render: (interview) => interview.location || "Not set",
    },
    {
      key: "status",
      header: "Status",
      render: (interview) => <AppBadge>{interview.interviewStatus}</AppBadge>,
    },
  ];

  return (
    <div className="animate-in">
      <PageHeader
        title="Interviews"
        description="Coordinate candidate interviews and connected calendar invitations."
        actions={
          <AppButton
            icon={<CalendarPlus size={17} aria-hidden="true" />}
            onClick={() => setModalOpen(true)}
          >
            Schedule interview
          </AppButton>
        }
      />

      {error ? (
        <div className="alert alert--error" role="alert">
          {error}
        </div>
      ) : null}

      {scheduledResult ? (
        <section className="content-panel result-banner">
          <div className="panel-header">
            <div>
              <h2>Interview scheduled</h2>
              <p>
                {scheduledResult.candidateFullName} ·{" "}
                {formatDateTime(scheduledResult.interviewDate)}
              </p>
            </div>
            {scheduledResult.calendarResult?.startsWith("http") ? (
              <a
                className="app-button app-button--secondary"
                href={scheduledResult.calendarResult}
                target="_blank"
                rel="noreferrer"
              >
                <ExternalLink size={16} aria-hidden="true" />
                Open calendar
              </a>
            ) : null}
          </div>
          {scheduledResult.emailResult ? (
            <p className="field-hint">{scheduledResult.emailResult}</p>
          ) : null}
        </section>
      ) : null}

      <section className="content-panel">
        {loading ? (
          <div className="state-panel">
            <AppLoader label="Loading interviews" />
          </div>
        ) : error && interviews.length === 0 ? (
          <ErrorState message={error} onRetry={loadInterviews} />
        ) : (
          <DataTable
            columns={columns}
            data={interviews}
            getRowKey={(interview) => interview.interviewId}
            emptyTitle="No interviews scheduled"
            emptyDescription="Scheduled candidate interviews will appear here."
          />
        )}
      </section>

      <AppModal
        open={modalOpen}
        title="Schedule interview"
        onClose={closeModal}
        footer={
          <>
            <AppButton variant="secondary" onClick={closeModal}>
              Cancel
            </AppButton>
            <AppButton
              type="submit"
              form="schedule-interview-form"
              loading={isSubmitting}
            >
              Schedule
            </AppButton>
          </>
        }
      >
        <form
          className="stack"
          id="schedule-interview-form"
          noValidate
          onSubmit={onSubmit}
        >
          <AppInput
            label="Application ID"
            placeholder="Application GUID"
            error={errors.applicationId?.message}
            {...register("applicationId")}
          />
          <AppInput
            label="Interview date and time"
            type="datetime-local"
            error={errors.interviewDate?.message}
            {...register("interviewDate")}
          />
          <AppInput
            label="Location or meeting link"
            placeholder="Colombo office or meeting URL"
            error={errors.location?.message}
            {...register("location")}
          />
        </form>
      </AppModal>
    </div>
  );
}
