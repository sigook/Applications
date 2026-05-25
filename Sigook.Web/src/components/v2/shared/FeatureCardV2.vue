<template>
  <article
    ref="cardRef"
    class="feature-card"
    :class="[
      `feature-card--${variant}`,
      { 'feature-card--visible': visible },
    ]"
    :style="{ transitionDelay: `${delay}ms` }"
  >
    <span class="feature-card__eyebrow">{{ eyebrow }}</span>
    <h3 class="feature-card__title">{{ title }}</h3>
    <div class="feature-card__body">
      <slot />
    </div>
  </article>
</template>

<script lang="ts">
/**
 * Canonical feature card — mirrors the visual vocabulary of the Home page's
 * WhyChooseUsSectionV2 feature cards. Use this for any "eyebrow + title +
 * body" card across About, Special Projects, Talents, etc., so all internal
 * pages speak the same visual language as the Home.
 */
export type FeatureCardVariant = 'blue' | 'cyan' | 'red'
</script>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'

withDefaults(defineProps<{
  variant?: FeatureCardVariant
  eyebrow: string
  title: string
  /** Stagger delay in ms — let the parent space out card entrances. */
  delay?: number
}>(), {
  variant: 'cyan',
  delay: 0,
})

const cardRef = ref<HTMLElement | null>(null)
const visible = ref(false)
let observer: IntersectionObserver | null = null

onMounted(() => {
  if (!cardRef.value) return
  observer = new IntersectionObserver(
    (entries) => { visible.value = entries[0].isIntersecting },
    { threshold: 0.15, rootMargin: '0px 0px -10% 0px' }
  )
  observer.observe(cardRef.value)
})

onUnmounted(() => observer?.disconnect())
</script>

<style scoped>
/* ── Card shell — glass + IntersectionObserver fade ─────────────────────── */
.feature-card {
  position: relative;
  display: flex;
  flex-direction: column;
  padding:
    clamp(32px, 4vw, 48px)
    clamp(24px, 3vw, 36px);
  backdrop-filter: blur(20px) saturate(160%);
  -webkit-backdrop-filter: blur(20px) saturate(160%);
  border: 1px solid rgba(255, 255, 255, 0.20);
  box-shadow: 0 16px 36px rgba(0, 0, 0, 0.22);
  isolation: isolate;
  overflow: hidden;

  /* Entrance fade — driven by IntersectionObserver */
  opacity: 0;
  transform: translateY(40px) scale(0.92);
  transition:
    opacity 0.7s ease-out,
    transform 0.7s cubic-bezier(0.22, 1, 0.36, 1);
}

.feature-card--visible {
  opacity: 1;
  transform: translateY(0) scale(1);
}

/* Radial corner glow — placed at the corner with the larger asymmetric radius */
.feature-card::before {
  content: '';
  position: absolute;
  pointer-events: none;
  width: clamp(120px, 14vw, 200px);
  height: clamp(120px, 14vw, 200px);
  z-index: 0;
}

/* ── Variant: blue (blue dominant + red accent on top-right) ────────────── */
.feature-card--blue {
  background: linear-gradient(135deg,
    rgba(21, 117, 187, 0.42) 0%,
    rgba(21, 117, 187, 0.14) 50%,
    rgba(229, 45, 39, 0.32) 100%);
  border-radius:
    clamp(20px, 2.4vw, 32px) clamp(48px, 6vw, 80px)
    clamp(20px, 2.4vw, 32px) clamp(48px, 6vw, 80px);
}

.feature-card--blue::before {
  top: 0;
  right: 0;
  background: radial-gradient(circle at top right,
    rgba(229, 45, 39, 0.55) 0%,
    rgba(229, 45, 39, 0.18) 35%,
    transparent 70%);
  border-radius: 0 clamp(48px, 6vw, 80px) 0 0;
}

/* ── Variant: cyan (triple cyan → blue → red gradient) ──────────────────── */
.feature-card--cyan {
  background: linear-gradient(135deg,
    rgba(0, 173, 239, 0.34) 0%,
    rgba(21, 117, 187, 0.22) 50%,
    rgba(229, 45, 39, 0.30) 100%);
  border-radius:
    clamp(48px, 6vw, 80px) clamp(20px, 2.4vw, 32px)
    clamp(48px, 6vw, 80px) clamp(20px, 2.4vw, 32px);
}

.feature-card--cyan::before {
  top: 0;
  left: 0;
  background: radial-gradient(circle at top left,
    rgba(0, 173, 239, 0.55) 0%,
    rgba(0, 173, 239, 0.18) 35%,
    transparent 70%);
  border-radius: clamp(48px, 6vw, 80px) 0 0 0;
}

/* ── Variant: red (red dominant + blue accent on top-right) ─────────────── */
.feature-card--red {
  background: linear-gradient(135deg,
    rgba(229, 45, 39, 0.42) 0%,
    rgba(229, 45, 39, 0.14) 50%,
    rgba(21, 117, 187, 0.32) 100%);
  border-radius:
    clamp(20px, 2.4vw, 32px) clamp(48px, 6vw, 80px)
    clamp(20px, 2.4vw, 32px) clamp(48px, 6vw, 80px);
}

.feature-card--red::before {
  top: 0;
  right: 0;
  background: radial-gradient(circle at top right,
    rgba(21, 117, 187, 0.60) 0%,
    rgba(21, 117, 187, 0.20) 35%,
    transparent 70%);
  border-radius: 0 clamp(48px, 6vw, 80px) 0 0;
}

/* ── Typography ─────────────────────────────────────────────────────────── */
.feature-card__eyebrow {
  position: relative;
  z-index: 1;
  display: inline-block;
  font-family: var(--font-family);
  font-size: clamp(10px, 0.9vw, 12px);
  font-weight: 700;
  letter-spacing: 0.22em;
  text-transform: uppercase;
  color: var(--c-brand-cyan);
  margin-bottom: clamp(12px, 1.4vw, 18px);
}

/* Red variant — eyebrow uses white-soft instead of cyan so it reads on the
   red-dominant gradient (matches Home WhyChooseUs --red treatment). */
.feature-card--red .feature-card__eyebrow {
  color: #fff;
  opacity: 0.85;
}

.feature-card__title {
  position: relative;
  z-index: 1;
  font-family: var(--font-family);
  font-size: clamp(20px, 2.2vw, 26px);
  font-weight: 600;
  line-height: 1.2;
  letter-spacing: -0.01em;
  color: #fff;
  margin: 0 0 clamp(12px, 1.4vw, 18px);
}

.feature-card__body {
  position: relative;
  z-index: 1;
  font-family: var(--font-family);
  font-size: clamp(13px, 1.15vw, 15px);
  font-weight: 400;
  line-height: 1.65;
  color: rgba(255, 255, 255, 0.86);
}

/* Reset default top/bottom margins inside the slot so consumer content sits
   flush against title/card bottom (lets each parent control its own spacing). */
.feature-card__body :deep(> *:first-child) { margin-top: 0; }
.feature-card__body :deep(> *:last-child)  { margin-bottom: 0; }
</style>
