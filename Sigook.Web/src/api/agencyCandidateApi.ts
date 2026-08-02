import { api } from '@/security/apiService';
import type { PaginatedList } from '@/types/common';
import type {
  Candidate,
  CandidateDocument,
  CreateCandidateDocumentPayload,
  AgencyCandidateFilter,
  CreateCandidateModel,
  CandidatePhoneNumberModel,
  CandidateSkillModel,
} from '@/types/candidate';

// ---------------------------------------------------------------------------
// Candidates CRUD
// ---------------------------------------------------------------------------

export function getAgencyCandidates(filter: AgencyCandidateFilter): Promise<PaginatedList<Candidate>> {
  return api.get<PaginatedList<Candidate>>('/api/agency/candidates', { params: { ...filter } });
}

export function getAgencyCandidate(candidateId: string): Promise<Candidate> {
  return api.get<Candidate>(`/api/agency/candidates/${candidateId}`);
}

export function createAgencyCandidate(model: CreateCandidateModel): Promise<{ id: string }> {
  return api.post<{ id: string }>('/api/agency/candidates', model);
}

export function updateAgencyCandidate(candidateId: string, model: CreateCandidateModel): Promise<void> {
  return api.put(`/api/agency/candidates/${candidateId}`, model);
}

export function deleteAgencyCandidate(candidateId: string): Promise<void> {
  return api.del(`/api/agency/candidates/${candidateId}`);
}

export function updateAgencyCandidateRecruiter(candidateId: string): Promise<void> {
  return api.put(`/api/agency/candidates/${candidateId}/Recruiter`, null);
}

export function convertCandidateToWorker(candidateId: string): Promise<{ id: string }> {
  return api.post<{ id: string }>(`/api/agency/candidates/${candidateId}/convert-to-worker`);
}

// ---------------------------------------------------------------------------
// Phone numbers
// ---------------------------------------------------------------------------

export function addCandidatePhoneNumber(candidateId: string, model: CandidatePhoneNumberModel): Promise<{ id: string }> {
  return api.post<{ id: string }>(`/api/agency/candidates/${candidateId}/PhoneNumbers`, model);
}

export function deleteCandidatePhoneNumber(candidateId: string, numberId: string): Promise<void> {
  return api.del(`/api/agency/candidates/${candidateId}/PhoneNumbers/${numberId}`);
}

// ---------------------------------------------------------------------------
// Skills
// ---------------------------------------------------------------------------

export function addCandidateSkill(candidateId: string, model: CandidateSkillModel): Promise<{ id: string }> {
  return api.post<{ id: string }>(`/api/agency/candidates/${candidateId}/Skills`, model);
}

export function deleteCandidateSkill(candidateId: string, skillId: string): Promise<void> {
  return api.del(`/api/agency/candidates/${candidateId}/Skills/${skillId}`);
}

// ---------------------------------------------------------------------------
// Documents
// ---------------------------------------------------------------------------

export function getCandidateDocuments(candidateId: string): Promise<PaginatedList<CandidateDocument>> {
  return api.get<PaginatedList<CandidateDocument>>(`/api/agency/candidates/${candidateId}/Documents`);
}

export function addCandidateDocument(candidateId: string, model: CreateCandidateDocumentPayload): Promise<CandidateDocument> {
  return api.post<CandidateDocument>(`/api/agency/candidates/${candidateId}/Documents`, model);
}

export function deleteCandidateDocument(candidateId: string, id: string): Promise<void> {
  return api.del(`/api/agency/candidates/${candidateId}/Documents/${id}`);
}

// ---------------------------------------------------------------------------
// Bulk upload
// ---------------------------------------------------------------------------

export function bulkAgencyCandidates(agencyId: string, file: File): Promise<Blob> {
  const formData = new FormData();
  formData.append('file', file);
  return api.post<Blob>(`/api/agency/candidates/bulk/${agencyId}`, formData, {
    responseType: 'blob',
    headers: { 'Content-Type': 'multipart/form-data' },
  });
}
