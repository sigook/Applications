<template>
  <section class="partner-benefits">
    <LandingSectionHeader
      eyebrow="Why Partner With Us"
      heading="Built for builders."
      heading-accent="Four reasons partners stay."
      subtitle="The trade-offs other staffing networks ask you to accept — control, reach, compliance, upside — are the four things our program is built to give you instead."
      subtitle-max-width="620px"
    />

    <div class="partner-benefits__grid">
      <SecondaryCard
        v-for="(benefit, idx) in BENEFITS"
        :key="benefit.title"
        :variant="benefit.variant"
        :eyebrow="benefit.eyebrow"
        :title="benefit.title"
        :delay="idx * 130"
        class="partner-benefits__card"
      >
        <template #icon>
          <component :is="benefit.iconComponent" />
        </template>

        {{ benefit.body }}
      </SecondaryCard>
    </div>
  </section>
</template>

<script setup lang="ts">
/**
 * Partner — "Why Partner With Us" section.
 *
 * WINDOW-style section (transparent over GlobalBackground) — keeps the
 * page rhythm window → panel → WINDOW → panel → window.
 *
 * Four SecondaryCards in a 4-col grid (2x2 tablet, stack mobile). Variants
 * cycle through the canonical palette (cyan / blue / cyan / red) so the row
 * has visual variety without losing the system.
 */
import { h, type FunctionalComponent } from 'vue'
import LandingSectionHeader from '@/components/landing/shared/LandingSectionHeader.vue'
import SecondaryCard, { type SecondaryCardVariant } from '@/components/landing/shared/SecondaryCard.vue'

/**
 * Tiny inline SVG icons. Functional components keep the bundle small and
 * let us pass them straight to `<component :is="...">`.
 */
const svg = (paths: string[]): FunctionalComponent => () =>
  h('svg', {
    viewBox: '0 0 24 24',
    fill: 'none',
    stroke: 'currentColor',
    'stroke-width': 1.5,
    'stroke-linecap': 'round',
    'stroke-linejoin': 'round',
    'aria-hidden': true,
    style: 'width:1em;height:1em;display:block;color:inherit;',
  }, paths.map((d) => h('path', { d })))

// Compass — full autonomy / direction
const IconAutonomy = svg([
  'M12 22a10 10 0 1 0 0-20 10 10 0 0 0 0 20z',
  'm16.24 7.76-2.12 6.36-6.36 2.12 2.12-6.36 6.36-2.12z',
])
// Globe + network — nationwide reach
const IconReach = svg([
  'M12 22a10 10 0 1 0 0-20 10 10 0 0 0 0 20z',
  'M2 12h20',
  'M12 2a15.3 15.3 0 0 1 4 10 15.3 15.3 0 0 1-4 10 15.3 15.3 0 0 1-4-10 15.3 15.3 0 0 1 4-10z',
])
// Shield-check — licensed / compliance
const IconLicensed = svg([
  'M12 2 4 5v7c0 5 4 9 8 10 4-1 8-5 8-10V5l-8-3z',
  'm9 12 2 2 4-4',
])
// Rocket — entrepreneurial upside
const IconRocket = svg([
  'M4.5 16.5c-1.5 1.26-2 5-2 5s3.74-.5 5-2c.71-.84.7-2.13-.09-2.91a2.18 2.18 0 0 0-2.91-.09z',
  'M12 15l-3-3a22 22 0 0 1 2-3.95A12.88 12.88 0 0 1 22 2c0 2.72-.78 7.5-6 11a22.35 22.35 0 0 1-4 2z',
  'M9 12H4s.55-3.03 2-4c1.62-1.08 5 0 5 0',
  'M12 15v5s3.03-.55 4-2c1.08-1.62 0-5 0-5',
])

interface Benefit {
  readonly eyebrow: string
  readonly title: string
  readonly body: string
  readonly variant: SecondaryCardVariant
  readonly iconComponent: FunctionalComponent
}

const BENEFITS: readonly Benefit[] = [
  {
    eyebrow: 'Set Your Own Pace',
    title: 'Full Autonomy',
    body:
      'Pick your hours, your territory, your specialty. No quotas, no central dispatch — your book of business runs on your rhythm.',
    variant: 'cyan',
    iconComponent: IconAutonomy,
  },
  {
    eyebrow: 'Coast to Coast',
    title: 'Nationwide Reach',
    body:
      'Place across the United States through one license, one contract, one payroll system. We absorb the multi-state overhead.',
    variant: 'blue',
    iconComponent: IconReach,
  },
  {
    eyebrow: 'Compliant by Default',
    title: 'Licensed Agency',
    body:
      'Operate under Sigook\'s certifications, insurance, and audit-ready compliance framework — you get the credibility of a tier-1 firm on day one.',
    variant: 'cyan',
    iconComponent: IconLicensed,
  },
  {
    eyebrow: 'Real Upside',
    title: 'Entrepreneurial Rewards',
    body:
      'Performance-based comp with no ceiling. The deals you close compound into ongoing revenue, not capped commissions.',
    variant: 'red',
    iconComponent: IconRocket,
  },
] as const
</script>

<style scoped>
/* ── Window shell — transparent (GlobalBackground shows through) ────────── */
.partner-benefits {
  position: relative;
  width: 100%;
  padding:
    clamp(72px, 10vw, 140px)
    clamp(20px, 3vw, 40px)
    clamp(96px, 12vw, 180px);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: clamp(48px, 6vw, 80px);
  isolation: isolate;
  overflow: hidden;
  font-family: var(--font-family);
}

/* ── Grid — 4 cols desktop, 2 tablet, 1 mobile ──────────────────────────── */
.partner-benefits__grid {
  position: relative;
  z-index: 2;
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: clamp(20px, 2.4vw, 28px);
  align-items: stretch;
  width: 100%;
  max-width: 1280px;
  margin: 0 auto;
}

.partner-benefits__card {
  display: flex;
  flex-direction: column;
}

/* ── Responsive ─────────────────────────────────────────────────────────── */
@media (max-width: 1023px) {
  .partner-benefits__grid { grid-template-columns: repeat(2, 1fr); }
}

@media (max-width: 599px) {
  .partner-benefits__grid { grid-template-columns: 1fr; }
}
</style>
