<template>
  <article
    ref="cardRef"
    class="news-card"
    :class="[
      `news-card--${tone}`,
      { 'news-card--visible': visible },
    ]"
    :style="{ transitionDelay: `${delay}ms` }"
  >
    <!-- Media — top photo with hover zoom -->
    <div class="news-card__media">
      <img
        :src="article.imageUrl"
        :alt="article.imageAlt"
        class="news-card__photo"
        loading="lazy"
      />
      <span class="news-card__chip" :class="`news-card__chip--${tone}`">
        {{ categoryLabel }}
      </span>
    </div>

    <!-- Body — meta + headline + excerpt + footer -->
    <div class="news-card__body">
      <div class="news-card__meta">
        <time class="news-card__date" :datetime="article.publishedAt">
          {{ formattedDate }}
        </time>
        <span class="news-card__dot" aria-hidden="true">·</span>
        <span class="news-card__readtime">
          {{ article.readTimeMinutes }} min read
        </span>
      </div>

      <h3 class="news-card__title">{{ article.title }}</h3>

      <p class="news-card__excerpt">{{ article.excerpt }}</p>

      <div class="news-card__footer">
        <span class="news-card__author">By {{ article.author }}</span>
        <a
          :href="`#${article.slug}`"
          class="news-card__link"
          :aria-label="`Read article: ${article.title}`"
        >
          <span>Read</span>
          <span class="news-card__link-arrow" aria-hidden="true">→</span>
        </a>
      </div>
    </div>
  </article>
</template>

<script setup lang="ts">
/**
 * NewsCard — editorial card for the /v2/news landing.
 *
 * Layout: top media (4:3 photo + floating category chip) → body (date · read
 * time → title → excerpt → author + Read link). On hover the card lifts and
 * the photo zooms subtly.
 *
 * The chip and the radial-glow corner change color by category so the user
 * can scan the grid visually without reading every chip.
 *
 * Slug links are anchor placeholders until an article detail page exists.
 */
import { computed } from 'vue'
import { useRevealOnScroll } from '@/composables/useRevealOnScroll'
import type { NewsArticle, NewsCategoryKey } from '@/data/news'
import { CATEGORY_LABEL, formatPublishedDate } from '@/data/news'

const props = withDefaults(defineProps<{
  article: NewsArticle
  /** Stagger entry delay in ms (for grid reveal animations). */
  delay?: number
}>(), {
  delay: 0,
})

/**
 * Category → brand color mapping. Cyan = analytical / insight, blue =
 * corporate / company, red = compliance / urgency.
 */
const TONE_BY_CATEGORY: Readonly<Record<NewsCategoryKey, 'cyan' | 'blue' | 'red'>> = {
  'industry-insights': 'cyan',
  'company-news':      'blue',
  'press-releases':    'blue',
  'hiring-trends':     'cyan',
  'compliance':        'red',
}

const tone = computed(() => TONE_BY_CATEGORY[props.article.category])
const categoryLabel = computed(() => CATEGORY_LABEL[props.article.category])
const formattedDate = computed(() => formatPublishedDate(props.article.publishedAt))

const { el: cardRef, visible } = useRevealOnScroll()
</script>

<style scoped>
/* ── Card shell ─────────────────────────────────────────────────────────── */
.news-card {
  position: relative;
  display: flex;
  flex-direction: column;
  background: linear-gradient(180deg,
    rgba(255, 255, 255, 0.08) 0%,
    rgba(255, 255, 255, 0.03) 100%);
  backdrop-filter: blur(18px) saturate(160%);
  -webkit-backdrop-filter: blur(18px) saturate(160%);
  border: 1px solid rgba(255, 255, 255, 0.16);
  border-radius:
    clamp(20px, 2.4vw, 28px) clamp(20px, 2.4vw, 28px)
    clamp(20px, 2.4vw, 28px) clamp(40px, 5vw, 64px);
  box-shadow: 0 12px 28px rgba(0, 0, 0, 0.22);
  color: #fff;
  font-family: var(--font-family);
  overflow: hidden;
  isolation: isolate;
  text-decoration: none;

  opacity: 0;
  transform: translateY(40px) scale(0.96);
  transition:
    opacity 0.7s ease-out,
    transform 0.45s cubic-bezier(0.22, 1, 0.36, 1),
    border-color 0.3s ease,
    box-shadow 0.3s ease;
}

.news-card--visible {
  opacity: 1;
  transform: translateY(0) scale(1);
}

/* Hover lift + accent border */
.news-card:hover {
  border-color: rgba(255, 255, 255, 0.30);
  box-shadow: 0 22px 44px rgba(0, 0, 0, 0.32);
  transform: translateY(-6px);
}

/* ── Tone-specific corner glow (matches SecondaryCard rhythm) ───────────── */
.news-card::after {
  content: '';
  position: absolute;
  pointer-events: none;
  bottom: 0;
  left: 0;
  width: clamp(120px, 14vw, 180px);
  height: clamp(120px, 14vw, 180px);
  z-index: 0;
  border-radius: 0 clamp(48px, 6vw, 80px) 0 0;
}

.news-card--cyan::after {
  background: radial-gradient(circle at bottom left,
    rgba(0, 173, 239, 0.40) 0%,
    rgba(0, 173, 239, 0.12) 40%,
    transparent 70%);
}

.news-card--blue::after {
  background: radial-gradient(circle at bottom left,
    rgba(21, 117, 187, 0.45) 0%,
    rgba(21, 117, 187, 0.14) 40%,
    transparent 70%);
}

.news-card--red::after {
  background: radial-gradient(circle at bottom left,
    rgba(229, 45, 39, 0.42) 0%,
    rgba(229, 45, 39, 0.14) 40%,
    transparent 70%);
}

/* ── Media — top photo ──────────────────────────────────────────────────── */
.news-card__media {
  position: relative;
  width: 100%;
  aspect-ratio: 4 / 3;
  overflow: hidden;
  background: rgba(0, 0, 0, 0.30); /* fallback while img loads */
}

.news-card__photo {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.6s cubic-bezier(0.22, 1, 0.36, 1);
  will-change: transform;
}

.news-card:hover .news-card__photo {
  transform: scale(1.06);
}

/* Soft gradient at the bottom of the photo to anchor the chip */
.news-card__media::after {
  content: '';
  position: absolute;
  inset: 0;
  background: linear-gradient(180deg,
    rgba(0, 0, 0, 0) 60%,
    rgba(0, 0, 0, 0.45) 100%);
  pointer-events: none;
}

/* ── Category chip — sits on the photo bottom-left ──────────────────────── */
.news-card__chip {
  position: absolute;
  left: clamp(14px, 1.6vw, 18px);
  bottom: clamp(14px, 1.6vw, 18px);
  z-index: 2;
  display: inline-block;
  padding: 6px 14px;
  border-radius: 999px;
  font-size: clamp(10px, 0.9vw, 11px);
  font-weight: 700;
  letter-spacing: 0.16em;
  text-transform: uppercase;
  line-height: 1.2;
  backdrop-filter: blur(8px);
  -webkit-backdrop-filter: blur(8px);
}

.news-card__chip--cyan {
  color: var(--c-brand-cyan);
  background: rgba(0, 173, 239, 0.18);
  border: 1px solid rgba(0, 173, 239, 0.45);
}

.news-card__chip--blue {
  color: #fff;
  background: rgba(21, 117, 187, 0.45);
  border: 1px solid rgba(21, 117, 187, 0.70);
}

.news-card__chip--red {
  color: #fff;
  background: rgba(229, 45, 39, 0.40);
  border: 1px solid rgba(229, 45, 39, 0.70);
}

/* ── Body ───────────────────────────────────────────────────────────────── */
.news-card__body {
  position: relative;
  z-index: 1;
  display: flex;
  flex-direction: column;
  flex: 1 1 auto;
  padding:
    clamp(20px, 2.4vw, 28px)
    clamp(22px, 2.6vw, 30px)
    clamp(22px, 2.6vw, 30px);
  gap: clamp(10px, 1.2vw, 14px);
}

/* Meta row — date · read time */
.news-card__meta {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: clamp(11px, 0.95vw, 12px);
  font-weight: 600;
  letter-spacing: 0.06em;
  color: rgba(255, 255, 255, 0.65);
  text-transform: uppercase;
}

.news-card__dot {
  opacity: 0.7;
}

/* Title */
.news-card__title {
  font-size: clamp(17px, 1.7vw, 21px);
  font-weight: 700;
  line-height: 1.30;
  letter-spacing: -0.01em;
  color: #fff;
  margin: 0;
}

/* Excerpt */
.news-card__excerpt {
  font-size: clamp(13px, 1.1vw, 14px);
  font-weight: 400;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.78);
  margin: 0;
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

/* Footer — author + read link */
.news-card__footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  margin-top: auto;
  padding-top: clamp(12px, 1.4vw, 16px);
  border-top: 1px solid rgba(255, 255, 255, 0.10);
}

.news-card__author {
  font-size: clamp(11px, 0.95vw, 12px);
  color: rgba(255, 255, 255, 0.55);
  font-weight: 500;
}

.news-card__link {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  font-size: clamp(12px, 1vw, 13px);
  font-weight: 700;
  letter-spacing: 0.04em;
  color: #fff;
  text-decoration: none;
  transition: color 0.25s ease;
}

.news-card__link-arrow {
  font-size: 1.15em;
  line-height: 1;
  transition: transform 0.25s ease;
}

.news-card:hover .news-card__link {
  color: var(--c-brand-cyan);
}

.news-card:hover .news-card__link-arrow {
  transform: translateX(4px);
}

/* Red-tone cards use red as the hover accent instead of cyan */
.news-card--red:hover .news-card__link {
  color: var(--c-brand-red);
}

/* ── Mobile GPU budget — drop blur, bake legibility, cap shadows ──────────── */
@media (max-width: 1023px) {
  .news-card {
    backdrop-filter: none;
    -webkit-backdrop-filter: none;
    background: linear-gradient(180deg,
      rgba(255, 255, 255, 0.22) 0%,
      rgba(255, 255, 255, 0.16) 100%);
    box-shadow: 0 12px 24px rgba(0, 0, 0, 0.22);
  }

  .news-card:hover {
    box-shadow: 0 22px 24px rgba(0, 0, 0, 0.34);
  }

  .news-card__chip {
    backdrop-filter: none;
    -webkit-backdrop-filter: none;
  }

  .news-card__chip--cyan {
    background: rgba(0, 173, 239, 0.34);
  }
}
</style>
