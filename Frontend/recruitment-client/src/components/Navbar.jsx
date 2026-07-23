import { Link } from "react-router-dom";

function Navbar() {
  return (
    <nav
      className="
      h-20
      bg-white
      border-b
      border-gray-100
      flex
      items-center
      justify-between
      px-8
      shadow-sm
      "
    >
      <div>
        <h2 className="text-2xl font-bold text-teal-900">
          AI Recruitment Platform
        </h2>

        <p className="text-sm text-gray-500">
          Intelligent Hiring Dashboard
        </p>
      </div>

      <div className="flex items-center gap-5">
        <Link
          to="/candidate/applications"
          className="
          w-10
          h-10
          rounded-full
          bg-teal-50
          text-teal-700
          hover:bg-teal-100
          transition
          flex
          items-center
          justify-center
          "
        >
          🔔
        </Link>

        <div
          className="
          flex
          items-center
          gap-3
          "
        >
          <Link
            to="/candidate/profile"
            className="
            w-11
            h-11
            rounded-full
            bg-teal-700
            text-white
            flex
            items-center
            justify-center
            font-bold
            "
          >
            S
          </Link>

          <div>
            <Link to="/candidate/profile" className="font-semibold text-gray-800">
              Sineli
            </Link>

            <p className="text-sm text-gray-500">
              Candidate
            </p>
          </div>
        </div>
      </div>
    </nav>
  );
}

export default Navbar;
