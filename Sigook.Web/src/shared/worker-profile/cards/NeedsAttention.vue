<template>
  <section class="worker-card worker-needs-attention">
    <div class="worker-card-header">
      <h3>Needs attention</h3>
      <span v-if="props.items.length" class="worker-count is-warning">{{ props.items.length }}</span>
    </div>
    <p v-if="!props.items.length" class="worker-needs-attention-empty">
      <b-icon icon="check-circle-outline" size="is-small" />
      Nothing pending
    </p>
    <button v-for="item in props.items" :key="item.key" type="button" class="worker-attention-item"
      :class="`is-${item.severity}`" @click="emit('select', item.sectionId)">
      <b-icon :icon="item.severity === 'danger' ? 'alert-circle-outline' : 'clock-outline'" size="is-small" />
      <span class="worker-attention-text">
        <strong>{{ item.title }}</strong>
        <span>{{ item.detail }}</span>
      </span>
    </button>
  </section>
</template>

<script setup lang="ts">
import type { WorkerAttentionItem, WorkerProfileSectionId } from '@/shared/worker-profile/types';

const props = defineProps<{ items: WorkerAttentionItem[] }>();
const emit = defineEmits<{ (e: 'select', id: WorkerProfileSectionId): void }>();
</script>

<style lang="scss" scoped>
@import '@/assets/scss/variables';

.worker-needs-attention {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.worker-card-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 4px;

  h3 {
    margin: 0;
    font-weight: 700;
  }
}

.worker-count {
  min-width: 22px;
  padding: 0 7px;
  border-radius: 999px;
  font-size: 0.75rem;
  font-weight: 700;
  text-align: center;
  background: rgba($accent, 0.16);
  color: $accent-text;
}

.worker-needs-attention-empty {
  display: flex;
  align-items: center;
  gap: 6px;
  margin: 0;
  color: $green-text;
  font-size: 0.9rem;
}

.worker-attention-item {
  display: flex;
  align-items: flex-start;
  gap: 10px;
  width: 100%;
  padding: 10px 12px;
  border: 0;
  border-radius: 8px;
  font: inherit;
  text-align: left;
  cursor: pointer;

  &:hover {
    filter: brightness(0.97);
  }

  &.is-danger {
    background: rgba($danger, 0.07);

    .icon {
      color: $danger-hover;
    }
  }

  &.is-warning {
    background: rgba($accent, 0.1);

    .icon {
      color: $accent-text;
    }
  }
}

.worker-attention-text {
  display: flex;
  flex-direction: column;
  gap: 2px;
  font-size: 0.875rem;
  color: $navy;

  span {
    font-size: 0.8rem;
    color: $grey-font;
  }
}
</style>
