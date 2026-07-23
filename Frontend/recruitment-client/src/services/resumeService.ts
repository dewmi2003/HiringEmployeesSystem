import type { Resume } from "../models/resume";
import apiClient from "./apiClient";

export async function uploadResume(
  file: File,
  candidateId?: string,
  onProgress?: (percentage: number) => void,
): Promise<Resume> {
  const formData = new FormData();
  formData.append("file", file);

  if (candidateId) {
    formData.append("candidateId", candidateId);
  }

  const { data } = await apiClient.post<Resume>(
    "/resumes/upload",
    formData,
    {
      onUploadProgress: (event) => {
        if (event.total && onProgress) {
          onProgress(Math.round((event.loaded * 100) / event.total));
        }
      },
    },
  );

  return data;
}

export async function getResumeHistory(
  candidateId: string,
): Promise<Resume[]> {
  const { data } = await apiClient.get<Resume[]>(
    `/resumes/candidate/${candidateId}/history`,
  );
  return data;
}

export async function deleteResume(resumeId: string): Promise<void> {
  await apiClient.delete(`/resumes/${resumeId}/soft`);
}

export async function searchResumes(searchTerm = ""): Promise<Resume[]> {
  const { data } = await apiClient.get<Resume[]>("/resumes/search", {
    params: { searchTerm },
  });
  return data;
}
