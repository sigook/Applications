import { api } from '@/app/security/apiService';
import type {
  AgencyRunnersFilter,
  AgencyRunnersPagedResponse,
  ChangeRunnersStatusModel,
  ChangeRunnersStatusResult,
} from '@/modules/agency/recruiting/runners/types';

const baseUrl = '/api/agency/recruiting/runners';

export function getAgencyRunners(filter: AgencyRunnersFilter): Promise<AgencyRunnersPagedResponse> {
  return api.get<AgencyRunnersPagedResponse>(baseUrl, { params: { ...filter } });
}

export function changeAgencyRunnersStatus(model: ChangeRunnersStatusModel): Promise<ChangeRunnersStatusResult> {
  return api.put<ChangeRunnersStatusResult>(`${baseUrl}/Status`, model);
}
