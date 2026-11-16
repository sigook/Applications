import { onUnmounted, watch } from 'vue';
import { useRoute } from 'vue-router';
import type { JobViewModel } from '@/modules/landing/types';

const SITE_ORIGIN = 'https://www.sigook.com';
const SCRIPT_ID = 'job-posting-ld';
const HOURLY_RATE_LIMIT = 200;
const META_DESCRIPTION_LENGTH = 155;

type Country = 'CA' | 'US';

interface Region {
  code: string;
  country: Country;
}

const CA_REGIONS: Record<string, string> = {
  'ALBERTA': 'AB',
  'BRITISH COLUMBIA': 'BC',
  'MANITOBA': 'MB',
  'NEW BRUNSWICK': 'NB',
  'NEWFOUNDLAND AND LABRADOR': 'NL',
  'NOVA SCOTIA': 'NS',
  'NORTHWEST TERRITORIES': 'NT',
  'NUNAVUT': 'NU',
  'ONTARIO': 'ON',
  'PRINCE EDWARD ISLAND': 'PE',
  'QUEBEC': 'QC',
  'SASKATCHEWAN': 'SK',
  'YUKON': 'YT',
};

const US_REGIONS: Record<string, string> = {
  'ALABAMA': 'AL', 'ALASKA': 'AK', 'ARIZONA': 'AZ', 'ARKANSAS': 'AR', 'CALIFORNIA': 'CA',
  'COLORADO': 'CO', 'CONNECTICUT': 'CT', 'DELAWARE': 'DE', 'DISTRICT OF COLUMBIA': 'DC',
  'FLORIDA': 'FL', 'GEORGIA': 'GA', 'HAWAII': 'HI', 'IDAHO': 'ID', 'ILLINOIS': 'IL',
  'INDIANA': 'IN', 'IOWA': 'IA', 'KANSAS': 'KS', 'KENTUCKY': 'KY', 'LOUISIANA': 'LA',
  'MAINE': 'ME', 'MARYLAND': 'MD', 'MASSACHUSETTS': 'MA', 'MICHIGAN': 'MI', 'MINNESOTA': 'MN',
  'MISSISSIPPI': 'MS', 'MISSOURI': 'MO', 'MONTANA': 'MT', 'NEBRASKA': 'NE', 'NEVADA': 'NV',
  'NEW HAMPSHIRE': 'NH', 'NEW JERSEY': 'NJ', 'NEW MEXICO': 'NM', 'NEW YORK': 'NY',
  'NORTH CAROLINA': 'NC', 'NORTH DAKOTA': 'ND', 'OHIO': 'OH', 'OKLAHOMA': 'OK', 'OREGON': 'OR',
  'PENNSYLVANIA': 'PA', 'RHODE ISLAND': 'RI', 'SOUTH CAROLINA': 'SC', 'SOUTH DAKOTA': 'SD',
  'TENNESSEE': 'TN', 'TEXAS': 'TX', 'UTAH': 'UT', 'VERMONT': 'VT', 'VIRGINIA': 'VA',
  'WASHINGTON': 'WA', 'WEST VIRGINIA': 'WV', 'WISCONSIN': 'WI', 'WYOMING': 'WY',
};

const REGIONS: [string, Region][] = [
  ...Object.entries(CA_REGIONS).map(([name, code]): [string, Region] => [name, { code, country: 'CA' }]),
  ...Object.entries(US_REGIONS).map(([name, code]): [string, Region] => [name, { code, country: 'US' }]),
].sort(([a], [b]) => b.length - a.length);

const EMPLOYMENT_TYPES: Record<string, string> = {
  'full time': 'FULL_TIME',
  'part time': 'PART_TIME',
  'temporary': 'TEMPORARY',
  'contractor': 'CONTRACTOR',
};

export interface JobLocation {
  locality: string;
  region: string | null;
  country: Country | null;
}

export interface JobPostingSchema {
  '@context': 'https://schema.org';
  '@type': 'JobPosting';
  title: string;
  description: string;
  datePosted: string;
  url: string;
  directApply: boolean;
  identifier: { '@type': 'PropertyValue'; name: string; value: string };
  hiringOrganization: { '@type': 'Organization'; name: string; sameAs: string; logo: string };
  jobLocation: {
    '@type': 'Place';
    address: {
      '@type': 'PostalAddress';
      addressLocality: string;
      addressRegion?: string;
      addressCountry?: Country;
    };
  };
  employmentType?: string;
  baseSalary?: {
    '@type': 'MonetaryAmount';
    currency: 'CAD' | 'USD';
    value: { '@type': 'QuantitativeValue'; value: number; unitText: 'HOUR' | 'YEAR' };
  };
}

function toTitleCase(value: string): string {
  return value.toLowerCase().replace(/\b\p{L}/gu, (c) => c.toUpperCase());
}

export function parseJobLocation(location: string): JobLocation {
  const normalized = location.trim().replace(/[\s,]+/g, ' ').toUpperCase();
  const match = REGIONS.find(([name]) => normalized === name || normalized.endsWith(` ${name}`));
  if (!match) return { locality: toTitleCase(normalized), region: null, country: null };
  const [name, region] = match;
  const locality = normalized.slice(0, normalized.length - name.length).trim();
  return { locality: toTitleCase(locality || name), region: region.code, country: region.country };
}

export function parseSalaryAmount(salary: string): number | null {
  const amount = Number(salary.replace(/[^0-9.]/g, ''));
  return Number.isFinite(amount) && amount > 1 ? amount : null;
}

function joinDescription(job: JobViewModel): string {
  const sections = [
    job.description,
    job.responsibilities ? `<h3>Responsibilities</h3>${job.responsibilities}` : '',
    job.requirements ? `<h3>Requirements</h3>${job.requirements}` : '',
    job.shift ? `<p>Shift: ${job.shift}</p>` : '',
  ];
  return sections.filter(Boolean).join('') || `<p>${job.title.trim()}</p>`;
}

export function jobPostingUrl(numberId: string): string {
  return `${SITE_ORIGIN}/open-positions/${encodeURIComponent(numberId)}`;
}

export function buildJobPosting(job: JobViewModel): JobPostingSchema {
  const location = parseJobLocation(job.location);
  const schema: JobPostingSchema = {
    '@context': 'https://schema.org',
    '@type': 'JobPosting',
    title: job.title.trim(),
    description: joinDescription(job),
    datePosted: job.createdAt.slice(0, 10),
    url: jobPostingUrl(job.numberId),
    directApply: true,
    identifier: { '@type': 'PropertyValue', name: 'Sigook', value: job.numberId },
    hiringOrganization: {
      '@type': 'Organization',
      name: 'Sigook',
      sameAs: `${SITE_ORIGIN}/`,
      logo: `${SITE_ORIGIN}/images/logo.png`,
    },
    jobLocation: {
      '@type': 'Place',
      address: {
        '@type': 'PostalAddress',
        addressLocality: location.locality,
        ...(location.region ? { addressRegion: location.region } : {}),
        ...(location.country ? { addressCountry: location.country } : {}),
      },
    },
  };

  const employmentType = EMPLOYMENT_TYPES[job.type.trim().toLowerCase()];
  if (employmentType) schema.employmentType = employmentType;

  const amount = parseSalaryAmount(job.salary);
  if (amount !== null && location.country) {
    schema.baseSalary = {
      '@type': 'MonetaryAmount',
      currency: location.country === 'CA' ? 'CAD' : 'USD',
      value: { '@type': 'QuantitativeValue', value: amount, unitText: amount < HOURLY_RATE_LIMIT ? 'HOUR' : 'YEAR' },
    };
  }

  return schema;
}

function htmlToText(html: string): string {
  const doc = new DOMParser().parseFromString(html, 'text/html');
  return doc.body.textContent.replace(/\s+/g, ' ').trim();
}

function truncate(value: string, max: number): string {
  if (value.length <= max) return value;
  const cut = value.slice(0, max - 1);
  const lastSpace = cut.lastIndexOf(' ');
  return `${(lastSpace > 0 ? cut.slice(0, lastSpace) : cut).trimEnd()}…`;
}

function setMetaContent(selector: string, value: string): void {
  document.querySelector<HTMLMetaElement>(selector)?.setAttribute('content', value);
}

function removeSchema(): void {
  document.getElementById(SCRIPT_ID)?.remove();
}

function applyJobMeta(job: JobViewModel): void {
  const location = parseJobLocation(job.location);
  const place = [location.locality, location.region].filter(Boolean).join(', ');
  const title = `${job.title.trim()}${place ? ` - ${place}` : ''} | Sigook`;
  const description = truncate(htmlToText(job.description) || `${job.title.trim()} job opening at Sigook.`, META_DESCRIPTION_LENGTH);

  document.title = title;
  setMetaContent('meta[name="description"]', description);
  setMetaContent('meta[property="og:title"]', title);
  setMetaContent('meta[property="og:description"]', description);
  setMetaContent('meta[name="twitter:title"]', title);
  setMetaContent('meta[name="twitter:description"]', description);
}

export function useJobPostingSchema(job: () => JobViewModel | undefined): void {
  const route = useRoute();
  watch(
    [job, () => route.fullPath],
    ([current]) => {
      removeSchema();
      if (!current) return;
      const script = document.createElement('script');
      script.id = SCRIPT_ID;
      script.type = 'application/ld+json';
      script.textContent = JSON.stringify(buildJobPosting(current)).replace(/</g, '\\u003c');
      document.head.appendChild(script);
      applyJobMeta(current);
    },
    { immediate: true, flush: 'post' },
  );

  onUnmounted(removeSchema);
}
