<template>
  <section id="employers-solutions" class="employers-solutions">
    <!-- Inner glass surface — materializes the panel against GlobalBackground -->
    <div class="employers-solutions__surface" aria-hidden="true"></div>

    <LandingSectionHeader
      eyebrow="Professional Hiring Options"
      heading="Three ways to hire."
      heading-accent="One commitment to fit."
      subtitle="Build a permanent team or scale on demand — choose the model that matches your timeline, budget, and growth plan."
    />

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
 * Employers — "Professional Hiring Options" section.
 *
 * Mirror of TalentsSolutionsSection but flipped to the employer side:
 * the same hiring models framed as outcomes rather than career paths.
 *
 * Three SecondaryCard (canonical Home pattern) — Direct Hiring = blue,
 * Temp to Perm = cyan, Contract = red. Each card carries supporting copy,
 * a benefits list, and a CTA pill.
 */
import LandingSectionHeader from '@/components/landing/shared/LandingSectionHeader.vue'
import ArrowPillCta from '@/components/landing/shared/ArrowPillCta.vue'
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
    key: 'temp-to-perm',
    eyebrow: 'Try Before You Hire',
    title: 'Temp to Perm',
    body:
      'Bring talent on as a contractor first and convert to a permanent hire once they\'ve proven the fit on the job — the lowest-risk path to a long-term addition to your team.',
    benefits: [
      'On-the-job evaluation',
      'Lower hiring risk',
      'Smooth conversion to permanent',
    ],
    ctaLabel: 'Explore temp-to-perm',
    ctaTo: '#employers-contact',
    variant: 'cyan',
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

/* ── Cards grid — 3 cols desktop, stack mobile ──────────────────────────── */
.employers-solutions__cards {
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
.employers-solutions__card {
  display: flex;
  flex-direction: column;
}

/* ── Mobile-only behaviors ──────────────────────────────────────────────── */
@media (max-width: 899px) {
  .employers-solutions__cards { grid-template-columns: 1fr; }
}
</style>
