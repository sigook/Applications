<template>
  <section id="about-numbers" class="about-numbers">
    <div class="about-numbers__surface" aria-hidden="true"></div>

    <LandingSectionHeader
      eyebrow="By the Numbers"
      eyebrow-variant="cyan"
      heading="A track record built"
      heading-accent="connection by connection."
      subtitle="Seventeen years of bringing people and opportunities together — and the numbers behind every story."
      heading-margin-bottom="clamp(16px, 2vw, 24px)"
    />

    <NumbersGrid :stats="ABOUT_STATS" class="about-numbers__grid" />
  </section>
</template>

<script setup lang="ts">
import LandingSectionHeader from '@/components/landing/shared/LandingSectionHeader.vue'
import NumbersGrid, { type Stat } from '@/components/landing/shared/NumbersGrid.vue'

const ABOUT_STATS: readonly Stat[] = [
  { value: '10',    label: 'Years of Experience', tone: 'blue' },
  { value: '780',   label: 'Clients Served',      tone: 'cyan' },
  { value: '1,700', label: 'Jobs Posted',         tone: 'red'  },
  { value: '5,000', label: 'Applications Filed',  tone: 'cyan' },
] as const
</script>

<style scoped>
.about-numbers {
  position: relative;
  width: 100%;
  margin-top: var(--panel-overlap);
  padding:
    var(--section-pad-y-lg)
    clamp(20px, 3vw, 64px);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: clamp(36px, 5vw, 64px);
  z-index: 5;
  border-radius:
    var(--r-brand-fluid) 0
    var(--r-brand-fluid) 0;
  box-shadow:
    0 -22px 40px -12px rgba(0, 0, 0, 0.45),
    0  22px 40px -12px rgba(0, 0, 0, 0.45);
  isolation: isolate;
}

.about-numbers::before {
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

.about-numbers__surface {
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

.about-numbers__grid {
  position: relative;
  z-index: 2;
  margin: 0 auto;
}
</style>
