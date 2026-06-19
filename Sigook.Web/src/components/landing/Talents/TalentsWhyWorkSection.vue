<template>
  <section class="talents-why">
    <LandingSectionHeader
      eyebrow="Why Work With Us"
      heading="Three things"
      heading-accent="you'll feel from day one."
      subtitle="It's not just placement. It's a relationship — built on local presence, genuine care, and a recruiter who actually knows your industry."
    />

    <div class="talents-why__grid">
      <SecondaryCard
        v-for="(reason, idx) in REASONS"
        :key="reason.title"
        :variant="reason.variant"
        :eyebrow="reason.eyebrow"
        :title="reason.title"
        :list="reason.bullets"
        :delay="idx * 140"
        :class="{ 'talents-why__card--offset': idx === 1 }"
      />
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
import LandingSectionHeader from '@/components/landing/shared/LandingSectionHeader.vue'
import SecondaryCard, { type SecondaryCardVariant } from '@/components/landing/shared/SecondaryCard.vue'

interface Reason {
  readonly eyebrow: string
  readonly title: string
  readonly bullets: readonly string[]
  readonly variant: SecondaryCardVariant
}

const REASONS: readonly Reason[] = [
  {
    eyebrow: 'Local Reach',
    title: 'Working where you are',
    bullets: [
      'Roles available across major US states and metro areas',
      'Local recruiters who know your market and the employers in it',
      'Placements that grow into lasting careers, not just one-time gigs',
    ],
    variant: 'blue',
  },
  {
    eyebrow: 'Human-First',
    title: 'We care about you',
    bullets: [
      'We listen first — your goals shape every role we recommend',
      'A real person in your corner from first conversation to first day',
      'We don\'t stop at placement — we check in to make sure it\'s working',
    ],
    variant: 'cyan',
  },
  {
    eyebrow: 'Sector Expertise',
    title: 'Focused on your experience',
    bullets: [
      'Recruiters who specialize in skilled trades, industrial, and professional sectors',
      'Deep knowledge of what great looks like in your specific field',
      'Matched to roles where your skills are genuinely valued and put to use',
    ],
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
