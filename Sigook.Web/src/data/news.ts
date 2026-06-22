/**
 * Mock newsroom data — drives the /v2/news landing experience.
 *
 * In production this will be fetched from a CMS; for now the page is fully
 * static. Keep the shape stable so the eventual API client can return the
 * same types.
 *
 * All copy is in English (CLAUDE.md mandate). Images use Unsplash hot-link
 * URLs sized for editorial cards (~1200px wide, q=80, fit=crop) — they're
 * placeholders until the marketing team supplies branded photography.
 */

export type NewsCategoryKey =
  | 'industry-insights'
  | 'company-news'
  | 'press-releases'
  | 'hiring-trends'
  | 'compliance'

export interface NewsCategory {
  readonly key: NewsCategoryKey | 'all'
  readonly label: string
}

export interface NewsArticle {
  readonly id: string
  readonly slug: string
  readonly category: NewsCategoryKey
  readonly title: string
  readonly excerpt: string
  readonly imageUrl: string
  readonly imageAlt: string
  /** ISO-8601 date string (YYYY-MM-DD). */
  readonly publishedAt: string
  readonly author: string
  readonly readTimeMinutes: number
  /** When true, the article is eligible for the Featured carousel. */
  readonly featured?: boolean
}

/** Filter chips rendered in the hero — keep ordered intentionally. */
export const NEWS_CATEGORIES: readonly NewsCategory[] = [
  { key: 'all',               label: 'All stories' },
  { key: 'industry-insights', label: 'Industry insights' },
  { key: 'company-news',      label: 'Company news' },
  { key: 'press-releases',    label: 'Press releases' },
  { key: 'hiring-trends',     label: 'Hiring trends' },
  { key: 'compliance',        label: 'Compliance' },
] as const

/** Map category key → human label (used by cards / chips). */
export const CATEGORY_LABEL: Readonly<Record<NewsCategoryKey, string>> = {
  'industry-insights': 'Industry insights',
  'company-news':      'Company news',
  'press-releases':    'Press releases',
  'hiring-trends':     'Hiring trends',
  'compliance':        'Compliance',
}

/**
 * Staffing & Recruitment News — U.S. Market Roundup (June 2026).
 * Five stories — 3 flagged `featured: true` for the carousel.
 * Order matters: the LatestGrid renders these in array order, so newer
 * stories sit at the top of the data file.
 */
export const NEWS_ARTICLES: readonly NewsArticle[] = [
  {
    id: 'ai-embedded-staffing',
    slug: 'ai-embedded-in-staffing-skill-based-hiring',
    category: 'industry-insights',
    title: 'AI becomes embedded in the staffing workflow as skill-based hiring takes hold',
    excerpt:
      'AI is now standard infrastructure in recruiting — sourcing, screening, and candidate comms — even as hiring tilts toward skills over credentials. The edge goes to firms that pair automation with human judgment.',
    imageUrl:
      'https://images.unsplash.com/photo-1677442136019-21780ecad995?w=1200&q=80&auto=format&fit=crop',
    imageAlt: 'Abstract 3D \'AI\' lettering over a digital network field',
    publishedAt: '2026-06-16',
    author: 'Sigook',
    readTimeMinutes: 8,
    featured: true,
  },
  {
    id: 'industrial-staffing-outperforms',
    slug: 'industrial-staffing-outperforms-forecasts',
    category: 'industry-insights',
    title: 'Industrial staffing outperforms forecasts as recruiting pressures ease',
    excerpt:
      'Logistics and warehousing keep getting easier to staff, with time-to-fill falling since late 2025. Manufacturing runs the other way — rising openings, flat hiring — making the case for a vertical-specific strategy.',
    imageUrl:
      'https://images.unsplash.com/photo-1565793298595-6a879b1d9492?w=1200&q=80&auto=format&fit=crop',
    imageAlt: 'Aerial view of freight trucks lined up at a logistics distribution yard',
    publishedAt: '2026-06-12',
    author: 'Sigook',
    readTimeMinutes: 7,
  },
  {
    id: 'manufacturing-hiring-momentum',
    slug: 'manufacturing-hiring-regains-momentum',
    category: 'industry-insights',
    title: 'Manufacturing hiring regains momentum',
    excerpt:
      'Manufacturing added 7,000 jobs in May, reversing April\'s stall, with nine of 18 subsectors growing. Plants increasingly run blended crews — a permanent core plus on-demand skilled trades from staffing firms.',
    imageUrl:
      'https://images.unsplash.com/photo-1504328345606-18bbc8c9d7d1?w=1200&q=80&auto=format&fit=crop',
    imageAlt: 'Welder working with sparks on a metal fabrication shop floor',
    publishedAt: '2026-06-08',
    author: 'Sigook',
    readTimeMinutes: 6,
    featured: true,
  },
  {
    id: 'may-jobs-report-2026',
    slug: 'may-2026-us-jobs-report',
    category: 'hiring-trends',
    title: 'May jobs report signals cautious but steady U.S. hiring',
    excerpt:
      'Payrolls rose 172,000 in May and unemployment held at 4.3%. The tell for staffing: temporary help kept expanding — the pattern that shows up when employers favor flexible labor over permanent hires.',
    imageUrl:
      'https://images.unsplash.com/photo-1454165804606-c3d57bc86b40?w=1200&q=80&auto=format&fit=crop',
    imageAlt: 'Two colleagues reviewing labor-market figures at a desk',
    publishedAt: '2026-06-05',
    author: 'Sigook',
    readTimeMinutes: 6,
    featured: true,
  },
  {
    id: 'asa-staffing-index-2026',
    slug: 'asa-staffing-index-near-two-year-highs',
    category: 'hiring-trends',
    title: 'ASA Staffing Index holds near two-year highs',
    excerpt:
      'Temporary and contract staffing ran 4.6% above 2025, holding the index near levels last seen in 2024. A near real-time read on labor demand points to stabilization — a welcome story after a hard 2025.',
    imageUrl:
      'https://images.unsplash.com/photo-1551288049-bebda4e38f71?w=1200&q=80&auto=format&fit=crop',
    imageAlt: 'Analytics dashboard showing charts and performance metrics',
    publishedAt: '2026-05-27',
    author: 'Sigook',
    readTimeMinutes: 5,
  },
] as const

/**
 * Helpers — pure functions kept next to the data so the components can stay
 * declarative.
 */

/** Featured articles for the hero carousel — preserves source order. */
export function getFeaturedArticles(): readonly NewsArticle[] {
  return NEWS_ARTICLES.filter((a) => a.featured)
}

/** Latest N articles for the grid (newest first; includes featured). */
export function getLatestArticles(limit = 6): readonly NewsArticle[] {
  return NEWS_ARTICLES.slice(0, limit)
}

/** Counts per category — used by Topics cards to show story counts. */
export function getCategoryCounts(): Readonly<Record<NewsCategoryKey, number>> {
  const counts: Record<NewsCategoryKey, number> = {
    'industry-insights': 0,
    'company-news':      0,
    'press-releases':    0,
    'hiring-trends':     0,
    'compliance':        0,
  }
  for (const article of NEWS_ARTICLES) counts[article.category]++
  return counts
}

/** Format an ISO date as "May 18, 2026" without pulling a date library. */
export function formatPublishedDate(iso: string): string {
  const date = new Date(`${iso}T00:00:00Z`)
  return date.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    timeZone: 'UTC',
  })
}
