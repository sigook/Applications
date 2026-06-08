/**
 * Mock newsroom data — drives the /v2/news landing experience.
 *
 * In production this will be fetched from a CMS; for now the page is fully
 * static. Keep the shape stable so the eventual API client can return the
 * same types.
 *
 * All copy is in English (CLAUDE.md mandate). Images use Unsplash hot-link
 * URLs sized for editorial cards (~800px wide, q=80, fit=crop) — they're
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
 * 10 mock articles — 3 flagged `featured: true` for the carousel.
 * Order matters: the LatestGrid renders these in array order, so newer
 * stories sit at the top of the data file.
 */
export const NEWS_ARTICLES: readonly NewsArticle[] = [
  {
    id: 'osha-2026-compliance',
    slug: 'osha-2026-compliance-guide',
    category: 'compliance',
    title: 'OSHA 2026 updates: a practical compliance checklist for employers',
    excerpt:
      'The new federal safety standards take effect Q3. Here\'s what changes for industrial workplaces — and the three audit gaps we keep finding.',
    imageUrl:
      'https://images.unsplash.com/photo-1581094288338-2314dddb7ece?w=1200&q=80&auto=format&fit=crop',
    imageAlt: 'Construction workers reviewing a safety checklist on site',
    publishedAt: '2026-05-18',
    author: 'Maria Chen',
    readTimeMinutes: 6,
    featured: true,
  },
  {
    id: 'q1-hiring-pulse-2026',
    slug: 'q1-2026-us-hiring-pulse',
    category: 'hiring-trends',
    title: 'Q1 2026 U.S. hiring pulse: where demand actually is',
    excerpt:
      'We pulled 1,400 placements across our network. The headline: contract hiring grew 22% YoY, with manufacturing and logistics leading.',
    imageUrl:
      'https://images.unsplash.com/photo-1551836022-d5d88e9218df?w=1200&q=80&auto=format&fit=crop',
    imageAlt: 'Modern warehouse floor with workers and forklift',
    publishedAt: '2026-05-12',
    author: 'David Park',
    readTimeMinutes: 8,
    featured: true,
  },
  {
    id: 'sigook-houston-office',
    slug: 'sigook-opens-houston-office',
    category: 'company-news',
    title: 'Sigook opens a Houston office — bilingual recruitment now in-region',
    excerpt:
      'Our newest branch brings bilingual recruitment to clients across the Gulf Coast region. Meet the team and the roles we\'re staffing first.',
    imageUrl:
      'https://images.unsplash.com/photo-1497366216548-37526070297c?w=1200&q=80&auto=format&fit=crop',
    imageAlt: 'Bright modern office interior with team working at desks',
    publishedAt: '2026-05-04',
    author: 'Anaïs Tremblay',
    readTimeMinutes: 4,
    featured: true,
  },
  {
    id: 'minimum-wage-florida',
    slug: 'florida-minimum-wage-2026',
    category: 'compliance',
    title: 'Florida minimum wage rises September 30 — what it means for contract rates',
    excerpt:
      'The state confirmed a $1.00 increase. We break down which contract billing models absorb it and which need renegotiation now.',
    imageUrl:
      'https://images.unsplash.com/photo-1454165804606-c3d57bc86b40?w=1200&q=80&auto=format&fit=crop',
    imageAlt: 'Person signing a payroll document at a desk',
    publishedAt: '2026-04-28',
    author: 'Jordan Lee',
    readTimeMinutes: 5,
  },
  {
    id: 'manufacturing-talent-gap',
    slug: 'closing-the-manufacturing-talent-gap',
    category: 'industry-insights',
    title: 'Closing the manufacturing talent gap: three things that actually work',
    excerpt:
      'Apprenticeship pipelines, internal mobility, and one underrated retention play we\'ve seen move the needle on U.S. shop floors.',
    imageUrl:
      'https://images.unsplash.com/photo-1565793298595-6a879b1d9492?w=1200&q=80&auto=format&fit=crop',
    imageAlt: 'Manufacturing worker operating industrial machinery',
    publishedAt: '2026-04-20',
    author: 'Priya Sharma',
    readTimeMinutes: 7,
  },
  {
    id: 'remote-vs-onsite-2026',
    slug: 'remote-vs-onsite-the-2026-reality',
    category: 'hiring-trends',
    title: 'Remote vs. onsite: the 2026 reality in skilled trades and offices',
    excerpt:
      'Hybrid is the new default for professional roles. For industrial roles, "remote" is reshaping logistics and dispatch. The numbers, by sector.',
    imageUrl:
      'https://images.unsplash.com/photo-1521737711867-e3b97375f902?w=1200&q=80&auto=format&fit=crop',
    imageAlt: 'Team in a hybrid meeting with remote attendees on screen',
    publishedAt: '2026-04-14',
    author: 'Sarah Kowalski',
    readTimeMinutes: 6,
  },
  {
    id: 'sigook-iso-9001',
    slug: 'sigook-achieves-iso-9001-certification',
    category: 'press-releases',
    title: 'Sigook achieves ISO 9001:2015 certification across all U.S. offices',
    excerpt:
      'Independent audit confirms our quality management standards meet the international benchmark — full statement and downloadable certificate inside.',
    imageUrl:
      'https://images.unsplash.com/photo-1521791136064-7986c2920216?w=1200&q=80&auto=format&fit=crop',
    imageAlt: 'Two professionals shaking hands over a certification document',
    publishedAt: '2026-04-08',
    author: 'Press Office',
    readTimeMinutes: 3,
  },
  {
    id: 'aviation-mro-shortage',
    slug: 'aviation-mro-shortage-deepens-2026',
    category: 'industry-insights',
    title: 'Aviation MRO shortage deepens — why airlines are turning to staffing',
    excerpt:
      'With certified mechanics retiring faster than schools can graduate them, contract staffing is keeping fleets in the air. The economics, explained.',
    imageUrl:
      'https://images.unsplash.com/photo-1556388158-158ea5ccacbd?w=1200&q=80&auto=format&fit=crop',
    imageAlt: 'Aviation mechanic working on an aircraft engine',
    publishedAt: '2026-03-30',
    author: 'David Park',
    readTimeMinutes: 9,
  },
  {
    id: 'diversity-hiring-playbook',
    slug: 'diversity-hiring-playbook-2026',
    category: 'hiring-trends',
    title: 'The diversity hiring playbook our clients are running in 2026',
    excerpt:
      'Beyond statements: structured intake, blind résumé review, and panel diversity. The plays that survived a year of real-world testing.',
    imageUrl:
      'https://images.unsplash.com/photo-1556761175-5973dc0f32e7?w=1200&q=80&auto=format&fit=crop',
    imageAlt: 'Diverse team collaborating around a meeting table',
    publishedAt: '2026-03-24',
    author: 'Maria Chen',
    readTimeMinutes: 8,
  },
  {
    id: 'sigook-partner-program',
    slug: 'introducing-the-sigook-partner-program',
    category: 'company-news',
    title: 'Introducing the Sigook Partner Program for boutique recruiters',
    excerpt:
      'Independent recruiters can now plug into our compliance, payroll, and tech stack while keeping their book of business. Applications open.',
    imageUrl:
      'https://images.unsplash.com/photo-1600880292203-757bb62b4baf?w=1200&q=80&auto=format&fit=crop',
    imageAlt: 'Business professionals in conversation at a modern office',
    publishedAt: '2026-03-15',
    author: 'Anaïs Tremblay',
    readTimeMinutes: 4,
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

/** Latest N non-featured articles for the grid. */
export function getLatestArticles(limit = 6): readonly NewsArticle[] {
  return NEWS_ARTICLES.filter((a) => !a.featured).slice(0, limit)
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
