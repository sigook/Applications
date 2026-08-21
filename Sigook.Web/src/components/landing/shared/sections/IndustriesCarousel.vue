<template>
  <section class="industries-carousel">
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

    <div
      class="industries-carousel__row"
      @mouseenter="onMouseEnter"
      @mouseleave="onMouseLeave"
      @focusin="stop"
      @focusout="start"
    >
      <button
        class="industries-carousel__nav industries-carousel__nav--prev"
        type="button"
        aria-label="Previous slide"
        @click="prev"
      >
        <LandingIcon name="chevron-left" />
      </button>

      <div
        class="industries-carousel__viewport"
        @pointerdown="onPointerDown"
        @pointerup="onPointerUp"
      >
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
              <TertiaryCard
                v-if="card.kind === 'industry'"
                :variant="card.tone"
                :title="card.title"
              >
                <template #icon>
                  <IndustryIcon :name="card.iconName" />
                </template>
              </TertiaryCard>

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
                    <LandingIcon name="arrow-circle" />
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
        <LandingIcon name="chevron-right" />
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
import { computed } from 'vue'
import LandingIcon from '@/components/landing/shared/icons/LandingIcon.vue'
import EyebrowPill from '@/components/landing/shared/ui/EyebrowPill.vue'
import SliderDots from '@/components/landing/shared/ui/SliderDots.vue'
import TertiaryCard, { type TertiaryCardVariant } from '@/components/landing/shared/cards/TertiaryCard.vue'
import IndustryIcon, { type IndustryIconName } from '@/components/landing/shared/icons/IndustryIcon.vue'
import { useCarousel } from '@/composables/useCarousel'
import { useSwipe } from '@/composables/useSwipe'

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

const INDUSTRIES: readonly IndustryCard[] = [
  { kind: 'industry', key: 'automotive', title: 'Automotive', iconName: 'automotive', tone: 'blue' },
  { kind: 'industry', key: 'aviation', title: 'Aviation', iconName: 'aviation', tone: 'cyan' },
  { kind: 'industry', key: 'construction', title: 'Construction', iconName: 'construction', tone: 'red' },
  { kind: 'industry', key: 'engineering', title: 'Engineering', iconName: 'engineering', tone: 'blue' },
  { kind: 'industry', key: 'technology', title: 'Technology & IT', iconName: 'technology', tone: 'cyan' },
  { kind: 'industry', key: 'finance', title: 'Finance', iconName: 'finance', tone: 'red' },
  { kind: 'industry', key: 'legal', title: 'Legal', iconName: 'legal', tone: 'blue' },
  { kind: 'industry', key: 'logistics', title: 'Logistics', iconName: 'logistics', tone: 'cyan' },
  { kind: 'industry', key: 'manufacturing', title: 'Manufacturing', iconName: 'manufacturing', tone: 'red' },
  { kind: 'industry', key: 'retail', title: 'Retail', iconName: 'retail', tone: 'blue' },
  { kind: 'industry', key: 'transportation', title: 'Transportation', iconName: 'transportation', tone: 'cyan' },
]

const CARDS_PER_SLIDE = 3

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

const { currentIndex, next, goTo, start, stop } = useCarousel(() => SLIDES.value, {
  intervalMs: 6500,
  autoStart: true,
})

function prev(): void {
  const total = SLIDES.value.length
  currentIndex.value = (currentIndex.value - 1 + total) % total
  start()
}

const hoverCapable = typeof window !== 'undefined' && window.matchMedia('(hover: hover)').matches

function onMouseEnter(): void {
  if (hoverCapable) stop()
}

function onMouseLeave(): void {
  if (hoverCapable) start()
}

const { onPointerDown, onPointerUp } = useSwipe(
  () => {
    next()
    start()
  },
  () => prev(),
)

const trackStyle = computed(() => ({
  transform: `translateX(-${currentIndex.value * 100}%)`,
}))
</script>

<style scoped>
.industries-carousel {
  position: relative;
  width: 100%;
  margin-top: var(--panel-overlap);
  padding:
    var(--section-pad-y)
    clamp(20px, 3vw, 64px)
    var(--section-pad-y-lg);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: clamp(36px, 5vw, 60px);
  z-index: 5;
  isolation: isolate;
  font-family: var(--font-family);
}

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
  font-size: var(--text-section-heading-size);
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

.industries-carousel__viewport {
  overflow: hidden;
  border-radius: clamp(20px, 2.5vw, 36px);
  touch-action: pan-y;
}

.industries-carousel__track {
  display: flex;
  transition: transform 0.55s var(--ease-brand);
  will-change: transform;
}

.industries-carousel__slide {
  flex: 0 0 100%;
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: clamp(14px, 1.6vw, 22px);
}

.industries-carousel__cta-link {
  text-decoration: none;
  display: flex;
}

.industries-carousel__cta-card {
  flex: 1;
  cursor: pointer;
}

.industries-carousel__dots {
  position: relative;
  z-index: 2;
}

@media (max-width: 1023px) {
  .industries-carousel__slide {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 599px) {
  .industries-carousel__slide {
    grid-template-columns: 1fr;
  }
}
</style>
