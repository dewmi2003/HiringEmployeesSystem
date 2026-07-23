import api from "./api";

export const getMyCandidateProfile = async () => {
  const response = await api.get("/candidates/me");
  return response.data;
};

export const getCandidateById = async (id) => {
  const response = await api.get(`/candidates/${id}`);
  return response.data;
};

export const updateCandidateProfile = async (id, data) => {
  const response = await api.put(`/candidates/${id}`, data);
  return response.data;
};
