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

    <div
      class="employers-solutions__cards"
      :class="{ 'employers-solutions__cards--detail': tempExpanded }"
    >
      <SecondaryCard
        v-for="(option, idx) in OPTIONS"
        :key="option.key"
        :variant="option.key === 'temp-to-perm' && tempExpanded ? 'red' : option.variant"
        :eyebrow="option.eyebrow"
        :title="option.title"
        :list="option.benefits"
        :delay="idx * 160"
        :expanded="option.key === 'temp-to-perm' && tempExpanded"
        class="employers-solutions__card"
        :class="{
          'employers-solutions__card--popped': option.key === 'temp-to-perm' && tempExpanded,
          'employers-solutions__card--dimmed': option.key !== 'temp-to-perm' && tempExpanded,
        }"
      >
        {{ option.body }}

        <template v-if="option.key === 'temp-to-perm'" #expanded>
          <TempToPermDetail />
        </template>

        <template #button>
          <ArrowPillCta
            v-if="option.key !== 'temp-to-perm'"
            :to="option.ctaTo"
            :hover-variant="option.variant === 'red' ? 'red' : 'cyan'"
          >
            {{ option.ctaLabel }}
          </ArrowPillCta>
          <ArrowPillCta
            v-else
            :hover-variant="tempExpanded ? 'red' : 'cyan'"
            :show-arrow="false"
            @click="tempExpanded = !tempExpanded"
          >
            {{ tempExpanded ? 'Show less' : 'View details' }}
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
import { ref } from 'vue'
import LandingSectionHeader from '@/components/landing/shared/LandingSectionHeader.vue'
import ArrowPillCta from '@/components/landing/shared/ArrowPillCta.vue'
import SecondaryCard, { type SecondaryCardVariant } from '@/components/landing/shared/SecondaryCard.vue'
import TempToPermDetail from '@/components/landing/shared/TempToPermDetail.vue'

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

const tempExpanded = ref(false)
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
  align-items: stretch;
  gap: clamp(20px, 2.4vw, 32px);
  width: 100%;
  max-width: 1180px;
  margin: 0 auto;
}

.employers-solutions__card {
  position: relative;
  display: flex;
  flex-direction: column;
  transition:
    transform 0.55s cubic-bezier(0.34, 1.56, 0.64, 1),
    opacity 0.4s ease,
    filter 0.4s ease,
    box-shadow 0.5s ease;
}

/* ── Detail open: Temp-to-Perm pops forward; the others recede in place ───── */
.employers-solutions__cards--detail {
  align-items: start;
}

.employers-solutions__cards--detail .employers-solutions__card--popped {
  z-index: 20;
  opacity: 1;
  transform: scale(1.06) translateY(-6px);
  box-shadow:
    0 44px 90px -28px rgba(0, 0, 0, 0.65),
    0 0 0 1px rgba(255, 255, 255, 0.12);
}

.employers-solutions__cards--detail .employers-solutions__card--dimmed {
  opacity: 0.22;
  filter: blur(2px) saturate(0.85);
  transform: scale(0.95);
  pointer-events: none;
}

/* ── Mobile-only behaviors — stack, plain accordion (no pop / no dim) ────── */
@media (max-width: 899px) {
  .employers-solutions__cards {
    grid-template-columns: 1fr;
  }

  .employers-solutions__cards--detail .employers-solutions__card--popped {
    transform: none;
    box-shadow: 0 24px 50px -20px rgba(0, 0, 0, 0.55);
  }

  .employers-solutions__cards--detail .employers-solutions__card--dimmed {
    opacity: 1;
    filter: none;
    transform: none;
    pointer-events: auto;
  }
}
</style>
