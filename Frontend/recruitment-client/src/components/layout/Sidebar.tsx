import { NavLink } from "react-router-dom";
import { BrainCircuit } from "lucide-react";
import AppAvatar from "../common/AppAvatar";
import { useAuth } from "../../hooks/useAuth";
import { getNavigation } from "./navigation";

interface SidebarProps {
  collapsed: boolean;
  mobileOpen: boolean;
  onNavigate: () => void;
}

export default function Sidebar({
  collapsed,
  mobileOpen,
  onNavigate,
}: SidebarProps) {
  const { user } = useAuth();
  const sections = getNavigation(user?.role || "Candidate");
  const classes = [
    "sidebar",
    collapsed ? "sidebar--collapsed" : "",
    mobileOpen ? "sidebar--mobile-open" : "",
  ]
    .filter(Boolean)
    .join(" ");

  return (
    <aside className={classes} aria-label="Primary navigation">
      <div className="sidebar-brand">
        <span className="brand-mark">
          <BrainCircuit size={22} aria-hidden="true" />
        </span>
        <div className="sidebar-copy">
          <p className="brand-name">TalentAI</p>
          <p className="brand-caption">Recruitment intelligence</p>
        </div>
      </div>

      <nav className="sidebar-nav">
        {sections.map((section) => (
          <div key={section.label}>
            <p className="nav-section-label">{section.label}</p>
            {section.items.map(({ label, to, icon: Icon }) => (
              <NavLink
                key={to}
                className={({ isActive }) =>
                  `nav-link${isActive ? " active" : ""}`
                }
                to={to}
                title={collapsed ? label : undefined}
                onClick={onNavigate}
              >
                <Icon size={19} aria-hidden="true" />
                <span>{label}</span>
              </NavLink>
            ))}
          </div>
        ))}
      </nav>

      <div className="sidebar-user">
        <AppAvatar name={user?.fullName || "TalentAI user"} />
        <div className="sidebar-user-copy">
          <p className="sidebar-user-name">{user?.fullName || "User"}</p>
          <p className="sidebar-user-role">{user?.role || "Account"}</p>
        </div>
      </div>
    </aside>
  );
}
