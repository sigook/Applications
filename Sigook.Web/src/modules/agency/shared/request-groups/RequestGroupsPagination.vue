<template>
  <b-pagination v-if="total > perPage" class="request-groups-pagination" :model-value="page" :total="total"
    :per-page="perPage" size="is-small" rounded @update:modelValue="(value: number) => emit('change', value)">
  </b-pagination>
</template>

<script setup lang="ts">
defineProps<{ page: number; total: number; perPage: number }>();
const emit = defineEmits<{ (e: 'change', page: number): void }>();
</script>

<style scoped lang="scss">
.pagination.request-groups-pagination {
  justify-content: flex-end;
  padding: 12px 0 4px;

  :deep(.pagination-list) {
    flex-grow: 0;
    justify-content: flex-end;
  }
}
</style>
