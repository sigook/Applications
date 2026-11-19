import { describe, expect, it } from 'vitest';
import emailName from '@/shared/format/emailName';

describe('emailName', () => {
  it('returns the local part of an email', () => {
    expect(emailName('jane.doe@sigook.com')).toBe('jane.doe');
  });

  it('returns the value unchanged when there is no at sign', () => {
    expect(emailName('jane')).toBe('jane');
  });

  it('returns an empty string for empty values', () => {
    expect(emailName('')).toBe('');
    expect(emailName(null)).toBe('');
    expect(emailName(undefined)).toBe('');
  });
});
