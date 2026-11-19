import { describe, expect, it } from 'vitest';
import dateFilter from '@/shared/format/dateFilter';

describe('dateFilter', () => {
  it('formats an ISO string with weekday, day, month and year', () => {
    expect(dateFilter('2026-03-05T10:00:00')).toBe('Thu 05 March, 2026');
  });

  it('formats a Date instance', () => {
    expect(dateFilter(new Date(2025, 11, 31, 23, 59))).toBe('Wed 31 December, 2025');
  });

  it('uses the local day for a UTC timestamp', () => {
    expect(dateFilter('2026-01-10T03:00:00Z')).toBe('Fri 09 January, 2026');
  });

  it('returns null for empty values', () => {
    expect(dateFilter(null)).toBeNull();
    expect(dateFilter(undefined)).toBeNull();
    expect(dateFilter('')).toBeNull();
  });
});
