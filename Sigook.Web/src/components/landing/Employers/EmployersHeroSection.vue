<template>
  <section class="employers-hero">
    <HeroBackground :image="heroImage" :image-sm="heroImageSm" focal="center 40%" />
    <!-- Atmospheric magnifier decoration — center-left anchor
         (Home top-left, About top-right, SP bottom-right, Industries
          bottom-left, Talents center-right → Employers center-left closes
          the symmetry: talent looks outward, employer looks inward) -->
    <DecoMagnifier class="employers-hero__magnifier" />

    <div class="employers-hero__content">
      <EyebrowPill variant="red" class="employers-hero__eyebrow">
        For Employers
      </EyebrowPill>

      <h1 class="employers-hero__heading">
        Your next great <span class="employers-hero__heading-accent">hire.</span><br>
        <span class="employers-hero__heading-accent">Found.</span>
      </h1>

      <p class="employers-hero__subtitle">
        We help companies recruit highly qualified professionals for
        specialized positions — turning hiring from a months-long search
        into a confident, measurable decision.
      </p>

      <LabeledChipList
        label="We staff for"
        :items="STAFFING_TYPES"
        class="employers-hero__roles"
      />
    </div>

    <ScrollIndicator href="#employers-solutions" class="employers-hero__scroll" />
  </section>
</template>

<script setup lang="ts">
/**
 * Employers hero — window-style intro that lets GlobalBackground show
 * through, matching the editorial rhythm of the other V2 hero sections.
 *
 * Differentiates from Talents (its closest sibling) via:
 *  • Magnifier anchor: center-left (Talents uses center-right) — visual
 *    pairing between the two audience pages
 *  • Red eyebrow → business / urgency tone vs Talents' cyan / optimism
 *  • Copy speaks from the employer side: hiring outcomes, not career outcomes
 */
import DecoMagnifier from '@/components/landing/shared/DecoMagnifier.vue'
import HeroBackground from '@/components/landing/shared/HeroBackground.vue'
import heroImage from '@/assets/images/v2/hero/employers.webp'
import heroImageSm from '@/assets/images/v2/hero/employers-960.webp'
import EyebrowPill from '@/components/landing/shared/EyebrowPill.vue'
import LabeledChipList from '@/components/landing/shared/LabeledChipList.vue'
import ScrollIndicator from '@/components/landing/shared/ScrollIndicator.vue'

const STAFFING_TYPES = [
  'Professional hires',
  'Industrial hires',
] as const
</script>

<style scoped>
/* ── Section shell — transparent (GlobalBackground shows through) ───────── */
.employers-hero {
  position: relative;
  width: 100%;
  height: auto;
  min-height: max(100vh, 1080px);
  overflow: hidden;
  isolation: isolate;
}

/* ── Decorative magnifier — center-left anchor ──────────────────────────── */
.employers-hero__magnifier {
  top: clamp(28%, 30vw, 36%);
  left: clamp(6%, 7vw, 10%);
}

/* ── Content stack — vertically centered editorial layout ───────────────── */
.employers-hero__content {
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

.employers-hero__eyebrow {
  margin-bottom: clamp(24px, 3.5vw, 36px);
}

/* ── Main heading — large editorial with red accent ─────────────────────── */
.employers-hero__heading {
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

.employers-hero__heading-accent {
  color: var(--c-brand-cyan);
}

/* ── Subtitle / value prop ──────────────────────────────────────────────── */
.employers-hero__subtitle {
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
.employers-hero__scroll {
  position: absolute;
  bottom: clamp(20px, 3vw, 36px);
  left: 50%;
  transform: translateX(-50%);
  z-index: 2;
}

/* ── Mobile-only behaviors (clamp can't express conditional layout) ─────── */
@media (max-width: 1023px) {
  .employers-hero {
    min-height: 100svh;
  }

  .employers-hero__content {
    min-height: 100svh;
  }

  .employers-hero__scroll {
    display: none;
  }

  .employers-hero__magnifier {
    top: 28%;
    left: 6%;
  }
}
</style>
