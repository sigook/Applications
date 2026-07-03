import { api } from '@/security/apiService';
import type { RequestShiftModel } from '@/types/agency';

export function fetchRequestShift(requestId: string): Promise<RequestShiftModel> {
  return api.get<RequestShiftModel>(`/api/Request/${requestId}/Shift`);
}
