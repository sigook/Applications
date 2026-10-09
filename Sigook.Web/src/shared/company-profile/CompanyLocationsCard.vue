<template>
  <detail-card title="Locations">
    <template v-if="locations.length" #actions>
      <b-button type="is-ghost" size="is-small" class="detail-link" @click="emit('showAll')">
        All {{ locations.length }}
      </b-button>
    </template>
    <iframe v-if="embedUrl" :src="embedUrl" class="company-locations-map" title="Company location map" loading="lazy"
      allowfullscreen></iframe>
    <div v-if="featured" class="detail-list-item">
      <div class="company-location-head">
        <span class="company-location-address">{{ featured.address }}</span>
        <span v-if="featured.isBilling" class="detail-tag is-info">Billing</span>
      </div>
      <span class="detail-muted">{{ cityLine(featured) }}</span>
    </div>
    <div v-else class="company-locations-empty">
      <span class="detail-empty">No locations</span>
      <b-button type="is-ghost" size="is-small" class="detail-link" @click="emit('showAll')">Add location</b-button>
    </div>
    <span v-if="locations.length > 1" class="detail-muted">+ {{ locations.length - 1 }} more</span>
  </detail-card>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import type { LocationDetailModel } from '@/shared/types/common';
import { showAlertError } from '@/shared/utils/toast';
import { mapsEmbedUrl } from '@/shared/request-detail/requestDetail';
import DetailCard from '@/shared/detail-page/DetailCard.vue';

const props = defineProps<{ fetchLocations: () => Promise<LocationDetailModel[]> }>();
const emit = defineEmits<{
  (e: 'showAll'): void;
  (e: 'loaded', count: number): void;
}>();

const locations = ref<LocationDetailModel[]>([]);

const featured = computed(() => locations.value.find((location) => location.isBilling) ?? locations.value[0]);
const embedUrl = computed(() => mapsEmbedUrl(featured.value));

function cityLine(location: LocationDetailModel) {
  return [location.city?.value, location.city?.province?.code, location.postalCode].filter(Boolean).join(', ');
}

function load() {
  props.fetchLocations()
    .then((response) => {
      locations.value = response;
      emit('loaded', response.length);
    })
    .catch(showAlertError);
}

defineExpose({ load });

load();
</script>

<style lang="scss" scoped>
.company-locations-map {
  width: 100%;
  height: 140px;
  border: 0;
  border-radius: 8px;
}

.company-location-head {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 8px;
}

.company-location-address {
  font-weight: 600;
  overflow-wrap: anywhere;
}

.company-locations-empty {
  display: flex;
  align-items: center;
  justify-content: space-between;
}
</style>
