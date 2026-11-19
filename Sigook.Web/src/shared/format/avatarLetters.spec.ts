import { describe, expect, it } from 'vitest';
import avatarLetters from '@/shared/format/avatarLetters';

describe('avatarLetters', () => {
  it('returns the first two letters in upper case', () => {
    expect(avatarLetters('maria')).toBe('MA');
  });

  it('returns a single letter for a one character value', () => {
    expect(avatarLetters('j')).toBe('J');
  });

  it('falls back to S for empty values', () => {
    expect(avatarLetters('')).toBe('S');
    expect(avatarLetters(null)).toBe('S');
    expect(avatarLetters(undefined)).toBe('S');
  });
});
