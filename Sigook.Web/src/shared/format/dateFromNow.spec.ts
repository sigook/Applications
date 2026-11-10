import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import dateFromNow from '@/shared/format/dateFromNow';

describe('dateFromNow', () => {
  beforeEach(() => {
    vi.useFakeTimers();
    vi.setSystemTime(new Date('2026-01-15T12:00:00Z'));
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it('returns relative time for past dates', () => {
    expect(dateFromNow('2026-01-13T12:00:00Z')).toBe('2 days ago');
  });

  it('returns relative time for future dates', () => {
    expect(dateFromNow('2026-01-15T15:00:00Z')).toBe('in 3 hours');
  });

  it('returns an empty string for missing dates', () => {
    expect(dateFromNow(null)).toBe('');
  });
});
