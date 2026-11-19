import { describe, expect, it } from 'vitest';
import { formatPhone } from '@/shared/utils/phoneFormat';

describe('formatPhone', () => {
  it('formats ten digits as area code, exchange and line', () => {
    expect(formatPhone('4165551234')).toBe('416 555-1234');
  });

  it('strips non digit characters before formatting', () => {
    expect(formatPhone('(416) 555-1234')).toBe('416 555-1234');
  });

  it('formats partial input progressively', () => {
    expect(formatPhone('41')).toBe('41');
    expect(formatPhone('416')).toBe('416');
    expect(formatPhone('4165')).toBe('416 5');
    expect(formatPhone('416555')).toBe('416 555');
    expect(formatPhone('4165551')).toBe('416 555-1');
  });

  it('ignores digits beyond ten', () => {
    expect(formatPhone('41655512349999')).toBe('416 555-1234');
  });

  it('returns an empty string for empty values or input without digits', () => {
    expect(formatPhone('')).toBe('');
    expect(formatPhone(null)).toBe('');
    expect(formatPhone(undefined)).toBe('');
    expect(formatPhone('abc')).toBe('');
  });
});
