<template>
  <teleport to="body">
    <div
      v-if="isOpen"
      class="reg-modal"
      role="dialog"
      aria-modal="true"
      aria-labelledby="candidate-modal-title"
      @click.self="onBackdropClick"
    >
      <div ref="panelRef" class="reg-modal__panel" role="document">
        <header class="reg-modal__head">
          <h2 id="candidate-modal-title" class="reg-modal__title">
            {{ title }}
            <span v-if="subtitle" class="reg-modal__subtitle">{{ subtitle }}</span>
          </h2>
          <button
            type="button"
            class="reg-modal__close"
            aria-label="Close application dialog"
            @click="close"
          >
<LandingIcon name="close" />
          </button>
        </header>

        <div class="reg-modal__body">
          <CandidateApplyForm
            :job-title="context?.jobTitle"
            :request-id="context?.requestId"
            :redirect-on-success="false"
            @submitted="onSubmitted"
          />
        </div>
      </div>
    </div>
  </teleport>
</template>

<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref } from 'vue'
import CandidateApplyForm from '@/components/landing/shared/forms/CandidateApplyForm.vue'
import LandingIcon from '@/components/landing/shared/LandingIcon.vue'
import { useCandidateApplyModal } from '@/components/landing/shared/forms/useCandidateApplyModal'
import { useFocusTrap } from '@/composables/useFocusTrap'

const { isOpen, context, close } = useCandidateApplyModal()

const title = computed(() => (context.value?.jobTitle ? 'Apply for this role' : 'Register with us'))
const subtitle = computed(() => context.value?.jobTitle ?? null)

const panelRef = ref<HTMLElement | null>(null)
useFocusTrap(isOpen, panelRef)

let closeTimer: ReturnType<typeof setTimeout> | null = null

function onBackdropClick(): void {
  close()
}

function onSubmitted(): void {
  if (closeTimer) clearTimeout(closeTimer)
  closeTimer = setTimeout(() => close(), 800)
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
  if (closeTimer) clearTimeout(closeTimer)
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
