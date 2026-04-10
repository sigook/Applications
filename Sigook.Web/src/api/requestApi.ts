import http from '@/security/apiService';
import type { Shift } from '@/types/request';

export function fetchRequestShift(requestId: string): Promise<Shift> {
  return http.get(`/api/Request/${requestId}/Shift`).then(r => r.data);
}
