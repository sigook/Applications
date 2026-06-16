<template>
  <div class="industries-carousel-panel">
    <!-- Brand panel (top-left + bottom-right rounded) behind the carousel -->
    <div class="industries-carousel-panel__surface" aria-hidden="true"></div>
    <slot />
  </div>
</template>

<script setup lang="ts">
/**
 * IndustriesCarouselPanel — brand panel wrapper for the shared
 * IndustriesCarousel.
 *
 * The shared carousel is intentionally background-less so it stays reusable.
 * Pages that want the brand panel rhythm wrap their carousel in this at the
 * page level — the asymmetric rounded rectangle, the glass surface and the
 * overlap margin live here, not in the carousel component itself.
 */
</script>

<style scoped>
/* ── Brand panel shell — owns the overlap + the asymmetric rounded shape ── */
.industries-carousel-panel {
  position: relative;
  margin-top: clamp(-180px, -10vw, -80px);
  z-index: 5;
  isolation: isolate;
  border-radius:
    clamp(80px, 10vw, 150px) 0
    clamp(80px, 10vw, 150px) 0;
  box-shadow:
    0 -22px 40px -12px rgba(0, 0, 0, 0.45),
    0  22px 40px -12px rgba(0, 0, 0, 0.45);
}

/* Depth back-layer — transparent glass, peeks ~16px top & bottom */
.industries-carousel-panel::before {
  content: '';
  position: absolute;
  top: -16px;
  bottom: -16px;
  left: 0;
  right: 0;
  z-index: -1;
  border-radius:
    clamp(80px, 10vw, 150px) 0
    clamp(80px, 10vw, 150px) 0;
  background: rgba(255, 255, 255, 0.07);
  backdrop-filter: blur(10px) saturate(120%);
  -webkit-backdrop-filter: blur(10px) saturate(120%);
  border: 1px solid rgba(255, 255, 255, 0.14);
  box-shadow: 0 18px 40px -20px rgba(0, 0, 0, 0.4);
  pointer-events: none;
}

/* Glass surface clipped to the brand shape (top-left + bottom-right rounded) */
.industries-carousel-panel__surface {
  position: absolute;
  inset: 0;
  z-index: 0;
  border-radius:
    clamp(80px, 10vw, 150px) 0
    clamp(80px, 10vw, 150px) 0;
  background: linear-gradient(
    180deg,
    rgba(9, 48, 85, 0.65) 0%,
    rgba(9, 48, 85, 0.55) 100%
  );
  backdrop-filter: blur(20px) saturate(160%);
  -webkit-backdrop-filter: blur(20px) saturate(160%);
  pointer-events: none;
}

/* The wrapper now owns the overlap margin; restore panel-era top padding so
   the header isn't cramped against the rounded top edge. */
.industries-carousel-panel :deep(.industries-carousel) {
  margin-top: 0;
  padding-top: clamp(140px, 14vw, 200px);
}
</style>
