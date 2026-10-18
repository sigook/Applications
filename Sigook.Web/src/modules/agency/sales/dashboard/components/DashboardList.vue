<template>
  <div class="sd-list">
    <div v-if="loading && isEmpty" class="sd-list__loading">
      <b-icon icon="loading" custom-class="mdi-spin" size="is-small"></b-icon>
    </div>
    <p v-else-if="isEmpty" class="sd-list__empty">{{ emptyText }}</p>
    <slot v-else></slot>
  </div>
</template>

<script setup lang="ts">
withDefaults(
  defineProps<{
    isEmpty: boolean;
    loading?: boolean;
    emptyText?: string;
  }>(),
  { loading: false, emptyText: 'Nothing yet' }
);
</script>

<style scoped lang="scss">
.sd-list {
  flex: 1;
  min-height: 0;
  overflow-y: auto;
  padding: 5px 7px;
}

.sd-list__loading {
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 1rem 0.5rem;
  color: #b3b3b3;
}

.sd-list__empty {
  margin: 0;
  padding: 0.56rem 0.5rem;
  font-size: 0.75rem;
  color: #9a9a9a;
}
</style>
