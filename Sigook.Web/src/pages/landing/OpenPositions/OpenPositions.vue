<template>
  <main class="op-page">
    <OpenPositionsHeroSection />
    <OpenPositionsListSection />
    <OpenPositionsBrowseSection />
    <OpenPositionsCtaSection />

    <div id="openpos-contact">
      <ContactSection />
    </div>
    <WorkerRegisterModal />
  </main>
</template>

<script setup lang="ts">

import OpenPositionsHeroSection from '@/components/landing/OpenPositions/OpenPositionsHeroSection.vue'
import OpenPositionsListSection from '@/components/landing/OpenPositions/OpenPositionsListSection.vue'
import OpenPositionsBrowseSection from '@/components/landing/OpenPositions/OpenPositionsBrowseSection.vue'
import OpenPositionsCtaSection from '@/components/landing/OpenPositions/OpenPositionsCtaSection.vue'
import ContactSection from '@/components/landing/shared/ContactSection.vue'
import WorkerRegisterModal from '@/components/landing/shared/forms/WorkerRegisterModal.vue'
</script>

<style scoped>
.op-page :deep(.industries-carousel) {
  margin-top: clamp(8px, 1.5vw, 24px);
  padding-top: clamp(48px, 7vw, 88px);
  padding-bottom: clamp(48px, 7vw, 88px);
  border-radius: 0;
  box-shadow: none;
}

.op-page :deep(.industries-carousel__surface) {
  display: none;
}

.op-page :deep(.industries-carousel__viewport) {
  padding: clamp(24px, 3.2vw, 40px) clamp(6px, 1vw, 12px);
}

.op-page :deep(.tertiary-card) {
  min-height: clamp(154px, 16vw, 194px);
  gap: clamp(12px, 1.4vw, 16px);
  transition:
    opacity 0.6s ease-out,
    transform 0.4s cubic-bezier(0.22, 1, 0.36, 1),
    border-color 0.3s ease,
    box-shadow 0.3s ease;
}

.op-page :deep(.tertiary-card--visible:hover) {
  transform: translateY(-6px) scale(1.02);
  border-color: rgba(0, 173, 239, 0.5);
  box-shadow:
    0 20px 38px -14px rgba(0, 0, 0, 0.45),
    0 12px 30px -8px rgba(0, 173, 239, 0.45);
}

.op-page :deep(.tertiary-card__icon) {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: clamp(56px, 6vw, 72px);
  height: clamp(56px, 6vw, 72px);
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.22);
  box-shadow:
    inset 0 1px 0 rgba(255, 255, 255, 0.18),
    0 8px 18px rgba(0, 0, 0, 0.2);
  font-size: clamp(22px, 2.6vw, 30px);
  transition:
    transform 0.4s cubic-bezier(0.22, 1, 0.36, 1),
    background 0.3s ease,
    border-color 0.3s ease;
}

.op-page :deep(.tertiary-card__icon svg) {
  width: 1em;
  height: 1em;
}

.op-page :deep(.tertiary-card--visible:hover .tertiary-card__icon) {
  transform: scale(1.08);
  background: rgba(0, 173, 239, 0.18);
  border-color: rgba(0, 173, 239, 0.4);
}

.op-page :deep(.industries-carousel__nav) {
  width: clamp(44px, 4.4vw, 56px);
  height: clamp(44px, 4.4vw, 56px);
  background: rgba(255, 255, 255, 0.1);
  border-color: rgba(255, 255, 255, 0.28);
  backdrop-filter: blur(10px) saturate(150%);
  -webkit-backdrop-filter: blur(10px) saturate(150%);
  box-shadow: 0 10px 24px rgba(0, 0, 0, 0.28);
}
</style>
