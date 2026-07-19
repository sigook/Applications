import type { InteractionType } from './companyInteraction';

export type SalesDealStage = 'Lead' | 'Qualified' | 'Proposal' | 'Negotiation' | 'Won';

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

export interface SalesDeal {
  readonly id: string;
  readonly name: string;
  readonly clientName: string;
  readonly stage: SalesDealStage;
  readonly value: number;
}

export interface SalesClientsBlock {
  readonly items: readonly SalesClient[];
  readonly totalItems: number;
  readonly activeCount: number;
  readonly newThisMonth: number;
}

export interface SalesDealsBlock {
  readonly items: readonly SalesDeal[];
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
  readonly stage: SalesDealStage;
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

export const SALES_STAGE_ORDER: readonly SalesDealStage[] = [
  'Lead',
  'Qualified',
  'Proposal',
  'Negotiation',
  'Won',
];

export const SALES_STAGE_COLORS: Record<SalesDealStage, string> = {
  Lead: '#9ad6ff',
  Qualified: '#5cc4ff',
  Proposal: '#21b7ff',
  Negotiation: '#ff9932',
  Won: '#3eb800',
};
