<template>
  <section class="worker-header">
    <div class="worker-header-top">
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
      </div>
      <div v-if="$slots.actions" class="worker-header-actions">
        <slot name="actions" />
      </div>
    </div>
    <detail-kpis v-if="contactKpis.length" :kpis="contactKpis">
      <template #kpi-phone>
        <copy-link kind="phone" :value="props.worker.mobileNumber" />
      </template>
      <template #kpi-email>
        <copy-link kind="email" :value="props.worker.email" />
      </template>
    </detail-kpis>
  </section>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { lowercase } from '@/shared/format';
import type { DetailKpi } from '@/shared/detail-page/types';
import type { WorkerProfileDetail } from '@/shared/worker-profile/types';
import DetailKpis from '@/shared/detail-page/DetailKpis.vue';
import ImageDetail from '@/shared/worker-profile/forms/WorkImageDetail.vue';
import CopyLink from '@/shared/ui/CopyLink.vue';

const props = defineProps<{ worker: WorkerProfileDetail; numberClass?: string }>();
const emit = defineEmits<{ (e: 'updateProfile'): void }>();

const city = computed(() => {
  const value = props.worker.location?.city;
  if (!value) {
    return '';
  }
  return value.province?.code ? `${value.value}, ${value.province.code}` : value.value;
});

const contactKpis = computed<DetailKpi[]>(() => [
  ...(props.worker.mobileNumber ? [{ key: 'phone', label: 'Phone', value: props.worker.mobileNumber }] : []),
  ...(city.value ? [{ key: 'location', label: 'Location', value: city.value }] : []),
  ...(props.worker.email ? [{ key: 'email', label: 'Email', value: props.worker.email }] : []),
]);
</script>

<style lang="scss" scoped>
@import '@/assets/scss/variables';
@import '@/assets/scss/breakpoints';

.worker-header.worker-card {
  padding: 0;
}

.worker-header-top {
  display: flex;
  align-items: center;
  gap: 16px;
  padding: 16px 20px;

  @include mobile {
    flex-wrap: wrap;
    align-items: flex-start;
    gap: 12px;
    padding: 14px;
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
  gap: 6px;

  @include mobile {
    flex-basis: 0;
  }
}

.worker-header-name {
  margin: 0;
  font-size: 24px;
  font-weight: 700;
  line-height: 1.25;
  color: $navy;

  @include compact-desktop {
    font-size: 22px;
  }

  @include mobile {
    font-size: 21px;
  }
}

.worker-header-number {
  margin-left: 6px;
  font-size: 16px;
  font-weight: 400;
  color: $grey-font;
}

.worker-header-actions {
  display: flex;
  gap: 8px;
  flex-shrink: 0;

  :deep(.button) {
    height: 40px;
    font-size: 15px;
  }

  @include mobile {
    width: 100%;

    :deep(> .button:first-child) {
      flex: 1;
    }
  }
}

:deep(.copy-link-text) {
  color: $blue;
}
</style>
