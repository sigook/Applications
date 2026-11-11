import { describe, expect, it } from 'vitest';
import {
  calendarDate,
  compactMoney,
  currentMonthRange,
  initialsOf,
  ratioOf,
  relativeTime,
  shortDate,
} from '@/modules/agency/sales/dashboard/format';

describe('compactMoney', () => {
  it.each([
    [0, '$0'],
    [999, '$999'],
    [999.4, '$999'],
    [999.6, '$1K'],
    [1000, '$1K'],
    [1500, '$1.5K'],
    [99_950, '$100K'],
    [250_000, '$250K'],
    [999_499, '$999K'],
    [999_500, '$1M'],
    [999_999, '$1M'],
    [2_500_000, '$2.5M'],
    [-999_999, '-$1M'],
    [-1500, '-$1.5K'],
    [Number.NaN, '$0'],
    [Number.POSITIVE_INFINITY, '$0'],
  ])('%d -> %s', (amount, expected) => {
    expect(compactMoney(amount)).toBe(expected);
  });
});

describe('relativeTime', () => {
  const asOf = '2026-01-15T12:00:00Z';

  it.each([
    ['2026-01-15T11:59:30Z', 'Now'],
    ['2026-01-15T11:55:00Z', '5m'],
    ['2026-01-15T09:00:00Z', '3h'],
    ['2026-01-14T06:00:00Z', 'Yest.'],
    ['2026-01-12T12:00:00Z', '3d'],
  ])('%s -> %s', (occurredAt, expected) => {
    expect(relativeTime(occurredAt, asOf)).toBe(expected);
  });

  it('falls back to a short date after a week', () => {
    expect(relativeTime('2026-01-01T12:00:00Z', asOf)).toBe(shortDate('2026-01-01T12:00:00Z'));
  });

  it('returns an empty string for invalid dates', () => {
    expect(relativeTime('nope', asOf)).toBe('');
    expect(shortDate('nope')).toBe('');
  });
});

describe('calendarDate', () => {
  it('formats as YYYY-MM-DD in local time', () => {
    expect(calendarDate(new Date(2026, 0, 5, 23, 30))).toBe('2026-01-05');
  });
});

describe('currentMonthRange', () => {
  it('returns the first and last day of the month', () => {
    const [first, last] = currentMonthRange(new Date(2026, 1, 10));
    expect(calendarDate(first)).toBe('2026-02-01');
    expect(calendarDate(last)).toBe('2026-02-28');
  });
});

describe('initialsOf', () => {
  it.each([
    ['maria de la cruz', 'MD'],
    ['  John   Smith ', 'JS'],
    ['Cher', 'C'],
    ['', ''],
  ])('%s -> %s', (name, expected) => {
    expect(initialsOf(name)).toBe(expected);
  });
});

describe('ratioOf', () => {
  it.each([
    [5, 10, 0.5],
    [20, 10, 1],
    [1, 0, 0],
  ])('%d / %d -> %d', (count, max, expected) => {
    expect(ratioOf(count, max)).toBe(expected);
  });
});
