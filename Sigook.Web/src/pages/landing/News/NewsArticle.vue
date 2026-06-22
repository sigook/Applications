<template>
  <main v-if="article && content" class="news-article">
    <NewsArticleHero :article="article" :lede="content.lede" />

    <div id="article-body" class="news-article__body">
      <template v-for="(section, i) in content.sections" :key="section.heading">
        <NewsArticleSection
          tone="panel"
          :corner="i % 2 === 0 ? 'a' : 'b'"
          :heading="section.heading"
          :image="section.imageUrl"
          :image-alt="section.imageAlt"
          :image-side="section.imageSide || 'right'"
        >
          <p v-for="(paragraph, j) in section.paragraphs" :key="j">{{ paragraph }}</p>
        </NewsArticleSection>

        <!-- ── Stats band — after the first section ──────────────────────── -->
        <section v-if="i === 0" class="article-band article-stats">
          <span class="article-band__eyebrow">By the numbers</span>
          <div class="article-stats__grid">
            <div v-for="(stat, s) in content.keyStats" :key="s" class="article-stats__card">
              <span class="article-stats__value">{{ stat.value }}</span>
              <span class="article-stats__label">{{ stat.label }}</span>
            </div>
          </div>
        </section>

        <!-- ── Pull quote — after the second section ─────────────────────── -->
        <section v-if="i === 1" class="article-band article-quote">
          <figure class="article-quote__figure">
            <span class="article-quote__mark" aria-hidden="true">&ldquo;</span>
            <blockquote class="article-quote__text">{{ content.pullQuote.text }}</blockquote>
            <figcaption class="article-quote__cite">{{ content.pullQuote.attribution }}</figcaption>
          </figure>
        </section>

        <!-- ── Takeaways — after the third section ───────────────────────── -->
        <section v-if="i === 2" class="article-band article-takeaways">
          <div class="article-takeaways__card">
            <h2 class="article-takeaways__heading">{{ content.takeaways.heading }}</h2>
            <ul class="article-takeaways__list">
              <li v-for="(point, p) in content.takeaways.points" :key="p">{{ point }}</li>
            </ul>
          </div>
        </section>
      </template>

      <!-- ── Sources + related — closing window ──────────────────────────── -->
      <section class="article-band article-outro">
        <div class="article-outro__sources">
          <span class="article-band__eyebrow">Sources</span>
          <ul class="article-outro__list">
            <li v-for="(source, i) in content.sources" :key="i">{{ source }}</li>
          </ul>
        </div>

        <div class="article-related">
          <h2 class="article-related__heading">More from the Newsroom</h2>
          <div class="article-related__grid">
            <NewsCard
              v-for="(item, i) in related"
              :key="item.id"
              :article="item"
              :delay="i * 90"
              class="article-related__card"
            />
          </div>
        </div>
      </section>
    </div>

    <div id="news-article-contact">
      <ContactSection />
    </div>
  </main>
</template>

<script setup lang="ts">
/**
 * NewsArticle — the /news/:slug detail page.
 *
 * One dynamic page renders every article: card metadata (title, category,
 * hero image, date, author, read time) comes from NEWS_ARTICLES, the expanded
 * body comes from newsContent.ts. The page follows the landing rhythm —
 * hero → panel(section) → window(band) → panel(section) → … — by rendering
 * each body section as an alternating-corner panel with a window band
 * (stats / quote / takeaways) between them. Unknown slugs redirect to /news.
 */
import { computed, watchEffect } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import NewsArticleHero from '@/components/landing/News/NewsArticleHero.vue'
import NewsArticleSection from '@/components/landing/News/NewsArticleSection.vue'
import NewsCard from '@/components/landing/shared/NewsCard.vue'
import ContactSection from '@/components/landing/shared/ContactSection.vue'
import { NEWS_ARTICLES } from '@/data/news'
import { getArticleContent } from '@/data/newsContent'

const route = useRoute()
const router = useRouter()

const slug = computed(() => String(route.params.slug ?? ''))
const article = computed(() => NEWS_ARTICLES.find((a) => a.slug === slug.value))
const content = computed(() => getArticleContent(slug.value))
const related = computed(() =>
  NEWS_ARTICLES.filter((a) => a.slug !== slug.value).slice(0, 3),
)

watchEffect(() => {
  if (slug.value && (!article.value || !content.value)) {
    router.replace('/news')
  }
})
</script>

<style scoped>
.news-article {
  position: relative;
  width: 100%;
  font-family: var(--font-family);
}

.news-article__body {
  position: relative;
  scroll-margin-top: 90px;
}

/* ── Shared band (window-tone insert between panels) ─────────────────────── */
.article-band {
  position: relative;
  z-index: 2;
  width: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  padding:
    clamp(120px, 13vw, 190px)
    clamp(20px, 3vw, 40px);
}

.article-band__eyebrow {
  display: inline-block;
  font-size: clamp(10px, 0.9vw, 12px);
  font-weight: 700;
  letter-spacing: 0.22em;
  text-transform: uppercase;
  color: var(--c-brand-cyan);
  margin-bottom: clamp(20px, 2.4vw, 28px);
}

/* ── Stats band ─────────────────────────────────────────────────────────── */
.article-stats__grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: clamp(16px, 2vw, 24px);
  width: 100%;
  max-width: 1080px;
}

.article-stats__card {
  display: flex;
  flex-direction: column;
  gap: 10px;
  padding: clamp(22px, 2.6vw, 32px);
  border-radius:
    clamp(16px, 2vw, 24px) clamp(28px, 3.4vw, 44px)
    clamp(16px, 2vw, 24px) clamp(28px, 3.4vw, 44px);
  background: linear-gradient(180deg,
    rgba(255, 255, 255, 0.08) 0%,
    rgba(255, 255, 255, 0.03) 100%);
  backdrop-filter: blur(16px) saturate(150%);
  -webkit-backdrop-filter: blur(16px) saturate(150%);
  border: 1px solid rgba(255, 255, 255, 0.16);
}

.article-stats__value {
  font-size: clamp(28px, 3.4vw, 44px);
  font-weight: 700;
  line-height: 1;
  letter-spacing: -0.02em;
  color: var(--c-brand-cyan);
}

.article-stats__label {
  font-size: clamp(12px, 1vw, 13px);
  line-height: 1.45;
  color: rgba(255, 255, 255, 0.74);
}

/* ── Pull quote ─────────────────────────────────────────────────────────── */
.article-quote__figure {
  position: relative;
  max-width: 860px;
  margin: 0;
  text-align: center;
}

.article-quote__mark {
  display: block;
  font-size: clamp(80px, 10vw, 130px);
  line-height: 0.7;
  color: var(--c-brand-cyan);
  opacity: 0.55;
  margin-bottom: clamp(4px, 1vw, 10px);
}

.article-quote__text {
  font-size: clamp(20px, 2.6vw, 32px);
  font-weight: 600;
  line-height: 1.4;
  letter-spacing: -0.01em;
  color: #fff;
  margin: 0;
  text-shadow: 0 2px 18px rgba(0, 0, 0, 0.3);
}

.article-quote__cite {
  margin-top: clamp(18px, 2.2vw, 26px);
  font-size: clamp(12px, 1.05vw, 14px);
  font-weight: 600;
  letter-spacing: 0.1em;
  text-transform: uppercase;
  color: rgba(255, 255, 255, 0.62);
}

/* ── Takeaways ──────────────────────────────────────────────────────────── */
.article-takeaways__card {
  width: 100%;
  max-width: 880px;
  padding: clamp(32px, 4vw, 52px);
  border-radius:
    0 clamp(40px, 5vw, 64px)
    0 clamp(40px, 5vw, 64px);
  background: linear-gradient(135deg,
    rgba(0, 173, 239, 0.16) 0%,
    rgba(21, 117, 187, 0.12) 55%,
    rgba(229, 45, 39, 0.12) 100%);
  backdrop-filter: blur(18px) saturate(150%);
  -webkit-backdrop-filter: blur(18px) saturate(150%);
  border: 1px solid rgba(255, 255, 255, 0.18);
  box-shadow: 0 20px 44px -22px rgba(0, 0, 0, 0.5);
}

.article-takeaways__heading {
  font-size: clamp(20px, 2.4vw, 28px);
  font-weight: 700;
  line-height: 1.25;
  letter-spacing: -0.01em;
  color: #fff;
  margin: 0 0 clamp(20px, 2.4vw, 28px);
}

.article-takeaways__list {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: clamp(12px, 1.6vw, 18px);
}

.article-takeaways__list li {
  position: relative;
  padding-left: 34px;
  font-size: clamp(14px, 1.2vw, 16px);
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.86);
}

.article-takeaways__list li::before {
  content: '✓';
  position: absolute;
  left: 0;
  top: 0;
  width: 22px;
  height: 22px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
  font-weight: 700;
  color: var(--c-brand-navy);
  background: var(--c-brand-cyan);
  border-radius: 50%;
}

/* ── Outro — sources + related ──────────────────────────────────────────── */
.article-outro {
  gap: clamp(56px, 7vw, 96px);
}

.article-outro__sources {
  width: 100%;
  max-width: 1080px;
}

.article-outro__list {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.article-outro__list li {
  position: relative;
  padding-left: 18px;
  font-size: clamp(12px, 1vw, 13px);
  line-height: 1.55;
  color: rgba(255, 255, 255, 0.6);
}

.article-outro__list li::before {
  content: '';
  position: absolute;
  left: 0;
  top: 0.6em;
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.4);
}

/* ── Related ────────────────────────────────────────────────────────────── */
.article-related {
  width: 100%;
  max-width: 1080px;
}

.article-related__heading {
  font-size: clamp(22px, 2.6vw, 32px);
  font-weight: 700;
  letter-spacing: -0.015em;
  color: #fff;
  margin: 0 0 clamp(28px, 3.4vw, 40px);
  text-align: center;
}

.article-related__grid {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: clamp(22px, 2.6vw, 32px);
}

.article-related__card {
  flex: 0 1 calc((100% - 2 * clamp(22px, 2.6vw, 32px)) / 3);
}

/* ── Responsive ─────────────────────────────────────────────────────────── */
@media (max-width: 1023px) {
  .article-stats__grid { grid-template-columns: repeat(2, 1fr); }
  .article-related__card { flex-basis: calc((100% - clamp(22px, 2.6vw, 32px)) / 2); }
}

@media (max-width: 639px) {
  .article-stats__grid { grid-template-columns: 1fr; }
  .article-related__card { flex-basis: 100%; }
}
</style>
