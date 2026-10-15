<template>
  <div class="request-groups-toolbar">
    <b-switch v-model="onlyMine" class="request-groups-toolbar-filter" @update:modelValue="emit('search')">
      Assigned to me
    </b-switch>
    <template v-if="hasGroups">
      <b-button size="is-small" icon-left="chevron-double-down" @click="emit('expandAll')">Expand all requests</b-button>
      <b-button size="is-small" icon-left="chevron-double-up" @click="emit('collapseAll')">Collapse all</b-button>
    </template>
    <export :url="exportUrl" :params="exportParams" :file-name="exportFileName"
      @onDataLoading="(value: boolean) => emit('loading', value)" />
  </div>
</template>

<script setup lang="ts">
import Export from '@/shared/ui/Export.vue';
import type { GridParams } from '@/shared/types/common';

defineProps<{
  hasGroups: boolean;
  exportUrl: string;
  exportParams: GridParams;
  exportFileName: string;
}>();

const onlyMine = defineModel<boolean>('onlyMine', { default: false });

const emit = defineEmits<{
  (e: 'search'): void;
  (e: 'expandAll'): void;
  (e: 'collapseAll'): void;
  (e: 'loading', value: boolean): void;
}>();
</script>

<style scoped lang="scss">
.request-groups-toolbar {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 8px;
  margin-bottom: 8px;

  :deep(.field) {
    margin-bottom: 0;
  }

  .request-groups-toolbar-filter {
    margin-right: auto;
  }
}
</style>
