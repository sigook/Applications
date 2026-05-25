<template>
  <section class="news-topics">
    <!-- Inner glass surface — materializes the panel against GlobalBackground -->
    <div class="news-topics__surface" aria-hidden="true"></div>

    <header class="news-topics__header">
      <EyebrowPill variant="white" class="news-topics__eyebrow">
        Topics
      </EyebrowPill>

      <h2 class="news-topics__heading">
        Browse by topic.
        <span class="news-topics__heading-accent">Pick your beat.</span>
      </h2>

      <p class="news-topics__subtitle">
        Cut through the feed — every story is filed under one of five beats.
        Follow the one that matters most for your role.
      </p>
    </header>

    <div class="news-topics__grid">
      <SecondaryCard
        v-for="(topic, idx) in TOPICS"
        :key="topic.key"
        :variant="topic.variant"
        :eyebrow="topic.eyebrow"
        :title="topic.title"
        :delay="idx * 110"
        class="news-topics__card"
      >
        <template #icon>
          <component :is="topic.iconComponent" />
        </template>

        {{ topic.body }}

        <template #button>
          <a
            :href="topic.href"
            class="news-topics__cta"
            :class="`news-topics__cta--${topic.variant}`"
          >
            <span>{{ topic.ctaLabel }}</span>
            <span class="news-topics__cta-arrow" aria-hidden="true">→</span>
          </a>
        </template>
      </SecondaryCard>
    </div>
  </section>
</template>

<script setup lang="ts">
/**
 * NewsTopicsSection — panel-style 6-card grid for category browsing.
 *
 * One card per beat (5) + a leading "All stories" overview card. Each card
 * is a SecondaryCard with the canonical 3-variant palette and an inline SVG
 * icon. The CTA scrolls back to the Latest grid (a future article archive
 * will replace this).
 */
import { h, type FunctionalComponent } from 'vue'
import EyebrowPill from '@/components/v2/landing/shared/EyebrowPill.vue'
import SecondaryCard, { type SecondaryCardVariant } from '@/components/v2/landing/shared/SecondaryCard.vue'
import { getCategoryCounts } from '@/data/news'

const counts = getCategoryCounts()
const totalStories =
  counts['industry-insights'] +
  counts['company-news'] +
  counts['press-releases'] +
  counts['hiring-trends'] +
  counts['compliance']

/**
 * Tiny inline SVG icons — line-art set, currentColor stroke.
 * Functional components keep the bundle minimal and let us pass them
 * straight to `<component :is="...">`.
 */
const svg = (paths: string[]): FunctionalComponent => () =>
  h('svg', {
    viewBox: '0 0 24 24',
    fill: 'none',
    stroke: 'currentColor',
    'stroke-width': 1.5,
    'stroke-linecap': 'round',
    'stroke-linejoin': 'round',
    'aria-hidden': true,
    style: 'width:1em;height:1em;display:block;color:inherit;',
  }, paths.map((d) => h('path', { d })))

// Layered-rectangles overview icon
const IconAll        = svg(['M3 8h18', 'M3 12h18', 'M3 16h18', 'M3 4h18'])
// Lightbulb-ish insights icon
const IconInsights   = svg(['M9 18h6', 'M10 22h4', 'M12 2a7 7 0 0 0-4 12.7c.6.4 1 1.1 1 1.8V18h6v-1.5c0-.7.4-1.4 1-1.8A7 7 0 0 0 12 2z'])
// Trending-up arrow
const IconTrending   = svg(['M3 17l6-6 4 4 8-8', 'M14 7h7v7'])
// Shield-check compliance icon
const IconCompliance = svg(['M12 2 4 5v7c0 5 4 9 8 10 4-1 8-5 8-10V5l-8-3z', 'm9 12 2 2 4-4'])
// Building company icon
const IconCompany    = svg(['M3 21h18', 'M5 21V8l7-4 7 4v13', 'M9 9h.01M13 9h.01M9 13h.01M13 13h.01M9 17h.01M13 17h.01'])
// Megaphone press icon
const IconPress      = svg(['M3 11l16-7v16L3 13z', 'M3 11v2', 'M9 18a3 3 0 0 1-3 3'])

interface Topic {
  readonly key: string
  readonly eyebrow: string
  readonly title: string
  readonly body: string
  readonly ctaLabel: string
  readonly href: string
  readonly variant: SecondaryCardVariant
  readonly iconComponent: FunctionalComponent
}

const TOPICS: readonly Topic[] = [
  {
    key: 'all',
    eyebrow: `${totalStories} stories`,
    title: 'All stories',
    body:
      'The complete feed — every newsroom piece in one place, sorted by date. Skim everything in under five minutes.',
    ctaLabel: 'Browse all',
    href: '#news-latest',
    variant: 'cyan',
    iconComponent: IconAll,
  },
  {
    key: 'industry-insights',
    eyebrow: `${counts['industry-insights']} stories`,
    title: 'Industry insights',
    body:
      'Deep dives into the sectors we recruit for — talent gaps, salary signals, and the trends shaping how work gets done.',
    ctaLabel: 'Read insights',
    href: '#news-latest',
    variant: 'cyan',
    iconComponent: IconInsights,
  },
  {
    key: 'hiring-trends',
    eyebrow: `${counts['hiring-trends']} stories`,
    title: 'Hiring trends',
    body:
      'The data behind hiring — pulse reports, market shifts, and what our recruiters are seeing in live placements.',
    ctaLabel: 'See trends',
    href: '#news-latest',
    variant: 'blue',
    iconComponent: IconTrending,
  },
  {
    key: 'compliance',
    eyebrow: `${counts['compliance']} stories`,
    title: 'Compliance',
    body:
      'Wage updates, safety standards, and regulatory shifts — what HR and operations need on their radar this quarter.',
    ctaLabel: 'Stay compliant',
    href: '#news-latest',
    variant: 'red',
    iconComponent: IconCompliance,
  },
  {
    key: 'company-news',
    eyebrow: `${counts['company-news']} stories`,
    title: 'Company news',
    body:
      'Inside Sigook — new offices, leadership announcements, programs, and the milestones we want you to know about.',
    ctaLabel: 'Read updates',
    href: '#news-latest',
    variant: 'blue',
    iconComponent: IconCompany,
  },
  {
    key: 'press-releases',
    eyebrow: `${counts['press-releases']} stories`,
    title: 'Press releases',
    body:
      'Official statements, certifications, and downloadable assets for press, analysts, and partners.',
    ctaLabel: 'View releases',
    href: '#news-latest',
    variant: 'cyan',
    iconComponent: IconPress,
  },
] as const
</script>

<style scoped>
/* ── Panel shell — radius mirrored from Featured (TR+BL) for visual zig-zag */
.news-topics {
  position: relative;
  width: 100%;
  margin-top: clamp(-180px, -10vw, -80px);
  padding:
    clamp(140px, 14vw, 200px)
    clamp(20px, 3vw, 64px);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: clamp(48px, 6vw, 80px);
  z-index: 5;
  border-radius:
    0 clamp(80px, 10vw, 150px)
    0 clamp(80px, 10vw, 150px);
  box-shadow:
    0 -22px 40px -12px rgba(0, 0, 0, 0.45),
    0  22px 40px -12px rgba(0, 0, 0, 0.45);
  overflow: hidden;
  isolation: isolate;
  font-family: var(--font-family);
}

.news-topics__surface {
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
.news-topics__header {
  position: relative;
  z-index: 2;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  max-width: 880px;
  margin: 0 auto;
}

.news-topics__eyebrow {
  margin-bottom: clamp(20px, 2.5vw, 28px);
}

.news-topics__heading {
  font-size: clamp(28px, 4.2vw, 46px);
  font-weight: 700;
  line-height: 1.15;
  letter-spacing: -0.02em;
  color: #fff;
  margin: 0 0 clamp(14px, 1.8vw, 22px);
  max-width: 780px;
  text-shadow: 0 4px 24px rgba(0, 0, 0, 0.40);
}

.news-topics__heading-accent {
  color: var(--c-brand-cyan);
  display: block;
}

.news-topics__subtitle {
  font-size: clamp(13px, 1.2vw, 16px);
  font-weight: 400;
  line-height: 1.6;
  color: rgba(255, 255, 255, 0.78);
  margin: 0;
  max-width: 580px;
  text-shadow: 0 2px 12px rgba(0, 0, 0, 0.30);
}

/* ── Grid — 3 cols desktop, 2 tablet, 1 mobile ──────────────────────────── */
.news-topics__grid {
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

.news-topics__card {
  display: flex;
  flex-direction: column;
}

/* ── CTA pill in the #button slot ───────────────────────────────────────── */
.news-topics__cta {
  display: inline-flex;
  align-items: center;
  gap: 10px;
  padding: clamp(10px, 1.1vw, 12px) clamp(18px, 2vw, 24px);
  background: rgba(255, 255, 255, 0.10);
  border: 1px solid rgba(255, 255, 255, 0.40);
  border-radius: 999px;
  color: #fff;
  font-family: var(--font-family);
  font-size: clamp(12px, 1vw, 13px);
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

.news-topics__cta--blue:hover,
.news-topics__cta--cyan:hover {
  background: var(--c-brand-cyan);
  border-color: var(--c-brand-cyan);
  color: var(--c-brand-navy);
  transform: translateX(4px);
}

.news-topics__cta--red:hover {
  background: #fff;
  border-color: #fff;
  color: var(--c-brand-red);
  transform: translateX(4px);
}

.news-topics__cta-arrow {
  font-size: 1.15em;
  line-height: 1;
  transition: transform 0.25s ease;
}

.news-topics__cta:hover .news-topics__cta-arrow {
  transform: translateX(3px);
}

/* ── Responsive ─────────────────────────────────────────────────────────── */
@media (max-width: 1023px) {
  .news-topics__grid { grid-template-columns: repeat(2, 1fr); }
}

@media (max-width: 639px) {
  .news-topics__grid { grid-template-columns: 1fr; }
}
</style>
