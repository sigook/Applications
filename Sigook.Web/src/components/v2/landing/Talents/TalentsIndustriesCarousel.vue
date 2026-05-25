<template>
  <section class="talents-industries">
    <!-- Inner glass surface — materializes the panel against GlobalBackground -->
    <div class="talents-industries__surface" aria-hidden="true"></div>

    <header class="talents-industries__header">
      <EyebrowPill variant="white" class="talents-industries__eyebrow">
        Roles We Recruit
      </EyebrowPill>

      <h2 class="talents-industries__heading">
        Eleven industries.
        <span class="talents-industries__heading-accent">
          Where you'll find your role.
        </span>
      </h2>

      <p class="talents-industries__subtitle">
        Browse the sectors we recruit for. Each one comes with deep market
        knowledge and a recruiter who speaks the language.
      </p>
    </header>

    <!-- Carousel -->
    <div class="talents-industries__carousel">
      <button
        class="talents-industries__nav talents-industries__nav--prev"
        type="button"
        aria-label="Previous slide"
        @click="prev"
      >
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
          <path d="m15 18-6-6 6-6" />
        </svg>
      </button>

      <div class="talents-industries__viewport">
        <div
          class="talents-industries__track"
          :style="trackStyle"
        >
          <div
            v-for="(slide, slideIdx) in SLIDES"
            :key="slideIdx"
            class="talents-industries__slide"
            :aria-hidden="currentIndex !== slideIdx"
          >
            <template v-for="card in slide" :key="card.key">
              <!-- Industry card — minimal: icon + title -->
              <TertiaryCard
                v-if="card.kind === 'industry'"
                :variant="card.tone"
                :title="card.title"
              >
                <template #icon>
                  <IndustryIcon :name="card.iconName" />
                </template>
              </TertiaryCard>

              <!-- Learn-more CTA card — wrapped in router-link -->
              <router-link
                v-else
                to="/v2/industries"
                class="talents-industries__cta-link"
              >
                <TertiaryCard
                  variant="cyan"
                  :title="card.title"
                  class="talents-industries__cta-card"
                >
                  <template #icon>
                    <svg
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
                  </template>

                  {{ card.subtitle }}
                </TertiaryCard>
              </router-link>
            </template>
          </div>
        </div>
      </div>

      <button
        class="talents-industries__nav talents-industries__nav--next"
        type="button"
        aria-label="Next slide"
        @click="next"
      >
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
          <path d="m9 18 6-6-6-6" />
        </svg>
      </button>
    </div>

    <SliderDots
      v-model="currentIndex"
      :count="SLIDES.length"
      aria-label="Industries carousel"
      class="talents-industries__dots"
      @update:model-value="goTo"
    />
  </section>
</template>

<script setup lang="ts">
/**
 * Talents — Industries carousel.
 *
 * 3 cards per slide, 11 sectors + 1 "Learn More" CTA = 12 cards / 4 slides.
 * Cards use TertiaryCard (compact canonical) so the visual rhythm matches
 * Home WhyChooseUs / Numbers feature cards, just at a smaller size suited
 * for dense carousel layouts.
 *
 * State + auto-advance live in useCarousel; SliderDots handles indicator UI.
 * Slide transition is a simple translateX on the track.
 */
import { computed } from 'vue'
import EyebrowPill from '@/components/v2/landing/shared/EyebrowPill.vue'
import SliderDots from '@/components/v2/landing/shared/SliderDots.vue'
import TertiaryCard, { type TertiaryCardVariant } from '@/components/v2/landing/shared/TertiaryCard.vue'
import IndustryIcon, { type IndustryIconName } from '@/components/v2/landing/shared/IndustryIcon.vue'
import { useCarousel } from '@/composables/useCarousel'

interface IndustryCard {
  readonly kind: 'industry'
  readonly key: string
  readonly title: string
  readonly iconName: IndustryIconName
  readonly tone: TertiaryCardVariant
}

interface LearnMoreCard {
  readonly kind: 'learn-more'
  readonly key: string
  readonly title: string
  readonly subtitle: string
}

type CarouselCard = IndustryCard | LearnMoreCard

// 11 industries + 1 Learn More card = 12 cards.
// Industries cycle through the 3 canonical tones (blue → cyan → red).
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
  start()
}

const trackStyle = computed(() => ({
  transform: `translateX(-${currentIndex.value * 100}%)`,
}))
</script>

<style scoped>
/* ── Panel shell ────────────────────────────────────────────────────────── */
.talents-industries {
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

.talents-industries__surface {
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
.talents-industries__header {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  max-width: 880px;
  margin: 0 auto;
}

.talents-industries__eyebrow {
  margin-bottom: clamp(20px, 2.5vw, 28px);
}

.talents-industries__heading {
  font-size: clamp(28px, 4.2vw, 46px);
  font-weight: 700;
  line-height: 1.15;
  letter-spacing: -0.02em;
  color: #fff;
  margin: 0 0 clamp(14px, 1.8vw, 22px);
  max-width: 780px;
  text-shadow: 0 4px 24px rgba(0, 0, 0, 0.40);
}

.talents-industries__heading-accent {
  color: var(--c-brand-cyan);
  display: block;
}

.talents-industries__subtitle {
  font-size: clamp(13px, 1.2vw, 16px);
  font-weight: 400;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.78);
  margin: 0;
  max-width: 560px;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.30);
}

/* ── Carousel row — viewport + prev/next nav ────────────────────────────── */
.talents-industries__carousel {
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
.talents-industries__nav {
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

.talents-industries__nav:hover {
  background: var(--c-brand-cyan);
  border-color: var(--c-brand-cyan);
  color: var(--c-brand-navy);
  transform: scale(1.05);
}

.talents-industries__nav svg {
  width: 50%;
  height: 50%;
}

/* Viewport — clips overflow */
.talents-industries__viewport {
  overflow: hidden;
  border-radius: clamp(20px, 2.5vw, 36px);
}

/* Track — flex row of slides, slides snap full-width */
.talents-industries__track {
  display: flex;
  transition: transform 0.55s cubic-bezier(0.22, 1, 0.36, 1);
  will-change: transform;
}

.talents-industries__slide {
  flex: 0 0 100%;
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: clamp(14px, 1.6vw, 22px);
}

/* ── Learn-more CTA card link wrapper ───────────────────────────────────── */
.talents-industries__cta-link {
  text-decoration: none;
  display: flex;
}

.talents-industries__cta-card {
  flex: 1;
  cursor: pointer;
}

/* ── Dots ───────────────────────────────────────────────────────────────── */
.talents-industries__dots {
  position: relative;
  z-index: 2;
}
</style>
