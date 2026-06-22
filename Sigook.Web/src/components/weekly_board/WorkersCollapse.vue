<template>
  <div class="workers-collapse">
    <div class="workers-collapse-toggle" role="button" @click="open = !open">
      <span class="workers-collapse-title">{{ label }} ({{ count }})</span>
      <b-icon :icon="open ? 'chevron-up' : 'chevron-down'" size="is-small"></b-icon>
    </div>
    <div v-show="open" class="workers-collapse-body">
      <slot></slot>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';

defineProps<{
  label: string;
  count: number;
}>();

const open = ref(false);
</script>

<style scoped lang="scss">
@import "../../assets/scss/variables";

.workers-collapse-toggle {
  display: flex;
  justify-content: space-between;
  align-items: center;
  cursor: pointer;
  padding: 0.4rem 0.55rem;
  border: 1px solid $gray-border;
  border-radius: 8px;
  color: $grey-light;

  &:hover {
    color: $primary;
    border-color: $primary;
  }
}

.workers-collapse-title {
  font-size: 0.7rem;
  font-weight: 700;
}

.workers-collapse-body {
  margin-top: 0.4rem;
}
</style>
