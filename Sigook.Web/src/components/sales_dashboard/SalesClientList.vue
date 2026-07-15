<template>
  <sales-list :is-empty="!items.length">
    <sales-list-row v-for="item in items" :key="item.id">
      <template #avatar>
        <span class="sd-client-avatar">{{ initialsOf(item.name) }}</span>
      </template>
      <template #title>{{ item.name }}</template>
      <template #meta>{{ item.industry }}</template>
      <template #trailing>
        <span class="sd-client-value">{{ compactMoney(item.annualValue) }}</span>
      </template>
    </sales-list-row>
  </sales-list>
</template>

<script setup lang="ts">
import SalesList from './SalesList.vue';
import SalesListRow from './SalesListRow.vue';
import type { SalesClient } from '@/types/salesDashboard';
import { compactMoney, initialsOf } from '@/utils/salesDashboardFormat';

defineProps<{
  items: readonly SalesClient[];
}>();
</script>

<style scoped lang="scss">
@import "../../assets/scss/variables";

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

.sd-client-value {
  font-size: 0.75rem;
  font-weight: 600;
  color: $green-vivid;
}
</style>
