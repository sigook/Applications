<template>
  <div class="sd-chart" role="img" :aria-label="ariaLabel">
    <div v-for="point in points" :key="point.label" class="sd-bar-col" aria-hidden="true">
      <span class="sd-bar-amount">{{ compactMoney(point.value) }}</span>
      <div class="sd-bar-track">
        <div class="sd-bar-fill" :style="{ height: ratioOf(point.value, max) * 100 + '%' }"></div>
      </div>
      <span class="sd-bar-label">{{ point.label }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import type { SalesSeriesPoint } from '@/types/salesDashboard';
import { compactMoney, ratioOf } from '@/utils/salesDashboardFormat';

const props = defineProps<{ points: readonly SalesSeriesPoint[] }>();

const max = computed<number>(() => {
  const highest = props.points.reduce((acc, point) => (point.value > acc ? point.value : acc), 0);
  return highest > 0 ? highest : 1;
});

const ariaLabel = computed<string>(() => {
  if (props.points.length === 0) {
    return 'Deals closed: no data';
  }
  const parts = props.points.map((point) => `${point.label} ${compactMoney(point.value)}`);
  return `Deals closed: ${parts.join(', ')}`;
});
</script>

<style scoped lang="scss">
@import "../../assets/scss/variables";

.sd-chart {
  flex: 1;
  min-height: 0;
  display: flex;
  align-items: flex-end;
  gap: 0.625rem;
  padding: 1rem 1.25rem 0.75rem;
}

.sd-bar-col {
  flex: 1;
  height: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.375rem;
}

.sd-bar-amount {
  flex: none;
  font-size: 0.66rem;
  color: #9a9a9a;
}

.sd-bar-track {
  flex: 1;
  min-height: 0;
  width: 100%;
  display: flex;
  align-items: flex-end;
  justify-content: center;
}

.sd-bar-fill {
  width: 100%;
  max-width: 34px;
  background: $primary;
  border-radius: 6px 6px 0 0;
  transition: height 0.35s ease;
}

.sd-bar-label {
  flex: none;
  font-size: 0.69rem;
  color: #9a9a9a;
}
</style>
