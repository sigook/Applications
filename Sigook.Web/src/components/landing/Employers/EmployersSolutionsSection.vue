<template>
  <section id="employers-solutions" class="employers-solutions">
    <!-- Inner glass surface — materializes the panel against GlobalBackground -->
    <div class="employers-solutions__surface" aria-hidden="true"></div>

    <header class="employers-solutions__header">
      <EyebrowPill variant="white" class="employers-solutions__eyebrow">
        Professional Hiring Options
      </EyebrowPill>

      <h2 class="employers-solutions__heading">
        Two ways to hire.
        <span class="employers-solutions__heading-accent">
          One commitment to fit.
        </span>
      </h2>

      <p class="employers-solutions__subtitle">
        Build a permanent team or scale on demand — choose the model that
        matches your timeline, budget, and growth plan.
      </p>
    </header>

    <div class="employers-solutions__cards">
      <SecondaryCard
        v-for="(option, idx) in OPTIONS"
        :key="option.key"
        :variant="option.variant"
        :eyebrow="option.eyebrow"
        :title="option.title"
        :list="option.benefits"
        :delay="idx * 160"
        class="employers-solutions__card"
      >
        {{ option.body }}

        <template #button>
          <router-link
            :to="option.ctaTo"
            class="employers-solutions__cta"
            :class="`employers-solutions__cta--${option.variant}`"
          >
            <span>{{ option.ctaLabel }}</span>
            <span class="employers-solutions__cta-arrow" aria-hidden="true">→</span>
          </router-link>
        </template>
      </SecondaryCard>
    </div>
  </section>
</template>

<script setup lang="ts">
/**
 * Employers — "Professional Hiring Options" section.
 *
 * Mirror of TalentsSolutionsSection but flipped to the employer side:
 * the same two services (Direct Hiring + Contract) framed as hiring
 * outcomes rather than career paths.
 *
 * Two SecondaryCard (canonical Home pattern) — Direct Hiring = blue
 * variant, Contract = red variant. Each card carries supporting copy,
 * a benefits list, and a CTA pill.
 */
import EyebrowPill from '@/components/landing/shared/EyebrowPill.vue'
import SecondaryCard, { type SecondaryCardVariant } from '@/components/landing/shared/SecondaryCard.vue'

interface HiringOption {
  readonly key: string
  readonly eyebrow: string
  readonly title: string
  readonly body: string
  readonly benefits: readonly string[]
  readonly ctaLabel: string
  readonly ctaTo: string
  readonly variant: SecondaryCardVariant
}

const OPTIONS: readonly HiringOption[] = [
  {
    key: 'direct-hiring',
    eyebrow: 'Long-Term Fit',
    title: 'Direct Hiring',
    body:
      'Find long-term professionals who align with your company\'s culture and goals — vetted candidates, structured search, full handoff to your team.',
    benefits: [
      'Culture-matched candidates',
      'End-to-end vetting',
      'Replacement guarantee',
    ],
    ctaLabel: 'Talk to a recruiter',
    ctaTo: '#employers-contact',
    variant: 'blue',
  },
  {
    key: 'contract',
    eyebrow: 'Flexibility & Scale',
    title: 'Contract',
    body:
      'Access skilled talent quickly with flexible contracts tailored to project needs — ramp up for a peak, fill a gap, or pilot a role before going permanent.',
    benefits: [
      'Fast deployment',
      'Project-based pricing',
      'Easy contract-to-hire',
    ],
    ctaLabel: 'Request talent',
    ctaTo: '#employers-contact',
    variant: 'red',
  },
] as const
</script>

<style scoped>
/* ── Panel shell ────────────────────────────────────────────────────────── */
.employers-solutions {
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

.employers-solutions::before {
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

.employers-solutions__surface {
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
.employers-solutions__header {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  max-width: 880px;
  margin: 0 auto;
}

.employers-solutions__eyebrow {
  margin-bottom: clamp(20px, 2.5vw, 28px);
}

.employers-solutions__heading {
  font-size: clamp(28px, 4.2vw, 46px);
  font-weight: 700;
  line-height: 1.15;
  letter-spacing: -0.02em;
  color: #fff;
  margin: 0 0 clamp(14px, 1.8vw, 22px);
  max-width: 780px;
  text-shadow: 0 4px 24px rgba(0, 0, 0, 0.40);
}

.employers-solutions__heading-accent {
  color: var(--c-brand-cyan);
  display: block;
}

.employers-solutions__subtitle {
  font-size: clamp(13px, 1.2vw, 16px);
  font-weight: 400;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.78);
  margin: 0;
  max-width: 560px;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.30);
}

/* ── Cards grid — 2 cols desktop, stack mobile ──────────────────────────── */
.employers-solutions__cards {
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
.employers-solutions__card {
  display: flex;
  flex-direction: column;
}

/* ── CTA pill — sits in the SecondaryCard #button slot ─────────────────── */
.employers-solutions__cta {
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

.employers-solutions__cta--blue:hover,
.employers-solutions__cta--cyan:hover {
  background: var(--c-brand-cyan);
  border-color: var(--c-brand-cyan);
  color: var(--c-brand-navy);
  transform: translateX(4px);
}

.employers-solutions__cta--red:hover {
  background: #fff;
  border-color: #fff;
  color: var(--c-brand-red);
  transform: translateX(4px);
}

.employers-solutions__cta-arrow {
  font-size: 1.15em;
  font-weight: 700;
  line-height: 1;
  transition: transform 0.25s ease;
}

.employers-solutions__cta:hover .employers-solutions__cta-arrow {
  transform: translateX(3px);
}

/* ── Mobile-only behaviors ──────────────────────────────────────────────── */
@media (max-width: 899px) {
  .employers-solutions__cards { grid-template-columns: 1fr; }
}
</style>
