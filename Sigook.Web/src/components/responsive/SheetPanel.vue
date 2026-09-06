<template>
  <teleport to="body">
    <transition name="sheet-panel">
      <div v-if="modelValue" class="sheet-panel">
        <div class="sheet-panel__overlay" @click="close"></div>
        <div
          ref="panel"
          class="sheet-panel__sheet"
          role="dialog"
          aria-modal="true"
          tabindex="-1"
          @keydown.esc="close"
        >
          <header class="sheet-panel__head">
            <span class="sheet-panel__title">
              <slot name="title">{{ title }}</slot>
            </span>
            <b-button icon-left="close" size="is-small" @click="close" />
          </header>
          <div class="sheet-panel__body">
            <slot />
          </div>
          <footer v-if="$slots.foot" class="sheet-panel__foot">
            <slot name="foot" />
          </footer>
        </div>
      </div>
    </transition>
  </teleport>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import { useBodyScrollLock } from '@/composables/useBodyScrollLock';
import { useFocusTrap } from '@/composables/useFocusTrap';

const props = defineProps<{
  modelValue: boolean;
  title: string;
}>();

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void;
}>();

const panel = ref<HTMLElement | null>(null);
const open = computed(() => props.modelValue);
const { lock, unlock } = useBodyScrollLock();

useFocusTrap(open, panel);

watch(open, (value) => {
  if (value) {
    lock();
  } else {
    unlock();
  }
});

function close() {
  emit('update:modelValue', false);
}
</script>

<style lang="scss" scoped>
@import '../../assets/scss/variables';

.sheet-panel {
  position: fixed;
  inset: 0;
  z-index: 55;
}

.sheet-panel__overlay {
  position: absolute;
  inset: 0;
  background: rgba($navy, 0.45);
}

.sheet-panel__sheet {
  position: absolute;
  left: 0;
  right: 0;
  bottom: 0;
  max-height: 85vh;
  display: flex;
  flex-direction: column;
  background: $white;
  border-radius: 12px 12px 0 0;
  box-shadow: 0 -4px 24px rgba(0, 0, 0, 0.15);
  outline: none;
}

.sheet-panel__head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 8px;
  padding: 12px 16px;
  border-bottom: 1px solid $gray-border;
}

.sheet-panel__title {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  font-weight: 600;
  font-size: 1rem;
  color: $navy;
}

.sheet-panel__body {
  flex: 1 1 auto;
  min-height: 0;
  overflow-y: auto;
  padding: 16px;
}

.sheet-panel__foot {
  display: flex;
  gap: 8px;
  padding: 12px 16px;
  border-top: 1px solid $gray-border;
  background: $white;

  :deep(.button) {
    flex: 1 1 0;
  }
}

.sheet-panel-enter-active,
.sheet-panel-leave-active {
  transition: opacity 0.2s ease;

  .sheet-panel__sheet {
    transition: transform 0.25s ease;
  }
}

.sheet-panel-enter-from,
.sheet-panel-leave-to {
  opacity: 0;

  .sheet-panel__sheet {
    transform: translateY(100%);
  }
}
</style>
