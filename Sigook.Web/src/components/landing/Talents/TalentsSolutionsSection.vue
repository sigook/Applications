<template>
  <section id="talents-solutions" class="talents-solutions">
    <!-- Inner glass surface — materializes the panel against GlobalBackground -->
    <div class="talents-solutions__surface" aria-hidden="true"></div>

    <LandingSectionHeader
      eyebrow="Your Career, Your Way"
      heading="Three paths."
      heading-accent="One commitment to your growth."
      subtitle="Whether you're after stability or variety, we'll match you with roles that keep moving your career forward."
    />

    <div class="talents-solutions__cards">
      <SecondaryCard
        v-for="(option, idx) in OPTIONS"
        :key="option.key"
        :variant="option.variant"
        :eyebrow="option.eyebrow"
        :title="option.title"
        :list="option.benefits"
        :delay="idx * 160"
        class="talents-solutions__card"
      >
        <template #button>
          <ArrowPillCta
            :to="option.ctaTo"
            :hover-variant="option.variant === 'red' ? 'red' : 'cyan'"
          >
            {{ option.ctaLabel }}
          </ArrowPillCta>
        </template>
      </SecondaryCard>
    </div>
  </section>
</template>

<script setup lang="ts">
/**
 * Talents — "Your Career, Your Way" section.
 *
 * Three SecondaryCard cards (canonical Home pattern) as career-path CTAs:
 * Direct Hiring = blue, Temp to Perm = cyan, Contract = red. Each card
 * carries a benefits list + a CTA pill — benefits via the `list` prop and
 * the CTA via the #button slot.
 */
import LandingSectionHeader from '@/components/landing/shared/LandingSectionHeader.vue'
import ArrowPillCta from '@/components/landing/shared/ArrowPillCta.vue'
import SecondaryCard, { type SecondaryCardVariant } from '@/components/landing/shared/SecondaryCard.vue'

interface CareerOption {
  readonly key: string
  readonly eyebrow: string
  readonly title: string
  readonly benefits: readonly string[]
  readonly ctaLabel: string
  readonly ctaTo: string
  readonly variant: SecondaryCardVariant
}

const OPTIONS: readonly CareerOption[] = [
  {
    key: 'direct-hiring',
    eyebrow: 'Long-Term Career',
    title: 'Direct Hiring',
    benefits: [
      'Permanent roles with competitive pay and full US benefits',
      'Join companies that invest in your development and long-term growth',
      'Build real career momentum with stability and a team behind you',
    ],
    ctaLabel: 'Browse direct hires',
    ctaTo: '/open-positions',
    variant: 'blue',
  },
  {
    key: 'temp-to-perm',
    eyebrow: 'Try Before You Commit',
    title: 'Temp to Perm',
    benefits: [
      'Start on a contract with a clear path to a permanent role',
      'Test the team and the day-to-day before committing long-term',
      'Convert to full-time once it\'s the right fit for both sides',
    ],
    ctaLabel: 'Browse temp-to-perm',
    ctaTo: '/open-positions',
    variant: 'cyan',
  },
  {
    key: 'contract',
    eyebrow: 'Flexibility & Reach',
    title: 'Contract',
    benefits: [
      'Pick up project-based roles across multiple industries and locations',
      'Grow your skillset faster by working in diverse environments',
      'No long-term commitment — work on your terms, at your pace',
    ],
    ctaLabel: 'Browse contracts',
    ctaTo: '/open-positions',
    variant: 'red',
  },
] as const
</script>

<style scoped>
/* ── Panel shell ────────────────────────────────────────────────────────── */
.talents-solutions {
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
  isolation: isolate;
  font-family: var(--font-family);
}

/* ── Depth back-layer — same brand shape, peeks ~24px top & bottom ──────── */
.talents-solutions::before {
  content: '';
  position: absolute;
  top: -16px;
  bottom: -16px;
  left: 0;
  right: 0;
  z-index: -1;
  border-radius:
    clamp(80px, 10vw, 150px) 0
    clamp(80px, 10vw, 150px) 0;
  background: rgba(255, 255, 255, 0.07);
  backdrop-filter: blur(10px) saturate(120%);
  -webkit-backdrop-filter: blur(10px) saturate(120%);
  border: 1px solid rgba(255, 255, 255, 0.14);
  box-shadow: 0 18px 40px -20px rgba(0, 0, 0, 0.4);
  pointer-events: none;
}

.talents-solutions__surface {
  position: absolute;
  inset: 0;
  z-index: 0;
  border-radius:
    clamp(80px, 10vw, 150px) 0
    clamp(80px, 10vw, 150px) 0;
  background: linear-gradient(
    180deg,
    rgba(9, 48, 85, 0.65) 0%,
    rgba(9, 48, 85, 0.55) 100%
  );
  backdrop-filter: blur(20px) saturate(160%);
  -webkit-backdrop-filter: blur(20px) saturate(160%);
  pointer-events: none;
}

/* ── Cards grid — 3 cols desktop, stack mobile ──────────────────────────── */
.talents-solutions__cards {
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

/* SecondaryCard ships padding/bg/list styling; we only need fill-height
   column so the CTA pill sits at the bottom of each card. */
.talents-solutions__card {
  display: flex;
  flex-direction: column;
}

/* ── Mobile-only behaviors ──────────────────────────────────────────────── */
@media (max-width: 899px) {
  .talents-solutions__cards { grid-template-columns: 1fr; }
}
</style>
