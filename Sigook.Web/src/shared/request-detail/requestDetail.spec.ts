import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { currency } from '@/shared/format';
import {
  datesKpi,
  formatBreak,
  formatLocation,
  mapsEmbedUrl,
  mapsUrl,
  matchesLocation,
  rateKpi,
  shiftKpi,
  staffingItems,
} from '@/shared/request-detail/requestDetail';
import type { LocationDetailModel } from '@/shared/types/common';

describe('formatBreak', () => {
  it.each([
    [null, false, '—'],
    ['00:00:00', false, 'No break'],
    ['00:30:00', false, '30 min break · unpaid'],
    ['01:00:00', true, '1 h break · paid'],
    ['01:30:00', true, '90 min break · paid'],
  ])('%s paid=%s -> %s', (duration, paid, expected) => {
    expect(formatBreak(duration, paid)).toBe(expected);
  });
});

describe('rateKpi', () => {
  it('shows the hourly rate', () => {
    expect(rateKpi(20)).toEqual({ key: 'rate', label: 'Rate', value: `${currency(20)} / hr`, hint: undefined });
  });

  it('prefers the yearly salary over the rate', () => {
    const kpi = rateKpi(20, 52000);
    expect(kpi.label).toBe('Salary');
    expect(kpi.value).toBe(`${currency(52000)} / year`);
  });

  it('adds the incentive as hint', () => {
    expect(rateKpi(20, null, 2).hint).toBe(`+ ${currency(2)} incentive`);
  });

  it('shows a dash without rate or salary', () => {
    expect(rateKpi().value).toBe('—');
  });
});

describe('datesKpi', () => {
  it('shows start and finish', () => {
    expect(datesKpi('2026-03-01T08:00:00', '2026-03-15T08:00:00')).toEqual({
      key: 'dates',
      label: 'Start / Finish',
      value: '01-Mar-2026',
      hint: '→ 15-Mar-2026',
    });
  });

  it('hides the finish when asked', () => {
    const kpi = datesKpi('2026-03-01T08:00:00', '2026-03-15T08:00:00', false);
    expect(kpi.label).toBe('Start');
    expect(kpi.hint).toBeUndefined();
  });

  it('shows a dash without start', () => {
    expect(datesKpi(null).value).toBe('—');
  });
});

describe('shiftKpi', () => {
  it('shows the shift with its break as hint', () => {
    expect(shiftKpi('08:00 - 16:30', '00:30:00', true)).toEqual({
      key: 'shift',
      label: 'Shift',
      value: '08:00 - 16:30',
      hint: '30 min break · paid',
    });
  });

  it('shows a dash without shift', () => {
    expect(shiftKpi(null).value).toBe('—');
    expect(shiftKpi(null).hint).toBeUndefined();
  });
});

describe('staffingItems', () => {
  beforeEach(() => {
    vi.useFakeTimers();
    vi.setSystemTime(new Date('2026-01-15T12:00:00'));
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it('returns nothing for closed requests', () => {
    expect(staffingItems({ startAt: '2026-01-16T12:00:00', openSpots: 2, applicantsCount: 3, isOpen: false })).toEqual([]);
  });

  it('warns when a request starting this week still has open spots', () => {
    expect(staffingItems({ startAt: '2026-01-16T12:00:00', openSpots: 2, isOpen: true })).toEqual([
      { key: 'start', title: 'Starts tomorrow', detail: '2 spots still open', variant: 'is-warning' },
    ]);
  });

  it('is neutral when the start is far or fully staffed', () => {
    const [far] = staffingItems({ startAt: '2026-02-04T12:00:00', openSpots: 1, isOpen: true });
    expect(far).toEqual({ key: 'start', title: 'Starts in 20 days', detail: '1 spot still open', variant: 'is-neutral' });

    const [full] = staffingItems({ startAt: '2026-01-18T12:00:00', openSpots: 0, isOpen: true });
    expect(full).toEqual({ key: 'start', title: 'Starts in 3 days', detail: undefined, variant: 'is-neutral' });
  });

  it('warns about open spots on a request that already started', () => {
    expect(staffingItems({ startAt: '2026-01-10T08:00:00', openSpots: 1, isOpen: true })).toEqual([
      { key: 'start', title: '1 spot still open', detail: 'Already started', variant: 'is-warning' },
    ]);
  });

  it('says nothing about a started request with no open spots', () => {
    expect(staffingItems({ startAt: '2026-01-10T08:00:00', openSpots: 0, isOpen: true })).toEqual([]);
  });

  it('asks to review applicants only while there are open spots', () => {
    const [item] = staffingItems({ openSpots: 1, applicantsCount: 3, isOpen: true });
    expect(item).toMatchObject({ key: 'applicants', title: '3 applicants', actionLabel: 'Review' });

    expect(staffingItems({ openSpots: 1, applicantsCount: 1, isOpen: true })[0].title).toBe('1 applicant');
    expect(staffingItems({ openSpots: 0, applicantsCount: 3, isOpen: true })).toEqual([]);
  });

  it('shows when the next invitation can be sent', () => {
    expect(staffingItems({ openSpots: 0, invitationSentAt: '2026-01-15T09:00:00', isOpen: true })[0]).toMatchObject({
      title: 'Invitation sent today',
      detail: 'Next one available 22-Jan-2026',
    });
    expect(staffingItems({ openSpots: 0, invitationSentAt: '2026-01-14T09:00:00', isOpen: true })[0].title).toBe('Invitation sent 1 day ago');
    expect(staffingItems({ openSpots: 0, invitationSentAt: '2026-01-05T09:00:00', isOpen: true })[0]).toMatchObject({
      title: 'Invitation sent 10 days ago',
      detail: 'A new one can be sent',
    });
  });
});

describe('location helpers', () => {
  const location: LocationDetailModel = {
    address: '100 King St W',
    postalCode: 'M5X 1A9',
    city: { id: '1', value: 'Toronto', province: { id: '1', value: 'Ontario', code: 'ON' } },
    latitude: 43.6487,
    longitude: -79.3817,
  };

  it('joins the address parts', () => {
    expect(formatLocation(location)).toBe('100 King St W, Toronto, ON, M5X 1A9');
  });

  it('prefers the formatted address', () => {
    expect(formatLocation({ ...location, formattedAddress: '100 King St W, Toronto' })).toBe('100 King St W, Toronto');
  });

  it('returns an empty string without location', () => {
    expect(formatLocation(null)).toBe('');
  });

  it('builds map links only with coordinates', () => {
    expect(mapsUrl(location)).toBe('https://www.google.com/maps/search/?api=1&query=43.6487,-79.3817');
    expect(mapsEmbedUrl(location)).toContain('q=43.6487,-79.3817');
    expect(mapsUrl({ ...location, latitude: null })).toBe('');
    expect(mapsEmbedUrl({ ...location, longitude: null })).toBe('');
  });

  it.each(['king', 'TORONTO', 'on', 'm5x'])('matches %s case-insensitively', (term) => {
    expect(matchesLocation(location, term)).toBe(true);
  });

  it('does not match unrelated terms', () => {
    expect(matchesLocation(location, 'vancouver')).toBe(false);
  });
});
