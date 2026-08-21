<template>
  <section id="sp-focus" class="sp-focus">
    <div class="sp-focus__surface" aria-hidden="true"></div>

    <LandingSectionHeader
      eyebrow="Our Focus Areas"
      heading="Four ways our work"
      heading-accent="moves communities forward."
      subtitle="Each initiative weaves workforce, technology, and community engagement into outcomes that outlast a single placement."
    />

    <div class="sp-focus__grid">
      <SecondaryCard
        v-for="(area, idx) in AREAS"
        :key="area.title"
        :variant="area.variant"
        :eyebrow="area.eyebrow"
        :title="area.title"
        :delay="idx * 140"
      >
        {{ area.body }}
      </SecondaryCard>
    </div>
  </section>
</template>

<script setup lang="ts">
import LandingSectionHeader from '@/components/landing/shared/sections/LandingSectionHeader.vue'
import SecondaryCard, { type SecondaryCardVariant } from '@/components/landing/shared/cards/SecondaryCard.vue'

interface FocusArea {
  readonly eyebrow: string
  readonly title: string
  readonly body: string
  readonly variant: SecondaryCardVariant
}

const AREAS: readonly FocusArea[] = [
  {
    eyebrow: 'Capability Building',
    title: 'Workforce Development',
    body: 'Initiatives that support workforce growth, industry needs, and talent development at every level — from entry to specialist.',
    variant: 'blue',
  },
  {
    eyebrow: 'In Motion',
    title: 'Featured Initiatives',
    body: 'Programs and projects making real impact today — across cities, sectors, and public–private partnerships.',
    variant: 'cyan',
  },
  {
    eyebrow: 'Way of Working',
    title: 'How We Collaborate',
    body: 'Shoulder-to-shoulder with clients, partners, and communities — co-designing workforce solutions that actually stick.',
    variant: 'red',
  },
  {
    eyebrow: 'On the Horizon',
    title: 'Future Initiatives',
    body: 'Exploring the opportunities and programs that will shape the next decade of work — and the people who power it.',
    variant: 'cyan',
  },
]
</script>

<style scoped>
.sp-focus {
  position: relative;
  width: 100%;
  margin-top: var(--panel-overlap);
  padding:
    var(--section-pad-y-lg)
    clamp(20px, 3vw, 64px);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: clamp(48px, 6vw, 80px);
  z-index: 5;
  border-radius:
    var(--r-brand-fluid) 0
    var(--r-brand-fluid) 0;
  box-shadow:
    0 -22px 40px -12px rgba(0, 0, 0, 0.45),
    0  22px 40px -12px rgba(0, 0, 0, 0.45);
  isolation: isolate;
  font-family: var(--font-family);
}

.sp-focus::before {
  content: '';
  position: absolute;
  top: -16px;
  bottom: -16px;
  left: 0;
  right: 0;
  z-index: -1;
  border-radius:
    var(--r-brand-fluid) 0
    var(--r-brand-fluid) 0;
  background: rgba(255, 255, 255, 0.07);
  backdrop-filter: var(--glass-blur-soft);
  -webkit-backdrop-filter: var(--glass-blur-soft);
  border: 1px solid var(--c-glass-border-soft);
  box-shadow: var(--sh-back);
  pointer-events: none;
}

.sp-focus__surface {
  position: absolute;
  inset: 0;
  z-index: 0;
  border-radius:
    var(--r-brand-fluid) 0
    var(--r-brand-fluid) 0;
  background: linear-gradient(
    180deg,
    rgba(9, 48, 85, 0.65) 0%,
    rgba(9, 48, 85, 0.55) 100%
  );
  backdrop-filter: blur(20px) saturate(160%);
  -webkit-backdrop-filter: blur(20px) saturate(160%);
  pointer-events: none;
}

.sp-focus__grid {
  position: relative;
  z-index: 2;
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: clamp(20px, 2.4vw, 32px);
  align-items: start;
  width: 100%;
  max-width: 1180px;
  margin: 0 auto;
}

@media (max-width: 1023px) {
  .sp-focus__grid { grid-template-columns: 1fr; }
}
</style>
