<template>
  <section class="industries-carousel">
    <!-- Inner glass surface — materializes the panel against GlobalBackground -->
    <div class="industries-carousel__surface" aria-hidden="true"></div>

    <header class="industries-carousel__header">
      <EyebrowPill variant="white" class="industries-carousel__eyebrow">
        {{ eyebrow }}
      </EyebrowPill>

      <h2 class="industries-carousel__heading">
        {{ heading }}
        <span class="industries-carousel__heading-accent">
          {{ headingAccent }}
        </span>
      </h2>

      <p class="industries-carousel__subtitle">{{ subtitle }}</p>
    </header>

    <!-- Carousel -->
    <div class="industries-carousel__row">
      <button
        class="industries-carousel__nav industries-carousel__nav--prev"
        type="button"
        aria-label="Previous slide"
        @click="prev"
      >
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
          <path d="m15 18-6-6 6-6" />
        </svg>
      </button>

      <div class="industries-carousel__viewport">
        <div
          class="industries-carousel__track"
          :style="trackStyle"
        >
          <div
            v-for="(slide, slideIdx) in SLIDES"
            :key="slideIdx"
            class="industries-carousel__slide"
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
                to="/industries"
                class="industries-carousel__cta-link"
              >
                <TertiaryCard
                  variant="cyan"
                  :title="card.title"
                  class="industries-carousel__cta-card"
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
        class="industries-carousel__nav industries-carousel__nav--next"
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
      class="industries-carousel__dots"
      @update:model-value="goTo"
    />
  </section>
</template>

<script setup lang="ts">
/**
 * IndustriesCarousel — canonical industries carousel.
 *
 * Extracted from TalentsIndustriesCarousel so it can be reused across pages
 * (Talents, Employers, ...) with page-specific copy.
 *
 * 3 cards per slide, 11 sectors + 1 "Learn More" CTA = 12 cards / 4 slides.
 * Cards use TertiaryCard (compact canonical) so the visual rhythm matches
 * Home WhyChooseUs / Numbers feature cards.
 *
 * Props parameterize the header copy and the learn-more CTA card text.
 * Industries list + tones + icons are fixed by design (same 11 sectors).
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

const props = withDefaults(defineProps<{
  eyebrow?: string
  heading: string
  headingAccent: string
  subtitle: string
  learnMoreTitle?: string
  learnMoreSubtitle?: string
}>(), {
  eyebrow: 'Industries',
  learnMoreTitle: 'Learn more',
  learnMoreSubtitle: 'Explore all sectors',
})

// 11 industries cycle through the 3 canonical tones (blue → cyan → red).
const INDUSTRIES: readonly IndustryCard[] = [
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
] as const

const CARDS_PER_SLIDE = 3

// Append the learn-more card from props, then chunk all 12 cards into 4 slides.
// Computed so the learn-more copy stays reactive if the parent rebinds props.
const SLIDES = computed<readonly CarouselCard[][]>(() => {
  const learnMore: LearnMoreCard = {
    kind: 'learn-more',
    key: 'learn-more',
    title: props.learnMoreTitle,
    subtitle: props.learnMoreSubtitle,
  }

  const all: readonly CarouselCard[] = [...INDUSTRIES, learnMore]

  const out: CarouselCard[][] = []
  for (let i = 0; i < all.length; i += CARDS_PER_SLIDE) {
    out.push([...all.slice(i, i + CARDS_PER_SLIDE)])
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
.industries-carousel {
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

.industries-carousel__surface {
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
.industries-carousel__header {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  max-width: 880px;
  margin: 0 auto;
}

.industries-carousel__eyebrow {
  margin-bottom: clamp(20px, 2.5vw, 28px);
}

.industries-carousel__heading {
  font-size: clamp(28px, 4.2vw, 46px);
  font-weight: 700;
  line-height: 1.15;
  letter-spacing: -0.02em;
  color: #fff;
  margin: 0 0 clamp(14px, 1.8vw, 22px);
  max-width: 780px;
  text-shadow: 0 4px 24px rgba(0, 0, 0, 0.40);
}

.industries-carousel__heading-accent {
  color: var(--c-brand-cyan);
  display: block;
}

.industries-carousel__subtitle {
  font-size: clamp(13px, 1.2vw, 16px);
  font-weight: 400;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.78);
  margin: 0;
  max-width: 560px;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.30);
}

/* ── Carousel row — viewport + prev/next nav ────────────────────────────── */
.industries-carousel__row {
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
.industries-carousel__nav {
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

.industries-carousel__nav:hover {
  background: var(--c-brand-cyan);
  border-color: var(--c-brand-cyan);
  color: var(--c-brand-navy);
  transform: scale(1.05);
}

.industries-carousel__nav svg {
  width: 50%;
  height: 50%;
}

/* Viewport — clips overflow */
.industries-carousel__viewport {
  overflow: hidden;
  border-radius: clamp(20px, 2.5vw, 36px);
}

/* Track — flex row of slides, slides snap full-width */
.industries-carousel__track {
  display: flex;
  transition: transform 0.55s cubic-bezier(0.22, 1, 0.36, 1);
  will-change: transform;
}

.industries-carousel__slide {
  flex: 0 0 100%;
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: clamp(14px, 1.6vw, 22px);
}

/* ── Learn-more CTA card link wrapper ───────────────────────────────────── */
.industries-carousel__cta-link {
  text-decoration: none;
  display: flex;
}

.industries-carousel__cta-card {
  flex: 1;
  cursor: pointer;
}

/* ── Dots ───────────────────────────────────────────────────────────────── */
.industries-carousel__dots {
  position: relative;
  z-index: 2;
}
</style>
