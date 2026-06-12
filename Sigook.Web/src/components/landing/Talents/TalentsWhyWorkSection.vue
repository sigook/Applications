<template>
  <section class="talents-why">
    <header class="talents-why__header">
      <EyebrowPill variant="white" class="talents-why__eyebrow">
        Why Work With Us
      </EyebrowPill>

      <h2 class="talents-why__heading">
        Three things
        <span class="talents-why__heading-accent">you'll feel from day one.</span>
      </h2>

      <p class="talents-why__subtitle">
        It's not just placement. It's a relationship — built on local presence,
        genuine care, and a recruiter who actually knows your industry.
      </p>
    </header>

    <div class="talents-why__grid">
      <SecondaryCard
        v-for="(reason, idx) in REASONS"
        :key="reason.title"
        :variant="reason.variant"
        :eyebrow="reason.eyebrow"
        :title="reason.title"
        :delay="idx * 140"
        :class="{ 'talents-why__card--offset': idx === 1 }"
      >
        {{ reason.body }}
      </SecondaryCard>
    </div>
  </section>
</template>

<script setup lang="ts">
/**
 * Talents — "Why Work With Us" section.
 *
 * WINDOW-style section (transparent over GlobalBackground) — sits between
 * the Solutions panel (above) and the Industries carousel panel (below) to
 * keep the page rhythm: window → panel → window → panel → window.
 *
 * Three SecondaryCard (canonical Home pattern) with a "valley" stagger
 * (middle card sits lower) — keeps a distinct rhythm from the About page's
 * WhyWorkWithUs grid. Same family, different beat.
 */
import EyebrowPill from '@/components/landing/shared/EyebrowPill.vue'
import SecondaryCard, { type SecondaryCardVariant } from '@/components/landing/shared/SecondaryCard.vue'

interface Reason {
  readonly eyebrow: string
  readonly title: string
  readonly body: string
  readonly variant: SecondaryCardVariant
}

const REASONS: readonly Reason[] = [
  {
    eyebrow: 'Local Reach',
    title: 'Working where you are',
    body:
      'With clients located across multiple states, you\'ll find roles that match your skills and interests — and build the relationships that turn placement into long-term stability.',
    variant: 'blue',
  },
  {
    eyebrow: 'Human-First',
    title: 'We care about you',
    body:
      'You\'re not just a candidate. Your aspirations and vision define how we work — we take the time to understand where you want to go before we recommend a single opportunity.',
    variant: 'cyan',
  },
  {
    eyebrow: 'Sector Expertise',
    title: 'Focused on your experience',
    body:
      'Our recruiters specialize in the sectors where your skills are most valued. They know each industry\'s rhythms, language, and what excellent looks like — and they match you accordingly.',
    variant: 'red',
  },
] as const
</script>

<style scoped>
/* ── Window shell — transparent (GlobalBackground shows through) ────────── */
.talents-why {
  position: relative;
  width: 100%;
  padding:
    clamp(72px, 10vw, 140px)
    clamp(20px, 3vw, 40px)
    clamp(180px, 18vw, 300px);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: clamp(48px, 6vw, 80px);
  isolation: isolate;
  overflow: hidden;
  font-family: var(--font-family);
}

/* ── Header ─────────────────────────────────────────────────────────────── */
.talents-why__header {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  max-width: 880px;
  margin: 0 auto;
}

.talents-why__eyebrow {
  margin-bottom: clamp(20px, 2.5vw, 28px);
}

.talents-why__heading {
  font-size: clamp(28px, 4.2vw, 46px);
  font-weight: 700;
  line-height: 1.15;
  letter-spacing: -0.02em;
  color: #fff;
  margin: 0 0 clamp(14px, 1.8vw, 22px);
  max-width: 780px;
  text-shadow: 0 4px 24px rgba(0, 0, 0, 0.40);
}

.talents-why__heading-accent {
  color: var(--c-brand-cyan);
  display: block;
}

.talents-why__subtitle {
  font-size: clamp(13px, 1.2vw, 16px);
  font-weight: 400;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.78);
  margin: 0;
  max-width: 560px;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.30);
}

/* ── Cards grid — 3 cols desktop, stack mobile ──────────────────────────── */
.talents-why__grid {
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

/* "Valley" stagger — middle card drops to break the rigid 3-col rhythm */
.talents-why__card--offset {
  margin-top: clamp(36px, 5vw, 72px);
}

/* ── Mobile-only behaviors ──────────────────────────────────────────────── */
@media (max-width: 899px) {
  .talents-why__grid { grid-template-columns: 1fr; }

  /* Drop the valley offset on stacked layout — makes no visual sense in 1 col */
  .talents-why__card--offset { margin-top: 0; }
}
</style>
