<template>
  <sales-list :is-empty="!items.length">
    <sales-list-row v-for="item in items" :key="item.id">
      <template #avatar>
        <span class="sd-interaction-avatar">
          <b-icon :icon="SALES_INTERACTION_ICONS[item.type]" size="is-small"></b-icon>
        </span>
      </template>
      <template #title>{{ item.title }}</template>
      <template #meta>{{ item.type }} · {{ item.clientName }}</template>
      <template #trailing>
        <span class="sd-interaction-time">{{ relativeTime(item.occurredAt, asOf) }}</span>
      </template>
    </sales-list-row>
  </sales-list>
</template>

<script setup lang="ts">
import SalesList from './SalesList.vue';
import SalesListRow from './SalesListRow.vue';
import { SALES_INTERACTION_ICONS } from '@/types/salesDashboard';
import type { SalesInteraction } from '@/types/salesDashboard';
import { relativeTime } from '@/utils/salesDashboardFormat';

defineProps<{
  items: readonly SalesInteraction[];
  asOf: string;
}>();
</script>

<style scoped lang="scss">
@import "../../assets/scss/variables";

.sd-interaction-avatar {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 100%;
  height: 100%;
  border-radius: 50%;
  background: rgba(33, 183, 255, 0.1);
  color: $primary;
}

.sd-interaction-time {
  font-size: 0.69rem;
  color: #b3b3b3;
}
</style>
