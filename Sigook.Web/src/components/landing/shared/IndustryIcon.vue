<template>
  <svg
    class="industry-icon"
    viewBox="0 0 24 24"
    fill="none"
    stroke="currentColor"
    stroke-width="1.5"
    stroke-linecap="round"
    stroke-linejoin="round"
    aria-hidden="true"
  >
    <component
      :is="shape.tag"
      v-for="(shape, i) in ICONS[name]"
      :key="i"
      v-bind="shape.attrs"
    />
  </svg>
</template>

<script lang="ts">
export type IndustryIconName =
  | 'automotive'
  | 'aviation'
  | 'construction'
  | 'engineering'
  | 'technology'
  | 'finance'
  | 'legal'
  | 'logistics'
  | 'manufacturing'
  | 'retail'
  | 'transportation'

interface IconShape {
  tag: 'path' | 'circle' | 'rect'
  attrs: Record<string, string | number>
}

const ICONS: Record<IndustryIconName, IconShape[]> = {
  automotive: [
    { tag: 'path', attrs: { d: 'M14.7 6.3a1 1 0 0 0 0 1.4l1.6 1.6a1 1 0 0 0 1.4 0l3.77-3.77a6 6 0 0 1-7.94 7.94l-6.91 6.91a2.12 2.12 0 0 1-3-3l6.91-6.91a6 6 0 0 1 7.94-7.94l-3.76 3.76z' } },
  ],
  aviation: [
    { tag: 'path', attrs: { d: 'M22 2 11 13' } },
    { tag: 'path', attrs: { d: 'M22 2 15 22l-4-9-9-4z' } },
  ],
  construction: [
    { tag: 'path', attrs: { d: 'M2 18h20v2H2z' } },
    { tag: 'path', attrs: { d: 'M4 18a8 8 0 0 1 16 0' } },
    { tag: 'path', attrs: { d: 'M10 6h4v6h-4z' } },
  ],
  engineering: [
    { tag: 'circle', attrs: { cx: 12, cy: 12, r: 3 } },
    { tag: 'path', attrs: { d: 'M12 1v3M12 20v3M4.22 4.22l2.12 2.12M17.66 17.66l2.12 2.12M1 12h3M20 12h3M4.22 19.78l2.12-2.12M17.66 6.34l2.12-2.12' } },
  ],
  technology: [
    { tag: 'rect', attrs: { x: 6, y: 6, width: 12, height: 12, rx: 1 } },
    { tag: 'path', attrs: { d: 'M4 9h2M4 12h2M4 15h2M18 9h2M18 12h2M18 15h2M9 4v2M12 4v2M15 4v2M9 18v2M12 18v2M15 18v2' } },
  ],
  finance: [
    { tag: 'circle', attrs: { cx: 12, cy: 12, r: 9 } },
    { tag: 'path', attrs: { d: 'M12 6v12' } },
    { tag: 'path', attrs: { d: 'M15 9.5C15 8.12 13.66 7 12 7s-3 1.12-3 2.5S10.34 12 12 12s3 1.12 3 2.5-1.34 2.5-3 2.5-3-1.12-3-2.5' } },
  ],
  legal: [
    { tag: 'path', attrs: { d: 'M12 3v18' } },
    { tag: 'path', attrs: { d: 'M5 6h14' } },
    { tag: 'path', attrs: { d: 'm7 6-3 9a3 3 0 0 0 6 0L7 6z' } },
    { tag: 'path', attrs: { d: 'm17 6-3 9a3 3 0 0 0 6 0L17 6z' } },
    { tag: 'path', attrs: { d: 'M8 21h8' } },
  ],
  logistics: [
    { tag: 'path', attrs: { d: 'M21 16V8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16z' } },
    { tag: 'path', attrs: { d: 'M3.27 6.96 12 12.01l8.73-5.05' } },
    { tag: 'path', attrs: { d: 'M12 22.08V12' } },
  ],
  manufacturing: [
    { tag: 'path', attrs: { d: 'M2 20V10l6 4V10l6 4V10l6 4v6z' } },
    { tag: 'path', attrs: { d: 'M2 20h20' } },
    { tag: 'path', attrs: { d: 'M19 6V4l3 3v3' } },
  ],
  retail: [
    { tag: 'path', attrs: { d: 'M6 2 3 6v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V6l-3-4z' } },
    { tag: 'path', attrs: { d: 'M3 6h18' } },
    { tag: 'path', attrs: { d: 'M16 10a4 4 0 0 1-8 0' } },
  ],
  transportation: [
    { tag: 'path', attrs: { d: 'M14 18V6a1 1 0 0 0-1-1H2a1 1 0 0 0-1 1v12' } },
    { tag: 'path', attrs: { d: 'M14 8h4l3 5v5h-3' } },
    { tag: 'path', attrs: { d: 'M3 18h2M11 18h-3' } },
    { tag: 'circle', attrs: { cx: 7, cy: 18, r: 2 } },
    { tag: 'circle', attrs: { cx: 17, cy: 18, r: 2 } },
  ],
}
</script>

<script setup lang="ts">
defineProps<{
  name: IndustryIconName
}>()
</script>

<style scoped>
.industry-icon {
  display: block;
  width: 1em;
  height: 1em;
  color: inherit;
}
</style>
