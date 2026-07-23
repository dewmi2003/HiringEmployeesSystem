function Sidebar() {
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

      {/* Brand */}
      <div className="mb-10">
        <h1 className="text-2xl font-bold tracking-wide">
          TalentAI
        </h1>

        <p className="text-teal-200 text-sm">
          Recruitment Intelligence
        </p>
      </div>


      {/* Navigation */}
      <nav className="space-y-3">


        <button
          className="
          w-full
          text-left
          px-4
          py-3
          rounded-xl
          bg-teal-600
          hover:bg-teal-500
          transition
          "
        >
          Dashboard
        </button>


        <button
          className="
          w-full
          text-left
          px-4
          py-3
          rounded-xl
          hover:bg-teal-600
          transition
          "
        >
          Profile
        </button>


        <button
          className="
          w-full
          text-left
          px-4
          py-3
          rounded-xl
          hover:bg-teal-600
          transition
          "
        >
          Jobs
        </button>


        <button
          className="
          w-full
          text-left
          px-4
          py-3
          rounded-xl
          hover:bg-teal-600
          transition
          "
        >
          Applications
        </button>


      </nav>


    </aside>
  );
}

export default Sidebar;