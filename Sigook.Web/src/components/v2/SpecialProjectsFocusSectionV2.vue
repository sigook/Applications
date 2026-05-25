<template>
  <section id="sp-focus" class="sp-focus-v2">
    <!-- Inner glass surface — materializes the panel against GlobalBackground -->
    <div class="sp-focus-v2__surface" aria-hidden="true"></div>

    <header class="sp-focus-v2__header">
      <EyebrowPillV2 variant="white" class="sp-focus-v2__eyebrow">
        Our Focus Areas
      </EyebrowPillV2>

      <h2 class="sp-focus-v2__heading">
        Four ways our work
        <span class="sp-focus-v2__heading-accent">moves communities forward.</span>
      </h2>

      <p class="sp-focus-v2__subtitle">
        Each initiative weaves workforce, technology, and community engagement
        into outcomes that outlast a single placement.
      </p>
    </header>

    <div class="sp-focus-v2__grid">
      <FeatureCardV2
        v-for="(area, idx) in AREAS"
        :key="area.title"
        :variant="area.variant"
        :eyebrow="area.eyebrow"
        :title="area.title"
        :delay="idx * 140"
      >
        {{ area.body }}
      </FeatureCardV2>
    </div>
  </section>
</template>

<script setup lang="ts">
/**
 * Special Projects — Focus Areas section.
 *
 * Four FeatureCardV2 cards (canonical Home pattern) in a 2x2 grid. Variant
 * cycle blue → cyan → red → cyan so the brand palette stays balanced across
 * the panel.
 */
import EyebrowPillV2 from '@/components/v2/shared/EyebrowPillV2.vue'
import FeatureCardV2, { type FeatureCardVariant } from '@/components/v2/shared/FeatureCardV2.vue'

interface FocusArea {
  readonly eyebrow: string
  readonly title: string
  readonly body: string
  readonly variant: FeatureCardVariant
}

const AREAS: readonly FocusArea[] = [
  {
    eyebrow: 'Capability Building',
    title: 'Workforce Development',
    body:
      'Initiatives that support workforce growth, industry needs, and talent development at every level — from entry to specialist.',
    variant: 'blue',
  },
  {
    eyebrow: 'In Motion',
    title: 'Featured Initiatives',
    body:
      'Programs and projects making real impact today — across cities, sectors, and public–private partnerships.',
    variant: 'cyan',
  },
  {
    eyebrow: 'Way of Working',
    title: 'How We Collaborate',
    body:
      'Shoulder-to-shoulder with clients, partners, and communities — co-designing workforce solutions that actually stick.',
    variant: 'red',
  },
  {
    eyebrow: 'On the Horizon',
    title: 'Future Initiatives',
    body:
      'Exploring the opportunities and programs that will shape the next decade of work — and the people who power it.',
    variant: 'cyan',
  },
] as const
</script>

<style scoped>
/* ── Panel shell ────────────────────────────────────────────────────────── */
.sp-focus-v2 {
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

.sp-focus-v2__surface {
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
.sp-focus-v2__header {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  max-width: 880px;
  margin: 0 auto;
}

.sp-focus-v2__eyebrow {
  margin-bottom: clamp(20px, 2.5vw, 28px);
}

.sp-focus-v2__heading {
  font-size: clamp(28px, 4.2vw, 46px);
  font-weight: 700;
  line-height: 1.15;
  letter-spacing: -0.02em;
  color: #fff;
  margin: 0 0 clamp(14px, 1.8vw, 22px);
  max-width: 780px;
  text-shadow: 0 4px 24px rgba(0, 0, 0, 0.40);
}

.sp-focus-v2__heading-accent {
  color: var(--c-brand-cyan);
  display: block;
}

.sp-focus-v2__subtitle {
  font-size: clamp(13px, 1.2vw, 16px);
  font-weight: 400;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.78);
  margin: 0;
  max-width: 560px;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.30);
}

/* ── Cards grid — 2x2 desktop, 1 col mobile ─────────────────────────────── */
.sp-focus-v2__grid {
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

/* ── Mobile-only behaviors ──────────────────────────────────────────────── */
@media (max-width: 899px) {
  .sp-focus-v2__grid { grid-template-columns: 1fr; }
}
</style>
