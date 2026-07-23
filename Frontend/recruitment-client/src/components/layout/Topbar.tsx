import { useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import {
  Bell,
  LogOut,
  Menu,
  Moon,
  PanelLeftClose,
  PanelLeftOpen,
  Search,
  Sun,
} from "lucide-react";
import AppAvatar from "../common/AppAvatar";
import { useAuth } from "../../hooks/useAuth";
import { useTheme } from "../../hooks/useTheme";
import { allNavigationItems } from "./navigation";

interface TopbarProps {
  collapsed: boolean;
  onToggleSidebar: () => void;
  onOpenMobileMenu: () => void;
}

export default function Topbar({
  collapsed,
  onToggleSidebar,
  onOpenMobileMenu,
}: TopbarProps) {
  const [query, setQuery] = useState("");
  const { pathname } = useLocation();
  const navigate = useNavigate();
  const { user, signOut } = useAuth();
  const { theme, toggleTheme } = useTheme();

  const currentLabel =
    allNavigationItems.find((item) => pathname.startsWith(item.to))?.label ||
    "Workspace";

  const handleSearch = (event: React.FormEvent) => {
    event.preventDefault();
    const trimmedQuery = query.trim();
    if (!trimmedQuery) return;

    if (user?.role === "Recruiter") {
      navigate(`/recruiter/jobs?search=${encodeURIComponent(trimmedQuery)}`);
    } else if (user?.role === "Admin") {
      navigate(`/admin/jobs?search=${encodeURIComponent(trimmedQuery)}`);
    } else {
      navigate(`/candidate/jobs?title=${encodeURIComponent(trimmedQuery)}`);
    }
  };

  const handleSignOut = () => {
    signOut();
    navigate("/login", { replace: true });
  };

  return (
    <header className="topbar">
      <div className="topbar-left">
        <button
          className="icon-button mobile-only"
          type="button"
          aria-label="Open navigation"
          onClick={onOpenMobileMenu}
        >
          <Menu size={21} aria-hidden="true" />
        </button>
        <button
          className="icon-button desktop-only"
          type="button"
          aria-label={collapsed ? "Expand navigation" : "Collapse navigation"}
          onClick={onToggleSidebar}
        >
          {collapsed ? (
            <PanelLeftOpen size={20} aria-hidden="true" />
          ) : (
            <PanelLeftClose size={20} aria-hidden="true" />
          )}
        </button>
        <span className="desktop-only" aria-current="page">
          {currentLabel}
        </span>
        <form className="topbar-search" role="search" onSubmit={handleSearch}>
          <div className="input-wrap">
            <Search className="input-icon" size={18} aria-hidden="true" />
            <input
              className="app-input app-input--with-icon"
              type="search"
              value={query}
              placeholder="Search jobs"
              aria-label="Search jobs"
              onChange={(event) => setQuery(event.target.value)}
            />
          </div>
        </form>
      </div>

      <div className="topbar-actions">
        <button
          className="icon-button"
          type="button"
          aria-label={`Switch to ${theme === "light" ? "dark" : "light"} theme`}
          title="Change theme"
          onClick={toggleTheme}
        >
          {theme === "light" ? (
            <Moon size={19} aria-hidden="true" />
          ) : (
            <Sun size={19} aria-hidden="true" />
          )}
        </button>
        <button
          className="icon-button"
          type="button"
          aria-label="Open notifications"
          title="Notifications"
          onClick={() => navigate("/notifications")}
        >
          <Bell size={19} aria-hidden="true" />
          <span className="notification-dot" aria-hidden="true" />
        </button>
        <div className="topbar-user">
          <AppAvatar name={user?.fullName || "User"} />
          <div className="topbar-user-copy">
            <p className="topbar-user-name">{user?.fullName}</p>
            <p className="topbar-user-role">{user?.role}</p>
          </div>
        </div>
        <button
          className="icon-button"
          type="button"
          aria-label="Sign out"
          title="Sign out"
          onClick={handleSignOut}
        >
          <LogOut size={19} aria-hidden="true" />
        </button>
      </div>
    </header>
  );
}
