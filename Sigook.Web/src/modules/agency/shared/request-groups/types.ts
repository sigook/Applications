import type { GridParams } from '@/shared/types/common';

export interface RequestGroupFilter extends GridParams {
  name?: string;
  numberId?: number | null;
  jobTitle?: string;
  companyProfileId?: string | null;
  statuses?: number[];
  onlyMine?: boolean;
  startAtFrom?: string | null;
  startAtTo?: string | null;
}

export interface RequestGroupSummary {
  requestId: string;
  numberId: number;
  companyFullName: string;
  jobTitle: string;
  city?: string | null;
  provinceName?: string | null;
  displayShift?: string | null;
  startAt?: string | null;
  isAsap: boolean;
  isDirectHiring: boolean;
}

export interface RequestGroupFill {
  value: number;
  total: number;
  label: string;
}

export interface SkippedItem {
  id: string;
  reason: string;
}
