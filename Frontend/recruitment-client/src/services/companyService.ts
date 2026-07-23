import type { Company, CompanyCreateRequest } from "../models/company";
import apiClient from "./apiClient";

export async function getCompanies(): Promise<Company[]> {
  const { data } = await apiClient.get<Company[]>("/company");
  return data;
}

export async function createCompany(
  request: CompanyCreateRequest,
): Promise<Company> {
  const { data } = await apiClient.post<Company>("/company", request);
  return data;
}

export async function deleteCompany(companyId: string): Promise<void> {
  await apiClient.delete(`/company/${companyId}`);
}
