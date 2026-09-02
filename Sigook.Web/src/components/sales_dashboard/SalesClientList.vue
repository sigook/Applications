<template>
  <sales-list :is-empty="!items.length">
    <sales-list-row
      v-for="item in items"
      :key="item.id"
      class="sd-client-row"
      role="button"
      tabindex="0"
      :title="`Log interaction for ${item.fullName}`"
      :aria-label="`Log interaction for ${item.fullName}`"
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
    </sales-list-row>
  </sales-list>
</template>

<script setup lang="ts">
import SalesList from './SalesList.vue';
import SalesListRow from './SalesListRow.vue';
import type { AgencyCompanyListItem } from '@/types/agency';
import { initialsOf } from '@/utils/salesDashboardFormat';

defineProps<{
  items: readonly AgencyCompanyListItem[];
}>();

const emit = defineEmits<{
  (e: 'select', client: AgencyCompanyListItem): void;
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
