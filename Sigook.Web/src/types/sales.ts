import type { DealStatus, InteractionType } from './company';

// Mirrors backend SalesPeriod (Covenant.Common/Enums/SalesPeriod.cs).
export enum SalesPeriod {
  Day = 0,
  Week = 1,
  Month = 2,
  Quarter = 3,
}

export type SalesCreateKind = 'interaction' | 'client' | 'deal';

// Mirrors backend SalesPeriodRangeModel. `from`/`to` are calendar dates in the
// business time zone (America/New_York), serialized without a UTC offset.
export interface SalesPeriodRange {
  readonly period: SalesPeriod;
  readonly from: string;
  readonly to: string;
  readonly label: string;
  readonly timeZone: string;
}

export interface DealStatusSummary {
  readonly status: DealStatus;
  readonly count: number;
  readonly totalValue: number;
}

export interface InteractionTypeSummary {
  readonly type: InteractionType;
  readonly count: number;
}

// GET /api/agency/sales/dashboard/deals-by-status
export interface DealsByStatusFilter {
  period: SalesPeriod;
  statuses?: DealStatus[];
  ownerId?: string | null;
}

export interface DealsByStatusModel {
  readonly period: SalesPeriodRange;
  readonly totalCount: number;
  readonly totalValue: number;
  readonly items: readonly DealStatusSummary[];
}

// GET /api/agency/sales/dashboard/summary
export interface SalesDashboardSummary {
  readonly asOf: string;
  readonly quarter: SalesPeriodRange;
  readonly week: SalesPeriodRange;
  readonly pipeline: readonly DealStatusSummary[];
  readonly activity: readonly InteractionTypeSummary[];
}

export interface SalesBarPoint {
  readonly key: string;
  readonly label: string;
  readonly value: number;
  readonly color: string;
  readonly caption?: string;
}

export interface SalesMeter {
  readonly label: string;
  readonly count: number;
  readonly color: string;
}

export const SALES_PERIOD_TABS: readonly { readonly key: SalesPeriod; readonly label: string }[] = [
  { key: SalesPeriod.Day, label: 'Today' },
  { key: SalesPeriod.Week, label: 'This week' },
  { key: SalesPeriod.Month, label: 'This month' },
];
