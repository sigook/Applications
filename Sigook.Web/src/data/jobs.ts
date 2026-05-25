/**
 * Mock job-board data — drives the /v2/open-positions landing experience.
 *
 * In production this will be fetched from the backend job-board API; for
 * now the page is fully static. Keep the shape stable so the eventual API
 * client can return the same types.
 *
 * All copy is in English (CLAUDE.md mandate).
 */

/**
 * Industry icon names — kept in sync with IndustryIcon.vue's IndustryIconName
 * export. Declared locally because TS can't resolve named type exports from
 * .vue files in plain .ts modules.
 */
type IndustryIconName =
  | 'automotive'
  | 'aviation'
  | 'construction'
  | 'engineering'
  | 'technology'
  | 'finance'
  | 'legal'
  | 'logistics'
  | 'manufacturing'
  | 'retail'
  | 'transportation'

/** Employment relationship the role is offered under. */
export type JobType = 'full-time' | 'contract' | 'temp' | 'part-time'

/** Broad track — used by the hero filter chips. */
export type JobTrack = 'professional' | 'industrial'

export interface JobLocation {
  readonly city: string
  readonly province: string
  /** 'CA' or 'US' for the flag in the search filters. */
  readonly country: 'CA' | 'US'
}

export interface JobSalary {
  readonly min: number
  readonly max: number
  /** Display unit — drives the "$X / hr" or "$X / yr" rendering. */
  readonly unit: 'hour' | 'year'
}

export interface Job {
  readonly id: string
  /** Human-readable job number shown in the list (e.g., "(2110)"). */
  readonly jobNumber: string
  readonly slug: string
  readonly title: string
  readonly type: JobType
  readonly track: JobTrack
  readonly industry: IndustryIconName
  readonly location: JobLocation
  readonly salary: JobSalary
  readonly schedule: readonly string[]
  readonly responsibilities: readonly string[]
  readonly requirements: readonly string[]
  /** ISO date string (YYYY-MM-DD). */
  readonly postedAt: string
}

/* ── Pickers used by the search form ────────────────────────────────────── */

export const JOB_TYPE_OPTIONS: ReadonlyArray<{ value: JobType; label: string }> = [
  { value: 'full-time', label: 'Full-time' },
  { value: 'contract',  label: 'Contract'  },
  { value: 'part-time', label: 'Part-time' },
  { value: 'temp',      label: 'Temporary' },
] as const

export const JOB_COUNTRY_OPTIONS: ReadonlyArray<{ value: 'CA' | 'US'; label: string; flag: string }> = [
  { value: 'CA', label: 'Canada',        flag: '🇨🇦' },
  { value: 'US', label: 'United States', flag: '🇺🇸' },
] as const

/* ── Filter chips on the list section ───────────────────────────────────── */

export interface JobTrackFilter {
  readonly key: JobTrack | 'all'
  readonly label: string
}

export const JOB_TRACK_FILTERS: readonly JobTrackFilter[] = [
  { key: 'all',          label: 'All roles' },
  { key: 'professional', label: 'Professional' },
  { key: 'industrial',   label: 'Industrial' },
] as const

/* ── Mock job listings ──────────────────────────────────────────────────── */

export const JOBS: readonly Job[] = [
  {
    id: 'csr-medley-1',
    jobNumber: '(2110)',
    slug: 'customer-service-representative-medley',
    title: 'Customer Service Representative',
    type: 'full-time',
    track: 'professional',
    industry: 'retail',
    location: { city: 'Medley', province: 'FL', country: 'US' },
    salary: { min: 40700, max: 40700, unit: 'year' },
    schedule: [
      'Monday to Friday, 8:00 AM – 5:00 PM',
      'Hybrid after a 90-day onboarding period',
    ],
    responsibilities: [
      'Answer inbound calls, emails, and chat from B2B clients',
      'Coordinate with operations to track and resolve orders',
      'Maintain CRM records and escalate complex issues',
    ],
    requirements: [
      '2+ years in B2B customer service',
      'Strong written and verbal communication',
      'Comfortable with Salesforce or similar CRM',
    ],
    postedAt: '2026-05-20',
  },
  {
    id: 'az-driver-toronto',
    jobNumber: '(1466)',
    slug: 'az-dump-truck-float-trailer-driver',
    title: 'AZ Dump Truck / Float Trailer Driver',
    type: 'full-time',
    track: 'industrial',
    industry: 'transportation',
    location: { city: 'Caledon', province: 'ON', country: 'CA' },
    salary: { min: 40, max: 40, unit: 'hour' },
    schedule: [
      'Monday to Friday, early morning starts',
      'Occasional Saturday overtime available',
    ],
    responsibilities: [
      'Operate AZ dump truck and float trailer combinations',
      'Pre- and post-trip inspections with CVOR-compliant logs',
      'Load and secure equipment to construction sites',
    ],
    requirements: [
      'Valid AZ license with clean abstract',
      '3+ years dump or float experience',
      'Comfortable with manual transmission and Q-decking',
    ],
    postedAt: '2026-05-18',
  },
  {
    id: 'dz-driver-fuel-delivery',
    jobNumber: '(3110)',
    slug: 'dz-driver-fuel-delivery',
    title: 'DZ Driver – Fuel Delivery',
    type: 'full-time',
    track: 'industrial',
    industry: 'transportation',
    location: { city: 'Toronto', province: 'ON', country: 'CA' },
    salary: { min: 24, max: 24, unit: 'hour' },
    schedule: [
      'Primarily daytime shifts',
      'Operation runs 24/7 — must be available for weekend and evening shifts as required',
      'Tons of overtime offered (nights and weekends)',
    ],
    responsibilities: [
      'Delivering wholesale petroleum services in the GTA area',
      'Daily vehicle inspection — complete necessary driver\'s paperwork',
      'Installing and de-installing tanks and pumps',
      'Delivering at construction sites and gas stations',
      'Training in the fuel trucks is provided',
    ],
    requirements: [
      'At least 2 years of experience',
      'Clean Abstract and CVOR',
      'Manual transmission 6, 8 and 10 speed trucks',
      'Fuel delivery knowledge about the different fuels, fuel delivery, fuel rules, the applications of fuel, loading at terminals, etc.',
      'Must be able to work under minimal supervision',
      'Must be reliable in maintaining a good customer service relationship',
      'Must be comfortable with onboard computer technology',
      'Flexible to extended workdays, nights, and weekends when needed if available',
    ],
    postedAt: '2026-05-16',
  },
  {
    id: 'estimator-account-mgr',
    jobNumber: '(2820)',
    slug: 'estimator-account-manager',
    title: 'Estimator and Account Manager',
    type: 'full-time',
    track: 'professional',
    industry: 'construction',
    location: { city: 'Miami', province: 'FL', country: 'US' },
    salary: { min: 110000, max: 110000, unit: 'year' },
    schedule: ['Monday to Friday, 8:00 AM – 6:00 PM'],
    responsibilities: [
      'Prepare detailed cost estimates for commercial projects',
      'Own client relationships from quote to project close',
      'Coordinate with project managers and field supervisors',
    ],
    requirements: [
      '5+ years estimating in commercial construction',
      'Proficient with Bluebeam, PlanSwift, or similar',
      'Bilingual English/Spanish preferred',
    ],
    postedAt: '2026-05-14',
  },
  {
    id: 'forklift-wday-am',
    jobNumber: '(1672)',
    slug: 'forklift-operators-wday-am',
    title: 'Forklift Operators – WDAY AM Morning',
    type: 'full-time',
    track: 'industrial',
    industry: 'logistics',
    location: { city: 'Hialeah', province: 'FL', country: 'US' },
    salary: { min: 19, max: 19, unit: 'hour' },
    schedule: [
      'Weekdays 6:00 AM – 2:30 PM',
      'Overtime available on demand',
    ],
    responsibilities: [
      'Operate sit-down and stand-up forklifts in a refrigerated facility',
      'Load and unload trailers, stage pallets for outbound shipments',
      'Maintain cleanliness and safety per OSHA guidelines',
    ],
    requirements: [
      'Valid forklift certification',
      '1+ year warehouse experience',
      'Comfortable in cold storage environments',
    ],
    postedAt: '2026-05-12',
  },
  {
    id: 'general-labor-london',
    jobNumber: '(2110)',
    slug: 'general-labor-london',
    title: 'General Labor',
    type: 'temp',
    track: 'industrial',
    industry: 'manufacturing',
    location: { city: 'London', province: 'ON', country: 'CA' },
    salary: { min: 20, max: 20, unit: 'hour' },
    schedule: [
      'Rotating shifts (mornings, afternoons, nights)',
      '40 hours per week, overtime available',
    ],
    responsibilities: [
      'Assist on production lines and packaging stations',
      'Quality checks and basic equipment cleaning',
      'Move materials between stations safely',
    ],
    requirements: [
      'Reliable transportation',
      'Steel-toed boots required (bring your own)',
      'Comfortable with repetitive lifting up to 25 kg',
    ],
    postedAt: '2026-05-10',
  },
  {
    id: 'csr-medley-2',
    jobNumber: '(2330)',
    slug: 'customer-service-representative-medley-2',
    title: 'Customer Service Representative',
    type: 'full-time',
    track: 'professional',
    industry: 'retail',
    location: { city: 'Medley', province: 'FL', country: 'US' },
    salary: { min: 38000, max: 38000, unit: 'year' },
    schedule: ['Monday to Friday, 9:00 AM – 6:00 PM'],
    responsibilities: [
      'Handle incoming product enquiries and orders',
      'Process returns and resolve billing questions',
      'Coordinate with warehouse for delivery updates',
    ],
    requirements: [
      '1+ year customer service experience',
      'High school diploma or equivalent',
      'Bilingual English/Spanish a plus',
    ],
    postedAt: '2026-05-08',
  },
  {
    id: 'heavy-truck-mechanic',
    jobNumber: '(3081)',
    slug: 'heavy-machinery-truck-mechanic',
    title: 'Heavy Machinery Truck Mechanic',
    type: 'full-time',
    track: 'industrial',
    industry: 'transportation',
    location: { city: 'Concord', province: 'ON', country: 'CA' },
    salary: { min: 35, max: 35, unit: 'hour' },
    schedule: [
      'Monday to Friday, 7:00 AM – 4:00 PM',
      'On-call rotation one weekend per month',
    ],
    responsibilities: [
      'Diagnose and repair heavy-duty trucks and trailers',
      'Preventive maintenance and DVI inspections',
      'Document repairs in fleet maintenance system',
    ],
    requirements: [
      '310T license (or equivalent)',
      '5+ years experience with diesel engines and air brakes',
      'Own tools required',
    ],
    postedAt: '2026-05-06',
  },
  {
    id: 'lab-technician',
    jobNumber: '(1620)',
    slug: 'lab-technician-palm',
    title: 'Lab Technician',
    type: 'contract',
    track: 'professional',
    industry: 'engineering',
    location: { city: 'Palm', province: 'ON', country: 'CA' },
    salary: { min: 22, max: 22, unit: 'hour' },
    schedule: [
      '6-month contract with extension potential',
      'Monday to Friday, 8:30 AM – 4:30 PM',
    ],
    responsibilities: [
      'Run routine quality-control tests on incoming materials',
      'Maintain calibration logs and equipment',
      'Report findings to senior chemists',
    ],
    requirements: [
      'Diploma in chemistry or related field',
      '1+ year lab experience',
      'Familiar with HPLC and GC instruments',
    ],
    postedAt: '2026-05-04',
  },
  {
    id: 'manufacturing-equipment-op',
    jobNumber: '(2040)',
    slug: 'manufacturing-equipment-operator',
    title: 'Manufacturing Equipment Operator',
    type: 'full-time',
    track: 'industrial',
    industry: 'manufacturing',
    location: { city: 'Miami', province: 'FL', country: 'US' },
    salary: { min: 22, max: 22, unit: 'hour' },
    schedule: [
      'Rotating 12-hour shifts (2 on, 2 off, 3 on)',
      'Day shift premium available',
    ],
    responsibilities: [
      'Set up and operate CNC and stamping equipment',
      'Read blueprints and run quality checks per spec',
      'Perform first-line maintenance and changeovers',
    ],
    requirements: [
      '2+ years operating production equipment',
      'Comfortable reading micrometers and calipers',
      'Steel-toed boots and safety glasses required',
    ],
    postedAt: '2026-05-02',
  },
  {
    id: 'manufacturing-support-forklift',
    jobNumber: '(3170)',
    slug: 'manufacturing-support-associate-forklift',
    title: 'Manufacturing Support Associate / Forklift',
    type: 'temp',
    track: 'industrial',
    industry: 'manufacturing',
    location: { city: 'Miami', province: 'FL', country: 'US' },
    salary: { min: 17, max: 17, unit: 'hour' },
    schedule: ['Monday to Friday, day shift'],
    responsibilities: [
      'Move raw materials between staging and production',
      'Stack finished goods and prep for shipping',
      'Cycle counts and basic inventory checks',
    ],
    requirements: [
      'Current forklift certification (sit-down)',
      'Ability to lift 30 kg consistently',
      'Reliable attendance',
    ],
    postedAt: '2026-04-30',
  },
  {
    id: 'process-inventory-specialist',
    jobNumber: '(2380)',
    slug: 'process-inventory-specialist',
    title: 'Process Inventory Specialist',
    type: 'full-time',
    track: 'professional',
    industry: 'logistics',
    location: { city: 'Medley', province: 'FL', country: 'US' },
    salary: { min: 45700, max: 45700, unit: 'year' },
    schedule: ['Monday to Friday, 8:00 AM – 5:00 PM'],
    responsibilities: [
      'Audit inventory cycles and reconcile WMS data',
      'Lead annual physical inventory and cycle counts',
      'Build dashboards for ops leadership',
    ],
    requirements: [
      'Bachelor\'s in supply chain or related',
      'Advanced Excel + experience with Power BI or Tableau',
      'WMS experience (Manhattan, SAP, or similar)',
    ],
    postedAt: '2026-04-28',
  },
  {
    id: 'qa-aviation-specialist',
    jobNumber: '(2901)',
    slug: 'qa-aviation-specialist',
    title: 'QA Aviation Specialist',
    type: 'full-time',
    track: 'professional',
    industry: 'aviation',
    location: { city: 'Mississauga', province: 'ON', country: 'CA' },
    salary: { min: 78000, max: 95000, unit: 'year' },
    schedule: ['Monday to Friday, 8:00 AM – 5:00 PM'],
    responsibilities: [
      'Audit MRO operations against Transport Canada standards',
      'Lead root-cause analysis on non-conformances',
      'Maintain quality documentation per AS9100',
    ],
    requirements: [
      'AME license or equivalent training',
      '4+ years aviation quality assurance',
      'AS9100 / Transport Canada audit experience',
    ],
    postedAt: '2026-04-26',
  },
  {
    id: 'legal-paralegal',
    jobNumber: '(1830)',
    slug: 'corporate-paralegal-toronto',
    title: 'Corporate Paralegal',
    type: 'full-time',
    track: 'professional',
    industry: 'legal',
    location: { city: 'Toronto', province: 'ON', country: 'CA' },
    salary: { min: 65000, max: 78000, unit: 'year' },
    schedule: ['Monday to Friday, hybrid 3 days in office'],
    responsibilities: [
      'Draft and review corporate documents and minute books',
      'Manage corporate filings and annual returns',
      'Support M&A due-diligence research',
    ],
    requirements: [
      'Law Clerk diploma or paralegal license',
      '3+ years corporate experience',
      'Strong attention to detail',
    ],
    postedAt: '2026-04-24',
  },
  {
    id: 'finance-analyst',
    jobNumber: '(2270)',
    slug: 'financial-analyst-toronto',
    title: 'Financial Analyst',
    type: 'full-time',
    track: 'professional',
    industry: 'finance',
    location: { city: 'Toronto', province: 'ON', country: 'CA' },
    salary: { min: 72000, max: 88000, unit: 'year' },
    schedule: ['Monday to Friday, hybrid'],
    responsibilities: [
      'Build monthly variance reports and rolling forecasts',
      'Support budgeting cycles for ops and sales',
      'Model scenarios for capital investment decisions',
    ],
    requirements: [
      'CPA or in progress preferred',
      '3+ years FP&A experience',
      'Advanced Excel + experience with NetSuite or SAP',
    ],
    postedAt: '2026-04-22',
  },
] as const

/* ── Helpers ────────────────────────────────────────────────────────────── */

/** Format salary for the list / detail header (e.g., "$24.00 / hr", "$40,700 / yr"). */
export function formatSalary(salary: JobSalary): string {
  const unit = salary.unit === 'hour' ? '/ hr' : '/ yr'
  if (salary.min === salary.max) {
    return `$${formatNumber(salary.min, salary.unit)} ${unit}`
  }
  return `$${formatNumber(salary.min, salary.unit)} – $${formatNumber(salary.max, salary.unit)} ${unit}`
}

function formatNumber(n: number, unit: 'hour' | 'year'): string {
  if (unit === 'hour') return n.toFixed(2)
  return n.toLocaleString('en-US')
}

/** Format a location for the list (e.g., "Toronto, ON"). */
export function formatLocation(loc: JobLocation): string {
  return `${loc.city}, ${loc.province}`
}

/** Filter the list by track + free-text title query. */
export function filterJobs(
  jobs: readonly Job[],
  track: JobTrack | 'all',
  query: string,
): readonly Job[] {
  const q = query.trim().toLowerCase()
  return jobs.filter((job) => {
    if (track !== 'all' && job.track !== track) return false
    if (q && !job.title.toLowerCase().includes(q)) return false
    return true
  })
}
