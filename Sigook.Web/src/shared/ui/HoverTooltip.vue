<template>
  <span ref="trigger" class="hover-tooltip-trigger" @mouseenter="open" @mouseleave="close" @focusin="open"
    @focusout="close">
    <slot></slot>
    <Teleport v-if="isOpen" to="body">
      <div class="hover-tooltip" :class="[`is-${placement}`, { 'is-multiline': multilined }]" role="tooltip"
        :style="style">{{ label }}</div>
    </Teleport>
  </span>
</template>

<script setup lang="ts">
import { onBeforeUnmount, ref, useTemplateRef } from 'vue';

const { label, active = true, multilined } = defineProps<{
  label?: string | null;
  active?: boolean;
  multilined?: boolean;
}>();

const GAP = 8;
const MIN_SPACE_ABOVE = 40;

const trigger = useTemplateRef<HTMLElement>('trigger');
const isOpen = ref(false);
const placement = ref<'top' | 'bottom'>('top');
const style = ref<Record<string, string>>({});

function open() {
  if (!label || !active || !trigger.value) return;
  const rect = trigger.value.getBoundingClientRect();
  const above = rect.top >= MIN_SPACE_ABOVE;
  placement.value = above ? 'top' : 'bottom';
  style.value = {
    left: `${rect.left + rect.width / 2}px`,
    top: `${above ? rect.top - GAP : rect.bottom + GAP}px`,
  };
  isOpen.value = true;
  window.addEventListener('scroll', close, true);
}

function close() {
  if (!isOpen.value) return;
  isOpen.value = false;
  window.removeEventListener('scroll', close, true);
}

onBeforeUnmount(close);
</script>

<style scoped lang="scss">
@import '@/assets/scss/variables';

.hover-tooltip-trigger {
  display: inline-block;
}

.hover-tooltip {
  position: fixed;
  z-index: 100;
  max-width: 300px;
  padding: 6px 10px;
  border-radius: 6px;
  background: $navy;
  color: $white;
  font-size: 12px;
  line-height: 1.3;
  white-space: nowrap;
  pointer-events: none;
  box-shadow: 0 2px 8px rgba($navy, 0.25);

  &.is-multiline {
    width: 300px;
    white-space: normal;
  }

  &::after {
    content: '';
    position: absolute;
    left: 50%;
    border: 5px solid transparent;
    transform: translateX(-50%);
  }

  &.is-top {
    transform: translate(-50%, -100%);

    &::after {
      top: 100%;
      border-top-color: $navy;
    }
  }

  &.is-bottom {
    transform: translateX(-50%);

    &::after {
      bottom: 100%;
      border-bottom-color: $navy;
    }
  }
}
</style>
