<template>
  <SheetPanel :model-value="modelValue" :title="title" @update:modelValue="(v) => emit('update:modelValue', v)">
    <template #title>
      {{ title }}
      <span v-if="activeCount > 0" class="filters-panel__count">{{ activeCount }}</span>
    </template>
    <slot />
    <template #foot>
      <b-button expanded @click="emit('clear')">Clear all</b-button>
      <b-button type="is-primary" expanded @click="onApply">Apply</b-button>
    </template>
  </SheetPanel>
</template>

<script setup lang="ts">
import SheetPanel from '@/components/responsive/SheetPanel.vue';

withDefaults(
  defineProps<{
    modelValue: boolean;
    activeCount?: number;
    title?: string;
  }>(),
  { activeCount: 0, title: 'Filters' },
);

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void;
  (e: 'apply'): void;
  (e: 'clear'): void;
}>();

function onApply() {
  emit('apply');
  emit('update:modelValue', false);
}
</script>

<style lang="scss" scoped>
@import '../../assets/scss/variables';

.filters-panel__count {
  min-width: 20px;
  height: 20px;
  padding: 0 6px;
  border-radius: 10px;
  background: $primary;
  color: $white;
  font-size: 0.75rem;
  font-weight: 700;
  display: inline-flex;
  align-items: center;
  justify-content: center;
}
</style>
