<template>
  <article
    ref="cardRef"
    class="secondary-card"
    :class="[
      `secondary-card--${variant}`,
      { 'secondary-card--visible': visible },
    ]"
    :style="{ transitionDelay: `${delay}ms` }"
  >
    <div v-if="$slots.icon" class="secondary-card__icon">
      <slot name="icon" />
    </div>

    <span v-if="eyebrow" class="secondary-card__eyebrow">{{ eyebrow }}</span>

    <h3 v-if="title" class="secondary-card__title">{{ title }}</h3>

    <div v-if="$slots.default" class="secondary-card__body">
      <slot />
    </div>

    <ul v-if="list && list.length" class="secondary-card__list">
      <li v-for="item in list" :key="item">{{ item }}</li>
    </ul>

    <div v-if="$slots.button" class="secondary-card__button-wrap">
      <slot name="button" />
    </div>
  </article>
</template>

<script lang="ts">
/**
 * Triple-gradient variants — mirror the Home WhyChooseUs / Numbers cards.
 * Exported so parents can tightly type their data arrays.
 */
export type SecondaryCardVariant = 'blue' | 'cyan' | 'red'
</script>

<script setup lang="ts">
/**
 * SecondaryCard — medium feature/stat card.
 *
 * Canonical pattern lifted from the Home page's WhyChooseUsSection feature
 * cards and NumbersSection stat cards: triple gradient background with 3
 * brand variants, radial corner glow, asymmetric brand radius. The workhorse
 * card across all internal pages (About, Special Projects, Talents).
 *
 * Slots (all optional):
 *  • #icon         — optional visual lockup at the top
 *  • #default      — body copy
 *  • #button       — CTA pill / button at the bottom
 *
 * Props (all optional):
 *  • eyebrow       — uppercase tracked pretitle
 *  • title         — main heading
 *  • list          — bulleted list of strings
 *  • delay         — stagger entry delay in ms
 *
 * Variants:
 *  • blue — blue dominant + red accent on top-right
 *  • cyan — triple cyan → blue → red gradient
 *  • red  — red dominant + blue accent on top-right
 */
import { ref, onMounted, onUnmounted } from 'vue'

withDefaults(defineProps<{
  variant?: SecondaryCardVariant
  eyebrow?: string
  title?: string
  list?: readonly string[]
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
/* ── Card shell — glass + triple gradient ───────────────────────────────── */
.secondary-card {
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
  color: #fff;
  isolation: isolate;
  font-family: var(--font-family);
  overflow: hidden;

  opacity: 0;
  transform: translateY(40px) scale(0.92);
  transition:
    opacity 0.7s ease-out,
    transform 0.7s cubic-bezier(0.22, 1, 0.36, 1);
}

.secondary-card--visible {
  opacity: 1;
  transform: translateY(0) scale(1);
}

/* Radial corner glow — placed at the larger asymmetric corner */
.secondary-card::before {
  content: '';
  position: absolute;
  pointer-events: none;
  width: clamp(120px, 14vw, 200px);
  height: clamp(120px, 14vw, 200px);
  z-index: 0;
}

/* ── Variant: blue (blue dominant + red accent on top-right) ────────────── */
.secondary-card--blue {
  background: linear-gradient(135deg,
    rgba(21, 117, 187, 0.42) 0%,
    rgba(21, 117, 187, 0.14) 50%,
    rgba(229, 45, 39, 0.32) 100%);
  border-radius:
    clamp(20px, 2.4vw, 32px) clamp(48px, 6vw, 80px)
    clamp(20px, 2.4vw, 32px) clamp(48px, 6vw, 80px);
}

.secondary-card--blue::before {
  top: 0;
  right: 0;
  background: radial-gradient(circle at top right,
    rgba(229, 45, 39, 0.55) 0%,
    rgba(229, 45, 39, 0.18) 35%,
    transparent 70%);
  border-radius: 0 clamp(48px, 6vw, 80px) 0 0;
}

/* ── Variant: cyan (triple cyan → blue → red gradient) ──────────────────── */
.secondary-card--cyan {
  background: linear-gradient(135deg,
    rgba(0, 173, 239, 0.34) 0%,
    rgba(21, 117, 187, 0.22) 50%,
    rgba(229, 45, 39, 0.30) 100%);
  border-radius:
    clamp(48px, 6vw, 80px) clamp(20px, 2.4vw, 32px)
    clamp(48px, 6vw, 80px) clamp(20px, 2.4vw, 32px);
}

.secondary-card--cyan::before {
  top: 0;
  left: 0;
  background: radial-gradient(circle at top left,
    rgba(0, 173, 239, 0.55) 0%,
    rgba(0, 173, 239, 0.18) 35%,
    transparent 70%);
  border-radius: clamp(48px, 6vw, 80px) 0 0 0;
}

/* ── Variant: red (red dominant + blue accent on top-right) ─────────────── */
.secondary-card--red {
  background: linear-gradient(135deg,
    rgba(229, 45, 39, 0.42) 0%,
    rgba(229, 45, 39, 0.14) 50%,
    rgba(21, 117, 187, 0.32) 100%);
  border-radius:
    clamp(20px, 2.4vw, 32px) clamp(48px, 6vw, 80px)
    clamp(20px, 2.4vw, 32px) clamp(48px, 6vw, 80px);
}

.secondary-card--red::before {
  top: 0;
  right: 0;
  background: radial-gradient(circle at top right,
    rgba(21, 117, 187, 0.60) 0%,
    rgba(21, 117, 187, 0.20) 35%,
    transparent 70%);
  border-radius: 0 clamp(48px, 6vw, 80px) 0 0;
}

/* ── Slots & content ────────────────────────────────────────────────────── */
.secondary-card__icon {
  position: relative;
  z-index: 1;
  margin-bottom: clamp(14px, 1.6vw, 20px);
  font-size: clamp(32px, 3.8vw, 48px);
  color: #fff;
  filter: drop-shadow(0 4px 10px rgba(0, 0, 0, 0.25));
  line-height: 0;
}

.secondary-card__eyebrow {
  position: relative;
  z-index: 1;
  display: inline-block;
  font-size: clamp(10px, 0.9vw, 12px);
  font-weight: 700;
  letter-spacing: 0.22em;
  text-transform: uppercase;
  color: var(--c-brand-cyan);
  margin-bottom: clamp(12px, 1.4vw, 18px);
}

/* Red variant — eyebrow uses white-soft so it reads on the red bg */
.secondary-card--red .secondary-card__eyebrow {
  color: #fff;
  opacity: 0.85;
}

.secondary-card__title {
  position: relative;
  z-index: 1;
  font-size: clamp(20px, 2.4vw, 28px);
  font-weight: 600;
  line-height: 1.2;
  letter-spacing: -0.01em;
  color: #fff;
  margin: 0;
}

.secondary-card__body {
  position: relative;
  z-index: 1;
  font-size: clamp(13px, 1.15vw, 15px);
  font-weight: 400;
  line-height: 1.65;
  color: rgba(255, 255, 255, 0.86);
}

.secondary-card__title + .secondary-card__body {
  margin-top: clamp(12px, 1.4vw, 16px);
}

.secondary-card__list {
  position: relative;
  z-index: 1;
  list-style: none;
  padding: 0;
  margin: clamp(14px, 1.6vw, 18px) 0 0;
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.secondary-card__list li {
  position: relative;
  padding-left: 18px;
  font-size: clamp(13px, 1.1vw, 14px);
  line-height: 1.5;
  color: rgba(255, 255, 255, 0.88);
}

.secondary-card__list li::before {
  content: '';
  position: absolute;
  left: 0;
  top: 0.55em;
  width: 7px;
  height: 7px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.75);
}

.secondary-card__button-wrap {
  position: relative;
  z-index: 1;
  margin-top: clamp(20px, 2.4vw, 28px);
}

/* Slot content resets */
.secondary-card__body :deep(> *:first-child) { margin-top: 0; }
.secondary-card__body :deep(> *:last-child)  { margin-bottom: 0; }

/* ── Mobile GPU optimizations ───────────────────────────────────────────── */
@media (max-width: 1023px) {
  /* Drop backdrop blur; cap big shadow; bump bg alpha on variants below */
  .secondary-card {
    backdrop-filter: none;
    -webkit-backdrop-filter: none;
    box-shadow: 0 16px 24px rgba(0, 0, 0, 0.28);
  }

  .secondary-card--blue {
    background: linear-gradient(135deg,
      rgba(21, 117, 187, 0.56) 0%,
      rgba(21, 117, 187, 0.30) 50%,
      rgba(229, 45, 39, 0.46) 100%);
  }

  .secondary-card--cyan {
    background: linear-gradient(135deg,
      rgba(0, 173, 239, 0.50) 0%,
      rgba(21, 117, 187, 0.38) 50%,
      rgba(229, 45, 39, 0.44) 100%);
  }

  .secondary-card--red {
    background: linear-gradient(135deg,
      rgba(229, 45, 39, 0.56) 0%,
      rgba(229, 45, 39, 0.30) 50%,
      rgba(21, 117, 187, 0.46) 100%);
  }
}
</style>
