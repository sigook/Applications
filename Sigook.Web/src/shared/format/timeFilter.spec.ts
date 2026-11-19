import { describe, expect, it } from 'vitest';
import timeFilter from '@/shared/format/timeFilter';

describe('timeFilter', () => {
  it('formats a local datetime as 24 hour time', () => {
    expect(timeFilter('2026-03-05T18:07:45')).toBe('18:07');
  });

  it('converts a UTC timestamp to local time', () => {
    expect(timeFilter('2026-07-01T12:00:00Z')).toBe('08:00');
  });

  it('returns an empty string for empty values', () => {
    expect(timeFilter(null)).toBe('');
    expect(timeFilter('')).toBe('');
  });
});
