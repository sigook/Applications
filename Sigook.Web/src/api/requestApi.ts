import http from '@/security/apiService';
import type { Shift } from '@/types/request';

export function fetchRequestShift(requestId: string): Promise<Shift> {
  return http.get(`/api/Request/${requestId}/Shift`).then(r => r.data);
}

export function fetchQRCode(text: string): Promise<Blob> {
  return http.get(`/api/QrCode/${text}`, { responseType: 'blob' }).then(r => r.data);
}
