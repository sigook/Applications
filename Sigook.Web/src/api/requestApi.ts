import http from '@/security/apiService';
import type { RequestShiftModel } from '@/types/agency';

export function fetchRequestShift(requestId: string): Promise<RequestShiftModel> {
  return http.get(`/api/Request/${requestId}/Shift`).then(r => r.data);
}
