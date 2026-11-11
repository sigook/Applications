<template>
  <detail-header :title="`${numberId} · ${title}`" :meta="subtitle" :org-name="orgName" :org-link="orgLink"
    :logo="logo" :logo-icon="logoIcon" :status-label="statusLabel" :status-variant="statusVariant" :chips="headerChips"
    :kpis="headerKpis">
    <template v-for="(_, name) in $slots" #[name]="scope">
      <slot :name="name" v-bind="scope ?? {}" />
    </template>
    <template v-if="$slots['quantity-actions']" #kpi-workers-extra>
      <slot name="quantity-actions" />
    </template>
  </detail-header>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import type { RouteLocationRaw } from 'vue-router';
import type { DetailChip, DetailKpi } from '@/shared/detail-page/types';
import type { RequestChipVariant, RequestKpi } from '@/shared/request-detail/types';
import DetailHeader from '@/shared/detail-page/DetailHeader.vue';

const { isAsap, isDirectHiring, chips, workersTotal, kpis, workersWorking } = defineProps<{
  numberId: number;
  title: string;
  subtitle?: string;
  orgName?: string;
  orgLink?: RouteLocationRaw;
  logo?: string | null;
  logoIcon?: string;
  statusLabel?: string;
  statusVariant?: RequestChipVariant;
  isAsap?: boolean;
  isDirectHiring?: boolean;
  chips?: string[];
  workersWorking?: number;
  workersTotal?: number;
  kpis: RequestKpi[];
}>();

const headerChips = computed<DetailChip[]>(() => [
  ...(isAsap ? [{ label: 'ASAP', variant: 'is-warning' as const }] : []),
  ...(isDirectHiring ? [{ label: 'Direct hiring', variant: 'is-info' as const }] : []),
  ...(chips ?? []).map((label) => ({ label })),
]);

const headerKpis = computed<DetailKpi[]>(() => {
  if (workersTotal === undefined) return kpis;
  const isSpots = workersWorking === undefined;
  const workers: DetailKpi = {
    key: 'workers',
    label: isSpots ? 'Spots' : 'Workers',
    value: isSpots ? String(workersTotal) : `${workersWorking} / ${workersTotal}`,
    progress: isSpots
      ? undefined
      : workersTotal ? Math.min(100, Math.round((workersWorking / workersTotal) * 100)) : 0,
  };
  return [workers, ...kpis];
});
</script>
