<template>
  <section id="news-latest" class="news-latest">
    <LandingSectionHeader
      eyebrow="Latest"
      heading="Fresh off the press."
      heading-accent="Updated weekly."
      subtitle="The most recent stories from the newsroom — sorted by date, freshest first. Skim the cards, click into anything that catches your eye."
      subtitle-max-width="580px"
    />

    <div class="news-latest__grid">
      <NewsCard
        v-for="(article, idx) in ARTICLES"
        :key="article.id"
        :article="article"
        :delay="idx * 90"
      />
    </div>

    <div class="news-latest__footer">
      <ArrowPillCta href="#" hover-variant="cyan">View the full archive</ArrowPillCta>
    </div>
  </section>
</template>

<script setup lang="ts">
/**
 * NewsLatestGridSection — window-style grid of the 6 most recent articles.
 *
 * Sits between two panels in the page rhythm
 * (window → panel → WINDOW → panel → window). Uses NewsCard shared atom
 * so any future tweaks to the card travel everywhere.
 *
 * The footer link points to a future archive route — kept as `#` until
 * the article detail page lands.
 */
import LandingSectionHeader from '@/components/landing/shared/LandingSectionHeader.vue'
import ArrowPillCta from '@/components/landing/shared/ArrowPillCta.vue'
import NewsCard from '@/components/landing/shared/NewsCard.vue'
import { getLatestArticles } from '@/data/news'

const ARTICLES = getLatestArticles(6)
</script>

<style scoped>
/* ── Window shell — transparent (GlobalBackground shows through) ────────── */
.news-latest {
  position: relative;
  width: 100%;
  padding:
    clamp(72px, 10vw, 140px)
    clamp(20px, 3vw, 40px)
    clamp(96px, 12vw, 180px);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: clamp(48px, 6vw, 80px);
  isolation: isolate;
  overflow: hidden;
  font-family: var(--font-family);
}

/* ── Grid — 3 cols desktop, 2 tablet, 1 mobile ──────────────────────────── */
.news-latest__grid {
  position: relative;
  z-index: 2;
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: clamp(22px, 2.6vw, 32px);
  align-items: stretch;
  width: 100%;
  max-width: 1180px;
  margin: 0 auto;
}

/* ── Footer — view-all link ─────────────────────────────────────────────── */
.news-latest__footer {
  position: relative;
  z-index: 2;
}

/* ── Responsive ─────────────────────────────────────────────────────────── */
@media (max-width: 1023px) {
  .news-latest__grid { grid-template-columns: repeat(2, 1fr); }
}

@media (max-width: 639px) {
  .news-latest__grid { grid-template-columns: 1fr; }
}
</style>
