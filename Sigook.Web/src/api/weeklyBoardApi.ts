import http from '@/security/apiService';
import type {
  WeeklyBoard,
  RecruiterWeeklyBoard,
  WeeklyBoardFilter,
  AssignRecruitersPayload,
  UnassignRecruiterPayload,
  MoveAssignmentPayload,
  DispatchWorkersPayload,
  RemoveWorkerPayload,
} from '@/types/weeklyBoard';

const baseUrl = '/api/agency/recruiting/WeeklyBoard';

export function getWeeklyBoard(filter: WeeklyBoardFilter): Promise<WeeklyBoard> {
  return http.get(baseUrl, { params: { ...filter } }).then(r => r.data);
}

export function getRecruiterWeeklyBoard(filter: WeeklyBoardFilter): Promise<RecruiterWeeklyBoard> {
  return http.get(`${baseUrl}/mine`, { params: { ...filter } }).then(r => r.data);
}

export function addWorkers(payload: DispatchWorkersPayload): Promise<void> {
  return http.post(`${baseUrl}/dispatch`, payload).then(() => {});
}

export function removeWorker(payload: RemoveWorkerPayload): Promise<void> {
  return http.delete(`${baseUrl}/dispatch`, { params: { ...payload } }).then(() => {});
}

export function assignRecruiters(payload: AssignRecruitersPayload): Promise<void> {
  return http.post(baseUrl, payload).then(() => {});
}

export function unassignRecruiter(payload: UnassignRecruiterPayload): Promise<void> {
  return http.delete(baseUrl, { params: { ...payload } }).then(() => {});
}

export function moveAssignment(payload: MoveAssignmentPayload): Promise<void> {
  return http.post(`${baseUrl}/move`, payload).then(() => {});
}
