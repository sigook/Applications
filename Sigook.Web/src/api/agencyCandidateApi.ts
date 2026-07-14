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
  return api.get<PaginatedList<Candidate>>('/api/AgencyCandidate', { params: { ...filter } });
}

export function getAgencyCandidate(candidateId: string): Promise<Candidate> {
  return api.get<Candidate>(`/api/AgencyCandidate/${candidateId}`);
}

export function createAgencyCandidate(model: CreateCandidateModel): Promise<{ id: string }> {
  return api.post<{ id: string }>('/api/AgencyCandidate', model);
}

export function updateAgencyCandidate(candidateId: string, model: CreateCandidateModel): Promise<void> {
  return api.put(`/api/AgencyCandidate/${candidateId}`, model);
}

export function deleteAgencyCandidate(candidateId: string): Promise<void> {
  return api.del(`/api/AgencyCandidate/${candidateId}`);
}

export function updateAgencyCandidateRecruiter(candidateId: string): Promise<void> {
  return api.put(`/api/AgencyCandidate/${candidateId}/Recruiter`, null);
}

export function convertCandidateToWorker(candidateId: string): Promise<{ id: string }> {
  return api.post<{ id: string }>(`/api/AgencyCandidate/${candidateId}/convert-to-worker`);
}

// ---------------------------------------------------------------------------
// Phone numbers
// ---------------------------------------------------------------------------

export function addCandidatePhoneNumber(candidateId: string, model: CandidatePhoneNumberModel): Promise<{ id: string }> {
  return api.post<{ id: string }>(`/api/AgencyCandidate/${candidateId}/PhoneNumber`, model);
}

export function deleteCandidatePhoneNumber(candidateId: string, numberId: string): Promise<void> {
  return api.del(`/api/AgencyCandidate/${candidateId}/PhoneNumber/${numberId}`);
}

// ---------------------------------------------------------------------------
// Skills
// ---------------------------------------------------------------------------

export function addCandidateSkill(candidateId: string, model: CandidateSkillModel): Promise<{ id: string }> {
  return api.post<{ id: string }>(`/api/AgencyCandidate/${candidateId}/Skill`, model);
}

export function deleteCandidateSkill(candidateId: string, skillId: string): Promise<void> {
  return api.del(`/api/AgencyCandidate/${candidateId}/Skill/${skillId}`);
}

// ---------------------------------------------------------------------------
// Documents
// ---------------------------------------------------------------------------

export function getCandidateDocuments(candidateId: string): Promise<PaginatedList<CandidateDocument>> {
  return api.get<PaginatedList<CandidateDocument>>(`/api/AgencyCandidate/${candidateId}/Document`);
}

export function addCandidateDocument(candidateId: string, model: CreateCandidateDocumentPayload): Promise<CandidateDocument> {
  return api.post<CandidateDocument>(`/api/AgencyCandidate/${candidateId}/Document`, model);
}

export function deleteCandidateDocument(candidateId: string, id: string): Promise<void> {
  return api.del(`/api/AgencyCandidate/${candidateId}/Document/${id}`);
}

// ---------------------------------------------------------------------------
// Bulk upload
// ---------------------------------------------------------------------------

export function bulkAgencyCandidates(agencyId: string, file: File): Promise<Blob> {
  const formData = new FormData();
  formData.append('file', file);
  return api.post<Blob>(`/api/AgencyCandidate/bulk/${agencyId}`, formData, {
    responseType: 'blob',
    headers: { 'Content-Type': 'multipart/form-data' },
  });
}
