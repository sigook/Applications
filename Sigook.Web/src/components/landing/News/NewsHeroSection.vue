<template>
  <section class="news-hero">
    <HeroBackground :image="heroImage" :image-sm="heroImageSm" focal="center 40%" />
    <!-- Atmospheric magnifier decoration — top-center anchor (completes the
         family: Home TL, About TR, SP BR, Industries BL, Talents CR,
         Employers CL → News TC) -->
    <DecoMagnifier class="news-hero__magnifier" />

    <div class="news-hero__content">
      <!-- "Live" pulse + eyebrow — communicates editorial / always-on -->
      <div class="news-hero__eyebrow-row">
        <span class="news-hero__live" aria-hidden="true">
          <span class="news-hero__live-dot"></span>
          <span class="news-hero__live-label">Live</span>
        </span>
        <EyebrowPill variant="cyan" class="news-hero__eyebrow">
          Newsroom
        </EyebrowPill>
      </div>

      <h1 class="news-hero__heading">
        What's happening at <span class="news-hero__heading-accent">Sigook®.</span><br>
        <span class="news-hero__heading-accent">Stories,</span> insights, signals.
      </h1>

      <p class="news-hero__subtitle">
        The newsroom for workforce — industry insights, compliance updates,
        and the placements behind the headlines. Updated weekly.
      </p>

      <!-- Category tabs — the visual signal that this is editorial.
           They scroll the page to the Latest grid; filtering is left as a
           future enhancement (will wire to a query string). -->
      <nav class="news-hero__tabs" aria-label="News categories">
        <button
          v-for="cat in CATEGORIES"
          :key="cat.key"
          type="button"
          class="news-hero__tab"
          :class="{ 'news-hero__tab--active': cat.key === 'all' }"
          @click="onTabClick"
        >
          {{ cat.label }}
        </button>
      </nav>
    </div>

    <ScrollIndicator href="#news-featured" class="news-hero__scroll" />
  </section>
</template>

<script setup lang="ts">
/**
 * News hero — window-style intro that lets GlobalBackground show through.
 *
 * Differentiates from sibling heroes via:
 *  • Magnifier anchor: top-center (last unused slot, completes the family)
 *  • "Live" pulse + Newsroom eyebrow → immediately reads as editorial
 *  • Category tab row → tells the user this is a multi-story archive, not a
 *    single product / service pitch
 *
 * The tabs visually communicate the editorial nature even before the user
 * scrolls. Clicking any tab scrolls to the Latest grid (filter wiring is a
 * future enhancement).
 */
import DecoMagnifier from '@/components/landing/shared/DecoMagnifier.vue'
import HeroBackground from '@/components/landing/shared/HeroBackground.vue'
import heroImage from '@/assets/images/v2/hero/news.webp'
import heroImageSm from '@/assets/images/v2/hero/news-960.webp'
import EyebrowPill from '@/components/landing/shared/EyebrowPill.vue'
import ScrollIndicator from '@/components/landing/shared/ScrollIndicator.vue'
import { NEWS_CATEGORIES } from '@/data/news'

const CATEGORIES = NEWS_CATEGORIES

function onTabClick(): void {
  const target = document.getElementById('news-latest')
  target?.scrollIntoView({ behavior: 'smooth', block: 'start' })
}
</script>

<style scoped>
/* ── Section shell — transparent (GlobalBackground shows through) ───────── */
.news-hero {
  position: relative;
  width: 100%;
  height: auto;
  min-height: max(100vh, 1080px);
  overflow: hidden;
  isolation: isolate;
}

/* ── Decorative magnifier — top-center anchor ───────────────────────────── */
.news-hero__magnifier {
  top: clamp(14%, 16vw, 20%);
  left: 50%;
  transform: translateX(-50%);
}

/* ── Content stack — vertically centered editorial layout ───────────────── */
.news-hero__content {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: max(100vh, 1080px);
  padding:
    clamp(80px, 12vw, 140px)
    clamp(20px, 3vw, 32px)
    clamp(120px, 16vw, 220px);
  text-align: center;
  max-width: 1100px;
  margin: 0 auto;
}

/* ── Eyebrow row — Live pulse + Newsroom pill ───────────────────────────── */
.news-hero__eyebrow-row {
  display: inline-flex;
  align-items: center;
  gap: 14px;
  margin-bottom: clamp(24px, 3.5vw, 36px);
}

.news-hero__live {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 6px 14px;
  border-radius: 999px;
  background: rgba(229, 45, 39, 0.14);
  border: 1px solid rgba(229, 45, 39, 0.45);
  backdrop-filter: blur(8px);
  -webkit-backdrop-filter: blur(8px);
}

.news-hero__live-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: var(--c-brand-red);
  box-shadow: 0 0 0 0 rgba(229, 45, 39, 0.55);
  animation: news-hero-live-pulse 1.8s ease-out infinite;
}

@keyframes news-hero-live-pulse {
  0%   { box-shadow: 0 0 0 0   rgba(229, 45, 39, 0.55); }
  70%  { box-shadow: 0 0 0 12px rgba(229, 45, 39, 0); }
  100% { box-shadow: 0 0 0 0   rgba(229, 45, 39, 0); }
}

@media (prefers-reduced-motion: reduce) {
  .news-hero__live-dot { animation: none; }
}

.news-hero__live-label {
  font-family: var(--font-family);
  font-size: 11px;
  font-weight: 700;
  letter-spacing: 0.18em;
  text-transform: uppercase;
  color: var(--c-brand-red);
}

/* ── Main heading — large editorial with cyan accent ────────────────────── */
.news-hero__heading {
  font-family: var(--font-family);
  font-size: clamp(32px, 5.5vw, 60px);
  font-weight: 700;
  line-height: 1.05;
  letter-spacing: -0.02em;
  color: #fff;
  margin: 0 0 clamp(24px, 3.5vw, 36px);
  max-width: 920px;
  text-shadow: 0 4px 24px rgba(0, 0, 0, 0.40);
}

.news-hero__heading-accent {
  color: var(--c-brand-cyan);
}

/* ── Subtitle / value prop ──────────────────────────────────────────────── */
.news-hero__subtitle {
  font-family: var(--font-family);
  font-size: clamp(14px, 1.4vw, 17px);
  font-weight: 400;
  line-height: 1.65;
  color: rgba(255, 255, 255, 0.85);
  margin: 0 0 clamp(36px, 5vw, 56px);
  max-width: 680px;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.35);
}

/* ── Category tabs — the editorial signal ───────────────────────────────── */
.news-hero__tabs {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: clamp(8px, 1vw, 12px);
  max-width: 760px;
}

.news-hero__tab {
  font-family: var(--font-family);
  font-size: clamp(12px, 1vw, 13px);
  font-weight: 600;
  letter-spacing: 0.04em;
  padding: clamp(8px, 0.9vw, 11px) clamp(16px, 1.7vw, 22px);
  border-radius: 999px;
  background: rgba(255, 255, 255, 0.06);
  border: 1px solid rgba(255, 255, 255, 0.18);
  color: rgba(255, 255, 255, 0.80);
  backdrop-filter: blur(10px) saturate(140%);
  -webkit-backdrop-filter: blur(10px) saturate(140%);
  cursor: pointer;
  transition:
    background 0.25s ease,
    border-color 0.25s ease,
    color 0.25s ease,
    transform 0.25s ease;
}

.news-hero__tab:hover {
  background: rgba(255, 255, 255, 0.12);
  border-color: rgba(255, 255, 255, 0.35);
  color: #fff;
  transform: translateY(-1px);
}

.news-hero__tab--active {
  background: var(--c-brand-cyan);
  border-color: var(--c-brand-cyan);
  color: var(--c-brand-navy);
  box-shadow: 0 8px 20px rgba(0, 173, 239, 0.30);
}

.news-hero__tab--active:hover {
  background: var(--c-brand-cyan);
  border-color: var(--c-brand-cyan);
  color: var(--c-brand-navy);
  transform: translateY(-1px);
}

/* ── Scroll indicator ───────────────────────────────────────────────────── */
.news-hero__scroll {
  position: absolute;
  bottom: clamp(20px, 3vw, 36px);
  left: 50%;
  transform: translateX(-50%);
  z-index: 2;
}

/* ── Mobile-only behaviors ──────────────────────────────────────────────── */
@media (max-width: 1023px) {
  .news-hero {
    min-height: 100svh;
  }

  .news-hero__content {
    min-height: 100svh;
  }

  .news-hero__scroll {
    display: none;
  }

  .news-hero__magnifier {
    top: 14%;
  }
}
</style>
