<template>
  <div class="industries-carousel-panel">
    <div class="industries-carousel-panel__surface" aria-hidden="true"></div>
    <slot />
  </div>
</template>

<script setup lang="ts">
</script>

<style scoped>
.industries-carousel-panel {
  position: relative;
  margin-top: var(--panel-overlap);
  z-index: 5;
  isolation: isolate;
  border-radius:
    var(--r-brand-fluid) 0
    var(--r-brand-fluid) 0;
  box-shadow:
    0 -22px 40px -12px rgba(0, 0, 0, 0.45),
    0  22px 40px -12px rgba(0, 0, 0, 0.45);
}

.industries-carousel-panel::before {
  content: '';
  position: absolute;
  top: -16px;
  bottom: -16px;
  left: 0;
  right: 0;
  z-index: -1;
  border-radius:
    var(--r-brand-fluid) 0
    var(--r-brand-fluid) 0;
  background: rgba(255, 255, 255, 0.07);
  backdrop-filter: var(--glass-blur-soft);
  -webkit-backdrop-filter: var(--glass-blur-soft);
  border: 1px solid var(--c-glass-border-soft);
  box-shadow: var(--sh-back);
  pointer-events: none;
}

.industries-carousel-panel__surface {
  position: absolute;
  inset: 0;
  z-index: 0;
  border-radius:
    var(--r-brand-fluid) 0
    var(--r-brand-fluid) 0;
  background: linear-gradient(
    180deg,
    rgba(9, 48, 85, 0.65) 0%,
    rgba(9, 48, 85, 0.55) 100%
  );
  backdrop-filter: blur(20px) saturate(160%);
  -webkit-backdrop-filter: blur(20px) saturate(160%);
  pointer-events: none;
}

.industries-carousel-panel :deep(.industries-carousel) {
  margin-top: 0;
  padding-top: var(--section-pad-y-lg);
}

@media (max-width: 1023px) {
  .industries-carousel-panel {
    box-shadow:
      0 -22px 24px -12px rgba(0, 0, 0, 0.5),
      0  22px 24px -12px rgba(0, 0, 0, 0.5);
  }

  .industries-carousel-panel::before {
    backdrop-filter: none;
    -webkit-backdrop-filter: none;
    background: var(--c-glass-fill-active);
  }

  .industries-carousel-panel__surface {
    backdrop-filter: none;
    -webkit-backdrop-filter: none;
  }
}
</style>
