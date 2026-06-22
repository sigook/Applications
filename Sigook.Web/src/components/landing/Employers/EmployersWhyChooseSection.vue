<template>
  <section class="employers-why">
    <LandingSectionHeader
      eyebrow="Why Employers Choose Us"
      heading="Three reasons"
      heading-accent="your shortlist arrives faster."
      subtitle="It's not just placement. It's a partnership — built on speed, sector expertise, and a network that took years to grow."
    />

    <div class="employers-why__grid">
      <SecondaryCard
        v-for="(reason, idx) in REASONS"
        :key="reason.title"
        :variant="reason.variant"
        :eyebrow="reason.eyebrow"
        :title="reason.title"
        :delay="idx * 140"
        :class="{ 'employers-why__card--offset': idx === 1 }"
      >
        {{ reason.body }}
      </SecondaryCard>
    </div>
  </section>
</template>

<script setup lang="ts">
/**
 * Employers — "Why Employers Choose Us" section.
 *
 * WINDOW-style section (transparent over GlobalBackground) — sits between
 * the Solutions panel (above) and the Industries carousel panel (below) to
 * keep the page rhythm: window → panel → window → panel → window.
 *
 * Mirror of TalentsWhyWorkSection: same valley-stagger 3-card layout, but
 * the cards speak from the employer side — speed of placement, depth of
 * sector expertise, breadth of the talent network.
 */
import LandingSectionHeader from '@/components/landing/shared/LandingSectionHeader.vue'
import SecondaryCard, { type SecondaryCardVariant } from '@/components/landing/shared/SecondaryCard.vue'

interface Reason {
  readonly eyebrow: string
  readonly title: string
  readonly body: string
  readonly variant: SecondaryCardVariant
}

const REASONS: readonly Reason[] = [
  {
    eyebrow: 'Speed to Hire',
    title: 'Shortlists in days, not months',
    body:
      'We tap an already-vetted bench of candidates the moment your role opens — most engagements deliver a qualified shortlist inside a week, not the industry-standard quarter.',
    variant: 'blue',
  },
  {
    eyebrow: 'Sector Expertise',
    title: 'Recruiters who speak your language',
    body:
      'Our recruiters specialize by industry — they know each sector\'s titles, salary bands, and what excellent looks like. You won\'t spend the kickoff explaining your own job description.',
    variant: 'cyan',
  },
  {
    eyebrow: 'Talent Network',
    title: 'A bench you didn\'t have to build',
    body:
      'Years of relationships across professional and industrial roles means the people you need are usually one phone call away — including the passive candidates job boards never reach.',
    variant: 'red',
  },
] as const
</script>

<style scoped>
/* ── Window shell — transparent (GlobalBackground shows through) ────────── */
.employers-why {
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
.employers-why__grid {
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
.employers-why__card--offset {
  margin-top: clamp(36px, 5vw, 72px);
}

/* ── Mobile-only behaviors ──────────────────────────────────────────────── */
@media (max-width: 899px) {
  .employers-why__grid { grid-template-columns: 1fr; }

  /* Drop the valley offset on stacked layout — makes no visual sense in 1 col */
  .employers-why__card--offset { margin-top: 0; }
}
</style>
