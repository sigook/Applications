<template>
  <section class="article-section" :class="rootClass">
    <div v-if="tone === 'panel'" class="article-section__surface" aria-hidden="true"></div>

    <div class="article-section__inner" :class="{ 'article-section__inner--split': image }">
      <div class="article-section__body">
        <span v-if="eyebrow" class="article-section__eyebrow">{{ eyebrow }}</span>
        <h2 class="article-section__heading">{{ heading }}</h2>
        <div class="article-section__prose">
          <slot />
        </div>
      </div>

      <figure v-if="image" class="article-section__media">
        <img :src="image" :alt="imageAlt || ''" loading="lazy" />
      </figure>
    </div>
  </section>
</template>

<script setup lang="ts">
/**
 * NewsArticleSection — one body block of a news article.
 *
 * Reproduces the landing's panel / window rhythm: `panel` carries the glass
 * surface, brand-asymmetric radius (corner `a` = TL+BR, `b` = TR+BL) and the
 * float shadow; `window` is transparent so GlobalBackground shows through.
 * With an `image` the inner becomes a two-column split (text + media), with
 * the photo on the requested side and stacked on mobile.
 */
import { computed } from 'vue'

const props = withDefaults(defineProps<{
  tone?: 'panel' | 'window'
  corner?: 'a' | 'b'
  eyebrow?: string
  heading: string
  image?: string
  imageAlt?: string
  imageSide?: 'left' | 'right'
}>(), {
  tone: 'panel',
  corner: 'a',
  imageSide: 'right',
})

const rootClass = computed(() => [
  `article-section--${props.tone}`,
  props.tone === 'panel' ? `article-section--corner-${props.corner}` : 'article-section--window',
  props.image ? `article-section--media-${props.imageSide}` : '',
])
</script>

<style scoped>
.article-section {
  position: relative;
  width: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  isolation: isolate;
  font-family: var(--font-family);
}

/* ── Window tone — transparent, GlobalBackground shows through ───────────── */
.article-section--window {
  padding:
    clamp(64px, 8vw, 120px)
    clamp(20px, 3vw, 40px);
}

/* ── Panel tone — glass surface, asymmetric radius, float shadow, overlap ─── */
.article-section--panel {
  margin-top: clamp(-110px, -6.5vw, -60px);
  padding:
    clamp(120px, 13vw, 190px)
    clamp(20px, 3vw, 64px);
  z-index: 5;
  box-shadow:
    0 -22px 40px -12px rgba(0, 0, 0, 0.45),
    0  22px 40px -12px rgba(0, 0, 0, 0.45);
}

.article-section--corner-a {
  border-radius:
    clamp(80px, 10vw, 150px) 0
    clamp(80px, 10vw, 150px) 0;
}

.article-section--corner-b {
  border-radius:
    0 clamp(80px, 10vw, 150px)
    0 clamp(80px, 10vw, 150px);
}

.article-section--panel::before {
  content: '';
  position: absolute;
  top: -16px;
  bottom: -16px;
  left: 0;
  right: 0;
  z-index: -1;
  border-radius: inherit;
  background: rgba(255, 255, 255, 0.07);
  backdrop-filter: blur(10px) saturate(120%);
  -webkit-backdrop-filter: blur(10px) saturate(120%);
  border: 1px solid rgba(255, 255, 255, 0.14);
  box-shadow: 0 18px 40px -20px rgba(0, 0, 0, 0.4);
  pointer-events: none;
}

.article-section__surface {
  position: absolute;
  inset: 0;
  z-index: 0;
  border-radius: inherit;
  background: linear-gradient(
    180deg,
    rgba(9, 48, 85, 0.65) 0%,
    rgba(9, 48, 85, 0.55) 100%
  );
  backdrop-filter: blur(20px) saturate(160%);
  -webkit-backdrop-filter: blur(20px) saturate(160%);
  pointer-events: none;
}

/* ── Inner content ──────────────────────────────────────────────────────── */
.article-section__inner {
  position: relative;
  z-index: 2;
  width: 100%;
  max-width: 1080px;
  margin: 0 auto;
}

.article-section__inner--split {
  display: grid;
  grid-template-columns: 1.1fr 0.9fr;
  align-items: center;
  gap: clamp(28px, 4vw, 64px);
}

.article-section--media-left .article-section__inner--split {
  grid-template-columns: 0.9fr 1.1fr;
}

.article-section--media-left .article-section__media {
  order: -1;
}

.article-section__body {
  min-width: 0;
}

.article-section__eyebrow {
  display: inline-block;
  font-size: clamp(10px, 0.9vw, 12px);
  font-weight: 700;
  letter-spacing: 0.22em;
  text-transform: uppercase;
  color: var(--c-brand-cyan);
  margin-bottom: clamp(12px, 1.4vw, 18px);
}

.article-section__heading {
  font-size: clamp(24px, 3vw, 38px);
  font-weight: 700;
  line-height: 1.18;
  letter-spacing: -0.015em;
  color: #fff;
  margin: 0 0 clamp(18px, 2.2vw, 28px);
  text-shadow: 0 2px 16px rgba(0, 0, 0, 0.3);
}

.article-section__prose :deep(p) {
  font-size: clamp(15px, 1.25vw, 17px);
  font-weight: 400;
  line-height: 1.75;
  color: rgba(255, 255, 255, 0.84);
  margin: 0 0 clamp(16px, 1.6vw, 20px);
}

.article-section__prose :deep(p:last-child) {
  margin-bottom: 0;
}

/* ── Media ──────────────────────────────────────────────────────────────── */
.article-section__media {
  position: relative;
  margin: 0;
  border-radius: clamp(20px, 2.4vw, 32px);
  overflow: hidden;
  box-shadow: 0 20px 44px -18px rgba(0, 0, 0, 0.55);
  border: 1px solid rgba(255, 255, 255, 0.14);
}

.article-section__media img {
  display: block;
  width: 100%;
  height: 100%;
  aspect-ratio: 4 / 3;
  object-fit: cover;
}

/* ── Responsive ─────────────────────────────────────────────────────────── */
@media (max-width: 899px) {
  .article-section__inner--split,
  .article-section--media-left .article-section__inner--split {
    grid-template-columns: 1fr;
    gap: clamp(22px, 5vw, 32px);
  }

  .article-section--media-left .article-section__media {
    order: 0;
  }
}
</style>
