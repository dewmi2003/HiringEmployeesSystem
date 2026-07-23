import { FiSearch, FiBell, FiUser } from "react-icons/fi";

function Navbar() {
  return (
    <nav className="h-20 bg-white shadow-sm flex items-center justify-between px-8">

      {/* Brand */}
      <div>
        <h1 className="text-2xl font-bold text-[#0F4C5C]">
          Hire<span className="text-[#D4AF37]">AI</span>
        </h1>

        <p className="text-xs text-gray-500">
          Career Intelligence Platform
        </p>
      </div>


      {/* Search */}
      <div className="flex items-center bg-gray-100 rounded-xl px-4 py-2 w-96">

        <FiSearch className="text-gray-500 mr-3" />

        <input
          type="text"
          placeholder="Search opportunities..."
          className="bg-transparent outline-none w-full text-sm"
        />

      </div>


      {/* Right Section */}
      <div className="flex items-center gap-6">


        {/* Notification */}
        <button className="relative text-[#0F4C5C]">

          <FiBell size={22}/>

          <span className="
            absolute 
            -top-1 
            -right-1
            bg-[#D4AF37]
            w-3
            h-3
            rounded-full
          ">
          </span>

        </button>



        {/* Profile */}
        <div className="flex items-center gap-3">

          <div className="
            w-10 h-10 
            rounded-full 
            bg-[#0F4C5C]
            flex items-center justify-center
            text-white
          ">
            <FiUser />
          </div>


          <div>
            <p className="text-sm font-semibold">
              Sineli
            </p>

            <p className="text-xs text-gray-500">
              Candidate
            </p>
          </div>

        </div>


      </div>

    </nav>
  );
}

export default Navbar;