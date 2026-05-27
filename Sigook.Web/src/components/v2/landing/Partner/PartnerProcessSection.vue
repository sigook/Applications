<template>
  <section class="partner-process">
    <!-- Inner glass surface — materializes the panel against GlobalBackground -->
    <div class="partner-process__surface" aria-hidden="true"></div>

    <header class="partner-process__header">
      <EyebrowPill variant="white" class="partner-process__eyebrow">
        How It Works
      </EyebrowPill>

      <h2 class="partner-process__heading">
        From apply to first commission —
        <span class="partner-process__heading-accent">
          typically four weeks.
        </span>
      </h2>

      <p class="partner-process__subtitle">
        We've stripped the partner onboarding down to four steps. No
        12-week certification tracks, no scattered orientation calls — a
        clear path from application to your first paid placement.
      </p>
    </header>

    <ol class="partner-process__steps" aria-label="Partner onboarding steps">
      <li
        v-for="(step, idx) in STEPS"
        :key="step.title"
        ref="stepRefs"
        class="partner-process__step"
        :class="[
          `partner-process__step--${step.tone}`,
          { 'partner-process__step--visible': visibleSteps[idx] },
        ]"
        :style="{ transitionDelay: `${idx * 140}ms` }"
      >
        <!-- Big number — the visual anchor of each card -->
        <span class="partner-process__number" aria-hidden="true">
          {{ String(idx + 1).padStart(2, '0') }}
        </span>

        <div class="partner-process__step-body">
          <span class="partner-process__step-eyebrow">{{ step.eyebrow }}</span>
          <h3 class="partner-process__step-title">{{ step.title }}</h3>
          <p class="partner-process__step-text">{{ step.body }}</p>
        </div>
      </li>
    </ol>
  </section>
</template>

<script setup lang="ts">
/**
 * Partner — "How it works" 4-step onboarding section.
 *
 * Panel-style (mirror border-radius from Types section → TR+BL to create
 * a zig-zag rhythm between the two panels of the page).
 *
 * Each step is an ordered-list item with:
 *  • A giant 2-digit number (01..04) as the visual anchor
 *  • Eyebrow tag (timing or label)
 *  • Title (the verb of the step)
 *  • Short body
 *
 * Steps fade up with IntersectionObserver stagger — handled manually here
 * because the wrapper isn't a SecondaryCard.
 */
import { ref, onMounted, onUnmounted } from 'vue'
import EyebrowPill from '@/components/v2/landing/shared/EyebrowPill.vue'

interface Step {
  readonly eyebrow: string
  readonly title: string
  readonly body: string
  readonly tone: 'cyan' | 'blue' | 'red'
}

const STEPS: readonly Step[] = [
  {
    eyebrow: 'Day 1',
    title: 'Apply',
    body:
      'Tell us about your specialty, your network, and the track you want. A partner success manager replies within two business days.',
    tone: 'cyan',
  },
  {
    eyebrow: 'Week 1',
    title: 'Onboard',
    body:
      'One half-day session: compliance setup, platform access, payroll wiring, and a walkthrough of our active client list.',
    tone: 'blue',
  },
  {
    eyebrow: 'Weeks 2–3',
    title: 'Match',
    body:
      'You source candidates or open client conversations. We back you with templates, market data, and a recruiter buddy for the first deal.',
    tone: 'cyan',
  },
  {
    eyebrow: 'Week 4+',
    title: 'Earn',
    body:
      'Your placement closes. Revenue is calculated transparently, paid on the same cycle as our employees — and it compounds from there.',
    tone: 'red',
  },
] as const

// Per-step intersection observer so the cards fade up in sequence.
const stepRefs = ref<HTMLElement[]>([])
const visibleSteps = ref<boolean[]>(STEPS.map(() => false))
let observer: IntersectionObserver | null = null

onMounted(() => {
  if (typeof IntersectionObserver === 'undefined') return
  observer = new IntersectionObserver(
    (entries) => {
      for (const entry of entries) {
        const idx = stepRefs.value.indexOf(entry.target as HTMLElement)
        if (idx === -1) continue
        if (entry.isIntersecting) visibleSteps.value[idx] = true
      }
    },
    { threshold: 0.20, rootMargin: '0px 0px -10% 0px' }
  )
  for (const el of stepRefs.value) if (el) observer.observe(el)
})

onUnmounted(() => observer?.disconnect())
</script>

<style scoped>
/* ── Panel shell — TR+BL radius mirrors Types section (TL+BR) ───────────── */
.partner-process {
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
    0 clamp(80px, 10vw, 150px)
    0 clamp(80px, 10vw, 150px);
  box-shadow:
    0 -22px 40px -12px rgba(0, 0, 0, 0.45),
    0  22px 40px -12px rgba(0, 0, 0, 0.45);
  overflow: hidden;
  isolation: isolate;
  font-family: var(--font-family);
}

.partner-process__surface {
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
.partner-process__header {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  max-width: 880px;
  margin: 0 auto;
}

.partner-process__eyebrow {
  margin-bottom: clamp(20px, 2.5vw, 28px);
}

.partner-process__heading {
  font-size: clamp(28px, 4.2vw, 46px);
  font-weight: 700;
  line-height: 1.15;
  letter-spacing: -0.02em;
  color: #fff;
  margin: 0 0 clamp(14px, 1.8vw, 22px);
  max-width: 880px;
  text-shadow: 0 4px 24px rgba(0, 0, 0, 0.40);
}

.partner-process__heading-accent {
  color: var(--c-brand-cyan);
  display: block;
}

.partner-process__subtitle {
  font-size: clamp(13px, 1.2vw, 16px);
  font-weight: 400;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.78);
  margin: 0;
  max-width: 620px;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.30);
}

/* ── Steps — 4 cols desktop, 2 tablet, 1 mobile ─────────────────────────── */
.partner-process__steps {
  list-style: none;
  padding: 0;
  margin: 0;
  position: relative;
  z-index: 2;
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: clamp(18px, 2.2vw, 26px);
  width: 100%;
  max-width: 1280px;
}

.partner-process__step {
  position: relative;
  padding:
    clamp(24px, 2.8vw, 36px)
    clamp(22px, 2.6vw, 32px)
    clamp(22px, 2.6vw, 32px);
  background: linear-gradient(180deg,
    rgba(255, 255, 255, 0.10) 0%,
    rgba(255, 255, 255, 0.04) 100%);
  backdrop-filter: blur(18px) saturate(160%);
  -webkit-backdrop-filter: blur(18px) saturate(160%);
  border: 1px solid rgba(255, 255, 255, 0.18);
  border-radius:
    clamp(20px, 2.4vw, 28px) clamp(20px, 2.4vw, 28px)
    clamp(20px, 2.4vw, 28px) clamp(40px, 5vw, 60px);
  color: #fff;
  isolation: isolate;
  overflow: hidden;
  min-height: clamp(220px, 24vw, 280px);

  opacity: 0;
  transform: translateY(40px) scale(0.95);
  transition:
    opacity 0.7s ease-out,
    transform 0.7s cubic-bezier(0.22, 1, 0.36, 1);
}

.partner-process__step--visible {
  opacity: 1;
  transform: translateY(0) scale(1);
}

/* Tone glow in the BL corner — accents the step's brand color */
.partner-process__step::before {
  content: '';
  position: absolute;
  pointer-events: none;
  bottom: 0;
  left: 0;
  width: clamp(100px, 12vw, 140px);
  height: clamp(100px, 12vw, 140px);
  z-index: 0;
  border-radius: 0 clamp(40px, 5vw, 60px) 0 0;
}

.partner-process__step--cyan::before {
  background: radial-gradient(circle at bottom left,
    rgba(0, 173, 239, 0.45) 0%,
    rgba(0, 173, 239, 0.14) 40%,
    transparent 70%);
}

.partner-process__step--blue::before {
  background: radial-gradient(circle at bottom left,
    rgba(21, 117, 187, 0.50) 0%,
    rgba(21, 117, 187, 0.16) 40%,
    transparent 70%);
}

.partner-process__step--red::before {
  background: radial-gradient(circle at bottom left,
    rgba(229, 45, 39, 0.45) 0%,
    rgba(229, 45, 39, 0.14) 40%,
    transparent 70%);
}

/* ── Big step number — the visual anchor ─────────────────────────────────── */
.partner-process__number {
  position: relative;
  z-index: 1;
  display: block;
  font-family: var(--font-family);
  font-size: clamp(54px, 6.5vw, 84px);
  font-weight: 800;
  line-height: 1;
  letter-spacing: -0.04em;
  margin-bottom: clamp(14px, 1.8vw, 22px);
  text-shadow: 0 4px 18px rgba(0, 0, 0, 0.40);
}

.partner-process__step--cyan .partner-process__number {
  color: var(--c-brand-cyan);
}

.partner-process__step--blue .partner-process__number {
  /* Slightly lighter than brand blue so it reads on the dark panel */
  color: #5cb1ff;
}

.partner-process__step--red .partner-process__number {
  color: var(--c-brand-red);
}

/* ── Step body ──────────────────────────────────────────────────────────── */
.partner-process__step-body {
  position: relative;
  z-index: 1;
  display: flex;
  flex-direction: column;
  gap: clamp(8px, 1vw, 12px);
}

.partner-process__step-eyebrow {
  font-size: clamp(10px, 0.9vw, 11px);
  font-weight: 700;
  letter-spacing: 0.20em;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.60);
}

.partner-process__step-title {
  font-size: clamp(20px, 2.2vw, 26px);
  font-weight: 700;
  line-height: 1.2;
  letter-spacing: -0.01em;
  color: #fff;
  margin: 0;
}

.partner-process__step-text {
  font-size: clamp(13px, 1.1vw, 14px);
  font-weight: 400;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.82);
  margin: 0;
}

/* ── Responsive ─────────────────────────────────────────────────────────── */
@media (max-width: 1023px) {
  .partner-process__steps { grid-template-columns: repeat(2, 1fr); }
}

@media (max-width: 599px) {
  .partner-process__steps { grid-template-columns: 1fr; }
}
</style>
