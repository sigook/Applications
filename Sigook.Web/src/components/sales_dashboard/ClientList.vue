<template>
  <dashboard-list :is-empty="!items.length" empty-text="No interactions yet">
    <dashboard-list-row
      v-for="item in items"
      :key="item.id"
      class="sd-client-row"
      role="button"
      tabindex="0"
      :title="`View interactions for ${item.fullName}`"
      :aria-label="`View interactions for ${item.fullName}`"
      @click="emit('select', item)"
      @keydown.enter="emit('select', item)"
      @keydown.space.prevent="emit('select', item)"
    >
      <template #avatar>
        <span class="sd-client-avatar">{{ initialsOf(item.fullName) }}</span>
      </template>
      <template #title>{{ item.fullName }}</template>
      <template #meta>
        <span class="sd-client-email">{{ item.email }}</span>
      </template>
      <template #trailing>
        <span class="sd-client-time">{{ relativeTime(item.lastInteractionAt, asOf) }}</span>
      </template>
    </dashboard-list-row>
  </dashboard-list>
</template>

<script setup lang="ts">
import DashboardList from './DashboardList.vue';
import DashboardListRow from './DashboardListRow.vue';
import type { SalesRecentClient } from '@/types/sales';
import { initialsOf, relativeTime } from '@/utils/salesDashboardFormat';

defineProps<{
  items: readonly SalesRecentClient[];
  asOf: string;
}>();

const emit = defineEmits<{
  (e: 'select', client: SalesRecentClient): void;
}>();
</script>

<style scoped lang="scss">
@import "../../assets/scss/variables";

.sd-client-row {
  cursor: pointer;

  &:focus-visible {
    outline: 2px solid rgba($primary, 0.5);
    outline-offset: 1px;
  }
}

.sd-client-email {
  min-width: 0;
  overflow: hidden;
  text-overflow: ellipsis;
}

.sd-client-time {
  font-size: 0.69rem;
  color: #b3b3b3;
}

.sd-client-avatar {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 100%;
  height: 100%;
  border-radius: 50%;
  background: #eef0f3;
  color: #7a7a7a;
  font-size: 0.69rem;
  font-weight: 700;
}
</style>
