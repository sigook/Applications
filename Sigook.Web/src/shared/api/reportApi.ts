import { api } from '@/app/security/apiService';
import type { GridParams } from '@/shared/types/common';

// Query-string values passed to the generic blob report downloader.
export type ReportQueryParams = Record<string, string | number | boolean | null | undefined | readonly (string | number)[]>;

// Generic blob report downloader (used by SigookGrid, Export.vue and Companies.vue)
export function downloadAgencyReport(url: string, filter: ReportQueryParams | GridParams): Promise<Blob> {
  return api.get<Blob>(url, { params: { ...filter }, responseType: 'blob' });
}
