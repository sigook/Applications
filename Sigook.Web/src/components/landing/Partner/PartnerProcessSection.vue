<template>
  <section class="partner-process">
    <div class="partner-process__surface" aria-hidden="true"></div>

    <LandingSectionHeader
      eyebrow="How It Works"
      heading="From apply to first commission —"
      heading-accent="typically four weeks."
      subtitle="We've stripped the partner onboarding down to four steps. No 12-week certification tracks, no scattered orientation calls — a clear path from application to your first paid placement."
      heading-max-width="880px"
      subtitle-max-width="620px"
    />

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
import { ref, onMounted, onUnmounted } from 'vue'
import LandingSectionHeader from '@/components/landing/shared/LandingSectionHeader.vue'
import partnerProcessData from '@/data/landing/partnerProcess.json'

interface Step {
  readonly eyebrow: string
  readonly title: string
  readonly body: string
  readonly tone: 'cyan' | 'blue' | 'red'
}

const STEPS = partnerProcessData as readonly Step[]

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
.partner-process {
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
    0 var(--r-brand-fluid)
    0 var(--r-brand-fluid);
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
    transform 0.7s var(--ease-brand);
}

.partner-process__step--visible {
  opacity: 1;
  transform: translateY(0) scale(1);
}

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
  color: #5cb1ff;
}

.partner-process__step--red .partner-process__number {
  color: var(--c-brand-red);
}

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

@media (max-width: 1023px) {
  .partner-process__steps { grid-template-columns: repeat(2, 1fr); }
}

@media (max-width: 599px) {
  .partner-process__steps { grid-template-columns: 1fr; }
}
</style>
