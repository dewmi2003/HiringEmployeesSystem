import Navbar from "../components/Navbar";
import Sidebar from "../components/Sidebar";
import Footer from "../components/Footer";

function MainLayout({ children }) {
  return (
    <div className="
      min-h-screen
      bg-[#F7FAF9]
      flex
      flex-col
    ">

      {/* Top Navbar */}
      <Navbar />


      <div className="flex flex-1">


        {/* Sidebar */}
        <Sidebar />


        {/* Main Dashboard Area */}
        <main
          className="
          flex-1
          p-8
          overflow-y-auto
          "
        >

          <div
            className="
            bg-white
            rounded-3xl
            shadow-sm
            p-8
            min-h-full
            "
          >

            {children}

          </div>

        </main>


      </div>


      <Footer />

    </div>
  );
}

export default MainLayout;