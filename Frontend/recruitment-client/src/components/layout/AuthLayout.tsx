import { Outlet } from "react-router-dom";
import { BrainCircuit, CheckCircle2 } from "lucide-react";

export default function AuthLayout() {
  return (
    <main className="auth-shell">
      <section className="auth-brand-panel" aria-label="TalentAI introduction">
        <div className="auth-brand-lockup">
          <span className="brand-mark">
            <BrainCircuit size={22} aria-hidden="true" />
          </span>
          <div>
            <p className="brand-name">TalentAI</p>
            <p className="brand-caption">Recruitment intelligence</p>
          </div>
        </div>

        <div className="auth-brand-content">
          <h1>Make every hiring decision clearer.</h1>
          <p>
            One secure workspace for candidates, recruiters, interviews,
            applications, and responsible AI-assisted evaluation.
          </p>
          <div className="auth-benefits">
            {[
              "ATS-friendly resume analysis",
              "Structured recruitment workflows",
              "Role-based access and reporting",
            ].map((benefit) => (
              <div className="auth-benefit" key={benefit}>
                <CheckCircle2 size={18} aria-hidden="true" />
                <span>{benefit}</span>
              </div>
            ))}
          </div>
        </div>

        <p className="auth-brand-footer">
          AI-generated insights support, but do not replace, human decisions.
        </p>
      </section>
      <section className="auth-form-panel">
        <Outlet />
      </section>
    </main>
  );
}
