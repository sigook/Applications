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
      <g transform="translate(60 60)">
        <path class="sd-goal__track" :d="trackPath ?? ''"></path>
        <path class="sd-goal__value" :d="valuePath ?? ''"></path>
      </g>
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
import { arc } from 'd3-shape';
import { scaleLinear } from 'd3-scale';
import type { SalesGoal } from '@/types/salesDashboard';
import { compactMoney } from '@/utils/salesDashboardFormat';
import { useTween } from '@/composables/useTween';

const props = defineProps<{ goal: SalesGoal }>();

const INNER_RADIUS = 46;
const OUTER_RADIUS = 58;
const FULL_TURN = 2 * Math.PI;

// Rounded, UNCLAMPED value for the label + aria (an over-target goal still
// announces e.g. 130%; may read 100% for anything in the 99.5-99.99% band).
const percent = computed<number>(() =>
  props.goal.target > 0 ? Math.round((props.goal.actual / props.goal.target) * 100) : 0
);

// Raw ratio drives the ring so it only closes at a true 100%, not at a rounded one.
const ratioPercent = computed<number>(() =>
  props.goal.target > 0 ? (props.goal.actual / props.goal.target) * 100 : 0
);

// clamp lives only on the geometry: the ring never sweeps past a full turn.
const angleScale = scaleLinear().domain([0, 100]).range([0, FULL_TURN]).clamp(true);

const arcGen = arc().cornerRadius(6);

const trackPath = arcGen({
  innerRadius: INNER_RADIUS,
  outerRadius: OUTER_RADIUS,
  startAngle: 0,
  endAngle: FULL_TURN,
});

const targetAngle = computed<number>(() => angleScale(ratioPercent.value));
const sweepAngle = useTween(targetAngle, 600);

const valuePath = computed<string | null>(() =>
  arcGen({
    innerRadius: INNER_RADIUS,
    outerRadius: OUTER_RADIUS,
    startAngle: 0,
    endAngle: sweepAngle.value,
  })
);
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

.sd-goal__track {
  fill: #eef0f3;
}

.sd-goal__value {
  fill: $green;
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
