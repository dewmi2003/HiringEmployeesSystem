import type { LucideIcon } from "lucide-react";
import {
  Activity,
  Bell,
  Bot,
  BriefcaseBusiness,
  Building2,
  CalendarDays,
  ChartNoAxesCombined,
  CircleUserRound,
  ClipboardList,
  FileSearch,
  FileText,
  Gauge,
  ListChecks,
  MessageSquareText,
  Search,
  Settings,
  ShieldCheck,
  Sparkles,
  UserRoundSearch,
  Users,
} from "lucide-react";
import type { UserRole } from "../../models/auth";
import { routes } from "../../utils/routePaths";

export interface NavigationItem {
  label: string;
  to: string;
  icon: LucideIcon;
}

interface NavigationSection {
  label: string;
  items: NavigationItem[];
}

const candidateNavigation: NavigationSection[] = [
  {
    label: "Workspace",
    items: [
      {
        label: "Dashboard",
        to: routes.candidate.dashboard,
        icon: Gauge,
      },
      { label: "Find jobs", to: routes.candidate.jobs, icon: Search },
      {
        label: "AI job matches",
        to: routes.candidate.jobMatches,
        icon: Sparkles,
      },
      {
        label: "My applications",
        to: routes.candidate.applications,
        icon: ClipboardList,
      },
      { label: "Resumes", to: routes.candidate.resumes, icon: FileText },
      {
        label: "Interviews",
        to: routes.candidate.interviews,
        icon: CalendarDays,
      },
    ],
  },
  {
    label: "Account",
    items: [
      {
        label: "Notifications",
        to: routes.candidate.notifications,
        icon: Bell,
      },
      { label: "Profile", to: routes.candidate.profile, icon: CircleUserRound },
      { label: "Settings", to: "/candidate/settings", icon: Settings },
    ],
  },
];

const recruiterNavigation: NavigationSection[] = [
  {
    label: "Recruitment",
    items: [
      {
        label: "Dashboard",
        to: routes.recruiter.dashboard,
        icon: Gauge,
      },
      { label: "Jobs", to: routes.recruiter.jobs, icon: BriefcaseBusiness },
      {
        label: "Applications",
        to: routes.recruiter.applications,
        icon: ClipboardList,
      },
      {
        label: "Candidates",
        to: routes.recruiter.candidates,
        icon: UserRoundSearch,
      },
      {
        label: "Interviews",
        to: routes.recruiter.interviews,
        icon: CalendarDays,
      },
    ],
  },
  {
    label: "Intelligence",
    items: [
      {
        label: "AI resume ranking",
        to: routes.recruiter.ranking,
        icon: FileSearch,
      },
      {
        label: "AI questions",
        to: routes.recruiter.questions,
        icon: MessageSquareText,
      },
      {
        label: "Reports",
        to: routes.recruiter.reports,
        icon: ChartNoAxesCombined,
      },
      { label: "Notifications", to: "/notifications", icon: Bell },
      { label: "Settings", to: "/recruiter/settings", icon: Settings },
    ],
  },
];

const adminNavigation: NavigationSection[] = [
  {
    label: "Platform",
    items: [
      { label: "Dashboard", to: routes.admin.dashboard, icon: Gauge },
      { label: "Users", to: routes.admin.users, icon: Users },
      { label: "Companies", to: routes.admin.companies, icon: Building2 },
      { label: "Jobs", to: routes.admin.jobs, icon: BriefcaseBusiness },
      {
        label: "Applications",
        to: routes.admin.applications,
        icon: ClipboardList,
      },
      {
        label: "Interviews",
        to: routes.admin.interviews,
        icon: CalendarDays,
      },
    ],
  },
  {
    label: "Governance",
    items: [
      { label: "Reports", to: routes.admin.reports, icon: ChartNoAxesCombined },
      { label: "AI usage", to: routes.admin.aiUsage, icon: Bot },
      { label: "Audit logs", to: routes.admin.auditLogs, icon: Activity },
      { label: "Roles", to: "/admin/roles", icon: ShieldCheck },
      { label: "Notifications", to: "/notifications", icon: Bell },
      { label: "Settings", to: "/admin/settings", icon: Settings },
    ],
  },
];

export function getNavigation(role: UserRole): NavigationSection[] {
  if (role === "Admin") return adminNavigation;
  if (role === "Recruiter") return recruiterNavigation;
  return candidateNavigation;
}

export const allNavigationItems = [
  ...candidateNavigation,
  ...recruiterNavigation,
  ...adminNavigation,
].flatMap((section) => section.items);

export const moduleIcons = {
  applications: ListChecks,
};
