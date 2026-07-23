import api from "./api";

export const createApplication = async (data) => {
  const response = await api.post("/applications", data);
  return response.data;
};

export const getMyApplications = async () => {
  const response = await api.get("/applications/my");
  return response.data;
};

export const getApplicationsByJob = async (jobId) => {
  const response = await api.get(`/applications/job/${jobId}`);
  return response.data;
};

export const updateApplicationStatus = async (id, data) => {
  const response = await api.patch(`/applications/${id}/status`, data);
  return response.data;
};
