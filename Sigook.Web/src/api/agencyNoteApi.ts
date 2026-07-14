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
    `/api/AgencyCompanyProfile/${userId}/Note?PageSize=${pagination.size}&PageIndex=${pagination.page}`,
  );
}

export function createAgencyCompanyNote(userId: string, model: NoteModel): Promise<CreateNoteResponse> {
  return api.post<CreateNoteResponse>(`/api/AgencyCompanyProfile/${userId}/Note`, model);
}

export function updateAgencyCompanyNote(userId: string, id: string, model: NoteModel): Promise<void> {
  return api.put(`/api/AgencyCompanyProfile/${userId}/Note/${id}`, model);
}

export function deleteAgencyCompanyNote(userId: string, id: string): Promise<void> {
  return api.del(`/api/AgencyCompanyProfile/${userId}/Note/${id}`);
}

// ---------------------------------------------------------------------------
// Request notes (full CRUD)
// ---------------------------------------------------------------------------

export function getAgencyRequestNotes(userId: string, pagination: NotePagination): Promise<PaginatedList<NoteItem>> {
  return api.get<PaginatedList<NoteItem>>(
    `/api/AgencyRequest/${userId}/Note?PageSize=${pagination.size}&PageIndex=${pagination.page}`,
  );
}

export function createAgencyRequestNote(userId: string, model: NoteModel): Promise<CreateNoteResponse> {
  return api.post<CreateNoteResponse>(`/api/AgencyRequest/${userId}/Note`, model);
}

export function updateAgencyRequestNote(userId: string, id: string, model: NoteModel): Promise<void> {
  return api.put(`/api/AgencyRequest/${userId}/Note/${id}`, model);
}

export function deleteAgencyRequestNote(userId: string, id: string): Promise<void> {
  return api.del(`/api/AgencyRequest/${userId}/Note/${id}`);
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
    `/api/AgencyRequest/${requestId}/Worker/${userId}/Note?PageSize=${pagination.size}&PageIndex=${pagination.page}`,
  );
}

export function createAgencyRequestWorkerNote(
  requestId: string,
  userId: string,
  model: NoteModel,
): Promise<CreateNoteResponse> {
  return api.post<CreateNoteResponse>(`/api/AgencyRequest/${requestId}/Worker/${userId}/Note`, model);
}

export function updateAgencyRequestWorkerNote(
  requestId: string,
  userId: string,
  id: string,
  model: NoteModel,
): Promise<void> {
  return api.put(`/api/AgencyRequest/${requestId}/Worker/${userId}/Note/${id}`, model);
}

export function deleteAgencyRequestWorkerNote(requestId: string, userId: string, id: string): Promise<void> {
  return api.del(`/api/AgencyRequest/${requestId}/Worker/${userId}/Note/${id}`);
}
