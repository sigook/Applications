import { describe, expect, it } from 'vitest';
import lowercase from '@/shared/format/lowercaseFilter';

describe('lowercase', () => {
  it('lower cases the whole value', () => {
    expect(lowercase('GENERAL Labour')).toBe('general labour');
  });

  it('returns an empty string for empty values', () => {
    expect(lowercase(null)).toBe('');
    expect(lowercase('')).toBe('');
  });
});
