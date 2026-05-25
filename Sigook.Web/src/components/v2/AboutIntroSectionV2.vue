<template>
  <section class="about-intro-v2">
    <!-- Atmospheric magnifier decoration — right-side anchor (mirrors Home Hero's left placement) -->
    <DecoMagnifierV2 class="about-intro-v2__magnifier" />

    <div class="about-intro-v2__content">
      <EyebrowPillV2 variant="red" class="about-intro-v2__eyebrow">
        Our Story
      </EyebrowPillV2>

      <h1 class="about-intro-v2__heading">
        Empowering people.
        <span class="about-intro-v2__heading-accent">Strengthening businesses.</span>
      </h1>

      <p class="about-intro-v2__subtitle">
        Since Florida in 2008, Sigook Work Factory has grown with one clear purpose —
        to bring people and opportunities together. Built on fairness, respect, and
        genuine care, we turn trust and integrity into lasting partnerships.
      </p>

      <!-- Credential line — historical footprint (replaces the legacy "Get in Touch" CTA) -->
      <ul class="about-intro-v2__credentials">
        <li
          v-for="(credential, idx) in CREDENTIALS"
          :key="credential"
          class="about-intro-v2__credential"
        >
          {{ credential }}
          <span
            v-if="idx < CREDENTIALS.length - 1"
            class="about-intro-v2__credential-dot"
            aria-hidden="true"
          ></span>
        </li>
      </ul>

      <LabeledChipListV2
        label="Our values"
        :items="VALUES"
        class="about-intro-v2__values"
      />
    </div>

    <ScrollIndicatorV2 href="#about-numbers" class="about-intro-v2__scroll" />
  </section>
</template>

<script setup lang="ts">
import DecoMagnifierV2 from '@/components/v2/shared/DecoMagnifierV2.vue'
import EyebrowPillV2 from '@/components/v2/shared/EyebrowPillV2.vue'
import LabeledChipListV2 from '@/components/v2/shared/LabeledChipListV2.vue'
import ScrollIndicatorV2 from '@/components/v2/shared/ScrollIndicatorV2.vue'

const CREDENTIALS = [
  'Since 2008',
  'Florida → Canada & USA',
  'Talent Management since 2016',
] as const

const VALUES = [
  'Fairness',
  'Respect',
  'Integrity',
  'Trust',
  'Care',
] as const
</script>

<style scoped>
/* ── Section shell — transparent (GlobalBackground shows through) ───────── */
.about-intro-v2 {
  position: relative;
  width: 100%;
  height: auto;
  min-height: max(100vh, 1080px);
  overflow: hidden;
  isolation: isolate;
}

/* ── Decorative magnifier — top-right anchor (mirror of Home Hero) ──────── */
.about-intro-v2__magnifier {
  top: clamp(14%, 16vw, 18%);
  right: clamp(6%, 7vw, 9%);
}

/* ── Content stack — vertically centered editorial layout ───────────────── */
.about-intro-v2__content {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: max(100vh, 1080px);
  /* Fluid padding: clears fixed navbar + leaves room before next section overlap */
  padding:
    clamp(80px, 12vw, 140px)
    clamp(20px, 3vw, 32px)
    clamp(120px, 16vw, 220px);
  text-align: center;
  max-width: 1100px;
  margin: 0 auto;
}

/* Spacing between stack elements lives here — atoms own their own visual */
.about-intro-v2__eyebrow {
  margin-bottom: clamp(24px, 3.5vw, 36px);
}

/* ── Main heading — large editorial with cyan accent ────────────────────── */
.about-intro-v2__heading {
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

.about-intro-v2__heading-accent {
  color: var(--c-brand-cyan);
  display: block;
}

/* ── Subtitle / value prop ──────────────────────────────────────────────── */
.about-intro-v2__subtitle {
  font-family: var(--font-family);
  font-size: clamp(14px, 1.4vw, 17px);
  font-weight: 400;
  line-height: 1.65;
  color: rgba(255, 255, 255, 0.85);
  margin: 0 0 clamp(28px, 3.6vw, 44px);
  max-width: 640px;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.35);
}

/* ── Credential line — historical footprint badges separated by cyan dots ── */
.about-intro-v2__credentials {
  list-style: none;
  margin: 0 0 clamp(36px, 5vw, 64px);
  padding: 0;
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  justify-content: center;
  gap: clamp(8px, 1vw, 14px);
}

.about-intro-v2__credential {
  display: inline-flex;
  align-items: center;
  gap: clamp(8px, 1vw, 14px);
  font-family: var(--font-family);
  font-size: clamp(10px, 0.85vw, 12px);
  font-weight: 600;
  letter-spacing: 0.22em;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.70);
  white-space: nowrap;
}

.about-intro-v2__credential-dot {
  width: 4px;
  height: 4px;
  border-radius: 50%;
  background: var(--c-brand-cyan);
  flex-shrink: 0;
}

/* ── Scroll indicator — absolute positioning lives on the parent class ──── */
.about-intro-v2__scroll {
  position: absolute;
  bottom: clamp(20px, 3vw, 36px);
  left: 50%;
  transform: translateX(-50%);
  z-index: 2;
}

/* ── Mobile-only behaviors (clamp can't express conditional layout) ─────── */
@media (max-width: 1023px) {
  .about-intro-v2 {
    min-height: 100svh;
  }

  .about-intro-v2__content {
    min-height: 100svh;
  }

  /* Stack credentials vertically on narrow viewports for legibility */
  .about-intro-v2__credentials {
    flex-direction: column;
  }

  .about-intro-v2__credential-dot {
    display: none;
  }

  .about-intro-v2__scroll {
    display: none;
  }

  .about-intro-v2__magnifier {
    top: 12%;
    right: 6%;
  }
}
</style>
