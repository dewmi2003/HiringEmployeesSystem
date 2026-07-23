export interface Resume {
  id: string;
  candidateId: string;
  fileName: string;
  filePath: string;
  uploadedDate: string;
  fileSize: number;
  fileType: string;
  parsedText?: string;
  aiScore?: number;
  version: number;
  isActive: boolean;
  isDeleted: boolean;
}
