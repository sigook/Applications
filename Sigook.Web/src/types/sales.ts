import type { DealStatus, InteractionType } from './company';

export type SalesRangeKey = 'week' | 'month' | 'quarter';

export type SalesCreateKind = 'interaction' | 'client' | 'deal';

export interface SalesPeriod {
  readonly label: string;
  readonly asOf: string;
}

export interface SalesDealsBlock {
  readonly totalItems: number;
  readonly pipelineValue: number;
}

export interface SalesSeriesPoint {
  readonly label: string;
  readonly value: number;
}

export interface SalesClosedSeries {
  readonly week: readonly SalesSeriesPoint[];
  readonly month: readonly SalesSeriesPoint[];
  readonly quarter: readonly SalesSeriesPoint[];
}

export interface SalesGoal {
  readonly actual: number;
  readonly target: number;
}

export interface SalesPipelineStage {
  readonly status: DealStatus;
  readonly count: number;
}

export interface SalesActivity {
  readonly type: InteractionType;
  readonly count: number;
}

export interface SalesDashboardModel {
  readonly period: SalesPeriod;
  readonly deals: SalesDealsBlock;
  readonly dealsClosed: SalesClosedSeries;
  readonly goal: SalesGoal;
  readonly pipeline: readonly SalesPipelineStage[];
  readonly activity: readonly SalesActivity[];
}

export interface SalesMeter {
  readonly label: string;
  readonly count: number;
  readonly color: string;
}

export const SALES_RANGE_TABS: readonly { readonly key: SalesRangeKey; readonly label: string }[] = [
  { key: 'week', label: 'Week' },
  { key: 'month', label: 'Month' },
  { key: 'quarter', label: 'Quarter' },
];
