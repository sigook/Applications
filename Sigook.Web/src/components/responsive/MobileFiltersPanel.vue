<template>
  <teleport to="body">
    <transition name="filters-panel">
      <div v-if="modelValue" class="filters-panel">
        <div class="filters-panel__overlay" @click="close"></div>
        <div
          ref="panel"
          class="filters-panel__sheet"
          role="dialog"
          aria-modal="true"
          tabindex="-1"
          @keydown.esc="close"
        >
          <header class="filters-panel__head">
            <span class="filters-panel__title">
              {{ title }}
              <span v-if="activeCount > 0" class="filters-panel__count">{{ activeCount }}</span>
            </span>
            <b-button icon-left="close" size="is-small" @click="close" />
          </header>
          <div class="filters-panel__body">
            <slot />
          </div>
          <footer class="filters-panel__foot">
            <b-button expanded @click="onClear">Clear all</b-button>
            <b-button type="is-primary" expanded @click="onApply">Apply</b-button>
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

const props = withDefaults(
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

function onApply() {
  emit('apply');
  close();
}

function onClear() {
  emit('clear');
}
</script>

<style lang="scss" scoped>
.filters-panel {
  position: fixed;
  inset: 0;
  z-index: 55;
}

.filters-panel__overlay {
  position: absolute;
  inset: 0;
  background: rgba(15, 47, 68, 0.45);
}

.filters-panel__sheet {
  position: absolute;
  left: 0;
  right: 0;
  bottom: 0;
  max-height: 85vh;
  display: flex;
  flex-direction: column;
  background: #fff;
  border-radius: 12px 12px 0 0;
  box-shadow: 0 -4px 24px rgba(0, 0, 0, 0.15);
  outline: none;
}

.filters-panel__head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 8px;
  padding: 12px 16px;
  border-bottom: 1px solid #e6e6e6;
}

.filters-panel__title {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  font-weight: 600;
  font-size: 1rem;
}

.filters-panel__count {
  min-width: 20px;
  height: 20px;
  padding: 0 6px;
  border-radius: 10px;
  background: #00adef;
  color: #fff;
  font-size: 0.75rem;
  font-weight: 700;
  display: inline-flex;
  align-items: center;
  justify-content: center;
}

.filters-panel__body {
  flex: 1 1 auto;
  min-height: 0;
  overflow-y: auto;
  padding: 16px;
}

.filters-panel__foot {
  display: flex;
  gap: 8px;
  padding: 12px 16px;
  border-top: 1px solid #e6e6e6;
  background: #fff;

  .button {
    flex: 1 1 0;
  }
}

.filters-panel-enter-active,
.filters-panel-leave-active {
  transition: opacity 0.2s ease;

  .filters-panel__sheet {
    transition: transform 0.25s ease;
  }
}

.filters-panel-enter-from,
.filters-panel-leave-to {
  opacity: 0;

  .filters-panel__sheet {
    transform: translateY(100%);
  }
}

@media (min-width: 768px) {
  .filters-panel__sheet {
    left: auto;
    top: 0;
    width: 400px;
    max-width: 90vw;
    max-height: none;
    border-radius: 0;
  }

  .filters-panel-enter-from,
  .filters-panel-leave-to {
    .filters-panel__sheet {
      transform: translateX(100%);
    }
  }
}
</style>
