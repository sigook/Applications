<template>
  <detail-header :number-id="numberId" :title="title" :meta="subtitle" :org-name="orgName" :org-link="orgLink"
    :logo="logo" :status-label="statusLabel" :status-variant="statusVariant" :chips="headerChips" :kpis="headerKpis">
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
import type { DetailChip, DetailKpi } from '@/types/detailPage';
import type { RequestChipVariant, RequestKpi } from '@/types/requestDetail';
import DetailHeader from '@/components/detail_page/DetailHeader.vue';

const props = defineProps<{
  numberId: number;
  title: string;
  subtitle?: string;
  orgName?: string;
  orgLink?: RouteLocationRaw;
  logo?: string | null;
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
  ...(props.isAsap ? [{ label: 'ASAP', variant: 'is-warning' as const }] : []),
  ...(props.isDirectHiring ? [{ label: 'Direct hiring', variant: 'is-info' as const }] : []),
  ...(props.chips ?? []).map((label) => ({ label })),
]);

const headerKpis = computed<DetailKpi[]>(() => {
  if (props.workersTotal === undefined) return props.kpis;
  const isSpots = props.workersWorking === undefined;
  const workers: DetailKpi = {
    key: 'workers',
    label: isSpots ? 'Spots' : 'Workers',
    value: isSpots ? String(props.workersTotal) : `${props.workersWorking} / ${props.workersTotal}`,
    progress: isSpots
      ? undefined
      : props.workersTotal ? Math.min(100, Math.round(((props.workersWorking ?? 0) / props.workersTotal) * 100)) : 0,
  };
  return [workers, ...props.kpis];
});
</script>
