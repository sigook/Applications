<template>
  <img
    src="@/assets/images/v2/branding/sigook-magnifier.webp"
    alt=""
    aria-hidden="true"
    class="deco-magnifier"
  />
</template>

<script setup lang="ts">
/**
 * Floating brand magnifier — decorative element used across Hero,
 * WhyChooseUs hero, Testimonials, and Certified.
 *
 * Position is set by the parent via fall-through attributes (style or class).
 * The parent should provide `top/left/right/bottom` via inline style or its
 * own scoped CSS targeting `.deco-magnifier`.
 *
 * Animation (translate + rotate) lives here so all instances move identically.
 */
</script>

<style scoped>
.deco-magnifier {
  position: absolute;
  width: 88px;
  height: 88px;
  z-index: 1;
  pointer-events: none;
  filter: drop-shadow(0 8px 16px rgba(0, 0, 0, 0.30));
  animation: magnifier-float 6.5s ease-in-out infinite;
  will-change: transform;
}

@keyframes magnifier-float {
  0%, 100% { transform: translate(0, 0) rotate(-6deg); }
  25%      { transform: translate(6px, -8px) rotate(4deg); }
  50%      { transform: translate(0, -14px) rotate(8deg); }
  75%      { transform: translate(-6px, -8px) rotate(-4deg); }
}

@media (prefers-reduced-motion: reduce) {
  .deco-magnifier { animation: none; }
}

@media (max-width: 1023px) {
  .deco-magnifier {
    width: 56px;
    height: 56px;
  }
}
</style>
