<template>
  <section id="news-latest" class="news-latest">
    <header class="news-latest__header">
      <EyebrowPill variant="white" class="news-latest__eyebrow">
        Latest
      </EyebrowPill>

      <h2 class="news-latest__heading">
        Fresh off the press.
        <span class="news-latest__heading-accent">
          Updated weekly.
        </span>
      </h2>

      <p class="news-latest__subtitle">
        The most recent stories from the newsroom — sorted by date, freshest
        first. Skim the cards, click into anything that catches your eye.
      </p>
    </header>

    <div class="news-latest__grid">
      <NewsCard
        v-for="(article, idx) in ARTICLES"
        :key="article.id"
        :article="article"
        :delay="idx * 90"
      />
    </div>

    <div class="news-latest__footer">
      <a href="#" class="news-latest__view-all">
        <span>View the full archive</span>
        <span class="news-latest__view-all-arrow" aria-hidden="true">→</span>
      </a>
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
import EyebrowPill from '@/components/v2/landing/shared/EyebrowPill.vue'
import NewsCard from '@/components/v2/landing/shared/NewsCard.vue'
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

/* ── Header ─────────────────────────────────────────────────────────────── */
.news-latest__header {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  max-width: 880px;
  margin: 0 auto;
}

.news-latest__eyebrow {
  margin-bottom: clamp(20px, 2.5vw, 28px);
}

.news-latest__heading {
  font-size: clamp(28px, 4.2vw, 46px);
  font-weight: 700;
  line-height: 1.15;
  letter-spacing: -0.02em;
  color: #fff;
  margin: 0 0 clamp(14px, 1.8vw, 22px);
  max-width: 780px;
  text-shadow: 0 4px 24px rgba(0, 0, 0, 0.40);
}

.news-latest__heading-accent {
  color: var(--c-brand-cyan);
  display: block;
}

.news-latest__subtitle {
  font-size: clamp(13px, 1.2vw, 16px);
  font-weight: 400;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.78);
  margin: 0;
  max-width: 580px;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.30);
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

.news-latest__view-all {
  display: inline-flex;
  align-items: center;
  gap: 10px;
  padding: clamp(11px, 1.2vw, 14px) clamp(24px, 2.4vw, 32px);
  background: rgba(255, 255, 255, 0.08);
  border: 1px solid rgba(255, 255, 255, 0.35);
  border-radius: 999px;
  color: #fff;
  font-family: var(--font-family);
  font-size: clamp(13px, 1.05vw, 14px);
  font-weight: 600;
  letter-spacing: 0.04em;
  text-decoration: none;
  cursor: pointer;
  transition:
    background 0.25s ease,
    border-color 0.25s ease,
    color 0.25s ease,
    transform 0.25s ease;
}

.news-latest__view-all:hover {
  background: var(--c-brand-cyan);
  border-color: var(--c-brand-cyan);
  color: var(--c-brand-navy);
  transform: translateY(-2px);
}

.news-latest__view-all-arrow {
  font-size: 1.15em;
  line-height: 1;
  transition: transform 0.25s ease;
}

.news-latest__view-all:hover .news-latest__view-all-arrow {
  transform: translateX(3px);
}

/* ── Responsive ─────────────────────────────────────────────────────────── */
@media (max-width: 1023px) {
  .news-latest__grid { grid-template-columns: repeat(2, 1fr); }
}

@media (max-width: 639px) {
  .news-latest__grid { grid-template-columns: 1fr; }
}
</style>
