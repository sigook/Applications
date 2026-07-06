<template>
  <transition name="vtoast">
    <div
      v-if="visible"
      class="vtoast"
      role="status"
      aria-live="polite"
      aria-atomic="true"
    >
      <span class="vtoast__dot" aria-hidden="true">
        <span class="vtoast__dot-inner"></span>
      </span>

      <span class="vtoast__copy">
        <span class="vtoast__title">New version available</span>
        <span class="vtoast__hint">Reload to get the latest improvements.</span>
      </span>

      <button
        type="button"
        class="vtoast__update"
        @click="onUpdate"
      >
        <span>Update</span>
        <span class="vtoast__update-arrow" aria-hidden="true">→</span>
      </button>

      <button
        type="button"
        class="vtoast__dismiss"
        aria-label="Dismiss notification"
        @click="onDismiss"
      >
<LandingIcon name="close" :stroke-width="2.5" />
      </button>
    </div>
  </transition>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import LandingIcon from '@/components/landing/shared/icons/LandingIcon.vue'

const SESSION_DISMISS_KEY = 'app-version-toast-dismissed'

const props = withDefaults(defineProps<{
  modelValue?: boolean
}>(), {
  modelValue: false,
})

const emit = defineEmits<{
  (e: 'update'): void
  (e: 'update:modelValue', value: boolean): void
}>()

const visible = ref(false)

watch(
  () => props.modelValue,
  (isAvailable) => {
    if (!isAvailable) {
      visible.value = false
      return
    }
    const dismissed =
      typeof sessionStorage !== 'undefined' &&
      sessionStorage.getItem(SESSION_DISMISS_KEY) === '1'
    visible.value = !dismissed
  },
  { immediate: true },
)

function onUpdate(): void {
  emit('update')
}

function onDismiss(): void {
  visible.value = false
  if (typeof sessionStorage !== 'undefined') {
    sessionStorage.setItem(SESSION_DISMISS_KEY, '1')
  }
  emit('update:modelValue', false)
}
</script>

<style scoped>
.vtoast {
  position: fixed;
  right: clamp(16px, 2vw, 24px);
  bottom: clamp(16px, 2vw, 24px);
  z-index: 950;
  display: inline-flex;
  align-items: center;
  gap: 12px;
  padding: 12px 14px 12px 18px;
  max-width: calc(100vw - 32px);
  background: linear-gradient(180deg,
    rgba(15, 47, 68, 0.85) 0%,
    rgba(9, 48, 85, 0.85) 100%);
  border: 1px solid rgba(0, 173, 239, 0.45);
  border-radius:
    16px 16px 16px 32px;
  box-shadow: 0 18px 40px rgba(0, 0, 0, 0.40),
              0 0 0 1px rgba(0, 173, 239, 0.10);
  backdrop-filter: blur(22px) saturate(160%);
  -webkit-backdrop-filter: blur(22px) saturate(160%);
  color: #fff;
  font-family: var(--font-family);
}

.vtoast__dot {
  position: relative;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 12px;
  height: 12px;
  flex-shrink: 0;
}

.vtoast__dot-inner {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: var(--c-brand-cyan);
  box-shadow: 0 0 0 0 rgba(0, 173, 239, 0.50);
  animation: vtoast-pulse 1.8s ease-out infinite;
}

@keyframes vtoast-pulse {
  0%   { box-shadow: 0 0 0 0   rgba(0, 173, 239, 0.55); }
  70%  { box-shadow: 0 0 0 10px rgba(0, 173, 239, 0); }
  100% { box-shadow: 0 0 0 0   rgba(0, 173, 239, 0); }
}

@media (prefers-reduced-motion: reduce) {
  .vtoast__dot-inner { animation: none; }
}

.vtoast__copy {
  display: flex;
  flex-direction: column;
  gap: 2px;
  line-height: 1.25;
}

.vtoast__title {
  font-size: 13px;
  font-weight: 700;
  letter-spacing: 0.02em;
}

.vtoast__hint {
  font-size: 11px;
  font-weight: 500;
  color: rgba(255, 255, 255, 0.65);
  letter-spacing: 0.02em;
}

.vtoast__update {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 8px 16px;
  background: var(--c-brand-cyan);
  border: 1px solid var(--c-brand-cyan);
  border-radius: 999px;
  color: var(--c-brand-navy);
  font-family: var(--font-family);
  font-size: 12px;
  font-weight: 700;
  letter-spacing: 0.04em;
  cursor: pointer;
  white-space: nowrap;
  transition: background 0.2s ease, border-color 0.2s ease, transform 0.2s ease, box-shadow 0.2s ease;
}

.vtoast__update:hover {
  background: var(--c-brand-cyan-strong);
  border-color: var(--c-brand-cyan-strong);
  transform: translateY(-1px);
  box-shadow: 0 8px 18px rgba(0, 173, 239, 0.35);
}

.vtoast__update-arrow {
  font-size: 1.15em;
  line-height: 1;
  transition: transform 0.2s ease;
}

.vtoast__update:hover .vtoast__update-arrow {
  transform: translateX(2px);
}

.vtoast__dismiss {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 28px;
  height: 28px;
  background: rgba(255, 255, 255, 0.08);
  border: 1px solid var(--c-glass-border);
  border-radius: 50%;
  color: rgba(255, 255, 255, 0.70);
  cursor: pointer;
  flex-shrink: 0;
  transition: background 0.2s ease, color 0.2s ease, border-color 0.2s ease, transform 0.2s ease;
}

.vtoast__dismiss:hover {
  background: rgba(229, 45, 39, 0.20);
  border-color: rgba(229, 45, 39, 0.55);
  color: #fff;
  transform: rotate(90deg);
}

.vtoast__dismiss svg {
  width: 14px;
  height: 14px;
}

.vtoast-enter-active {
  transition: opacity 0.35s ease, transform 0.35s var(--ease-brand);
}

.vtoast-leave-active {
  transition: opacity 0.25s ease, transform 0.25s ease;
}

.vtoast-enter-from {
  opacity: 0;
  transform: translateY(20px);
}

.vtoast-leave-to {
  opacity: 0;
  transform: translateY(10px);
}

@media (max-width: 599px) {
  .vtoast {
    left: 16px;
    right: 16px;
    bottom: 16px;
    flex-wrap: wrap;
    gap: 10px;
    padding: 12px 14px;
    max-width: none;
  }

  .vtoast__dismiss {
    margin-left: auto;
  }

  .vtoast__update {
    order: 99;
    flex: 1 1 100%;
    justify-content: center;
    padding: 10px 16px;
  }

  .vtoast__copy {
    flex: 1 1 auto;
    min-width: 0;
  }
}

@media (max-width: 1023px) {
  .vtoast {
    backdrop-filter: none;
    -webkit-backdrop-filter: none;
    box-shadow: 0 18px 24px rgba(0, 0, 0, 0.40),
                0 0 0 1px rgba(0, 173, 239, 0.10);
  }

  .vtoast__dot-inner {
    animation: none;
  }
}
</style>
