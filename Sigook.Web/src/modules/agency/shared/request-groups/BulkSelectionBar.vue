<template>
  <div v-if="count" class="bulk-selection">
    <span class="bulk-selection-count">{{ count }} selected</span>
    <slot></slot>
    <b-button size="is-small" type="is-text" @click="emit('clear')">Clear</b-button>
  </div>
</template>

<script setup lang="ts">
defineProps<{ count: number }>();
const emit = defineEmits<{ (e: 'clear'): void }>();
</script>

<style scoped lang="scss">
@import "@/assets/scss/variables";

.bulk-selection {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 8px;
  padding: 6px 10px;
  border: 1px solid $gray-border;
  border-radius: 4px;
  background: $gray-bg;
}

.bulk-selection-count {
  margin-right: auto;
  font-weight: 600;
}

:global(.bulk-skipped) {
  max-height: 40vh;
  overflow-y: auto;
  text-align: left;
}

:global(.bulk-skipped li + li) {
  margin-top: 4px;
}
</style>
