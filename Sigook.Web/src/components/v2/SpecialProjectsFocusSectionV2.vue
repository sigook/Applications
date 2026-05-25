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
      <article
        v-for="(area, idx) in AREAS"
        :key="area.title"
        class="sp-focus-v2__card"
        :class="[
          `sp-focus-v2__card--${area.tone}`,
          idx % 2 === 0
            ? 'sp-focus-v2__card--shape-a'
            : 'sp-focus-v2__card--shape-b',
        ]"
      >
        <span class="sp-focus-v2__index" aria-hidden="true">{{ formatIndex(idx) }}</span>

        <span class="sp-focus-v2__card-eyebrow">{{ area.eyebrow }}</span>

        <h3 class="sp-focus-v2__card-heading">{{ area.title }}</h3>

        <p class="sp-focus-v2__card-body">{{ area.body }}</p>
      </article>
    </div>
  </section>
</template>

<script setup lang="ts">
/**
 * Special Projects — Focus Areas section.
 *
 * Adopts the panel vocabulary established by DualCtaSectionV2 (Home),
 * AboutNumbersSectionV2 and WhyWorkWithUsSectionV2: asymmetric brand
 * radius shell (TL + BR), dual top/bottom drop shadows, negative
 * margin-top for overlap, inner glass navy surface.
 *
 * Inside the panel: editorial header + 4 glass cards in a 2x2 grid with
 * magazine-style ghost indices (01-04), alternating cyan/red accent
 * tones and mirrored asymmetric corners for visual rhythm.
 *
 * Renames the legacy first card "Our Focus Areas" to "Workforce
 * Development" — that title already lives in the section eyebrow and
 * duplicating it inside a card was redundant.
 */
import EyebrowPillV2 from '@/components/v2/shared/EyebrowPillV2.vue'

type Tone = 'cyan' | 'red'

interface FocusArea {
  readonly eyebrow: string
  readonly title: string
  readonly body: string
  readonly tone: Tone
}

const AREAS: readonly FocusArea[] = [
  {
    eyebrow: 'Capability Building',
    title: 'Workforce Development',
    body:
      'Initiatives that support workforce growth, industry needs, and talent development at every level — from entry to specialist.',
    tone: 'cyan',
  },
  {
    eyebrow: 'In Motion',
    title: 'Featured Initiatives',
    body:
      'Programs and projects making real impact today — across cities, sectors, and public–private partnerships.',
    tone: 'red',
  },
  {
    eyebrow: 'Way of Working',
    title: 'How We Collaborate',
    body:
      'Shoulder-to-shoulder with clients, partners, and communities — co-designing workforce solutions that actually stick.',
    tone: 'cyan',
  },
  {
    eyebrow: 'On the Horizon',
    title: 'Future Initiatives',
    body:
      'Exploring the opportunities and programs that will shape the next decade of work — and the people who power it.',
    tone: 'red',
  },
] as const

function formatIndex(idx: number): string {
  return String(idx + 1).padStart(2, '0')
}
</script>

<style scoped>
/* ── Panel shell — adopts DualCta vocabulary (TL+BR radius, dual shadow) ── */
.sp-focus-v2 {
  position: relative;
  width: 100%;
  /* Overlap previous section (Hero) — same vocab as Numbers/WhyWork panels */
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

/* ── Inner glass surface ────────────────────────────────────────────────── */
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

/* ── Header block ───────────────────────────────────────────────────────── */
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
  width: 100%;
  max-width: 1180px;
  margin: 0 auto;
}

/* ── Feature card ──────────────────────────────────────────────────────── */
.sp-focus-v2__card {
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

.sp-focus-v2__card:hover {
  background: rgba(255, 255, 255, 0.07);
  border-color: rgba(255, 255, 255, 0.20);
  transform: translateY(-4px);
}

/* Alternating asymmetric brand radius — diagonal mirror for visual rhythm */
.sp-focus-v2__card--shape-a {
  border-radius:
    clamp(40px, 5.5vw, 72px) 0
    clamp(40px, 5.5vw, 72px) 0;
}

.sp-focus-v2__card--shape-b {
  border-radius:
    0 clamp(40px, 5.5vw, 72px)
    0 clamp(40px, 5.5vw, 72px);
}

/* ── Ghost numeral 01-04 — magazine accent ──────────────────────────────── */
.sp-focus-v2__index {
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

.sp-focus-v2__card--cyan .sp-focus-v2__index {
  color: rgba(0, 173, 239, 0.10);
}

.sp-focus-v2__card--red .sp-focus-v2__index {
  color: rgba(229, 45, 39, 0.10);
}

/* ── Card eyebrow ──────────────────────────────────────────────────────── */
.sp-focus-v2__card-eyebrow {
  position: relative;
  z-index: 1;
  display: inline-block;
  font-size: clamp(10px, 0.85vw, 12px);
  font-weight: 700;
  letter-spacing: 0.22em;
  text-transform: uppercase;
  margin-bottom: clamp(4px, 0.5vw, 8px);
}

.sp-focus-v2__card--cyan .sp-focus-v2__card-eyebrow { color: var(--c-brand-cyan); }
.sp-focus-v2__card--red  .sp-focus-v2__card-eyebrow { color: var(--c-brand-red);  }

/* ── Card heading ───────────────────────────────────────────────────────── */
.sp-focus-v2__card-heading {
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
.sp-focus-v2__card-body {
  position: relative;
  z-index: 1;
  font-size: clamp(13px, 1.15vw, 15px);
  font-weight: 400;
  line-height: 1.65;
  color: rgba(255, 255, 255, 0.78);
  margin: 0;
}

/* ── Mobile-only behaviors (grid column shift) ──────────────────────────── */
@media (max-width: 899px) {
  .sp-focus-v2__grid { grid-template-columns: 1fr; }
}
</style>
