<template>
  <div ref="el" class="sd-chart" role="img" :aria-label="ariaLabel">
    <svg v-if="width > 0" class="sd-chart__svg" :width="width" :height="height" aria-hidden="true">
      <g v-for="bar in bars" :key="bar.key">
        <title>{{ bar.tooltip }}</title>
        <path class="sd-bar-fill" :d="bar.path" :style="{ fill: bar.color }"></path>
        <text class="sd-bar-amount" :x="bar.cx" :y="bar.amountY" text-anchor="middle">{{ bar.amount }}</text>
        <text
          v-for="(line, index) in bar.labelLines"
          :key="line"
          class="sd-bar-label"
          :x="bar.cx"
          :y="bar.labelY + index * LABEL_LINE_HEIGHT"
          :text-anchor="isRotated ? 'end' : 'middle'"
          :transform="isRotated ? `rotate(-45, ${bar.cx}, ${bar.labelY})` : undefined"
        >{{ line }}</text>
      </g>
    </svg>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { scaleBand, scaleLinear } from 'd3-scale';
import { max as d3max } from 'd3-array';
import type { SalesBarPoint } from '@/types/sales';
import { useElementSize } from '@/composables/useElementSize';

const props = withDefaults(
  defineProps<{
    points: readonly SalesBarPoint[];
    title: string;
    formatValue?: (value: number) => string;
  }>(),
  { formatValue: (value: number) => String(value) }
);

const PAD_X = 20;
const PAD_TOP = 16;
const PAD_BOTTOM = 12;
const AMOUNT_RESERVE = 18;
const LABEL_RESERVE = 30;
const LABEL_LINE_HEIGHT = 11;
const LABEL_CHAR_WIDTH = 6.5;
const BAR_MAX_WIDTH = 34;
const CORNER_RADIUS = 6;
/** sin(45deg): a label rotated 45 degrees needs this share of its width in both axes. */
const ROTATED_RATIO = Math.SQRT1_2;

interface BarGeom {
  readonly key: string;
  readonly amount: string;
  readonly labelLines: string[];
  readonly tooltip: string;
  readonly color: string;
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

function textWidth(text: string): number {
  return text.length * LABEL_CHAR_WIDTH;
}

/** Split a two-word label across two lines. */
function wrapLabel(label: string): string[] {
  const words = label.split(' ');
  if (words.length < 2) {
    return [label];
  }
  return [words[0], words.slice(1).join(' ')];
}

/**
 * Labels get progressively more room: one line, then wrapped onto two, then
 * rotated 45 degrees when even the wrapped lines are wider than the band.
 */
const layout = computed(() => {
  const w = width.value;
  if (w <= 0 || props.points.length === 0) {
    return { band: 0, bandX: () => 0, lines: new Map<string, string[]>(), rotated: false, labelReserve: LABEL_RESERVE };
  }

  const x = scaleBand<string>()
    .domain(props.points.map((point) => point.key))
    .range([PAD_X, w - PAD_X])
    .paddingInner(0.3)
    .paddingOuter(0.05);
  const band = x.bandwidth();

  // Wrap only the labels that do not fit on one line.
  const wrapped = new Map(
    props.points.map((point) => [point.key, textWidth(point.label) <= band ? [point.label] : wrapLabel(point.label)])
  );
  const widestLine = d3max([...wrapped.values()].flat(), (line) => textWidth(line)) ?? 0;
  if (widestLine <= band) {
    return { band, bandX: (key: string) => x(key) ?? 0, lines: wrapped, rotated: false, labelReserve: LABEL_RESERVE };
  }

  // Rotated text runs down-left from the tick, so it needs both extra bottom room
  // and a wider left margin to stay inside the SVG.
  const widest = d3max(props.points, (point) => textWidth(point.label)) ?? 0;
  const diagonal = Math.ceil(widest * ROTATED_RATIO) + 10;
  const rotatedReserve = Math.min(diagonal, Math.max(LABEL_RESERVE, height.value * 0.45));
  const rotatedX = scaleBand<string>()
    .domain(props.points.map((point) => point.key))
    .range([Math.min(rotatedReserve, w / 3), w - PAD_X])
    .paddingInner(0.3)
    .paddingOuter(0.05);
  return {
    band: rotatedX.bandwidth(),
    bandX: (key: string) => rotatedX(key) ?? 0,
    lines: new Map(props.points.map((point) => [point.key, [point.label]])),
    rotated: true,
    labelReserve: rotatedReserve,
  };
});

const isRotated = computed<boolean>(() => layout.value.rotated);

const bars = computed<BarGeom[]>(() => {
  const w = width.value;
  const h = height.value;
  if (w <= 0 || h <= 0 || props.points.length === 0) {
    return [];
  }

  const { band, bandX, lines, labelReserve } = layout.value;
  const baselineY = h - PAD_BOTTOM - labelReserve;
  const topY = PAD_TOP + AMOUNT_RESERVE;
  if (band <= 0 || baselineY <= topY) {
    return [];
  }

  const highest = d3max(props.points, (point) => point.value) ?? 0;
  const domainMax = highest > 0 ? highest : 1;
  const y = scaleLinear().domain([0, domainMax]).range([baselineY, topY]).clamp(true);

  const barW = Math.min(band, BAR_MAX_WIDTH);

  return props.points.map((point) => {
    const x0 = bandX(point.key);
    const yTop = y(point.value);
    const amount = props.formatValue(point.value);
    return {
      key: point.key,
      amount,
      labelLines: lines.get(point.key) ?? [point.label],
      tooltip: point.caption ? `${point.label}: ${amount} · ${point.caption}` : `${point.label}: ${amount}`,
      color: point.color,
      path: topRoundedBar(x0 + (band - barW) / 2, yTop, barW, baselineY - yTop, CORNER_RADIUS),
      cx: x0 + band / 2,
      amountY: yTop - 6,
      labelY: baselineY + 14,
    };
  });
});

const ariaLabel = computed<string>(() => {
  if (props.points.length === 0) {
    return `${props.title}: no data`;
  }
  const parts = props.points.map((point) => `${point.label} ${props.formatValue(point.value)}`);
  return `${props.title}: ${parts.join(', ')}`;
});
</script>

<style scoped lang="scss">
.sd-chart {
  flex: 1;
  min-height: 0;
}

.sd-chart__svg {
  display: block;
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
