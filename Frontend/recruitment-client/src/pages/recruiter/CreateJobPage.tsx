import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { zodResolver } from "@hookform/resolvers/zod";
import { Save } from "lucide-react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import AppButton from "../../components/common/AppButton";
import AppInput from "../../components/common/AppInput";
import AppSelect from "../../components/common/AppSelect";
import AppTextarea from "../../components/common/AppTextarea";
import PageHeader from "../../components/layout/PageHeader";
import type { Company } from "../../models/company";
import { getErrorMessage } from "../../services/apiClient";
import { getCompanies } from "../../services/companyService";
import { createJob } from "../../services/jobService.ts";
import { routes } from "../../utils/routePaths";

const jobSchema = z.object({
  companyId: z.string().min(1, "Choose a company."),
  title: z.string().trim().min(3, "Enter a job title.").max(200),
  location: z.string().trim().max(200).nullable(),
  salary: z.string().optional(),
  description: z
    .string()
    .trim()
    .min(30, "Add a clear role description.")
    .max(8000),
  requirements: z
    .string()
    .trim()
    .min(20, "Add the main role requirements.")
    .max(8000),
});

type JobFormValues = z.infer<typeof jobSchema>;

export default function CreateJobPage() {
  const [companies, setCompanies] = useState<Company[]>([]);
  const [error, setError] = useState("");
  const navigate = useNavigate();
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<JobFormValues>({
    resolver: zodResolver(jobSchema),
    defaultValues: {
      companyId: "",
      title: "",
      location: "",
      salary: "",
      description: "",
      requirements: "",
    },
  });

  useEffect(() => {
    getCompanies()
      .then(setCompanies)
      .catch((loadError) => setError(getErrorMessage(loadError)));
  }, []);

  const onSubmit = handleSubmit(async (values) => {
    setError("");
    try {
      await createJob({
        companyId: values.companyId,
        title: values.title,
        location: values.location || null,
        salary: values.salary ? Number(values.salary) : null,
        description: values.description,
        requirements: values.requirements,
      });
      navigate(routes.recruiter.jobs);
    } catch (createError) {
      setError(getErrorMessage(createError));
    }
  });

  return (
    <div className="animate-in">
      <PageHeader
        title="Create job"
        description="Publish a clear vacancy for candidates and AI-assisted matching."
        actions={
          <AppButton
            form="create-job-form"
            type="submit"
            loading={isSubmitting}
            icon={<Save size={17} aria-hidden="true" />}
          >
            Create job
          </AppButton>
        }
      />

      {error ? (
        <div className="alert alert--error" role="alert">
          {error}
        </div>
      ) : null}

      <form
        className="content-panel form-grid"
        id="create-job-form"
        noValidate
        onSubmit={onSubmit}
      >
        <AppSelect
          label="Company"
          options={companies.map((company) => ({
            label: company.name,
            value: company.id,
          }))}
          error={errors.companyId?.message}
          {...register("companyId")}
        />
        <AppInput
          label="Job title"
          placeholder="Senior backend developer"
          error={errors.title?.message}
          {...register("title")}
        />
        <AppInput
          label="Location"
          placeholder="Colombo or Remote"
          error={errors.location?.message}
          {...register("location")}
        />
        <AppInput
          label="Salary"
          type="number"
          min="0"
          step="100"
          placeholder="Optional"
          error={errors.salary?.message}
          {...register("salary")}
        />
        <div className="full-width">
          <AppTextarea
            label="Description"
            placeholder="Describe responsibilities, team context, and outcomes."
            error={errors.description?.message}
            {...register("description")}
          />
        </div>
        <div className="full-width">
          <AppTextarea
            label="Requirements"
            placeholder="List required skills, experience, and qualifications."
            error={errors.requirements?.message}
            {...register("requirements")}
          />
        </div>
      </form>
    </div>
  );
}
