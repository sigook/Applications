<template>
  <section class="news-topics">
    <!-- Inner glass surface — materializes the panel against GlobalBackground -->
    <div class="news-topics__surface" aria-hidden="true"></div>

    <LandingSectionHeader
      eyebrow="Topics"
      heading="Browse by topic."
      heading-accent="Pick your beat."
      subtitle="Cut through the feed — every story is filed under one of five beats. Follow the one that matters most for your role."
      subtitle-max-width="580px"
    />

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
          <ArrowPillCta
            :href="topic.href"
            size="sm"
            :hover-variant="topic.variant === 'red' ? 'red' : 'cyan'"
          >
            {{ topic.ctaLabel }}
          </ArrowPillCta>
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
import LandingSectionHeader from '@/components/landing/shared/LandingSectionHeader.vue'
import ArrowPillCta from '@/components/landing/shared/ArrowPillCta.vue'
import SecondaryCard, { type SecondaryCardVariant } from '@/components/landing/shared/SecondaryCard.vue'
import { getCategoryCounts, type NewsCategoryKey } from '@/data/news'

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

const ALL_TOPICS: readonly Topic[] = [
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
      'Inside Sigook® — new offices, leadership announcements, programs, and the milestones we want you to know about.',
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

const TOPICS = ALL_TOPICS.filter(
  (topic) => topic.key === 'all' || counts[topic.key as NewsCategoryKey] > 0,
)
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
  isolation: isolate;
  font-family: var(--font-family);
}

/* Depth back-layer — transparent glass, peeks ~16px top & bottom (mirror shape) */
.news-topics::before {
  content: '';
  position: absolute;
  top: -16px;
  bottom: -16px;
  left: 0;
  right: 0;
  z-index: -1;
  border-radius:
    0 clamp(80px, 10vw, 150px)
    0 clamp(80px, 10vw, 150px);
  background: rgba(255, 255, 255, 0.07);
  backdrop-filter: blur(10px) saturate(120%);
  -webkit-backdrop-filter: blur(10px) saturate(120%);
  border: 1px solid rgba(255, 255, 255, 0.14);
  box-shadow: 0 18px 40px -20px rgba(0, 0, 0, 0.4);
  pointer-events: none;
}

.news-topics__surface {
  position: absolute;
  inset: 0;
  z-index: 0;
  border-radius:
    0 clamp(80px, 10vw, 150px)
    0 clamp(80px, 10vw, 150px);
  background: linear-gradient(
    180deg,
    rgba(9, 48, 85, 0.65) 0%,
    rgba(9, 48, 85, 0.55) 100%
  );
  backdrop-filter: blur(20px) saturate(160%);
  -webkit-backdrop-filter: blur(20px) saturate(160%);
  pointer-events: none;
}

/* ── Grid — 3 cols desktop, 2 tablet, 1 mobile ──────────────────────────── */
.news-topics__grid {
  position: relative;
  z-index: 2;
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: clamp(22px, 2.6vw, 32px);
  align-items: stretch;
  width: 100%;
  max-width: 1180px;
  margin: 0 auto;
}

.news-topics__card {
  display: flex;
  flex-direction: column;
  flex: 0 1 calc((100% - 2 * clamp(22px, 2.6vw, 32px)) / 3);
}

/* ── Responsive — flex-basis drives the column count so incomplete rows center */
@media (max-width: 1023px) {
  .news-topics__card { flex-basis: calc((100% - clamp(22px, 2.6vw, 32px)) / 2); }
}

@media (max-width: 639px) {
  .news-topics__card { flex-basis: 100%; }
}
</style>
