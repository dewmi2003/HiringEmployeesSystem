import { 
  FiHome, 
  FiUser, 
  FiBriefcase, 
  FiFileText, 
  FiCpu, 
  FiSettings 
} from "react-icons/fi";

function Sidebar() {

  const menuItems = [
    { name: "Dashboard", icon: <FiHome /> },
    { name: "Profile", icon: <FiUser /> },
    { name: "Jobs", icon: <FiBriefcase /> },
    { name: "Applications", icon: <FiFileText /> },
    { name: "AI Insights", icon: <FiCpu /> },
    { name: "Settings", icon: <FiSettings /> },
  ];


  return (
    <aside className="w-64 min-h-screen bg-[#0F4C5C] text-white p-6 shadow-xl">

      {/* Logo */}
      <div className="mb-10">
        <h1 className="text-2xl font-bold">
          Hire<span className="text-[#D4AF37]">AI</span>
        </h1>

        <p className="text-sm text-gray-300 mt-1">
          Career Intelligence
        </p>
      </div>


      {/* Menu */}
      <nav className="space-y-3">

        {menuItems.map((item, index) => (

          <div
            key={index}
            className="
              flex items-center gap-3
              px-4 py-3
              rounded-xl
              cursor-pointer
              transition
              hover:bg-[#0D9488]
            "
          >

            <span className="text-xl">
              {item.icon}
            </span>

            <span className="text-sm font-medium">
              {item.name}
            </span>

          </div>

        ))}

      </nav>


      {/* Bottom badge */}
      <div className="mt-10 p-4 rounded-xl bg-white/10">

        <p className="text-sm">
          AI Career Assistant
        </p>

        <p className="text-xs text-gray-300 mt-2">
          Improve your profile with AI insights
        </p>

      </div>


    </aside>
  );
}

export default Sidebar;