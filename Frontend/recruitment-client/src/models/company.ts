export interface Company {
  id: string;
  name: string;
  description: string | null;
  website: string | null;
  address: string | null;
}

export type CompanyCreateRequest = Omit<Company, "id">;
