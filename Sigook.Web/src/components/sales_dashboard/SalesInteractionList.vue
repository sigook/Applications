<template>
  <sales-list :is-empty="!items.length">
    <sales-list-row
      v-for="item in items"
      :key="item.id"
      class="sd-interaction-row"
      role="button"
      tabindex="0"
      @click="emit('edit', item)"
      @keydown.enter="emit('edit', item)"
    >
      <template #avatar>
        <span class="sd-interaction-avatar">
          <b-icon :icon="INTERACTION_TYPE_ICONS[item.interactionType]" size="is-small"></b-icon>
        </span>
      </template>
      <template #title>{{ item.description }}</template>
      <template #meta>{{ INTERACTION_TYPE_LABELS[item.interactionType] }} · {{ item.companyName }}</template>
      <template #trailing>
        <span class="sd-interaction-time">{{ relativeTime(item.createdAt, asOf) }}</span>
      </template>
    </sales-list-row>
  </sales-list>
</template>

<script setup lang="ts">
import SalesList from './SalesList.vue';
import SalesListRow from './SalesListRow.vue';
import { INTERACTION_TYPE_ICONS, INTERACTION_TYPE_LABELS } from '@/types/companyInteraction';
import type { CompanyInteraction } from '@/types/companyInteraction';
import { relativeTime } from '@/utils/salesDashboardFormat';

defineProps<{
  items: readonly CompanyInteraction[];
  asOf: string;
}>();

const emit = defineEmits<{ (e: 'edit', interaction: CompanyInteraction): void }>();
</script>

<style scoped lang="scss">
@import "../../assets/scss/variables";

.sd-interaction-row {
  cursor: pointer;

  &:focus-visible {
    outline: 2px solid rgba(33, 183, 255, 0.5);
    outline-offset: 1px;
  }
}

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
