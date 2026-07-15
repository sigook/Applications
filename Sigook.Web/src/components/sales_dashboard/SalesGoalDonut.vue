<template>
  <div class="sd-goal">
    <svg
      class="sd-goal__svg"
      width="86"
      height="86"
      viewBox="0 0 120 120"
      role="img"
      :aria-label="`${percent}% to goal`"
    >
      <circle cx="60" cy="60" r="52" fill="none" stroke="#eef0f3" stroke-width="12"></circle>
      <circle
        class="sd-goal__value"
        cx="60"
        cy="60"
        r="52"
        fill="none"
        stroke-width="12"
        stroke-linecap="round"
        transform="rotate(-90 60 60)"
        :stroke-dasharray="circumference"
        :stroke-dashoffset="dashOffset"
      ></circle>
      <text class="sd-goal__percent" x="60" y="57" text-anchor="middle">{{ percent }}%</text>
      <text class="sd-goal__caption" x="60" y="77" text-anchor="middle">to goal</text>
    </svg>

    <div class="sd-goal__info">
      <span class="sd-goal__label">Target vs. actual</span>
      <span class="sd-goal__actual">{{ compactMoney(goal.actual) }}</span>
      <span class="sd-goal__target">of {{ compactMoney(goal.target) }} goal</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import type { SalesGoal } from '@/types/salesDashboard';
import { compactMoney } from '@/utils/salesDashboardFormat';

const props = defineProps<{ goal: SalesGoal }>();

const circumference = 2 * Math.PI * 52;

const percent = computed(() =>
  props.goal.target > 0 ? Math.round((props.goal.actual / props.goal.target) * 100) : 0
);

const dashOffset = computed(() => {
  const clamped = Math.min(100, Math.max(0, percent.value));
  return circumference * (1 - clamped / 100);
});
</script>

<style scoped lang="scss">
@import "../../assets/scss/variables";

.sd-goal {
  display: flex;
  align-items: center;
  gap: 0.875rem;
}

.sd-goal__svg {
  flex: none;
}

.sd-goal__value {
  stroke: $green-vivid;
  transition: stroke-dashoffset 0.6s ease;
}

.sd-goal__percent {
  font-size: 24px;
  font-weight: 700;
  fill: #333;
}

.sd-goal__caption {
  font-size: 11px;
  fill: #9a9a9a;
}

.sd-goal__info {
  display: flex;
  flex-direction: column;
}

.sd-goal__label {
  font-size: 0.75rem;
  color: #9a9a9a;
}

.sd-goal__actual {
  font-size: 1.25rem;
  font-weight: 700;
  color: #333;
}

.sd-goal__target {
  font-size: 0.75rem;
  color: #9a9a9a;
}
</style>
