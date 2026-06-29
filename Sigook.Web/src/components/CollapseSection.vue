<template>
  <div class="collapse-section" :class="`collapse-section--${variant}`">
    <div class="collapse-section__header" role="button" tabindex="0" @click="toggle" @keydown.enter="toggle">
      <slot name="title">
        <span class="collapse-section__title">{{ title }}</span>
      </slot>
      <b-icon :icon="isOpen ? 'chevron-up' : 'chevron-down'" size="is-small" />
    </div>
    <div v-show="isOpen" class="collapse-section__body">
      <slot />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';

const props = withDefaults(defineProps<{
  title?: string;
  modelValue?: boolean;
  variant?: 'card' | 'compact';
}>(), {
  modelValue: true,
  variant: 'card',
});
const emit = defineEmits<{ (e: 'update:modelValue', value: boolean): void }>();

const isOpen = ref(props.modelValue);

watch(() => props.modelValue, value => {
  isOpen.value = value;
});

function toggle() {
  isOpen.value = !isOpen.value;
  emit('update:modelValue', isOpen.value);
}
</script>

<style scoped lang="scss">
@import "../assets/scss/variables";

.collapse-section {
  &__header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    cursor: pointer;
    user-select: none;
  }

  // Card variant: boxed section with a divider above the body.
  &--card {
    border: 1px solid #ededed;
    border-radius: 8px;
    margin-bottom: 16px;
    background: #fcfcfc;

    &:last-child {
      margin-bottom: 0;
    }

    .collapse-section__header {
      padding: 12px 16px;
    }

    .collapse-section__title {
      font-size: 1rem;
      font-weight: 600;
    }

    .collapse-section__body {
      padding: 14px 16px 16px;
      border-top: 1px solid #ededed;
    }
  }

  // Compact variant: lightweight pill-style toggle (weekly board).
  &--compact {
    .collapse-section__header {
      padding: 0.4rem 0.55rem;
      border: 1px solid $gray-border;
      border-radius: 8px;
      color: $grey-light;
      font-size: 0.7rem;
      font-weight: 700;

      &:hover {
        color: $primary;
        border-color: $primary;
      }
    }

    .collapse-section__body {
      margin-top: 0.4rem;
    }
  }
}
</style>
