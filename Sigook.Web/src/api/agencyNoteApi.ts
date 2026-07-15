import { api } from '@/security/apiService';
import type { PaginatedList } from '@/types/common';
import type { NoteModel, NoteItem, NotePagination, CreateNoteResponse } from '@/types/agency';

// ---------------------------------------------------------------------------
// Worker notes (read + create only)
// ---------------------------------------------------------------------------

export function getWorkerProfileNotes(userId: string, pagination: NotePagination): Promise<PaginatedList<NoteItem>> {
  return api.get<PaginatedList<NoteItem>>(
    `/api/AgencyWorkerProfile/${userId}/Note?PageSize=${pagination.size}&PageIndex=${pagination.page}`,
  );
}

export function createWorkerProfileNote(userId: string, model: NoteModel): Promise<CreateNoteResponse> {
  return api.post<CreateNoteResponse>(`/api/AgencyWorkerProfile/${userId}/Note`, model);
}

// ---------------------------------------------------------------------------
// Candidate notes (read, create, delete)
// ---------------------------------------------------------------------------

export function getCandidateNotes(userId: string, pagination: NotePagination): Promise<PaginatedList<NoteItem>> {
  return api.get<PaginatedList<NoteItem>>(
    `/api/AgencyCandidate/${userId}/Note?PageSize=${pagination.size}&PageIndex=${pagination.page}`,
  );
}

export function createCandidateNote(userId: string, model: NoteModel): Promise<CreateNoteResponse> {
  return api.post<CreateNoteResponse>(`/api/AgencyCandidate/${userId}/Note`, model);
}

export function deleteCandidateNote(userId: string, id: string): Promise<void> {
  return api.del(`/api/AgencyCandidate/${userId}/Note/${id}`);
}

// ---------------------------------------------------------------------------
// Company notes (full CRUD)
// ---------------------------------------------------------------------------

export function getAgencyCompanyNotes(userId: string, pagination: NotePagination): Promise<PaginatedList<NoteItem>> {
  return api.get<PaginatedList<NoteItem>>(
    `/api/agency/companyprofiles/${userId}/Notes?PageSize=${pagination.size}&PageIndex=${pagination.page}`,
  );
}

export function createAgencyCompanyNote(userId: string, model: NoteModel): Promise<CreateNoteResponse> {
  return api.post<CreateNoteResponse>(`/api/agency/companyprofiles/${userId}/Notes`, model);
}

export function updateAgencyCompanyNote(userId: string, id: string, model: NoteModel): Promise<void> {
  return api.put(`/api/agency/companyprofiles/${userId}/Notes/${id}`, model);
}

export function deleteAgencyCompanyNote(userId: string, id: string): Promise<void> {
  return api.del(`/api/agency/companyprofiles/${userId}/Notes/${id}`);
}

// ---------------------------------------------------------------------------
// Request notes (full CRUD)
// ---------------------------------------------------------------------------

export function getAgencyRequestNotes(userId: string, pagination: NotePagination): Promise<PaginatedList<NoteItem>> {
  return api.get<PaginatedList<NoteItem>>(
    `/api/agency/requests/${userId}/Notes?PageSize=${pagination.size}&PageIndex=${pagination.page}`,
  );
}

export function createAgencyRequestNote(userId: string, model: NoteModel): Promise<CreateNoteResponse> {
  return api.post<CreateNoteResponse>(`/api/agency/requests/${userId}/Notes`, model);
}

export function updateAgencyRequestNote(userId: string, id: string, model: NoteModel): Promise<void> {
  return api.put(`/api/agency/requests/${userId}/Notes/${id}`, model);
}

export function deleteAgencyRequestNote(userId: string, id: string): Promise<void> {
  return api.del(`/api/agency/requests/${userId}/Notes/${id}`);
}

// ---------------------------------------------------------------------------
// Request worker notes (full CRUD)
// ---------------------------------------------------------------------------

export function getAgencyRequestWorkerNotes(
  requestId: string,
  userId: string,
  pagination: NotePagination,
): Promise<PaginatedList<NoteItem>> {
  return api.get<PaginatedList<NoteItem>>(
    `/api/agency/requests/${requestId}/Workers/${userId}/Notes?PageSize=${pagination.size}&PageIndex=${pagination.page}`,
  );
}

export function createAgencyRequestWorkerNote(
  requestId: string,
  userId: string,
  model: NoteModel,
): Promise<CreateNoteResponse> {
  return api.post<CreateNoteResponse>(`/api/agency/requests/${requestId}/Workers/${userId}/Notes`, model);
}

export function updateAgencyRequestWorkerNote(
  requestId: string,
  userId: string,
  id: string,
  model: NoteModel,
): Promise<void> {
  return api.put(`/api/agency/requests/${requestId}/Workers/${userId}/Notes/${id}`, model);
}

export function deleteAgencyRequestWorkerNote(requestId: string, userId: string, id: string): Promise<void> {
  return api.del(`/api/agency/requests/${requestId}/Workers/${userId}/Notes/${id}`);
}
