export interface CandidateProfile {
  candidateId: string;
  userId: string;
  fullName: string;
  email: string;
  phone: string | null;
  address: string | null;
  bio: string | null;
  experience: string | null;
  education: string | null;
  skills: string[];
}

export type CandidateProfileUpdate = Pick<
  CandidateProfile,
  "phone" | "address" | "bio" | "experience" | "education"
>;
