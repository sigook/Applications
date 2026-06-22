<template>
  <section class="article-hero">
    <HeroBackground :image="heroImage" focal="center 35%" />

    <div class="article-hero__content">
      <router-link to="/news" class="article-hero__back">
        <span class="article-hero__back-arrow" aria-hidden="true">←</span>
        Back to Newsroom
      </router-link>

      <span class="article-hero__chip" :class="`article-hero__chip--${tone}`">
        {{ categoryLabel }}
      </span>

      <h1 class="article-hero__title">{{ article.title }}</h1>

      <p v-if="lede" class="article-hero__lede">{{ lede }}</p>

      <div class="article-hero__meta">
        <time :datetime="article.publishedAt">{{ formattedDate }}</time>
        <span class="article-hero__dot" aria-hidden="true">·</span>
        <span>{{ article.readTimeMinutes }} min read</span>
        <span class="article-hero__dot" aria-hidden="true">·</span>
        <span>By {{ article.author }}</span>
      </div>
    </div>

    <ScrollIndicator href="#article-body" class="article-hero__scroll" />
  </section>
</template>

<script setup lang="ts">
/**
 * NewsArticleHero — editorial hero for a /news/:slug detail page.
 *
 * Reuses HeroBackground (image + scrim + fade) like the section heroes, then
 * stacks a back-link, category chip, headline, standfirst lede and byline.
 * The hero photo is the article's signature image upsized to 1920w.
 */
import { computed } from 'vue'
import HeroBackground from '@/components/landing/shared/HeroBackground.vue'
import ScrollIndicator from '@/components/landing/shared/ScrollIndicator.vue'
import { CATEGORY_LABEL, formatPublishedDate, type NewsArticle, type NewsCategoryKey } from '@/data/news'

const props = defineProps<{
  article: NewsArticle
  lede?: string
}>()

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
const heroImage = computed(() => props.article.imageUrl.replace('w=1200', 'w=1920'))
</script>

<style scoped>
.article-hero {
  position: relative;
  width: 100%;
  min-height: clamp(560px, 80vh, 900px);
  overflow: hidden;
  isolation: isolate;
  display: flex;
}

.article-hero__content {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  text-align: center;
  width: 100%;
  max-width: 920px;
  margin: 0 auto;
  padding:
    clamp(120px, 16vw, 200px)
    clamp(20px, 4vw, 40px)
    clamp(96px, 12vw, 160px);
  font-family: var(--font-family);
}

.article-hero__back {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  font-size: clamp(12px, 1vw, 13px);
  font-weight: 600;
  letter-spacing: 0.04em;
  color: rgba(255, 255, 255, 0.78);
  text-decoration: none;
  margin-bottom: clamp(20px, 2.6vw, 30px);
  transition: color 0.25s ease, transform 0.25s ease;
}

.article-hero__back:hover {
  color: var(--c-brand-cyan);
}

.article-hero__back:hover .article-hero__back-arrow {
  transform: translateX(-3px);
}

.article-hero__back-arrow {
  font-size: 1.1em;
  transition: transform 0.25s ease;
}

.article-hero__chip {
  display: inline-block;
  padding: 7px 16px;
  border-radius: 999px;
  font-size: clamp(10px, 0.9vw, 12px);
  font-weight: 700;
  letter-spacing: 0.18em;
  text-transform: uppercase;
  line-height: 1.2;
  margin-bottom: clamp(20px, 2.4vw, 28px);
  backdrop-filter: blur(8px);
  -webkit-backdrop-filter: blur(8px);
}

.article-hero__chip--cyan {
  color: var(--c-brand-cyan);
  background: rgba(0, 173, 239, 0.18);
  border: 1px solid rgba(0, 173, 239, 0.45);
}

.article-hero__chip--blue {
  color: #fff;
  background: rgba(21, 117, 187, 0.45);
  border: 1px solid rgba(21, 117, 187, 0.70);
}

.article-hero__chip--red {
  color: #fff;
  background: rgba(229, 45, 39, 0.40);
  border: 1px solid rgba(229, 45, 39, 0.70);
}

.article-hero__title {
  font-size: clamp(30px, 5vw, 56px);
  font-weight: 700;
  line-height: 1.1;
  letter-spacing: -0.02em;
  color: #fff;
  margin: 0;
  max-width: 16ch;
  text-shadow: 0 4px 24px rgba(0, 0, 0, 0.4);
}

.article-hero__lede {
  font-size: clamp(15px, 1.5vw, 19px);
  font-weight: 400;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.86);
  margin: clamp(20px, 2.6vw, 30px) 0 0;
  max-width: 680px;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.35);
}

.article-hero__meta {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  justify-content: center;
  gap: 10px;
  margin-top: clamp(24px, 3vw, 34px);
  font-size: clamp(11px, 0.95vw, 13px);
  font-weight: 600;
  letter-spacing: 0.06em;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.66);
}

.article-hero__dot {
  opacity: 0.7;
}

.article-hero__scroll {
  position: absolute;
  bottom: clamp(20px, 3vw, 36px);
  left: 50%;
  transform: translateX(-50%);
  z-index: 2;
}

@media (max-width: 1023px) {
  .article-hero__scroll {
    display: none;
  }
}
</style>
