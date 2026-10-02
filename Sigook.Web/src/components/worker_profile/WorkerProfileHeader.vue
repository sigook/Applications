<template>
  <section class="worker-header">
    <image-detail class="worker-header-photo" :data="props.worker" @updateProfile="emit('updateProfile')" />
    <div class="worker-header-main">
      <h2 class="worker-header-name">
        {{ lowercase(props.worker.firstName) }}
        {{ lowercase(props.worker.middleName) }}
        {{ lowercase(props.worker.lastName) }}
        {{ lowercase(props.worker.secondLastName) }}
        <span class="worker-header-number" :class="props.numberClass">#{{ props.worker.numberId }}</span>
      </h2>
      <slot name="chips" />
      <div class="worker-header-contact">
        <span v-if="props.worker.mobileNumber"><b-icon icon="cellphone" size="is-small" />{{ props.worker.mobileNumber }}</span>
        <a v-if="props.worker.email" :href="`mailto:${props.worker.email}`"><b-icon icon="email-outline" size="is-small" />{{ props.worker.email }}</a>
        <span v-if="city"><b-icon icon="map-marker-outline" size="is-small" />{{ city }}</span>
      </div>
    </div>
    <div v-if="$slots.actions" class="worker-header-actions">
      <slot name="actions" />
    </div>
  </section>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { lowercase } from '@/utils/filters';
import type { WorkerProfileDetail } from '@/types/worker';
import ImageDetail from '@/components/worker/WorkImageDetail.vue';

const props = defineProps<{ worker: WorkerProfileDetail; numberClass?: string }>();
const emit = defineEmits<{ (e: 'updateProfile'): void }>();

const city = computed(() => {
  const value = props.worker.location?.city;
  if (!value) {
    return '';
  }
  return value.province?.code ? `${value.value}, ${value.province.code}` : value.value;
});
</script>

<style lang="scss" scoped>
@import '../../assets/scss/variables';
@import '../../assets/scss/breakpoints';

.worker-header {
  display: flex;
  align-items: flex-start;
  gap: 20px;

  @include mobile {
    flex-wrap: wrap;
  }
}

.worker-header-photo {
  flex-shrink: 0;
}

.worker-header-main {
  flex-grow: 1;
  min-width: 0;
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.worker-header-name {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 700;
  line-height: 1.25;

  @include mobile {
    font-size: 1.2rem;
  }
}

.worker-header-number {
  margin-left: 6px;
  font-size: 1rem;
  font-weight: 400;
  color: $grey-font;
}

.worker-header-contact {
  display: flex;
  flex-wrap: wrap;
  gap: 6px 24px;
  font-size: 0.9rem;
  color: $grey-font;
  overflow-wrap: anywhere;

  span,
  a {
    display: inline-flex;
    align-items: center;
    gap: 4px;
  }

  a {
    color: $blue;
  }
}

.worker-header-actions {
  display: flex;
  gap: 8px;
  flex-shrink: 0;

  @include mobile {
    width: 100%;
  }
}
</style>
