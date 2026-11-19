import { describe, expect, it } from 'vitest';
import agencyTypeFilter from '@/shared/format/agencyTypeFilter';

describe('agencyTypeFilter', () => {
  it('returns the label of a known agency type', () => {
    expect(agencyTypeFilter(2)).toBe('Regular');
    expect(agencyTypeFilter(3)).toBe('Business Partner');
  });

  it('returns an empty string for an unknown type', () => {
    expect(agencyTypeFilter(1)).toBe('');
    expect(agencyTypeFilter(99)).toBe('');
  });

  it('returns an empty string for null and zero', () => {
    expect(agencyTypeFilter(null)).toBe('');
    expect(agencyTypeFilter(0)).toBe('');
  });
});
