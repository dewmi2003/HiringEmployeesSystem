function CandidateDashboard() {

  return (
    <div>

      <h1 className="text-3xl font-bold mb-6">
        Candidate Dashboard
      </h1>


      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">


        {/* Profile Card */}
        <div className="bg-white p-6 rounded-xl shadow">
          <h2 className="text-xl font-semibold">
            Profile
          </h2>

          <p>Name: Test Candidate</p>
          <p>Email: candidate@gmail.com</p>
          <p>Skills: React, Java, SQL</p>
        </div>


        {/* ATS Score */}
        <div className="bg-white p-6 rounded-xl shadow">

          <h2 className="text-xl font-semibold">
            AI Resume Score
          </h2>

          <p className="text-4xl font-bold mt-4">
            85%
          </p>

        </div>



        {/* Match Score */}
        <div className="bg-white p-6 rounded-xl shadow">

          <h2 className="text-xl font-semibold">
            Job Match
          </h2>

          <p className="text-4xl font-bold mt-4">
            92%
          </p>

        </div>


      </div>


      <div className="mt-8 bg-white p-6 rounded-xl shadow">

        <h2 className="text-xl font-semibold mb-4">
          AI Recommendations
        </h2>

        <ul>
          <li>
            Improve Python skills
          </li>

          <li>
            Add more projects to resume
          </li>

          <li>
            Complete cloud certification
          </li>
        </ul>

      </div>


    </div>
  );
}


export default CandidateDashboard;