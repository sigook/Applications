<template>
  <section class="why-work">
    <!-- Inner glass surface — materializes the panel against GlobalBackground -->
    <div class="why-work__surface" aria-hidden="true"></div>

    <header class="why-work__header">
      <EyebrowPill variant="white" class="why-work__eyebrow">
        What Sets Us Apart
      </EyebrowPill>

      <h2 class="why-work__heading">
        Three reasons people
        <span class="why-work__heading-accent">choose &amp; stay.</span>
      </h2>

      <p class="why-work__subtitle">
        Local presence, modern technology, and seventeen years of know-how —
        woven into every match we make.
      </p>
    </header>

    <div class="why-work__cards">
      <SecondaryCard
        v-for="(reason, idx) in REASONS"
        :key="reason.title"
        :variant="reason.variant"
        :eyebrow="reason.eyebrow"
        :title="reason.title"
        :delay="idx * 140"
      >
        <span v-html="reason.body" />
      </SecondaryCard>
    </div>
  </section>
</template>

<script setup lang="ts">
/**
 * Why Work With Us section (About page panel).
 *
 * Three SecondaryCard cards (canonical Home pattern: triple gradient, radial
 * glow, asymmetric brand radius) showing the company's differentiators.
 * Cycle: blue → cyan → red. Stagger entry via `delay` prop.
 */
import EyebrowPill from '@/components/v2/landing/shared/EyebrowPill.vue'
import SecondaryCard, { type SecondaryCardVariant } from '@/components/v2/landing/shared/SecondaryCard.vue'

interface Reason {
  readonly eyebrow: string
  readonly title: string
  readonly body: string
  readonly variant: SecondaryCardVariant
}

const REASONS: readonly Reason[] = [
  {
    eyebrow: 'Local Presence',
    title: 'We’re in your neighborhood',
    body:
      'Most of our employer partners are in the Greater Toronto Area, so we know their teams, their needs, and the right person to send when the call comes in.',
    variant: 'blue',
  },
  {
    eyebrow: 'Speed by Design',
    title: 'From profile to paycheck — fast',
    body:
      'SIGOOK&trade; — our self-serve platform — connects employers with our talent database in clicks. Get hired today, start working tomorrow.',
    variant: 'cyan',
  },
  {
    eyebrow: 'Seventeen Years In',
    title: 'Experience you can lean on',
    body:
      'We’ve matched thousands of workers to clients across industries since 2008. We know what works because we’ve done it — at scale, every season.',
    variant: 'red',
  },
] as const
</script>

<style scoped>
/* ── Panel shell — adopts DualCta vocabulary ────────────────────────────── */
.why-work {
  position: relative;
  width: 100%;
  margin-top: clamp(-180px, -10vw, -80px);
  padding:
    clamp(140px, 14vw, 200px)
    clamp(20px, 3vw, 64px);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: clamp(48px, 6vw, 80px);
  z-index: 5;
  border-radius:
    clamp(80px, 10vw, 150px) 0
    clamp(80px, 10vw, 150px) 0;
  box-shadow:
    0 -22px 40px -12px rgba(0, 0, 0, 0.45),
    0  22px 40px -12px rgba(0, 0, 0, 0.45);
  overflow: hidden;
  isolation: isolate;
  font-family: var(--font-family);
}

.why-work__surface {
  position: absolute;
  inset: 0;
  z-index: 0;
  background: linear-gradient(
    180deg,
    rgba(9, 48, 85, 0.65) 0%,
    rgba(9, 48, 85, 0.55) 100%
  );
  backdrop-filter: blur(20px) saturate(160%);
  -webkit-backdrop-filter: blur(20px) saturate(160%);
  pointer-events: none;
}

/* ── Header ─────────────────────────────────────────────────────────────── */
.why-work__header {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  max-width: 880px;
  margin: 0 auto;
}

.why-work__eyebrow {
  margin-bottom: clamp(20px, 2.5vw, 28px);
}

.why-work__heading {
  font-size: clamp(28px, 4.2vw, 46px);
  font-weight: 700;
  line-height: 1.15;
  letter-spacing: -0.02em;
  color: #fff;
  margin: 0 0 clamp(14px, 1.8vw, 22px);
  max-width: 780px;
  text-shadow: 0 4px 24px rgba(0, 0, 0, 0.40);
}

.why-work__heading-accent {
  color: var(--c-brand-cyan);
  display: block;
}

.why-work__subtitle {
  font-size: clamp(13px, 1.2vw, 16px);
  font-weight: 400;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.78);
  margin: 0;
  max-width: 560px;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.30);
}

/* ── Cards grid — 3 columns desktop, stack mobile ───────────────────────── */
.why-work__cards {
  position: relative;
  z-index: 2;
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: clamp(20px, 2.4vw, 32px);
  align-items: start;
  width: 100%;
  max-width: 1180px;
  margin: 0 auto;
}

/* Inline-link styling inside SecondaryCard slot — preserves SIGOOK™ link */
.why-work__cards :deep(a) {
  color: var(--c-brand-cyan);
  text-decoration: none;
  border-bottom: 1px solid rgba(0, 173, 239, 0.40);
  transition: border-color 0.25s ease;
}

.why-work__cards :deep(a:hover) {
  border-bottom-color: var(--c-brand-cyan);
}

/* ── Mobile-only behaviors ──────────────────────────────────────────────── */
@media (max-width: 899px) {
  .why-work__cards { grid-template-columns: 1fr; }
}
</style>
