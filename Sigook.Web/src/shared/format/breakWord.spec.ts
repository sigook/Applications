import { describe, expect, it } from 'vitest';
import breakWord from '@/shared/format/breakWord';

describe('breakWord', () => {
  it('adds spaces around every pipe separator', () => {
    expect(breakWord('Forklift|WHMIS|First Aid')).toBe('Forklift | WHMIS | First Aid');
  });

  it('keeps a value without separators unchanged', () => {
    expect(breakWord('Forklift')).toBe('Forklift');
  });

  it('returns an empty string for empty values', () => {
    expect(breakWord(null)).toBe('');
    expect(breakWord('')).toBe('');
  });
});
