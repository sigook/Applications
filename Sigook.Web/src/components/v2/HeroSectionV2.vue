<template>
  <section class="hero-v2">
    <!-- Atmospheric magnifier decoration -->
    <DecoMagnifierV2 class="hero-v2__magnifier" />

    <div class="hero-v2__content">
      <EyebrowPillV2 variant="red" class="hero-v2__eyebrow">
        Workforce Platform
      </EyebrowPillV2>

      <img
        src="@/assets/images/v2/footer/footer-logo.png"
        alt="Sigook Work Factory"
        class="hero-v2__logo"
      />

      <h1 class="hero-v2__heading">
        Where great talent meets
        <span class="hero-v2__heading-accent">great opportunities</span>
      </h1>

      <p class="hero-v2__subtitle">
        Sigook connects North America's leading employers with skilled workers —
        from onboarding and timesheets to payroll, fully connected in one platform.
      </p>

      <div class="hero-v2__industries">
        <span class="hero-v2__industries-label">Trusted across</span>
        <div class="hero-v2__industries-chips">
          <span
            v-for="industry in INDUSTRIES"
            :key="industry"
            class="hero-v2__industry"
          >{{ industry }}</span>
          <span class="hero-v2__industry hero-v2__industry--more">+ more</span>
        </div>
      </div>
    </div>

    <ScrollIndicatorV2 href="#dual-cta" class="hero-v2__scroll" />
  </section>
</template>

<script setup lang="ts">
import DecoMagnifierV2 from '@/components/v2/shared/DecoMagnifierV2.vue'
import EyebrowPillV2 from '@/components/v2/shared/EyebrowPillV2.vue'
import ScrollIndicatorV2 from '@/components/v2/shared/ScrollIndicatorV2.vue'

const INDUSTRIES = [
  'Manufacturing',
  'Logistics',
  'Healthcare',
  'Retail',
  'Construction',
] as const
</script>

<style scoped>
/* ── Section shell — transparent (GlobalBackground shows through) ───────── */
.hero-v2 {
  position: relative;
  width: 100%;
  height: auto;
  min-height: max(100vh, 1080px);
  overflow: hidden;
  isolation: isolate;
}

/* ── Decorative magnifier — top-left anchor ─────────────────────────────── */
.hero-v2__magnifier {
  top: 18%;
  left: 7%;
}

/* ── Content stack — vertically centered editorial layout ───────────────── */
.hero-v2__content {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: max(100vh, 1080px);
  /* Top padding clears the 80px fixed navbar + 60px buffer.
     Bottom padding clears the DualCta overlap (-140px) + 80px buffer. */
  padding: 140px 24px 220px;
  text-align: center;
  max-width: 1100px;
  margin: 0 auto;
}

/* Spacing between stack elements lives here — atoms own their own visual */
.hero-v2__eyebrow   { margin-bottom: 36px; }

.hero-v2__logo {
  display: block;
  width: auto;
  height: 96px;
  max-width: 280px;
  object-fit: contain;
  margin: 0 auto 44px;
  filter: drop-shadow(0 8px 24px rgba(0, 0, 0, 0.30));
}

/* ── Main heading — large editorial with cyan accent ────────────────────── */
.hero-v2__heading {
  font-family: var(--font-family);
  font-size: 60px;
  font-weight: 700;
  line-height: 1.05;
  letter-spacing: -0.02em;
  color: #fff;
  margin: 0 0 36px;
  max-width: 920px;
  text-shadow: 0 4px 24px rgba(0, 0, 0, 0.40);
}

.hero-v2__heading-accent {
  color: var(--c-brand-cyan);
  white-space: nowrap;
}

/* ── Subtitle / value prop ──────────────────────────────────────────────── */
.hero-v2__subtitle {
  font-family: var(--font-family);
  font-size: 17px;
  font-weight: 400;
  line-height: 1.65;
  color: rgba(255, 255, 255, 0.85);
  margin: 0 0 64px;
  max-width: 640px;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.35);
}

/* ── Industries served — chips list ─────────────────────────────────────── */
.hero-v2__industries {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 14px;
}

.hero-v2__industries-label {
  font-family: var(--font-family);
  font-size: 11px;
  font-weight: 600;
  line-height: 1.2;
  color: rgba(255, 255, 255, 0.55);
  letter-spacing: 0.22em;
  text-transform: uppercase;
}

.hero-v2__industries-chips {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 10px;
  max-width: 720px;
}

.hero-v2__industry {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 8px 18px;
  background: rgba(255, 255, 255, 0.06);
  border: 1px solid rgba(255, 255, 255, 0.18);
  border-radius: 999px;
  backdrop-filter: blur(10px) saturate(140%);
  -webkit-backdrop-filter: blur(10px) saturate(140%);
  font-family: var(--font-family);
  font-size: 13px;
  font-weight: 500;
  line-height: 1;
  color: rgba(255, 255, 255, 0.88);
  letter-spacing: 0.02em;
  transition: background 0.25s ease, border-color 0.25s ease, transform 0.25s ease;
}

.hero-v2__industry::before {
  content: '';
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background: var(--c-brand-cyan);
  flex-shrink: 0;
}

.hero-v2__industry:hover {
  background: rgba(255, 255, 255, 0.10);
  border-color: rgba(255, 255, 255, 0.30);
  transform: translateY(-1px);
}

/* Overflow indicator — muted variant of the chip */
.hero-v2__industry--more {
  background: transparent;
  border-color: rgba(255, 255, 255, 0.14);
  color: rgba(255, 255, 255, 0.60);
}

.hero-v2__industry--more::before {
  background: rgba(255, 255, 255, 0.40);
}

/* ── Scroll indicator — absolute positioning lives on the parent class ──── */
.hero-v2__scroll {
  position: absolute;
  bottom: 36px;
  left: 50%;
  transform: translateX(-50%);
  z-index: 2;
}

/* ── Mobile ─────────────────────────────────────────────────────────────── */
@media (max-width: 1023px) {
  .hero-v2 {
    height: 100svh;
    min-height: 640px;
  }

  .hero-v2__content {
    padding: 80px 24px 120px;
  }

  .hero-v2__eyebrow { margin-bottom: 28px; }

  .hero-v2__logo {
    height: 64px;
    max-width: 200px;
    margin-bottom: 32px;
  }

  .hero-v2__heading {
    font-size: 32px;
    margin-bottom: 24px;
  }

  .hero-v2__subtitle {
    font-size: 14px;
    margin-bottom: 44px;
  }

  .hero-v2__industries { gap: 10px; }

  .hero-v2__industries-chips {
    gap: 8px;
    max-width: 100%;
  }

  .hero-v2__industry {
    font-size: 12px;
    padding: 6px 14px;
  }

  .hero-v2__industry::before {
    width: 5px;
    height: 5px;
  }

  .hero-v2__magnifier {
    top: 14%;
    left: 6%;
  }

  .hero-v2__scroll { display: none; }
}
</style>
