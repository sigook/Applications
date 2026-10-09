import { api } from '@/app/security/apiService';
import type { PaginatedList } from '@/shared/types/common';
import type { NoteModel, NoteItem, NotePagination, CreateNoteResponse } from '@/modules/agency/shared/notes/types';

// ---------------------------------------------------------------------------
// Worker notes (read + create only)
// ---------------------------------------------------------------------------

export function getWorkerProfileNotes(userId: string, pagination: NotePagination): Promise<PaginatedList<NoteItem>> {
  return api.get<PaginatedList<NoteItem>>(
    `/api/agency/recruiting/workers/${userId}/Notes?PageSize=${pagination.size}&PageIndex=${pagination.page}`,
  );
}

export function createWorkerProfileNote(userId: string, model: NoteModel): Promise<CreateNoteResponse> {
  return api.post<CreateNoteResponse>(`/api/agency/recruiting/workers/${userId}/Notes`, model);
}

// ---------------------------------------------------------------------------
// Candidate notes (read, create, delete)
// ---------------------------------------------------------------------------

export function getCandidateNotes(userId: string, pagination: NotePagination): Promise<PaginatedList<NoteItem>> {
  return api.get<PaginatedList<NoteItem>>(
    `/api/agency/recruiting/candidates/${userId}/Notes?PageSize=${pagination.size}&PageIndex=${pagination.page}`,
  );
}

export function createCandidateNote(userId: string, model: NoteModel): Promise<CreateNoteResponse> {
  return api.post<CreateNoteResponse>(`/api/agency/recruiting/candidates/${userId}/Notes`, model);
}

export function deleteCandidateNote(userId: string, id: string): Promise<void> {
  return api.del(`/api/agency/recruiting/candidates/${userId}/Notes/${id}`);
}

// ---------------------------------------------------------------------------
// Company notes (full CRUD)
// ---------------------------------------------------------------------------

export function getAgencyCompanyNotes(userId: string, pagination: NotePagination): Promise<PaginatedList<NoteItem>> {
  return api.get<PaginatedList<NoteItem>>(
    `/api/agency/recruiting/clients/${userId}/Notes?PageSize=${pagination.size}&PageIndex=${pagination.page}`,
  );
}

export function createAgencyCompanyNote(userId: string, model: NoteModel): Promise<CreateNoteResponse> {
  return api.post<CreateNoteResponse>(`/api/agency/recruiting/clients/${userId}/Notes`, model);
}

export function updateAgencyCompanyNote(userId: string, id: string, model: NoteModel): Promise<void> {
  return api.put(`/api/agency/recruiting/clients/${userId}/Notes/${id}`, model);
}

export function deleteAgencyCompanyNote(userId: string, id: string): Promise<void> {
  return api.del(`/api/agency/recruiting/clients/${userId}/Notes/${id}`);
}

// ---------------------------------------------------------------------------
// Request notes (full CRUD)
// ---------------------------------------------------------------------------

export function getAgencyRequestNotes(userId: string, pagination: NotePagination): Promise<PaginatedList<NoteItem>> {
  return api.get<PaginatedList<NoteItem>>(
    `/api/agency/recruiting/requests/${userId}/Notes?PageSize=${pagination.size}&PageIndex=${pagination.page}`,
  );
}

export function createAgencyRequestNote(userId: string, model: NoteModel): Promise<CreateNoteResponse> {
  return api.post<CreateNoteResponse>(`/api/agency/recruiting/requests/${userId}/Notes`, model);
}

export function updateAgencyRequestNote(userId: string, id: string, model: NoteModel): Promise<void> {
  return api.put(`/api/agency/recruiting/requests/${userId}/Notes/${id}`, model);
}

export function deleteAgencyRequestNote(userId: string, id: string): Promise<void> {
  return api.del(`/api/agency/recruiting/requests/${userId}/Notes/${id}`);
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
    `/api/agency/recruiting/requests/${requestId}/Workers/${userId}/Notes?PageSize=${pagination.size}&PageIndex=${pagination.page}`,
  );
}

export function createAgencyRequestWorkerNote(
  requestId: string,
  userId: string,
  model: NoteModel,
): Promise<CreateNoteResponse> {
  return api.post<CreateNoteResponse>(`/api/agency/recruiting/requests/${requestId}/Workers/${userId}/Notes`, model);
}

export function updateAgencyRequestWorkerNote(
  requestId: string,
  userId: string,
  id: string,
  model: NoteModel,
): Promise<void> {
  return api.put(`/api/agency/recruiting/requests/${requestId}/Workers/${userId}/Notes/${id}`, model);
}

export function deleteAgencyRequestWorkerNote(requestId: string, userId: string, id: string): Promise<void> {
  return api.del(`/api/agency/recruiting/requests/${requestId}/Workers/${userId}/Notes/${id}`);
}
