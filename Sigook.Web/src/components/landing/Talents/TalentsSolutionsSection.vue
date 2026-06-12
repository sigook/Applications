<template>
  <section id="talents-solutions" class="talents-solutions">
    <!-- Inner glass surface — materializes the panel against GlobalBackground -->
    <div class="talents-solutions__surface" aria-hidden="true"></div>

    <header class="talents-solutions__header">
      <EyebrowPill variant="white" class="talents-solutions__eyebrow">
        Your Career, Your Way
      </EyebrowPill>

      <h2 class="talents-solutions__heading">
        Three paths.
        <span class="talents-solutions__heading-accent">
          One commitment to your growth.
        </span>
      </h2>

      <p class="talents-solutions__subtitle">
        Whether you're after stability or variety, we'll match you with roles
        that keep moving your career forward.
      </p>
    </header>

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
          <router-link
            :to="option.ctaTo"
            class="talents-solutions__cta"
            :class="`talents-solutions__cta--${option.variant}`"
          >
            <span>{{ option.ctaLabel }}</span>
            <span class="talents-solutions__cta-arrow" aria-hidden="true">→</span>
          </router-link>
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
import EyebrowPill from '@/components/landing/shared/EyebrowPill.vue'
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

/* ── Header ─────────────────────────────────────────────────────────────── */
.talents-solutions__header {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  max-width: 880px;
  margin: 0 auto;
}

.talents-solutions__eyebrow {
  margin-bottom: clamp(20px, 2.5vw, 28px);
}

.talents-solutions__heading {
  font-size: clamp(28px, 4.2vw, 46px);
  font-weight: 700;
  line-height: 1.15;
  letter-spacing: -0.02em;
  color: #fff;
  margin: 0 0 clamp(14px, 1.8vw, 22px);
  max-width: 780px;
  text-shadow: 0 4px 24px rgba(0, 0, 0, 0.40);
}

.talents-solutions__heading-accent {
  color: var(--c-brand-cyan);
  display: block;
}

.talents-solutions__subtitle {
  font-size: clamp(13px, 1.2vw, 16px);
  font-weight: 400;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.78);
  margin: 0;
  max-width: 560px;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.30);
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

/* ── CTA pill — sits in the SecondaryCard #button slot ─────────────────── */
.talents-solutions__cta {
  display: inline-flex;
  align-items: center;
  gap: 10px;
  padding: clamp(11px, 1.2vw, 14px) clamp(22px, 2.4vw, 30px);
  background: rgba(255, 255, 255, 0.12);
  border: 1px solid rgba(255, 255, 255, 0.45);
  border-radius: 999px;
  color: #fff;
  font-family: var(--font-family);
  font-size: clamp(13px, 1.05vw, 14px);
  font-weight: 600;
  letter-spacing: 0.04em;
  text-decoration: none;
  cursor: pointer;
  transition:
    background 0.25s ease,
    border-color 0.25s ease,
    color 0.25s ease,
    transform 0.25s ease;
}

.talents-solutions__cta--blue:hover,
.talents-solutions__cta--cyan:hover {
  background: var(--c-brand-cyan);
  border-color: var(--c-brand-cyan);
  color: var(--c-brand-navy);
  transform: translateX(4px);
}

.talents-solutions__cta--red:hover {
  background: #fff;
  border-color: #fff;
  color: var(--c-brand-red);
  transform: translateX(4px);
}

.talents-solutions__cta-arrow {
  font-size: 1.15em;
  font-weight: 700;
  line-height: 1;
  transition: transform 0.25s ease;
}

.talents-solutions__cta:hover .talents-solutions__cta-arrow {
  transform: translateX(3px);
}

/* ── Mobile-only behaviors ──────────────────────────────────────────────── */
@media (max-width: 899px) {
  .talents-solutions__cards { grid-template-columns: 1fr; }
}
</style>
