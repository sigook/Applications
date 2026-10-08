<template>
  <detail-card v-if="location" title="Location">
    <iframe v-if="embedUrl" :src="embedUrl" class="request-location-map" title="Job location map" loading="lazy"
      allowfullscreen></iframe>
    <div class="request-location-text">
      <span>{{ address }}</span>
      <span v-if="location.mainIntersection" class="request-location-muted">{{ location.mainIntersection }}</span>
      <span v-if="location.entrance" class="request-location-muted">{{ location.entrance }}</span>
    </div>
    <b-button v-if="link" tag="a" :href="link" target="_blank" rel="noopener noreferrer" type="is-ghost"
      size="is-small" icon-right="open-in-new" class="request-location-link">Open in maps</b-button>
  </detail-card>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import type { LocationDetailModel } from '@/shared/types/common';
import { formatLocation, mapsEmbedUrl, mapsUrl } from '@/shared/request-detail/requestDetail';
import DetailCard from '@/shared/detail-page/DetailCard.vue';

const props = defineProps<{ location?: LocationDetailModel | null }>();

const address = computed(() => formatLocation(props.location));
const link = computed(() => mapsUrl(props.location));
const embedUrl = computed(() => mapsEmbedUrl(props.location));
</script>

<style lang="scss" scoped>
@import '@/assets/scss/variables';

.request-location-map {
  width: 100%;
  height: 160px;
  border: 0;
  border-radius: 8px;
}

.request-location-text {
  display: flex;
  flex-direction: column;
  gap: 2px;
  font-size: 0.9rem;
  color: $navy;
}

.request-location-muted {
  font-size: 0.8rem;
  color: $grey-font;
}

.button.request-location-link {
  align-self: flex-start;
  padding-inline: 0;
  font-weight: 600;
  color: $blue;

  &:hover,
  &:focus {
    color: $blue-dark;
    text-decoration: underline;
  }
}
</style>
