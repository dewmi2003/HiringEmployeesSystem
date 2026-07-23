import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { zodResolver } from "@hookform/resolvers/zod";
import { LockKeyhole, Mail, UserRound } from "lucide-react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import AppButton from "../../components/common/AppButton";
import AppInput from "../../components/common/AppInput";
import AppSelect from "../../components/common/AppSelect";
import { useAuth } from "../../hooks/useAuth";
import { getErrorMessage } from "../../services/apiClient";
import { getRoleHome, routes } from "../../utils/routePaths";

const registerSchema = z
  .object({
    fullName: z.string().trim().min(2, "Enter your full name."),
    email: z.string().trim().email("Enter a valid email address."),
    role: z.enum(["Candidate", "Recruiter"], {
      message: "Choose an account type.",
    }),
    password: z
      .string()
      .min(8, "Use at least 8 characters.")
      .regex(/[A-Z]/, "Include an uppercase letter.")
      .regex(/[a-z]/, "Include a lowercase letter.")
      .regex(/[0-9]/, "Include a number."),
    confirmPassword: z.string(),
  })
  .refine((values) => values.password === values.confirmPassword, {
    path: ["confirmPassword"],
    message: "Passwords do not match.",
  });

type RegisterValues = z.infer<typeof registerSchema>;

export default function RegisterPage() {
  const [serverError, setServerError] = useState("");
  const { signUp } = useAuth();
  const navigate = useNavigate();
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<RegisterValues>({
    resolver: zodResolver(registerSchema),
    defaultValues: {
      fullName: "",
      email: "",
      role: "Candidate",
      password: "",
      confirmPassword: "",
    },
  });

  const onSubmit = handleSubmit(async (values) => {
    setServerError("");
    try {
      const user = await signUp({
        fullName: values.fullName,
        email: values.email,
        role: values.role,
        password: values.password,
      });
      navigate(getRoleHome(user.role), { replace: true });
    } catch (error) {
      setServerError(
        getErrorMessage(error, "We could not create your account."),
      );
    }
  });

  return (
    <div className="auth-form-wrap animate-in">
      <h2>Create your account</h2>
      <p>Choose your workspace and start managing recruitment securely.</p>

      {serverError ? (
        <div className="alert alert--error" role="alert">
          {serverError}
        </div>
      ) : null}

      <form className="auth-form" noValidate onSubmit={onSubmit}>
        <AppInput
          label="Full name"
          autoComplete="name"
          placeholder="Your full name"
          icon={<UserRound size={18} aria-hidden="true" />}
          error={errors.fullName?.message}
          {...register("fullName")}
        />
        <AppInput
          label="Email address"
          type="email"
          autoComplete="email"
          placeholder="you@example.com"
          icon={<Mail size={18} aria-hidden="true" />}
          error={errors.email?.message}
          {...register("email")}
        />
        <AppSelect
          label="Account type"
          options={[
            { label: "Candidate", value: "Candidate" },
            { label: "Recruiter", value: "Recruiter" },
          ]}
          error={errors.role?.message}
          {...register("role")}
        />
        <AppInput
          label="Password"
          type="password"
          autoComplete="new-password"
          placeholder="Create a strong password"
          icon={<LockKeyhole size={18} aria-hidden="true" />}
          error={errors.password?.message}
          {...register("password")}
        />
        <AppInput
          label="Confirm password"
          type="password"
          autoComplete="new-password"
          placeholder="Repeat your password"
          icon={<LockKeyhole size={18} aria-hidden="true" />}
          error={errors.confirmPassword?.message}
          {...register("confirmPassword")}
        />
        <AppButton
          className="auth-submit"
          type="submit"
          loading={isSubmitting}
        >
          Create account
        </AppButton>
      </form>

      <p className="auth-switch">
        Already have an account? <Link to={routes.login}>Sign in</Link>
      </p>
    </div>
  );
}
