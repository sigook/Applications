import { describe, expect, it } from 'vitest';
import currency from '@/shared/format/currencyFilter';

describe('currencyFilter', () => {
  it('formats with a dollar sign and two decimals', () => {
    const expected = (1234.5).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    expect(currency(1234.5)).toBe(`$${expected}`);
  });

  it('returns an empty string for null and zero', () => {
    expect(currency(null)).toBe('');
    expect(currency(0)).toBe('');
  });
});
