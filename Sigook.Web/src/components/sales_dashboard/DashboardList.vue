<template>
  <div class="sd-list">
    <p v-if="isEmpty" class="sd-list__empty">{{ emptyText }}</p>
    <slot v-else></slot>
  </div>
</template>

<script setup lang="ts">
withDefaults(
  defineProps<{
    isEmpty: boolean;
    emptyText?: string;
  }>(),
  { emptyText: 'Nothing yet' }
);
</script>

<style scoped lang="scss">
.sd-list {
  flex: 1;
  min-height: 0;
  overflow-y: auto;
  padding: 5px 7px;
}

.sd-list__empty {
  margin: 0;
  padding: 0.56rem 0.5rem;
  font-size: 0.75rem;
  color: #9a9a9a;
}
</style>
