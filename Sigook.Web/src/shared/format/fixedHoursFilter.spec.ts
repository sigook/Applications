import { describe, expect, it } from 'vitest';
import fixedHours from '@/shared/format/fixedHoursFilter';

describe('fixedHoursFilter', () => {
  it.each([null, 0, '0', ''])('returns 0 for empty value %s', (value) => {
    expect(fixedHours(value)).toBe(0);
  });

  it('keeps whole numbers as numbers', () => {
    expect(fixedHours(8)).toBe(8);
    expect(fixedHours('8')).toBe(8);
  });

  it('rounds fractional numbers to two decimals', () => {
    expect(fixedHours(7.3333)).toBe('7.33');
    expect(fixedHours('7.5')).toBe('7.50');
  });

  it('converts HH:mm:ss to decimal hours', () => {
    expect(fixedHours('08:00:00')).toBe(8);
    expect(fixedHours('07:30:00')).toBe('7.50');
    expect(fixedHours('01:20')).toBe('1.33');
  });

  it('returns non-numeric input unchanged', () => {
    expect(fixedHours('abc')).toBe('abc');
  });
});
