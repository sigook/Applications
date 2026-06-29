<template>
  <LandingIcon :name="name" class="industry-icon" />
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
</script>

<script setup lang="ts">
import LandingIcon from './LandingIcon.vue'

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
