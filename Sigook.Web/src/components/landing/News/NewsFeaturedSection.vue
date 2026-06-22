<template>
  <section id="news-featured" class="news-featured">
    <!-- Inner glass surface — materializes the panel against GlobalBackground -->
    <div class="news-featured__surface" aria-hidden="true"></div>

    <LandingSectionHeader
      eyebrow="Editor's Picks"
      heading="Three reads"
      heading-accent="worth your minute."
      subtitle="The stories the Sigook® team is following this week — handpicked from the newsroom."
    />

    <!-- Carousel -->
    <div class="news-featured__carousel">
      <button
        class="news-featured__nav news-featured__nav--prev"
        type="button"
        aria-label="Previous featured story"
        @click="prev"
      >
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
          <path d="m15 18-6-6 6-6" />
        </svg>
      </button>

      <div class="news-featured__viewport">
        <div
          class="news-featured__track"
          :style="trackStyle"
        >
          <article
            v-for="(article, idx) in FEATURED"
            :key="article.id"
            class="news-featured__slide"
            :aria-hidden="currentIndex !== idx"
          >
            <!-- Media — large photo left -->
            <div class="news-featured__media">
              <img
                :src="article.imageUrl"
                :alt="article.imageAlt"
                class="news-featured__photo"
                loading="lazy"
              />
              <span
                class="news-featured__chip"
                :class="`news-featured__chip--${toneFor(article.category)}`"
              >
                {{ CATEGORY_LABEL[article.category] }}
              </span>
            </div>

            <!-- Body — meta + headline + excerpt + read link -->
            <div class="news-featured__body">
              <div class="news-featured__meta">
                <time :datetime="article.publishedAt">
                  {{ formatPublishedDate(article.publishedAt) }}
                </time>
                <span class="news-featured__dot" aria-hidden="true">·</span>
                <span>{{ article.readTimeMinutes }} min read</span>
                <span class="news-featured__dot" aria-hidden="true">·</span>
                <span>By {{ article.author }}</span>
              </div>

              <h3 class="news-featured__title">{{ article.title }}</h3>

              <p class="news-featured__excerpt">{{ article.excerpt }}</p>

              <ArrowPillCta
                :to="`/news/${article.slug}`"
                class="news-featured__link"
                hover-variant="cyan"
                :aria-label="`Read full article: ${article.title}`"
              >
                Read full article
              </ArrowPillCta>
            </div>
          </article>
        </div>
      </div>

      <button
        class="news-featured__nav news-featured__nav--next"
        type="button"
        aria-label="Next featured story"
        @click="next"
      >
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
          <path d="m9 18 6-6-6-6" />
        </svg>
      </button>
    </div>

    <SliderDots
      v-model="currentIndex"
      :count="FEATURED.length"
      aria-label="Featured stories carousel"
      class="news-featured__dots"
      @update:model-value="goTo"
    />
  </section>
</template>

<script setup lang="ts">
/**
 * NewsFeaturedSection — panel-style carousel of featured stories.
 *
 * Layout per slide: 50/50 split — large hero photo on the left, metadata +
 * headline + excerpt + read link on the right. On mobile it stacks vertically
 * (photo on top).
 *
 * State + auto-advance live in useCarousel; SliderDots handles indicator UI.
 * The slide transition is a translateX on the track (matches IndustriesCarousel).
 */
import { computed } from 'vue'
import LandingSectionHeader from '@/components/landing/shared/LandingSectionHeader.vue'
import ArrowPillCta from '@/components/landing/shared/ArrowPillCta.vue'
import SliderDots from '@/components/landing/shared/SliderDots.vue'
import {
  CATEGORY_LABEL,
  formatPublishedDate,
  getFeaturedArticles,
  type NewsCategoryKey,
} from '@/data/news'
import { useCarousel } from '@/composables/useCarousel'

const FEATURED = getFeaturedArticles()

/** Same tone map as NewsCard — duplicated here to avoid coupling. */
function toneFor(category: NewsCategoryKey): 'cyan' | 'blue' | 'red' {
  if (category === 'compliance') return 'red'
  if (category === 'company-news' || category === 'press-releases') return 'blue'
  return 'cyan'
}

const { currentIndex, next, goTo, start } = useCarousel(() => FEATURED, {
  intervalMs: 8000,
  autoStart: true,
})

function prev(): void {
  const total = FEATURED.length
  currentIndex.value = (currentIndex.value - 1 + total) % total
  start()
}

const trackStyle = computed(() => ({
  transform: `translateX(-${currentIndex.value * 100}%)`,
}))
</script>

<style scoped>
/* ── Panel shell ────────────────────────────────────────────────────────── */
.news-featured {
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
  isolation: isolate;
  font-family: var(--font-family);
}

.news-featured::before {
  content: '';
  position: absolute;
  top: -16px;
  bottom: -16px;
  left: 0;
  right: 0;
  z-index: -1;
  border-radius:
    clamp(80px, 10vw, 150px) 0
    clamp(80px, 10vw, 150px) 0;
  background: rgba(255, 255, 255, 0.07);
  backdrop-filter: blur(10px) saturate(120%);
  -webkit-backdrop-filter: blur(10px) saturate(120%);
  border: 1px solid rgba(255, 255, 255, 0.14);
  box-shadow: 0 18px 40px -20px rgba(0, 0, 0, 0.4);
  pointer-events: none;
}

.news-featured__surface {
  position: absolute;
  inset: 0;
  z-index: 0;
  border-radius:
    clamp(80px, 10vw, 150px) 0
    clamp(80px, 10vw, 150px) 0;
  background: linear-gradient(
    180deg,
    rgba(9, 48, 85, 0.65) 0%,
    rgba(9, 48, 85, 0.55) 100%
  );
  backdrop-filter: blur(20px) saturate(160%);
  -webkit-backdrop-filter: blur(20px) saturate(160%);
  pointer-events: none;
}

/* ── Carousel row — viewport + prev/next nav ────────────────────────────── */
.news-featured__carousel {
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
.news-featured__nav {
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

.news-featured__nav:hover {
  background: var(--c-brand-cyan);
  border-color: var(--c-brand-cyan);
  color: var(--c-brand-navy);
  transform: scale(1.05);
}

.news-featured__nav svg {
  width: 50%;
  height: 50%;
}

/* Viewport — clips overflow */
.news-featured__viewport {
  overflow: hidden;
  border-radius: clamp(20px, 2.5vw, 32px);
  background: rgba(255, 255, 255, 0.04);
  border: 1px solid rgba(255, 255, 255, 0.10);
}

/* Track — flex row of slides, slides snap full-width */
.news-featured__track {
  display: flex;
  transition: transform 0.65s cubic-bezier(0.22, 1, 0.36, 1);
  will-change: transform;
}

/* ── Slide — 50/50 split: photo left, content right ─────────────────────── */
.news-featured__slide {
  flex: 0 0 100%;
  display: grid;
  grid-template-columns: 1.05fr 1fr;
  min-height: clamp(360px, 36vw, 480px);
}

/* Media (left half) */
.news-featured__media {
  position: relative;
  overflow: hidden;
  min-height: 280px;
}

.news-featured__photo {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
  object-fit: cover;
}

/* Soft gradient bottom for chip legibility */
.news-featured__media::after {
  content: '';
  position: absolute;
  inset: 0;
  background: linear-gradient(180deg,
    rgba(0, 0, 0, 0) 60%,
    rgba(0, 0, 0, 0.50) 100%);
  pointer-events: none;
}

.news-featured__chip {
  position: absolute;
  left: clamp(18px, 1.8vw, 22px);
  bottom: clamp(18px, 1.8vw, 22px);
  z-index: 2;
  display: inline-block;
  padding: 7px 16px;
  border-radius: 999px;
  font-size: clamp(10px, 0.9vw, 12px);
  font-weight: 700;
  letter-spacing: 0.18em;
  text-transform: uppercase;
  line-height: 1.2;
  backdrop-filter: blur(8px);
  -webkit-backdrop-filter: blur(8px);
}

.news-featured__chip--cyan {
  color: var(--c-brand-cyan);
  background: rgba(0, 173, 239, 0.18);
  border: 1px solid rgba(0, 173, 239, 0.45);
}

.news-featured__chip--blue {
  color: #fff;
  background: rgba(21, 117, 187, 0.45);
  border: 1px solid rgba(21, 117, 187, 0.70);
}

.news-featured__chip--red {
  color: #fff;
  background: rgba(229, 45, 39, 0.40);
  border: 1px solid rgba(229, 45, 39, 0.70);
}

/* Body (right half) */
.news-featured__body {
  display: flex;
  flex-direction: column;
  justify-content: center;
  gap: clamp(14px, 1.6vw, 20px);
  padding: clamp(28px, 3.4vw, 44px) clamp(26px, 3vw, 44px);
}

.news-featured__meta {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  gap: 8px;
  font-size: clamp(11px, 0.95vw, 12px);
  font-weight: 600;
  letter-spacing: 0.06em;
  color: rgba(255, 255, 255, 0.65);
  text-transform: uppercase;
}

.news-featured__dot {
  opacity: 0.7;
}

.news-featured__title {
  font-size: clamp(22px, 2.6vw, 32px);
  font-weight: 700;
  line-height: 1.18;
  letter-spacing: -0.015em;
  color: #fff;
  margin: 0;
}

.news-featured__excerpt {
  font-size: clamp(14px, 1.2vw, 16px);
  font-weight: 400;
  line-height: 1.65;
  color: rgba(255, 255, 255, 0.82);
  margin: 0;
}

.news-featured__link {
  margin-top: clamp(8px, 1vw, 12px);
  align-self: flex-start;
}

/* ── Dots ───────────────────────────────────────────────────────────────── */
.news-featured__dots {
  position: relative;
  z-index: 2;
}

/* ── Responsive — stack the slide vertically on tablet / mobile ─────────── */
@media (max-width: 899px) {
  .news-featured__slide {
    grid-template-columns: 1fr;
    min-height: 0;
  }

  .news-featured__media {
    aspect-ratio: 16 / 10;
    min-height: 0;
  }

  .news-featured__body {
    padding: clamp(22px, 4vw, 32px);
  }
}

@media (max-width: 599px) {
  /* On phones, hide the side nav buttons — users swipe / use dots */
  .news-featured__nav { display: none; }
  .news-featured__carousel { grid-template-columns: 1fr; }
}
</style>
