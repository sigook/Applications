<template>
  <div ref="el" class="sd-chart" role="img" :aria-label="ariaLabel">
    <svg v-if="width > 0" class="sd-chart__svg" :width="width" :height="height" aria-hidden="true">
      <g v-for="bar in bars" :key="bar.label">
        <path class="sd-bar-fill" :d="bar.path"></path>
        <text class="sd-bar-amount" :x="bar.cx" :y="bar.amountY" text-anchor="middle">{{ bar.amount }}</text>
        <text class="sd-bar-label" :x="bar.cx" :y="bar.labelY" text-anchor="middle">{{ bar.label }}</text>
      </g>
    </svg>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { scaleBand, scaleLinear } from 'd3-scale';
import { max as d3max } from 'd3-array';
import type { SalesSeriesPoint } from '@/types/salesDashboard';
import { compactMoney } from '@/utils/salesDashboardFormat';
import { useElementSize } from '@/composables/useElementSize';

const props = defineProps<{ points: readonly SalesSeriesPoint[] }>();

const PAD_X = 20;
const PAD_TOP = 16;
const PAD_BOTTOM = 12;
const AMOUNT_RESERVE = 18;
const LABEL_RESERVE = 18;
const BAR_MAX_WIDTH = 34;
const CORNER_RADIUS = 6;

interface BarGeom {
  readonly label: string;
  readonly amount: string;
  readonly path: string;
  readonly cx: number;
  readonly amountY: number;
  readonly labelY: number;
}

const { el, width, height } = useElementSize();

/** Rect with rounded TOP corners only (6px 6px 0 0), matching the old CSS. */
function topRoundedBar(x: number, y: number, w: number, h: number, r: number): string {
  const rr = Math.max(0, Math.min(r, w / 2, h));
  return (
    `M${x},${y + h}` +
    `L${x},${y + rr}` +
    `Q${x},${y} ${x + rr},${y}` +
    `L${x + w - rr},${y}` +
    `Q${x + w},${y} ${x + w},${y + rr}` +
    `L${x + w},${y + h}` +
    'Z'
  );
}

const bars = computed<BarGeom[]>(() => {
  const w = width.value;
  const h = height.value;
  if (w <= 0 || h <= 0 || props.points.length === 0) {
    return [];
  }

  const innerLeft = PAD_X;
  const innerRight = w - PAD_X;
  const baselineY = h - PAD_BOTTOM - LABEL_RESERVE;
  const topY = PAD_TOP + AMOUNT_RESERVE;
  if (innerRight <= innerLeft || baselineY <= topY) {
    return [];
  }

  const highest = d3max(props.points, (point) => point.value) ?? 0;
  const domainMax = highest > 0 ? highest : 1;

  const x = scaleBand<string>()
    .domain(props.points.map((point) => point.label))
    .range([innerLeft, innerRight])
    .paddingInner(0.3)
    .paddingOuter(0.05);
  const y = scaleLinear().domain([0, domainMax]).range([baselineY, topY]).clamp(true);

  const band = x.bandwidth();
  const barW = Math.min(band, BAR_MAX_WIDTH);

  return props.points.map((point) => {
    const bandX = x(point.label) ?? 0;
    const yTop = y(point.value);
    return {
      label: point.label,
      amount: compactMoney(point.value),
      path: topRoundedBar(bandX + (band - barW) / 2, yTop, barW, baselineY - yTop, CORNER_RADIUS),
      cx: bandX + band / 2,
      amountY: yTop - 6,
      labelY: baselineY + 14,
    };
  });
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
}

.sd-chart__svg {
  display: block;
}

.sd-bar-fill {
  fill: $agency-primary;
}

.sd-bar-amount {
  font-size: 0.66rem;
  fill: #9a9a9a;
}

.sd-bar-label {
  font-size: 0.69rem;
  fill: #9a9a9a;
}
</style>
