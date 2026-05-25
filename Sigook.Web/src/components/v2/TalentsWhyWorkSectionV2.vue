<template>
  <section class="talents-why-v2">
    <header class="talents-why-v2__header">
      <EyebrowPillV2 variant="white" class="talents-why-v2__eyebrow">
        Why Work With Us
      </EyebrowPillV2>

      <h2 class="talents-why-v2__heading">
        Three things
        <span class="talents-why-v2__heading-accent">you'll feel from day one.</span>
      </h2>

      <p class="talents-why-v2__subtitle">
        It's not just placement. It's a relationship — built on local presence,
        genuine care, and a recruiter who actually knows your industry.
      </p>
    </header>

    <div class="talents-why-v2__grid">
      <article
        v-for="(reason, idx) in REASONS"
        :key="reason.title"
        class="talents-why-v2__card"
        :class="[
          `talents-why-v2__card--${reason.tone}`,
          { 'talents-why-v2__card--offset': idx === 1 },
        ]"
      >
        <span class="talents-why-v2__index" aria-hidden="true">
          {{ formatIndex(idx) }}
        </span>

        <span class="talents-why-v2__card-eyebrow">{{ reason.eyebrow }}</span>

        <h3 class="talents-why-v2__card-heading">{{ reason.title }}</h3>

        <p class="talents-why-v2__card-body">{{ reason.body }}</p>
      </article>
    </div>
  </section>
</template>

<script setup lang="ts">
/**
 * Talents — "Why Work With Us" section.
 *
 * WINDOW-style section (transparent over GlobalBackground) — sits between
 * the Solutions panel (above) and the Industries carousel panel (below) to
 * keep the page rhythm: window → panel → window → panel → window.
 *
 * Three glass cards with a "valley" stagger (middle card sits lower) keep
 * a distinct rhythm from the About page's WhyWorkWithUs grid. Same family,
 * different beat.
 *
 * Copy is rewritten from the talent's perspective — the Figma's three blocks
 * (Working where you are / We care about you / Focused on your experience)
 * become editorial cards with thematic eyebrows and tightened body copy.
 */
import EyebrowPillV2 from '@/components/v2/shared/EyebrowPillV2.vue'

type Tone = 'cyan' | 'red'

interface Reason {
  readonly eyebrow: string
  readonly title: string
  readonly body: string
  readonly tone: Tone
}

const REASONS: readonly Reason[] = [
  {
    eyebrow: 'Local Reach',
    title: 'Working where you are',
    body:
      'With clients located across multiple states and provinces, you\'ll find roles that match your skills and interests — and build the relationships that turn placement into long-term stability.',
    tone: 'cyan',
  },
  {
    eyebrow: 'Human-First',
    title: 'We care about you',
    body:
      'You\'re not just a candidate. Your aspirations and vision define how we work — we take the time to understand where you want to go before we recommend a single opportunity.',
    tone: 'red',
  },
  {
    eyebrow: 'Sector Expertise',
    title: 'Focused on your experience',
    body:
      'Our recruiters specialize in the sectors where your skills are most valued. They know each industry\'s rhythms, language, and what excellent looks like — and they match you accordingly.',
    tone: 'cyan',
  },
] as const

function formatIndex(idx: number): string {
  return String(idx + 1).padStart(2, '0')
}
</script>

<style scoped>
/* ── Window shell — transparent (GlobalBackground shows through) ────────── */
.talents-why-v2 {
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

/* ── Header ─────────────────────────────────────────────────────────────── */
.talents-why-v2__header {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  max-width: 880px;
  margin: 0 auto;
}

.talents-why-v2__eyebrow {
  margin-bottom: clamp(20px, 2.5vw, 28px);
}

.talents-why-v2__heading {
  font-size: clamp(28px, 4.2vw, 46px);
  font-weight: 700;
  line-height: 1.15;
  letter-spacing: -0.02em;
  color: #fff;
  margin: 0 0 clamp(14px, 1.8vw, 22px);
  max-width: 780px;
  text-shadow: 0 4px 24px rgba(0, 0, 0, 0.40);
}

.talents-why-v2__heading-accent {
  color: var(--c-brand-cyan);
  display: block;
}

.talents-why-v2__subtitle {
  font-size: clamp(13px, 1.2vw, 16px);
  font-weight: 400;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.78);
  margin: 0;
  max-width: 560px;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.30);
}

/* ── Cards grid — 3 cols desktop, stack mobile ──────────────────────────── */
.talents-why-v2__grid {
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

/* ── Card shell ─────────────────────────────────────────────────────────── */
.talents-why-v2__card {
  position: relative;
  display: flex;
  flex-direction: column;
  gap: clamp(12px, 1.4vw, 18px);
  padding:
    clamp(32px, 4vw, 56px)
    clamp(26px, 3vw, 40px);
  background: rgba(255, 255, 255, 0.045);
  border: 1px solid rgba(255, 255, 255, 0.10);
  backdrop-filter: blur(14px) saturate(140%);
  -webkit-backdrop-filter: blur(14px) saturate(140%);
  overflow: hidden;
  isolation: isolate;
  transition:
    background 0.35s ease,
    border-color 0.35s ease,
    transform 0.35s cubic-bezier(0.22, 1, 0.36, 1);
}

.talents-why-v2__card:hover {
  background: rgba(255, 255, 255, 0.075);
  border-color: rgba(255, 255, 255, 0.22);
  transform: translateY(-4px);
}

/* Alternating asymmetric brand radius */
.talents-why-v2__card:nth-child(odd) {
  border-radius:
    clamp(40px, 5.5vw, 72px) 0
    clamp(40px, 5.5vw, 72px) 0;
}

.talents-why-v2__card:nth-child(even) {
  border-radius:
    0 clamp(40px, 5.5vw, 72px)
    0 clamp(40px, 5.5vw, 72px);
}

/* "Valley" stagger — middle card drops to break the rigid 3-col rhythm */
.talents-why-v2__card--offset {
  margin-top: clamp(36px, 5vw, 72px);
}

/* ── Ghost numeral ──────────────────────────────────────────────────────── */
.talents-why-v2__index {
  position: absolute;
  top: clamp(-6px, -0.6vw, 2px);
  right: clamp(10px, 1.8vw, 24px);
  z-index: 0;
  font-size: clamp(110px, 14vw, 180px);
  font-weight: 800;
  line-height: 0.85;
  letter-spacing: -0.04em;
  color: rgba(255, 255, 255, 0.07);
  pointer-events: none;
  user-select: none;
}

.talents-why-v2__card--cyan .talents-why-v2__index {
  color: rgba(0, 173, 239, 0.10);
}

.talents-why-v2__card--red .talents-why-v2__index {
  color: rgba(229, 45, 39, 0.10);
}

/* ── Card eyebrow ──────────────────────────────────────────────────────── */
.talents-why-v2__card-eyebrow {
  position: relative;
  z-index: 1;
  display: inline-block;
  font-size: clamp(10px, 0.85vw, 12px);
  font-weight: 700;
  letter-spacing: 0.22em;
  text-transform: uppercase;
  margin-bottom: clamp(4px, 0.5vw, 8px);
}

.talents-why-v2__card--cyan .talents-why-v2__card-eyebrow { color: var(--c-brand-cyan); }
.talents-why-v2__card--red  .talents-why-v2__card-eyebrow { color: var(--c-brand-red);  }

/* ── Card heading ───────────────────────────────────────────────────────── */
.talents-why-v2__card-heading {
  position: relative;
  z-index: 1;
  font-size: clamp(20px, 2.2vw, 28px);
  font-weight: 700;
  line-height: 1.25;
  letter-spacing: -0.01em;
  color: #fff;
  margin: 0;
  text-shadow: 0 2px 14px rgba(0, 0, 0, 0.30);
}

/* ── Card body ─────────────────────────────────────────────────────────── */
.talents-why-v2__card-body {
  position: relative;
  z-index: 1;
  font-size: clamp(13px, 1.15vw, 15px);
  font-weight: 400;
  line-height: 1.65;
  color: rgba(255, 255, 255, 0.78);
  margin: 0;
}

/* ── Mobile-only behaviors ──────────────────────────────────────────────── */
@media (max-width: 899px) {
  .talents-why-v2__grid { grid-template-columns: 1fr; }

  /* Drop the valley offset on stacked layout — makes no visual sense in 1 col */
  .talents-why-v2__card--offset { margin-top: 0; }
}
</style>
