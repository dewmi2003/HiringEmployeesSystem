import { useCallback, useEffect, useState } from "react";
import { zodResolver } from "@hookform/resolvers/zod";
import { Save } from "lucide-react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import AppBadge from "../../components/common/AppBadge";
import AppButton from "../../components/common/AppButton";
import AppInput from "../../components/common/AppInput";
import AppLoader from "../../components/common/AppLoader";
import AppTextarea from "../../components/common/AppTextarea";
import ErrorState from "../../components/common/ErrorState";
import PageHeader from "../../components/layout/PageHeader";
import type { CandidateProfile } from "../../models/candidate";
import { getErrorMessage } from "../../services/apiClient";
import {
  getMyCandidateProfile,
  updateCandidateProfile,
} from "../../services/candidateService.ts";

const profileSchema = z.object({
  phone: z.string().trim().max(30).nullable(),
  address: z.string().trim().max(300).nullable(),
  bio: z.string().trim().max(1200).nullable(),
  experience: z.string().trim().max(3000).nullable(),
  education: z.string().trim().max(3000).nullable(),
});

type ProfileValues = z.infer<typeof profileSchema>;

export default function CandidateProfilePage() {
  const [profile, setProfile] = useState<CandidateProfile | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const {
    register,
    reset,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<ProfileValues>({
    resolver: zodResolver(profileSchema),
    defaultValues: {
      phone: "",
      address: "",
      bio: "",
      experience: "",
      education: "",
    },
  });

  const loadProfile = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      const result = await getMyCandidateProfile();
      setProfile(result);
      reset({
        phone: result.phone || "",
        address: result.address || "",
        bio: result.bio || "",
        experience: result.experience || "",
        education: result.education || "",
      });
    } catch (loadError) {
      setError(getErrorMessage(loadError));
    } finally {
      setLoading(false);
    }
  }, [reset]);

  useEffect(() => {
    void loadProfile();
  }, [loadProfile]);

  const onSubmit = handleSubmit(async (values) => {
    if (!profile) return;
    setError("");
    setSuccess("");
    try {
      await updateCandidateProfile(profile.candidateId, values);
      setSuccess("Your profile was updated successfully.");
      await loadProfile();
    } catch (saveError) {
      setError(getErrorMessage(saveError));
    }
  });

  if (loading) {
    return (
      <div className="state-panel">
        <AppLoader label="Loading profile" />
      </div>
    );
  }

  if (!profile) {
    return <ErrorState message={error} onRetry={loadProfile} />;
  }

  return (
    <div className="animate-in">
      <PageHeader
        title="Candidate profile"
        description="Keep your professional background accurate for applications and recruiter review."
        actions={
          <AppButton
            type="submit"
            form="candidate-profile-form"
            loading={isSubmitting}
            icon={<Save size={17} aria-hidden="true" />}
          >
            Save changes
          </AppButton>
        }
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
        <form
          className="content-panel form-grid"
          id="candidate-profile-form"
          noValidate
          onSubmit={onSubmit}
        >
          <AppInput label="Full name" value={profile.fullName} disabled />
          <AppInput label="Email address" value={profile.email} disabled />
          <AppInput
            label="Phone number"
            placeholder="+94 77 123 4567"
            error={errors.phone?.message}
            {...register("phone")}
          />
          <AppInput
            label="Address"
            placeholder="City, country"
            error={errors.address?.message}
            {...register("address")}
          />
          <div className="full-width">
            <AppTextarea
              label="Professional summary"
              placeholder="Summarize your professional background and goals."
              error={errors.bio?.message}
              {...register("bio")}
            />
          </div>
          <div className="full-width">
            <AppTextarea
              label="Experience"
              placeholder="Describe relevant roles, responsibilities, and outcomes."
              error={errors.experience?.message}
              {...register("experience")}
            />
          </div>
          <div className="full-width">
            <AppTextarea
              label="Education"
              placeholder="List qualifications, institutions, and completion dates."
              error={errors.education?.message}
              {...register("education")}
            />
          </div>
        </form>

        <aside className="content-panel">
          <div className="panel-header">
            <div>
              <h2>Skills</h2>
              <p>Skills currently linked to your candidate record.</p>
            </div>
          </div>
          <div className="inline-list">
            {profile.skills.length > 0 ? (
              profile.skills.map((skill) => (
                <AppBadge key={skill} tone="primary">
                  {skill}
                </AppBadge>
              ))
            ) : (
              <p className="field-hint">No skills have been added yet.</p>
            )}
          </div>
        </aside>
      </div>
    </div>
  );
}
