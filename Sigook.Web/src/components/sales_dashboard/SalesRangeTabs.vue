<template>
  <div class="sd-tabs">
    <b-button
      v-for="tab in SALES_RANGE_TABS"
      :key="tab.key"
      size="is-small"
      :class="['sd-tab', { 'is-active': tab.key === modelValue }]"
      :aria-pressed="tab.key === modelValue"
      @click="select(tab.key)"
    >
      {{ tab.label }}
    </b-button>
  </div>
</template>

<script setup lang="ts">
import type { SalesRangeKey } from '@/types/sales';
import { SALES_RANGE_TABS } from '@/types/sales';

defineProps<{ modelValue: SalesRangeKey }>();

const emit = defineEmits<{
  (e: 'update:modelValue', value: SalesRangeKey): void;
}>();

const select = (value: SalesRangeKey): void => {
  emit('update:modelValue', value);
};
</script>

<style scoped lang="scss">
@import "../../assets/scss/variables";

.sd-tabs {
  display: inline-flex;
  gap: 2px;
  background: $gray-bg;
  border-radius: 8px;
  padding: 3px;
}

.sd-tabs :deep(.button) {
  border: none;
  background: transparent;
  box-shadow: none;
  height: auto;
  min-height: 0;
  padding: 5px 13px;
  margin: 0;
  color: #555;
  font-size: 0.75rem;
  line-height: 1.2;
  border-radius: 6px;
  transition: background 0.2s ease, box-shadow 0.2s ease;
}

.sd-tabs :deep(.button:hover),
.sd-tabs :deep(.button:focus) {
  border: none;
  box-shadow: none;
  color: #333;
}

.sd-tabs :deep(.button:active) {
  border: none;
  box-shadow: none;
}

.sd-tabs :deep(.sd-tab.is-active),
.sd-tabs :deep(.sd-tab.is-active:hover),
.sd-tabs :deep(.sd-tab.is-active:focus) {
  background: $white;
  border-radius: 6px;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.12);
  color: #333;
}
</style>
