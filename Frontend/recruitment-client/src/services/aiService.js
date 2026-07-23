import api from "./api";

export const analyzeResume = async (data) => {
  const response = await api.post("/ai/resume-analysis", data);
  return response.data;
};

export const analyzeStoredResume = async (resumeId, jobId) => {
  const response = await api.get(`/ai/resumes/${resumeId}/analysis`, {
    params: jobId ? { jobId } : {}
  });
  return response.data;
};

export const matchJob = async (data) => {
  const response = await api.post("/ai/job-match", data);
  return response.data;
};

export const rankCandidates = async (data) => {
  const response = await api.post("/ai/candidate-ranking", data);
  return response.data;
};

export const rankJobApplicants = async (jobId) => {
  const response = await api.get(`/ai/jobs/${jobId}/candidate-ranking`);
  return response.data;
};

export const generateInterviewQuestions = async (data) => {
  const response = await api.post("/ai/interview-questions", data);
  return response.data;
};
