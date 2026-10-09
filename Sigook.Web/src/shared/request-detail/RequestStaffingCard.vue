<template>
  <detail-card v-if="items.length" title="Staffing">
    <ul class="request-staffing">
      <li v-for="item in items" :key="item.key" class="request-staffing-item" :class="item.variant">
        <b-icon :icon="icons[item.key] ?? 'information-outline'" size="is-small" class="request-staffing-icon" />
        <div class="request-staffing-text">
          <span class="request-staffing-title">{{ item.title }}</span>
          <span v-if="item.detail || item.actionLabel" class="request-staffing-detail">
            {{ item.detail }}
            <template v-if="item.actionLabel">
              ·
              <b-button type="is-ghost" size="is-small" class="request-staffing-action" @click="emit('action', item.key)">
                {{ item.actionLabel }}
              </b-button>
            </template>
          </span>
        </div>
      </li>
    </ul>
  </detail-card>
</template>

<script setup lang="ts">
import type { RequestStaffingItem } from '@/shared/request-detail/types';
import DetailCard from '@/shared/detail-page/DetailCard.vue';

defineProps<{ items: RequestStaffingItem[] }>();
const emit = defineEmits<{ (e: 'action', key: string): void }>();

const icons: Record<string, string> = {
  start: 'clock-outline',
  applicants: 'account-multiple-outline',
  invitation: 'email-outline',
};
</script>

<style lang="scss" scoped>
@import '@/assets/scss/variables';

.request-staffing {
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin: 0;
  padding: 0;
  list-style: none;
}

.request-staffing-item {
  display: flex;
  align-items: flex-start;
  gap: 10px;
  padding: 12px;
  border-radius: 8px;
  background: $gray-bg;
  color: $navy;

  &.is-warning {
    background: rgba($accent, 0.12);

    .request-staffing-icon {
      color: $accent-text;
    }
  }
}

.request-staffing-icon {
  flex-shrink: 0;
  margin-top: 2px;
  color: $grey-font;
}

.request-staffing-text {
  display: flex;
  flex-direction: column;
  gap: 2px;
  min-width: 0;
}

.request-staffing-title {
  font-size: 0.9rem;
  font-weight: 600;
}

.request-staffing-detail {
  font-size: 0.8rem;
  color: $grey-font;
}

.button.request-staffing-action {
  height: auto;
  padding: 0;
  font-size: 0.8rem;
  font-weight: 600;
  color: $blue;
  vertical-align: baseline;

  &:hover,
  &:focus {
    color: $blue-dark;
    text-decoration: underline;
  }
}
</style>
