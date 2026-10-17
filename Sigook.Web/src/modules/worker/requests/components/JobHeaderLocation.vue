<template>
  <div class="job-header-location">
    <span v-if="address" class="job-header-location__line">
      <b-icon icon="map-marker" size="is-small"></b-icon>
      {{ address }}
    </span>
    <span v-if="coordinates" class="job-header-location__line is-coords">
      <b-icon icon="crosshairs-gps" size="is-small"></b-icon>
      {{ coordinates }}
    </span>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import type { WorkerRequestDetail } from '@/modules/worker/requests/types';

const props = defineProps<{ request: WorkerRequestDetail }>();

const address = computed(() => [props.request.location, props.request.entrance].filter(Boolean).join(' - '));

const coordinates = computed(() => {
  const { latitude, longitude } = props.request.jobLocation ?? {};
  if (latitude == null || longitude == null) return '';
  return `${latitude.toFixed(6)}, ${longitude.toFixed(6)}`;
});
</script>

<style lang="scss" scoped>
@import '@/assets/scss/variables';

.job-header-location {
  display: flex;
  flex-direction: column;
  gap: 4px;
  margin-top: -2px;
}

.job-header-location__line {
  display: inline-flex;
  align-items: flex-start;
  gap: 4px;
  font-size: 0.9rem;
  color: $grey-font;

  &.is-coords {
    font-size: 0.85rem;
    font-variant-numeric: tabular-nums;
  }
}
</style>
