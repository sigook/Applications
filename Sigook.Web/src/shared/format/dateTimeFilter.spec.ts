import { describe, expect, it } from 'vitest';
import dateTimeFilter from '@/shared/format/dateTimeFilter';

describe('dateTimeFilter', () => {
  it('formats date and 24 hour time', () => {
    expect(dateTimeFilter('2026-03-05T18:07:45')).toBe('Thu 05 March, 2026, 18:07');
  });

  it('converts a UTC timestamp to local date and time', () => {
    expect(dateTimeFilter('2026-01-10T03:15:00Z')).toBe('Fri 09 January, 2026, 22:15');
  });

  it('returns the original value when it is empty', () => {
    expect(dateTimeFilter(null)).toBeNull();
    expect(dateTimeFilter('')).toBe('');
  });
});
