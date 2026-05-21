<template>
  <!-- Fixed internal viewBox from original Figma; width/height control display size -->
  <svg
    :width="width"
    :height="height"
    viewBox="0 0 52 20"
    fill="none"
    xmlns="http://www.w3.org/2000/svg"
    aria-hidden="true"
  >
    <line x1="0" y1="10" x2="42" y2="10" :stroke="color" :stroke-width="strokeWidth" />
    <polyline
      points="34,2 50,10 34,18"
      :stroke="color"
      :stroke-width="strokeWidth"
      fill="none"
      stroke-linecap="round"
      stroke-linejoin="round"
    />
  </svg>
</template>

<script setup lang="ts">
const props = withDefaults(defineProps<{
  width?: number
  height?: number
  color?: string
  strokeWidth?: number
}>(), {
  width: 52,
  height: 20,
  color: 'white',
  strokeWidth: 2,
})
</script>
