<template>
  <section id="talents-solutions" class="talents-solutions-v2">
    <!-- Inner glass surface — materializes the panel against GlobalBackground -->
    <div class="talents-solutions-v2__surface" aria-hidden="true"></div>

    <header class="talents-solutions-v2__header">
      <EyebrowPillV2 variant="white" class="talents-solutions-v2__eyebrow">
        Your Career, Your Way
      </EyebrowPillV2>

      <h2 class="talents-solutions-v2__heading">
        Two paths.
        <span class="talents-solutions-v2__heading-accent">
          One commitment to your growth.
        </span>
      </h2>

      <p class="talents-solutions-v2__subtitle">
        Whether you're after stability or variety, we'll match you with roles
        that keep moving your career forward.
      </p>
    </header>

    <div class="talents-solutions-v2__cards">
      <article
        v-for="(option, idx) in OPTIONS"
        :key="option.key"
        class="talents-solutions-v2__card"
        :class="`talents-solutions-v2__card--${option.tone}`"
      >
        <span class="talents-solutions-v2__index" aria-hidden="true">
          {{ formatIndex(idx) }}
        </span>

        <span class="talents-solutions-v2__card-eyebrow">
          {{ option.eyebrow }}
        </span>

        <h3 class="talents-solutions-v2__card-heading">{{ option.title }}</h3>

        <p class="talents-solutions-v2__card-body">{{ option.body }}</p>

        <span class="talents-solutions-v2__benefits-label">What you get</span>
        <ul class="talents-solutions-v2__benefits">
          <li
            v-for="benefit in option.benefits"
            :key="benefit"
          >{{ benefit }}</li>
        </ul>

        <router-link
          :to="option.ctaTo"
          class="talents-solutions-v2__cta"
        >
          <span>{{ option.ctaLabel }}</span>
          <span class="talents-solutions-v2__cta-arrow" aria-hidden="true">→</span>
        </router-link>
      </article>
    </div>
  </section>
</template>

<script setup lang="ts">
/**
 * Talents — "Your Career, Your Way" section.
 *
 * Two CTA cards (Direct Hiring + Contract) inside the panel vocabulary
 * (TL+BR radius, dual shadow, margin-top overlap, glass surface). Aligned
 * horizontally — not staggered like AboutMissionSection — because these
 * cards are action prompts, not declarations.
 *
 * Copy is written from the talent's perspective (the Figma legacy had it
 * from the employer's perspective, which doesn't fit the Talents page).
 */
import EyebrowPillV2 from '@/components/v2/shared/EyebrowPillV2.vue'

type Tone = 'cyan' | 'red'

interface CareerOption {
  readonly key: string
  readonly eyebrow: string
  readonly title: string
  readonly body: string
  readonly benefits: readonly string[]
  readonly ctaLabel: string
  readonly ctaTo: string
  readonly tone: Tone
}

const OPTIONS: readonly CareerOption[] = [
  {
    key: 'direct-hiring',
    eyebrow: 'Long-Term Career',
    title: 'Direct Hiring',
    body:
      'Build a lasting career with permanent placements at companies that invest in your growth — full benefits, real progression, lasting team relationships.',
    benefits: [
      'Permanent placement',
      'Full benefits package',
      'Clear career progression',
    ],
    ctaLabel: 'Browse direct hires',
    ctaTo: '/v2/open-positions',
    tone: 'cyan',
  },
  {
    key: 'contract',
    eyebrow: 'Flexibility & Reach',
    title: 'Contract',
    body:
      'Project-based roles that let you choose your pace, diversify your skills, and work across the industries that interest you most — without the long-term tie-down.',
    benefits: [
      'Variety of projects',
      'Faster skill expansion',
      'Multi-industry exposure',
    ],
    ctaLabel: 'Browse contracts',
    ctaTo: '/v2/open-positions',
    tone: 'red',
  },
] as const

function formatIndex(idx: number): string {
  return String(idx + 1).padStart(2, '0')
}
</script>

<style scoped>
/* ── Panel shell ────────────────────────────────────────────────────────── */
.talents-solutions-v2 {
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

.talents-solutions-v2__surface {
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
.talents-solutions-v2__header {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  max-width: 880px;
  margin: 0 auto;
}

.talents-solutions-v2__eyebrow {
  margin-bottom: clamp(20px, 2.5vw, 28px);
}

.talents-solutions-v2__heading {
  font-size: clamp(28px, 4.2vw, 46px);
  font-weight: 700;
  line-height: 1.15;
  letter-spacing: -0.02em;
  color: #fff;
  margin: 0 0 clamp(14px, 1.8vw, 22px);
  max-width: 780px;
  text-shadow: 0 4px 24px rgba(0, 0, 0, 0.40);
}

.talents-solutions-v2__heading-accent {
  color: var(--c-brand-cyan);
  display: block;
}

.talents-solutions-v2__subtitle {
  font-size: clamp(13px, 1.2vw, 16px);
  font-weight: 400;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.78);
  margin: 0;
  max-width: 560px;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.30);
}

/* ── Cards grid — 2 cols desktop, stack mobile ──────────────────────────── */
.talents-solutions-v2__cards {
  position: relative;
  z-index: 2;
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: clamp(24px, 3.2vw, 40px);
  width: 100%;
  max-width: 1180px;
  margin: 0 auto;
}

/* ── Card shell ─────────────────────────────────────────────────────────── */
.talents-solutions-v2__card {
  position: relative;
  display: flex;
  flex-direction: column;
  gap: clamp(12px, 1.4vw, 18px);
  padding:
    clamp(36px, 4.2vw, 56px)
    clamp(28px, 3.2vw, 44px);
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

.talents-solutions-v2__card:hover {
  background: rgba(255, 255, 255, 0.075);
  border-color: rgba(255, 255, 255, 0.22);
  transform: translateY(-4px);
}

/* Mirrored asymmetric radius — Direct Hiring rounds TL+BR, Contract rounds TR+BL */
.talents-solutions-v2__card--cyan {
  border-radius:
    clamp(48px, 6.5vw, 88px) 0
    clamp(48px, 6.5vw, 88px) 0;
}

.talents-solutions-v2__card--red {
  border-radius:
    0 clamp(48px, 6.5vw, 88px)
    0 clamp(48px, 6.5vw, 88px);
}

/* ── Ghost numeral ──────────────────────────────────────────────────────── */
.talents-solutions-v2__index {
  position: absolute;
  top: clamp(-6px, -0.6vw, 4px);
  right: clamp(14px, 2.2vw, 32px);
  z-index: 0;
  font-size: clamp(120px, 16vw, 200px);
  font-weight: 800;
  line-height: 0.85;
  letter-spacing: -0.04em;
  color: rgba(255, 255, 255, 0.07);
  pointer-events: none;
  user-select: none;
}

.talents-solutions-v2__card--cyan .talents-solutions-v2__index {
  color: rgba(0, 173, 239, 0.10);
}

.talents-solutions-v2__card--red .talents-solutions-v2__index {
  color: rgba(229, 45, 39, 0.10);
}

/* ── Card eyebrow ──────────────────────────────────────────────────────── */
.talents-solutions-v2__card-eyebrow {
  position: relative;
  z-index: 1;
  display: inline-block;
  font-size: clamp(10px, 0.85vw, 12px);
  font-weight: 700;
  letter-spacing: 0.22em;
  text-transform: uppercase;
  margin-bottom: clamp(4px, 0.5vw, 8px);
}

.talents-solutions-v2__card--cyan .talents-solutions-v2__card-eyebrow { color: var(--c-brand-cyan); }
.talents-solutions-v2__card--red  .talents-solutions-v2__card-eyebrow { color: var(--c-brand-red);  }

/* ── Card heading ───────────────────────────────────────────────────────── */
.talents-solutions-v2__card-heading {
  position: relative;
  z-index: 1;
  font-size: clamp(24px, 2.6vw, 34px);
  font-weight: 700;
  line-height: 1.2;
  letter-spacing: -0.01em;
  color: #fff;
  margin: 0;
  text-shadow: 0 2px 14px rgba(0, 0, 0, 0.30);
}

/* ── Card body ─────────────────────────────────────────────────────────── */
.talents-solutions-v2__card-body {
  position: relative;
  z-index: 1;
  font-size: clamp(13px, 1.15vw, 15px);
  font-weight: 400;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.82);
  margin: 0 0 clamp(8px, 1vw, 14px);
}

/* ── Benefits list ──────────────────────────────────────────────────────── */
.talents-solutions-v2__benefits-label {
  position: relative;
  z-index: 1;
  display: inline-block;
  font-size: clamp(10px, 0.8vw, 11px);
  font-weight: 700;
  letter-spacing: 0.22em;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.60);
  margin-bottom: clamp(4px, 0.5vw, 8px);
}

.talents-solutions-v2__benefits {
  position: relative;
  z-index: 1;
  list-style: none;
  padding: 0;
  margin: 0 0 clamp(20px, 2.4vw, 32px);
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.talents-solutions-v2__benefits li {
  position: relative;
  padding-left: 18px;
  font-size: clamp(13px, 1.1vw, 14px);
  color: rgba(255, 255, 255, 0.85);
  line-height: 1.5;
}

.talents-solutions-v2__benefits li::before {
  content: '';
  position: absolute;
  left: 0;
  top: 0.55em;
  width: 7px;
  height: 7px;
  border-radius: 50%;
}

.talents-solutions-v2__card--cyan .talents-solutions-v2__benefits li::before {
  background: var(--c-brand-cyan);
}

.talents-solutions-v2__card--red .talents-solutions-v2__benefits li::before {
  background: var(--c-brand-red);
}

/* ── CTA pill ──────────────────────────────────────────────────────────── */
.talents-solutions-v2__cta {
  position: relative;
  z-index: 1;
  margin-top: auto;
  align-self: flex-start;
  display: inline-flex;
  align-items: center;
  gap: 10px;
  padding: clamp(11px, 1.2vw, 14px) clamp(22px, 2.4vw, 30px);
  background: rgba(255, 255, 255, 0.10);
  border: 1px solid rgba(255, 255, 255, 0.40);
  border-radius: 999px;
  color: #fff;
  font-family: var(--font-family);
  font-size: clamp(13px, 1.05vw, 14px);
  font-weight: 600;
  letter-spacing: 0.04em;
  text-decoration: none;
  cursor: pointer;
  transition:
    background 0.25s ease,
    border-color 0.25s ease,
    color 0.25s ease,
    transform 0.25s ease;
}

.talents-solutions-v2__card--cyan .talents-solutions-v2__cta:hover {
  background: var(--c-brand-cyan);
  border-color: var(--c-brand-cyan);
  color: var(--c-brand-navy);
  transform: translateX(4px);
}

.talents-solutions-v2__card--red .talents-solutions-v2__cta:hover {
  background: var(--c-brand-red);
  border-color: var(--c-brand-red);
  color: #fff;
  transform: translateX(4px);
}

.talents-solutions-v2__cta-arrow {
  font-size: 1.15em;
  font-weight: 700;
  line-height: 1;
  transition: transform 0.25s ease;
}

.talents-solutions-v2__cta:hover .talents-solutions-v2__cta-arrow {
  transform: translateX(3px);
}

/* ── Mobile-only behaviors ──────────────────────────────────────────────── */
@media (max-width: 899px) {
  .talents-solutions-v2__cards { grid-template-columns: 1fr; }
}
</style>
