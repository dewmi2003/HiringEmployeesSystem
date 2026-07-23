import { useEffect, useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { zodResolver } from "@hookform/resolvers/zod";
import { LockKeyhole, Mail } from "lucide-react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import AppButton from "../../components/common/AppButton";
import AppInput from "../../components/common/AppInput";
import { useAuth } from "../../hooks/useAuth";
import { getErrorMessage } from "../../services/apiClient";
import { getRoleHome, routes } from "../../utils/routePaths";

const loginSchema = z.object({
  email: z.string().trim().email("Enter a valid email address."),
  password: z.string().min(1, "Password is required."),
});

type LoginValues = z.infer<typeof loginSchema>;

interface LocationState {
  from?: string;
}

export default function LoginPage() {
  const [serverError, setServerError] = useState("");
  const { signIn, isAuthenticated, user } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<LoginValues>({
    resolver: zodResolver(loginSchema),
    defaultValues: { email: "", password: "" },
  });

  useEffect(() => {
    if (isAuthenticated && user) {
      navigate(getRoleHome(user.role), { replace: true });
    }
  }, [isAuthenticated, navigate, user]);

  const onSubmit = handleSubmit(async (values) => {
    setServerError("");
    try {
      const authenticatedUser = await signIn(values);
      const requestedPath = (location.state as LocationState | null)?.from;
      navigate(requestedPath || getRoleHome(authenticatedUser.role), {
        replace: true,
      });
    } catch (error) {
      setServerError(
        getErrorMessage(
          error,
          "Login failed. Check your email and password.",
        ),
      );
    }
  });

  return (
    <div className="auth-form-wrap animate-in">
      <h2>Welcome back</h2>
      <p>Sign in to continue to your recruitment workspace.</p>

      {serverError ? (
        <div className="alert alert--error" role="alert">
          {serverError}
        </div>
      ) : null}

      <form className="auth-form" noValidate onSubmit={onSubmit}>
        <AppInput
          label="Email address"
          type="email"
          autoComplete="email"
          placeholder="you@company.com"
          icon={<Mail size={18} aria-hidden="true" />}
          error={errors.email?.message}
          {...register("email")}
        />
        <AppInput
          label="Password"
          type="password"
          autoComplete="current-password"
          placeholder="Enter your password"
          icon={<LockKeyhole size={18} aria-hidden="true" />}
          error={errors.password?.message}
          {...register("password")}
        />
        <AppButton
          className="auth-submit"
          type="submit"
          loading={isSubmitting}
        >
          Sign in
        </AppButton>
      </form>

      <p className="auth-switch">
        New to TalentAI? <Link to={routes.register}>Create an account</Link>
      </p>

      {import.meta.env.DEV ? (
        <div className="demo-credentials">
          Local demo: <strong>admin@local</strong>,{" "}
          <strong>demo.recruiter001@local</strong>, or{" "}
          <strong>demo.candidate001@local</strong>. Password:{" "}
          <strong>P@ssw0rd!</strong>
        </div>
      ) : null}
    </div>
  );
}
