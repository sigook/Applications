<template>
  <div class="numbers-grid-v2">
    <article
      v-for="(stat, idx) in stats"
      :key="stat.label"
      class="numbers-grid-v2__card"
      :class="[
        `numbers-grid-v2__card--${stat.tone}`,
        idx % 2 === 0
          ? 'numbers-grid-v2__card--shape-a'
          : 'numbers-grid-v2__card--shape-b',
      ]"
    >
      <span class="numbers-grid-v2__accent" aria-hidden="true"></span>

      <span class="numbers-grid-v2__num">
        <span class="numbers-grid-v2__plus" aria-hidden="true">+</span>{{ stat.value }}
      </span>

      <span class="numbers-grid-v2__lbl">{{ stat.label }}</span>
    </article>
  </div>
</template>

<script lang="ts">
/**
 * Shared types for NumbersGridV2. Lives in a regular <script> block so
 * parents can `import type { Stat } from '@/components/v2/NumbersGridV2.vue'`
 * and tightly type their data arrays.
 */
export type StatTone = 'cyan' | 'red'

export interface Stat {
  readonly value: string
  readonly label: string
  readonly tone: StatTone
}
</script>

<script setup lang="ts">
/**
 * NumbersGridV2 — 4 editorial glass stat cards with asymmetric brand radius.
 *
 * Replaces the legacy "petal cluster" 2×2 of red/blue rounded shapes which
 * didn't match the rest of the V2 vocabulary (glass + asymmetric radius +
 * cyan/red accents). Each card carries its accent tone via a small top line
 * and the leading "+" sign — keeps the brand duality without shouting.
 *
 * Layout: 4 columns desktop → 2 columns tablet → 1 column small mobile.
 *
 * Data lives in the parent — pass an array of Stat via the `stats` prop.
 * The grid renders as many cards as items provided; alternating shape and
 * tone are driven by item index + the tone field on each stat.
 */
defineProps<{
  stats: readonly Stat[]
}>()
</script>

<style scoped>
/* ── Grid shell — fluid 4-col → 2-col → 1-col responsive ────────────────── */
.numbers-grid-v2 {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: clamp(16px, 1.8vw, 28px);
  width: 100%;
  max-width: 1180px;
  margin: 0 auto;
  font-family: var(--font-family);
}

/* ── Card shell — glass surface, editorial vertical stack ───────────────── */
.numbers-grid-v2__card {
  position: relative;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: clamp(12px, 1.4vw, 20px);
  padding:
    clamp(28px, 3.4vw, 44px)
    clamp(22px, 2.6vw, 32px);
  background: rgba(255, 255, 255, 0.045);
  border: 1px solid rgba(255, 255, 255, 0.10);
  backdrop-filter: blur(14px) saturate(140%);
  -webkit-backdrop-filter: blur(14px) saturate(140%);
  isolation: isolate;
  transition:
    background 0.35s ease,
    border-color 0.35s ease,
    transform 0.35s cubic-bezier(0.22, 1, 0.36, 1);
}

.numbers-grid-v2__card:hover {
  background: rgba(255, 255, 255, 0.075);
  border-color: rgba(255, 255, 255, 0.22);
  transform: translateY(-4px);
}

/* Asymmetric brand radius — alternates per card position for visual rhythm */
.numbers-grid-v2__card--shape-a {
  border-radius:
    clamp(28px, 3.4vw, 48px) 0
    clamp(28px, 3.4vw, 48px) 0;
}

.numbers-grid-v2__card--shape-b {
  border-radius:
    0 clamp(28px, 3.4vw, 48px)
    0 clamp(28px, 3.4vw, 48px);
}

/* ── Top accent line — short tinted bar anchored top-left ───────────────── */
.numbers-grid-v2__accent {
  width: clamp(36px, 4.2vw, 56px);
  height: 3px;
  border-radius: 999px;
}

.numbers-grid-v2__card--cyan .numbers-grid-v2__accent {
  background: var(--c-brand-cyan);
  box-shadow: 0 0 14px rgba(0, 173, 239, 0.55);
}

.numbers-grid-v2__card--red .numbers-grid-v2__accent {
  background: var(--c-brand-red);
  box-shadow: 0 0 14px rgba(229, 45, 39, 0.55);
}

/* ── Number — large editorial figure ────────────────────────────────────── */
.numbers-grid-v2__num {
  font-size: clamp(36px, 4.5vw, 60px);
  font-weight: 800;
  line-height: 1;
  letter-spacing: -0.02em;
  color: #fff;
  white-space: nowrap;
  text-shadow: 0 4px 18px rgba(0, 0, 0, 0.30);
}

.numbers-grid-v2__plus {
  display: inline-block;
  margin-right: clamp(2px, 0.2vw, 4px);
  font-weight: 700;
}

.numbers-grid-v2__card--cyan .numbers-grid-v2__plus { color: var(--c-brand-cyan); }
.numbers-grid-v2__card--red  .numbers-grid-v2__plus { color: var(--c-brand-red);  }

/* ── Label — uppercase tracked muted ────────────────────────────────────── */
.numbers-grid-v2__lbl {
  font-size: clamp(11px, 0.95vw, 13px);
  font-weight: 600;
  letter-spacing: 0.22em;
  text-transform: uppercase;
  line-height: 1.4;
  color: rgba(255, 255, 255, 0.72);
}

/* ── Mobile-only behaviors (clamp can't express grid-column shifts) ─────── */
@media (max-width: 899px) {
  .numbers-grid-v2 { grid-template-columns: 1fr 1fr; }
}

@media (max-width: 480px) {
  .numbers-grid-v2 { grid-template-columns: 1fr; }
}
</style>
