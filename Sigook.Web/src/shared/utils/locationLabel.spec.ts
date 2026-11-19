import { describe, expect, it } from 'vitest';
import { locationLabel } from '@/shared/utils/locationLabel';

describe('locationLabel', () => {
  it('joins city and province code', () => {
    expect(locationLabel({ city: 'Toronto', provinceCode: 'ON' })).toBe('Toronto, ON');
  });

  it('returns only the city when there is no province code', () => {
    expect(locationLabel({ city: 'Toronto' })).toBe('Toronto');
    expect(locationLabel({ city: 'Toronto', provinceCode: null })).toBe('Toronto');
    expect(locationLabel({ city: 'Toronto', provinceCode: '' })).toBe('Toronto');
  });

  it('returns an empty string without a city', () => {
    expect(locationLabel({ provinceCode: 'ON' })).toBe('');
    expect(locationLabel({ city: null, provinceCode: 'ON' })).toBe('');
    expect(locationLabel(null)).toBe('');
    expect(locationLabel(undefined)).toBe('');
  });
});
