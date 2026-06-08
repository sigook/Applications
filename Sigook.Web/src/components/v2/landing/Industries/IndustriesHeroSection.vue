<template>
  <section class="industries-hero">
    <HeroBackground :image="heroImage" focal="center 35%" />
    <!-- Atmospheric magnifier decoration — bottom-left anchor (only quadrant
         not used yet across Home top-left, About top-right, SP bottom-right) -->
    <DecoMagnifier class="industries-hero__magnifier" />

    <div class="industries-hero__content">
      <EyebrowPill variant="cyan" class="industries-hero__eyebrow">
        Our Areas of Expertise
      </EyebrowPill>

      <h1 class="industries-hero__heading">
        Eleven sectors.
        <span class="industries-hero__heading-accent">Endless possibilities.</span>
      </h1>

      <p class="industries-hero__subtitle">
        We recruit highly qualified professionals for specialized positions
        across the industries that power North America's economy — pairing
        deep sector knowledge with long-term, measurable impact.
      </p>

      <LabeledChipList
        label="Sectors we serve"
        :items="PREVIEW_INDUSTRIES"
        more-label="+ 6 more"
        class="industries-hero__chips"
      />
    </div>

    <ScrollIndicator href="#industries-grid" class="industries-hero__scroll" />
  </section>
</template>

<script setup lang="ts">
/**
 * Industries hero — window-style intro (transparent over GlobalBackground).
 * Mirrors the editorial rhythm of HeroSection (Home), AboutIntroSection
 * and SpecialProjectsHeroSection.
 *
 * Differentiates via:
 *  • Magnifier anchor: bottom-left (4th and final quadrant for the family)
 *  • Cyan eyebrow → signals expertise/breadth
 *  • Preview chips show 5 of 11 sectors + "+6 more" indicator — invites
 *    the user to scroll to the full grid panel below.
 */
import DecoMagnifier from '@/components/v2/landing/shared/DecoMagnifier.vue'
import HeroBackground from '@/components/v2/landing/shared/HeroBackground.vue'
import heroImage from '@/assets/images/v2/hero/industries.jpg'
import EyebrowPill from '@/components/v2/landing/shared/EyebrowPill.vue'
import LabeledChipList from '@/components/v2/landing/shared/LabeledChipList.vue'
import ScrollIndicator from '@/components/v2/landing/shared/ScrollIndicator.vue'

// 5 most representative sectors — the full 11 live in the grid section below
const PREVIEW_INDUSTRIES = [
  'Manufacturing',
  'Construction',
  'Logistics',
  'Engineering',
  'Technology',
] as const
</script>

<style scoped>
/* ── Section shell — transparent (GlobalBackground shows through) ───────── */
.industries-hero {
  position: relative;
  width: 100%;
  height: auto;
  min-height: max(100vh, 1080px);
  overflow: hidden;
  isolation: isolate;
}

/* ── Decorative magnifier — bottom-left anchor ──────────────────────────── */
.industries-hero__magnifier {
  bottom: clamp(12%, 14vw, 18%);
  left: clamp(6%, 7vw, 10%);
}

/* ── Content stack — vertically centered editorial layout ───────────────── */
.industries-hero__content {
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

.industries-hero__eyebrow {
  margin-bottom: clamp(24px, 3.5vw, 36px);
}

/* ── Main heading — large editorial with cyan accent ────────────────────── */
.industries-hero__heading {
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

.industries-hero__heading-accent {
  color: var(--c-brand-cyan);
  display: block;
}

/* ── Subtitle / value prop ──────────────────────────────────────────────── */
.industries-hero__subtitle {
  font-family: var(--font-family);
  font-size: clamp(14px, 1.4vw, 17px);
  font-weight: 400;
  line-height: 1.65;
  color: rgba(255, 255, 255, 0.85);
  margin: 0 0 clamp(44px, 6vw, 64px);
  max-width: 680px;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.35);
}

/* ── Scroll indicator — absolute positioning lives on the parent class ──── */
.industries-hero__scroll {
  position: absolute;
  bottom: clamp(20px, 3vw, 36px);
  left: 50%;
  transform: translateX(-50%);
  z-index: 2;
}

/* ── Mobile-only behaviors (clamp can't express conditional layout) ─────── */
@media (max-width: 1023px) {
  .industries-hero {
    min-height: 100svh;
  }

  .industries-hero__content {
    min-height: 100svh;
  }

  .industries-hero__scroll {
    display: none;
  }

  .industries-hero__magnifier {
    bottom: 14%;
    left: 6%;
  }
}
</style>
