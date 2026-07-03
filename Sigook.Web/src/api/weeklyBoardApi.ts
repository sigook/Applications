import { api } from '@/security/apiService';
import type {
  WeeklyBoard,
  RecruiterWeeklyBoard,
  WeeklyBoardDispatch,
  WeeklyBoardFilter,
  AssignRecruitersPayload,
  UnassignRecruiterPayload,
  MoveAssignmentPayload,
  DispatchWorkersPayload,
  RemoveWorkerPayload,
} from '@/types/weeklyBoard';

const baseUrl = '/api/agency/recruiting/WeeklyBoard';

export function getWeeklyBoard(filter: WeeklyBoardFilter): Promise<WeeklyBoard> {
  return api.get<WeeklyBoard>(baseUrl, { params: { ...filter } });
}

export function getRecruiterWeeklyBoard(filter: WeeklyBoardFilter): Promise<RecruiterWeeklyBoard> {
  return api.get<RecruiterWeeklyBoard>(`${baseUrl}/mine`, { params: { ...filter } });
}

export function getRequestDispatches(requestId: string): Promise<WeeklyBoardDispatch[]> {
  return api.get<WeeklyBoardDispatch[]>(`${baseUrl}/${requestId}/dispatches`);
}

export function addWorkers(payload: DispatchWorkersPayload): Promise<void> {
  return api.post(`${baseUrl}/dispatch`, payload);
}

export function removeWorker(payload: RemoveWorkerPayload): Promise<void> {
  return api.del(`${baseUrl}/dispatch`, { params: { ...payload } });
}

export function assignRecruiters(payload: AssignRecruitersPayload): Promise<void> {
  return api.post(baseUrl, payload);
}

export function unassignRecruiter(payload: UnassignRecruiterPayload): Promise<void> {
  return api.del(baseUrl, { params: { ...payload } });
}

export function moveAssignment(payload: MoveAssignmentPayload): Promise<void> {
  return api.post(`${baseUrl}/move`, payload);
}
