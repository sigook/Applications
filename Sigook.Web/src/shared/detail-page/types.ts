import type { RouteLocationRaw } from 'vue-router';

export type DetailChipVariant = 'is-success' | 'is-info' | 'is-warning' | 'is-danger' | '';

export interface DetailChip {
  label: string;
  variant?: DetailChipVariant;
}

export interface DetailKpi {
  key: string;
  label: string;
  value: string;
  hint?: string;
  progress?: number;
}

export interface DetailFact {
  label: string;
  value: string;
  to?: RouteLocationRaw;
  href?: string;
  linkLabel?: string;
  onAction?: () => void;
}

export interface DetailSection {
  id: string;
  label: string;
  hint?: string;
  count?: number;
}
