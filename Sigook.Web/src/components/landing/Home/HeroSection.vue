<template>
  <section class="hero">
    <HeroBackground :image="heroImage" focal="center 28%" />

    <DecoMagnifier class="hero__magnifier" />

    <div class="hero__content">
      <EyebrowPill variant="red" class="hero__eyebrow">
        Workforce Platform
      </EyebrowPill>

      <img
        :src="logoWhite"
        alt="Sigook Work Factory"
        class="hero__logo"
      />

      <h1 class="hero__heading">
        Where great talent meets
        <span class="hero__heading-accent">great opportunities</span>
      </h1>

      <p class="hero__subtitle">
        Sigook® connects leading U.S. employers with skilled workers
        from onboarding and timesheets to payroll, fully connected in one platform.
      </p>

      <LabeledChipList
        label="Trusted across"
        :items="INDUSTRIES"
        more-label="+ more"
        more-to="/industries"
        class="hero__industries"
      />
    </div>

    <ScrollIndicator href="#dual-cta" class="hero__scroll" />
  </section>
</template>

<script setup lang="ts">
import DecoMagnifier from '@/components/landing/shared/DecoMagnifier.vue'
import HeroBackground from '@/components/landing/shared/HeroBackground.vue'
import EyebrowPill from '@/components/landing/shared/EyebrowPill.vue'
import LabeledChipList from '@/components/landing/shared/LabeledChipList.vue'
import ScrollIndicator from '@/components/landing/shared/ScrollIndicator.vue'
import logoWhite from '@/assets/images/logo-white-v2.png'
import heroImage from '@/assets/images/v2/hero/home.jpg'

const INDUSTRIES = [
  'Cibersecurity',
  'Artificial Intelligence',
  'Manufacturing',
  'Logistics',
  'Retail',
  'Construction',
] as const
</script>

<style scoped>
/* ── Section shell — transparent (GlobalBackground shows through) ───────── */
.hero {
  position: relative;
  width: 100%;
  height: auto;
  min-height: max(100vh, 1080px);
  overflow: hidden;
  isolation: isolate;
}

.hero__magnifier {
  top: clamp(14%, 16vw, 18%);
  left: clamp(6%, 7vw, 9%);
}

/* ── Content stack — vertically centered editorial layout ───────────────── */
.hero__content {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: max(100vh, 1080px);
  /* Top padding clears the fixed navbar + buffer.
     Bottom padding clears the DualCta overlap + buffer. */
  padding:
    clamp(80px, 12vw, 140px)
    clamp(20px, 3vw, 32px)
    clamp(120px, 16vw, 220px);
  text-align: center;
  max-width: 1100px;
  margin: 0 auto;
}

/* Spacing between stack elements lives here — atoms own their own visual */
.hero__eyebrow {
  margin-bottom: clamp(24px, 3.5vw, 36px);
}

.hero__logo {
  display: block;
  width: auto;
  height: clamp(64px, 8vw, 96px);
  max-width: clamp(200px, 22vw, 280px);
  object-fit: contain;
  margin: 0 auto clamp(28px, 4vw, 44px);
  filter: drop-shadow(0 10px 30px rgba(0, 0, 0, 0.45));
}

/* ── Main heading — large editorial with cyan accent ────────────────────── */
.hero__heading {
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

.hero__heading-accent {
  color: var(--c-brand-cyan);
  white-space: nowrap;
}

/* ── Subtitle / value prop ──────────────────────────────────────────────── */
.hero__subtitle {
  font-family: var(--font-family);
  font-size: clamp(14px, 1.4vw, 17px);
  font-weight: 400;
  line-height: 1.65;
  color: rgba(255, 255, 255, 0.85);
  margin: 0 0 clamp(44px, 6vw, 64px);
  max-width: 640px;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.35);
}

/* ── Scroll indicator — absolute positioning lives on the parent class ──── */
.hero__scroll {
  position: absolute;
  bottom: clamp(20px, 3vw, 36px);
  left: 50%;
  transform: translateX(-50%);
  z-index: 2;
}

/* ── Mobile-only behaviors (clamp can't express conditional layout) ─────── */
@media (max-width: 1023px) {
  .hero {
    min-height: 100svh;
  }

  .hero__content {
    min-height: 100svh;
  }

  .hero__scroll {
    display: none;
  }
}
</style>
