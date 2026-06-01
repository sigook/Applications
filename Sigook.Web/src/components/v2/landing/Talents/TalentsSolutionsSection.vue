<template>
  <section id="talents-solutions" class="talents-solutions">
    <!-- Inner glass surface — materializes the panel against GlobalBackground -->
    <div class="talents-solutions__surface" aria-hidden="true"></div>

    <header class="talents-solutions__header">
      <EyebrowPill variant="white" class="talents-solutions__eyebrow">
        Your Career, Your Way
      </EyebrowPill>

      <h2 class="talents-solutions__heading">
        Two paths.
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
        {{ option.body }}

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
 * Two SecondaryCard cards (canonical Home pattern) as career-path CTAs.
 * Direct Hiring = blue variant, Contract = red variant. Each card carries
 * supporting copy + a benefits list + a CTA pill — body via default slot,
 * benefits via the `list` prop, and the CTA via the #button slot.
 */
import EyebrowPill from '@/components/v2/landing/shared/EyebrowPill.vue'
import SecondaryCard, { type SecondaryCardVariant } from '@/components/v2/landing/shared/SecondaryCard.vue'

interface CareerOption {
  readonly key: string
  readonly eyebrow: string
  readonly title: string
  readonly body: string
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
    body:
      'Build a lasting career with permanent placements at companies that invest in your growth — full benefits, real progression, lasting team relationships.',
    benefits: [
      'Permanent placement',
      'Full benefits package',
      'Clear career progression',
    ],
    ctaLabel: 'Browse direct hires',
    ctaTo: '/v2/open-positions',
    variant: 'blue',
  },
  {
    key: 'contract',
    eyebrow: 'Flexibility & Reach',
    title: 'Contract',
    body:
      'Project-based roles that let you choose your pace, diversify your skills, and work across the industries that interest you most — without the long-term tie-down.',
    benefits: [
      'Variety of projects',
      'Faster skill expansion',
      'Multi-industry exposure',
    ],
    ctaLabel: 'Browse contracts',
    ctaTo: '/v2/open-positions',
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
  overflow: hidden;
  isolation: isolate;
  font-family: var(--font-family);
}

.talents-solutions__surface {
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

/* ── Cards grid — 2 cols desktop, stack mobile ──────────────────────────── */
.talents-solutions__cards {
  position: relative;
  z-index: 2;
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: clamp(24px, 3.2vw, 40px);
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
