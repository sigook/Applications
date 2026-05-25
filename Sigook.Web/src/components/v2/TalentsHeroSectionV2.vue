<template>
  <section class="talents-hero-v2">
    <!-- Atmospheric magnifier decoration — center-right anchor
         (Home top-left, About top-right, SP bottom-right, Industries bottom-left) -->
    <DecoMagnifierV2 class="talents-hero-v2__magnifier" />

    <div class="talents-hero-v2__content">
      <EyebrowPillV2 variant="cyan" class="talents-hero-v2__eyebrow">
        Talents
      </EyebrowPillV2>

      <h1 class="talents-hero-v2__heading">
        Your career.
        <span class="talents-hero-v2__heading-accent">Your way.</span>
      </h1>

      <p class="talents-hero-v2__subtitle">
        At Sigook we connect skilled people with top employers — matching your
        ambition with opportunities to grow, earn, and build a career that
        actually fits the life you want.
      </p>

      <LabeledChipListV2
        label="We recruit for"
        :items="ROLE_TYPES"
        class="talents-hero-v2__roles"
      />
    </div>

    <ScrollIndicatorV2 href="#talents-solutions" class="talents-hero-v2__scroll" />
  </section>
</template>

<script setup lang="ts">
/**
 * Talents hero — window-style intro that lets GlobalBackground show through,
 * matching the editorial rhythm of the other V2 hero sections.
 *
 * Differentiates via:
 *  • Magnifier anchor: center-right (last unused position in the family of
 *    Home top-left, About top-right, SP bottom-right, Industries bottom-left)
 *  • Cyan eyebrow → matches the talent-focused tone
 *  • Role-types chip list replaces the Figma's segmented Professional /
 *    Industrial toggle — same intent, vocabulary that scales with the system
 */
import DecoMagnifierV2 from '@/components/v2/shared/DecoMagnifierV2.vue'
import EyebrowPillV2 from '@/components/v2/shared/EyebrowPillV2.vue'
import LabeledChipListV2 from '@/components/v2/shared/LabeledChipListV2.vue'
import ScrollIndicatorV2 from '@/components/v2/shared/ScrollIndicatorV2.vue'

const ROLE_TYPES = [
  'Professional roles',
  'Industrial roles',
] as const
</script>

<style scoped>
/* ── Section shell — transparent (GlobalBackground shows through) ───────── */
.talents-hero-v2 {
  position: relative;
  width: 100%;
  height: auto;
  min-height: max(100vh, 1080px);
  overflow: hidden;
  isolation: isolate;
}

/* ── Decorative magnifier — center-right anchor ─────────────────────────── */
.talents-hero-v2__magnifier {
  top: clamp(28%, 30vw, 36%);
  right: clamp(6%, 7vw, 10%);
}

/* ── Content stack — vertically centered editorial layout ───────────────── */
.talents-hero-v2__content {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: max(100vh, 1080px);
  padding:
    clamp(80px, 12vw, 140px)
    clamp(20px, 3vw, 32px)
    clamp(120px, 16vw, 220px);
  text-align: center;
  max-width: 1100px;
  margin: 0 auto;
}

.talents-hero-v2__eyebrow {
  margin-bottom: clamp(24px, 3.5vw, 36px);
}

/* ── Main heading — large editorial with cyan accent ────────────────────── */
.talents-hero-v2__heading {
  font-family: var(--font-family);
  font-size: clamp(32px, 5.5vw, 60px);
  font-weight: 700;
  line-height: 1.05;
  letter-spacing: -0.02em;
  color: #fff;
  margin: 0 0 clamp(24px, 3.5vw, 36px);
  max-width: 920px;
  text-shadow: 0 4px 24px rgba(0, 0, 0, 0.40);
}

.talents-hero-v2__heading-accent {
  color: var(--c-brand-cyan);
  display: block;
}

/* ── Subtitle / value prop ──────────────────────────────────────────────── */
.talents-hero-v2__subtitle {
  font-family: var(--font-family);
  font-size: clamp(14px, 1.4vw, 17px);
  font-weight: 400;
  line-height: 1.65;
  color: rgba(255, 255, 255, 0.85);
  margin: 0 0 clamp(44px, 6vw, 64px);
  max-width: 680px;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.35);
}

/* ── Scroll indicator ───────────────────────────────────────────────────── */
.talents-hero-v2__scroll {
  position: absolute;
  bottom: clamp(20px, 3vw, 36px);
  left: 50%;
  transform: translateX(-50%);
  z-index: 2;
}

/* ── Mobile-only behaviors (clamp can't express conditional layout) ─────── */
@media (max-width: 1023px) {
  .talents-hero-v2 {
    min-height: 100svh;
  }

  .talents-hero-v2__content {
    min-height: 100svh;
  }

  .talents-hero-v2__scroll {
    display: none;
  }

  .talents-hero-v2__magnifier {
    top: 28%;
    right: 6%;
  }
}
</style>
