<template>
  <sales-list :is-empty="!items.length">
    <sales-list-row
      v-for="item in items"
      :key="item.id"
      class="sd-deal-row"
      role="button"
      tabindex="0"
      @click="emit('edit', item)"
      @keydown.enter="emit('edit', item)"
    >
      <template #title>{{ item.title }}</template>
      <template #meta>
        <span class="sd-deal-client">{{ item.companyName }}</span>
        <span class="sd-deal-stage">{{ DEAL_STATUS_LABELS[item.status] }}</span>
      </template>
      <template #trailing>
        <a
          v-if="item.documentPath"
          class="sd-deal-doc"
          :href="item.documentPath"
          target="_blank"
          rel="noopener"
          :title="item.documentName || 'Open document'"
          @click.stop
        >
          <b-icon icon="paperclip" size="is-small"></b-icon>
        </a>
        <span class="sd-deal-value">{{ compactMoney(item.value) }}</span>
      </template>
    </sales-list-row>
  </sales-list>
</template>

<script setup lang="ts">
import SalesList from './SalesList.vue';
import SalesListRow from './SalesListRow.vue';
import { DEAL_STATUS_LABELS } from '@/types/company';
import type { Deal } from '@/types/company';
import { compactMoney } from '@/utils/salesDashboardFormat';

defineProps<{
  items: readonly Deal[];
}>();

const emit = defineEmits<{ (e: 'edit', deal: Deal): void }>();
</script>

<style scoped lang="scss">
@import "../../assets/scss/variables";

.sd-deal-row {
  cursor: pointer;

  &:focus-visible {
    outline: 2px solid rgba($primary, 0.5);
    outline-offset: 1px;
  }
}

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

.sd-deal-doc {
  flex: none;
  display: inline-flex;
  align-items: center;
  margin-right: 0.4rem;
  color: $primary;

  &:hover {
    color: $primary;
    filter: brightness(0.9);
  }
}

.sd-deal-value {
  font-size: 0.75rem;
  font-weight: 700;
  color: #333;
}
</style>
