import { Link, useLocation } from "react-router-dom";

function Sidebar() {
  const location = useLocation();
  const navItems = [
    { to: "/candidate/dashboard", label: "Dashboard" },
    { to: "/candidate/profile", label: "Profile" },
    { to: "/candidate/resume", label: "Resume" },
    { to: "/candidate/skills", label: "Skills" },
    { to: "/jobs", label: "Jobs" },
    { to: "/candidate/applications", label: "Applications" }
  ];

  return (
    <aside
      className="
      w-72
      min-h-screen
      bg-gradient-to-b
      from-teal-900
      to-teal-700
      text-white
      p-6
      shadow-xl
      "
    >
      <div className="mb-10">
        <h1 className="text-2xl font-bold tracking-wide">
          TalentAI
        </h1>

        <p className="text-teal-200 text-sm">
          Recruitment Intelligence
        </p>
      </div>

      <nav className="space-y-3">
        {navItems.map((item) => {
          const active = location.pathname === item.to;

          return (
            <Link
              key={item.to}
              to={item.to}
              className={`
                block
                w-full
                text-left
                px-4
                py-3
                rounded-xl
                transition
                ${active ? "bg-teal-600" : "hover:bg-teal-600"}
              `}
            >
              {item.label}
            </Link>
          );
        })}
      </nav>
    </aside>
  );
}

export default Sidebar;
