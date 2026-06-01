<template>
  <teleport to="body">
    <div
      v-if="isOpen"
      class="reg-modal"
      role="dialog"
      aria-modal="true"
      aria-labelledby="reg-modal-title"
      @click.self="onBackdropClick"
    >
      <div class="reg-modal__panel" role="document">
        <header class="reg-modal__head">
          <h2 id="reg-modal-title" class="reg-modal__title">
            {{ title }}
            <span v-if="subtitle" class="reg-modal__subtitle">{{ subtitle }}</span>
          </h2>
          <button
            type="button"
            class="reg-modal__close"
            aria-label="Close registration dialog"
            @click="close"
          >
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <line x1="18" y1="6" x2="6" y2="18" />
              <line x1="6" y1="6" x2="18" y2="18" />
            </svg>
          </button>
        </header>

        <div class="reg-modal__body">
          <WorkerRegisterForm
            :job-title="context?.jobTitle"
            :redirect-on-success="false"
            @submitted="onSubmitted"
          />
        </div>
      </div>
    </div>
  </teleport>
</template>

<script setup lang="ts">
import { computed, onMounted, onUnmounted, watch } from 'vue'
import WorkerRegisterForm from '@/components/v2/landing/shared/forms/WorkerRegisterForm.vue'
import { useWorkerRegisterModal } from '@/components/v2/landing/shared/forms/useWorkerRegisterModal'

/**
 * WorkerRegisterModal — full-screen glass modal that wraps WorkerRegisterForm.
 *
 * - Triggered through the `useWorkerRegisterModal` singleton, so any V2
 *   component can call `.open({ jobTitle })`.
 * - Body scroll lock is handled by the composable on open/close.
 * - ESC key + backdrop click both close the modal.
 * - On successful submission the modal closes; redirecting is the caller's
 *   responsibility (we pass `redirectOnSuccess=false` to the inner form).
 */
const { isOpen, context, close } = useWorkerRegisterModal()

const title = computed(() => (context.value?.jobTitle ? 'Apply for this role' : 'Join Sigook'))
const subtitle = computed(() => context.value?.jobTitle ?? null)

function onBackdropClick(): void {
  close()
}

function onSubmitted(): void {
  // Give the success toast a moment, then dismiss.
  setTimeout(() => close(), 800)
}

function onKeydown(event: KeyboardEvent): void {
  if (event.key === 'Escape' && isOpen.value) {
    close()
  }
}

onMounted(() => {
  window.addEventListener('keydown', onKeydown)
})

onUnmounted(() => {
  window.removeEventListener('keydown', onKeydown)
})

// Make sure body scroll is reset if the component unmounts mid-open
watch(isOpen, (open) => {
  if (!open && typeof document !== 'undefined') {
    document.body.style.overflow = ''
  }
})
</script>

<style scoped>
.reg-modal {
  position: fixed;
  inset: 0;
  z-index: 1000;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: clamp(16px, 3vw, 32px);
  background: rgba(6, 25, 40, 0.78);
  backdrop-filter: blur(14px) saturate(140%);
  -webkit-backdrop-filter: blur(14px) saturate(140%);
}

.reg-modal__panel {
  position: relative;
  width: 100%;
  max-width: 960px;
  max-height: 90vh;
  display: flex;
  flex-direction: column;
  background: linear-gradient(180deg,
    rgba(15, 47, 68, 0.96) 0%,
    rgba(9, 48, 85, 0.96) 100%);
  border: 1px solid rgba(255, 255, 255, 0.18);
  border-radius:
    clamp(20px, 2.4vw, 28px) clamp(20px, 2.4vw, 28px)
    clamp(20px, 2.4vw, 28px) clamp(48px, 6vw, 72px);
  box-shadow: 0 32px 80px rgba(0, 0, 0, 0.55);
  overflow: hidden;
  isolation: isolate;
  font-family: var(--font-family);
  color: #fff;
}

/* Decorative cyan corner glow */
.reg-modal__panel::before {
  content: '';
  position: absolute;
  pointer-events: none;
  bottom: 0;
  left: 0;
  width: clamp(220px, 24vw, 320px);
  height: clamp(220px, 24vw, 320px);
  z-index: 0;
  background: radial-gradient(circle at bottom left,
    rgba(0, 173, 239, 0.30) 0%,
    rgba(0, 173, 239, 0.10) 40%,
    transparent 70%);
  border-radius: 0 clamp(48px, 6vw, 72px) 0 0;
}

.reg-modal__head {
  position: relative;
  z-index: 2;
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 12px;
  padding: clamp(20px, 2.4vw, 28px) clamp(24px, 3vw, 36px) 0;
}

.reg-modal__title {
  font-size: clamp(20px, 2.4vw, 26px);
  font-weight: 700;
  line-height: 1.2;
  letter-spacing: -0.01em;
  margin: 0;
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.reg-modal__subtitle {
  font-size: clamp(12px, 1vw, 13px);
  font-weight: 600;
  letter-spacing: 0.04em;
  color: var(--c-brand-cyan);
}

.reg-modal__close {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 38px;
  height: 38px;
  background: rgba(255, 255, 255, 0.08);
  border: 1px solid rgba(255, 255, 255, 0.22);
  border-radius: 50%;
  color: #fff;
  cursor: pointer;
  transition: background 0.2s ease, border-color 0.2s ease, transform 0.2s ease;
  flex-shrink: 0;
}

.reg-modal__close:hover {
  background: rgba(229, 45, 39, 0.20);
  border-color: rgba(229, 45, 39, 0.55);
  transform: rotate(90deg);
}

.reg-modal__close svg {
  width: 18px;
  height: 18px;
}

.reg-modal__body {
  position: relative;
  z-index: 2;
  flex: 1 1 auto;
  overflow-y: auto;
  padding: clamp(20px, 2.4vw, 28px) clamp(24px, 3vw, 36px) clamp(24px, 3vw, 36px);
  scrollbar-width: thin;
  scrollbar-color: rgba(255, 255, 255, 0.25) transparent;
}

.reg-modal__body::-webkit-scrollbar { width: 6px; }
.reg-modal__body::-webkit-scrollbar-thumb {
  background: rgba(255, 255, 255, 0.25);
  border-radius: 999px;
}

/* ── Enter / leave transition ───────────────────────────────────────────── */
.reg-modal-enter-active,
.reg-modal-leave-active {
  transition: opacity 0.25s ease;
}
.reg-modal-enter-active .reg-modal__panel,
.reg-modal-leave-active .reg-modal__panel {
  transition: opacity 0.25s ease, transform 0.35s cubic-bezier(0.22, 1, 0.36, 1);
}
.reg-modal-enter-from,
.reg-modal-leave-to {
  opacity: 0;
}
.reg-modal-enter-from .reg-modal__panel,
.reg-modal-leave-to .reg-modal__panel {
  opacity: 0;
  transform: scale(0.96) translateY(12px);
}

/* ── Mobile ─────────────────────────────────────────────────────────────── */
@media (max-width: 720px) {
  .reg-modal {
    padding: 0;
  }

  .reg-modal__panel {
    max-width: none;
    max-height: 100vh;
    height: 100vh;
    border-radius: 0;
  }
}
</style>
