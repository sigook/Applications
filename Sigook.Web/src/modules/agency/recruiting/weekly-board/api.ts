import { api } from '@/app/security/apiService';
import type {
  WeeklyBoard,
  RecruiterWeeklyBoard,
  WeeklyBoardRunner,
  WeeklyBoardFilter,
  AssignRecruitersPayload,
  UnassignRecruiterPayload,
  MoveAssignmentPayload,
  AddRunnerPayload,
} from '@/modules/agency/recruiting/weekly-board/types';

const baseUrl = '/api/agency/recruiting/weeklyboard';

export function getWeeklyBoard(filter: WeeklyBoardFilter): Promise<WeeklyBoard> {
  return api.get<WeeklyBoard>(baseUrl, { params: { ...filter } });
}

export function getRecruiterWeeklyBoard(filter: WeeklyBoardFilter): Promise<RecruiterWeeklyBoard> {
  return api.get<RecruiterWeeklyBoard>(`${baseUrl}/mine`, { params: { ...filter } });
}

export function getRequestRunners(requestId: string): Promise<WeeklyBoardRunner[]> {
  return api.get<WeeklyBoardRunner[]>(`${baseUrl}/${requestId}/runners`);
}

export function addRunner(payload: AddRunnerPayload): Promise<void> {
  return api.post(`${baseUrl}/runner`, payload);
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
