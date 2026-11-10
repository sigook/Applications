import { describe, expect, it } from 'vitest';
import sin from '@/shared/format/sinFilter';

describe('sinFilter', () => {
  it('masks everything except the last four digits', () => {
    expect(sin('123456789')).toBe('******6789');
  });

  it('returns an empty string for missing values', () => {
    expect(sin(null)).toBe('');
    expect(sin('')).toBe('');
  });
});
