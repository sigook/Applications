<template>
  <section class="sp-hero">
    <HeroBackground :image="heroImage" focal="center 58%" />
    <!-- Atmospheric magnifier decoration — bottom-right anchor (differs from
         Home top-left and About Intro top-right placements) -->
    <DecoMagnifier class="sp-hero__magnifier" />

    <div class="sp-hero__content">
      <EyebrowPill variant="cyan" class="sp-hero__eyebrow">
        Special Projects
      </EyebrowPill>

      <h1 class="sp-hero__heading">
        Beyond staffing.
        <span class="sp-hero__heading-accent">Built for what's next.</span>
      </h1>

      <p class="sp-hero__subtitle">
        Where workforce development, specialized training, and innovative
        programs converge — strengthening local economies and creating
        opportunity across industries, communities, and public institutions.
      </p>

      <LabeledChipList
        label="Focus areas"
        :items="FOCUS_AREAS"
        class="sp-hero__focus"
      />
    </div>

    <ScrollIndicator href="#sp-focus" class="sp-hero__scroll" />
  </section>
</template>

<script setup lang="ts">
/**
 * Special Projects hero — window-style intro that lets GlobalBackground show
 * through, mirrors the editorial rhythm of HeroSection (Home) and
 * AboutIntroSection.
 *
 * Differentiates from those two via:
 *  • Magnifier anchor (bottom-right vs Home top-left / About top-right)
 *  • Cyan eyebrow (vs Home red / About red) — signals innovation/future tone
 *  • Focus areas chip list previews the next section (panel below)
 */
import DecoMagnifier from '@/components/landing/shared/DecoMagnifier.vue'
import HeroBackground from '@/components/landing/shared/HeroBackground.vue'
import heroImage from '@/assets/images/v2/hero/special-projects.jpg'
import EyebrowPill from '@/components/landing/shared/EyebrowPill.vue'
import LabeledChipList from '@/components/landing/shared/LabeledChipList.vue'
import ScrollIndicator from '@/components/landing/shared/ScrollIndicator.vue'

const FOCUS_AREAS = [
  'Workforce',
  'Training',
  'Community',
  'Innovation',
] as const
</script>

<style scoped>
/* ── Section shell — transparent (GlobalBackground shows through) ───────── */
.sp-hero {
  position: relative;
  width: 100%;
  height: auto;
  min-height: max(100vh, 1080px);
  overflow: hidden;
  isolation: isolate;
}

/* ── Decorative magnifier — bottom-right anchor ─────────────────────────── */
.sp-hero__magnifier {
  bottom: clamp(12%, 14vw, 18%);
  right: clamp(6%, 7vw, 10%);
}

/* ── Content stack — vertically centered editorial layout ───────────────── */
.sp-hero__content {
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

.sp-hero__eyebrow {
  margin-bottom: clamp(24px, 3.5vw, 36px);
}

/* ── Main heading — large editorial with cyan accent ────────────────────── */
.sp-hero__heading {
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

.sp-hero__heading-accent {
  color: var(--c-brand-cyan);
  display: block;
}

/* ── Subtitle / value prop ──────────────────────────────────────────────── */
.sp-hero__subtitle {
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
.sp-hero__scroll {
  position: absolute;
  bottom: clamp(20px, 3vw, 36px);
  left: 50%;
  transform: translateX(-50%);
  z-index: 2;
}

/* ── Mobile-only behaviors (clamp can't express conditional layout) ─────── */
@media (max-width: 1023px) {
  .sp-hero {
    min-height: 100svh;
  }

  .sp-hero__content {
    min-height: 100svh;
  }

  .sp-hero__scroll {
    display: none;
  }

  .sp-hero__magnifier {
    bottom: 14%;
    right: 6%;
  }
}
</style>
