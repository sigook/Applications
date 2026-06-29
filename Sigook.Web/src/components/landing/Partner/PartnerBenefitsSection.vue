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
import { h, type FunctionalComponent } from 'vue'
import LandingSectionHeader from '@/components/landing/shared/LandingSectionHeader.vue'
import SecondaryCard, { type SecondaryCardVariant } from '@/components/landing/shared/SecondaryCard.vue'
import partnerBenefitsData from '@/data/landing/partnerBenefits.json'

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

const IconAutonomy = svg([
  'M12 22a10 10 0 1 0 0-20 10 10 0 0 0 0 20z',
  'm16.24 7.76-2.12 6.36-6.36 2.12 2.12-6.36 6.36-2.12z',
])
const IconReach = svg([
  'M12 22a10 10 0 1 0 0-20 10 10 0 0 0 0 20z',
  'M2 12h20',
  'M12 2a15.3 15.3 0 0 1 4 10 15.3 15.3 0 0 1-4 10 15.3 15.3 0 0 1-4-10 15.3 15.3 0 0 1 4-10z',
])
const IconLicensed = svg([
  'M12 2 4 5v7c0 5 4 9 8 10 4-1 8-5 8-10V5l-8-3z',
  'm9 12 2 2 4-4',
])
const IconRocket = svg([
  'M4.5 16.5c-1.5 1.26-2 5-2 5s3.74-.5 5-2c.71-.84.7-2.13-.09-2.91a2.18 2.18 0 0 0-2.91-.09z',
  'M12 15l-3-3a22 22 0 0 1 2-3.95A12.88 12.88 0 0 1 22 2c0 2.72-.78 7.5-6 11a22.35 22.35 0 0 1-4 2z',
  'M9 12H4s.55-3.03 2-4c1.62-1.08 5 0 5 0',
  'M12 15v5s3.03-.55 4-2c1.08-1.62 0-5 0-5',
])

interface BenefitData {
  readonly eyebrow: string
  readonly title: string
  readonly body: string
  readonly variant: SecondaryCardVariant
  readonly icon: string
}

interface Benefit {
  readonly eyebrow: string
  readonly title: string
  readonly body: string
  readonly variant: SecondaryCardVariant
  readonly iconComponent: FunctionalComponent
}

const ICONS: Record<string, FunctionalComponent> = {
  autonomy: IconAutonomy,
  reach: IconReach,
  licensed: IconLicensed,
  rocket: IconRocket,
}

const BENEFITS: readonly Benefit[] = (partnerBenefitsData as readonly BenefitData[]).map(
  (b) => ({
    eyebrow: b.eyebrow,
    title: b.title,
    body: b.body,
    variant: b.variant,
    iconComponent: ICONS[b.icon],
  })
)
</script>

<style scoped>
.partner-benefits {
  position: relative;
  width: 100%;
  padding:
    var(--section-pad-y)
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

@media (max-width: 1023px) {
  .partner-benefits__grid { grid-template-columns: repeat(2, 1fr); }
}

@media (max-width: 599px) {
  .partner-benefits__grid { grid-template-columns: 1fr; }
}
</style>
