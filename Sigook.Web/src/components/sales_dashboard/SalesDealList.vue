<template>
  <sales-list :is-empty="!items.length">
    <sales-list-row v-for="item in items" :key="item.id">
      <template #title>{{ item.name }}</template>
      <template #meta>
        <span class="sd-deal-client">{{ item.clientName }}</span>
        <span class="sd-deal-stage">{{ item.stage }}</span>
      </template>
      <template #trailing>
        <span class="sd-deal-value">{{ compactMoney(item.value) }}</span>
      </template>
    </sales-list-row>
  </sales-list>
</template>

<script setup lang="ts">
import SalesList from './SalesList.vue';
import SalesListRow from './SalesListRow.vue';
import type { SalesDeal } from '@/types/salesDashboard';
import { compactMoney } from '@/utils/salesDashboardFormat';

defineProps<{
  items: readonly SalesDeal[];
}>();
</script>

<style scoped lang="scss">
.sd-deal-client {
  min-width: 0;
  overflow: hidden;
  text-overflow: ellipsis;
}

.sd-deal-stage {
  flex: none;
  font-size: 0.625rem;
  padding: 2px 7px;
  border-radius: 999px;
  background: #eef0f3;
  color: #666;
}

.sd-deal-value {
  font-size: 0.75rem;
  font-weight: 700;
  color: #333;
}
</style>
