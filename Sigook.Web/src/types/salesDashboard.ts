import type { InteractionType } from './companyInteraction';
import type { DealStatus } from './deal';

export type SalesRangeKey = 'week' | 'month' | 'quarter';

export type SalesCreateKind = 'interaction' | 'client' | 'deal';

export interface SalesAgent {
  readonly fullName: string;
  readonly initials: string;
}

export interface SalesPeriod {
  readonly label: string;
  readonly asOf: string;
}

export interface SalesClient {
  readonly id: string;
  readonly name: string;
  readonly industry: string;
}

export interface SalesClientsBlock {
  readonly items: readonly SalesClient[];
  readonly totalItems: number;
  readonly activeCount: number;
  readonly newThisMonth: number;
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
  readonly agent: SalesAgent;
  readonly period: SalesPeriod;
  readonly clients: SalesClientsBlock;
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
