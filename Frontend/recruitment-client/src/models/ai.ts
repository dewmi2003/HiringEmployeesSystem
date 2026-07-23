export interface ResumeAnalysis {
  candidateName: string;
  atsScore: number;
  matchScore: number;
  summary: string;
  matchedSkills: string[];
  missingSkills: string[];
  strengths: string[];
  improvementSuggestions: string[];
  atsIssues: string[];
  keywords: string[];
  usedAiProvider: boolean;
  aiProviderResult: string;
  generatedAt: string;
}

export interface JobMatch {
  jobTitle: string;
  matchPercentage: number;
  matchedSkills: string[];
  missingSkills: string[];
  experienceFit: string;
  educationFit: string;
  recommendation: string;
  rationale: string;
  suggestedInterviewFocus: string[];
  usedAiProvider: boolean;
  aiProviderResult: string;
  generatedAt: string;
}

export interface CandidateRank {
  rank: number;
  candidateId: string | null;
  candidateName: string;
  score: number;
  matchedSkills: string[];
  missingSkills: string[];
  rationale: string;
  recommendation: string;
}

export interface CandidateRanking {
  jobId: string | null;
  jobTitle: string;
  candidates: CandidateRank[];
  usedAiProvider: boolean;
  aiProviderResult: string;
  generatedAt: string;
}

export interface InterviewQuestions {
  questions: string[];
  focusAreas: string[];
  usedAiProvider: boolean;
  aiProviderResult: string;
  generatedAt: string;
}
