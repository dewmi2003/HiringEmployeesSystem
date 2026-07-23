import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import ScoreCard from "../../components/ScoreCard";
import { getMyCandidateProfile } from "../../services/candidateService";
import { getMyApplications } from "../../services/applicationService";
import { analyzeStoredResume } from "../../services/aiService";

function CandidateDashboard() {
  const [candidate, setCandidate] = useState({
    name: "Sineli",
    role: "Frontend Developer",
    skills: ["React", "Java", "Spring Boot", "SQL"],
    resumeScore: 87,
    matchScore: 92
  });

  const [applications, setApplications] = useState([
    { company: "TechNova Solutions", role: "Frontend Developer", status: "Interview Scheduled" },
    { company: "CloudSphere", role: "React Developer", status: "Under Review" },
    { company: "AI Labs", role: "Software Engineer", status: "Applied" }
  ]);

  const [insights, setInsights] = useState([
    "Your React skills match 85% of frontend jobs.",
    "Adding cloud projects can improve your ranking.",
    "Your resume has strong technical keywords."
  ]);

  useEffect(() => {
    getMyCandidateProfile()
      .then((profile) => {
        const fullName = [profile.firstName, profile.lastName].filter(Boolean).join(" ").trim();
        setCandidate((current) => ({
          ...current,
          name: fullName || profile.fullName || current.name,
          role: profile.currentRole || profile.title || current.role,
          skills: profile.skills || current.skills,
          resumeScore: profile.resumeScore ?? profile.aiScore ?? current.resumeScore,
          matchScore: profile.matchScore ?? current.matchScore
        }));

        if (profile.resumeId) {
          return analyzeStoredResume(profile.resumeId)
            .then((analysis) => {
              setCandidate((current) => ({
                ...current,
                resumeScore: analysis.atsScore ?? current.resumeScore,
                matchScore: analysis.matchScore ?? current.matchScore
              }));

              const suggestionList = [
                analysis.summary,
                ...(analysis.improvementSuggestions || []),
                ...(analysis.atsIssues || [])
              ].filter(Boolean);

              if (suggestionList.length > 0) {
                setInsights(suggestionList.slice(0, 3));
              }
            });
        }
      })
      .catch(() => {});

    getMyApplications()
      .then((items) => {
        const mapped = (items || []).map((item) => ({
          company: item.companyName || item.jobTitle || "Company",
          role: item.jobTitle || "Role",
          status: item.status || "Applied"
        }));

        if (mapped.length > 0) {
          setApplications(mapped);
        }
      })
      .catch(() => {});
  }, []);

  return (
    <div>
      <div className="mb-8">
        <h1 className="text-4xl font-bold text-teal-900">
          Welcome back, {candidate.name}
        </h1>

        <p className="text-gray-500 mt-2">
          Track your career progress and AI-powered job opportunities.
        </p>
      </div>

      <div className="mt-6 flex flex-wrap gap-3">
        <Link to="/candidate/profile" className="bg-teal-700 text-white px-4 py-2 rounded-xl">
          Edit Profile
        </Link>
        <Link to="/candidate/resume" className="bg-teal-100 text-teal-800 px-4 py-2 rounded-xl">
          Upload Resume
        </Link>
        <Link to="/ai/resume-analysis" className="bg-gray-100 text-gray-800 px-4 py-2 rounded-xl">
          Run AI Analysis
        </Link>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mt-8">
        <div className="bg-white rounded-3xl shadow-sm p-6 border border-gray-100">
          <h2 className="text-lg font-semibold text-teal-800">
            Profile
          </h2>

          <p className="mt-4 text-gray-700">
            Role: {candidate.role}
          </p>

          <div className="mt-4">
            <p className="text-sm text-gray-500">
              Skills
            </p>

            <div className="flex flex-wrap gap-2 mt-2">
              {candidate.skills.map((skill) => (
                <span
                  key={skill}
                  className="bg-teal-50 text-teal-700 px-3 py-1 rounded-full text-sm"
                >
                  {skill}
                </span>
              ))}
            </div>
          </div>
        </div>

        <ScoreCard
          title="AI Resume Score"
          score={candidate.resumeScore}
          description="Your resume strength based on AI analysis"
        />

        <div className="bg-white rounded-3xl shadow-sm p-6 border border-gray-100">
          <h2 className="text-lg font-semibold text-teal-800">
            Job Match
          </h2>

          <p className="text-6xl font-bold text-teal-700 mt-5">
            {candidate.matchScore}%
          </p>

          <p className="text-gray-500 mt-3">
            AI matched opportunities
          </p>
        </div>
      </div>

      <div className="mt-8 bg-white rounded-3xl shadow-sm border border-gray-100 p-8">
        <h2 className="text-2xl font-semibold text-teal-900">
          AI Career Recommendations
        </h2>

        <ul className="mt-5 space-y-3 text-gray-700">
          {insights.map((insight, index) => (
            <li key={index}>{insight}</li>
          ))}
        </ul>
      </div>

      <div className="mt-8 bg-white rounded-3xl shadow-sm border border-gray-100 p-8">
        <h2 className="text-2xl font-semibold text-teal-900 mb-6">
          Recent Applications
        </h2>

        <div className="space-y-4">
          {applications.map((application, index) => (
            <div
              key={index}
              className="flex justify-between items-center bg-gray-50 rounded-2xl p-5"
            >
              <div>
                <h3 className="font-semibold text-gray-800">
                  {application.role}
                </h3>

                <p className="text-sm text-gray-500">
                  {application.company}
                </p>
              </div>

              <span className="bg-teal-100 text-teal-700 px-4 py-2 rounded-full text-sm">
                {application.status}
              </span>
            </div>
          ))}
        </div>

        <div className="mt-6 flex flex-wrap gap-3">
          <Link to="/candidate/applications" className="bg-teal-700 text-white px-4 py-2 rounded-xl">
            View All Applications
          </Link>
          <Link to="/jobs" className="bg-gray-100 text-gray-800 px-4 py-2 rounded-xl">
            Browse Jobs
          </Link>
        </div>
      </div>
    </div>
  );
}

export default CandidateDashboard;
