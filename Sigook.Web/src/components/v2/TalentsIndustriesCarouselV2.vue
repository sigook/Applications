<template>
  <section class="talents-industries-v2">
    <!-- Inner glass surface — materializes the panel against GlobalBackground -->
    <div class="talents-industries-v2__surface" aria-hidden="true"></div>

    <header class="talents-industries-v2__header">
      <EyebrowPillV2 variant="white" class="talents-industries-v2__eyebrow">
        Roles We Recruit
      </EyebrowPillV2>

      <h2 class="talents-industries-v2__heading">
        Eleven industries.
        <span class="talents-industries-v2__heading-accent">
          Where you'll find your role.
        </span>
      </h2>

      <p class="talents-industries-v2__subtitle">
        Browse the sectors we recruit for. Each one comes with deep market
        knowledge and a recruiter who speaks the language.
      </p>
    </header>

    <!-- Carousel -->
    <div class="talents-industries-v2__carousel">
      <button
        class="talents-industries-v2__nav talents-industries-v2__nav--prev"
        type="button"
        aria-label="Previous slide"
        @click="prev"
      >
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
          <path d="m15 18-6-6 6-6" />
        </svg>
      </button>

      <div class="talents-industries-v2__viewport">
        <div
          class="talents-industries-v2__track"
          :style="trackStyle"
        >
          <div
            v-for="(slide, slideIdx) in SLIDES"
            :key="slideIdx"
            class="talents-industries-v2__slide"
            :aria-hidden="currentIndex !== slideIdx"
          >
            <component
              :is="card.kind === 'learn-more' ? 'router-link' : 'div'"
              v-for="card in slide"
              :key="card.key"
              :to="card.kind === 'learn-more' ? '/v2/industries' : undefined"
              class="talents-industries-v2__card"
              :class="
                card.kind === 'industry'
                  ? `talents-industries-v2__card--${card.tone}`
                  : 'talents-industries-v2__card--cta'
              "
            >
              <template v-if="card.kind === 'industry'">
                <IndustryIconV2
                  :name="card.iconName"
                  class="talents-industries-v2__card-icon"
                />
                <span class="talents-industries-v2__card-title">{{ card.title }}</span>
              </template>

              <template v-else>
                <svg
                  class="talents-industries-v2__card-icon"
                  viewBox="0 0 24 24"
                  fill="none"
                  stroke="currentColor"
                  stroke-width="1.5"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  aria-hidden="true"
                >
                  <circle cx="12" cy="12" r="9" />
                  <path d="M9 12h6M13 9l3 3-3 3" />
                </svg>

                <span class="talents-industries-v2__card-title">{{ card.title }}</span>
                <span class="talents-industries-v2__card-meta">{{ card.subtitle }}</span>

                <span class="talents-industries-v2__card-cta">
                  Learn more <span aria-hidden="true">→</span>
                </span>
              </template>
            </component>
          </div>
        </div>
      </div>

      <button
        class="talents-industries-v2__nav talents-industries-v2__nav--next"
        type="button"
        aria-label="Next slide"
        @click="next"
      >
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
          <path d="m9 18 6-6-6-6" />
        </svg>
      </button>
    </div>

    <SliderDotsV2
      v-model="currentIndex"
      :count="SLIDES.length"
      aria-label="Industries carousel"
      class="talents-industries-v2__dots"
      @update:model-value="goTo"
    />
  </section>
</template>

<script setup lang="ts">
/**
 * Talents — Industries carousel.
 *
 * 3 cards per slide, 11 sectors + 1 "Learn More" CTA = 12 cards / 4 slides.
 * Cards are minimal: just an icon + the industry name (no body, no positions
 * list — that detail lives on /v2/industries). The last card on slide 4 is
 * a CTA that routes to the full Industries page.
 *
 * State + auto-advance live in useCarousel; SliderDotsV2 handles indicator
 * UI. Slide transition is a simple translateX on the track.
 */
import { computed } from 'vue'
import EyebrowPillV2 from '@/components/v2/shared/EyebrowPillV2.vue'
import SliderDotsV2 from '@/components/v2/shared/SliderDotsV2.vue'
import IndustryIconV2, { type IndustryIconName } from '@/components/v2/shared/IndustryIconV2.vue'
import { useCarousel } from '@/composables/useCarousel'

type Tone = 'blue' | 'cyan' | 'red'

interface IndustryCard {
  readonly kind: 'industry'
  readonly key: string
  readonly title: string
  readonly iconName: IndustryIconName
  readonly tone: Tone
}

interface LearnMoreCard {
  readonly kind: 'learn-more'
  readonly key: string
  readonly title: string
  readonly subtitle: string
}

type CarouselCard = IndustryCard | LearnMoreCard

// 11 industries + 1 Learn More card = 12 cards.
// Industries cycle through the 3 canonical Home tones (blue → cyan → red)
// so the brand palette stays balanced across slides.
const CARDS: readonly CarouselCard[] = [
  { kind: 'industry', key: 'automotive',     title: 'Automotive',      iconName: 'automotive',     tone: 'blue' },
  { kind: 'industry', key: 'aviation',       title: 'Aviation',        iconName: 'aviation',       tone: 'cyan' },
  { kind: 'industry', key: 'construction',   title: 'Construction',    iconName: 'construction',   tone: 'red'  },
  { kind: 'industry', key: 'engineering',    title: 'Engineering',     iconName: 'engineering',    tone: 'blue' },
  { kind: 'industry', key: 'technology',     title: 'Technology & IT', iconName: 'technology',     tone: 'cyan' },
  { kind: 'industry', key: 'finance',        title: 'Finance',         iconName: 'finance',        tone: 'red'  },
  { kind: 'industry', key: 'legal',          title: 'Legal',           iconName: 'legal',          tone: 'blue' },
  { kind: 'industry', key: 'logistics',      title: 'Logistics',       iconName: 'logistics',      tone: 'cyan' },
  { kind: 'industry', key: 'manufacturing',  title: 'Manufacturing',   iconName: 'manufacturing',  tone: 'red'  },
  { kind: 'industry', key: 'retail',         title: 'Retail',          iconName: 'retail',         tone: 'blue' },
  { kind: 'industry', key: 'transportation', title: 'Transportation',  iconName: 'transportation', tone: 'cyan' },
  {
    kind: 'learn-more',
    key: 'learn-more',
    title: 'Learn more',
    subtitle: 'Explore all sectors',
  },
]

const CARDS_PER_SLIDE = 3

// Chunk the 12 cards into 4 slides of 3
const SLIDES = computed<readonly CarouselCard[][]>(() => {
  const out: CarouselCard[][] = []
  for (let i = 0; i < CARDS.length; i += CARDS_PER_SLIDE) {
    out.push([...CARDS.slice(i, i + CARDS_PER_SLIDE)])
  }
  return out
})

const { currentIndex, next, goTo, start } = useCarousel(() => SLIDES.value, {
  intervalMs: 6500,
  autoStart: true,
})

function prev(): void {
  const total = SLIDES.value.length
  currentIndex.value = (currentIndex.value - 1 + total) % total
  start() // reset auto-advance timer on manual nav
}

const trackStyle = computed(() => ({
  transform: `translateX(-${currentIndex.value * 100}%)`,
}))
</script>

<style scoped>
/* ── Panel shell ────────────────────────────────────────────────────────── */
.talents-industries-v2 {
  position: relative;
  width: 100%;
  margin-top: clamp(-180px, -10vw, -80px);
  padding:
    clamp(140px, 14vw, 200px)
    clamp(20px, 3vw, 64px);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: clamp(36px, 5vw, 60px);
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

.talents-industries-v2__surface {
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
.talents-industries-v2__header {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  max-width: 880px;
  margin: 0 auto;
}

.talents-industries-v2__eyebrow {
  margin-bottom: clamp(20px, 2.5vw, 28px);
}

.talents-industries-v2__heading {
  font-size: clamp(28px, 4.2vw, 46px);
  font-weight: 700;
  line-height: 1.15;
  letter-spacing: -0.02em;
  color: #fff;
  margin: 0 0 clamp(14px, 1.8vw, 22px);
  max-width: 780px;
  text-shadow: 0 4px 24px rgba(0, 0, 0, 0.40);
}

.talents-industries-v2__heading-accent {
  color: var(--c-brand-cyan);
  display: block;
}

.talents-industries-v2__subtitle {
  font-size: clamp(13px, 1.2vw, 16px);
  font-weight: 400;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.78);
  margin: 0;
  max-width: 560px;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.30);
}

/* ── Carousel row — viewport + prev/next nav ────────────────────────────── */
.talents-industries-v2__carousel {
  position: relative;
  z-index: 2;
  display: grid;
  grid-template-columns: auto 1fr auto;
  align-items: center;
  gap: clamp(12px, 1.6vw, 24px);
  width: 100%;
  max-width: 1180px;
  margin: 0 auto;
}

/* Nav buttons — glass round, hover invert */
.talents-industries-v2__nav {
  flex-shrink: 0;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: clamp(40px, 4vw, 52px);
  height: clamp(40px, 4vw, 52px);
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.08);
  border: 1px solid rgba(255, 255, 255, 0.30);
  color: #fff;
  cursor: pointer;
  transition:
    background 0.25s ease,
    border-color 0.25s ease,
    color 0.25s ease,
    transform 0.25s ease;
}

.talents-industries-v2__nav:hover {
  background: var(--c-brand-cyan);
  border-color: var(--c-brand-cyan);
  color: var(--c-brand-navy);
  transform: scale(1.05);
}

.talents-industries-v2__nav svg {
  width: 50%;
  height: 50%;
}

/* Viewport — clips overflow */
.talents-industries-v2__viewport {
  overflow: hidden;
  border-radius: clamp(20px, 2.5vw, 36px);
}

/* Track — flex row of slides, slides snap full-width */
.talents-industries-v2__track {
  display: flex;
  transition: transform 0.55s cubic-bezier(0.22, 1, 0.36, 1);
  will-change: transform;
}

.talents-industries-v2__slide {
  flex: 0 0 100%;
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: clamp(14px, 1.6vw, 22px);
}

/* ── Card — canonical Home pattern: triple gradient + radial glow ───────── */
.talents-industries-v2__card {
  position: relative;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: clamp(14px, 1.6vw, 22px);
  padding:
    clamp(32px, 3.8vw, 52px)
    clamp(16px, 2vw, 28px);
  min-height: clamp(180px, 18vw, 240px);
  backdrop-filter: blur(20px) saturate(160%);
  -webkit-backdrop-filter: blur(20px) saturate(160%);
  border: 1px solid rgba(255, 255, 255, 0.20);
  box-shadow: 0 16px 36px rgba(0, 0, 0, 0.22);
  overflow: hidden;
  isolation: isolate;
  text-align: center;
  text-decoration: none;
  transition:
    border-color 0.35s ease,
    box-shadow 0.35s ease,
    transform 0.35s cubic-bezier(0.22, 1, 0.36, 1);
}

.talents-industries-v2__card:hover {
  border-color: rgba(255, 255, 255, 0.32);
  box-shadow: 0 22px 48px rgba(0, 0, 0, 0.32);
  transform: translateY(-4px);
}

/* Radial corner glow — placed at the larger asymmetric corner */
.talents-industries-v2__card::before {
  content: '';
  position: absolute;
  pointer-events: none;
  width: clamp(100px, 12vw, 160px);
  height: clamp(100px, 12vw, 160px);
  z-index: 0;
}

/* ── Variant: blue (blue dominant + red accent on top-right) ────────────── */
.talents-industries-v2__card--blue {
  background: linear-gradient(135deg,
    rgba(21, 117, 187, 0.42) 0%,
    rgba(21, 117, 187, 0.14) 50%,
    rgba(229, 45, 39, 0.32) 100%);
  border-radius:
    clamp(20px, 2.4vw, 32px) clamp(40px, 5vw, 64px)
    clamp(20px, 2.4vw, 32px) clamp(40px, 5vw, 64px);
}

.talents-industries-v2__card--blue::before {
  top: 0;
  right: 0;
  background: radial-gradient(circle at top right,
    rgba(229, 45, 39, 0.55) 0%,
    rgba(229, 45, 39, 0.18) 35%,
    transparent 70%);
  border-radius: 0 clamp(40px, 5vw, 64px) 0 0;
}

/* ── Variant: cyan (triple cyan → blue → red gradient) ──────────────────── */
.talents-industries-v2__card--cyan {
  background: linear-gradient(135deg,
    rgba(0, 173, 239, 0.34) 0%,
    rgba(21, 117, 187, 0.22) 50%,
    rgba(229, 45, 39, 0.30) 100%);
  border-radius:
    clamp(40px, 5vw, 64px) clamp(20px, 2.4vw, 32px)
    clamp(40px, 5vw, 64px) clamp(20px, 2.4vw, 32px);
}

.talents-industries-v2__card--cyan::before {
  top: 0;
  left: 0;
  background: radial-gradient(circle at top left,
    rgba(0, 173, 239, 0.55) 0%,
    rgba(0, 173, 239, 0.18) 35%,
    transparent 70%);
  border-radius: clamp(40px, 5vw, 64px) 0 0 0;
}

/* ── Variant: red (red dominant + blue accent on top-right) ─────────────── */
.talents-industries-v2__card--red {
  background: linear-gradient(135deg,
    rgba(229, 45, 39, 0.42) 0%,
    rgba(229, 45, 39, 0.14) 50%,
    rgba(21, 117, 187, 0.32) 100%);
  border-radius:
    clamp(20px, 2.4vw, 32px) clamp(40px, 5vw, 64px)
    clamp(20px, 2.4vw, 32px) clamp(40px, 5vw, 64px);
}

.talents-industries-v2__card--red::before {
  top: 0;
  right: 0;
  background: radial-gradient(circle at top right,
    rgba(21, 117, 187, 0.60) 0%,
    rgba(21, 117, 187, 0.20) 35%,
    transparent 70%);
  border-radius: 0 clamp(40px, 5vw, 64px) 0 0;
}

/* Icon — large, neutral white on the rich gradient bg */
.talents-industries-v2__card-icon {
  position: relative;
  z-index: 1;
  font-size: clamp(36px, 4vw, 52px);
  color: #fff;
  filter: drop-shadow(0 4px 10px rgba(0, 0, 0, 0.25));
}

/* Title */
.talents-industries-v2__card-title {
  position: relative;
  z-index: 1;
  font-size: clamp(15px, 1.4vw, 18px);
  font-weight: 700;
  line-height: 1.2;
  letter-spacing: -0.01em;
  color: #fff;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.30);
}

/* ── Learn-more CTA card — same vocabulary, distinct gradient ───────────── */
.talents-industries-v2__card--cta {
  cursor: pointer;
  background: linear-gradient(135deg,
    rgba(0, 173, 239, 0.30) 0%,
    rgba(21, 117, 187, 0.22) 50%,
    rgba(229, 45, 39, 0.40) 100%);
  border-radius:
    clamp(40px, 5vw, 64px) clamp(20px, 2.4vw, 32px)
    clamp(40px, 5vw, 64px) clamp(20px, 2.4vw, 32px);
}

.talents-industries-v2__card--cta::before {
  top: 0;
  left: 0;
  background: radial-gradient(circle at top left,
    rgba(0, 173, 239, 0.55) 0%,
    rgba(0, 173, 239, 0.18) 35%,
    transparent 70%);
  border-radius: clamp(40px, 5vw, 64px) 0 0 0;
}

.talents-industries-v2__card--cta:hover {
  border-color: rgba(255, 255, 255, 0.45);
}

.talents-industries-v2__card--cta .talents-industries-v2__card-icon {
  color: #fff;
}

.talents-industries-v2__card-meta {
  position: relative;
  z-index: 1;
  font-size: clamp(11px, 0.9vw, 12px);
  font-weight: 500;
  letter-spacing: 0.04em;
  color: rgba(255, 255, 255, 0.78);
}

.talents-industries-v2__card-cta {
  position: relative;
  z-index: 1;
  display: inline-flex;
  align-items: center;
  gap: 6px;
  margin-top: clamp(4px, 0.5vw, 8px);
  padding: clamp(8px, 1vw, 10px) clamp(18px, 2vw, 22px);
  border-radius: 999px;
  background: rgba(255, 255, 255, 0.15);
  border: 1px solid rgba(255, 255, 255, 0.50);
  font-size: clamp(12px, 1vw, 13px);
  font-weight: 600;
  letter-spacing: 0.04em;
  color: #fff;
  transition: background 0.25s ease, transform 0.25s ease;
}

.talents-industries-v2__card--cta:hover .talents-industries-v2__card-cta {
  background: #fff;
  color: var(--c-brand-navy);
  transform: translateX(2px);
}

/* ── Dots ───────────────────────────────────────────────────────────────── */
.talents-industries-v2__dots {
  position: relative;
  z-index: 2;
}

/* ── Mobile-only behaviors — shrink cards but keep 3 per slide ──────────── */
@media (max-width: 599px) {
  .talents-industries-v2__slide { gap: 8px; }
  .talents-industries-v2__card {
    padding: 24px 8px;
    min-height: 140px;
  }
  .talents-industries-v2__card-title { font-size: 12px; }
  .talents-industries-v2__card-meta { display: none; }
  .talents-industries-v2__card-cta {
    padding: 5px 10px;
    font-size: 10px;
  }
}
</style>
