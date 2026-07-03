<template>
  <svg
    class="landing-icon"
    :viewBox="def.viewBox ?? '0 0 24 24'"
    :fill="def.fill ?? 'none'"
    :stroke="isFilled ? 'none' : 'currentColor'"
    :stroke-width="strokeWidth ?? def.strokeWidth ?? 1.5"
    stroke-linecap="round"
    stroke-linejoin="round"
    aria-hidden="true"
  >
    <component
      :is="shape.tag"
      v-for="(shape, i) in def.shapes"
      :key="i"
      v-bind="shape.attrs"
    />
  </svg>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { ICONS, type IconDef, type LandingIconName } from './icons'

const props = defineProps<{
  name: LandingIconName
  /** Override the icon's default stroke width (stroke icons only). */
  strokeWidth?: number
}>()

const def = computed<IconDef>(() => ICONS[props.name])
const isFilled = computed(() => (def.value.fill ?? 'none') !== 'none')
</script>

<style scoped>
.landing-icon {
  display: block;
  width: 1em;
  height: 1em;
  color: inherit;
}
</style>
