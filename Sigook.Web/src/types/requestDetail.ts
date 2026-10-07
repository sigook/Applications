import type { DetailChipVariant, DetailFact, DetailKpi } from './detailPage';

export type RequestChipVariant = DetailChipVariant;

export type RequestFact = DetailFact;

export type RequestKpi = DetailKpi;

export interface RequestStatusSource {
  status: number;
  workersQuantity: number;
  workersQuantityWorking: number;
}

export interface RequestStaffingItem {
  key: string;
  title: string;
  detail?: string;
  variant: 'is-warning' | 'is-neutral';
  actionLabel?: string;
}
