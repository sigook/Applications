export const RATE_CATEGORIES = ['general-labor', 'skilled-labor', 'forklift', 'supervisor'] as const;

export type RateCategory = typeof RATE_CATEGORIES[number];

export interface QuoteRate {
  payRate: number;
  billRate: number;
}

export interface QuoteRow {
  row: number;
  location: string;
  state: string;
  city: string | null;
  rates: Partial<Record<RateCategory, QuoteRate>>;
}

export interface RoleRate {
  id: string;
  jobPosition: string;
  workerRate: number;
  rate: number;
}

export type ComparisonStatus = 'equal' | 'different' | 'ambiguous' | 'no-quote' | 'unparsed';

export type MatchKind = 'city' | 'fuzzy-city' | 'state';

export interface RoleComparison {
  roleId: string;
  jobPosition: string;
  status: ComparisonStatus;
  category?: RateCategory;
  state?: string;
  quoteLocation?: string;
  quoteRow?: number;
  matchedBy?: MatchKind;
  workerRate: number;
  rate: number;
  quotePayRate?: number;
  quoteBillRate?: number;
  payDifference?: number;
  billDifference?: number;
  candidates?: { location: string; payRate: number; billRate: number }[];
  reason?: string;
}

const US_STATES = new Set([
  'AL', 'AK', 'AZ', 'AR', 'CA', 'CO', 'CT', 'DE', 'DC', 'FL', 'GA', 'HI', 'ID', 'IL', 'IN', 'IA', 'KS', 'KY', 'LA', 'ME', 'MD',
  'MA', 'MI', 'MN', 'MS', 'MO', 'MT', 'NE', 'NV', 'NH', 'NJ', 'NM', 'NY', 'NC', 'ND', 'OH', 'OK', 'OR', 'PA', 'RI', 'SC', 'SD',
  'TN', 'TX', 'UT', 'VT', 'VA', 'WA', 'WV', 'WI', 'WY'
]);

const CITY_ALIASES: Record<string, string[]> = {
  la: ['los angeles'],
  nyc: ['new york', 'new york city']
};

const ROLE_WORDS = new Set([
  'general', 'labor', 'labour', 'laborer', 'labourer', 'warehouse', 'skilled', 'forklift', 'operator', 'operatot', 'supervisor', 'gl',
  'day', 'days', 'night', 'shift', 'week', 'weekend', 'regular', 'type', 'short', 'term', 'assignment', 'ii', 'iii', 'aft'
]);

const LOCATION_WITH_CITY = /^\s*([A-Z]{2})\s*[-–—�]\s*(.+?)\s*$/;
const LOCATION_STATE_ONLY = /^\s*([A-Z]{2})\s*$/;

export const normalize = (text: string): string => text
  .normalize('NFD')
  .replace(/[̀-ͯ]/g, '')
  .toLowerCase()
  .replace(/[^a-z0-9]+/g, ' ')
  .trim();

const cents = (value: number): number => Math.round(value * 100);

const difference = (current: number, quote: number): number => (cents(quote) - cents(current)) / 100;

export function parseLocation(location: string): { state: string; city: string | null } | undefined {
  const withCity = LOCATION_WITH_CITY.exec(location);
  if (withCity && US_STATES.has(withCity[1])) return { state: withCity[1], city: withCity[2] };
  const stateOnly = LOCATION_STATE_ONLY.exec(location);
  if (stateOnly && US_STATES.has(stateOnly[1])) return { state: stateOnly[1], city: null };
  return undefined;
}

export function roleCategory(jobPosition: string): RateCategory | undefined {
  const name = normalize(jobPosition);
  if (/\bsupervisor\b/.test(name)) return 'supervisor';
  if (/\bforklift\b/.test(name)) return 'forklift';
  if (/\bskilled\b/.test(name)) return 'skilled-labor';
  if (/\b(general labou?r(er)?|warehouse labou?r(er)?|gl)\b/.test(name)) return 'general-labor';
  return undefined;
}

export const roundToCents = (value: number): number => Math.round(value * 100 + 1e-6) / 100;

export function roleCity(jobPosition: string, state?: string): string | undefined {
  const tokens = normalize(jobPosition).split(' ')
    .filter(token => token !== '' && !ROLE_WORDS.has(token) && !/^\d+$/.test(token) && token !== state?.toLowerCase());
  return tokens.length > 0 ? tokens.join(' ') : undefined;
}

const cityNames = (city: string): string[] => {
  const name = normalize(city);
  return [name, ...(CITY_ALIASES[name] ?? [])];
};

export function roleState(jobPosition: string): string | undefined {
  return jobPosition.split(/[^A-Za-z]+/).find(token => token.length === 2 && token === token.toUpperCase() && US_STATES.has(token));
}

function levenshtein(a: string, b: string): number {
  const previous = Array.from({ length: b.length + 1 }, (_, index) => index);
  for (let i = 1; i <= a.length; i++) {
    let diagonal = previous[0];
    previous[0] = i;
    for (let j = 1; j <= b.length; j++) {
      const above = previous[j];
      previous[j] = Math.min(previous[j] + 1, previous[j - 1] + 1, diagonal + (a[i - 1] === b[j - 1] ? 0 : 1));
      diagonal = above;
    }
  }
  return previous[b.length];
}

function fuzzyContains(roleTokens: string[], city: string): boolean {
  const cityTokens = city.split(' ');
  const tolerance = Math.floor((city.length - 1) / 3);
  if (tolerance === 0) return false;
  for (let start = 0; start + cityTokens.length <= roleTokens.length; start++) {
    if (levenshtein(roleTokens.slice(start, start + cityTokens.length).join(' '), city) <= tolerance) return true;
  }
  return false;
}

function findCityRows(jobPosition: string, rows: QuoteRow[]): { rows: QuoteRow[]; matchedBy: MatchKind } | undefined {
  const name = ` ${normalize(jobPosition)} `;
  const withCity = rows.filter(row => row.city !== null);
  const exact = withCity
    .map(row => ({ row, length: Math.max(0, ...cityNames(row.city!).filter(city => name.includes(` ${city} `)).map(city => city.length)) }))
    .filter(match => match.length > 0);
  if (exact.length > 0) {
    const longest = Math.max(...exact.map(match => match.length));
    return { rows: exact.filter(match => match.length === longest).map(match => match.row), matchedBy: 'city' };
  }
  const tokens = name.trim().split(' ');
  const fuzzy = withCity.filter(row => fuzzyContains(tokens, normalize(row.city!)));
  return fuzzy.length > 0 ? { rows: fuzzy, matchedBy: 'fuzzy-city' } : undefined;
}

export function compareRole(role: RoleRate, quote: QuoteRow[]): RoleComparison {
  const base: RoleComparison = { roleId: role.id, jobPosition: role.jobPosition, status: 'unparsed', workerRate: role.workerRate, rate: role.rate };
  const category = roleCategory(role.jobPosition);
  if (!category) return { ...base, reason: 'No rate category (general labor, skilled labor, forklift, supervisor) in the role name' };
  const state = roleState(role.jobPosition);
  const stateRows = state ? quote.filter(row => row.state === state) : quote;
  const cityMatch = findCityRows(role.jobPosition, stateRows);
  const matchedStates = new Set(cityMatch?.rows.map(row => row.state));
  if (!state && matchedStates.size !== 1) return { ...base, category, reason: 'No US state code in the role name' };
  const resolvedState = state ?? [...matchedStates][0];
  const city = cityMatch ? undefined : roleCity(role.jobPosition, resolvedState);
  if (city) return { ...base, category, state: resolvedState, status: 'no-quote', reason: `City '${city}' is not in the ${resolvedState} rows of the quote` };
  const candidates = (cityMatch?.rows ?? stateRows).filter(row => row.rates[category]);
  const matchedBy: MatchKind = cityMatch?.matchedBy ?? 'state';
  if (candidates.length === 0) {
    return { ...base, category, state: resolvedState, status: 'no-quote', reason: cityMatch ? 'The quote row has no rate for this category' : `No quote row for ${resolvedState}` };
  }
  const distinct = new Map(candidates.map(row => [`${cents(row.rates[category]!.payRate)}|${cents(row.rates[category]!.billRate)}`, row]));
  if (distinct.size > 1) {
    return {
      ...base,
      category,
      state: resolvedState,
      status: 'ambiguous',
      matchedBy,
      candidates: candidates.map(row => ({ location: row.location, payRate: row.rates[category]!.payRate, billRate: row.rates[category]!.billRate })),
      reason: matchedBy === 'state' ? 'No quote city found in the role name and the cities of the state have different rates' : 'Several quote rows match the role name with different rates'
    };
  }
  const row = candidates[0];
  const { payRate, billRate } = row.rates[category]!;
  const equal = cents(payRate) === cents(role.workerRate) && cents(billRate) === cents(role.rate);
  return {
    ...base,
    category,
    state: resolvedState,
    status: equal ? 'equal' : 'different',
    matchedBy,
    quoteLocation: candidates.length === 1 ? row.location : `${resolvedState} (${candidates.length} cities, same rates)`,
    quoteRow: candidates.length === 1 ? row.row : undefined,
    quotePayRate: payRate,
    quoteBillRate: billRate,
    payDifference: difference(role.workerRate, payRate),
    billDifference: difference(role.rate, billRate)
  };
}

export function missingRoles(comparisons: RoleComparison[], quote: QuoteRow[]): { category: RateCategory; location: string; payRate: number; billRate: number }[] {
  const covered = new Set(comparisons.filter(item => item.quoteRow !== undefined).map(item => `${item.category}|${item.quoteRow}`));
  return quote.flatMap(row => RATE_CATEGORIES
    .filter(category => row.city !== null && row.rates[category] && !covered.has(`${category}|${row.row}`))
    .map(category => ({ category, location: row.location, ...row.rates[category]! })));
}
